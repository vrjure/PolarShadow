using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolarShadow
{
    internal static class AssetsExtensions
    {
        public static T FindResource<T>(this FrameworkElement element, string key)
        {
            return (T)element.FindResource(key);
        }
    }
}
