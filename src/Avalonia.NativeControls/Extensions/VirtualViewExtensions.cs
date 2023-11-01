using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls
{
    public static class VirtualViewExtensions
    {
        public static NativeControlHost AsHost(this IVirtualView virtualView)
        {
            return virtualView as NativeControlHost;
        }
    }
}
