using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;

namespace Avalonia.Controls.Windows
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
