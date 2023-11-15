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
                .ConfigureWebanalysis()
                .ConfigureParameter();
        }

        public static IPolarShadowBuilder ConfigureSiteDefault(this IPolarShadowBuilder builder)
        {
            return builder.ConfigureSiteItem(siteItemBuilder =>
            {
                siteItemBuilder.HttpHandler = new HttpClientRequestHandler();
                siteItemBuilder.RequestRules.Add(new RequestRule("*") { Writings = new List<IContentWriting> { new BasePropertyContentWriting() } });
            });
        }

        public static IPolarShadowBuilder ConfigureSiteItem(this IPolarShadowBuilder builder, Action<ISiteItemBuilder> itemBuilder)
        {
            if (!builder.TryGetItemBuilder(out SiteItemBuilder siteItemBuilder))
            {
                siteItemBuilder = builder.AddSiteItem() as SiteItemBuilder;
            }
            itemBuilder(siteItemBuilder);
            return builder;
        }

        public static ISiteItemBuilder AddSiteItem(this IPolarShadowBuilder builder)
        {
            if (builder.TryGetItemBuilder(out SiteItemBuilder siteItemBuilder))
            {
                return siteItemBuilder;
            }

            JsonOption.DefaultSerializer.Converters.Add(new TypeMappingConverter<ISite, SiteDefault>());
            JsonOption.DefaultSerializer.Converters.Add(new TypeMappingConverter<ILink, Link>());
            JsonOption.DefaultSerializer.Converters.Add(new TypeMappingConverter<ISiteRequest, SiteRequest>());

            siteItemBuilder = new SiteItemBuilder();
            builder.Add(siteItemBuilder);
            return siteItemBuilder;
        }




        public static IPolarShadowBuilder ConfigureWebanalysis(this IPolarShadowBuilder builder)
        {
            return builder.ConfigureWebAnalysis(itemBuilder =>
            {
                itemBuilder.HttpHandler = new HttpClientRequestHandler();
                itemBuilder.RequestRules.Add(new RequestRule("*") { Writings = new List<IContentWriting> { new BasePropertyContentWriting() } });
            });
        }

        public static IPolarShadowBuilder ConfigureWebAnalysis(this IPolarShadowBuilder builder, Action<ISiteItemBuilder> itemBuilder)
        {
            if (!builder.TryGetItemBuilder(out WebAnalysisItemBuilder webAnalysisItemBuilder))
            {
                webAnalysisItemBuilder = builder.AddWebAnalysisItem() as WebAnalysisItemBuilder;
            }

            itemBuilder(webAnalysisItemBuilder);
            return builder;
        }

        public static IPolarShadowItemBuilder AddWebAnalysisItem(this IPolarShadowBuilder builder)
        {
            if (builder.TryGetItemBuilder(out WebAnalysisItemBuilder webAnalysisBuilder))
            {
                return webAnalysisBuilder;
            }
            JsonOption.DefaultSerializer.Converters.Add(new TypeMappingConverter<IWebAnalysisSite, WebAnalysisSite>());

            webAnalysisBuilder = new WebAnalysisItemBuilder();
            builder.Add(webAnalysisBuilder);
            return webAnalysisBuilder;
        }




        public static IPolarShadowBuilder ConfigureParameter(this IPolarShadowBuilder builder)
        {
            if (!builder.TryGetItemBuilder(out ParameterItemBuilder _))
            {
                builder.AddParameterItem();
            }
            return builder;
        }
        

        public static IPolarShadowItemBuilder AddParameterItem(this IPolarShadowBuilder builder)
        {
            if (builder.TryGetItemBuilder(out ParameterItemBuilder parameterItemBuilder))
            {
                return parameterItemBuilder;
            }
            parameterItemBuilder = new ParameterItemBuilder();
            builder.Add(parameterItemBuilder);
            return parameterItemBuilder;
        }
    }
}
