using PolarShadow.Core.Site;
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
        public static Dictionary<Type, string> SupportAbilityTypes = new Dictionary<Type, string>();

        private Func<SearchVideoFilter, ISearcHandler> _searcHandlerFactory;
        private IPolarShadow _cache;

        internal Dictionary<string, IAbilityFactory> _supportAbilityFactories = new Dictionary<string, IAbilityFactory>();


        public PolarShadowOption Option { get; }

        public PolarShadowBuilder() :this(default)
        {
        }

        public PolarShadowBuilder(PolarShadowOption option)
        {
            this.Option = option;
            if (this.Option == null)
            {
                this.Option = new PolarShadowOption();
            }
            RegisterSupportDefaultAbility();
        }

        public void AddSearcHandlerFactory(Func<SearchVideoFilter, ISearcHandler> factory)
        {
            _searcHandlerFactory = factory;
        }

        public void RegisterSupportAbilityFactory<T>(string name, IAbilityFactory<T> ability)
        {
            _supportAbilityFactories[name] = ability;
            SupportAbilityTypes[typeof(T).GetInterfaces()[0]] = name;
        }

        public IPolarShadow Build()
        {
            if (Option.IsChanged || _cache == null)
            {
                List<IPolarShadowSite> sites = new List<IPolarShadowSite>();
                if (Option.Sites != null && Option.Sites.Count > 0)
                {
                    foreach (var item in Option.Sites)
                    {
                        sites.Add(new PolarShadowSiteBuilder(item, this).Build());
                    }
                }
                _cache = new PolarShadowDefault(sites, _searcHandlerFactory);
                Option.IsChanged = false;
            }

            return _cache;
            
        }

        private void RegisterSupportDefaultAbility()
        {
            RegisterSupportAbilityFactory(Abilities.SearchAble, new AbilityFactoryDefault<SearchAbleDefault>());
            RegisterSupportAbilityFactory(Abilities.GetDetailAble, new AbilityFactoryDefault<GetDetailAbleDefault>());
            RegisterSupportAbilityFactory(Abilities.HtmlAnalysisAble, new AbilityFactoryDefault<HtmlAnalysisAbleDefault>());
        }
    }
}
