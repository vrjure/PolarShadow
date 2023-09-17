﻿using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Resources
{
    internal class SiteDefault : ISite
    {
        [JsonRequired]
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool? UseWebView { get; set; }
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
                else if (UseWebView.HasValue)
                {
                    requestHandler = UseWebView.Value ? Item._webViewHandler : requestHandler;
                }

                if (requestHandler == null) throw new InvalidOperationException("RequestHandler not be set");

                Item._writings.TryGetValue(requestName, out ICollection<IContentWriting> writings);
                if(Item._writings.TryGetValue("*", out ICollection<IContentWriting> generalWritings))
                {
                    if (writings == null)
                    {
                        writings = generalWritings;
                    }
                    else
                    {
                        writings = generalWritings.Concat(writings).ToList();
                    }
                }

                return new SiteRequestHandler(this, requestHandler, requestName, request, ParametersInternal, writings);
            }
            return null;
        }
    }
}