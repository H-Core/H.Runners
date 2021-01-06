using System;
using System.Diagnostics;

namespace H.Runners.Utilities
{
    internal static class User32Utilities
    {
        public static int GetWindowProcessId(IntPtr hWnd)
        {
            var _ = User32Methods.GetWindowThreadProcessId(hWnd, out var pid);

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
    }
}
