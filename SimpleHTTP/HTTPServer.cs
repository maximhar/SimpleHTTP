using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace SimpleHTTP
{
    class HTTPServer
    {
        IPEndPoint endpoint;
        Socket serverSocket;
        StreamWriter writer;

        readonly string terminator = new string(new char[] { (char)13, (char)10, (char)13, (char)10 });

        public HTTPServer(IPAddress bindAddress, int bindPort)
        {
            endpoint = new IPEndPoint(bindAddress, bindPort);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        void StartClient(object parameter)
        {
            if (!(parameter is Tuple<TcpClient>)) return;
            var tcpClient = (parameter as Tuple<TcpClient>).Item1;
            var stream = tcpClient.GetStream();
            byte[] message = new byte[4096];
            int bytesRead;
            var request = new StringBuilder();
            while (true)
            {

                bytesRead = 0;

                try
                {
                    Console.WriteLine("Attempting to read...", bytesRead);
                    bytesRead = stream.Read(message, 0, 4096);
                    Console.WriteLine("{0} bytes read", bytesRead);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0) break;
                
                ASCIIEncoding encoder = new ASCIIEncoding();
                string read = encoder.GetString(message, 0, bytesRead);
                request.Append(read);
                if (read.EndsWith(terminator)) break;
            }
            using (var writer = new StreamWriter(stream))
            {
                writer.AutoFlush = true;
                OnClientHandled(request.ToString(), writer);
            }
            Console.WriteLine("Closing TCP stream and client.");
            stream.Close();
            tcpClient.Close();
        }
        public void Start()
        {//wait 
            TcpListener listener = new TcpListener(endpoint);
            listener.Start();
            
            while (true)
            {
                var tcpClient = listener.AcceptTcpClient();
                Console.WriteLine("TCP connection accepted.");
                Thread thread = new Thread(StartClient);
                thread.Start(new Tuple<TcpClient>(tcpClient));
            }
            
        }


        private void OnClientHandled(string request, StreamWriter writer)
        {
            if (ClientConnected != null)
                ClientConnected(this, new ClientConnectionEventArgs(request, writer));
        }

        public event EventHandler<ClientConnectionEventArgs> ClientConnected;
    }
}
