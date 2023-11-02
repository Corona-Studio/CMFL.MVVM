using System;
using System.Globalization;
using System.Windows.Data;

namespace CMFL.MVVM.Class.Converter
{
    /// <summary>
    ///     布尔类型反转
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType != typeof(bool)) throw new InvalidOperationException("The target must be a boolean");
            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            //throw new NotSupportedException();
            return !(bool) value;
        }

        #endregion
    }
}