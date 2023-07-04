using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class ParametersBuilder : IParametersBuilder
    {
        private readonly Dictionary<string, object> _parameters;
        private readonly IPolarShadowOptionBuilder _optionBuilder;

        public ParametersBuilder(IPolarShadowOptionBuilder builder):this(default, builder) { }

        public ParametersBuilder(Dictionary<string, object> parameter, IPolarShadowOptionBuilder optionBuilder)
        {
            _parameters = parameter ?? new Dictionary<string, object>();
            _optionBuilder = optionBuilder;
        }
        public IParametersBuilder AddParameter<T>(string name, T value)
        {
            _parameters[name] = value;
            _optionBuilder?.ChangeNodify();
            return this;
        }

        public IParametersBuilder RemoveParameter(string name)
        {
            _parameters.Remove(name);
            _optionBuilder?.ChangeNodify();
            return this;
        }

        public IParametersBuilder SetParameter(string parameters)
        {
            var param = JsonSerializer.Deserialize<Dictionary<string, object>>(parameters);
            _parameters.Clear();
            foreach (var item in param)
            {
                _parameters[item.Key] = item.Value;
            }
            _optionBuilder?.ChangeNodify();
            return this;
        }

        public void WriteTo(Stream output)
        {
            using var jsonWriter = new Utf8JsonWriter(output, JsonOption.DefaultWriteOption);
            var doc = JsonDocument.Parse(JsonSerializer.Serialize(_parameters, JsonOption.DefaultSerializer));
            doc.WriteTo(jsonWriter);
            jsonWriter.Flush();
        }
    }
}
