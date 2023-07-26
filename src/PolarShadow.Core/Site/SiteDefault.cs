﻿using System;
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
        internal SiteItem Item { get; set; }
        [JsonIgnore]
        internal IParameter ParametersInternal { get; set; }

        public ISiteRequestHandler CreateRequestHandler(string requestName)
        {
            if (Requests == null) return null;

            var requestHandler = Item._httpHandler;
            if (Requests.TryGetValue(requestName, out ISiteRequest request))
            {
                if (request.UseWebView.HasValue)
                {
                    requestHandler = request.UseWebView.Value ? Item._webViewHandler : requestHandler;
                }
                else if (UseWebView)
                {
                    requestHandler = Item._webViewHandler;
                }

                if (requestHandler == null) throw new InvalidOperationException("RequestHandler not be set");

                Item._requestsBuilders.TryGetValue(requestName, out IContentBuilder requestBuilder);
                Item._responseBuilders.TryGetValue(requestName, out IContentBuilder responseBuilder);

                return new SiteRequestHandler(this, requestHandler, request, Parameters, requestBuilder ?? SiteItem._requestBuilder, responseBuilder ?? SiteItem._responseBulder);
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
