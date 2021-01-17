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
        public static Point GetCursorPosition()
        {
            User32.GetCursorPos(out var point).Check();

            return new Point(point.x, point.y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dpi"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static Point GetCursorPosition(double dpi, IntPtr handle)
        {
            User32.GetCursorPos(out var point).Check();

            User32.LogicalToPhysicalPointForPerMonitorDPI(handle, ref point);

            return new Point((int)(dpi * point.x), (int)(dpi * point.y));
        }
    }
}
