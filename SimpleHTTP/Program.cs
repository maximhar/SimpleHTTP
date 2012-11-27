using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace SimpleHTTP
{
    class Program
    {
        public static int connectionsCounter = 0;
        static void Main(string[] args)
        {
            HTTPServer server = new HTTPServer(IPAddress.Loopback, 18080);
            server.OnGet += new EventHandler<ClientConnectionEventArgs>(server_OnGet);
            server.OnPost += new EventHandler<ClientConnectionEventArgs>(server_OnPost);
            server.Start();
            Console.ReadLine();
        }

        static void server_OnPost(object sender, ClientConnectionEventArgs e)
        {
            Console.WriteLine("POST request", ++connectionsCounter);
            e.Response.WriteLine("HTTP/1.1 200 OK");
            e.Response.WriteLine();
            foreach (var header in e.HeaderData.Headers)
            {
                e.Response.WriteLine("{0}:{1}", header.Key, header.Value);
            }
            e.Response.WriteLine("Post data: ");
            e.Response.Write(new string(e.PostData));
        }

        static void server_OnGet(object sender, ClientConnectionEventArgs e)
        {
            Console.WriteLine("GET request", ++connectionsCounter);
            e.Response.WriteLine("HTTP/1.1 200 OK");
            e.Response.WriteLine();
            e.Response.Write(File.ReadAllText("index.html"));
        }
        
    }
    

}
