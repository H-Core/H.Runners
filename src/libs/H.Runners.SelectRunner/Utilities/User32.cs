using System.Drawing;
using System.Runtime.InteropServices;

namespace H.Runners.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    internal static class User32
    {
        /// <summary>
        /// Retrieves the position of the mouse cursor, in screen coordinates.
        /// </summary>
        /// <param name="lpPoint">
        /// A pointer to a POINT structure that receives the screen coordinates of the cursor.
        /// </param>
        /// <returns>
        /// Returns nonzero if successful or zero otherwise.
        /// To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// The cursor position is always specified in screen coordinates
        /// and is not affected by the mapping mode of the window that contains the cursor. <br/>
        /// The calling process must have WINSTA_READATTRIBUTES access to the window station. <br/>
        /// The input desktop must be the current desktop when you call GetCursorPos.
        /// Call OpenInputDesktop to determine whether the current desktop is the input desktop.
        /// If it is not, call SetThreadDesktop with the HDESK returned by OpenInputDesktop
        /// to switch to that desktop.
        /// </remarks>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);
    }
}
