using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CMFL.MVVM.Class.Converter
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class ImageOptimizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is string fileName)
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.DelayCreation;
                image.CacheOption = BitmapCacheOption.OnDemand;
                image.UriSource = new Uri(fileName, UriKind.RelativeOrAbsolute);
                image.EndInit();

                return image;
            }

            return new BitmapImage();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}