using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal sealed class PolarShadowSiteBuilder : IPolarShadowSiteBuilder
    {
        private readonly static IRequestHandler _defaultHandler = new HttpClientRequestHandler();
        private readonly IRequestHandler _webViewHandler;
        public PolarShadowSiteBuilder(IRequestHandler webViewHandler)
        {
            _webViewHandler = webViewHandler;
        }

        public IPolarShadowSite Build(PolarShadowSiteOption option, NameSlotValueCollection parameters)
        {
            if (option.UseWebView)
            {
                return new PolarShadowSiteDefault(option, parameters?.Clone(), _webViewHandler);
            }
            return new PolarShadowSiteDefault(option, parameters?.Clone(), _defaultHandler);
        }
    }
}
