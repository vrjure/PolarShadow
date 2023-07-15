using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal sealed class SiteItemBuilder : IPolarShadowItemBuilder
    {
        public IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            return new SiteItem(builder.HttpHandler, builder.WebViewHandler, builder.Parameters);
        }
    }
}
