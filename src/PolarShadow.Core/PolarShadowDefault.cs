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

        internal PolarShadowDefault(IEnumerable<IPolarShadowSite> sites): this(sites, default)
        {

        }

        internal PolarShadowDefault(IEnumerable<IPolarShadowSite> sites, Func<SearchVideoFilter, ISearcHandler> searcHandlerFactory)
        {
            _searchHandlerFactory = searcHandlerFactory;
            foreach (var item in sites)
            {
                if (string.IsNullOrEmpty(item.Domain))
                {
                    continue;
                }
                _sites[item.Name] = item;
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        }

        public IPolarShadowSite GetSite(string name)
        {
            if (_sites.TryGetValue(name, out IPolarShadowSite site))
            {
                return site;
            }

            return null;
        }

        public IReadOnlyCollection<IPolarShadowSite> GetSites()
        {
            return _sites.Values;
        }

        public ISearcHandler BuildSearchHandler(SearchVideoFilter filter)
        {
            if (_searchHandlerFactory == null)
            {
                return new SearcHandlerDefault(filter.SearchKey, filter.PageSize, this.GetSites<ISearchAble>().ToArray());
            }

            return _searchHandlerFactory(filter);
        }
    }
}
