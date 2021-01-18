using System;
using System.Drawing;
using PInvoke;

namespace H.Runners.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class MouseUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsDesktop()
        {
            using var handle = User32.OpenInputDesktop(
                User32.DesktopCreationFlags.None,
                false,
                new Kernel32.ACCESS_MASK());

            return !handle.IsInvalid;
        }

        /// <summary>
        /// Returns cursor position, considering screen scales.
        /// </summary>
        /// <returns></returns>
        public static Point GetVirtualCursorPosition()
        {
            User32.GetCursorPos(out var point).Check();

            return new Point(point.x, point.y);
        }

        /// <summary>
        /// Returns cursor position, without considering screen scales.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static Point GetPhysicalCursorPosition(IntPtr handle)
        {
            User32.GetCursorPos(out var point).Check();

            //var hWnd = User32.WindowFromPhysicalPoint(point);
            //var visible = User32.IsWindowVisible(hWnd);
            //var awareness = User32.GetWindowDpiAwarenessContext(hWnd);
            //var dpiFromContext = User32.GetDpiFromDpiAwarenessContext(awareness);
            //var dpi = User32.GetDpiForWindow(hWnd);
            //var text = User32.GetWindowText(hWnd);
            //if (dpiFromContext > 0)
            //{
            //    var scaleFactor = dpiFromContext / 96.0;

            //    point = new POINT
            //    {
            //        x = (int)Math.Round(scaleFactor * point.x),
            //        y = (int)Math.Round(scaleFactor * point.y),
            //    };
            //}
            //var cursorPoint = point;

            User32.LogicalToPhysicalPointForPerMonitorDPI(handle, ref point).Check();

            // TODO: https://github.com/H-Core/H.Runners/issues/5
            // Trace.WriteLine($"{cursorPoint.x}, {cursorPoint.y}, {point.x}, {point.y}, {hWnd}, {dpi}, {dpiFromContext}, {text}, {visible}");

            return new Point(point.x, point.y);
        }
    }
}
