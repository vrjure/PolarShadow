using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class AnalysisSourceAbleDefault: AnalysisAbilityBase, IAnalysisSourceAble
    {
        public override string Name => Abilities.WebAnalysisAble;

        public async Task<WebAnalysisSource> ExecuteAsync(AnalysisAbility ability, WebAnalysisSourceFilter input, CancellationToken cancellation = default)
        {
            return await ExecuteAsync<WebAnalysisSourceFilter, WebAnalysisSource>(ability, input, cancellation);
        }
    }
}
