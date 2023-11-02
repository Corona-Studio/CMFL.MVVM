using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Game;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.UI;
using CMFL.MVVM.Models.DataModel.Game;
using CMFL.MVVM.Models.DataModel.Game.Account;
using CMFL.MVVM.Models.DataModel.Mojang;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Heyo.Class.Helper;
using MaterialDesignThemes.Wpf;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;
using ProjBobcat.Class.Model.LauncherProfile;
using ProjCrypto.Class.Helper;

namespace CMFL.MVVM.ViewModels
{
    public class SettingViewModel : PropertyChange, IDisposable
    {
        private ObservableCollection<AccountInfo> _accountInfos =
            new ObservableCollection<AccountInfo>(SettingsHelper.Settings.AccountInfos);

        private AccountProperties _accountProperties = new AccountProperties();
        private string _advanceLaunchSetting = SettingsHelper.Settings.FallBackGameSettings.AdvancedArguments;

        private int _animationFps = SettingsHelper.Settings.AnimationFps;

        private bool _autoCleanMemory = SettingsHelper.Settings.AutoCleanMemory;

        private bool _autoDetectBestDownloadServer = SettingsHelper.Settings.AutoDetectBestDownloadServer;

        private bool _autoMemory = SettingsHelper.Settings.FallBackGameSettings.AutoMemorySize;

        private string _bgPath = SettingsHelper.Settings.BgPath;

        private double _blurRadius = SettingsHelper.Settings.BlurRadius;

        private ObservableCollection<ConnectionTestModel> _connectionTestList =
            new ObservableCollection<ConnectionTestModel>();

        private string _connectionTestText = LanguageHelper.GetField("StartCheck");

        private bool _customFps = SettingsHelper.Settings.BoostMode;

        private bool _debugMode = SettingsHelper.Settings.DebugMode;

        private int _downloadThreads = SettingsHelper.Settings.DownloadThread.Equals(0)
            ? ProcessorHelper.GetPhysicalProcessorCount() * 2
            : SettingsHelper.Settings.DownloadThread;

        private bool _enableAcrylic =
            SettingsHelper.Settings.EnableBlur &&
            SettingsHelper.Settings.BlurType.Equals("Acrylic", StringComparison.Ordinal);

        private bool _enableAnimation = SettingsHelper.Settings.EnableAnimation;

        private bool _enableBmclApi = SettingsHelper.Settings.EnableBMCLAPI;

        private bool _enableGc = SettingsHelper.Settings.FallBackGameSettings.EnableGc;

        private bool _enableHttpsBmclApi;

        private bool _enableOldBlur =
            SettingsHelper.Settings.EnableBlur &&
            SettingsHelper.Settings.BlurType.Equals("Default", StringComparison.Ordinal);

        private bool _enableVersionInsulation = SettingsHelper.Settings.VersionInsulation;

        private List<string> _fontList = FontHelper.GetInstalledFontList();

        private bool _forceLaunch;

        private bool _isConnectionTestEnable = true;

        private bool _isDownloadJavaButtonEnable = true;

        private string _javaDownloadProgress;

        private ObservableCollection<string> _javas = new ObservableCollection<string>();

        private int _javaSelectedIndex = SettingsHelper.Settings.SelectedJavaIndex;

        private double _leftBorderBlurRadius = SettingsHelper.Settings.LeftBorderBlurRadius;

        private bool _leftBorderOpacityLayerVisibility = SettingsHelper.Settings.LeftBorderOpacityLayerVisibility;

        private bool _liteMode = SettingsHelper.Settings.LiteMode;

        private int _maxMemory = SettingsHelper.Settings.FallBackGameSettings.MaxMemory;

        private int _minMemory = SettingsHelper.Settings.FallBackGameSettings.MinMemory;

        private bool _randomWallpaper = SettingsHelper.Settings.RandomWallPaper;

        private bool _resumeMusic = SettingsHelper.Settings.ResumeMusic;

        private ScreenSetting _screenSetting = new ScreenSetting
        {
            ScreenX = SettingsHelper.Settings.FallBackGameSettings.ScreenSize.Width,
            ScreenY = SettingsHelper.Settings.FallBackGameSettings.ScreenSize.Height,
            IsFullScreen = SettingsHelper.Settings.FallBackGameSettings.ScreenSize.FullScreen
        };

        private int _selectedFontIndex = FontHelper.GetFontIndex(SettingsHelper.Settings.SelectedInterfaceFont);

        private int _selectedGcTypeIndex;

        private int _selectedLanguageIndex = SettingsHelper.Settings.SelectedLanguageIndex;

        private string _serverIp = SettingsHelper.Settings.FallBackGameSettings.ServerIp;

        private bool _useMojangServer = SettingsHelper.Settings.UseMojangServer;

        public SettingViewModel()
        {
            if (IsInDesignMode)
                return;
        }

        public int DownloadThreads
        {
            get => _downloadThreads;
            set
            {
                _downloadThreads = value;
                OnPropertyChanged(nameof(DownloadThreads));
            }
        }

        public bool ForceLaunch
        {
            get => _forceLaunch;
            set
            {
                _forceLaunch = value;
                OnPropertyChanged(nameof(ForceLaunch));
            }
        }

        public AccountProperties AccountProperties
        {
            get => _accountProperties;
            set
            {
                _accountProperties = value;
                OnPropertyChanged(nameof(AccountProperties));
            }
        }

        public ObservableCollection<AccountInfo> AccountInfos
        {
            get => _accountInfos;
            set
            {
                _accountInfos = value;
                OnPropertyChanged(nameof(AccountInfos));
            }
        }

        public bool EnableHttpsBmclApi
        {
            get => _enableHttpsBmclApi;
            set
            {
                _enableHttpsBmclApi = value;
                OnPropertyChanged(nameof(EnableHttpsBmclApi));
            }
        }

        public bool LeftBorderOpacityLayerVisibility
        {
            get => _leftBorderOpacityLayerVisibility;
            set
            {
                _leftBorderOpacityLayerVisibility = value;
                ViewModelLocator.MainWindowViewModel.LeftBorderOpacityLayerVisibility = value;
                OnPropertyChanged(nameof(LeftBorderOpacityLayerVisibility));
            }
        }

        public double LeftBorderBlurRadius
        {
            get => _leftBorderBlurRadius;
            set
            {
                _leftBorderBlurRadius = value;
                ViewModelLocator.MainWindowViewModel.LeftBorderBlurRadius = value;
                OnPropertyChanged(nameof(LeftBorderBlurRadius));
            }
        }

        public int MinMemory
        {
            get => _minMemory;
            set
            {
                _minMemory = value;
                OnPropertyChanged(nameof(MinMemory));
            }
        }

        public int MaxMemory
        {
            get => _maxMemory;
            set
            {
                _maxMemory = value;
                OnPropertyChanged(nameof(MinMemory));
            }
        }

        public bool AutoMemory
        {
            get => _autoMemory;
            set
            {
                _autoMemory = value;
                OnPropertyChanged(nameof(AutoMemory));
            }
        }

        public string ServerIp
        {
            get => _serverIp;
            set
            {
                _serverIp = value;
                OnPropertyChanged(nameof(ServerIp));
            }
        }

        public bool EnableGc
        {
            get => _enableGc;
            set
            {
                _enableGc = value;
                OnPropertyChanged(nameof(EnableGc));
            }
        }

        public string AdvanceLaunchSetting
        {
            get => _advanceLaunchSetting;
            set
            {
                _advanceLaunchSetting = value;
                OnPropertyChanged(nameof(AdvanceLaunchSetting));
            }
        }

        public ScreenSetting ScreenSetting
        {
            get => _screenSetting;
            set
            {
                _screenSetting = value;
                OnPropertyChanged(nameof(ScreenSetting));
            }
        }

        public bool EnableVersionInsulation
        {
            get => _enableVersionInsulation;
            set
            {
                _enableVersionInsulation = value;
                OnPropertyChanged(nameof(EnableVersionInsulation));
            }
        }

        public bool UseMojangServer
        {
            get => _useMojangServer;
            set
            {
                _useMojangServer = value;
                OnPropertyChanged(nameof(UseMojangServer));
            }
        }

        public bool EnableBmclApi
        {
            get => _enableBmclApi;
            set
            {
                _enableBmclApi = value;
                OnPropertyChanged(nameof(EnableBmclApi));
            }
        }

        public bool AutoDetectBestDownloadServer
        {
            get => _autoDetectBestDownloadServer;
            set
            {
                _autoDetectBestDownloadServer = value;
                OnPropertyChanged(nameof(AutoDetectBestDownloadServer));
            }
        }

        public ObservableCollection<string> Javas
        {
            get => _javas;
            set
            {
                _javas = value;
                OnPropertyChanged(nameof(Javas));
            }
        }

        public bool AutoCleanMemory
        {
            get => _autoCleanMemory;
            set
            {
                _autoCleanMemory = value;
                OnPropertyChanged(nameof(AutoCleanMemory));
            }
        }

        public bool CustomFps
        {
            get => _customFps;
            set
            {
                _customFps = value;
                OnPropertyChanged(nameof(CustomFps));
            }
        }

        public int AnimationFps
        {
            get => _animationFps;
            set
            {
                _animationFps = value;
                OnPropertyChanged(nameof(AnimationFps));
            }
        }

        public bool EnableOldBlur
        {
            get => _enableOldBlur;
            set
            {
                _enableOldBlur = value;
                OnPropertyChanged(nameof(EnableOldBlur));
            }
        }

        public bool EnableAcrylic
        {
            get => _enableAcrylic;
            set
            {
                _enableAcrylic = value;
                OnPropertyChanged(nameof(EnableAcrylic));
            }
        }

        public string BgPath
        {
            get => _bgPath;
            set
            {
                _bgPath = value;
                OnPropertyChanged(nameof(BgPath));
            }
        }

        public bool RandomWallpaper
        {
            get => _randomWallpaper;
            set
            {
                _randomWallpaper = value;
                OnPropertyChanged(nameof(RandomWallpaper));
            }
        }

        public double BlurRadius
        {
            get => _blurRadius;
            set
            {
                _blurRadius = value;
                ViewModelLocator.MainWindowViewModel.BlurRadius = value;
                OnPropertyChanged(nameof(BlurRadius));
            }
        }

        public bool IsDownloadJavaButtonEnable
        {
            get => _isDownloadJavaButtonEnable;
            set
            {
                _isDownloadJavaButtonEnable = value;
                OnPropertyChanged(nameof(IsDownloadJavaButtonEnable));
            }
        }

        public string JavaDownloadProgress
        {
            get => _javaDownloadProgress;
            set
            {
                _javaDownloadProgress = value;
                OnPropertyChanged(nameof(JavaDownloadProgress));
            }
        }

        public int JavaSelectedIndex
        {
            get => _javaSelectedIndex;
            set
            {
                _javaSelectedIndex = value;
                SettingsHelper.Settings.SelectedJavaIndex = _javaSelectedIndex;
                OnPropertyChanged(nameof(JavaSelectedIndex));
            }
        }

        public ObservableCollection<ConnectionTestModel> ConnectionTestList
        {
            get => _connectionTestList;
            set
            {
                _connectionTestList = value;
                OnPropertyChanged(nameof(ConnectionTestList));
            }
        }

        public string ConnectTestText
        {
            get => _connectionTestText;
            set
            {
                _connectionTestText = value;
                OnPropertyChanged(nameof(ConnectTestText));
            }
        }

        public bool IsConnectionTestEnable
        {
            get => _isConnectionTestEnable;
            set
            {
                _isConnectionTestEnable = value;
                OnPropertyChanged(nameof(IsConnectionTestEnable));
            }
        }

        public bool ResumeMusic
        {
            get => _resumeMusic;
            set
            {
                _resumeMusic = value;
                OnPropertyChanged(nameof(ResumeMusic));
            }
        }

        public bool DebugMode
        {
            get => _debugMode;
            set
            {
                _debugMode = value;
                OnPropertyChanged(nameof(DebugMode));
            }
        }

        public bool LiteMode
        {
            get => _liteMode;
            set
            {
                _liteMode = value;
                OnPropertyChanged(nameof(LiteMode));
            }
        }

        public bool EnableAnimation
        {
            get => _enableAnimation;
            set
            {
                _enableAnimation = value;
                OnPropertyChanged(nameof(EnableAnimation));
            }
        }

        public int SelectedGcTypeIndex
        {
            get => _selectedGcTypeIndex;
            set
            {
                _selectedGcTypeIndex = value;
                OnPropertyChanged(nameof(SelectedGcTypeIndex));
            }
        }


        public List<string> Language { get; } = LanguageHelper.GetAllLanguageNames();

        public int SelectedLanguageIndex
        {
            get => _selectedLanguageIndex;
            set
            {
                _selectedLanguageIndex = value;
                OnPropertyChanged(nameof(SelectedLanguageIndex));
            }
        }

        public List<string> FontList
        {
            get => _fontList;
            set
            {
                _fontList = value;
                OnPropertyChanged(nameof(FontList));
            }
        }

        public int SelectedFontIndex
        {
            get => _selectedFontIndex;
            set
            {
                _selectedFontIndex = value;
                OnPropertyChanged(nameof(SelectedFontIndex));
            }
        }

        public ICommand GoToDonatePageCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("DonatePage");
                });
            }
        }

        public ICommand AutoSetJavaCommand
        {
            get { return new DelegateCommand(obj => { GetJavas(); }); }
        }

        public ICommand DownloadJavaCommand
        {
            get { return new DelegateCommand(obj => { DownloadJavaAsync(); }); }
        }

        public ICommand OpenLogsFolderCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Process.Start($"{Environment.CurrentDirectory}\\CMFL\\LauncherLog\\");
                });
            }
        }

        public ICommand SetJavaManuallyCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using var javaFileDialog = new OpenFileDialog
                    {
                        InitialDirectory = "C:\\",
                        Filter = "javaw.exe|javaw.exe",
                        RestoreDirectory = true,
                        FilterIndex = 1
                    };
                    if (javaFileDialog.ShowDialog() != DialogResult.OK) return;

                    Javas.Insert(0, javaFileDialog.FileName);
                    JavaSelectedIndex = 0;

                    SettingsHelper.Settings.FallBackGameSettings.JavaPath = javaFileDialog.FileName;
                });
            }
        }

        public ICommand CleanMemoryCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    MemoryHelper.CleanAllMemory();
                    NotifyHelper.ShowNotification(LanguageHelper.GetField("CleanupCompleted"),
                        LanguageHelper.GetFields("Memory|CleanupCompleted"), 3000,
                        ToolTipIcon.Info);
                });
            }
        }

        public ICommand FindWallPaper
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    try
                    {
                        if (!Directory.Exists(Environment.CurrentDirectory + "\\CMFL\\WallPaper\\"))
                            Directory.CreateDirectory(Environment.CurrentDirectory + "\\CMFL\\WallPaper\\");
                        using var bgFileDialog = new OpenFileDialog
                        {
                            InitialDirectory = "WallPaper",
                            Filter = $"{LanguageHelper.GetField("Background")}|*.png;*.jpg;*.jpeg;*.gif;*.tif",
                            RestoreDirectory = true,
                            FilterIndex = 1
                        };
                        if (bgFileDialog.ShowDialog() != DialogResult.OK) return;

                        try
                        {
                            File.Copy(bgFileDialog.FileName,
                                Environment.CurrentDirectory + $"\\CMFL\\WallPaper\\{bgFileDialog.SafeFileName}");
                        }
                        catch (IOException ex)
                        {
                            LogHelper.WriteLogLine(
                                LanguageHelper.GetFields("CopyBackgroundImage|Failed"),
                                LogHelper.LogLevels.Error);
                            LogHelper.WriteError(ex);
                            NotifyHelper.ShowNotification(
                                LanguageHelper.GetFields("CopyBackgroundImage|Failed"),
                                LanguageHelper.GetField("PleaseCheckLog"), 3000, ToolTipIcon.Error);
                        }

                        var bgUri = new Uri($"{Environment.CurrentDirectory}\\CMFL\\WallPaper\\" +
                                            bgFileDialog.SafeFileName);

                        ViewModelLocator.MainWindowViewModel.BgImagePath =
                            new Uri(bgUri.AbsolutePath, UriKind.Absolute);

                        SettingsHelper.Settings.BgPath = bgUri.ToString();
                        BgPath = bgUri.ToString();
                    }
                    catch (IOException ex)
                    {
                        NotifyHelper.ShowNotification(
                            LanguageHelper.GetFields("ChangeBackground|Failed"),
                            LanguageHelper.GetField("PleaseCheckLog"), 3000, ToolTipIcon.Error);
                        LogHelper.WriteLogLine(
                            LanguageHelper.GetFields("ChangeBackground|Failed"),
                            LogHelper.LogLevels.Error);
                        LogHelper.WriteError(ex);
                    }
                });
            }
        }

        public ICommand OpenAboutPageCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("AboutPage");
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    try
                    {
                        LogHelper.WriteLogLine(" +----------------------------------+" +
                                               Environment.NewLine +
                                               $"丨 {LanguageHelper.GetField("StartSavingLauncherSettings")}                  丨" +
                                               Environment.NewLine + " +----------------------------------+",
                            LogHelper.LogLevels.Info, false);

                        LogHelper.WriteLogLine(" +----------------------------------+ ",
                            LogHelper.LogLevels.Info, false);

                        LogHelper.WriteLogLine($" 1.{LanguageHelper.GetField("BackgroundPath")}：{BgPath}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.BgPath = BgPath;

                        LogHelper.WriteLogLine($" 2.{LanguageHelper.GetField("JavaVm")}：{Javas.Count}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.JavasPath = new List<string>();
                        SettingsHelper.Settings.JavasPath.AddRange(Javas.ToArray());

                        if (Javas.Count == 0)
                        {
                            NotifyHelper.ShowNotification(LanguageHelper.GetField("NotSelectJava"),
                                LanguageHelper.GetField("NotSelectJavaWarning"), 3000,
                                ToolTipIcon.Warning);
                        }
                        else
                        {
                            LogHelper.WriteLogLine(
                                $" 3.{LanguageHelper.GetField("JavaPath")}：{Javas[JavaSelectedIndex]}",
                                LogHelper.LogLevels.Info, false);
                            SettingsHelper.Settings.FallBackGameSettings.JavaPath = Javas[JavaSelectedIndex];
                        }


                        LogHelper.WriteLogLine(
                            $" 4.{LanguageHelper.GetField("AnimationFps")}：{(CustomFps ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.BoostMode = CustomFps;

                        LogHelper.WriteLogLine(
                            $" 5.{LanguageHelper.GetField("AdvanceLaunchSetting")}：{AdvanceLaunchSetting}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.FallBackGameSettings.AdvancedArguments = AdvanceLaunchSetting;

                        LogHelper.WriteLogLine(
                            $" 6.{LanguageHelper.GetField("EnableBmclApi")}：{(EnableBmclApi && !UseMojangServer ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.EnableBMCLAPI = EnableBmclApi && !UseMojangServer;

                        LogHelper.WriteLogLine(
                            $" 7.{LanguageHelper.GetField("CleanMemoryAutomatic")}：{(AutoCleanMemory ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.AutoCleanMemory = AutoCleanMemory;

                        LogHelper.WriteLogLine(
                            $" 8.{LanguageHelper.GetField("EnableGc")}：{(EnableGc ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.FallBackGameSettings.EnableGc = EnableGc;
                        SettingsHelper.Settings.FallBackGameSettings.GcType = SelectedGcTypeIndex;

                        LogHelper.WriteLogLine(
                            $" 9.{LanguageHelper.GetField("AutoMemory")}：{(AutoMemory ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.FallBackGameSettings.AutoMemorySize = AutoMemory;

                        LogHelper.WriteLogLine($" 10.{LanguageHelper.GetField("BackgroundBlurRadius")}：{BlurRadius}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.BlurRadius = BlurRadius;

                        LogHelper.WriteLogLine(
                            $" 11.{LanguageHelper.GetField("FullScreen")}：{(ScreenSetting.IsFullScreen ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.FallBackGameSettings.ScreenSize.FullScreen = ScreenSetting.IsFullScreen;

                        LogHelper.WriteLogLine(
                            $" 12.{LanguageHelper.GetField("JoinServerAfterLaunch")}：{(string.IsNullOrEmpty(ServerIp) ? LanguageHelper.GetField("None") : ServerIp)}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.FallBackGameSettings.ServerIp = ServerIp;

                        LogHelper.WriteLogLine(
                            $" 13.{LanguageHelper.GetField("VersionInsulationSetting")}：{(EnableVersionInsulation ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.VersionInsulation = EnableVersionInsulation;

                        if (!ScreenSetting.IsFullScreen)
                        {
                            SettingsHelper.Settings.FallBackGameSettings.ScreenSize = new ResolutionModel
                            {
                                FullScreen = ScreenSetting.IsFullScreen,
                                Width = ScreenSetting.ScreenX,
                                Height = ScreenSetting.ScreenY
                            };
                            LogHelper.WriteLogLine(
                                $" 14.{LanguageHelper.GetField("ScreenResolution")}：{SettingsHelper.Settings.FallBackGameSettings.ScreenSize.Width} * {SettingsHelper.Settings.FallBackGameSettings.ScreenSize.Height}",
                                LogHelper.LogLevels.Info, false);
                        }

                        SettingsHelper.Settings.AnimationFps = CustomFps ? AnimationFps : 60;
                        LogHelper.WriteLogLine(
                            $" 15.{LanguageHelper.GetField("AnimationFpsHint")}：{SettingsHelper.Settings.AnimationFps}",
                            LogHelper.LogLevels.Info, false);

                        LogHelper.WriteLogLine(
                            $" 16.{LanguageHelper.GetField("OnlineMode")}：{(AccountInfo.GetSelectedAccount().IsOnlineAccount() ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);

                        LogHelper.WriteLogLine(
                            $" 17.{LanguageHelper.GetField("RandomBackground")}：{(RandomWallpaper ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.RandomWallPaper = RandomWallpaper;

                        LogHelper.WriteLogLine($" 18.{LanguageHelper.GetField("LaunchMemory")}：{MaxMemory} MB",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.FallBackGameSettings.MinMemory = MinMemory;
                        SettingsHelper.Settings.FallBackGameSettings.MaxMemory = MaxMemory;

                        LogHelper.WriteLogLine(
                            $" 19.{LanguageHelper.GetField("ResumeMusic")}：{(ResumeMusic ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.ResumeMusic = ResumeMusic;

                        LogHelper.WriteLogLine(
                            $" 20.{LanguageHelper.GetField("DetailedLogging")}：{(DebugMode ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.DebugMode = DebugMode;

                        LogHelper.WriteLogLine(
                            $" 21.{LanguageHelper.GetField("LiteMode")}：{(LiteMode ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.LiteMode = LiteMode;

                        LogHelper.WriteLogLine(
                            $" 22.{LanguageHelper.GetField("DetectServerAutomatic")}：{(AutoDetectBestDownloadServer ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.AutoDetectBestDownloadServer = AutoDetectBestDownloadServer;

                        LogHelper.WriteLogLine(
                            $" 23.{LanguageHelper.GetField("EnableAnimation")}：{(EnableAnimation ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.EnableAnimation = EnableAnimation;

                        LogHelper.WriteLogLine(
                            $" 24.{LanguageHelper.GetField("ForceLaunch")}：{(ForceLaunch ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.FallBackGameSettings.ForceLaunch = ForceLaunch;

                        LogHelper.WriteLogLine(
                            $" 25.{LanguageHelper.GetField("EnableHttpsForBmclapi")}：{(EnableHttpsBmclApi ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.UseHttpsForBmclapi = EnableHttpsBmclApi;

                        LogHelper.WriteLogLine(
                            $" 26.{LanguageHelper.GetField("Language")}：{Language[SelectedLanguageIndex]}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.SelectedLanguageIndex = SelectedLanguageIndex;
                        LanguageHelper.ChangeLanguage(Language[SelectedLanguageIndex]);

                        LogHelper.WriteLogLine(
                            $" 27.{LanguageHelper.GetField("InterfaceFont")}：{FontList[SelectedFontIndex]}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.SelectedInterfaceFont = FontList[SelectedFontIndex];

                        LogHelper.WriteLogLine(
                            $" 28.{LanguageHelper.GetField("LeftBorderBlurRadius")}：{LeftBorderBlurRadius}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.LeftBorderBlurRadius = LeftBorderBlurRadius;

                        LogHelper.WriteLogLine(
                            $" 29.{LanguageHelper.GetField("LeftBorderOpacityLayerVisibility")}：{(LeftBorderOpacityLayerVisibility ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.LeftBorderOpacityLayerVisibility = LeftBorderOpacityLayerVisibility;

                        LogHelper.WriteLogLine(
                            $" 30.{LanguageHelper.GetField("DownloadThreads")}：{DownloadThreads}",
                            LogHelper.LogLevels.Info, false);
                        SettingsHelper.Settings.DownloadThread = DownloadThreads;


                        if (EnableOldBlur && EnableAcrylic)
                        {
                            NotifyHelper.GetBasicMessageWithBadge(
                                LanguageHelper.GetField("SelectBothBlurTypeWarning"),
                                NotifyHelper.MessageType.Error).Queue();
                            EnableOldBlur = false;
                            EnableAcrylic = false;
                            return;
                        }

                        if (EnableOldBlur)
                        {
                            SettingsHelper.Settings.EnableBlur = true;
                            SettingsHelper.Settings.BlurType = "Default";
                        }
                        else
                        {
                            if (EnableAcrylic)
                            {
                                SettingsHelper.Settings.EnableBlur = true;
                                SettingsHelper.Settings.BlurType = "Acrylic";
                            }
                            else
                            {
                                if (!EnableAcrylic && !EnableOldBlur) SettingsHelper.Settings.EnableBlur = false;
                            }
                        }

                        LogHelper.WriteLogLine(
                            Environment.NewLine + " +----------------------------------+ " +
                            Environment.NewLine,
                            LogHelper.LogLevels.Info, false);

                        SettingsHelper.Save();

                        NotifyHelper.GetBasicMessageWithBadge(
                            LanguageHelper.GetFields("Save|Succeeded"),
                            NotifyHelper.MessageType.Success).Queue();
                    }
                    catch (NullReferenceException e)
                    {
                        NotifyHelper.GetBasicMessageWithBadge(
                            LanguageHelper.GetFields("Save|Succeeded|PleaseCheckAllSettings"),
                            NotifyHelper.MessageType.Error).Queue();
                        LogHelper.WriteLogLine(LanguageHelper.GetFields("Save|Failed"),
                            LogHelper.LogLevels.Error);
                        LogHelper.WriteError(e);
                    }
                });
            }
        }

        public ICommand ServerTestCommand
        {
            get { return new DelegateCommand(obj => { ConnectionTest(); }); }
        }

        public ICommand SaveAccountInfoCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (AccountProperties.IsOnline)
                    {
                        var online = new OnlineAccount
                        {
                            Email = StringEncryptHelper.StringEncrypt(AccountProperties.Email),
                            Password = StringEncryptHelper.StringEncrypt(AccountProperties.Password),
                            AuthServer = string.IsNullOrEmpty(AccountProperties.AuthLibServer)
                                ? null
                                : AccountProperties.AuthLibServer,
                            LastAuthTime = DateTime.Now,
                            SearchGuid = Guid.NewGuid().ToString("N")
                        };
                        SettingsHelper.Settings.AccountInfos.Add(online);
                        AccountInfos.Add(online);
                        AccountProperties = new AccountProperties();
                        SettingsHelper.Save();
                        DialogHost.CloseDialogCommand.Execute(null, null);
                        return;
                    }

                    var offline = new OfflineAccount
                    {
                        DisplayName = AccountProperties.DisplayName,
                        SkinPath = string.IsNullOrEmpty(AccountProperties.SkinPath) ? null : AccountProperties.SkinPath,
                        SearchGuid = Guid.NewGuid().ToString("N")
                    };
                    SettingsHelper.Settings.AccountInfos.Add(offline);
                    AccountInfos.Add(offline);
                    AccountProperties = new AccountProperties();

                    SettingsHelper.Save();
                    DialogHost.CloseDialogCommand.Execute(null, null);
                });
            }
        }

        /// <summary>
        ///     搜索Java
        /// </summary>
        public void GetJavas()
        {
            var javaPath = SystemInfoHelper.FindJava();
            var javasList = javaPath.ToList();
            if (javasList.Any())
            {
                if (SettingsHelper.Settings.JavasPath != null && SettingsHelper.Settings.JavasPath.Count > 0)
                    SettingsHelper.Settings.JavasPath.Clear();

                SettingsHelper.Settings.JavasPath = javasList;

                if (javasList.Count > 1)
                    NotifyHelper.GetBasicMessageWithBadge(
                        LanguageHelper.GetField("FoundJavasHint"),
                        NotifyHelper.MessageType.Info).Queue();

                Javas = new ObservableCollection<string>(javasList);
                SettingsHelper.Settings.FallBackGameSettings.JavaPath = Javas[0];
            }
            else
            {
                NotifyHelper.ShowNotification(
                    LanguageHelper.GetFields("SearchJava|Failed"),
                    LanguageHelper.GetField("PleaseCheckLog"), 3000,
                    ToolTipIcon.Error);
                NotifyHelper.GetBasicMessageWithBadge(
                    LanguageHelper.GetFields("SearchJava|Failed"),
                    NotifyHelper.MessageType.Error).Queue();
                LogHelper.WriteLogLine(LanguageHelper.GetField("NoJavaFoundHint"), LogHelper.LogLevels.Error);
            }
        }

        /// <summary>
        ///     下载Java
        /// </summary>
        private void DownloadJavaAsync()
        {
            if (!IsDownloadJavaButtonEnable)
                return;

            IsDownloadJavaButtonEnable = false;

            var sysModel = Class.Helper.Kernel.SystemInfoHelper.SysInfo["SysModel"]
                .Equals("32Bits", StringComparison.Ordinal)
                ? "32"
                : "64";
            var filePath = $"{Environment.CurrentDirectory}\\CMFL\\Download\\java{sysModel}.exe";
            var downloadNeeded = false;

            if (File.Exists(filePath))
                try
                {
                    Process.Start(filePath);
                }
                catch
                {
                    downloadNeeded = true;
                }

            if (downloadNeeded)
                return;

            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                NotifyHelper.ShowNotification(
                    LanguageHelper.GetFields("DownloadJava|Failed"),
                    LanguageHelper.GetField("JavaDownloadFailedText"), 3000, ToolTipIcon.Error);
                LogHelper.WriteLogLine(LanguageHelper.GetFields("Download|Failed"),
                    LogHelper.LogLevels.Error);
                LogHelper.WriteError(ex);
            }

            BMCLHelper.GetJavaDownloadLinks().ContinueWith(async task =>
            {
                try
                {
                    if (!Directory.Exists($"{Environment.CurrentDirectory}\\CMFL\\Download\\"))
                        Directory.CreateDirectory($"{Environment.CurrentDirectory}\\CMFL\\Download\\");

                    var dF = new DownloadFile
                    {
                        Changed = (sender, args) =>
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                JavaDownloadProgress = args.ProgressPercentage.ToString("P1");
                            });
                        },
                        Completed = (sender, args) =>
                        {
                            File.Move($"{Environment.CurrentDirectory}\\CMFL\\Download\\java{sysModel}.huaji",
                                filePath);
                            Process.Start(filePath);

                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                NotifyHelper.GetBasicMessageWithBadge(
                                    LanguageHelper.GetFields("Download|Succeeded"),
                                    NotifyHelper.MessageType.Success).Queue();
                                IsDownloadJavaButtonEnable = true;
                            });
                        },
                        DownloadPath = $"{Environment.CurrentDirectory}\\CMFL\\Download\\java{sysModel}.huaji",
                        DownloadUri = task.Result.Value[sysModel],
                        FileName = $"java{sysModel}.huaji"
                    };

                    await DownloadHelper.MultiPartDownloadTaskAsync(dF, 16).ConfigureAwait(false);
                }
                catch (WebException ex)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { IsDownloadJavaButtonEnable = true; });
                    NotifyHelper.ShowNotification(
                        LanguageHelper.GetFields("DownloadJava|Failed"),
                        LanguageHelper.GetField("JavaDownloadFailedText"), 3000, ToolTipIcon.Error);
                    LogHelper.WriteLogLine(LanguageHelper.GetFields("Download|Failed"),
                        LogHelper.LogLevels.Error);
                    LogHelper.WriteError(ex);
                }
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        private async void ConnectionTest()
        {
            try
            {
                ConnectionTestList.Clear();
                ConnectTestText = LanguageHelper.GetField("PleaseWait");
                IsConnectionTestEnable = false;

                var iconColor = "#DDFFFFFF";
                var statusTextColor = "#DDFFFFFF";
                var status = string.Empty;
                var icon = PackIconKind.CheckCircle;

                var mojangServerStatus = await MojangApi.ApiCheck().ConfigureAwait(true);
                // var serverJson = await webClient.DownloadStringTaskAsync("http://ddns.craftminefun.com:3001/").ConfigureAwait(true);
                // var serverTestJson = JsonConvert.DeserializeObject<List<ServerTestJson>>(serverJson.Replace("&#34;", "\""));

                if (!mojangServerStatus.Any())
                    throw new NullReferenceException();

                foreach (var item in mojangServerStatus)
                {
                    switch (item.Value)
                    {
                        case ServerStatus.green:
                            icon = PackIconKind.CheckCircle;
                            iconColor = "#FF00C853";
                            statusTextColor = "#DDFFFFFF";
                            status = LanguageHelper.GetField("Normal");
                            break;
                        case ServerStatus.yellow:
                            icon = PackIconKind.AlertCircle;
                            iconColor = "#FFCA28";
                            statusTextColor = "#FFCA28";
                            status = LanguageHelper.GetField("Warning");
                            break;
                        case ServerStatus.red:
                            icon = PackIconKind.CloseCircle;
                            iconColor = "#FF5252";
                            statusTextColor = "#FF5252";
                            status = LanguageHelper.GetField("Error");
                            break;
                    }

                    ConnectionTestList.Add(new ConnectionTestModel
                    {
                        Icon = icon,
                        IconColor = iconColor,
                        Name = item.Key,
                        StatusText = status,
                        StatusTextColor = statusTextColor
                    });
                }

                ConnectTestText = LanguageHelper.GetField("StartCheck");
                IsConnectionTestEnable = true;
            }
            catch (NullReferenceException e)
            {
                ConnectTestText = LanguageHelper.GetField("StartCheck");
                IsConnectionTestEnable = true;

                NotifyHelper.GetBasicMessageWithBadge(
                    LanguageHelper.GetFields("ServerConnectionTest|Failed"),
                    NotifyHelper.MessageType.Error).Queue();
                LogHelper.WriteLogLine(
                    LanguageHelper.GetFields("ServerConnectionTest|Failed"),
                    LogHelper.LogLevels.Error);
                LogHelper.WriteError(e);
            }
            catch (WebException e)
            {
                ConnectTestText = LanguageHelper.GetField("StartCheck");
                IsConnectionTestEnable = true;
                NotifyHelper.GetBasicMessageWithBadge(
                    LanguageHelper.GetFields("ServerConnectionTest|Failed"),
                    NotifyHelper.MessageType.Error).Queue();
                LogHelper.WriteLogLine(
                    LanguageHelper.GetFields("ServerConnectionTest|Failed"),
                    LogHelper.LogLevels.Error);
                LogHelper.WriteError(e);
            }
        }

        #region IDisposable Support

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        ~SettingViewModel()
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