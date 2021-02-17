using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Reflection;
using System.Diagnostics;

//url call = http://localhost:8081/MyMethods/MyMethod/?param1=Damien&param2=Allan
//url external call = http://localhost:8081/MyExternalCall/MyMethod/?param1=Damien&param2=Allan
//url webservice incr = http://localhost:8081/Webservice/Incr/?param1=5

namespace MyWebServerUrlParser
{
    class MyMethods
    {
        public string MyMethod(string[] args)
        {
            return "<HTML><BODY> Salut " + args[0] + " et " + args[1] + " (MyMethods)</BODY></HTML>";
        }
    }
    class Webservice
    {
        public string Incr(string[] args)
        {
            int val = int.Parse(args[0]);
            val++;
            return "{ \"success\":true,\"result\":" + val.ToString() + "}";
        }
    }
    class MyExternalCall
    {
        public string MyMethod(string[] args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\Nociv\source\repos\eiin839\TD2\MyExecMethod\bin\Debug\MyExecMethod.exe"; // Specify exe name.
            start.Arguments = args[0] + " " + args[1]; // Specify arguments.
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            //
            // Start the process.
            //
            using (Process process = Process.Start(start))
            {
                //
                // Read in all the text from the process with the StreamReader.
                //
                using (StreamReader reader = process.StandardOutput)
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
    internal class Program
    {
        private static void Main(string[] args)
        {

            //if HttpListener is not supported by the Framework
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }


            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            if (args.Length != 0)
            {
                foreach (string s in args)
                {
                    listener.Prefixes.Add(s);
                    // don't forget to authorize access to the TCP/IP addresses localhost:xxxx and localhost:yyyy 
                    // with netsh http add urlacl url=http://localhost:xxxx/ user="Tout le monde"
                    // and netsh http add urlacl url=http://localhost:yyyy/ user="Tout le monde"
                    // user="Tout le monde" is language dependent, use user=Everyone in english 

                }
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();

            // get args 
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };


            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }

                // get url 
                Console.WriteLine($"Received request for {request.Url}");

                //get url protocol
                Console.WriteLine(request.Url.Scheme);
                //get user in url
                Console.WriteLine(request.Url.UserInfo);
                //get host in url
                Console.WriteLine(request.Url.Host);
                //get port in url
                Console.WriteLine(request.Url.Port);
                //get path in url 
                Console.WriteLine(request.Url.LocalPath);

                // parse path in url 
                string[] segments = request.Url.Segments;

                //get params un url. After ? and between &

                Console.WriteLine(request.Url.Query);

                //parse params in url
                Console.WriteLine("param1 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param1"));
                Console.WriteLine("param2 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param2"));
                Console.WriteLine("param3 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param3"));
                Console.WriteLine("param4 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param4"));

                //
                Console.WriteLine(documentContents);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                string responseString = "Hi !";
                if (segments.Length == 3)
                {
                    // Construct a response.
                    Type type = Type.GetType("MyWebServerUrlParser." + segments[1].TrimEnd('/'));
                    MethodInfo method = type.GetMethod(segments[2].TrimEnd('/'));
                    object o = Activator.CreateInstance(type);


                    String[] param = { HttpUtility.ParseQueryString(request.Url.Query).Get("param1"), HttpUtility.ParseQueryString(request.Url.Query).Get("param2") };
                    object[] tab = { param };

                    responseString = (string)method.Invoke(o, tab);
                }


                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                if(segments[1].TrimEnd('/') == "Webservice") response.ContentType = "application/json";
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.

                output.Close();
            }
            // Httplistener neither stop ... But Ctrl-C do that ...
            // listener.Stop();
        }
    }
}