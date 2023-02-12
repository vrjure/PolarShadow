using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class PolarShadowSiteDefault : IPolarShadowSite
    {
        private readonly ICollection<IAnalysisAbility> _abilities;
        private readonly Dictionary<string, AnalysisAbility> _analysisAbilities;
        internal PolarShadowSiteDefault(SiteOption option, IEnumerable<IAnalysisAbility> abilities)
        {
            this.Name = option.Name;
            this.Domain = option.Domain;
            _abilities = new List<IAnalysisAbility>(abilities);
            _analysisAbilities = new Dictionary<string, AnalysisAbility>(option.Abilities);
        }

        public string Name { get; }

        public string Domain { get; }

        public IReadOnlyCollection<IAnalysisAbility> GetAbilities()
        {
            return new List<IAnalysisAbility>(_abilities);
        }

        public async Task<TOutput> ExecuteAsync<TInput, TOutput>(IAnalysisAbility<TInput,TOutput> ability, TInput input, CancellationToken cancellation = default) where TInput : new() where TOutput : new()
        {
            if (_analysisAbilities.TryGetValue(ability.Name, out AnalysisAbility analysis))
            {
                return await ability.ExecuteAsync(analysis, input, default);
            }
            return default;
        }

        public async Task<string> ExecuteAsync(IAnalysisAbility ability, string input, CancellationToken cancellation = default)
        {
            if (_analysisAbilities.TryGetValue(ability.Name, out AnalysisAbility analysis))
            {
                return await ability.ExecuteAsync(analysis, input, cancellation);
            }
            return default;
        }
    }
}
