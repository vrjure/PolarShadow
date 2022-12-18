using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Core.Aria2
{
    public class Aria2Request
    {
        private List<object> _params = new List<object>();

        internal Aria2Request() : this(default)
        {

        }

        internal Aria2Request(string secret) :this("aria2net", secret)
        {

        }

        internal Aria2Request(string id, string secret): this(id, "", secret)
        {

        }

        internal Aria2Request(string id, string method, string secret) : this(id, method, "2.0", secret)
        {

        }

        internal Aria2Request(string id, string method, string jsonRpc, string secret)
        {
            this.Id = id;
            this.Method = method;
            this.Jsonrpc = jsonRpc;
            this.Secret = secret;
            if (!string.IsNullOrEmpty(secret))
            {
                _params.Add(secret);
            }
        }

        public string Jsonrpc { get; internal set; }
        public string Id { get; internal set; }
        public string Method { get; internal set; }
        public IReadOnlyCollection<object> Params
        {
            get => _params;
            protected set => _params = (List<object>)value;
        }

        internal string Secret { get; set; }

        public void AddParams(params object[] parameters)
        {
            foreach (var item in parameters)
            {
                if (item == null)
                {
                    continue;
                }
                _params.Add(item);
            }
        }

        internal void ClearParams()
        {
            _params.Clear();
        }
    }

    public class Aria2Request<TResult> : Aria2Request
    {
        internal Aria2Request(string id, string secret) : base(id, "", secret)
        {

        }

        internal Aria2Request(IAria2Client client) : this(client.Id, client.Secret)
        {
            
        }
    }


}
