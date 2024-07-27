using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PolarShadow.Controls
{
    [ValueConversion(typeof(int), typeof(bool))]
    [ValueConversion(typeof(string), typeof(bool))]
    class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return string.IsNullOrEmpty(str);
            }

            var type = value?.GetType();
            if (type == null)
            {
                return false;
            }
            return type.IsValueType && type.IsPrimitive && (double)value != 0;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
