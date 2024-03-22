using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Resources
{
    public static class PolarShadowBuilderExtensions
    {
        public static IPolarShadowBuilder AddSupported(this IPolarShadowBuilder builder)
        {
            return builder.AddItemBuilder<ISiteItemBuilder>(new SiteItemBuilder())
                          .AddItemBuilder<IParameterItemBuilder>(new ParameterItemBuilder())
                          .AddItemName<ISiteItemBuilder>(PolarShadowItems.VideoSites)
                          .AddItemName<ISiteItemBuilder>(PolarShadowItems.WebAnalysisSites)
                          .AddItemName<IParameterItemBuilder>(PolarShadowItems.Parameters)
                          .ConfigureSiteItemDefault();
                          
        }

        public static IPolarShadowBuilder ConfigureSiteItemDefault(this IPolarShadowBuilder builder)
        {
            return builder.ConfigureItem<ISiteItemBuilder>(siteItemBuilder =>
            {
                siteItemBuilder.HttpHandler = new HttpClientRequestHandler();
                siteItemBuilder.RequestRules.Add(new RequestRule("*") { Writings = new List<IContentWriting> { new BasePropertyContentWriting() } });
            });
        }
    }
}
