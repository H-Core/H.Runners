using System;
using System.Runtime.InteropServices;

namespace H.Runners.Utilities
{
    internal static class User32Methods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref Rect rect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(HandleRef hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(HandleRef hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(HandleRef hwnd);

    }
}
