using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public interface IVirtualView
    {
        IViewHandler Handler { get; }
    }
}
