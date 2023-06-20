using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.XPath;

namespace PolarShadow.Core
{
    public struct NameSlotValue
    {
        private decimal _numberValue;
        private JsonElement _jsonValue;
        private string _stringValue;
        private HtmlElement _htmlValue;
        private bool _booleanValue;

        private NameSlotValueKind _valueKind;

        public NameSlotValue(decimal value)
        {
            _numberValue = value;
            _valueKind = NameSlotValueKind.Number;

            _jsonValue = default;
            _stringValue = default;
            _htmlValue = default;
            _booleanValue = default;
        }

        public NameSlotValue(string value)
        {
            _stringValue= value;
            _valueKind = NameSlotValueKind.String;
           
            _jsonValue = default;
            _numberValue = default;
            _htmlValue = default;
            _booleanValue = default;
        }

        public NameSlotValue(bool value)
        {
            _booleanValue = value;
            _valueKind = NameSlotValueKind.Boolean;

            _jsonValue = default;
            _stringValue = default;
            _htmlValue = default;
            _numberValue = default;
        }

        public NameSlotValue(JsonElement value)
        {
            _jsonValue = value;
            _valueKind = NameSlotValueKind.Json;

            _numberValue = default;
            _stringValue = default;
            _htmlValue = default;
            _booleanValue = default;
        }

        public NameSlotValue(HtmlElement value)
        {
            _htmlValue = value;
            _valueKind = NameSlotValueKind.Html;
           
            _stringValue = default;
            _numberValue = default;
            _jsonValue = default;
            _booleanValue = default;
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

        public string GetValue()
        {
            switch (_valueKind)
            {
                case NameSlotValueKind.Number:
                    return _numberValue.ToString();
                case NameSlotValueKind.String:
                    return _stringValue;
                case NameSlotValueKind.Json:
                    return GetJsonValue();
                case NameSlotValueKind.Html:
                    return GetHtmlValue();
                default:
                    break;
            }

            return null;
        }

        public NameSlotValue GetValue(string path)
        {
            return _valueKind switch
            {
                 NameSlotValueKind.Json => new NameSlotValue(GetJson().Read(path)),
                 NameSlotValueKind.Html => new NameSlotValue(GetHtml().Select(path)),
                 _ => default
            };
        }


        public decimal GetDecimal()
        {
            if (_valueKind == NameSlotValueKind.Number)
                return _numberValue;

            throw new InvalidOperationException();
        }

        public int GetInt32()
        {
            return (int)GetDecimal();
        }

        public short GetInt16()
        {
            return (short)GetDecimal();
        }

        public long GetInt64()
        {
            return (long)GetDecimal();
        }

        public float GetFloat()
        {
            return (float)GetDecimal();
        }

        public double GetDouble()
        {
            return (double)GetDecimal();
        }

        public string GetString()
        {
            if (_valueKind == NameSlotValueKind.String)
                return _stringValue;

            throw new InvalidOperationException();
        }

        public bool GetBoolean()
        {
            if (_valueKind == NameSlotValueKind.Boolean)
                return _booleanValue;
            throw new InvalidOperationException();
        }

        public JsonElement GetJson()
        {
            if (_valueKind == NameSlotValueKind.Json) return _jsonValue;

            throw new InvalidOperationException();
        }

        public HtmlElement GetHtml()
        {
            if (_valueKind == NameSlotValueKind.Html) return _htmlValue;

            throw new InvalidOperationException();
        }

        private string GetJsonValue()
        {
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
                    return _jsonValue.GetRawText();
            }
        }

        private string GetHtmlValue()
        {
            if (_htmlValue.ValueKind == HtmlValueKind.Nodes)
            {
                throw new InvalidOperationException($"Can not get a value from html nodes: {_htmlValue}");
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
