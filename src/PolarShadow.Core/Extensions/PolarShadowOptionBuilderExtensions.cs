using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PolarShadow.Core
{
    internal static class PolarShadowOptionBuilderExtensions
    {
        private static readonly string _sitesName = "sites";

        public static IPolarShadowOptionBuilder BuildSite(this IPolarShadowOptionBuilder optionBuilder, string siteName, Action<IPolarShadowSiteOptionBuilder> siteOptionBuilder)
        {
            if (!optionBuilder.TryGetOption(_sitesName, out KeyNameCollection<PolarShadowSiteOption> sites))
            {
                sites = new KeyNameCollection<PolarShadowSiteOption>();
            }

            if (!sites.TryGetValue(siteName, out PolarShadowSiteOption option))
            {
                option = new PolarShadowSiteOption(siteName);
                sites.Add(option);
            }

            siteOptionBuilder(new PolarShadowSiteOptionBuilder(option, optionBuilder));
            optionBuilder.RemoveOption(_sitesName);
            optionBuilder.AddOptions<PolarShadowSiteOption>(_sitesName, sites);

            return optionBuilder;
        }

        internal static ICollection<PolarShadowSiteOption> GetSites(this IPolarShadowOptionBuilder builder)
        {
            return builder.GetOption<KeyNameCollection<PolarShadowSiteOption>>(_sitesName);
        }
    }
}
