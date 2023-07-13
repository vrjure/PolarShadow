using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public static class PolarShadowBuilderExtensions
    {
        public static IPolarShadowBuilder ConfigureDefault(this IPolarShadowBuilder builder)
        {
            return builder.Add(new SiteItemBuilder());
        }

        public static IPolarShadowBuilder AddJsonFileSource(this IPolarShadowBuilder builder, string path)
        {
            return builder.Add(new JsonFileSource { Path = path });
        }
    }
}
