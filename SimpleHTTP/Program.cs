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
        static void Main(string[] args)
        {
            HTTPServer server = new HTTPServer(IPAddress.Loopback, 18080);
            server.ClientConnected += new EventHandler<ClientConnectionEventArgs>(server_ClientConnected);
            server.Start();
        }

        static void server_ClientConnected(object sender, ClientConnectionEventArgs e)
        {
            Console.WriteLine("Client connected, request: {0}", e.Request.ReadLine());
            e.Response.WriteLine("HTTP/1.1 200 OK");
            e.Response.WriteLine();
            e.Response.WriteLine("This is my response. The game.");
        }
    }
    

}
