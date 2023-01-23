using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IAbilityFactory
    {
        object Create(AnalysisAbility ability);
    }
}
