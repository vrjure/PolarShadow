using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public class NameSlotValueCollection : IEnumerable<NameSlotValue>
    {
        private readonly IDictionary<string, NameSlotValue> _parameters;
        private readonly ICollection<NameSlotValue> _objectParameters;

        public IReadOnlyDictionary<string, NameSlotValue> Parameters => new ReadOnlyDictionary<string, NameSlotValue>(_parameters);

        public NameSlotValueCollection() : this(default)
        {
        }

        public NameSlotValueCollection(IDictionary<string, NameSlotValue> parameters)
        {
            _parameters = parameters ?? new Dictionary<string, NameSlotValue>();
            _objectParameters = new List<NameSlotValue>();
        }

        public void Add(NameSlotValue value)
        {
            switch (value.ValueKind)
            {
                case NameSlotValueKind.Number:
                case NameSlotValueKind.String:
                    _parameters[value.GetParameterName()] = value;
                    return;
                case NameSlotValueKind.Json:
                case NameSlotValueKind.Html:
                    _objectParameters.Add(value);
                    return;
                default:
                    throw new InvalidOperationException("Invalid value");
            }
        }

        public void Add(KeyValuePair<string, decimal> value) => Add(new NameSlotValue(value));

        public void Add(KeyValuePair<string, string> value) => Add(new NameSlotValue(value));

        public void Add(JsonElement value) => Add(new NameSlotValue(value));

        public void Add(HtmlElement value) => Add(new NameSlotValue(value));

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

        public bool TryReadValue(string path, out NameSlotValue value)
        {
            value = default;
            if (NameSlotValue.IsXPath(path) || NameSlotValue.IsJsonPath(path))
            {
                foreach (var item in _objectParameters)
                {
                    value = item.ReadValue(path);
                    if (value.ValueKind == NameSlotValueKind.Undefined)
                    {
                        continue;
                    }

                    return true;
                }
                return false;
            }

            return _parameters.TryGetValue(path, out value);
        }
    }
}
