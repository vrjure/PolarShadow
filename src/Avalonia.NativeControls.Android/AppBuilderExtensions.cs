using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls.Android
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder UseNativeControls(this AppBuilder appBuilder)
        {
            NativeControlHandlers.AddHandler<IVLCHandler, VideoViewHandler>();
            NativeControlHandlers.AddHandler<IWebViewHandler, WebViewHandler>();
            return appBuilder;
        }
    }
}
