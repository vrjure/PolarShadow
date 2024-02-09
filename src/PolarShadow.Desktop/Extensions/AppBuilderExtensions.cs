using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Desktop.Essentials;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Desktop
{
    internal static class AppBuilderExtensions
    {
        public static AppBuilder UseEssentials(this AppBuilder appBuilder, IServiceCollection services)
        {
            services.AddSingleton<IDeviceService>(sp =>
            {
                return new DeviceService();
            });

            return appBuilder;
        }
    }
}
