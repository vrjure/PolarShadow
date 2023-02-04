using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PolarShadow.Core
{
    public interface IAnalysisAbility
    {
        string Name { get; }
    }

    public interface IAnalysisAbility<TInput,TOutput> : IAnalysisAbility
    {
        Task<TOutput> ExecuteAsync(AnalysisAbility ability, TInput input, CancellationToken cancellation = default);
    }
}
