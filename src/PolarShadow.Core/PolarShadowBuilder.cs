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
        private IPolarShadow _polarShadow;

        public bool IsOptionChanged => _optionBuilder == null ? false : _optionBuilder.IsChanged;

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
            if (_optionBuilder.IsChanged || _polarShadow == null)
            {
                var option = (_optionBuilder as PolarShadowOptionBuilder).Build();
                var parameter = new NameSlotValueCollection();
                if (option.Parameters != null)
                {
                    parameter.AddNameValue(option.Parameters);
                }
                if (option.Sites == null)
                {
                    return new PolarShadowDefault(this, Enumerable.Empty<IPolarShadowSite>(), parameter);
                }
                return new PolarShadowDefault(this, BuildSites(parameter, option.Sites), parameter);
            }
            else
            {
                return _polarShadow;
            }
        }

        private IEnumerable<IPolarShadowSite> BuildSites(NameSlotValueCollection parameter, IEnumerable<PolarShadowSiteOption> sites)
        {
            foreach (var item in sites)
            {
                if (!item.Enable)
                {
                    continue;
                }
                var siteBuilder = new PolarShadowSiteBuilder(_webViewHandler, item, parameter.Clone());
                yield return siteBuilder.Build();
            }
        }
    }
}
