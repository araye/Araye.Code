using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Araye.Code.Extensions
{
    /// <summary>
    /// some codes from DNT Extenstions
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Scales a Image to make it fit inside of a Height/Width
        /// Example:
        /// Image test = someImg.ScaleImage(591, 1096);
        /// </summary>
        /// <param name="img"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Image ScaleImage(this Image img, int height, int width)
        {
            if (img == null || height <= 0 || width <= 0)
            {
                return null;
            }
            int newWidth = (img.Width * height) / (img.Height);
            int newHeight = (img.Height * width) / (img.Width);
            int x;
            int y;

            var bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            // use this when debugging.
            //g.FillRectangle(Brushes.Aqua, 0, 0, bmp.Width - 1, bmp.Height - 1);
            if (newWidth > width)
            {
                // use new height
                x = (bmp.Width - width) / 2;
                y = (bmp.Height - newHeight) / 2;
                g.DrawImage(img, x, y, width, newHeight);
            }
            else
            {
                // use new width
                x = (bmp.Width / 2) - (newWidth / 2);
                y = (bmp.Height / 2) - (height / 2);
                g.DrawImage(img, x, y, newWidth, height);
            }
            // use this when debugging.
            //g.DrawRectangle(new Pen(Color.Red, 1), 0, 0, bmp.Width - 1, bmp.Height - 1);
            return bmp;
        }

        /// <summary>
        /// This method resizes a System.Drawing.Image and tries to fit it in the destination Size. 
        /// The source image size may be smaller or bigger then the target size. 
        /// Source and target layout orientation can be different. ResizeAndFit tries to fit it the best it can.
        /// Example:
        /// Image image = Image.FromStream(context.Request.InputStream).ResizeAndFit(new Size( 125, 100));
        /// </summary>
        /// <param name="image"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        public static Image ResizeAndFit(this Image image, Size newSize)
        {
            var sourceIsLandscape = image.Width > image.Height;
            var targetIsLandscape = newSize.Width > newSize.Height;

            var ratioWidth = (double)newSize.Width / (double)image.Width;
            var ratioHeight = (double)newSize.Height / (double)image.Height;

            var ratio = 0.0;

            if (ratioWidth > ratioHeight && sourceIsLandscape == targetIsLandscape)
                ratio = ratioWidth;
            else
                ratio = ratioHeight;

            int targetWidth = (int)(image.Width * ratio);
            int targetHeight = (int)(image.Height * ratio);

            var bitmap = new Bitmap(newSize.Width, newSize.Height);
            var graphics = Graphics.FromImage((Image)bitmap);

            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var offsetX = ((double)(newSize.Width - targetWidth)) / 2;
            var offsetY = ((double)(newSize.Height - targetHeight)) / 2;

            graphics.DrawImage(image, (int)offsetX, (int)offsetY, targetWidth, targetHeight);
            graphics.Dispose();

            return (Image)bitmap;
        }

        /// <summary>
        /// Create a new Image from a byte array
        /// Example:
        /// byte[] imageBytes = GetImageBytesFromDB();
        /// Image myImage = imageBytes.ToImage();
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Image ToImage(this byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            return Image.FromStream(new MemoryStream(bytes));
        }

        /// <summary>
        /// Convert image to byte array
        /// Example:
        /// Image image = ...;
        /// byte[] imageBytes = image.ToBytes(ImageFormat.Png);
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Image image, ImageFormat format)
        {
            if (image == null)
                throw new ArgumentNullException("image");
            if (format == null)
                throw new ArgumentNullException("format");

            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, format);
                return stream.ToArray();
            }
        }
    }
}
