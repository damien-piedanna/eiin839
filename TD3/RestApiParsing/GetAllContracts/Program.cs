using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

namespace GetAllContracts
{
    public class Contract
    {
        public string name { get; set; }
        public string commercial_name { get; set; }
        public List<string> cities { get; set; }
        public string country_code { get; set; }

        public override string ToString()
        {
            return " name : " + name + "\n"
                 + " commercial_name : " + commercial_name + "\n"
                 + " cities : " + name + "\n"
                 + " country_code : " + country_code + "\n";
        }

    }
    class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                string responseBody = await client.GetStringAsync("https://api.jcdecaux.com/vls/v3/contracts?apiKey=a0dbd529c92077bf289fd7bee14fd1b6f0002eae");
                List<Contract> contracts = JsonSerializer.Deserialize<List<Contract>>(responseBody);

                Console.WriteLine("Contract list:\n");
                foreach (var contract in contracts)
                {
                    Console.WriteLine(contract);
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
