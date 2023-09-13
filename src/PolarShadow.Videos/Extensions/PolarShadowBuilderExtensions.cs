using PolarShadow.Core;
using PolarShadow.Resources;
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
            builder.AddWebAnalysisItem();
            return builder;
        }

        
    }
}
