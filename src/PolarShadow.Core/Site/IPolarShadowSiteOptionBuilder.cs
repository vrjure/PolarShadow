using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IPolarShadowSiteOptionBuilder
    {
        IPolarShadowSiteOptionBuilder AddParameter<T>(string name, T value);
        IPolarShadowSiteOptionBuilder RemoveParameter(string name);
        IAnalysisAbilityBuilder AddAbility(string name);
        IPolarShadowSiteOptionBuilder RemoveAbility(string name);
    }
}
