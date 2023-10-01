using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public interface IVirtualView
    {
        Control VirtualView { get; }
        IViewHandler Handler { get; set; }
    }
}
