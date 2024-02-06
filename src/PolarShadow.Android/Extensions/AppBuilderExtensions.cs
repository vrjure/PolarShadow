using Android.Content;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Android.Essentials;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Android
{
    internal static class AppBuilderExtensions
    {
        public static AppBuilder UseEssentials(this AppBuilder appBuilder, IServiceCollection services, Context context)
        {
            services.AddSingleton<IDeviceService>(sp =>
            {
                return new DeviceService(context);
            });

            return appBuilder;
        }
    }
}
