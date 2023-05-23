using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IPolarShadowSite
    {
        string Name { get; }
        string Domain { get; }
        bool HasAbility(string name);
        bool TryGetParameter<TValue>(string name, out TValue value);
        Task<string> ExecuteAsync(string name, string input, CancellationToken cancellation = default);
        Task<string> ExecuteAsync(AnalysisAbility ability, string input, CancellationToken cancellation = default);
        Task<TOutput> ExecuteAsync<TInput,TOutput>(string name, TInput input, CancellationToken cancellation = default);
        Task<TOutput> ExecuteAsync<TInput, TOutput>(AnalysisAbility ability, TInput input, CancellationToken cancellation = default);
    }
}
