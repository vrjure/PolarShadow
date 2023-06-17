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
        private IPolarShadowOptionBuilder _optionBuilder;
        public IPolarShadowBuilder UseWebViewHandler(IRequestHandler requestHandler)
        {
            _webViewHandler = requestHandler;
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
            _optionBuilder ??= new PolarShadowOptionBuilder();
            var option = (_optionBuilder as PolarShadowOptionBuilder).Build();
            var parameter = new NameSlotValueCollection();
            if (option.Parameters != null)
            {
                parameter.AddNameValue(option.Parameters);
            }
            return new PolarShadowDefault(this, BuildSites(parameter, option.Sites), parameter);
        }

        private IEnumerable<IPolarShadowSite> BuildSites(NameSlotValueCollection parameter, IEnumerable<PolarShadowSiteOption> sites)
        {
            foreach (var item in sites)
            {
                var siteBuilder = new PolarShadowSiteBuilder(_webViewHandler, item, parameter.Clone());
                yield return siteBuilder.Build();
            }
        }
    }
}
