using System;
using System.Diagnostics;

namespace H.Runners.Utilities
{
    internal static class User32Utilities
    {
        public static int GetWindowProcessId(IntPtr hWnd)
        {
            User32Methods.GetWindowThreadProcessId(hWnd, out var pid);

            return pid;
        }

        public static User32Methods.Rect GetWindowRect(IntPtr hWnd)
        {
            var rect = new User32Methods.Rect();
            User32Methods.GetWindowRect(hWnd, ref rect);

            return rect;
        }

        public static Process GetForegroundProcess()
        {
            var hWnd = User32Methods.GetForegroundWindow();
            var id = GetWindowProcessId(hWnd);

            return Process.GetProcessById(id);
        }
        
        public static bool AreApplicationFullScreen()
        {
            var desktopHandle = User32Methods.GetDesktopWindow();
            var shellHandle = User32Methods.GetShellWindow();

            var hWnd = User32Methods.GetForegroundWindow();
            if (hWnd.Equals(IntPtr.Zero))
            {
                return false;
            }

            if (hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle))
            {
                return false;
            }

            var appBounds = GetWindowRect(hWnd);
            return Math.Abs(appBounds.Bottom - appBounds.Top - System.Windows.SystemParameters.VirtualScreenHeight) < 1 && 
                   Math.Abs(appBounds.Right - appBounds.Left - System.Windows.SystemParameters.VirtualScreenWidth) < 1;
        }
    }
}
