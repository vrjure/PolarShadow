using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PolarShadow.Controls
{
    [ValueConversion(typeof(TimeSpan), typeof(double))]
    class TimeSpanToSeconds : IValueConverter
    {
        public static readonly TimeSpanToSeconds Instance = new TimeSpanToSeconds();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TimeSpan)value).TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromSeconds((double)value);
        }
    }
}
