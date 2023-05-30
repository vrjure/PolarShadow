using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal class AnalysisAbilityBuilder : IAnalysisAbilityBuilder
    {
        private readonly AnalysisAbility _ability;

        public AnalysisAbilityBuilder(AnalysisAbility ability)
        {
            _ability = ability;
        }

        public IAnalysisAbilityBuilder AddParameter<T>(string name, T value)
        {
            _ability.Parameters ??= new Dictionary<string, object>();
            _ability.Parameters.Add(name, value);
            return this;
        }

        public IAnalysisAbilityBuilder Next()
        {
            return new AnalysisAbilityBuilder(new AnalysisAbility());
        }

        public IAnalysisAbilityBuilder RemoveParameter(string name)
        {
            _ability.Parameters?.Remove(name);
            return this;
        }

        public IAnalysisAbilityBuilder SetRequest(AnalysisRequest request)
        {
            _ability.Request = request;
            return this;
        }

        public IAnalysisAbilityBuilder SetResponse(AnalysisResponse response)
        {
            _ability.Response = response;
            return this;
        }
    }
}
