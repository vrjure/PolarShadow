using Avalonia.Controls;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal static class TopLevelServiceExtensions
    {
        public static void FullScreen(this ITopLevelService topLevel)
        {
            var top = topLevel.GetTopLevel();
            if (top is Window window)
            {
                window.WindowState = WindowState.FullScreen;
            }
        }

        public static void ExitFullScreen(this ITopLevelService topLevel)
        {
            var top = topLevel.GetTopLevel();
            if (top is Window window)
            {
                window.WindowState = WindowState.Normal;
            }
        }
    }
}
