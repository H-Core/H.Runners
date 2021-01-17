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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dpi"></param>
        /// <param name="handle"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static Rectangle ToPhysical(this Rectangle rectangle, double dpi, IntPtr handle)
        {
            var point1 = new POINT
            {
                x = (int)(rectangle.Left / dpi), 
                y = (int)(rectangle.Top / dpi)
            };
            var point2 = new POINT
            {
                x = (int)(rectangle.Right / dpi), 
                y = (int)(rectangle.Bottom / dpi)
            };
            User32.LogicalToPhysicalPointForPerMonitorDPI(handle, ref point1);
            User32.LogicalToPhysicalPointForPerMonitorDPI(handle, ref point2);

            return new Rectangle(point1, new Size(
                point2.x - point1.x, 
                point2.y - point1.y));
        }
    }
}
