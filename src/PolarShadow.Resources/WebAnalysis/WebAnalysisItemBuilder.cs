using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    internal class WebAnalysisItemBuilder : IPolarShadowItemBuilder
    {
        public IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            return new WebAnalysisItem();
        }
    }
}
