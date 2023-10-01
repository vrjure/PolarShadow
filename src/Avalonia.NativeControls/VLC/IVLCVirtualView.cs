using Avalonia.NativeControls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public interface IVLCVirtualView : IVirtualView
    {
        new IVLCHandler Handler { get; set; }
    }
}
