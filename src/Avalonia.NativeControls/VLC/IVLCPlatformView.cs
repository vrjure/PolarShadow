using Avalonia.NativeControls;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public interface IVLCPlatformView : IPlatformView
    {
        MediaPlayer MediaPlayer { get; set; }
    }
}
