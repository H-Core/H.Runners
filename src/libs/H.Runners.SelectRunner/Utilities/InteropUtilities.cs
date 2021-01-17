using System.ComponentModel;
using System.Runtime.InteropServices;

namespace H.Runners.Utilities
{
    internal static class InteropUtilities
    {
        /// <summary>
        /// Throws <see cref="Win32Exception"/> with <see cref="Marshal.GetLastWin32Error"/>
        /// if value is <see langword="false"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="Win32Exception"></exception>
        /// <returns></returns>
        public static void Check(this bool value)
        {
            if (!value)
            {
                ThrowWin32Exception();
            }
        }

        /// <summary>
        /// Throws <see cref="Win32Exception"/> with <see cref="Marshal.GetLastWin32Error"/>.
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        /// <returns>It always throws exception.</returns>
        public static void ThrowWin32Exception()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
