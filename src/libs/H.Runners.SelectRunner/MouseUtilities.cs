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

            return new System.Drawing.Point(point.X, point.Y);
        }
    }
}
