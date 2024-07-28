using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PolarShadow.Controls
{
    [ValueConversion(typeof(string), typeof(double))]
    [ValueConversion(typeof(double), typeof(string))]
    class NumericStringConverter : IValueConverter
    {
        public static readonly NumericStringConverter Instance = new NumericStringConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value.GetType();
            if (value is string str && targetType.IsAssignableTo(typeof(double)))
            {
                if(double.TryParse(str,out double val))
                {
                    return val;
                }
            }
            else if(type.IsValueType && type.IsPrimitive && targetType.IsAssignableTo(typeof(string)))
            {
                return value.ToString();
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value.GetType();
            if (value is string str && targetType.IsAssignableTo(typeof(double)))
            {
                if (double.TryParse(str, out double val))
                {
                    return val;
                }
            }
            else if (type.IsValueType && type.IsPrimitive && targetType.IsAssignableTo(typeof(string)))
            {
                return value.ToString();
            }
            return 0;
        }
    }
}
