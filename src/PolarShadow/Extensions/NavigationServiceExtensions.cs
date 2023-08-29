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
        public static void Navigate<TViewType>(this INavigationService nav, string containerName)
        {
            nav.Navigate(containerName, typeof(TViewType), default);
        }
    }
}
