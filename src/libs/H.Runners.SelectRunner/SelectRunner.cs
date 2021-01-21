using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using H.Core;
using H.Core.Runners;
using H.Core.Utilities;
using H.Hooks;
using H.Runners.Extensions;
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
        private Dispatcher Dispatcher { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public SelectRunner(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            Add(new ProcessAction("select", async (process, _, cancellationToken) =>
            {
                var rectangle = await SelectAsync(process, cancellationToken)
                    .ConfigureAwait(false);

                return new Value(
                    $"{rectangle.X}", 
                    $"{rectangle.Y}", 
                    $"{rectangle.Width}", 
                    $"{rectangle.Height}");
            }, "Returns: x, y, width, height"));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task InitializeAsync(
            CancellationToken cancellationToken = default)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Window = new RectangleWindow();
                Window.Show();
            }, DispatcherPriority.Normal, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public async Task<Rectangle> SelectAsync(
            IProcess<ICommand> process, 
            CancellationToken cancellationToken = default)
        {
            process = process ?? throw new ArgumentNullException(nameof(process));

            if (Window == null)
            {
                await InitializeAsync(cancellationToken).ConfigureAwait(false);
            }

            Window = Window ?? throw new InvalidOperationException("Window is null.");

            var scaleFactor = await Dispatcher.InvokeAsync(
                () => Window.GetDpi(), 
                DispatcherPriority.Normal, 
                cancellationToken);

            var startPoint = new Point();
            var endPoint = new Point();
            var currentPoint = new Point();

            using var exceptions = new ExceptionsBag();
            using var hook = new LowLevelMouseHook
            {
                GenerateMouseMoveEvents = true,
            };
            hook.ExceptionOccurred += (_, exception) =>
            {
                // ReSharper disable once AccessToDisposedClosure
                exceptions.OnOccurred(exception);
            };

            var isInitialized = false;
            hook.Move += (_, args) =>
            {
                currentPoint = args.Position;
                if (isInitialized)
                {
                    return;
                }

                startPoint = currentPoint.ToApp(scaleFactor);
                endPoint = startPoint;
                isInitialized = true;
            };
            hook.Start();

            using var timer = new Timer(15);
            timer.Elapsed += (_, _) =>
            {
                if (!isInitialized)
                {
                    return;
                }

                endPoint = currentPoint.ToApp(scaleFactor);

                Dispatcher.Invoke(() =>
                {
                    ApplyRectangle(
                        Window, 
                        startPoint,
                        endPoint);
                });
            };
            timer.Start();

            await Dispatcher.InvokeAsync(() =>
            {
                ApplyRectangle(
                    Window,
                    startPoint,
                    endPoint);
                Window.Border.Visibility = Visibility.Visible;
            }, DispatcherPriority.Normal, cancellationToken);

            await process.WaitAsync(cancellationToken).ConfigureAwait(false);

            timer.Dispose();
            hook.Dispose();

            await Dispatcher.InvokeAsync(() =>
            {
                Window.Border.Visibility = Visibility.Hidden;
            }, DispatcherPriority.Normal, cancellationToken);

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
            if (Window == null)
            {
                return;
            }

            Dispatcher.Invoke(() =>
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
