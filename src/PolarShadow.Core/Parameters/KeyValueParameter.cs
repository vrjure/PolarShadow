using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class KeyValueParameter : IKeyValueParameter
    {
        private readonly Dictionary<string, ParameterValue> _parameters;

        public KeyValueParameter()
        {
            _parameters = new Dictionary<string, ParameterValue>();
        }

        public string Name => "parameters";

        public void Add(string key, ParameterValue value)
        {
            _parameters.Add(key, value);
        }

        public bool TryGetValue(string key, out ParameterValue value)
        {
            return _parameters.TryGetValue(key, out value);
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            foreach (var item in _parameters)
            {
                switch (item.Value.ValueKind)
                {
                    case ParameterValueKind.Number:
                        writer.WriteNumber(item.Key, item.Value.GetDecimal());
                        break;
                    case ParameterValueKind.String:
                        writer.WriteString(item.Key, item.Value.GetString());
                        break;
                    case ParameterValueKind.Json:
                        writer.WritePropertyName(item.Key);
                        item.Value.GetJson().WriteTo(writer);
                        break;
                    case ParameterValueKind.Boolean:
                        writer.WriteBoolean(item.Key, item.Value.GetBoolean());
                        break;
                    default:
                        break;
                }
            }
            writer.WriteEndObject();
        }
    }
}
