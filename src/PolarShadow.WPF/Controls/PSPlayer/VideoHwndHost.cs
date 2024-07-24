using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace PolarShadow.Controls
{
    class VideoHwndHost : HwndHost
    {
        CustomWindow _window;
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            _window = new CustomWindow("static", hwndParent);
            return new HandleRef(this, _window.HWND);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _window?.Dispose();
            _window = null;
        }
    }
}
