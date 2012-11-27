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

        readonly string terminator = new string(new char[] { (char)13, (char)10, (char)13, (char)10 });

        public HTTPServer(IPAddress bindAddress, int bindPort)
        {
            endpoint = new IPEndPoint(bindAddress, bindPort);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
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

        void StartClient(object parameter)
        {
            if (!(parameter is Tuple<TcpClient>)) return;
            try
            {
                var tcpClient = (parameter as Tuple<TcpClient>).Item1;
                var stream = tcpClient.GetStream();
                var reader = new StreamReader(stream);
                string line;

                var request = new StringBuilder();

                while ((line=reader.ReadLine())!=null)
                {
                    if (line == string.Empty)
                        break;
                    request.AppendLine(line);
                }
            
                var headerData = ParseHeaders(request.ToString());

           
                Console.WriteLine("HTTP request detected");
                using (var writer = new StreamWriter(stream))
                {
                    if (headerData.Method == MethodType.Get) 
                        OnClientGetHandled(request.ToString(), writer, headerData);
                    else if (headerData.Method == MethodType.Post)
                    {
                        if (!headerData.Headers.ContainsKey("content-length"))
                            throw new HttpServerException("POST request does not contain content-length");
                        char[] postData = ReadPostData(int.Parse(headerData.Headers["content-length"]), reader);
                        OnClientPostHandled(request.ToString(), writer, headerData, postData);
                    }
                    writer.Flush();
                }
                CloseTcpClient(tcpClient, stream);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception generated while handling client: {0}", e.Message);
            }
            
        }

        private char[] ReadPostData(int length, StreamReader stream)
        {
            Console.WriteLine("Trying to read {0} bytes...", length);
            char[] buffer = new char[length];
            int realLength = stream.ReadBlock(buffer, 0, length);
            if (realLength != length)
            {
                throw new HttpServerException(string.Format("Server expected a length of {0}, instead got {1}.", length, realLength));
            }
            return buffer;
        }

        private HttpInfo ParseHeaders(string headerString)
        {
            var stream = new StringReader(headerString);
            string line;

            string[] topLine = (stream.ReadLine()??string.Empty).Split(' ');
            if (topLine.Length != 3) throw new HttpServerException("HTTP header not valid.");
            string methodString = topLine[0];
            MethodType method;
            string uri = topLine[1];
            Dictionary<string, string> headers = new Dictionary<string,string>();
            switch (methodString.ToUpper())
            {
                case "POST":
                    method = MethodType.Post;
                    break;
                case "GET":
                    method = MethodType.Get;
                    break;
                default:
                    throw new HttpServerException(string.Format("Invalid method {0}.", methodString));
            }
            

            while ((line = stream.ReadLine()) != null)
            {
                string[] pair = line.Split(':');
                headers.Add(pair[0].ToLower(), pair[1].TrimStart());
            }
            return new HttpInfo(headers, method, uri);
        }

        

        private bool IsHttpRequest(string request)
        {
            var stringStream = new StringReader(request);
            return (stringStream.ReadLine() ?? string.Empty).Contains("HTTP");
        }
        private static string GetEncodedString(byte[] message, int bytesRead)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            return encoder.GetString(message, 0, bytesRead);
        }

        private static void CloseTcpClient(TcpClient tcpClient, NetworkStream stream)
        {
            Console.WriteLine("Closing TCP stream and client.");
            stream.Close();
            tcpClient.Close();
        }

        private static int ReadStream(NetworkStream stream, byte[] message, int bytesRead)
        {
            Console.WriteLine("Attempting to read...", bytesRead);
            bytesRead = stream.Read(message, 0, 4096);
            Console.WriteLine("{0} bytes read", bytesRead);
            return bytesRead;//watcha thinkin?
        }
 
        private void OnClientGetHandled(string request, StreamWriter writer, HttpInfo headerData)
        {
            if (OnGet != null)
                OnGet(this, new ClientConnectionEventArgs(request, writer, headerData));
        }
        private void OnClientPostHandled(string request, StreamWriter writer, HttpInfo headerData, char[] postData)
        {
            if (OnPost != null)
                OnPost(this, new ClientConnectionEventArgs(request, writer, headerData, postData));
        }
        public event EventHandler<ClientConnectionEventArgs> OnGet;
        public event EventHandler<ClientConnectionEventArgs> OnPost;
    }
}
