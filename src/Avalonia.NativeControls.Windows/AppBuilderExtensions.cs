using Avalonia.NativeControls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls.Windows
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder UseNativeControls(this AppBuilder appBuilder)
        {
            NativeControlHandlers.AddHandler<IVLCHandler, VideoViewHandler>();
            return appBuilder;
        }
    }
}
