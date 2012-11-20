using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace SimpleHTTP
{
    class HTTPServer
    {
        IPEndPoint endpoint;
        Socket serverSocket;
        NetworkStream stream;
        StreamWriter writer;
        StreamReader reader;
        public HTTPServer(IPAddress bindAddress, int bindPort)
        {
            endpoint = new IPEndPoint(bindAddress, bindPort);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Start()
        {
            serverSocket.Bind(endpoint);
            Socket clientSocket;
            serverSocket.Listen(10);
            while (true)
            {
                clientSocket = serverSocket.Accept();
                Stopwatch s = new Stopwatch();
                if (clientSocket.Connected)
                {
                    s.Start();
                    using (stream = new NetworkStream(clientSocket))
                    using (writer = new StreamWriter(stream))
                    using (reader = new StreamReader(stream))
                    {
                        if (IsHttpGet())
                            OnClientHandled(reader, writer);
                        writer.Flush();
                        reader.Close();
                        writer.Close();
                        stream.Close();
                    }
                }
                clientSocket.Close();
                s.Stop();
                Console.WriteLine("Time elapsed: {0}ms.", s.ElapsedMilliseconds);
            }
        }

        private bool IsHttpGet()
        {
            var firstLine = reader.ReadLine();
            if (firstLine == null) return false;
            return firstLine.Contains("GET") && firstLine.Contains("HTTP");
        }

        private void OnClientHandled(StreamReader reader, StreamWriter writer)
        {
            if (ClientConnected != null)
                ClientConnected(this, new ClientConnectionEventArgs(reader, writer));
        }

        public event EventHandler<ClientConnectionEventArgs> ClientConnected;
    }
}
