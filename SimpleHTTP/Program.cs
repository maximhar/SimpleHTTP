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
            server.ClientConnected += new EventHandler<ClientConnectionEventArgs>(server_ClientConnected);
            server.Start();
            Console.ReadLine();
        }

        static void server_ClientConnected(object sender, ClientConnectionEventArgs e)
        {
            Console.WriteLine("Client {0} connected", ++connectionsCounter);
            e.Response.WriteLine("HTTP/1.1 200 OK");
            e.Response.WriteLine(); // need to catch the exception
            e.Response.WriteLine("<p>This is my response. The game. {0}</p>", DateTime.Now);
        }
    }
    

}
