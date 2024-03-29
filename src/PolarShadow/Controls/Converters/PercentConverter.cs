﻿using Avalonia.Data.Converters;
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
            if (!targetType.IsAssignableTo(typeof(double))) return value;
            var dParameter = 1d;
            if (parameter is string dp && double.TryParse(dp, out double dp1))
            {
                dParameter = dp1;
            }
            else
            {
                dParameter = (double)parameter;
            }
            return (double)value * dParameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
