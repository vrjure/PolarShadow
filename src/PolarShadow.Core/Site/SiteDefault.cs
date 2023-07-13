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
        public bool UseWebView { get; set; }

        [JsonConverter(typeof(KeyValueParameterConverter))]
        public IKeyValueParameter Parameters { get; set; }
        [JsonIgnore]
        public IEnumerable<ISiteRequest> Requests => RequestsInternal?.Values;

        [JsonPropertyName("requests")]
        public Dictionary<string, SiteRequest> RequestsInternal { get; set; }


        [JsonIgnore]
        internal IRequestHandler RequestHandlerInternal { get; set; }
        [JsonIgnore]
        internal IParameterCollection ParametersInternal { get; set; }
        public ISiteRequestHandler CreateRequestHandler(string requestName)
        {
            if (RequestsInternal == null) return null;
            if (RequestHandlerInternal == null) throw new InvalidOperationException("RequestHandler not be set");
            if (RequestsInternal.TryGetValue(requestName, out SiteRequest request))
            {
                return new SiteRequestHandler(RequestHandlerInternal, request, ParametersInternal);
            }
            return null;
        }

        public void Write(Utf8JsonWriter writer)
        {
            JsonSerializer.Serialize(writer, this, JsonOption.DefaultSerializer);
        }
    }
}
