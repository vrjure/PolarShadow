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
        Task<string> ExecuteAsync(AnalysisAbility ability, string input, CancellationToken cancellation = default);
    }

    public interface IAnalysisAbility<TInput,TOutput> : IAnalysisAbility where TInput : new() where TOutput : new()
    {
        Task<TOutput> ExecuteAsync(AnalysisAbility ability, TInput input, CancellationToken cancellation = default);
    }
}
