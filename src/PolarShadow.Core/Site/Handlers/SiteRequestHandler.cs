using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class SiteRequestHandler : ISiteRequestHandler
    {
        private readonly IRequestHandler _handler;
        private readonly AnalysisAbility _ability;
        private readonly NameSlotValueCollection _parameter;
        public SiteRequestHandler(IRequestHandler requesthandler, AnalysisAbility ability, NameSlotValueCollection parameter)
        {
            _handler = requesthandler;
            _ability = ability;
            _parameter = parameter;
            if (_ability.Parameters != null)
            {
                _parameter.AddNameValue(ability.Parameters);
            }
        }

        public async Task ExecuteAsync(Stream stream, CancellationToken cancellation = default)
        {
            await ExecuteAsync(default, stream, cancellation);
        }

        public async Task ExecuteAsync(string input, Stream stream, CancellationToken cancellation = default)
        {
            var p = _parameter.Clone();
            if (!string.IsNullOrEmpty(input))
            {
                using var doc = JsonDocument.Parse(input);
                p.Add(doc.RootElement.Clone());
            }
            await _handler.ExecuteAsync(stream, _ability.Request, _ability.Response, p, cancellation);
        }

        public bool TryGetParameter<T>(string name, out T value)
        {
            return _parameter.TryReadValue(name, out value);
        }
    }
}
