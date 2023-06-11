using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace PolarShadow.Core
{
    public class NameSlotValueCollection : IEnumerable<NameSlotValue>
    {
        private readonly IDictionary<string, NameSlotValue> _parameters;
        private readonly ICollection<NameSlotValue> _objectParameters;      

        public NameSlotValueCollection() : this(default)
        {
        }

        public NameSlotValueCollection(IDictionary<string, NameSlotValue> parameters)
        {
            if (parameters != null)
            {
                _parameters = new Dictionary<string, NameSlotValue>(parameters);
            }
            else
            {
                _parameters = new Dictionary<string, NameSlotValue>();
            }
            _objectParameters = new List<NameSlotValue>();
        }

        public void Add(NameSlotValue value)
        {
            _objectParameters.Add(value);
        }

        public void Add(decimal value) => Add(new NameSlotValue(value));

        public void Add(string value) => Add(new NameSlotValue(value));

        public void Add(bool value) => Add(new NameSlotValue(value));

        public void Add(JsonElement value) => Add(new NameSlotValue(value));

        public void Add(HtmlElement value) => Add(new NameSlotValue(value));

        public void Add(object value)
        {
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(value));
            Add(new NameSlotValue(doc.RootElement.Clone()));
        }

        public void AddNameValue(JsonElement value)
        {
            if (value.ValueKind == JsonValueKind.Array)
            {
                return;
            }

            if (value.ValueKind == JsonValueKind.Object)
            {
                foreach (var item in value.EnumerateObject())
                {
                    switch (item.Value.ValueKind)
                    {
                        case JsonValueKind.Object:
                        case JsonValueKind.Array:
                            AddNameValue(item.Name, item.Value);
                            break;
                        case JsonValueKind.String:
                            AddNameValue(item.Name, item.Value.GetString());
                            break;
                        case JsonValueKind.Number:
                            AddNameValue(item.Name, item.Value.GetDecimal());
                            break;
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            AddNameValue(item.Name, item.Value.GetBoolean());
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public void AddNameValue(object value)
        {
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(value));
            AddNameValue(new NameSlotValue(doc.RootElement.Clone()));
        }

        public void AddNameValue(string name, NameSlotValue value)
        {
            _parameters[name] = value;
        }

        public void AddNameValue(string name, decimal value) => AddNameValue(name, new NameSlotValue(value));
        public void AddNameValue(string name, string value) => AddNameValue(name, new NameSlotValue(value));
        public void AddNameValue(string name, bool value) => AddNameValue(name, new NameSlotValue(value));
        public void AddNameValue(string name, JsonElement value) => AddNameValue(name, new NameSlotValue(value));
        public void AddNameValue(string name, HtmlElement value) => AddNameValue(name, new NameSlotValue(value));
        public void Clear()
        {
            _parameters.Clear();
            _objectParameters.Clear();
        }

        public IEnumerator<NameSlotValue> GetEnumerator()
        {
            foreach (var item in _parameters)
            {
                yield return item.Value;
            }

            foreach (var item in _objectParameters)
            {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool ContainsName(string name)
        {
            return _parameters.ContainsKey(name);
        }

        public bool TryReadValue(string path, out NameSlotValue value)
        {
            value = default;
            if (NameSlotValue.IsXPath(path) || NameSlotValue.IsJsonPath(path))
            {
                foreach (var item in _objectParameters)
                {
                    if (NameSlotValue.IsJsonPath(path) && item.ValueKind == NameSlotValueKind.Json)
                    {
                        value = new NameSlotValue(JsonPath.Read(item.GetJson(), path));
                    }
                    else if (NameSlotValue.IsXPath(path) && item.ValueKind == NameSlotValueKind.Html)
                    {
                        value = new NameSlotValue(item.GetHtml().Select(path[1..]));
                    }

                    if (value.ValueKind != NameSlotValueKind.Undefined)
                    {
                        return true;
                    }
                }
                return false;
            }

            return _parameters.TryGetValue(path, out value);
        }

        public NameSlotValueCollection Clone()
        {
            return new NameSlotValueCollection(_parameters);
        }

        public bool TryReadValue<TValue>(string path, out TValue value)
        {
            if (TryReadValue(path, out NameSlotValue val))
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
                        if (val.GetString() is TValue v)
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
