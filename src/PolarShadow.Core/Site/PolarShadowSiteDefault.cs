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
        private readonly SiteOption _siteOption;
        internal PolarShadowSiteDefault(SiteOption option)
        {
            _siteOption = option;
        }

        public string Name => _siteOption.Name;

        public string Domain => _siteOption.Domain;

        public async Task<TOutput> ExecuteAsync<TInput, TOutput>(IAnalysisAbility<TInput,TOutput> ability, TInput input, CancellationToken cancellation = default) where TInput : new() where TOutput : new()
        {
            if (_siteOption.Abilities != null && _siteOption.Abilities.TryGetValue(ability.Name, out AnalysisAbility analysis))
            {
                return await ability.ExecuteAsync(analysis, input, default);
            }
            return default;
        }

        public async Task<string> ExecuteAsync(IAnalysisAbility ability, string input, CancellationToken cancellation = default)
        {
            if (_siteOption.Abilities != null && _siteOption.Abilities.TryGetValue(ability.Name, out AnalysisAbility analysis))
            {
                return await ability.ExecuteAsync(analysis, input, cancellation);
            }
            return default;
        }

        public bool HasAbility(string name)
        {
            return _siteOption.Abilities != null && _siteOption.Abilities.ContainsKey(name);
        }

        public bool TryGetParameter<TValue>(string name, out TValue value)
        {
            if (_siteOption.Parameters != null && _siteOption.Parameters.TryGetValue(name, out object v) && v is TValue val)
            {
                value = val;
                return true;
            }

            value = default;
            return false;
        }
    }
}
