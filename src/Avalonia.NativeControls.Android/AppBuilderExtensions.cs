using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls.Android
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
