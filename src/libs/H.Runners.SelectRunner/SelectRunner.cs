using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<Rectangle>? NewRectangle;

        private void OnNewRectangle(Rectangle rectangle)
        {
            NewRectangle?.Invoke(this, rectangle);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public SelectRunner()
        {
            Add(new ProcessAction("select", async (process, _, cancellationToken) =>
            {
                var rectangle = await SelectAsync(process, cancellationToken).ConfigureAwait(false);

                return new Value($"{rectangle.Left}", $"{rectangle.Top}", $"{rectangle.Right}", $"{rectangle.Bottom}");
            }));
        }

        #endregion

        #region Methods

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

            var dpi = await Window.Dispatcher.InvokeAsync(() => Window.GetDpi());
            var startPoint = GetCursorPosition(dpi);
            
            using var timer = new Timer(15);
            timer.Elapsed += (_, _) =>
            {
                Window.Dispatcher.Invoke(() =>
                {
                    ApplyRectangle(Window, CalculateRectangle(startPoint, GetCursorPosition(dpi)));
                });
            };
            timer.Start();

            await Window.Dispatcher.InvokeAsync(() =>
            {
                Window.Border.Visibility = Visibility.Visible;

                ApplyRectangle(Window, CalculateRectangle(startPoint, new Point(startPoint.X + 1, startPoint.Y + 1)));
            });

            await process.WaitAsync(cancellationToken).ConfigureAwait(false);

            await Window.Dispatcher.InvokeAsync(() =>
            {
                Window.Border.Visibility = Visibility.Hidden;
            });

            var rectangle = CalculateRectangle(startPoint, GetCursorPosition(dpi));
            if (rectangle.Width != 0 && rectangle.Height != 0)
            {
                OnNewRectangle(rectangle);
            }

            return rectangle;
        }

        private static Point GetCursorPosition(double dpi)
        {
            var point = MouseUtilities.GetCursorPosition();

            return new Point((int)(dpi * point.X), (int)(dpi * point.Y));
        }

        private static Rectangle CalculateRectangle(Point startPoint, Point endPoint)
        {
            var dx = endPoint.X - startPoint.X;
            var dy = endPoint.Y - startPoint.Y;

            var x = startPoint.X + Math.Min(0, dx);
            var y = startPoint.Y + Math.Min(0, dy);

            var width = dx > 0 ? dx : -dx;
            var height = dy > 0 ? dy : -dy;

            return new Rectangle(x, y, width, height);
        }

        private static void ApplyRectangle(RectangleWindow window, Rectangle rectangle)
        {
            window.Border.Margin = new Thickness(
                rectangle.Left - window.Left,
                rectangle.Top - window.Top,
                window.Width + window.Left - rectangle.Left - rectangle.Width,
                window.Height + window.Top - rectangle.Top - rectangle.Height);
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
    }
}
