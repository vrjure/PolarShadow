using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PolarShadow.Core
{
    internal class PolarShadowOptionBuilder : IPolarShadowOptionBuilder
    {
        private JsonObject _option = new JsonObject();
        private IParametersBuilder _parameterBuilder;
        public PolarShadowOptionBuilder()
        {
            _parameterBuilder = new ParametersBuilder(this);
        }

        public bool IsChanged { get; internal set; }

        public IParametersBuilder Parameters => _parameterBuilder;

        public void ChangeNodify()
        {
            IsChanged = true;
        }   

        public IPolarShadowOptionBuilder Load(Stream stream)
        {
            var node = JsonNode.Parse(stream, JsonOption.DefaultNodeOption);
            _option = node.AsObject();
            ChangeNodify();
            return this;
        }

        public void WriteTo(Stream stream)
        {
            JsonSerializer.Serialize(stream, _option, JsonOption.DefaultSerializer);
        }

        public IPolarShadowOptionBuilder AddOptions<T>(string name, IEnumerable<T> options)
        {
            if (!_option.TryGetPropertyValue(name, out JsonNode node))
            {
                node = new JsonArray(JsonOption.DefaultNodeOption);
                _option.Add(name, node);
            }

            if(!(node is JsonArray array))
            {
                throw new InvalidOperationException($"{name} value must be a JsonArray");
            }

            foreach (var option in options)
            {
                array.Add(option);
            }

            ChangeNodify();
            return this;
        }

        public IPolarShadowOptionBuilder AddOption<T>(string name, T option)
        {
            if (_option.TryGetPropertyValue(name, out JsonNode node))
            {
                if (node is JsonArray array)
                {
                    array.Add(option);
                    ChangeNodify();
                    return this;
                }
            }

            _option[name] = JsonNode.Parse(JsonSerializer.Serialize(option, JsonOption.DefaultSerializer));
            ChangeNodify();
            return this;
        }

        public IPolarShadowOptionBuilder RemoveOption(string name)
        {
            _option.Remove(name);
            return this;
        }

        public T GetOption<T>(string name)
        {
            if(_option.TryGetPropertyValue(name, out JsonNode node))
            {
                return JsonSerializer.Deserialize<T>(node, JsonOption.DefaultSerializer);
            }
            return default;
        }

        public bool ContainKey(string name)
        {
            return _option.ContainsKey(name);
        }

        public bool TryGetOption<T>(string name, out T option)
        {
            option = default;
            if (_option.TryGetPropertyValue(name, out JsonNode node))
            {
                option = JsonSerializer.Deserialize<T>(node, JsonOption.DefaultSerializer);
                return true;
            }
            return false;
        }
    }
}
