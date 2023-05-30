using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal class PolarShadowSiteOptionBuilder : IPolarShadowSiteOptionBuilder
    {
        private readonly PolarShadowSiteOption _option;
        public PolarShadowSiteOptionBuilder(PolarShadowSiteOption siteOption)
        {
            _option = siteOption;
        }
        public IAnalysisAbilityBuilder AddAbility(string name)
        {
            var ability = new AnalysisAbility();
            _option.Abilities.Add(name, ability);
            return new AnalysisAbilityBuilder(ability);
        }

        public IPolarShadowSiteOptionBuilder AddParameter<T>(string name, T value)
        {
            _option.Parameters ??= new Dictionary<string, object>();
            _option.Parameters.Add(name, value);
            return this;
        }

        public IPolarShadowSiteOptionBuilder RemoveAbility(string name)
        {
            _option.Abilities?.Remove(name);
            return this;
        }

        public IPolarShadowSiteOptionBuilder RemoveParameter(string name)
        {
            _option.Parameters?.Remove(name);
            return this;
        }
    }
}
