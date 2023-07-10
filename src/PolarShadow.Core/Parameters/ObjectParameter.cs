using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
namespace PolarShadow.Core
{
    public class ObjectParameter : IObjectParameter
    {
        private readonly ICollection<ParameterValue> _objectParameters;      

        public ObjectParameter() : this(Enumerable.Empty<ParameterValue>())
        {
        }

        public ObjectParameter(params ParameterValue[] values) : this(new List<ParameterValue>(values))
        {

        }

        public ObjectParameter(IEnumerable<ParameterValue> objectParameters)
        {
            if (objectParameters != null)
            {
                foreach (ParameterValue value in objectParameters)
                {
                    Add(value);
                }
            }
            else
            {
                _objectParameters = new List<ParameterValue>();
            }
        }

        public string Name => "parameters";

        public void Add(ParameterValue value)
        {
            if (value.ValueKind != ParameterValueKind.Json && value.ValueKind != ParameterValueKind.Html)
            {
                throw new ArgumentException("just support json or html object");
            }
            _objectParameters.Add(value);
        }

        public bool TryGetValue(string path, out ParameterValue value)
        {
            value = default;
            if (ParameterValue.IsXPath(path) || ParameterValue.IsJsonPath(path))
            {
                foreach (var item in _objectParameters)
                {
                    if (ParameterValue.IsJsonPath(path) && item.ValueKind == ParameterValueKind.Json)
                    {
                        var result = JsonPath.Read(item.GetJson(), path);
                        if (result.ValueKind != JsonValueKind.Undefined)
                        {
                            value = new ParameterValue(result);
                        }
                    }
                    else if (ParameterValue.IsXPath(path) && item.ValueKind == ParameterValueKind.Html)
                    {
                        var result = item.GetHtml().Select(path[1..]);
                        if (result.ValueKind != HtmlValueKind.Undefined)
                        {
                            value = new ParameterValue(result);
                        }
                    }

                    if (value.ValueKind != ParameterValueKind.Undefined)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            foreach (var item in _objectParameters)
            {
                if (item.ValueKind == ParameterValueKind.Json)
                {
                    item.GetJson().WriteTo(writer);
                }
            }
        }
    }
}
