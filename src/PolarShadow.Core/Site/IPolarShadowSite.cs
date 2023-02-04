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
        IEnumerable<IAnalysisAbility> EnumerableAbilities();
        Task<TOutput> ExecuteAsync<TInput, TOutput>(IAnalysisAbility<TInput, TOutput> ability, TInput input, CancellationToken cancellation = default);
    }
}
