using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    internal class WebAnalysisItem : SiteItemBase<IWebAnalysisSite>, IWebAnalysisItem
    {
        public WebAnalysisItem(IRequestHandler httpHandler, IRequestHandler webViewHandler, IEnumerable<RequestRule> requestRules) : base(httpHandler, webViewHandler, requestRules)
        {
        }

        public override string Name => "webAnalysisSites";
    }
}
