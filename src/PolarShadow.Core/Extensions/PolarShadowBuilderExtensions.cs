using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Core
{
    public static class PolarShadowBuilderExtensions
    {
        public static IPolarShadowBuilder ConfigureDefault(this IPolarShadowBuilder builder )
        {
            return builder.ConfigureSiteItem(siteItemBuilder =>
            {
                siteItemBuilder.HttpHandler = new HttpClientRequestHandler();
                siteItemBuilder.Writings.Add(new SitePropertyContentWriting());
            });
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
            if(builder.TryGetItemBuilder(out ISiteItemBuilder siteItemBuilder))
            {
                return siteItemBuilder;
            }

            siteItemBuilder = new SiteItemBuilder();
            builder.Add(siteItemBuilder);
            return siteItemBuilder;
        }

        public static bool HasItemBuilder<T>(this IPolarShadowBuilder builder) where T : IPolarShadowItemBuilder
        {
            return builder.ItemBuilders.Any(f => f is T);
        }

        public static bool TryGetItemBuilder<T>(this IPolarShadowBuilder builder, out T value) where T : class, IPolarShadowItemBuilder
        {
            value = builder.ItemBuilders.Where(f => f is T).FirstOrDefault() as T;
            return value != null;
            
        }
    }
}
