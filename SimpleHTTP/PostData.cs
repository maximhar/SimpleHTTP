using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleHTTP
{
    public class PostData
    {
        public Dictionary<string, byte[]> Data { get; private set; }
        public PostData(Dictionary<string, byte[]> data)
        {
            this.Data = data;
        }
    }
}
