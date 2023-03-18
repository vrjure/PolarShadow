using PolarShadow.Core;
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
        private Func<SearchVideoFilter, ISearcHandler> _searcHandlerFactory;
        private IPolarShadow _cache;

        internal Dictionary<string, IAnalysisAbility> _supportAbilities = new Dictionary<string, IAnalysisAbility>();


        public PolarShadowOption Option { get; }

        public PolarShadowBuilder() : this(default)
        {
        }

        public PolarShadowBuilder(PolarShadowOption option)
        {
            this.Option = option;
            if (this.Option == null)
            {
                this.Option = new PolarShadowOption();
            }
        }

        public void AddSearcHandlerFactory(Func<SearchVideoFilter, ISearcHandler> factory)
        {
            _searcHandlerFactory = factory;
        }

        public void AddAbility(IAnalysisAbility ability)
        {
            if (string.IsNullOrEmpty(ability.Name))
            {
                throw new ArgumentException(nameof(ability.Name));
            }
            _supportAbilities[ability.Name] = ability;
        }

        public IPolarShadow Build()
        {
            if (Option.IsChanged || _cache == null)
            {
                var ps = new PolarShadowDefault(_supportAbilities.Values, _searcHandlerFactory);
                var siteBuilder = new PolarShadowSiteBuilder(this);
                if (Option.Sites != null && Option.Sites.Count > 0)
                {
                    foreach (var item in Option.Sites)
                    {
                        ps.AddSite(siteBuilder.Build(item));
                    }
                }
                Option.IsChanged = false;
                _cache = ps;
            }

            return _cache;
            
        }
    }
}
