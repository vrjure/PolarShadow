using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Aria2RPC
{
    public class Aria2Request
    {
        public Aria2Request(string id, string method) : this(id, method, "2.0")
        {

        }

        public Aria2Request(string id, string method, string jsonRpc)
        {
            this.Id = id;
            this.Method = method;
            this.Jsonrpc = jsonRpc;
        }

        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public string Method { get; set; }
        public ICollection<string> Params { get; set; }
    }
}
