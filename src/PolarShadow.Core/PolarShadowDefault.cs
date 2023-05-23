using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class PolarShadowDefault : IPolarShadow
    {
        private readonly Dictionary<string, IPolarShadowSite> _sites = new Dictionary<string, IPolarShadowSite>();
        private readonly Func<SearchVideoFilter, ISearcHandler> _searchHandlerFactory;

        private readonly PolarShadowOption _option;
        private IPolarShadowSiteBuilder _siteBuilder;

        internal PolarShadowDefault(PolarShadowOption option, IPolarShadowSiteBuilder siteBuilder, Func<SearchVideoFilter, ISearcHandler> searcHandlerFactory)
        {
            _option = option;
            _siteBuilder = siteBuilder;
            _searchHandlerFactory = searcHandlerFactory;

            foreach (var item in _option.Sites)
            {
                _sites[item.Name] = _siteBuilder.Build(item);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public IEnumerable<IPolarShadowSite> GetSites()
        {
            return _sites.Values.AsEnumerable();
        }

        public ISearcHandler BuildSearchHandler(SearchVideoFilter filter)
        {
            

            return _searchHandlerFactory(filter);
        }

        public bool TryGetSite(string name, out IPolarShadowSite site)
        {
            return _sites.TryGetValue(name, out site);
        }
    }
}
