using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal sealed class SiteItemBuilder : ISiteItemBuilder
    {   
        public IRequestHandler WebViewHandler { get; set; }
        public IRequestHandler HttpHandler { get; set; }

        public ICollection<IContentBuilder> RequestBuilders { get; } = new List<IContentBuilder>();

        public ICollection<IContentBuilder> ResponseBuilders { get; } = new List<IContentBuilder>();

        public IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            var requestsDict = new Dictionary<string, IContentBuilder>();
            foreach (var b in RequestBuilders)
            {
                if (b.RequestFilter == null)
                {
                    continue;
                }

                foreach (var item in b.RequestFilter)
                {
                    requestsDict.Add(item, b);
                }
            }

            var responseDict = new Dictionary<string, IContentBuilder>();
            foreach (var b in ResponseBuilders)
            {
                if (b.RequestFilter == null)
                {
                    continue;
                }

                foreach (var item in b.RequestFilter)
                {
                    responseDict.Add(item, b);
                }
            }
            return new SiteItem(HttpHandler ?? new HttpClientRequestHandler(), WebViewHandler, builder.Parameters, requestsDict, responseDict);
        }
    }
}
