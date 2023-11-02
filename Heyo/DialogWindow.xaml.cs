using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Heyo.Pages;

namespace Heyo
{
    /// <summary>
    ///     DialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow(DialogPage page, Window owner = null)
        {
            InitializeComponent();
            Owner = owner;
            DialogFrame.Navigated += DialogFrame_Navigated;
            Loaded += DialogWindow_Loaded;
            CloseDialogButton.MouseDown += (s, e) => { Close(); };
            page.Closed += (s, e) => { Close(); };
            Page = page;
        }

        public DialogPage Page { get; set; }
        public double SpeedRatio { get; set; } = 1.4;
        public new event EventHandler Closed;

        public new void Close()
        {
            Closed?.Invoke(this, new EventArgs());
            var d = new DoubleAnimation();
            d.To = 0;
            var pe = new PowerEase();
            pe.EasingMode = EasingMode.EaseInOut;
            pe.Power = 4;
            d.EasingFunction = pe;
            d.SpeedRatio = SpeedRatio;
            d.Completed += (s, e) => { base.Close(); };
            BeginAnimation(OpacityProperty, d);
        }

        private void DialogFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var page = e.Content as DialogPage;

            if (page?.ShowBackButton == true)
                DialogTitle.Visibility = Visibility.Visible;
            //BackButton.Visibility = Visibility.Visible;
            if (page?.Title?.Length > 0)
            {
                DialogTitle.Visibility = Visibility.Visible;
                DialogTitleLabel.Content = page.Title;
            }

            if (page?.ShowCloseButton == true)
            {
                DialogTitle.Visibility = Visibility.Visible;
                CloseDialogButton.Visibility = Visibility.Visible;
            }

            //Width = page.Width;
            //Height = page.Height;
            UpdateLayout();
            if (Owner != null)
            {
                Left = Owner.Left + Owner.Width / 2 - ActualWidth / 2;
                Top = Owner.Top + Owner.Height / 2 - ActualHeight / 2;
            }
        }


        private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DialogFrame.Navigate(Page);
            var window = sender as Window;
            var d = new DoubleAnimation();
            d.From = 0;
            d.To = 1;
            var pe = new PowerEase();
            pe.EasingMode = EasingMode.EaseInOut;
            pe.Power = 4;
            d.SpeedRatio = SpeedRatio;
            d.EasingFunction = pe;
            d.Completed += (s, ee) => { Page.LoadCompleted(this, new EventArgs()); };
            window.BeginAnimation(OpacityProperty, d);
        }

        private void CloseDialogButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Page.Close();
        }

        private void DialogTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}