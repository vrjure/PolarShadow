using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public class PolarShadowBuilder : IPolarShadowBuilder
    {
        private readonly List<IPolarShadowSite> _sites = new List<IPolarShadowSite>();
        private Func<SearchVideoFilter, ISearcHandler> _searcHandlerFactory;
        private readonly List<PolarShadowSiteConfig> _siteConfigs = new List<PolarShadowSiteConfig>();
        internal Dictionary<string, IAbilityFactory> _supportAbilityFactories = new Dictionary<string, IAbilityFactory>();
        
        public PolarShadowBuilder()
        {
            RegisterSupportDefaultAbility();
        }

        public PolarShadowBuilder(IEnumerable<PolarShadowSiteConfig> siteConfigs) : this()
        {
            _siteConfigs.AddRange(siteConfigs);
        }

        public void AddSearcHandlerFactory(Func<SearchVideoFilter, ISearcHandler> factory)
        {
            _searcHandlerFactory = factory;
        }

        public void AddSite(IPolarShadowSite site)
        {
            _sites.Add(site);
        }

        public void AddSite(PolarShadowSiteConfig config)
        {
            _siteConfigs.Add(config);
        }

        public void RegisterSupportAbilityFactory(string name, IAbilityFactory ability)
        {
            _supportAbilityFactories[name] = ability;
        }

        public IPolarShadow Build()
        {
            if (_siteConfigs != null && _siteConfigs.Count > 0)
            {
                foreach (var item in _siteConfigs)
                {
                    _sites.Add(new PolarShadowSiteBuilder(item, this).Build());
                }
            }
            return new PolarShadowDefault(_sites, _searcHandlerFactory);
        }

        private void RegisterSupportDefaultAbility()
        {
            RegisterSupportAbilityFactory(Abilities.SearchAble, new AbilityFactoryDefault<SearchAbleDefault>());
        }
    }
}
