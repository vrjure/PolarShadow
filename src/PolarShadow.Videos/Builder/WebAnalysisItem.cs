using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Videos
{
    internal class WebAnalysisItem : IWebAnalysisItem
    {
        private readonly Dictionary<string, WebAnalysisSource> _source = new Dictionary<string, WebAnalysisSource>(StringComparer.OrdinalIgnoreCase);

        public WebAnalysisSource this[string name]
        {
            get => _source.ContainsKey(name) ? _source[name] : null;
            set => _source[name] = value;
        }

        public IEnumerable<WebAnalysisSource> Sources => _source.Values;

        public string Name => "webAnalysisSource";

        public void Load(IPolarShadowProvider provider, bool reLoad = false)
        {
            if (reLoad)
            {
                _source.Clear();
            }

            if (!provider.TryGet(Name, out JsonElement value)
                || value.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            var list = JsonSerializer.Deserialize<ICollection<WebAnalysisSource>>(value, JsonOption.DefaultSerializer);
            if (list == null) return;
            foreach (var item in list)
            {
                _source[item.Name] = item;
            }
        }

        public void Remove(string name)
        {
            _source.Remove(name);
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            writer.WritePropertyName(Name);
            JsonSerializer.Serialize(writer, Sources, JsonOption.DefaultSerializer);
        }
    }
}
