using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleHTTP
{
    public class HttpInfo
    {
        public Dictionary<string, string> Headers { get; private set; }
        public MethodType Method { get; private set; }
        public string Uri { get; set; }
        public HttpInfo(Dictionary<string, string> headers, MethodType method, string uri)
        {
            this.Headers = headers;
            this.Method = method;
            this.Uri = uri;
        }
    }
}
