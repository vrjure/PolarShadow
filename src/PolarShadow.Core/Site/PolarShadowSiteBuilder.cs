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
        private readonly PolarShadowSiteOption _option;
        private readonly NameSlotValueCollection _parameters;
        public PolarShadowSiteBuilder(IRequestHandler webViewHandler, PolarShadowSiteOption option, NameSlotValueCollection parameters)
        {
            _webViewHandler = webViewHandler;
            _option = option;
            _parameters = parameters;
        }

        public IPolarShadowSite Build()
        {
            if (_option.UseWebView)
            {
                return new PolarShadowSiteDefault(_option, _parameters?.Clone(), _webViewHandler);
            }
            return new PolarShadowSiteDefault(_option, _parameters?.Clone(), _defaultHandler);
        }
    }
}
