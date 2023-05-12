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
        private XPathNavigator _xpathNav;

        private NameSlotValueKind _valueKind;

        public NameSlotValue(KeyValuePair<string, decimal> value)
        {
            _numberValue = value;
            _valueKind = NameSlotValueKind.Number;

            _jsonValue = default;
            _stringValue = default;
            _xpathNav = null;
        }

        public NameSlotValue(KeyValuePair<string, string> value)
        {
            _stringValue= value;
            _valueKind = NameSlotValueKind.String;
           
            _jsonValue = default;
            _numberValue = default;
            _xpathNav = null;
        }

        public NameSlotValue(JsonElement value)
        {
            _jsonValue = value;
            _valueKind = NameSlotValueKind.Json;

            _numberValue = default;
            _stringValue = default;
            _xpathNav = null;
        }

        public NameSlotValue(XPathNavigator value)
        {
            _xpathNav = value;
            _valueKind = NameSlotValueKind.XPathNav;
           
            _stringValue = default;
            _numberValue = default;
            _jsonValue = default;
        }

        public NameSlotValueKind ValueKind => _valueKind;

        public NameSlotValue ReadValue(string path)
        {
            
            if(path.StartsWith("$") && _valueKind == NameSlotValueKind.Json)
            {
                var value = JsonPath.Read(_jsonValue, path);
                return new NameSlotValue(value);
            }
            else if (path.StartsWith("/") && _valueKind == NameSlotValueKind.XPathNav)
            {
                var value = _xpathNav.Select(path[1..]);
            }
            return default;
        }

    }
}
