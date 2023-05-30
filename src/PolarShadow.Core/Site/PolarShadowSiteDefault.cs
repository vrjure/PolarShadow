using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public async Task<TOutput> ExecuteAsync<TInput, TOutput>(AnalysisAbility ability, TInput input, CancellationToken cancellation = default)
        {
            var p = _paramaters.Clone();
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(input, JsonOption.DefaultSerializer));
            p.Add(doc.RootElement.Clone());

            return await _requestHandler.ExecuteAsync<TOutput>(ability, p, cancellation);
        }

        public async Task<string> ExecuteAsync(AnalysisAbility ability , string input, CancellationToken cancellation = default)
        {
            var p = _paramaters.Clone();
            using var doc = JsonDocument.Parse(input);
            p.Add(doc.RootElement.Clone());
            return await _requestHandler.ExecuteAsync(ability, p, cancellation);
        }

        public async Task<string> ExecuteAsync(string name, string input, CancellationToken cancellation = default)
        {
            if (_siteOption.Abilities != null && _siteOption.Abilities.TryGetValue(name, out AnalysisAbility ability))
            {
                return await ExecuteAsync(ability, input, cancellation);
            }
            return default;
        }

        public async Task<TOutput> ExecuteAsync<TInput, TOutput>(string name, TInput input, CancellationToken cancellation = default)
        {
            if (_siteOption.Abilities != null && _siteOption.Abilities.TryGetValue(name, out AnalysisAbility ability))
            {
                return await ExecuteAsync<TInput, TOutput>(ability, input, cancellation);
            }
            return default;
        }

        public bool HasAbility(string name)
        {
            return _siteOption.Abilities.ContainsKey(name);
        }

        public bool TryGetParameter<TValue>(string name, out TValue value)
        {
            if (_paramaters.TryReadValue(name, out NameSlotValue val))
            {
                switch (val.ValueKind)
                {
                    case NameSlotValueKind.Number:
                        if (val.GetDecimal() is TValue vd) { value = vd; return true; }
                        else if (val.GetInt16() is TValue v16) { value = v16; return true; }
                        else if (val.GetInt32() is TValue v32) { value = v32; return true; }
                        else if (val.GetInt64() is TValue v64) { value = v64; return true; }
                        else if (val.GetFloat() is TValue vfloat) { value = vfloat; return true; }
                        else if (val.GetDouble() is TValue vdouble) { value = vdouble; return true; }
                        break;
                    case NameSlotValueKind.String:
                        if(val.GetString() is TValue v)
                        {
                            value = v;
                            return true;
                        }
                        break;
                    case NameSlotValueKind.Boolean:
                        if (val.GetBoolean() is TValue vb)
                        {
                            value = vb;
                            return true;
                        }
                        break;
                    case NameSlotValueKind.Json:
                        if (val.GetJson() is TValue vj)
                        {
                            value = vj;
                            return true;
                        }
                        break;
                    case NameSlotValueKind.Html:
                        if (val.GetHtml() is TValue vh)
                        {
                            value = vh;
                            return true;
                        }
                        break;
                    default:
                        break;
                }
            }

            value = default;
            return false;
        }
    }
}
