using System.Drawing;

namespace H.Runners.Extensions
{
    internal static class RectangleExtensions
    {
        public static Rectangle Normalize(this Rectangle rectangle)
        {
            return Rectangle.FromLTRB(
                rectangle.Right > rectangle.Left ? rectangle.Left : rectangle.Right,
                rectangle.Bottom > rectangle.Top ? rectangle.Top : rectangle.Bottom,
                rectangle.Right > rectangle.Left ? rectangle.Right : rectangle.Left,
                rectangle.Bottom > rectangle.Top ? rectangle.Bottom : rectangle.Top
                );
        }
    }
}
