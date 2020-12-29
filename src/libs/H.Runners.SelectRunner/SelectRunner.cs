using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using H.Core.Runners;
using Point = System.Drawing.Point;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectRunner : Runner
    {
        #region Properties

        private RectangleWindow? Window { get; set; }
        private Point StartPoint { get; set; }
        private bool IsMouseDown { get; set; }

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
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public void SelectAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            var window = new RectangleWindow();

            var startPoint = MouseUtilities.GetCursorPosition();

            window.Border.Margin = new Thickness(
                startPoint.X - window.Left,
                startPoint.Y - window.Top,
                window.Width + window.Left - startPoint.X,
                window.Height + window.Top - startPoint.Y);
            window.Border.Visibility = Visibility.Visible;

            //while (!cancellationToken.IsCancellationRequested)
            {
                //await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);

                var endPoint = new Point(1000, 1000);//MouseUtilities.GetCursorPosition();
                var rectangle = CalculateRectangle(startPoint, endPoint);

                //await window.Dispatcher.InvokeAsync(() =>
                {
                    window.Border.Margin = new Thickness(
                        rectangle.Left - window.Left,
                        rectangle.Top - window.Top,
                        window.Width + window.Left - rectangle.Left - rectangle.Width,
                        window.Height + window.Top - rectangle.Top - rectangle.Height);
                }//);
            }

            window.Show();
            //window.Hide();
        }

        #endregion

        #region Event Handlers

        //public void Global_MouseUp(object? sender, MouseEventExtArgs e)
        //{
        //    IsMouseDown = false;
        //    if (!IsHookCombination())
        //    {
        //        return;
        //    }

        //    e.Handled = true;

        //    View?.Close();
        //    View = null;

        //    var rectangle = CalculateRectangle(e);
        //    if (rectangle.Width == 0 || rectangle.Height == 0)
        //    {
        //        return;
        //    }

        //    OnNewRectangle(rectangle);
        //}

        //public void Global_MouseMove(object? sender, MouseEventExtArgs e)
        //{
        //    //if (System.Diagnostics.Debugger.IsAttached)
        //    if (!IsMouseDown || !IsHookCombination())
        //    {
        //        View?.Close();
        //        View = null;
        //        return;
        //    }

        //    var rectangle = CalculateRectangle(e);

        //    View = View ?? throw new InvalidOperationException("View is null");
        //    View.Border.Margin = new Thickness(
        //        rectangle.Left - View.Left,
        //        rectangle.Top - View.Top,
        //        View.Width + View.Left - rectangle.Left - rectangle.Width,
        //        View.Height + View.Top - rectangle.Top - rectangle.Height);

        //}

        //public void Global_MouseDown(object? sender, MouseEventExtArgs e)
        //{
        //    IsMouseDown = true;
        //    if (!IsHookCombination())
        //    {
        //        return;
        //    }

        //    e.Handled = true;

        //    View = new RectangleView();

        //    StartPoint = new Point(e.X, e.Y);

        //    View.Border.Margin = new Thickness(
        //        e.X - View.Left,
        //        e.Y - View.Top,
        //        View.Width + View.Left - e.X,
        //        View.Height + View.Top - e.Y);
        //    View.Border.Visibility = Visibility.Visible;

        //    View.Show();

        //}

        //#endregion

        //#region Private Methods

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

        #endregion
    }
}
