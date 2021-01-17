using System;
using System.Windows;

namespace H.Runners.Extensions
{
    /// <summary>
    /// <see cref="Window"/> extensions.
    /// </summary>
    internal static class WindowExtensions
    {
        /// <summary>
        /// Returns window dpi.
        /// </summary>
        /// <param name="window"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>Returns window dpi scale factory.</returns>
        public static double GetDpi(this Window window)
        {
            window = window ?? throw new ArgumentNullException(nameof(window));

            var source = PresentationSource.FromVisual(window);

            return source?.CompositionTarget?.TransformFromDevice.M11 ?? 1.0;
        }
    }
}
