using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using H.Core;
using H.Core.Runners;
using H.Runners.Extensions;
using H.Runners.Utilities;
using Point = System.Drawing.Point;
using Timer = System.Timers.Timer;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SelectRunner : Runner
    {
        #region Properties

        private RectangleWindow? Window { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public SelectRunner()
        {
            Add(new ProcessAction("select", async (process, _, cancellationToken) =>
            {
                var rectangle = await SelectAsync(process, cancellationToken)
                    .ConfigureAwait(false);

                return new Value(
                    $"{rectangle.Left}", 
                    $"{rectangle.Top}", 
                    $"{rectangle.Right}", 
                    $"{rectangle.Bottom}");
            }));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var thread = new Thread(() =>
            {
                Window = new RectangleWindow();
                Window.ShowDialog();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            while (Window == null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Rectangle> SelectAsync(IProcess<ICommand> process, CancellationToken cancellationToken = default)
        {
            process = process ?? throw new ArgumentNullException(nameof(process));

            if (Window == null)
            {
                await InitializeAsync(cancellationToken).ConfigureAwait(false);
            }

            Window = Window ?? throw new InvalidOperationException("Window is null.");

            var scaleFactor = 0.0;
            var handle = (nint)0;

            await Window.Dispatcher.InvokeAsync(() =>
            {
                scaleFactor = Window.GetDpi();
                handle = new WindowInteropHelper(Window).Handle;
            });

            var startPoint = MouseUtilities
                .GetPhysicalCursorPosition(handle)
                .ToApp(scaleFactor);
            var endPoint = startPoint;

            using var timer = new Timer(1);
            timer.Elapsed += (_, _) =>
            {
                Window.Dispatcher.Invoke(() =>
                {
                    endPoint = MouseUtilities.GetPhysicalCursorPosition(handle).ToApp(scaleFactor);

                    ApplyRectangle(
                        Window, 
                        startPoint,
                        endPoint);
                });
            };
            timer.Start();

            await Window.Dispatcher.InvokeAsync(() =>
            {
                Window.Border.Visibility = Visibility.Visible;

                //ApplyRectangle(
                //    Window,
                //    startPoint,
                //    new Point(startPoint.X + 1, startPoint.Y + 1));
            });

            await process.WaitAsync(cancellationToken).ConfigureAwait(false);

            //var endPoint = MouseUtilities.GetPhysicalCursorPosition(handle)
            //    .ToApp(scaleFactor);

            timer.Stop();

            await Window.Dispatcher.InvokeAsync(() =>
            {
                Window.Border.Visibility = Visibility.Hidden;
            });

            return startPoint
                .ToRectangle(endPoint)
                .Normalize()
                .ToPhysical(scaleFactor);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            Window?.Dispatcher.Invoke(() =>
            {
                Window.Close();
            });
            Window = null;

            base.Dispose();
        }

        #endregion

        #region Private methods

        private static void ApplyRectangle(RectangleWindow window, Point startPoint, Point endPoint)
        {
            var rectangle = startPoint.ToRectangle(endPoint);

            window.Border.Margin = new Thickness(
                rectangle.Left - window.Left,
                rectangle.Top - window.Top,
                window.Width + window.Left - rectangle.Right,
                window.Height + window.Top - rectangle.Bottom);
        }

        #endregion
    }
}
