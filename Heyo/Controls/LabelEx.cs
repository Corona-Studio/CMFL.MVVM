using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Heyo.Controls
{
    public class LabelEx : ClippingBorder
    {
        private readonly Label strinHost = new Label
            {Padding = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Center};

        public LabelEx()
        {
            Child = new Viewbox {Child = strinHost, HorizontalAlignment = HorizontalAlignment.Center};
            strinHost.SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.ClearType);
            strinHost.Foreground = new SolidColorBrush(Colors.White);
            Background = new SolidColorBrush(Colors.LightBlue);
            //strinHost.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
        }

        public FontFamily FontFamily
        {
            get => strinHost.FontFamily;
            set => strinHost.FontFamily = value;
        }

        public Brush Foreground
        {
            get => strinHost.Foreground;
            set => strinHost.Foreground = value;
        }

        public object Content
        {
            get => strinHost.Content;
            set
            {
                strinHost.Content = value;
                if (value == null || value is string && ((string) value).Length == 0)
                    Visibility = Visibility.Collapsed;
                else
                    Visibility = Visibility.Visible;
            }
        }
    }
}