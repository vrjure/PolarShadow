﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PolarShadow.Controls
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BoolToVisibility : IValueConverter
    {
        public static readonly BoolToVisibility Instance = new BoolToVisibility();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility)value;
            return visibility == Visibility.Visible ? true : false;
        }
    }
}
