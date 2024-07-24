using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    internal static class DispatcherUI
    {
        public static IDispatcherUI UI => Ioc.Default.GetRequiredService<IDispatcherUI>();
    }
}
