﻿using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Videos
{
    public static class PolarShadowBuilderExtensions
    {
        public static IPolarShadowBuilder ConfigreVideo(this IPolarShadowBuilder builder)
        {
            if (builder.TryGetItemBuilder(out ISiteItemBuilder siteItemBuilder))
            {
                siteItemBuilder.ConfigureVideo();
            }
            else
            {
                builder.ConfigureSiteItem(itemBuilder => itemBuilder.ConfigureVideo());
            }

            builder.AddWebAnalysisItem();
            return builder;
        }

        public static ISiteItemBuilder ConfigureVideo(this ISiteItemBuilder itemBuilder)
        {
            return itemBuilder;
        }

        public static IPolarShadowBuilder AddWebAnalysisItem(this IPolarShadowBuilder builder)
        {
            return builder.Add(new WebAnalysisItemBuilder());
        }
    }
}
