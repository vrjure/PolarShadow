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
        private readonly Dictionary<string, AnalysisAbility> _analysisAbilities = new Dictionary<string, AnalysisAbility>();
        internal PolarShadowSiteDefault(SiteOption option, IEnumerable<IAnalysisAbility> abilities)
        {
            this.Name = option.Name;
            this.Domain = option.Domain;
            _abilities = new List<IAnalysisAbility>(abilities);
        }

        public string Name { get; }

        public string Domain { get; }


        public IEnumerable<IAnalysisAbility> EnumerableAbilities()
        {
            return _abilities;
        }

        public async Task<TOutput> ExecuteAsync<TInput, TOutput>(IAnalysisAbility<TInput,TOutput> ability, TInput input, CancellationToken cancellation = default)
        {
            if (_analysisAbilities.TryGetValue(ability.Name, out AnalysisAbility analysis))
            {
                return await ability.ExecuteAsync(analysis, input, default);
            }
            return default;
        }
    }
}
