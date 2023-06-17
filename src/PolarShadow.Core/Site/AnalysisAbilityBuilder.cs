using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal class AnalysisAbilityBuilder : IAnalysisAbilityBuilder
    {
        private readonly AnalysisAbility _ability;
        private readonly IPolarShadowOptionBuilder _optionBuilder;

        public AnalysisAbilityBuilder(AnalysisAbility ability, IPolarShadowOptionBuilder optionBuilder)
        {
            _ability = ability;
            _optionBuilder = optionBuilder;
        }

        public IAnalysisAbilityBuilder AddParameter<T>(string name, T value)
        {
            _ability.Parameters ??= new Dictionary<string, object>();
            _ability.Parameters.Add(name, value);
            _optionBuilder.ChangeNodify();
            return this;
        }

        public IAnalysisAbilityBuilder Next()
        {
            return new AnalysisAbilityBuilder(new AnalysisAbility(), _optionBuilder);
        }

        public IAnalysisAbilityBuilder RemoveParameter(string name)
        {
            _ability.Parameters?.Remove(name);
            _optionBuilder.ChangeNodify();
            return this;
        }

        public IAnalysisAbilityBuilder SetRequest(AnalysisRequest request)
        {
            _ability.Request = request;
            _optionBuilder.ChangeNodify();
            return this;
        }

        public IAnalysisAbilityBuilder SetResponse(AnalysisResponse response)
        {
            _ability.Response = response;
            _optionBuilder.ChangeNodify();
            return this;
        }
    }
}
