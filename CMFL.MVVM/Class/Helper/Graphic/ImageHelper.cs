using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CMFL.MVVM.Class.Helper.Launcher;
using Heyo.Class.Helper;
using Color = System.Windows.Media.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace CMFL.MVVM.Class.Helper.Graphic
{
    public static class ImageHelper
    {
        /// <summary>
        ///     实现System.Drawing.Bitmap到System.Windows.Media.Imaging.BitmapImage的转换
        /// </summary>
        /// <param name="bitmap"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>BitmapImage</returns>
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        /// <summary>
        ///     从bitmap转换成ImageSource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            var hBitmap = bitmap.GetHbitmap();
            var wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!NativeMethods.DeleteObject(hBitmap)) throw new Win32Exception();

            return wpfBitmap;
        }

        /// <summary>
        ///     从Bitmap转换成BitmapSource
        /// </summary>
        /// <param name="bitmapp"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            var returnSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return returnSource;
        }

        /// <summary>
        ///     从Icon到ImageSource的转换
        /// </summary>
        /// <param name="icon">图标</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ImageSource ToImageSource(this Icon icon)
        {
            if (icon == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        /// <summary>
        ///     马赛克处理
        /// </summary>
        /// <param name="bitmap">要处理的图像</param>
        /// <param name="effectWidth">每一格马赛克的宽度</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Bitmap AdjustToBitmapMosaic(this Bitmap bitmap, int effectWidth)
        {
            if (bitmap == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            // 差异最多的就是以照一定范围取样 玩之后直接去下一个范围
            for (var heightOfffset = 0; heightOfffset < bitmap.Height; heightOfffset += effectWidth)
            for (var widthOffset = 0; widthOffset < bitmap.Width; widthOffset += effectWidth)
            {
                int avgR = 0, avgG = 0, avgB = 0;
                var blurPixelCount = 0;

                for (var x = widthOffset; x < widthOffset + effectWidth && x < bitmap.Width; x++)
                for (var y = heightOfffset; y < heightOfffset + effectWidth && y < bitmap.Height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);

                    avgR += pixel.R;
                    avgG += pixel.G;
                    avgB += pixel.B;

                    blurPixelCount++;
                }

                // 计算范围平均
                avgR /= blurPixelCount;
                avgG /= blurPixelCount;
                avgB /= blurPixelCount;


                // 所有范围内都设定此值
                for (var x = widthOffset; x < widthOffset + effectWidth && x < bitmap.Width; x++)
                for (var y = heightOfffset; y < heightOfffset + effectWidth && y < bitmap.Height; y++)
                {
                    var newColor = System.Drawing.Color.FromArgb(avgR, avgG, avgB);
                    bitmap.SetPixel(x, y, newColor);
                }
            }

            return bitmap;
        }

        /// <summary>
        ///     优化图片（使用图片路径）
        /// </summary>
        /// <param name="path">Bitmap图像路径</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns>优化后的图片</returns>
        public static BitmapImage GetOptimizeImageFromPath(string path, int width = 1024, int height = 768)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.DecodePixelWidth = width;
            image.DecodePixelHeight = height;
            image.CreateOptions = BitmapCreateOptions.DelayCreation;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path);
            image.EndInit();
            image.Freeze();
            return image;
        }

        /// <summary>
        ///     将数组转化为Bitmap
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this byte[] Bytes)
        {
            using var stream = new MemoryStream(Bytes);
            return new Bitmap(stream);
        }

        /// <summary>
        ///     将Bitmap转化为数组
        /// </summary>
        /// <param name="bitmap"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static byte[] ToBytes(this Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            using var ms = new MemoryStream();
            bitmap.Save(ms, bitmap.RawFormat);
            return ms.ToArray();
        }

        /// <summary>
        ///     将数组转化为BitmapImage
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static BitmapImage ToBitmapImage(this byte[] byteArray)
        {
            var stream = new MemoryStream(byteArray);
            var bmp = new BitmapImage();

            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.EndInit();

            return bmp;
        }

        /// <summary>
        ///     DrawingImage转ImageSource
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static ImageSource DrawingImageToImageSource(this Image image)
        {
            using var bmp = new Bitmap(image);
            var hBitmap = bmp.GetHbitmap();
            var wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return wpfBitmap;
        }

        /// <summary>
        ///     将BitmapImage转化为数组
        /// </summary>
        /// <param name="bmp"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static byte[] ToBytes(this BitmapImage bmp)
        {
            byte[] byteArray = null;

            if (bmp == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            var sMarket = bmp.StreamSource;
            if (sMarket != null && sMarket.Length > 0)
            {
                //很重要，因为Position经常位于Stream的末尾，导致下面读取到的长度为0。 
                sMarket.Position = 0;

                using var br = new BinaryReader(sMarket);
                byteArray = br.ReadBytes((int) sMarket.Length);
            }

            return byteArray;
        }

        /// <summary>
        ///     重设Bitmap图片大小
        /// </summary>
        /// <param name="bm"></param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <param name="interpolationMode"></param>
        /// <param name="smoothingMode"></param>
        /// <param name="compositeQuality"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Bitmap SetSize(this Bitmap bm, int w, int h,
            InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic,
            SmoothingMode smoothingMode = SmoothingMode.HighQuality,
            CompositingQuality compositeQuality = CompositingQuality.HighSpeed
        )
        {
            if (bm == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            var bitmap = new Bitmap(w, h); //新建一个放大后大小的图片

            using var g = Graphics.FromImage(bitmap);
            g.InterpolationMode = interpolationMode;
            g.SmoothingMode = smoothingMode;
            g.CompositingQuality = compositeQuality;
            g.DrawImage(bm, new Rectangle(0, 0, w, h), new Rectangle(0, 0, bm.Width, bm.Height),
                GraphicsUnit.Pixel);

            return bitmap;
        }

        /// <summary>
        ///     裁剪图像
        /// </summary>
        /// <param name="b"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="iWidth"></param>
        /// <param name="iHeight"></param>
        /// <returns></returns>
        public static Bitmap Cut(this Bitmap b, int startX, int startY, int iWidth, int iHeight)
        {
            if (b == null) return null;
            var w = b.Width;
            var h = b.Height;
            if (startX >= w || startY >= h) return null;
            if (startX + iWidth > w) iWidth = w - startX;
            if (startY + iHeight > h) iHeight = h - startY;

            using var bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
            using var g = Graphics.FromImage(bmpOut);
            g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(startX, startY, iWidth, iHeight),
                GraphicsUnit.Pixel);
            return bmpOut;
        }

        /// <summary>
        ///     获取主题色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Color GetThemeColor(this Bitmap bitmap)
        {
            var c = bitmap.GetMajorColor();
            c = ColorHelper.FromAhsv(255, c.GetHue(), 0.6F, 0.7F);
            return c.ToMedia();
        }

        /// <summary>
        ///     获取图片主色调
        /// </summary>
        /// <param name="bitmap"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static System.Drawing.Color GetMajorColor(this Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            //色调的总和
            var sumHue = 0d;
            //色差的阈值
            const int threshold = 10;

            using var smaller = bitmap.SetSize((int) (bitmap.Size.Width * 0.4), (int) (bitmap.Size.Height * 0.4));
            //计算色调总和
            for (var h = 0; h < smaller.Height; h++)
            for (var w = 0; w < smaller.Width; w++)
            {
                var hue = smaller.GetPixel(w, h).GetHue();
                sumHue += hue;
            }

            var avgHue = sumHue / (smaller.Width * smaller.Height);

            //色差大于阈值的颜色值
            var rgbList = new List<System.Drawing.Color>();
            for (var h = 0; h < smaller.Height; h++)
            for (var w = 0; w < smaller.Width; w++)
            {
                var color = smaller.GetPixel(w, h);
                var hue = color.GetHue();
                //如果色差大于阈值，则加入列表
                if (Math.Abs(hue - avgHue) > threshold) rgbList.Add(color);
            }

            if (rgbList.Count == 0)
                return System.Drawing.Color.Black;

            //计算列表中的颜色均值，结果即为该图片的主色调
            int sumR = 0, sumG = 0, sumB = 0;
            foreach (var rgb in rgbList)
            {
                sumR += rgb.R;
                sumG += rgb.G;
                sumB += rgb.B;
            }

            return System.Drawing.Color.FromArgb(sumR / rgbList.Count,
                sumG / rgbList.Count,
                sumB / rgbList.Count);
        }

        /// <summary>
        ///     获取主题色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static System.Drawing.Color GetMostUsedColor(this Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            //List<System.Drawing.Color> TenMostUsedColors;
            //List<int> TenMostUsedColorIncidences;

            //int MostUsedColorIncidence;

            //TenMostUsedColors = new List<System.Drawing.Color>();
            //TenMostUsedColorIncidences = new List<int>();

            //MostUsedColorIncidence = 0;

            // does using Dictionary<int,int> here
            // really pay-off compared to using
            // Dictionary<Color, int> ?

            // would using a SortedDictionary be much slower, or ?

            var dctColorIncidence = new Dictionary<int, int>();
            using (var smaller = bitmap.SetSize((int) (bitmap.Size.Width * 0.4), (int) (bitmap.Size.Height * 0.4)))
                // this is what you want to speed up with unmanaged code
            {
                for (var row = 0; row < smaller.Size.Width; row++)
                for (var col = 0; col < smaller.Size.Height; col++)
                {
                    var pixelColor = smaller.GetPixel(row, col).ToArgb();

                    if (dctColorIncidence.Keys.Contains(pixelColor))
                        dctColorIncidence[pixelColor]++;
                    else
                        dctColorIncidence.Add(pixelColor, 1);
                }
            }

            // note that there are those who argue that a
            // .NET Generic Dictionary is never guaranteed
            // to be sorted by methods like this
            var dctSortedByValueHighToLow =
                dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            var mostUsedColor = System.Drawing.Color.FromArgb(dctSortedByValueHighToLow.First().Key);
            //MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value;
            return mostUsedColor;
        }

        public static async Task<ImageSource> GetImageSourceFromUri(Uri imageUri)
        {
            using var client = new WebClient();
            var result = await client.DownloadDataTaskAsync(imageUri).ConfigureAwait(true);
            return result.ToBitmapImage();
        }
    }
}