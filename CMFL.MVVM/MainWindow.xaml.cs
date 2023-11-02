using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using CMFL.MVVM.Class.Helper.Kernel;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.Launcher.SocketServer;
using CMFL.MVVM.ViewModels;

namespace CMFL.MVVM
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Key[] _keys =
            {Key.Up, Key.Up, Key.Down, Key.Down, Key.Left, Key.Right, Key.Left, Key.Right, Key.B, Key.A};

        private int _keyPosition;

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            if (ViewOperationBase.IsInDesignMode)
                return;

            ViewModelLocator.MainWindowViewModel.Initialize();
        }

        public static MainWindow Instance { get; private set; }

        #region 旧版Aero效果实现

        private void EnableBlurOld()
        {
            var windowHelper = new WindowInteropHelper(this);
            var accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
            };
            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);
            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            Dispatcher.Invoke(() => { NativeMethods.SetWindowCompositionAttribute(windowHelper.Handle, ref data); });

            Marshal.FreeHGlobal(accentPtr);

            Dispatcher.Invoke(() => { Window.BorderThickness = new Thickness(0, 0, 0, 0); });
        }

        #endregion

        private void CurrentWindowMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void MainWindow_OnActivated(object sender, EventArgs e)
        {
            if (SettingsHelper.Settings.EnableBlur &&
                SettingsHelper.Settings.BlurType.Equals("Default", StringComparison.Ordinal))
            {
                EnableBlurOld();
            }
            else
            {
                if (SettingsHelper.Settings.EnableBlur &&
                    SettingsHelper.Settings.BlurType.Equals("Acrylic", StringComparison.Ordinal))
                    EnableBlur();
            }
        }

        private void ActiveEasterEgg(object sender, KeyEventArgs e)
        {
            if (_keyPosition == _keys.Length - 1)
            {
                NotifyHelper.GetBasicMessageWithBadge("哇哦！你成功的发现了我。", NotifyHelper.MessageType.Success).Queue();
                _keyPosition = 0;
            }

            if (e.Key == _keys[_keyPosition])
                _keyPosition++;
            else
                _keyPosition = 0;
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!SettingsHelper.Settings.FirstTime)
                {
                    LauncherSocketServerHelper.CloseSocket();
                    SettingsHelper.Save();
                    ViewModelLocator.CleanUp();
                }

                ProcessHelper.RefreshTrayIcon();
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        #region 新版Acrylic效果实现

        private uint _blurOpacity;

        public double BlurOpacity
        {
            get => _blurOpacity;
            set
            {
                _blurOpacity = (uint) value;
                EnableBlur();
            }
        }

        private const uint BlurBackgroundColor = 0x990000; /* BGR color format */

        private void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);
            var accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                GradientColor = (_blurOpacity << 24) | (BlurBackgroundColor & 0xFFFFFF)
            };

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            Dispatcher.Invoke(() => { NativeMethods.SetWindowCompositionAttribute(windowHelper.Handle, ref data); });

            Marshal.FreeHGlobal(accentPtr);

            Dispatcher.Invoke(() => { Window.BorderThickness = new Thickness(0, 0, 0, 0); });
        }

        #endregion
    }
}