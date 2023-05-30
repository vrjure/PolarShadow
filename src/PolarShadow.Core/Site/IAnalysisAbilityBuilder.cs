using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IAnalysisAbilityBuilder
    {
        IAnalysisAbilityBuilder SetRequest(AnalysisRequest request);
        IAnalysisAbilityBuilder SetResponse(AnalysisResponse response);
        IAnalysisAbilityBuilder AddParameter<T>(string name, T value);
        IAnalysisAbilityBuilder RemoveParameter(string name);
        IAnalysisAbilityBuilder Next();
    }

}
