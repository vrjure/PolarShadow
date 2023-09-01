using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public static class NavigationServiceExtensions
    {
        public static void Navigate(this INavigationService nav, string containerName, Type viewType, IDictionary<string, object> parameters = null, bool canBack = false)
        {
            nav.Navigate(containerName, viewType, parameters, canBack);
        }

        public static void Navigate<TViewType>(this INavigationService nav, string containerName, IDictionary<string, object> parameters = null, bool canBack = false)
        {
            nav.Navigate(containerName, typeof(TViewType), parameters, canBack);
        }
    }
}
