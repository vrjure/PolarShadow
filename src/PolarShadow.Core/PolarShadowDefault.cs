using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class PolarShadowDefault : IPolarShadow
    {
        private readonly IPolarShadowBuilder _builder;
        private readonly ICollection<IPolarShadowItem> _items;
        private readonly IKeyValueParameter _globalePrameter;

        public PolarShadowDefault(IPolarShadowBuilder builder)
        {
            _builder = builder;
            _globalePrameter = builder.Parameters;

            _items = new List<IPolarShadowItem>();
            foreach (var b in builder.ItemBuilders)
            {
                _items.Add(b.Build(builder));
            }
        }

        public IEnumerable<IPolarShadowItem> Items => _items;

        public void Load(IPolarShadowSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var provider = source.Build(_builder);
            if (provider == null)
            {
                return;
            }

            provider.Load();
            BuildParameters(provider);

            foreach (var item in _items)
            {
                item.Load(provider);
            }
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();

            if (_globalePrameter.Count > 0)
            {
                writer.WritePropertyName("parameters");
                _globalePrameter.WriteTo(writer);
            }

            foreach (var item in _items)
            {
                item.WriteTo(writer);
            }

            writer.WriteEndObject();
        }

        private void BuildParameters(IPolarShadowProvider provider)
        {
            if (provider.TryGet("parameters", out JsonElement value))
            {
                _globalePrameter.Add(value);
            }
        }
    }
}
