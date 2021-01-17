using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace H.Runners.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static BitmapImage ToBitmapImage(this Image image)
        {
            image = image ?? throw new ArgumentNullException(nameof(image));

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = image.ToMemoryStream();
            bitmapImage.EndInit();

            return bitmapImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this Image image, ImageFormat? format = default)
        {
            image = image ?? throw new ArgumentNullException(nameof(image));

            var stream = new MemoryStream();
            image.Save(stream, format ?? ImageFormat.Bmp);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
