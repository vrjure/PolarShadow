using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal class PolarShadowSiteOptionBuilder : IPolarShadowSiteOptionBuilder
    {
        private readonly PolarShadowSiteOption _option;
        private readonly IPolarShadowOptionBuilder _optionBuilder;
        public PolarShadowSiteOptionBuilder(PolarShadowSiteOption siteOption, IPolarShadowOptionBuilder optionBuilder)
        {
            _option = siteOption;
            _optionBuilder = optionBuilder;
        }
        public IAnalysisAbilityBuilder AddAbility(string name)
        {
            _option.Abilities ??= new Dictionary<string, AnalysisAbility>();
            var ability = new AnalysisAbility();
            _option.Abilities.Add(name, ability);
            _optionBuilder.ChangeNodify();
            return new AnalysisAbilityBuilder(ability, _optionBuilder);
        }

        public IPolarShadowSiteOptionBuilder AddParameter<T>(string name, T value)
        {
            _option.Parameters ??= new Dictionary<string, object>();
            _option.Parameters.Add(name, value);
            _optionBuilder.ChangeNodify();
            return this;
        }

        public IPolarShadowSiteOptionBuilder RemoveAbility(string name)
        {
            _option.Abilities?.Remove(name);
            _optionBuilder.ChangeNodify();
            return this;
        }

        public IPolarShadowSiteOptionBuilder RemoveParameter(string name)
        {
            _option.Parameters?.Remove(name);
            _optionBuilder.ChangeNodify();
            return this;
        }
    }
}
