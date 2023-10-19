using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    internal sealed class SiteItemBuilder : ISiteItemBuilder
    {   
        public IRequestHandler WebViewHandler { get; set; }
        public IRequestHandler HttpHandler { get; set; }
        public ICollection<RequestRule> RequestRules { get; } = new List<RequestRule>();

        public IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            return new SiteItem(HttpHandler ?? new HttpClientRequestHandler(), WebViewHandler, RequestRules);
        }
    }
}
