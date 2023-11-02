using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.ViewModels;
using ProjCrypto.Class.Helper;

namespace CMFL.MVVM
{
    /// <summary>
    ///     WindowLogin.xaml 的交互逻辑
    /// </summary>
    public partial class WindowLogin : Window
    {
        public WindowLogin()
        {
            InitializeComponent();
            Instance = this;

            #region 登陆系统日志写入

            LogHelper.WriteLogLine(
                Environment.NewLine + "+----------------------------------+" +
                Environment.NewLine + "|                                  |" +
                Environment.NewLine + "| THIS MESSAGE                     |" +
                Environment.NewLine + "| WAS CREATED BY                   |" +
                Environment.NewLine + "| CRAFTMINEFUN LAUNCHER            |" +
                Environment.NewLine + "| LOGGING SYSTEM                   |" +
                Environment.NewLine + "|                                  |" +
                Environment.NewLine + "+----------------------------------+" +
                Environment.NewLine +
                Environment.NewLine + "+----------------------------------+" +
                Environment.NewLine + "+" +
                Environment.NewLine + "  IT'S " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " NOW." +
                Environment.NewLine + "  ATTEMPTING TO START." +
                Environment.NewLine + "                                   +" +
                Environment.NewLine + "+----------------------------------+",
                LogHelper.LogLevels.Info);

            #endregion

            #region 启动器登陆信息保存

            if (!SettingsHelper.Settings.IsRememberMe) return;

            ViewModelLocator.LoginWindowViewModel.LoginUsername = SettingsHelper.Settings.LUsername;
            ViewModelLocator.LoginWindowViewModel.LoginPassword =
                StringEncryptHelper.AesDecrypt(SettingsHelper.Settings.LPassword);
            ViewModelLocator.LoginWindowViewModel.RememberMe = SettingsHelper.Settings.IsRememberMe;

            #endregion
        }

        public static WindowLogin Instance { get; private set; }

        /// <summary>
        ///     关闭按钮
        /// </summary>
        private void CloseForm(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     窗口拖动
        /// </summary>
        private void CurrentWindowMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        #region 返回登陆页面事件

        private void BackToLogin(object sender, RoutedEventArgs e)
        {
            BottomArea.MoveTo(new Thickness(0), null);
            TopInfoMovePanel.MoveTo(new Thickness(0), null);
            LogoMovePanel.MoveTo(new Thickness(110, 0, 110, 48), null);
            LogoMovePanel.Anim();
            TopInfoMovePanel.Anim();
            BottomArea.Anim();
        }

        #endregion

        #region 前往注册页面事件

        private void GoToRegister(object sender, RoutedEventArgs e)
        {
            BottomArea.MoveTo(new Thickness(-361, 0, 0, 0), null);
            TopInfoMovePanel.MoveTo(new Thickness(-361, 0, 0, 0), null);
            LogoMovePanel.MoveTo(new Thickness(20, 0, 240, 10), null);
            LogoMovePanel.Anim();
            TopInfoMovePanel.Anim();
            BottomArea.Anim();
        }

        #endregion

        #region 前往登陆页面事件

        public void GotoLogin(object sender, RoutedEventArgs e)
        {
            CurrentPageTitle.Text = "注册";
            CurrentPageDetail.Text = "立刻拥有属于您的游戏账号吧！";
            BottomArea.MoveTo(new Thickness(0), null);
            TopInfoMovePanel.MoveTo(new Thickness(0), null);
            LogoMovePanel.MoveTo(new Thickness(110, 0, 110, 48), null);
            LogoMovePanel.Anim();
            TopInfoMovePanel.Anim();
            BottomArea.Anim();
        }

        #endregion

        #region 显示通知

        public void ShowToast(string title, string content, Color color, int time = 3000)
        {
            var showAnim =
                new DoubleAnimation
                {
                    To = 60,
                    SpeedRatio = 2,
                    EasingFunction = new BackEase {EasingMode = EasingMode.EaseInOut}
                };

            Dispatcher.Invoke(() =>
            {
                ToastTitle.Text = title;
                TotalDetail.Text = content;
                Toast.Visibility = Visibility.Visible;
                Toast.Background = new SolidColorBrush(color);
            });

            Toast.BeginAnimation(HeightProperty, showAnim);
            if (time > 0)
            {
                var timer = new DispatcherTimer();
                timer.Tick += (s, e) =>
                {
                    var hideAnim =
                        new DoubleAnimation
                        {
                            To = 0,
                            SpeedRatio = 2,
                            EasingFunction = new BackEase {EasingMode = EasingMode.EaseInOut}
                        };
                    Toast.BeginAnimation(HeightProperty, hideAnim);
                };
                timer.Interval = TimeSpan.FromMilliseconds(time);
                timer.Start();
            }
        }

        #endregion

        private async void TextBoxUsername_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
                await Dispatcher.InvokeAsync(() =>
                {
                    LoginPasswordBox.Focus();
                    LoginPasswordBox.SelectAll();
                });
        }

        private void GoToForgetPasswordPage(object sender, RoutedEventArgs e)
        {
            CurrentPageTitle.Text = "找回密码";
            CurrentPageDetail.Text = "忘记了密码？快速找回~";
            BottomArea.MoveTo(new Thickness(-722, 0, 0, 0), null);
            TopInfoMovePanel.MoveTo(new Thickness(-361, 0, 0, 0), null);
            LogoMovePanel.MoveTo(new Thickness(20, 0, 240, 10), null);
            LogoMovePanel.Anim();
            TopInfoMovePanel.Anim();
            BottomArea.Anim();
        }

        public void GoToVerifyPage(object sender, RoutedEventArgs e)
        {
            BottomArea.MoveTo(new Thickness(-1083, 0, 0, 0), null);
            BottomArea.Anim();
        }

        public void GoToSuccessPage(object sender, RoutedEventArgs e)
        {
            BottomArea.MoveTo(new Thickness(-1444, 0, 0, 0), null);
            BottomArea.Anim();
        }
    }
}