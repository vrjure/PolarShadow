using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class AbilityFactoryDefault<T>: IAbilityFactory
    {
        public T Create(AnalysisAbility ability)
        {
            return (T)Activator.CreateInstance(typeof(T), ability);
        }

        object IAbilityFactory.Create(AnalysisAbility ability)
        {
            return Create(ability);
        }
    }
}
