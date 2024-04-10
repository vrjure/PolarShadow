using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PolarShadow.Controls
{
    [ValueConversion(typeof(double), typeof(double))]
    class TextToIconSizeConverter : IValueConverter
    {
        public static readonly TextToIconSizeConverter Instance = new TextToIconSizeConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fontSize = (double)value;
            return fontSize + 5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var iconSize = (double)value;
            return iconSize - 5;
        }
    }
}
