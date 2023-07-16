using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Videos
{
    public static class PolarShadowBuilderExtensions
    {
        public static IPolarShadowBuilder AddWebAnalysisItem(this IPolarShadowBuilder builder)
        {
            return builder.Add(new WebAnalysisItemBuilder());
        }
    }
}
