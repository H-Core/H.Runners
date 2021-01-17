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

                var image = await ShotAsync(rectangle, cancellationToken)
                    .ConfigureAwait(false);
                var tempPath = Path.GetTempFileName();
                image.Save(tempPath);

                return new Value(tempPath);
            }));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates screenshot of all available screens. <br/>
        /// If <paramref name="rectangle"/> is not null, returns image of this region.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Image> ShotAsync(
            Rectangle? rectangle = null, 
            CancellationToken cancellationToken = default)
        {
            return await Screenshoter.ShotAsync(rectangle, cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion
    }
}
