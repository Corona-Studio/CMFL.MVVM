using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Heyo.Controls
{
    /// <summary>
    ///     Avatar.xaml 的交互逻辑
    /// </summary>
    public partial class Avatar : ClippingBorder
    {
        private readonly ImageSource defaultImageSource = new BitmapImage();

        public Avatar()
        {
            InitializeComponent();
            //Loaded += (s,e)=>{ defaultImageSource = AvatarImage.Clone(); } ;
        }

        public double AvatarBorderThickness
        {
            get => BorderThickness.Left;
            set => BorderThickness = new Thickness(value);
        }

        public ImageSource AvatarImage
        {
            get => Background == null ? null : ((ImageBrush) Background).ImageSource;
            set
            {
                if (value == null)
                    Background = new ImageBrush(defaultImageSource);
                else
                    Background = new ImageBrush(value);
            }
        }
    }
}