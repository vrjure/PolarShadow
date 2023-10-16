using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    internal class ParameterItem : IParameterItem
    {
        public IKeyValueParameter Parameters { get; set; }

        public string Name => "parameters";

        public void Load(IPolarShadowProvider provider, bool reload = false)
        {
            if (reload)
            {
                Parameters?.Clear();
            }

            if (!provider.Root.TryGetProperty(Name, out JsonElement value) || value.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            BuildItem(value);
        }

        public void LoadFrom(IPolarShadowSource source)
        {
            if (source == null) return;

            var provider = source.Build(null);
            if (provider.Root.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            BuildItem(provider.Root);
            
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            JsonSerializer.Serialize(writer, this.Parameters, JsonOption.DefaultSerializer);
        }

        private void BuildItem(JsonElement value)
        {
            var parameters = JsonSerializer.Deserialize<IKeyValueParameter>(value, JsonOption.DefaultSerializer);

            if (this.Parameters == null)
            {
                this.Parameters = parameters;
                return;
            }

            foreach (var item in parameters)
            {
                this.Parameters[item.Key] = item.Value;
            }
        }
    }
}
