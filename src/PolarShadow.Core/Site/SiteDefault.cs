using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    internal class SiteDefault : ISite
    {
        [JsonRequired]
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool UseWebView { get; set; }
        public IKeyValueParameter Parameters { get; set; }
        public IDictionary<string, ISiteRequest> Requests { get; set; }

        [JsonIgnore]
        internal IRequestHandler HttpRequestHandlerInternal { get; set; }
        [JsonIgnore]
        internal IRequestHandler WebViewRequestHandlerInteral { get; set; }
        [JsonIgnore]
        internal IParameterCollection ParametersInternal { get; set; }

        public ISiteRequestHandler CreateRequestHandler(string requestName)
        {
            if (Requests == null) return null;

            var requestHandler = HttpRequestHandlerInternal;
            if (Requests.TryGetValue(requestName, out ISiteRequest request))
            {
                if (request.UseWebView.HasValue)
                {
                    requestHandler = request.UseWebView.Value ? WebViewRequestHandlerInteral : requestHandler;
                }
                else if (UseWebView)
                {
                    requestHandler = WebViewRequestHandlerInteral;
                }

                if (requestHandler == null) throw new InvalidOperationException("RequestHandler not be set");
                return new SiteRequestHandler(requestHandler, request, ParametersInternal);
            }
            return null;
        }

        public void LoadFrom(IPolarShadowSource source)
        {
            var provider = source.Build(null);
            if (provider != null || provider.Root.ValueKind == JsonValueKind.Undefined) return;

            var site = JsonSerializer.Deserialize<SiteDefault>(provider.Root, JsonOption.DefaultSerializer);
            Apply(site);

        }

        private void Apply(SiteDefault site)
        {
            this.Name = site.Name;
            this.Domain = site.Domain;
            this.UseWebView = site.UseWebView;
            this.Parameters = site.Parameters;
            this.Requests = site.Requests;
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            JsonSerializer.Serialize(writer, this, JsonOption.DefaultSerializer);
        }
    }
}
