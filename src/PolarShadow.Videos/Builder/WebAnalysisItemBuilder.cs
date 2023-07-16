using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Videos
{
    internal class WebAnalysisItemBuilder : IPolarShadowItemBuilder
    {
        public IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            return new WebAnalysisItem();
        }
    }
}
