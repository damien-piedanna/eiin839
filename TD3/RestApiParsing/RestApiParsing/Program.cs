using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

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

                Console.WriteLine("Station list:\n");
                foreach (var station in stations)
                {
                    Console.WriteLine(station);
                    Console.WriteLine("---------");
                }
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
