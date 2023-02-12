using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public abstract class AnalysisAbilityBase<TInput, TOutput> : IAnalysisAbility
    {

        public abstract string Name { get; }

        public async Task<TOutput> ExecuteAsync(AnalysisAbility ability, TInput input, CancellationToken cancellation = default)
        {
            HandleInput(input);
            var result = await ability.ExecuteAsync<TInput, TOutput>(input, cancellation);
            ValueHandler(input, result);
            return result;
        }

        public async Task<string> ExecuteAsync(AnalysisAbility ability, string input, CancellationToken cancellation = default)
        {
            var inputObj = JsonSerializer.Deserialize<TInput>(input, JsonOption.DefaultSerializer);
            var result = await ExecuteAsync(ability, inputObj, cancellation);
            return JsonSerializer.Serialize(result, JsonOption.DefaultSerializer);
        }

        protected virtual void HandleInput(TInput input)
        {

        }

        protected virtual void ValueHandler(TInput input, TOutput output)
        {

        }
    }
}
