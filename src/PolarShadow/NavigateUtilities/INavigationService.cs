using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal interface INavigationService
    {
        void Back();

        bool CanBack();

        void NavigateTo(NavigationState state, bool forceLoad = false);
    }

    internal static class NavigationExtensions
    {

        public static void NavigateTo(this INavigationService nav, string url, bool canBack = false, bool forceLoad = false)
        {
            NavigateTo(nav, url, default, canBack, forceLoad);
        }

        public static void NavigateTo(this INavigationService nav, string url, IDictionary<string, object> paramaters, bool canBack, bool forceLoad = false)
        {
            NavigateTo(nav, url, paramaters, default, canBack, forceLoad);
        }

        public static void NavigateTo(this INavigationService nav, string url, IDictionary<string, object> paramaters, IDictionary<string, object> states, bool canBack, bool forceLoad = false)
        {
            nav.NavigateTo(new NavigationState(url)
            {
                Parameters = paramaters,
                States = states,
                CanBack = canBack
            }, forceLoad);
        }
    }
}
