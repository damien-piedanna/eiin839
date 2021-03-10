let stations;

function retrieveAllContracts() {
    var targetUrl = "https://api.jcdecaux.com/vls/v3/contracts?apiKey=" + document.getElementById("apiKey").value;
    var requestType = "GET";

    var caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    // The header set below limits the elements we are OK to retrieve from the server.
    caller.setRequestHeader ("Accept", "application/json");
    // onload shall contain the function that will be called when the call is finished.
    caller.onload=contractsRetrieved;

    caller.send();
}

function contractsRetrieved() {
    // Let's parse the response:
    let response = JSON.parse(this.responseText);
    response.forEach(contract => {
        let option = document.createElement("option");
        option.append(contract.name);
        document.getElementById('contracts').append(option)
    });
}

function retrieveContractStations() {
    var targetUrl = "https://api.jcdecaux.com/vls/v3/stations?contract=" + document.getElementById("contractChoice").value + "&apiKey=" + document.getElementById("apiKey").value;
    var requestType = "GET";

    var caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    // The header set below limits the elements we are OK to retrieve from the server.
    caller.setRequestHeader("Accept", "application/json");
    // onload shall contain the function that will be called when the call is finished.
    caller.onload = contractStationRetrieved;

    caller.send();
}

function contractStationRetrieved() {
    let response = JSON.parse(this.responseText);
    console.log(response);
    stations = response;
}

function getClosestStation() {
    retrieveContractStations();

    let latitude = document.getElementById("latitudeChoice").value;
    let longitude = document.getElementById("longitudeChoice").value;
    let bestStation = null;
    let bestDistance = null;

    stations.forEach(station => {
        let distance = getDistanceFrom2GpsCoordinates(station.position.latitude, station.position.longitude, latitude, longitude);
        if (bestDistance == null || distance < bestDistance) {
            bestStation = station;
            bestDistance = distance;
        }
    });
    console.log(bestStation.name + " (" + bestDistance + ")");
}

function getDistanceFrom2GpsCoordinates(lat1, lon1, lat2, lon2) {
    // Radius of the earth in km
    var earthRadius = 6371;
    var dLat = deg2rad(lat2 - lat1);
    var dLon = deg2rad(lon2 - lon1);
    var a =
        Math.sin(dLat / 2) * Math.sin(dLat / 2) +
        Math.cos(deg2rad(lat1)) * Math.cos(deg2rad(lat2)) *
        Math.sin(dLon / 2) * Math.sin(dLon / 2)
        ;
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = earthRadius * c; // Distance in km
    return d;
}

function deg2rad(deg) {
    return deg * (Math.PI / 180)
}