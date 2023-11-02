using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CMFL.MVVM.Class.Helper.Graphic;

namespace CMFL.MVVM.Controls
{
    /// <summary>
    ///     ImageBoxPlus.xaml 的交互逻辑
    /// </summary>
    public partial class ImageBoxPlus : UserControl, IDisposable
    {
        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register("ImageUri", typeof(Uri), typeof(ImageBoxPlus), new PropertyMetadata(null));

        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.Register("Link", typeof(string), typeof(ImageBoxPlus), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ImageBoxPlus), new PropertyMetadata(null));

        private readonly DoubleAnimation MainImgBlurDropAni = new DoubleAnimation
        {
            From = 20,
            To = 0,
            SpeedRatio = 7
        };

        private readonly DoubleAnimation MainImgBlurRaiseAni = new DoubleAnimation
        {
            From = 0,
            To = 20,
            SpeedRatio = 7
        };

        public ImageBoxPlus()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     所要加载的图片
        /// </summary>
        public Uri ImageUri
        {
            get => (Uri) GetValue(ImageUriProperty);
            set => SetValue(ImageUriProperty, value);
        }

        /// <summary>
        ///     访问的链接
        /// </summary>
        public string Link
        {
            get => (string) GetValue(LinkProperty);
            set => SetValue(LinkProperty, value);
        }

        /// <summary>
        ///     标题
        /// </summary>
        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private void ImageBoxPlus_OnLoaded(object sender, RoutedEventArgs e)
        {
            GetImage();
            TitleText.Width = Width - 50;
        }

        private async void GetImage()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    LoadingPanel.Visibility = Visibility.Visible;
                    ReloadButton.Visibility = Visibility.Collapsed;
                });

                var imageSource = await ImageHelper.GetImageSourceFromUri(ImageUri).ConfigureAwait(true);

                Dispatcher.Invoke(() =>
                {
                    ImageMain.Source = imageSource;
                    ImageBg.Source = imageSource;
                    TitleText.Text = Title;
                    LoadingPanel.Visibility = Visibility.Hidden;
                });
            }
            catch (WebException)
            {
                Dispatcher.Invoke(() => { ReloadButton.Visibility = Visibility.Visible; });
            }
        }

        private void ReloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            GetImage();
        }

        private void GoToLink(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Link)) Process.Start(Link);
        }

        #region IDisposible Support

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        ~ImageBoxPlus()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
        }

        #endregion
    }
}