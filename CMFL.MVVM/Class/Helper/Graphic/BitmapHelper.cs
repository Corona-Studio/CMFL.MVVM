using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CMFL.MVVM.Class.Helper.Launcher;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;

namespace CMFL.MVVM.Class.Helper.Graphic
{
    /// <summary>
    ///     图像处理
    /// </summary>
    public static class BitmapHelper
    {
        /// <summary>
        ///     读取图像
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="decodePixelHeight">解码图片高度</param>
        /// <param name="decodePixelWeight">解码图片宽度</param>
        /// ///
        /// <returns>Bitmap图片</returns>
        public static BitmapImage GetImage(string imagePath, int decodePixelHeight, int decodePixelWeight)
        {
            var bitmap = new BitmapImage();

            if (File.Exists(imagePath))
            {
                bitmap.BeginInit();
                bitmap.DecodePixelHeight = decodePixelHeight;
                bitmap.DecodePixelWidth = decodePixelWeight;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                var ms = new MemoryStream(File.ReadAllBytes(imagePath));
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            return bitmap;
        }

        /// <summary>
        ///     Bitmap 转 BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            var stream = new MemoryStream();
            bitmap?.Save(stream, ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

            stream.Position = 0;
            var result = new BitmapImage();
            result.BeginInit();
            // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
            // Force the bitmap to load right now so we can dispose the stream.
            result.CacheOption = BitmapCacheOption.OnLoad;
            result.StreamSource = stream;
            result.EndInit();
            result.Freeze();
            return result;
        }


        /// <summary>
        ///     BitmapImage 转 Bitmap
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            var outStream = new MemoryStream();
            var enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            enc.Save(outStream);
            return new Bitmap(outStream);
        }

        // ImageSource --> Bitmap
        public static Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            var m = (BitmapSource) imageSource;

            if (m == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            var bmp = new Bitmap(m.PixelWidth, m.PixelHeight, PixelFormat.Format32bppPArgb); // 坑点：选Format32bppRgb将不带透明度

            var data = bmp.LockBits(
                new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);

            m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);

            return bmp;
        }

        /// <summary>
        ///     RenderTargetBitmap 转 BitmapImage
        /// </summary>
        /// <param name="wbm"></param>
        /// <returns></returns>
        public static BitmapImage ConvertRenderTargetBitmapToBitmapImage(RenderTargetBitmap wbm)
        {
            var bmp = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bmp.StreamSource = new MemoryStream(stream.ToArray()); //stream;
                bmp.EndInit();
                bmp.Freeze();
            }

            return bmp;
        }


        /// <summary>
        ///     RenderTargetBitmap转BitmapImage
        /// </summary>
        /// <param name="rtb"></param>
        /// <returns></returns>
        public static BitmapImage RenderTargetBitmapToBitmapImage(RenderTargetBitmap rtb)
        {
            var renderTargetBitmap = rtb;
            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            var stream = new MemoryStream();
            bitmapEncoder.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        /// <summary>
        ///     ImageSource转Bitmap
        /// </summary>
        /// <param name="bitmapSource">Bitmap变量</param>
        /// <returns></returns>
        public static Bitmap BitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            var width = bitmapSource?.PixelWidth ?? 0;
            var height = bitmapSource?.PixelHeight ?? 0;
            var stride = width * ((bitmapSource?.Format.BitsPerPixel ?? 0 + 7) / 8);
            var memoryBlockPointer = Marshal.AllocHGlobal(height * stride);
            bitmapSource?.CopyPixels(new Int32Rect(0, 0, width, height), memoryBlockPointer, height * stride, stride);
            var bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppPArgb, memoryBlockPointer);
            return bitmap;
        }

        /// <summary>
        ///     Bitmap转byte[]
        /// </summary>
        /// <param name="bmp"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>Bytes</returns>
        public static byte[] BitmapImageToByteArray(BitmapImage bmp)
        {
            byte[] bytearray = null;

            if (bmp == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            var smarket = bmp.StreamSource;

            if (smarket != null && smarket.Length > 0)
            {
                //设置当前位置
                smarket.Position = 0;
                using var br = new BinaryReader(smarket);
                bytearray = br.ReadBytes((int) smarket.Length);
            }

            return bytearray;
        }


        /// <summary>
        ///     byte[]转Bitmap
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static BitmapImage ByteArrayToBitmapImage(byte[] array)
        {
            using var ms = new MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; // here
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze();
            return image;
        }

        /// <summary>
        ///     byte[]转Bitmap
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Bitmap ConvertByteArrayToBitmap(byte[] bytes)
        {
            Bitmap img = null;

            if (bytes != null && bytes.Length != 0)
            {
                using var ms = new MemoryStream(bytes);
                img = new Bitmap(ms);
            }

            return img;
        }

        /// <summary>
        ///     Base64转Bitmap
        /// </summary>
        /// <param name="base64String">Base64字符串</param>
        /// <returns></returns>
        public static Bitmap GetImageFromBase64(string base64String)
        {
            var b = Convert.FromBase64String(base64String);
            using var ms = new MemoryStream(b);
            var bitmap = new Bitmap(ms);
            return bitmap;
        }

        /// <summary>
        ///     Bitmap转Base64
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        public static string GetBase64FromImage(string imageFile)
        {
            string strBase64;

            using (var bmp = new Bitmap(imageFile))
            {
                var ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                var arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int) ms.Length);
                ms.Close();
                strBase64 = Convert.ToBase64String(arr);
            }

            return strBase64;
        }

        /// <summary>
        ///     从skin转换到头像
        /// </summary>
        /// <param name="fileName">skin文件的路径</param>
        /// <param name="targetSize">size必须是8的倍数 16 32 64 ...</param>
        public static Bitmap SkinToHeadFile(string fileName, int targetSize)
        {
            if (targetSize % 8 != 0) //不等于8的倍数
                throw new Exception();

            var size = targetSize / 8;

            var head = new Bitmap(targetSize, targetSize); //创建正方形画布
            var g = Graphics.FromImage(head); //从画布创建图形
            var skin = new Bitmap(fileName); //载入skin图片


            //取得头部的8*8位置，并按照比例填充矩形
            for (var x = 8; x <= 15; x++)
            for (var y = 8; y <= 15; y++)
            {
                //对每个点作为矩形填充
                using var brush = new SolidBrush(skin.GetPixel(x, y));
                g.FillRectangle(brush,
                    new Rectangle(0 + (x - 8) * size, (y - 8) * size, size, size));
            }

            //存储
            g.Save();
            skin.Dispose();
            //head.Save("a.png");
            return head;
        }
    }
}