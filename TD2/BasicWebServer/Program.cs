using System;
using System.IO;
using System.Net;
using System.Text;

namespace BasicWebServerUrlParser
{
    internal class Header
    {
        WebHeaderCollection webHeaderCollection;
        private string accept;
        private string acceptCharset;
        private string acceptEncoding;
        private string acceptLanguage;
        private string allow;
        private string authorization;
        private string cookie;
        private string from;
        private string userAgent;
        public Header(WebHeaderCollection headerCollection)
        {
            this.webHeaderCollection = headerCollection;
            this.accept = this.webHeaderCollection.Get("Accept");
            this.acceptCharset = this.webHeaderCollection.Get("Accept-Charset");
            this.acceptEncoding = this.webHeaderCollection.Get("Accept-Encoding");
            this.acceptLanguage = this.webHeaderCollection.Get("Accept-Language");
            this.allow = this.webHeaderCollection.Get("Allow");
            this.authorization = this.webHeaderCollection.Get("Authorization");
            this.cookie = this.webHeaderCollection.Get("Cookie");
            this.from = this.webHeaderCollection.Get("From");
            this.userAgent = this.webHeaderCollection.Get("User-Agent");
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append("== Headers ==\n");
            if (this.accept != null) sb.Append("Accept : " + this.accept + "\n");
            if (this.acceptCharset != null) sb.Append("Accept Charset : " + this.acceptCharset + "\n");
            if (this.acceptEncoding != null) sb.Append("Accept Encoding : " + this.acceptEncoding + "\n");
            if (this.acceptLanguage != null) sb.Append("Accept Language : " + this.acceptLanguage + "\n");
            if (this.allow != null) sb.Append("Allow : " + this.allow + "\n");
            if (this.authorization != null) sb.Append("Authorization : " + this.authorization + "\n");
            if (this.cookie != null) sb.Append("Cookie : " + this.cookie + "\n");
            if (this.from != null) sb.Append("From : " + this.from + "\n");
            if (this.userAgent != null) sb.Append("User Agent : " + this.userAgent + "\n");
            sb.Append("=============\n");
            return sb.ToString();
        }
    }
    internal class Program
    {
        private static void Main(string[] args)
        {


            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }

            // Create a listener.
            HttpListener listener = new HttpListener();

            // Trap Ctrl-C and exit 
            Console.CancelKeyPress += delegate
            {
                listener.Stop();
                System.Environment.Exit(0);
            };

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
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                Header header = new Header((WebHeaderCollection)request.Headers);
                Console.WriteLine(header);


                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }
                Console.WriteLine($"Received request for {request.Url}");
                Console.WriteLine(documentContents);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;

                // Construct a response.
                string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
            // Httplistener neither stop ...
            // listener.Stop();
        }
    }
}