using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls.Converters
{
    public class RowColumnConverter : IValueConverter
    {
        public static readonly RowColumnConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!targetType.IsAssignableTo(typeof(int)) || value == null) return 1;

            var p = 1d;
            if (parameter is string ps && double.TryParse(ps, out double dps))
            {
                p = dps;
            }
            else
            {
                p = (double)parameter;
            }

            var count = (int)((double)value / p);
            return count == 0 ? 1 : count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
