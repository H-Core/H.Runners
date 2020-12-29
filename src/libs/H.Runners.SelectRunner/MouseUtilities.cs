using System.ComponentModel;
using System.Runtime.InteropServices;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public static class MouseUtilities
    {
        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;
            public int Y;
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static System.Drawing.Point GetCursorPosition()
        {
            GetCursorPos(out var point);
            var dpi = GetDpi();
            return new System.Drawing.Point((int)(point.X / dpi), (int)(point.Y / dpi));
        }

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(nint hWnd, nint hDC);

        [DllImport("user32.dll")]
        private static extern nint GetDC(nint hwnd);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(nint hdc, int nIndex);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static double GetDpi()
        {
            var desktopWnd = (nint)0;
            var dc = GetDC(desktopWnd);
            if (dc == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            const int logpixelsx = 88;
            var value = GetDeviceCaps(dc, logpixelsx) / 96.0;
            if (ReleaseDC(desktopWnd, dc) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return value;
        }
    }
}
