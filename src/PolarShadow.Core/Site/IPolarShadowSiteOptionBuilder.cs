using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PolarShadow.Core
{
    public interface IPolarShadowSiteOptionBuilder
    {
        string Domain { get; set; }
        bool Enable { get; set; }
        bool UseWebView { get; set; }
        IParametersBuilder Parameters { get; }
        IPolarShadowSiteOptionBuilder BuildAbility(string name, Action<IAnalysisAbilityBuilder> abilityBuilder);
        IPolarShadowSiteOptionBuilder RemoveAbility(string name);
        void WriteTo(Stream output);
        void Load(string config);
    }
}
