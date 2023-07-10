using PolarShadow.Core.Site;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal sealed class SiteItemBuilder : ISiteItemBuilder
    {
        private readonly static string _sitesSection = "sites";
        private readonly JsonElement _sitesConfig;
        public SiteItemBuilder(JsonDocument doc)
        {
            if(!doc.RootElement.TryGetProperty(_sitesSection, out JsonElement siteProperty))
            {
                return;
            }
            _sitesConfig = siteProperty.Clone();
        }

        public ISiteBuilder SiteBuilder { get; set; }

        public IParameter Parameter { get; set; }

        public IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            var sites = new List<ISite>();
            var parameterItem = builder.ItemBuilders.Where(f => f is IParameterItemBuilder).FirstOrDefault();
            if (parameterItem != null)
            {
                Parameter = (IParameter)parameterItem.Build(builder);
            }
            var siteBuilder = SiteBuilder ?? new SiteBuilder();
            if(_sitesConfig.ValueKind != JsonValueKind.Array)
            {
                return new SiteItem(sites);
            }

            foreach (var item in _sitesConfig.EnumerateArray())
            {
                var site = siteBuilder.Build(builder, this, item);
                sites.Add(site);
            }

            return new SiteItem(sites);
        }

    }
}
