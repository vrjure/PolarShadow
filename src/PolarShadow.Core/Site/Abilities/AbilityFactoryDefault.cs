using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal class AbilityFactoryDefault<T> : IAbilityFactory<T>
    {
        public T Create(AnalysisAbility ability)
        {
            return (T)Activator.CreateInstance(typeof(T), ability);
        }

        object IAbilityFactory.Create(AnalysisAbility ability)
        {
            return this.Create(ability);
        }
    }
}
