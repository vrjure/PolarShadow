using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Resources
{
    public static class PolarShadowBuilderExtensions
    {
        public static IPolarShadowBuilder ConfigureAllSupported(this IPolarShadowBuilder builder)
        {
            return builder.ConfigureSiteDefault()
                .AddWebAnalysisItem();
        }

        public static IPolarShadowBuilder ConfigureSiteDefault(this IPolarShadowBuilder builder)
        {
            return builder.ConfigureSiteItem(siteItemBuilder =>
            {
                siteItemBuilder.HttpHandler = new HttpClientRequestHandler();
                siteItemBuilder.Writings.Add(new BasePropertyContentWriting());
            }).ConfigureSiteJsonOption();
        }

        public static IPolarShadowBuilder ConfigureSiteItem(this IPolarShadowBuilder builder, Action<ISiteItemBuilder> itemBuilder)
        {
            if (!builder.TryGetItemBuilder(out ISiteItemBuilder siteItemBuilder))
            {
                siteItemBuilder = builder.AddSiteItem();
            }
            itemBuilder(siteItemBuilder);
            return builder;
        }

        public static ISiteItemBuilder AddSiteItem(this IPolarShadowBuilder builder)
        {
            if (builder.TryGetItemBuilder(out ISiteItemBuilder siteItemBuilder))
            {
                return siteItemBuilder;
            }

            siteItemBuilder = new SiteItemBuilder();
            builder.Add(siteItemBuilder);
            return siteItemBuilder;
        }

        public static IPolarShadowBuilder ConfigureSiteJsonOption(this IPolarShadowBuilder builder)
        {
            JsonOption.DefaultSerializer.Converters.Add(new TypeMappingConverter<ISite, SiteDefault>());
            JsonOption.DefaultSerializer.Converters.Add(new TypeMappingConverter<ILink, Link>());
            JsonOption.DefaultSerializer.Converters.Add(new TypeMappingConverter<ISiteRequest, SiteRequest>());

            return builder;
        }

        public static IPolarShadowBuilder AddWebAnalysisItem(this IPolarShadowBuilder builder)
        {
            return builder.Add(new WebAnalysisItemBuilder());
        }
    }
}
