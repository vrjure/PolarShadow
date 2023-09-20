using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls.Converters
{
    public class PercentConverter : IValueConverter
    {
        public static readonly PercentConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType.IsAssignableTo(typeof(double)) 
                && value is string sourceValue 
                && parameter is string parameterValue
                && double.TryParse(sourceValue, out var dSource)
                && double.TryParse(parameterValue, out var dParemeter))
            {
                return dSource * dParemeter;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
