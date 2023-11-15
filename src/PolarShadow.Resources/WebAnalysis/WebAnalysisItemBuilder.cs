using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    internal class WebAnalysisItemBuilder : SiteItemBuilderBase
    {
        public override IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            return new WebAnalysisItem(HttpHandler ?? new HttpClientRequestHandler(), WebViewHandler, RequestRules);
        }
    }
}
