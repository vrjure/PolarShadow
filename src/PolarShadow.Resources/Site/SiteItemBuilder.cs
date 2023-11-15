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
    internal sealed class SiteItemBuilder : SiteItemBuilderBase, ISiteItemBuilder
    {   
        public override IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            return new SiteItem(HttpHandler ?? new HttpClientRequestHandler(), WebViewHandler, RequestRules);
        }
    }
}
