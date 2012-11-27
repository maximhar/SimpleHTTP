using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleHTTP
{
    class HttpServerException : Exception
    {
        public HttpServerException() : base() { }
        public HttpServerException(string message) : base(message) { }
    }
}
