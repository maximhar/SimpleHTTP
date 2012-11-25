using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SimpleHTTP
{
    class ClientConnectionEventArgs:EventArgs
    {
        public StreamWriter Response { get; set; }
        public string Request { get; set; }
        public ClientConnectionEventArgs(string request, StreamWriter response)
        {
            Request = request;
            Response = response;
        }
    }
}
