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
        public static AppBuilder UseNativeControls(this AppBuilder appBuilder)
        {
            NativeControls.AddHandler<IVideoViewHandler, VLCVideoViewHandler>();
            NativeControls.AddHandler<IWebViewHandler, WebViewHandler>();
            return appBuilder;
        }
    }
}
