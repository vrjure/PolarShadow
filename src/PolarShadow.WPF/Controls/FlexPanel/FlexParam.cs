using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    internal abstract class FlexParam
    {
        private const string _percent = "%";
        private string _value;
        private double _doubleValue;

        private bool _isPercent = false;
        private bool _isNumber = false;
        public FlexParam(string value)
        {
            _value = value;
            Deal(out _doubleValue);
        }

        public bool IsNumber => _isNumber;
        public bool IsPercent => _isPercent;
        public double GetDouble()
        {
            if (_isNumber || _isPercent) return _doubleValue;
            return double.NaN;
        }

        public string GetString()
        {
            return _value;
        }

        public override bool Equals(object obj)
        {
            var p = obj as FlexParam;
            if (p == null)
            {
                return false;
            }
            return _value.Equals(p._value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        private void Deal(out double value)
        {
            value = double.NaN;
            if (string.IsNullOrEmpty(_value)) return;

            if (_value.EndsWith(_percent) && double.TryParse(_value[..^1], out value))
            {
                _isPercent = true;
                value /= 100;
                return;
            }
            else if (double.TryParse(_value, out value))
            {
                _isNumber = true;
            }
        }
    }
}
