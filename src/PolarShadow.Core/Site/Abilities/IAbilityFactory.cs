using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IAbilityFactory
    {
        object Create(AnalysisAbility ability);
    }

    public interface IAbilityFactory<T> : IAbilityFactory
    {
        new T Create(AnalysisAbility ability);
    }
}
