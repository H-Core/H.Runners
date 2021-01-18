using System;
using System.Drawing;

namespace H.Runners.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class PointExtensions
    {
        /// <summary>
        /// Converts physical point to app point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static Point ToApp(
            this Point point, 
            double scaleFactor)
        {
            return new (
                (int)Math.Round(scaleFactor * point.X), 
                (int)Math.Round(scaleFactor * point.Y));
        }

        /// <summary>
        /// Converts app point to physical point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static Point ToPhysical(
            this Point point, 
            double scaleFactor)
        {
            return new(
                (int)Math.Round(point.X / scaleFactor), 
                (int)Math.Round(point.Y / scaleFactor)
                );
        }

        /// <summary>
        /// Converts app rectangle to physical rectangle.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static Rectangle ToPhysical(
            this Rectangle rectangle, 
            double scaleFactor)
        {
            var point1 = new Point(rectangle.Left, rectangle.Top)
                .ToPhysical(scaleFactor);
            var point2 = new Point(rectangle.Right, rectangle.Bottom)
                .ToPhysical(scaleFactor);

            return Rectangle.FromLTRB(
                point1.X, 
                point1.Y,
                point2.X, 
                point2.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Rectangle ToRectangle(
            this Point point1, 
            Point point2)
        {
            return Rectangle.FromLTRB(
                    point1.X,
                    point1.Y,
                    point2.X,
                    point2.Y
                )
                .Normalize();
        }
    }
}
