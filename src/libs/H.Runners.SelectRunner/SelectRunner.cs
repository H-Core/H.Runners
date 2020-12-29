using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using H.Core;
using H.Core.Runners;
using Point = System.Drawing.Point;
using Timer = System.Timers.Timer;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectRunner : Runner
    {
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
                await SelectAsync(process, cancellationToken).ConfigureAwait(false);

                return Value.Empty;
            }));
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SelectAsync(IProcess<ICommand> process, CancellationToken cancellationToken = default)
        {
            var thread = new Thread(() =>
            {
                var window = new RectangleWindow();
                var application = new Application();
                application.Startup += async (_, _) =>
                {
                    using var timer = new Timer(15);

                    var startPoint = MouseUtilities.GetCursorPosition();

                    window.Border.Visibility = Visibility.Visible;

                    ApplyRectangle(window, CalculateRectangle(startPoint, new Point(startPoint.X + 1, startPoint.Y + 1)));

                    window.Show();

                    timer.Elapsed += (_, _) =>
                    {
                        application.Dispatcher.Invoke(() =>
                        {
                            ApplyRectangle(window, CalculateRectangle(startPoint, MouseUtilities.GetCursorPosition()));
                        });
                    };

                    timer.Start();

                    await process.WaitAsync(cancellationToken).ConfigureAwait(false);

                    var rectangle = CalculateRectangle(startPoint, MouseUtilities.GetCursorPosition());
                    if (rectangle.Width != 0 && rectangle.Height != 0)
                    {
                        OnNewRectangle(rectangle);
                    }

                    application.Dispatcher.Invoke(() =>
                    {
                        application.Shutdown();
                    });
                };
                application.Run(window);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            
            await Task.Run(thread.Join, cancellationToken).ConfigureAwait(false);
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

        #endregion
    }
}
