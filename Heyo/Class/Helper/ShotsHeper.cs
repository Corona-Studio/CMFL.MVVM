using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Heyo.Class.Helper
{
    public class ShotsHeper
    {
        /// <summary>
        ///     将控件转换为图片
        /// </summary>
        /// <param name="frameworkElement">控件</param>
        /// <returns></returns>
        public static BitmapSource GetControlImage(FrameworkElement frameworkElement)
        {
            var bmp = new RenderTargetBitmap((int) frameworkElement.ActualWidth, (int) frameworkElement.ActualHeight,
                96.0, 96.0, PixelFormats.Pbgra32);
            bmp.Render(frameworkElement);
            return bmp;
        }
    }
}