using System.Drawing;

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

            return new Point(point.X, point.Y);
        }
    }
}
