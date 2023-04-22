using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public readonly struct JsonPathValue
    {
        private readonly JsonElement _json;
        private readonly decimal _number;
        private readonly string _str;
        private readonly bool _boolean;
        private readonly JsonPathValueType _valueKind;
        internal JsonPathValue(JsonElement json)
        {
            _json = json;
            _valueKind = JsonPathValueType.Json;
            _number = 0; 
            _str = string.Empty;
            _boolean = false;
        }

        internal JsonPathValue(decimal number)
        {
            _number = number;
            _valueKind = JsonPathValueType.Number;

            _str = string.Empty;
            _boolean = false;
            _json = default;
        }

        internal JsonPathValue(bool boolean)
        {
            _boolean = boolean;
            _valueKind = JsonPathValueType.Boolean;

            _str = string.Empty;
            _json = default;
            _number = 0;
        }

        internal JsonPathValue(string str)
        {
            _str = str;
            _valueKind = JsonPathValueType.String;

            _json = default;
            _number = 0;
            _boolean = false;
        }

        public static bool operator ==(JsonPathValue left, JsonPathValue right)
        {
            if (left._valueKind != right._valueKind || left._valueKind == JsonPathValueType.Undefined || right._valueKind == JsonPathValueType.Undefined)
            {
                return false;
            }

            return left._valueKind switch
            {
                JsonPathValueType.Boolean => left._boolean == right._boolean,
                JsonPathValueType.Number => left._number == right._number,
                JsonPathValueType.String => left._str == right._str,
                _ => false
            };
        }

        public static bool operator !=(JsonPathValue left, JsonPathValue right)
        {
            return !(left == right);
        }
    }
}
