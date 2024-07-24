using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolarShadow.Controls
{
    static class PInvoke
    {
        public const string User32 = "user32.dll";
        public const string GDI32 = "gdi32.dll";
        public const uint WM_ERASEBKGND = 0x0014;
        public const uint WM_PAINT = 0x000F;

        /// <summary>
        /// Provide the win32 window stype when call <see cref="CreateWindowEx"/>
        /// <para>See following link: <a href="https://docs.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles" /> </para>
        /// </summary>
        [Flags]
        internal enum ExtendedWindow32Styles : int
        {
            /// <summary>
            /// The window should not be painted until siblings beneath the window (that were created by the same thread) have been painted.
            /// The window appears transparent because the bits of underlying sibling windows have already been painted.
            /// </summary>
            WS_EX_TRANSPARENT = 0x00000020
        }

        /// <summary>
        /// Provide the win32 window stype when call <see cref="CreateWindowEx"/>
        /// <para>See following link: <a href="https://docs.microsoft.com/en-us/windows/win32/winmsg/window-styles" /> </para>
        /// </summary>
        [Flags]
        internal enum Window32Styles : int
        {
            /// <summary>
            /// The window is a child window. A window with this style cannot have a menu bar.
            /// </summary>
            WS_CHILD = 0x40000000,

            /// <summary>
            /// The window is initially visible.
            /// </summary>
            WS_VISIBLE = 0x10000000
        }


        [DllImport(User32)]
        internal static extern IntPtr CreateWindowEx(ExtendedWindow32Styles dwExStyle,
                string lpszClassName,
                string lpszWindowName,
                Window32Styles style,
                int x, int y, int width, int height,
                IntPtr hwndParent,
                IntPtr hMenu,
                IntPtr hInst,
                IntPtr lpParam);


        [DllImport(User32)]
        internal static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport(User32, SetLastError = true)]
        internal static extern IntPtr DefWindowProcW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport(User32, SetLastError = true)]
        internal static extern UInt16 RegisterClassW([In] ref WNDCLASS lpWndClass);

        [DllImport(GDI32, EntryPoint = "CreateSolidBrush", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateSolidBrush(uint theColor);

        [DllImport(User32)]
        internal static extern bool GetClientRect(IntPtr hWnd, ref LPRECT lpRect);

        [DllImport(GDI32)]
        internal static extern uint SetBkColor(IntPtr hdc, uint color);

        [DllImport(User32)]
        internal static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lPaint);

        [DllImport(User32)]
        internal static extern bool EndPaint(IntPtr hwnd, [In] ref PAINTSTRUCT lPaint);
        
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WNDCLASS
    {
        public readonly uint style;

        public IntPtr lpfnWndProc;

        public readonly int cbClsExtra;

        public readonly int cbWndExtra;

        public readonly IntPtr hInstance;

        public readonly IntPtr hIcon;

        public readonly IntPtr hCursor;

        public IntPtr hbrBackground;

        [MarshalAs(UnmanagedType.LPWStr)]
        public readonly string lpszMenuName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LPRECT
    {
        public long left;
        public long top;
        public long right;
        public long bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public bool fErase;
        public LPRECT rcPaint;
        public bool fRestore;
        public bool fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
    }
}
