using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Device.Location;

namespace RestApiParsing
{
    public class Station
    {
        public int number { get; set; }
        public string contractName { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public Position position { get; set; }

        public override string ToString()
        {
            return " number : " + number + "\n"
                 + " contractName : " + contractName + "\n"
                 + " name : " + name + "\n"
                 + " address : " + address + "\n"
                 + " position : [" + position + "]\n";
        }

    }
    public class Position
    {
        public float latitude { get; set; }
        public float longitude { get; set; }

        public override string ToString()
        {
            return "latitude : " + latitude + " longitude : " + longitude;
        }

    }
    class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                string responseBody = await client.GetStringAsync("https://api.jcdecaux.com/vls/v3/stations?apiKey=a0dbd529c92077bf289fd7bee14fd1b6f0002eae&contract=" + args[0]);
                List<Station> stations = JsonSerializer.Deserialize<List<Station>>(responseBody);

                GeoCoordinate coord = new GeoCoordinate(Convert.ToDouble(args[1]), Convert.ToDouble(args[2]));

                Station nearestStation = null;
                double bestDistance = 0;

                foreach (var station in stations)
                {
                    if (nearestStation == null) 
                    {
                        nearestStation = station;
                        bestDistance = coord.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude));
                    }
                    else if (coord.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude)) < bestDistance)
                    {
                        nearestStation = station;
                        bestDistance = coord.GetDistanceTo(new GeoCoordinate(station.position.latitude, station.position.longitude));

                    }
                }

                Console.WriteLine("Nearest station (" + Convert.ToInt32(bestDistance) + "m):\n" + nearestStation);

               
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            Console.Read();
        }
    }
}
