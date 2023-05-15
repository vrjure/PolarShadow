using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.XPath;

namespace PolarShadow.Core
{
    public struct NameSlotValue
    {
        private KeyValuePair<string, decimal> _numberValue;
        private JsonElement _jsonValue;
        private KeyValuePair<string, string> _stringValue;
        private HtmlElement _htmlValue;

        private NameSlotValueKind _valueKind;

        public NameSlotValue(KeyValuePair<string, decimal> value)
        {
            _numberValue = value;
            _valueKind = NameSlotValueKind.Number;

            _jsonValue = default;
            _stringValue = default;
            _htmlValue = default;
        }

        public NameSlotValue(KeyValuePair<string, string> value)
        {
            _stringValue= value;
            _valueKind = NameSlotValueKind.String;
           
            _jsonValue = default;
            _numberValue = default;
            _htmlValue = default;
        }

        public NameSlotValue(JsonElement value)
        {
            _jsonValue = value;
            _valueKind = NameSlotValueKind.Json;

            _numberValue = default;
            _stringValue = default;
            _htmlValue = default;
        }

        public NameSlotValue(HtmlElement value)
        {
            _htmlValue = value;
            _valueKind = NameSlotValueKind.Html;
           
            _stringValue = default;
            _numberValue = default;
            _jsonValue = default;
        }

        public NameSlotValueKind ValueKind => _valueKind;

        public static bool IsJsonPath(string path)
        {
            return path.StartsWith("$");
        }

        public static bool IsXPath(string path)
        {
            return path.StartsWith("/");
        }

        public NameSlotValue ReadValue(string path)
        {          
            if(path.StartsWith("$") && _valueKind == NameSlotValueKind.Json)
            {
                var value = JsonPath.Read(_jsonValue, path);
                return new NameSlotValue(value);
            }
            else if (path.StartsWith("/") && _valueKind == NameSlotValueKind.Html)
            {
                var value = _htmlValue.Select(path[1..]);
                return new NameSlotValue(value);
            }
            else if (_numberValue.Key == path || _stringValue.Key == path)
            {
                return this;
            }
            return default;
        }

        public string GetParameterName()
        {
            if (_valueKind == NameSlotValueKind.String)
            {
                return _stringValue.Key;
            }
            else if (_valueKind == NameSlotValueKind.Number)
            {
                return _numberValue.Key;
            }

            throw new InvalidOperationException("Can not get parameter name form json value or html value");
        }

        public string GetValue()
        {
            switch (_valueKind)
            {
                case NameSlotValueKind.Number:
                    return _numberValue.Value.ToString();
                case NameSlotValueKind.String:
                    return _stringValue.Value;
                case NameSlotValueKind.Json:
                    return GetJsonValue();
                case NameSlotValueKind.Html:
                    return GetHtmlValue();
                default:
                    break;
            }

            return null;
        }


        private string GetJsonValue()
        {
            if (_jsonValue.ValueKind == JsonValueKind.Object 
                || _jsonValue.ValueKind == JsonValueKind.Array)
            {
                throw new InvalidOperationException("Can not get a value form json object or json array");
            }

            switch (_jsonValue.ValueKind)
            {
                case JsonValueKind.String:
                    return _jsonValue.GetString();
                case JsonValueKind.Number:
                    return _jsonValue.GetDecimal().ToString();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return _jsonValue.GetBoolean().ToString();
                default:
                    break;
            }
            return null;
        }

        private string GetHtmlValue()
        {
            if (_htmlValue.ValueKind == HtmlValueKind.Nodes)
            {
                throw new InvalidOperationException("Can not get a value from html nodes");
            }

            if (_htmlValue.ValueKind == HtmlValueKind.Node)
            {
                return _htmlValue.GetValue();
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            var other = (NameSlotValue)obj;
            if (this.ValueKind != other._valueKind)
            {
                return false;
            }
            return this.GetValue() == other.GetValue();
        }

        public override int GetHashCode()
        {
            return $"{_valueKind}{GetValue()}".GetHashCode();
        }

    }
}
