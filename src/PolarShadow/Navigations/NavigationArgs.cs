using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public class NavigationArgs : RoutedEventArgs
    {
        public Control From { get; }
        public Control To { get; }
    }
}
