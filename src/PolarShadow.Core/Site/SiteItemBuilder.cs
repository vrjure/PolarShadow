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
        public IDictionary<string, IContentBuilder> RequestBuilders { get; set; }
        public IDictionary<string, IContentBuilder> ResponseBuilders { get; set; }

        public IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            return new SiteItem(HttpHandler ?? new HttpClientRequestHandler(), WebViewHandler, builder.Parameters, RequestBuilders, ResponseBuilders);
        }
    }
}
