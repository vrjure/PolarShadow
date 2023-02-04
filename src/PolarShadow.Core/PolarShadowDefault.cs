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
        private readonly Dictionary<string, IAnalysisAbility> _supportAbilitis = new Dictionary<string, IAnalysisAbility>();
        private readonly Func<SearchVideoFilter, ISearcHandler> _searchHandlerFactory;

        internal PolarShadowDefault(IEnumerable<IAnalysisAbility> supportAbilities, Func<SearchVideoFilter, ISearcHandler> searcHandlerFactory)
        {
            _searchHandlerFactory = searcHandlerFactory;

            foreach (var item in supportAbilities)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    continue;
                }

                _supportAbilitis[item.Name] = item;
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

        public IEnumerable<IPolarShadowSite> GetSites()
        {
            return _sites.Values.AsEnumerable();
        }

        public ISearcHandler BuildSearchHandler(SearchVideoFilter filter)
        {
            if (_searchHandlerFactory == null)
            {
                return new SearcHandlerDefault(filter.SearchKey, filter.PageSize, this.GetAbilitieSites<ISearchAble>().ToArray());
            }

            return _searchHandlerFactory(filter);
        }

        public IEnumerable<IAnalysisAbility> GetSupportAbilities()
        {
            return _supportAbilitis.Values;
        }

        public void AddSite(IPolarShadowSite site)
        {
            _sites[site.Name] = site;
        }
    }
}
