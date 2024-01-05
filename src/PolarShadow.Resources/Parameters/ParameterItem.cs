using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    internal class ParameterItem : IParameterItem
    {
        private readonly IKeyValueParameter _prefabParams;
        private readonly IKeyValueParameter _combineParams;
        public ParameterItem(string name, IKeyValueParameter prefabParams = null)
        {
            this.Name = name;
            _prefabParams = prefabParams;
            if (_prefabParams?.Count > 0)
            {
                _combineParams = new KeyValueParameter();
                foreach (var item in _prefabParams)
                {
                    _combineParams.Add(item.Key, item.Value);
                }
            }
        }

        private IKeyValueParameter _parameters;
        public IKeyValueParameter Parameters
        {
            get
            {
                if (_prefabParams?.Count > 0)
                {
                    return _combineParams;
                }
                else
                {
                    return _parameters;
                }
            }
            set
            {
                _parameters = value;
                if (_prefabParams?.Count > 0)
                {
                    _combineParams.Clear();
                    foreach (var item in _prefabParams)
                    {
                        _combineParams.Add(item.Key, item.Value);
                    }
                    if (_parameters?.Count > 0)
                    {
                        foreach (var item in _parameters)
                        {
                            _combineParams.Add(item.Key, item.Value);
                        }
                    }
                }
            }
        }

        public string Name { get; }

        public void Load(IPolarShadowProvider provider, bool reload = false)
        {
            if (reload)
            {
                _parameters?.Clear();
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
            JsonSerializer.Serialize(writer, _parameters, JsonOption.DefaultSerializer);
        }

        private void BuildItem(JsonElement value)
        {
            var parameters = JsonSerializer.Deserialize<IKeyValueParameter>(value, JsonOption.DefaultSerializer);

            if (_parameters == null)
            {
                this.Parameters = parameters;
                return;
            }

            foreach (var item in parameters)
            {
                _parameters[item.Key] = item.Value;
            }

            this.Parameters = _parameters;
        }
    }
}
