using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.Controls
{
    public interface IVirtualView
    {
        IViewHandler Handler { get; }
    }
}
