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
        private readonly Dictionary<string, IAnalysisAbility> _abilities = new Dictionary<string, IAnalysisAbility>();
        private readonly Func<SearchVideoFilter, ISearcHandler> _searchHandlerFactory;

        private readonly PolarShadowOption _option;
        private IPolarShadowSiteBuilder _siteBuilder;

        internal PolarShadowDefault(PolarShadowOption option, IPolarShadowSiteBuilder siteBuilder, IEnumerable<IAnalysisAbility> abilities, Func<SearchVideoFilter, ISearcHandler> searcHandlerFactory)
        {
            _option = option;
            _siteBuilder = siteBuilder;
            _searchHandlerFactory = searcHandlerFactory;

            foreach (var item in _option.Sites)
            {
                _sites[item.Name] = _siteBuilder.Build(item);
            }

            foreach (var item in abilities)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    continue;
                }

                _abilities[item.Name] = item;
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public IEnumerable<IPolarShadowSite> GetSites()
        {
            return _sites.Values.AsEnumerable();
        }

        public ISearcHandler BuildSearchHandler(SearchVideoFilter filter)
        {
            if (_searchHandlerFactory == null && this.TryGetAbility(out ISearchAble searchAble))
            {
                return new SearcHandlerDefault(filter.SearchKey, filter.PageSize, searchAble, this.GetAbilitySites<ISearchAble>().ToArray());
            }

            return _searchHandlerFactory(filter);
        }

        public IEnumerable<IAnalysisAbility> GetAbilities()
        {
            return _abilities.Values;
        }

        public bool TryGetSite(string name, out IPolarShadowSite site)
        {
            return _sites.TryGetValue(name, out site);
        }

        public bool TryGetAbility(string name, out IAnalysisAbility ability)
        {
            return _abilities.TryGetValue(name, out ability);
        }
    }
}
