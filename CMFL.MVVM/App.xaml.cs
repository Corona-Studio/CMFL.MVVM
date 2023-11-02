using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CMFL.MVVM.Class.Helper.Game;
using CMFL.MVVM.Class.Helper.Kernel;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.Other;
using CMFL.MVVM.ViewModels;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ProjBobcat.Class.Helper;

namespace CMFL.MVVM
{
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_INVALID_STATE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public uint AccentFlags;
        public uint GradientColor;
        public uint AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }

    /// <summary>
    ///     App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static readonly GcUtils GcUtils = new GcUtils();

        public App()
        {
            var splashScreen = new SplashScreen("\\Assets\\Images\\logo.png");
            splashScreen.Show(true);

            InitializeComponent();
            SetupAppCenterApi();
            Init();

            DispatcherHelper.Initialize();
            LogHelper.WriteLogLine(LanguageHelper.GetField("DispatcherInitialized"), LogHelper.LogLevels.Info);

            StartupUri = new Uri(
                $"pack://application:,,,/{(SettingsHelper.Settings.LiteMode ? "LiteWindow.xaml" : "MainWindow.xaml")}");
        }

        private static void SetupAppCenterApi()
        {
            var arch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            var filesList = new List<string>
            {
                $"{arch}\\e_sqlite3.dll",
                "SQLite-net.dll",
                "SQLitePCLRaw.batteries_green.dll",
                "SQLitePCLRaw.batteries_v2.dll",
                "SQLitePCLRaw.core.dll",
                "SQLitePCLRaw.provider.e_sqlite3.dll"
            };

            const string pathBase = "CMFL.MVVM.Assets.AppCenter.";
            var extractPath =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\CMFL\\Runtime\\";
            var additionExtractPath = $"{extractPath}runtimes\\win-{arch}\\native\\";

            if (!Directory.Exists(extractPath))
                Directory.CreateDirectory(extractPath);
            else
                try
                {
                    DirectoryHelper.CleanDirectory(extractPath, false);
                }
                catch (Exception)
                {
                    return;
                }

            if (!Directory.Exists(additionExtractPath))
                Directory.CreateDirectory(additionExtractPath);
            else
                try
                {
                    DirectoryHelper.CleanDirectory(additionExtractPath, false);
                }
                catch (Exception)
                {
                    return;
                }

            if (!Directory.Exists($"{extractPath}{arch}"))
                Directory.CreateDirectory($"{extractPath}{arch}");

            foreach (var f in filesList)
            {
                ResourceHelper.ExtractResFile($"{pathBase}{f}".FormatResourceName(), $"{extractPath}{f}");

                if (!f.Contains("x86") && !f.Contains("x64")) Assembly.LoadFile($"{extractPath}{f}");
            }

            var fF = filesList.First();
            ResourceHelper.ExtractResFile($"{pathBase}{fF}".FormatResourceName(),
                $"{additionExtractPath}e_sqlite3.dll");

            var countryCode = RegionInfo.CurrentRegion.TwoLetterISORegionName;
            AppCenter.SetCountryCode(countryCode);
            if (countryCode.Equals("CN", StringComparison.Ordinal))
                AppCenter.SetLogUrl("https://ac-proxy.gamerteam.cn");

            AppCenter.Start("ed36d991-c524-4036-bc62-2819f187fa02",
                typeof(Analytics), typeof(Crashes));
            Analytics.SetEnabledAsync(true);
        }

        /// <summary>
        ///     SSL有效性认证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            return true;
        }

        private static void Init()
        {
            SettingsHelper.GetSettingsFromDisk();

            GameHelper.InitGameCore();

            LogHelper.WriteLogLine(LanguageHelper.GetField("SettingsLoaded"), LogHelper.LogLevels.Info);

            LanguageHelper.ChangeLanguage(
                LanguageHelper.GetAllLanguageNames()[SettingsHelper.Settings.SelectedLanguageIndex]);

            DownloadHelper.SetUa(
                $"CMFL/{SettingsHelper.LauncherMajor}{SettingsHelper.LauncherMinor}{SettingsHelper.LauncherMirror}");

            var proc = Process.GetCurrentProcess();
            var count = Process.GetProcesses().Count(p => p.ProcessName == proc.ProcessName);
            if (count > 1)
            {
                MessageBox.Show(LanguageHelper.GetField("DoNotOpenTwoLauncher"),
                    LanguageHelper.GetField("LauncherName"), MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            LogHelper.WriteLogLine($"============{LanguageHelper.GetField("InitializationStarted")}============",
                LogHelper.LogLevels.Info);

            ServicePointManager.DefaultConnectionLimit = 512;

            Current.DispatcherUnhandledException += CurrentDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            LogHelper.WriteLogLine(LanguageHelper.GetField("ExceptionHandlerInitialized"), LogHelper.LogLevels.Info);

            GcUtils.Start();
            LogHelper.WriteLogLine(LanguageHelper.GetField("GcInitialized"), LogHelper.LogLevels.Info);

            if (!(SettingsHelper.Settings.JavasPath?.Any() ?? false))
            {
                ViewModelLocator.SettingPageViewModel.GetJavas();
                SettingsHelper.Save();
            }

            if (!SettingsHelper.Settings.LiteMode)
            {
                if (SettingsHelper.Settings.BoostMode)
                {
                    Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),
                        new FrameworkPropertyMetadata
                        {
                            DefaultValue = SettingsHelper.Settings.AnimationFps <= 0
                                ? 30
                                : SettingsHelper.Settings.AnimationFps
                        });
                }
                else
                {
                    if (!SettingsHelper.Settings.BoostMode)
                        Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),
                            new FrameworkPropertyMetadata
                            {
                                DefaultValue = 30
                            });
                }

                LogHelper.WriteLogLine(LanguageHelper.GetField("UiSettingsInitialized"), LogHelper.LogLevels.Info);
            }

            TextElement.FontFamilyProperty.OverrideMetadata(typeof(TextElement),
                new FrameworkPropertyMetadata(new FontFamily(SettingsHelper.Settings.SelectedInterfaceFont)));

            TextBlock.FontFamilyProperty.OverrideMetadata(typeof(TextBlock),
                new FrameworkPropertyMetadata(new FontFamily(SettingsHelper.Settings.SelectedInterfaceFont)));
        }

        /// <summary>
        ///     UI线程异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Crashes.TrackError(e.Exception);

            var ex = e.Exception;
            var sb = new StringBuilder();
            ex.GetAllExceptionString(ref sb);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ViewModelLocator.ExceptionPageViewModel.ErrorMessage = sb.ToString();
                ViewModelLocator.MainWindowViewModel.LeftMessageBorderVisibility = true;
                ViewModelLocator.MainWindowViewModel.LeftBorderIconColor = "#ff5252";
            });
            ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("ExceptionPage");
        }

        /// <summary>
        ///     非UI线程异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    Crashes.TrackError(ex);
                    var sb = new StringBuilder();
                    ex.GetAllExceptionString(ref sb);
                    ViewModelLocator.ExceptionPageViewModel.ErrorMessage =
                        $"{sb}{Environment.NewLine}{LanguageHelper.GetField("ClrExiting")}：{e.IsTerminating}";
                }

                ViewModelLocator.MainWindowViewModel.LeftMessageBorderVisibility = true;
                ViewModelLocator.MainWindowViewModel.LeftBorderIconColor = "#ff5252";
            });
            ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("ExceptionPage");
        }
    }
}