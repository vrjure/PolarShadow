using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    class CustomWindow : IDisposable
    {
        private const int ERROR_CLASS_ALREADY_EXISTS = 1410;

        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private bool m_disposed;
        private IntPtr m_hwnd;
        private WndProc m_wnd_proc_delegate;

        public CustomWindow(string class_name, HandleRef hwndParent)
        {
            if (class_name == null)
            {
                throw new Exception("class_name is null");
            }
            if (class_name == String.Empty)
            {
                throw new Exception("class_name is empty");
            }

            this.m_wnd_proc_delegate = CustomWndProc;

            // Create WNDCLASS
            var wind_class = new WNDCLASS();
            wind_class.lpszClassName = class_name;
            wind_class.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(this.m_wnd_proc_delegate);
            wind_class.hbrBackground = PInvoke.CreateSolidBrush(0);

            var class_atom = PInvoke.RegisterClassW(ref wind_class);

            var last_error = Marshal.GetLastWin32Error();

            if (class_atom == 0 && last_error != ERROR_CLASS_ALREADY_EXISTS)
            {
                throw new Exception("Could not register window class");
            }

            // Create window
            this.m_hwnd = PInvoke.CreateWindowEx(
              PInvoke.ExtendedWindow32Styles.WS_EX_TRANSPARENT,
              class_name,
              String.Empty,
              PInvoke.Window32Styles.WS_CHILD | PInvoke.Window32Styles.WS_VISIBLE,
              0,
              0,
              0,
              0,
              hwndParent.Handle,
              IntPtr.Zero,
              IntPtr.Zero,
              IntPtr.Zero);
        }

        public IntPtr HWND => this.m_hwnd;

        private static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case PInvoke.WM_ERASEBKGND:
                    PInvoke.SetBkColor(wParam, 0);
                    return 1;
                case PInvoke.WM_PAINT:
                    PInvoke.BeginPaint(hWnd, out PAINTSTRUCT ps1);
                    PInvoke.SetBkColor(wParam, 0);
                    PInvoke.EndPaint(hWnd, ref ps1);
                    return 0;
                default:
                    return PInvoke.DefWindowProcW(hWnd, msg, wParam, lParam);
            }
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.m_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                }

                // Dispose unmanaged resources
                if (this.m_hwnd != IntPtr.Zero)
                {
                    PInvoke.DestroyWindow(this.m_hwnd);
                    this.m_hwnd = IntPtr.Zero;
                }
                this.m_disposed = true;
            }
        }
    }
}
