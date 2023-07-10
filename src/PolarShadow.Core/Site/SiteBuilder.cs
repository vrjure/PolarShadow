using PolarShadow.Core.Site;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class SiteBuilder : ISiteBuilder
    {
        public ISite Build(IPolarShadowBuilder builder, ISiteItemBuilder itemBuilder, JsonElement siteConfig)
        {
            var site = JsonSerializer.Deserialize<SiteDefault>(siteConfig, JsonOption.DefaultSerializer);
            var p = new Parameters();
            if (itemBuilder.Parameter != null)
            {
                p.Add(itemBuilder.Parameter);
            }

            if (site.Parameters != null)
            {
                p.Add(site.Parameters);
            }
            site.Parameters = p;

            if(siteConfig.TryGetProperty("useWebView", out JsonElement value) 
                && (value.ValueKind == JsonValueKind.True))
            {
                site.RequestHandler = builder.WebViewHandler;
            }
            else
            {
                site.RequestHandler = builder.HttpHandler;
            }
            return site;
        }
    }
}
