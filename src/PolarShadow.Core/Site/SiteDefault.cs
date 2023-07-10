using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class SiteDefault : ISite
    {
        [JsonIgnore]
        public ISiteRequest this[string requestName]
        {
            get
            {
                if (string.IsNullOrEmpty(requestName) 
                    || RequestsInternal == null 
                    || !RequestsInternal.ContainsKey(requestName)) 
                    return null;

                return RequestsInternal[requestName];
            }
        }

        public string Name { get; set; }

        public string Domain { get; set; }

        [JsonConverter(typeof(KeyValueParameterConverter))]
        public IParameter Parameters { get; set; }
        [JsonPropertyName("requests")]
        public Dictionary<string, SiteRequest> RequestsInternal { get; set; }

        [JsonIgnore]
        public IEnumerable<ISiteRequest> Requests => RequestsInternal?.Values;
        [JsonIgnore]
        internal IRequestHandler RequestHandler { get; set; }
        public ISiteRequestHandler CreateRequestHandler(string requestName)
        {
            if (RequestsInternal == null) return null;
            if (RequestHandler == null) throw new InvalidOperationException("RequestHandler not be set");
            if (RequestsInternal.TryGetValue(requestName, out SiteRequest request))
            {
                return new SiteRequestHandler(RequestHandler, request, Parameters);
            }
            return null;
        }
    }
}
