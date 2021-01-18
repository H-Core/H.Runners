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

            // TODO: https://github.com/H-Core/H.Runners/issues/5
            //Trace.WriteLine($"{point.x}, {point.y}");

            User32.LogicalToPhysicalPointForPerMonitorDPI(handle, ref point).Check();

            return new Point(point.x, point.y);
        }

        /// <summary>
        /// Converts physical point to app point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static Point ToApp(this Point point, double scaleFactor)
        {
            return new (
                (int)Math.Round(scaleFactor * point.X), 
                (int)Math.Round(scaleFactor * point.Y));
        }

        /// <summary>
        /// Converts app point to physical point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static Point ToPhysical(this Point point, double scaleFactor)
        {
            return new(
                (int)Math.Round(point.X / scaleFactor), 
                (int)Math.Round(point.Y / scaleFactor)
                );
        }

        /// <summary>
        /// Converts app rectangle to physical rectangle.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static Rectangle ToPhysical(this Rectangle rectangle, double scaleFactor)
        {
            var point1 = new Point(rectangle.Left, rectangle.Top)
                .ToPhysical(scaleFactor);
            var point2 = new Point(rectangle.Right, rectangle.Bottom)
                .ToPhysical(scaleFactor);

            return Rectangle.FromLTRB(
                point1.X, 
                point1.Y,
                point2.X, 
                point2.Y);
        }
    }
}
