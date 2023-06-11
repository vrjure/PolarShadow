using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class PolarShadowSiteDefault : IPolarShadowSite
    {
        private readonly PolarShadowSiteOption _siteOption;
        private readonly NameSlotValueCollection _paramaters;
        private readonly IRequestHandler _requestHandler;
        internal PolarShadowSiteDefault(PolarShadowSiteOption option, NameSlotValueCollection paramaters, IRequestHandler requestHandler)
        {
            _siteOption = option;
            _paramaters = paramaters ?? new NameSlotValueCollection();
            _requestHandler = requestHandler;
            if (option.Parameters != null)
            {
                _paramaters.AddNameValue(option.Parameters);
            }
        }

        public string Name => _siteOption.Name;

        public string Domain => _siteOption.Domain;

        public ISiteRequestHandler CreateRequestHandler(string name)
        {
            if (_siteOption.Abilities == null || !_siteOption.Abilities.TryGetValue(name, out AnalysisAbility ability))
            {
                return null;
            }
            return new SiteRequestHandler(_requestHandler, ability, _paramaters.Clone());
        }

        public async Task ExecuteAsync(string name, string input, Stream stream, CancellationToken cancellation = default)
        {

            var p = _paramaters.Clone();
            if (ability.Parameters != null)
            {
                p.AddNameValue(ability.Parameters);
            }

            input.Format(p);
            using var doc = JsonDocument.Parse(input);
            p.AddNameValue(doc.RootElement);
            await _requestHandler.ExecuteAsync(ability, stream, p, cancellation);
        }

        public bool HasAbility(string name)
        {
            return _siteOption.Abilities.ContainsKey(name);
        }

        public bool TryGetParameter<TValue>(string name, out TValue value)
        {
            return _paramaters.TryReadValue(name, out value);
        }
    }
}
