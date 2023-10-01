using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls.Converters
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public static readonly TimeSpanToDoubleConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts && targetType.IsAssignableTo(typeof(double)))
            {
                return ts.TotalSeconds;
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is double dVal && targetType.IsAssignableTo(typeof(TimeSpan)))
            {
                return TimeSpan.FromSeconds(dVal);
            }
            throw new NotSupportedException();
        }
    }
}
