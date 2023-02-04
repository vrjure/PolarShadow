using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal sealed class PolarShadowSiteBuilder : IPolarShadowSiteBuilder
    {
        private readonly PolarShadowBuilder _builder;
        public PolarShadowSiteBuilder(PolarShadowBuilder builder)
        {
            _builder = builder;
        }

        public IPolarShadowSite Build(SiteOption option)
        {
            var abilities = new List<IAnalysisAbility>(option.Abilities.Count);
            
            foreach (var item in option.Abilities)
            {
                if (_builder._supportAbilities.TryGetValue(item.Key, out IAnalysisAbility ability))
                {
                    abilities.Add(ability);
                }
            }

            return new PolarShadowSiteDefault(option, abilities);
        }
    }
}
