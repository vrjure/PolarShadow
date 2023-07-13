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
        public IPolarShadowItem Build(IPolarShadowBuilder builder, ICollection<IPolarShadowProvider> providers)
        {
            var sites = new List<ISite>();

            foreach (var provider in providers)
            {
                if (!provider.TryGet(SiteItem.SitesName, out JsonElement sitesValue))
                {
                    return default;
                }

                if (sitesValue.ValueKind != JsonValueKind.Array)
                {
                    return default;
                }

                foreach (var item in sitesValue.EnumerateArray())
                {
                    var site = BuildSite(builder, builder.Parameters, item);
                    sites.Add(site);
                }
            }
            
            return new SiteItem(sites);
        }


        private ISite BuildSite(IPolarShadowBuilder builder, IParameter parameter, JsonElement siteConfig)
        {
            var site = JsonSerializer.Deserialize<SiteDefault>(siteConfig, JsonOption.DefaultSerializer);
            var p = new Parameters();
            if (parameter != null)
            {
                p.Add(parameter);
            }

            if (site.Parameters != null)
            {
                p.Add(site.Parameters);
            }
            site.ParametersInternal = p;

            if (siteConfig.TryGetProperty("useWebView", out JsonElement value)
                && (value.ValueKind == JsonValueKind.True))
            {
                site.RequestHandlerInternal = builder.WebViewHandler;
            }
            else
            {
                site.RequestHandlerInternal = builder.HttpHandler;
            }
            return site;
        }
    }
}
