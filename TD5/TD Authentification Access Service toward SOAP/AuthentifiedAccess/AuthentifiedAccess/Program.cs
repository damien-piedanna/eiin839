using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthentifiedAccess;

namespace AuthentifiedAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter username : ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter password : ");
            string password = Console.ReadLine();

            Authenticator.AuthenticatorClient authClient = new Authenticator.AuthenticatorClient();
            bool isvalid = authClient.ValidateCredentials(username, password);
            if(isvalid)
            {
                Console.WriteLine("Your are authenticated!");
                Console.WriteLine("enter a city name:");
                string city = Console.ReadLine();
                ServiceAccess.ServiceAccessClient serviceClient = new ServiceAccess.ServiceAccessClient();
                String weather = serviceClient.GetWeather(city);
                Console.WriteLine(weather);
            } 
            else
            {
                Console.WriteLine("Your are NOT authenticated!");
            }
            Console.ReadLine();     
        }
    }
}
