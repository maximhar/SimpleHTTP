using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SimpleHTTP
{
    class ClientConnectionEventArgs:EventArgs
    {
        public StreamWriter Response { get; private set; }
        public string Request { get; private set; }
        public HttpInfo HeaderData { get; private set; }
        public char[] PostData { get; private set; }
        public ClientConnectionEventArgs(string request, StreamWriter response, HttpInfo headerData, char[] postData=null)
        {
            Request = request;
            Response = response;
            HeaderData = headerData;
            PostData = postData;
        }
    }
}
