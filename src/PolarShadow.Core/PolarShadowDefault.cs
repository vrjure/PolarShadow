using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class PolarShadowDefault : IPolarShadow
    {
        private readonly ICollection<IPolarShadowItem> _items;
        private readonly IPolarShadowBuilder _builder;
        private IParameterCollection _parameters;
        private IKeyValueParameter _configParameters;
        public PolarShadowDefault(IPolarShadowBuilder builder)
        {
            _builder = builder;
            _items = new List<IPolarShadowItem>();
            _parameters = new Parameters();
            _configParameters = new KeyValueParameter();

            Load();
        }

        public IEnumerable<IPolarShadowItem> Items => _items;

        public void Load()
        {
            _parameters.Clear();
            _items.Clear();

            if (_builder.Parameters.Count > 0)
            {
                _parameters.Add(_builder.Parameters);
            }

            var providers = new List<IPolarShadowProvider>();
            foreach (var item in _builder.Sources)
            {
                var provider = item.Build(_builder);
                if (provider != null)
                {
                    provider.Load();
                    BuildParameters(provider);
                    providers.Add(provider);
                }
            }

            if (_configParameters.Count > 0)
            {
                _parameters.Add(_configParameters);
            }

            foreach (var builder in _builder.ItemBuilders)
            {
                var item = builder.Build(_builder, providers);
                if (item != null)
                {
                    _items.Add(item);
                }
            }
        }

        public void Write(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            if (_configParameters.Count > 0)
            {
                writer.WritePropertyName("parameters");
                _configParameters.Write(writer);
            }

            foreach (var item in _items)
            {
                item.Write(writer);
            }
            writer.WriteEndObject();
        }

        private void BuildParameters(IPolarShadowProvider provider)
        {
            if (provider.TryGet("parameters", out JsonElement value))
            {
                _configParameters.Add(value);
            }
        }
    }
}
