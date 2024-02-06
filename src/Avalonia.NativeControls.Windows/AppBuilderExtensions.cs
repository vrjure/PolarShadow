using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;

namespace Avalonia.Controls.Windows
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder UseNativeControls(this AppBuilder appBuilder, IServiceCollection services)
        {
            services.AddTransient<IVideoViewHandler, VLCVideoViewHandler>();
            services.AddTransient<IWebViewHandler, WebViewHandler>();
            return appBuilder;
        }
    }
}
