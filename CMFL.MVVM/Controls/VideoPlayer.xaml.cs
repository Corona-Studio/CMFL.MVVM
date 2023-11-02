using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CMFL.MVVM.Class.Helper.Graphic;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.ViewModels;

namespace CMFL.MVVM.Controls
{
    /// <summary>
    ///     UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class VideoPlayer : UserControl, IDisposable
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(VideoPlayer),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ExplainTextProperty =
            DependencyProperty.Register("ExplainText", typeof(string), typeof(VideoPlayer),
                new PropertyMetadata(null));

        public static readonly DependencyProperty VideoSourceProperty =
            DependencyProperty.Register("VideoSource", typeof(string), typeof(VideoPlayer),
                new PropertyMetadata(null));

        public static readonly DependencyProperty BgImageLinkProperty =
            DependencyProperty.Register("BgImageLink", typeof(string), typeof(VideoPlayer),
                new PropertyMetadata(null));

        private readonly DispatcherTimer _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(60)
        };

        private bool _isLoadedVideo;

        public VideoPlayer()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string ExplainText
        {
            get => (string) GetValue(ExplainTextProperty);
            set => SetValue(ExplainTextProperty, value);
        }

        public string VideoSource
        {
            get => (string) GetValue(VideoSourceProperty);
            set => SetValue(VideoSourceProperty, value);
        }

        public string BgImageLink
        {
            get => (string) GetValue(BgImageLinkProperty);
            set => SetValue(BgImageLinkProperty, value);
        }

        private async void GetBgImage()
        {
            try
            {
                var imageSource = await ImageHelper.GetImageSourceFromUri(new Uri(BgImageLink)).ConfigureAwait(true);
                BackgroundImage.Source = imageSource;
            }
            catch (WebException)
            {
                LogHelper.WriteLogLine(LanguageHelper.GetField("VideoCoverNotFound"), LogHelper.LogLevels.Error);
            }
        }

        private void LoadVideo()
        {
            try
            {
                BackgroundImage.Visibility = Visibility.Hidden;
                LoadingProgressBar.Visibility = Visibility.Visible;
                PlayerIndicatorText.Text = LanguageHelper.GetField("LoadingVideo");
                Video.Source = new Uri(VideoSource);

                Video.MediaOpened += (o, args) =>
                {
                    PlayVideoButton.IsChecked = true;
                    LoadingPanel.Visibility = Visibility.Hidden;
                    VideoSlider.Maximum = Video.NaturalDuration.TimeSpan.TotalSeconds;
                    _timer.Tick += (sender1, eventArgs) => { VideoSlider.Value = Video.Position.TotalSeconds; };
                    _timer.Start();
                };

                Video.MediaEnded += (o, args) => { _timer.Stop(); };

                Video.Play();
            }
            catch (InvalidOperationException ex)
            {
                BackgroundImage.Visibility = Visibility.Visible;
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                PlayerIndicatorText.Text = LanguageHelper.GetField("ClickToLoadVideo");
                LogHelper.WriteLogLine(LanguageHelper.GetFields("LoadVideoFailedReason|UnknownOperation"),
                    LogHelper.LogLevels.Error);
                LogHelper.WriteError(ex);
            }
            catch (UriFormatException ex)
            {
                BackgroundImage.Visibility = Visibility.Visible;
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                PlayerIndicatorText.Text = LanguageHelper.GetField("ClickToLoadVideo");
                LogHelper.WriteLogLine(LanguageHelper.GetFields("LoadVideoFailedReason|IllegalUrl"),
                    LogHelper.LogLevels.Error);
                LogHelper.WriteError(ex);
            }
        }

        private void ChangePosition(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Video.Position = TimeSpan.FromSeconds(e.NewValue);
        }

        private void ChangeVideoState(object sender, RoutedEventArgs e)
        {
            if (!_isLoadedVideo)
            {
                ViewModelLocator.HomePageViewModel.BgmControl.IsMusicMuted = true;
                LoadVideo();
                PlayVideoButton.IsChecked = true;
                _isLoadedVideo = true;
                return;
            }

            if (PlayVideoButton.IsChecked != null && !PlayVideoButton.IsChecked.Value)
            {
                ViewModelLocator.HomePageViewModel.BgmControl.IsMusicMuted = false;
                PlayVideoButton.IsChecked = false;
                Video.Pause();
            }
            else
            {
                ViewModelLocator.HomePageViewModel.BgmControl.IsMusicMuted = true;
                PlayVideoButton.IsChecked = true;
                Video.Play();
            }
        }

        private void BackgroundImage_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(BgImageLink)) GetBgImage();

            PlayerIndicatorText.Text = Title;
            IntroText.Text = ExplainText;
            PlayerIndicatorText.MaxWidth = Width - 60;
            IntroText.MaxWidth = Width - 60;
            VideoSlider.Width = Width - 100;
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
        ~VideoPlayer()
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