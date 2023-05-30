using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public class PolarShadowBuilder : IPolarShadowBuilder
    {
        private IRequestHandler _webViewHandler;
        private IPolarShadowSiteBuilder _siteBuilder;
        private IPolarShadowOptionBuilder _optionBuilder;
        public IPolarShadowBuilder UseWebViewHandler(IRequestHandler requestHandler)
        {
            _webViewHandler = requestHandler;
            return this;
        }

        public IPolarShadowBuilder UseSiteBuilder(IPolarShadowSiteBuilder siteBuilder)
        {
            _siteBuilder = siteBuilder;
            return this;
        }

        public IPolarShadowBuilder UseOptionBuilder(IPolarShadowOptionBuilder optionBuilder)
        {
            _optionBuilder = optionBuilder;
            return this;
        }

        public IPolarShadowBuilder Configure(Action<IPolarShadowOptionBuilder> optionBuilder)
        {
            if (_optionBuilder == null)
            {
                _optionBuilder = new PolarShadowOptionBuilder();
            }
            optionBuilder(_optionBuilder);
            return this;
        }

        public IPolarShadow Build()
        {
            return new PolarShadowDefault(this, BuildSites());
        }

        private IEnumerable<IPolarShadowSite> BuildSites()
        {
            _siteBuilder ??= new PolarShadowSiteBuilder(_webViewHandler);
            _optionBuilder ??= new PolarShadowOptionBuilder();
            var option = _optionBuilder.Build();

            var parameter = new NameSlotValueCollection();
            if (option.Parameters != null)
            {
                parameter.AddNameValue(option.Parameters);
            }

            foreach (var item in option.Sites)
            {
                yield return _siteBuilder.Build(item, parameter);
            }
        }
    }
}
