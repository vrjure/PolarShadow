using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public abstract class AnalysisAbilityBase : IAnalysisAbility
    {

        public abstract string Name { get; }

        protected Task<TOutput> ExecuteAsync<TInput, TOutput>(AnalysisAbility ability, TInput input, CancellationToken cancellation = default)
        {
            return ability.ExecuteAsync<TInput, TOutput>(input, cancellation);
        }

    }
}
