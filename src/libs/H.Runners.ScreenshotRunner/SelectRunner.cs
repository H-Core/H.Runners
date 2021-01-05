using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;
using H.Utilities;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ScreenshotRunner : Runner
    {
        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<Image>? Captured;

        private void OnCaptured(Image value)
        {
            Captured?.Invoke(this, value);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ScreenshotRunner()
        {
            Add(new AsyncAction("screenshot", async (command, cancellationToken) =>
            {
                var arguments = command.Input.Arguments;
                var rectangle = arguments.Length < 4
                    ? (Rectangle?)null
                    : new Rectangle(
                        Convert.ToInt32(arguments[0], CultureInfo.InvariantCulture),
                        Convert.ToInt32(arguments[1], CultureInfo.InvariantCulture),
                        Convert.ToInt32(arguments[2], CultureInfo.InvariantCulture),
                        Convert.ToInt32(arguments[3], CultureInfo.InvariantCulture)
                        );

                var image = await ScreenshotAsync(rectangle, cancellationToken).ConfigureAwait(false);
                var tempPath = Path.GetTempFileName();
                image.Save(tempPath);

                return new Value(tempPath);
            }));
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="_"></param>
        /// <returns></returns>
        public async Task<Image> ScreenshotAsync(Rectangle? rectangle = null, CancellationToken _ = default)
        {
            var image = rectangle == null
                ? await Screenshoter.ShotVirtualDisplayAsync().ConfigureAwait(false)
                : await Screenshoter.ShotVirtualDisplayRectangleAsync(rectangle.Value).ConfigureAwait(false);

            OnCaptured(image);

            return image;
        }

        #endregion
    }
}
