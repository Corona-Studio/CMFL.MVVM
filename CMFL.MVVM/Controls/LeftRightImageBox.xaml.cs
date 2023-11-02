using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using CMFL.MVVM.Models.DataModel.Launcher;

namespace CMFL.MVVM.Controls
{
    /// <summary>
    ///     LeftRightImageBox.xaml 的交互逻辑
    /// </summary>
    public partial class LeftRightImageBox : UserControl, IDisposable
    {
        public static readonly DependencyProperty ImagesProperty =
            DependencyProperty.Register("Images", typeof(List<LRImageBoxModel>), typeof(LeftRightImageBox),
                new PropertyMetadata(null));

        private readonly Timer _timer = new Timer(5000)
        {
            AutoReset = true,
            Enabled = true
        };

        public LeftRightImageBox()
        {
            InitializeComponent();
        }

        private int Count { get; set; }
        private int PageNumber { get; set; }
        private int Position { get; set; }

        /// <summary>
        ///     所要切换的图片集合
        /// </summary>
        public List<LRImageBoxModel> Images
        {
            get => (List<LRImageBoxModel>) GetValue(ImagesProperty);
            set => SetValue(ImagesProperty, value);
        }

        private void GoToLeft(object sender, RoutedEventArgs e)
        {
            MoveLeft();
        }

        private void GoToRihgt(object sender, RoutedEventArgs e)
        {
            MoveRight();
        }

        private void LeftRightImageBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            GetContent();
            AutoMove();
        }

        private void MoveLeft()
        {
            Dispatcher.Invoke(() =>
            {
                PageNumber--;
                if (PageNumber < 0)
                {
                    PageNumber = Count - 1;
                    Position = (Count - 1) * -500;
                    ContentAnimationPanel.MoveTo(new Thickness(Position, 0, 0, 0), null);
                    ContentAnimationPanel.Anim();
                    return;
                }

                Position += 500;
                ContentAnimationPanel.MoveTo(new Thickness(Position, 0, 0, 0), null);
                ContentAnimationPanel.Anim();
            });
        }

        private void MoveRight()
        {
            Dispatcher.Invoke(() =>
            {
                PageNumber++;
                if (PageNumber >= Count)
                {
                    PageNumber = 0;
                    Position = 0;
                    ContentAnimationPanel.MoveTo(new Thickness(Position, 0, 0, 0), null);
                    ContentAnimationPanel.Anim();
                    return;
                }

                Position -= 500;
                ContentAnimationPanel.MoveTo(new Thickness(Position, 0, 0, 0), null);
                ContentAnimationPanel.Anim();
            });
        }

        private void GetContent()
        {
            Count = Images?.Count ?? 0;

            if (Count == 0)
            {
                Dispatcher.Invoke(() => { LoadingErrorBorder.Visibility = Visibility.Visible; });
                return;
            }

            if (Images != null)
                foreach (var imageUri in Images)
                    Dispatcher.Invoke(() =>
                    {
                        ContentPanel.Children.Add(new ImageBoxPlus
                        {
                            ImageUri = imageUri.ImageUri,
                            Link = imageUri.Link,
                            Title = imageUri.Title
                        });
                    });
        }

        private void AutoMove()
        {
            _timer.Elapsed += (sender, args) => { MoveRight(); };
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
        ~LeftRightImageBox()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                // free managed resources
                _timer.Dispose();
        }

        #endregion
    }
}