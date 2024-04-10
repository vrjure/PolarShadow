using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PolarShadow.Controls
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    class StringToVisibility : IValueConverter
    {
        public static StringToVisibility Instance = new StringToVisibility();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (string)value;
            return BoolToVisibility.Instance.Convert(!string.IsNullOrEmpty(s), targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
