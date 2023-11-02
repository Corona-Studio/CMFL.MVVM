using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CMFL.MVVM.Class.Helper.Game;
using CMFL.MVVM.Class.Helper.Graphic;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.Launcher.SocketServer;
using CMFL.MVVM.Class.Helper.RandomAvatarHelper;
using CMFL.MVVM.Class.Helper.Web;
using CMFL.MVVM.Models.DataModel.Game.Account;
using CMFL.MVVM.Models.DataModel.Launcher;
using CMFL.MVVM.Models.DataModel.Launcher.Auth;
using CommonServiceLocator;
using Enterwell.Clients.Wpf.Notifications;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Heyo.Class.Helper;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;
using ProjBobcat.DefaultComponent;
using ProjBobcat.DefaultComponent.ResourceInfoResolver;
using ProjBobcat.Interface;
using ProjCrypto;
using ProjCrypto.Class.Helper;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;

namespace CMFL.MVVM.ViewModels
{
    public class MainWindowViewModel : PropertyChange, IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private bool _avatarLoadingVisibility;

        private double _avatarOpacity = 0.8;

        private ImageSource _avatarSource =
            RandomAvatarBuilder.Build(100).SetPadding(25).ToImage().DrawingImageToImageSource();

        private string _avatarToolTip = LanguageHelper.GetField("ToolTipEasterEgg2");

        private Uri _bgImagePath = new Uri(SettingsHelper.Settings.BgPath, UriKind.RelativeOrAbsolute);

        private bool _bgVisibility = !SettingsHelper.Settings.EnableBlur;

        private double _blurRadius = SettingsHelper.Settings.BlurRadius;

        private string _choseGame =
            string.IsNullOrEmpty(SettingsHelper.Settings.ChoseGame)
                ? LanguageHelper.GetField("PleaseSelectGameFirst")
                : SettingsHelper.Settings.ChoseGame;

        private bool _isBottomDrawerOpen = true;

        private bool _isGameStarting;

        private bool _isLoadingTextVisible;

        private bool _isStartGameButtonIndeterminate;

        private double _leftBorderBlurRadius = SettingsHelper.Settings.LeftBorderBlurRadius;

        private string _leftBorderIconColor = "#FF00E676";

        private bool _leftBorderOpacityLayerVisibility = SettingsHelper.Settings.LeftBorderOpacityLayerVisibility;

        private bool _leftMessageBorderVisibility = SettingsHelper.Settings.FirstTime;

        private string _loadingTextContent = LanguageHelper.GetField("Wait");

        private string _loadingTextTitle = LanguageHelper.GetField("PleaseWait");

        private SolidColorBrush _loginButtonBackground = new SolidColorBrush(Color.FromRgb(96, 125, 139));

        private string _loginButtonContent = LanguageHelper.GetField("Login");

        private string _loginButtonToolTip;

        private bool _loginInfoVisibility = true;

        private Visibility _loginPanelVisibility =
            SettingsHelper.Settings.IsConnectedToCMF ? Visibility.Visible : Visibility.Collapsed;

        private Visibility _reconnectButtonVisibility =
            SettingsHelper.Settings.IsConnectedToCMF ? Visibility.Collapsed : Visibility.Visible;

        private string _startGameText = LanguageHelper.GetField("Start");

        private Thickness _thickness = new Thickness(20);

        private string _welcomeText =
            $"{LanguageHelper.GetField("Welcome")}，" + (SettingsHelper.Settings.IsConnectedToCMF &&
                                                        SettingsHelper.Settings.LoggedInToCMFL &&
                                                        SettingsHelper.Settings.IsRememberMe
                ? SettingsHelper.Settings.LUsername
                : AccountInfo.GetSelectedAccount() == null
                    ? ""
                    : AccountInfo.GetSelectedAccount().DisplayName);

        private WindowLogin _windowLogin;

        private WindowState _windowState = WindowState.Normal;

        private bool _windowTopMost;

        public MainWindowViewModel()
        {
            if (IsInDesignMode)
                return;

            if (!string.IsNullOrEmpty(SettingsHelper.Settings.BgPath) && !SettingsHelper.Settings.EnableBlur &&
                !SettingsHelper.Settings.RandomWallPaper &&
                Directory.Exists($@"{Environment.CurrentDirectory}\CMFL\WallPaper\"))
            {
                BgImagePath = new Uri(SettingsHelper.Settings.BgPath, UriKind.RelativeOrAbsolute);
            }
            else
            {
                if (!string.IsNullOrEmpty(SettingsHelper.Settings.BgPath) && !SettingsHelper.Settings.EnableBlur &&
                    SettingsHelper.Settings.RandomWallPaper &&
                    Directory.Exists($@"{Environment.CurrentDirectory}\CMFL\WallPaper\"))
                {
                    var directoryInfo = new DirectoryInfo($@"{Environment.CurrentDirectory}\CMFL\WallPaper\");
                    var files = directoryInfo.GetFiles();
                    if (files.Length > 1)
                    {
                        BgImagePath = new Uri(files.RandomSample().FullName, UriKind.Absolute);
                    }
                    else
                    {
                        if (files.Length != 0) BgImagePath = new Uri(files.First().FullName, UriKind.Absolute);
                    }
                }
            }

            if (SettingsHelper.Settings.IsRememberMe && SettingsHelper.Settings.IsConnectedToCMF)
                LoginToCmfl();
            GetMusic();
        }

        private Dispatcher Dispatcher { get; } = Application.Current.Dispatcher;

        public string LoginButtonContent
        {
            get => _loginButtonContent;
            set
            {
                _loginButtonContent = value;
                OnPropertyChanged(nameof(LoginButtonContent));
            }
        }

        public double AvatarOpacity
        {
            get => _avatarOpacity;
            set
            {
                _avatarOpacity = value;
                OnPropertyChanged(nameof(AvatarOpacity));
            }
        }

        public bool LoginInfoVisibility
        {
            get => _loginInfoVisibility;
            set
            {
                _loginInfoVisibility = value;
                OnPropertyChanged(nameof(LoginInfoVisibility));
            }
        }

        public bool AvatarLoadingVisibility
        {
            get => _avatarLoadingVisibility;
            set
            {
                _avatarLoadingVisibility = value;
                OnPropertyChanged(nameof(AvatarLoadingVisibility));
            }
        }

        public Visibility LoginPanelVisibility
        {
            get => _loginPanelVisibility;
            set
            {
                _loginPanelVisibility = value;
                OnPropertyChanged(nameof(LoginPanelVisibility));
            }
        }

        public ImageSource AvatarSource
        {
            get => _avatarSource;
            set
            {
                _avatarSource = value;
                OnPropertyChanged(nameof(AvatarSource));
            }
        }

        public SolidColorBrush LoginButtonBackground
        {
            get => _loginButtonBackground;
            set
            {
                _loginButtonBackground = value;
                OnPropertyChanged(nameof(LoginButtonBackground));
            }
        }

        public string LoginButtonToolTip
        {
            get => _loginButtonToolTip;
            set
            {
                _loginButtonToolTip = value;
                OnPropertyChanged(nameof(LoginButtonToolTip));
            }
        }

        public bool BgVisibility
        {
            get => _bgVisibility;
            set
            {
                _bgVisibility = value;
                OnPropertyChanged(nameof(BgVisibility));
            }
        }

        public Thickness Thickness
        {
            get => _thickness;
            set
            {
                _thickness = value;
                OnPropertyChanged(nameof(Thickness));
            }
        }

        public string WelcomeText
        {
            get => _welcomeText;
            set
            {
                _welcomeText = value;
                OnPropertyChanged(nameof(WelcomeText));
            }
        }

        public Uri BgImagePath
        {
            get => _bgImagePath;
            set
            {
                _bgImagePath = value;
                OnPropertyChanged(nameof(BgImagePath));
            }
        }

        public double BlurRadius
        {
            get => _blurRadius;
            set
            {
                _blurRadius = value;
                OnPropertyChanged(nameof(BlurRadius));
            }
        }

        public double LeftBorderBlurRadius
        {
            get => _leftBorderBlurRadius;
            set
            {
                _leftBorderBlurRadius = value;
                OnPropertyChanged(nameof(LeftBorderBlurRadius));
            }
        }

        public bool LeftBorderOpacityLayerVisibility
        {
            get => _leftBorderOpacityLayerVisibility;
            set
            {
                _leftBorderOpacityLayerVisibility = value;
                OnPropertyChanged(nameof(LeftBorderOpacityLayerVisibility));
            }
        }

        public string ChoseGame
        {
            get => _choseGame;
            set
            {
                _choseGame = value;
                OnPropertyChanged(nameof(ChoseGame));
            }
        }

        public string StartGameText
        {
            get => _startGameText;
            set
            {
                _startGameText = value;
                OnPropertyChanged(nameof(StartGameText));
            }
        }

        public bool LeftMessageBorderVisibility
        {
            get => _leftMessageBorderVisibility;
            set
            {
                _leftMessageBorderVisibility = value;
                OnPropertyChanged(nameof(LeftMessageBorderVisibility));
            }
        }

        public string LeftBorderIconColor
        {
            get => _leftBorderIconColor;
            set
            {
                _leftBorderIconColor = value;
                OnPropertyChanged(nameof(LeftBorderIconColor));
            }
        }

        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                OnPropertyChanged(nameof(WindowState));
            }
        }

        public bool WindowTopMost
        {
            get => _windowTopMost;
            set
            {
                _windowTopMost = value;
                OnPropertyChanged(nameof(WindowTopMost));
            }
        }

        public string AvatarToolTip
        {
            get => _avatarToolTip;
            set
            {
                _avatarToolTip = value;
                OnPropertyChanged(nameof(AvatarToolTip));
            }
        }

        public Visibility ReconnectButtonVisibility
        {
            get => _reconnectButtonVisibility;
            set
            {
                _reconnectButtonVisibility = value;
                OnPropertyChanged(nameof(ReconnectButtonVisibility));
            }
        }

        public bool IsBottomDrawerOpen
        {
            get => _isBottomDrawerOpen;
            set
            {
                _isBottomDrawerOpen = value;
                OnPropertyChanged(nameof(IsBottomDrawerOpen));
            }
        }

        public bool IsLoadingTextVisible
        {
            get => _isLoadingTextVisible;
            set
            {
                _isLoadingTextVisible = value;
                OnPropertyChanged(nameof(IsLoadingTextVisible));
            }
        }

        public string LoadingTextTitle
        {
            get => _loadingTextTitle;
            set
            {
                _loadingTextTitle = value;
                OnPropertyChanged(nameof(LoadingTextTitle));
            }
        }

        public string LoadingTextContent
        {
            get => _loadingTextContent;
            set
            {
                _loadingTextContent = value;
                OnPropertyChanged(nameof(LoadingTextContent));
            }
        }

        public bool IsStartGameButtonIndeterminate
        {
            get => _isStartGameButtonIndeterminate;
            set
            {
                _isStartGameButtonIndeterminate = value;
                OnPropertyChanged(nameof(IsStartGameButtonIndeterminate));
            }
        }

        public INotificationMessageManager NotificationMessageManager { get; } = new NotificationMessageManager();

        public ICommand GoToHomePageCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("HomePage");
                });
            }
        }

        public ICommand GoToGamesListPageCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ViewModelLocator.GamePageViewModel.GameDownloadGridVisibility = false;
                    ViewModelLocator.GamePageViewModel.GamePathGridVisibility = false;
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("GamePage");
                });
            }
        }

        public ICommand GoToFeedBacksPageCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (!SettingsHelper.Settings.IsConnectedToCMF || !SettingsHelper.Settings.LoggedInToCMFL)
                    {
                        NotifyHelper.GetBasicMessageWithBadge(LanguageHelper.GetField("LoginRequired"),
                            NotifyHelper.MessageType.Warning).Queue();
                        return;
                    }

                    ViewModelLocator.FeedbackPageViewModel.GetFeedBack(null);
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("FeedbackPage");
                });
            }
        }

        public ICommand GoToPlazaPageCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("PlazaPage");
                    ViewModelLocator.PlazaPageViewModel.LoadContent();
                });
            }
        }

        public ICommand GoToSettingsPageCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("SettingPage");
                });
            }
        }

        public ICommand ChangeToolTip
        {
            get { return new DelegateCommand(obj => { AvatarToolTip = EasterEgg.Show(); }); }
        }

        public ICommand StartGameCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (SettingsHelper.Settings.AutoCleanMemory) MemoryHelper.CleanAllMemory();

                    if (SettingsHelper.Settings.FallBackGameSettings.AutoMemorySize)
                        SettingsHelper.Settings.FallBackGameSettings.MaxMemory = int.Parse(GameHelper.AutoMemory());

                    if (_isGameStarting)
                    {
                        NotifyHelper.GetBasicMessageWithBadge("您已经存在一个正在运行的游戏启动进程！", NotifyHelper.MessageType.Warning)
                            .Queue();
                        return;
                    }

                    StartGameInternal();
                });
            }
        }

        public ICommand TryReconnectCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    try
                    {
                        LauncherSocketServerHelper.Connect();
                    }
                    catch (Exception e)
                    {
                        NotifyHelper.GetBasicMessageWithBadge(LanguageHelper.GetField("FailedConnectToServer"),
                            NotifyHelper.MessageType.Error).Queue();
                        LogHelper.WriteError(e);
                    }
                });
            }
        }

        public ICommand ChangeAvatarCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    var failedMessage =
                        LanguageHelper.GetFields("ChangeAvatar|Failed");
                    var successMessage =
                        LanguageHelper.GetFields("ChangeAvatar|Succeeded");

                    try
                    {
                        var avatarSavePath = $"{Environment.CurrentDirectory}\\CMFL\\Avatar\\Upload.jpg";
                        var logoPath = new OpenFileDialog
                        {
                            InitialDirectory = @"C:\",
                            Filter = $"{LanguageHelper.GetField("Avatar")}|*.jpg;*png;*.jpeg;*.bmp",
                            RestoreDirectory = true,
                            FilterIndex = 1
                        };

                        if (logoPath.ShowDialog() != DialogResult.OK) return;

                        if (File.Exists(avatarSavePath))
                            File.Delete(avatarSavePath);

                        var imagePath = logoPath.FileName;
                        var imageBytes = File.ReadAllBytes(imagePath);
                        var image = imageBytes.ToBitmap();
                        var newImage = image.GetThumbnailImage(128, 128, null, new IntPtr());

                        using var mStream = new MemoryStream();
                        newImage.Save(mStream, ImageFormat.Jpeg);
                        newImage.Dispose();

                        var byData = new byte[mStream.Length];
                        mStream.Position = 0;
                        mStream.Read(byData, 0, byData.Length);

                        Task.Run(() =>
                        {
                            try
                            {
                                var result = LauncherSocketServerHelper.Send("AVATAR",
                                    new[] {"SET", Convert.ToBase64String(byData)});

                                if (string.IsNullOrEmpty(result))
                                {
                                    NotifyHelper
                                        .GetBasicMessageWithBadge(failedMessage, NotifyHelper.MessageType.Error)
                                        .Queue();
                                    return;
                                }

                                var resultModel = JsonConvert.DeserializeObject<CommandResultModel>(result);
                                if (resultModel.Result.Equals("SUCCEEDED", StringComparison.Ordinal))
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        NotifyHelper
                                            .GetBasicMessageWithBadge(successMessage, NotifyHelper.MessageType.Success)
                                            .Queue();
                                    });
                                else
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        NotifyHelper
                                            .GetBasicMessageWithBadge(failedMessage, NotifyHelper.MessageType.Error)
                                            .Queue();
                                    });
                            }
                            catch (Exception e)
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    NotifyHelper
                                        .GetBasicMessageWithBadge(failedMessage, NotifyHelper.MessageType.Error)
                                        .Queue();
                                });

                                LogHelper.WriteLogLine(failedMessage, LogHelper.LogLevels.Error);
                                LogHelper.WriteError(e);
                            }
                        });

                        if (!Directory.Exists("Avatar")) Directory.CreateDirectory("Avatar");
                        File.WriteAllBytes(avatarSavePath, byData);
                        AvatarSource = Image.FromFile($"{Environment.CurrentDirectory}\\Avatar\\Upload.jpg")
                            .DrawingImageToImageSource();
                    }
                    catch (ArgumentException ex)
                    {
                        NotifyHelper.ShowNotification(
                            $"{failedMessage}",
                            LanguageHelper.GetField("PleaseCheckLog"), 3000, ToolTipIcon.Error);
                        LogHelper.WriteLogLine($"{failedMessage}", LogHelper.LogLevels.Error);
                        LogHelper.WriteError(ex);
                    }
                });
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (_windowLogin?.IsVisible ?? false)
                        return;
                    _windowLogin = new WindowLogin();
                    _windowLogin.Show();
                });
            }
        }

        public ICommand MinimizeWindowCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                        _ = NativeMethods.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, 5, 5);

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        MainWindow.Instance.WindowState = WindowState.Minimized;
                    });
                });
            }
        }

        public void ChangeLoginPanelState(bool isSucceeded)
        {
            SettingsHelper.Settings.IsConnectedToCMF = isSucceeded;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ViewModelLocator.MainWindowViewModel.LoginPanelVisibility =
                    isSucceeded ? Visibility.Visible : Visibility.Collapsed;
                ViewModelLocator.MainWindowViewModel.ReconnectButtonVisibility =
                    isSucceeded ? Visibility.Collapsed : Visibility.Visible;
            });
        }

        private void LoginToCmfl()
        {
            Task.Run(async () =>
            {
                if (SettingsHelper.Settings.IsRememberMe && SettingsHelper.Settings.IsConnectedToCMF)
                {
                    var result = await LauncherHelper.CheckLauncherAuthInfo(SettingsHelper.Settings.LUsername,
                        StringEncryptHelper.AesDecrypt(SettingsHelper.Settings.LPassword)).ConfigureAwait(true);

                    if (string.IsNullOrEmpty(result))
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            NotifyHelper
                                .GetBasicMessageWithBadge(
                                    $"{LanguageHelper.GetFields("Login|Failed")}，{LanguageHelper.GetField("EmptyServerResponse")}",
                                    NotifyHelper.MessageType.Error)
                                .Queue();
                        });
                        return;
                    }

                    if (result.Equals("5", StringComparison.Ordinal))
                    {
                        Analytics.TrackEvent(AnalyticsEventNames.LoginToLauncherAccount);
                        LogHelper.WriteLogLine(LanguageHelper.GetFields("Login|Succeeded"), LogHelper.LogLevels.Info);
                        SettingsHelper.Settings.LoggedInToCMFL = true;
                    }
                    else
                    {
                        var authResult = Enum.TryParse(result, out AuthResultType auth)
                            ? auth
                            : AuthResultType.UnknownError;

                        var reason = authResult switch
                        {
                            AuthResultType.WrongCredential => LanguageHelper.GetField("WrongCredential"),
                            AuthResultType.UnknownError => LanguageHelper.GetField("UnknownError"),
                            AuthResultType.AlreadyAuth => LanguageHelper.GetField("AlreadyLoggedIn"),
                            _ => LanguageHelper.GetField("UnknownError")
                        };
                        SettingsHelper.Settings.LoggedInToCMFL = false;

                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            NotifyHelper
                                .GetBasicMessageWithBadge(
                                    $"{LanguageHelper.GetFields("Login|Failed")}，{reason}, {LanguageHelper.GetField("PleaseCheckLog")}",
                                    NotifyHelper.MessageType.Error)
                                .Queue();
                        });
                        LogHelper.WriteLogLine($"{LanguageHelper.GetFields("Login|Failed")}，{reason}",
                            LogHelper.LogLevels.Error);
                    }

                    LoginFinished();
                }
            });
        }

        private async Task<Tuple<TaskResultStatus, bool>> LostFilesCheck(VersionInfo version)
        {
            string libUriRoot = $"{SettingsHelper.BmclapiDownloadServer}maven/",
                assetUriRoot = $"{SettingsHelper.BmclapiDownloadServer}objects/",
                assetIndexUriRoot = SettingsHelper.BmclapiDownloadServer;

            if (SettingsHelper.Settings.AutoDetectBestDownloadServer)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    NotifyHelper.GetBasicMessageWithBadge(
                        LanguageHelper.GetField("StartGettingBestServer"),
                        NotifyHelper.MessageType.Info).Queue();
                });
                var libraryResult = await NetworkHelper.GetFastestAddress(new List<string>
                {
                    SettingsHelper.BmclapiDownloadServer,
                    SettingsHelper.MojangDownloadServer
                }).ConfigureAwait(false);

                var assetResult = await NetworkHelper.GetFastestAddress(new List<string>
                {
                    SettingsHelper.BmclapiDownloadServer,
                    SettingsHelper.MojangAssetDownloadServer
                }).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(libraryResult) && !string.IsNullOrEmpty(assetResult))
                {
                    assetIndexUriRoot = assetResult.Equals(SettingsHelper.BmclapiDownloadServer)
                        ? assetResult
                        : "https://launchermeta.mojang.com/";
                    libUriRoot = libraryResult == SettingsHelper.BmclapiDownloadServer
                        ? $"{SettingsHelper.BmclapiDownloadServer}maven/"
                        : libraryResult;
                    assetUriRoot = assetResult == SettingsHelper.BmclapiDownloadServer
                        ? $"{SettingsHelper.BmclapiDownloadServer}objects/"
                        : assetResult;
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        NotifyHelper.GetBasicMessageWithBadge(
                            $"{LanguageHelper.GetField("GetSuccessful")}：{libraryResult}",
                            NotifyHelper.MessageType.Success).Queue();
                    });
                }
                else
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        NotifyHelper.GetBasicMessageWithBadge(
                            LanguageHelper.GetField("GetFailed"),
                            NotifyHelper.MessageType.Warning).Queue();
                    });
                }
            }

            var dRc = new DefaultResourceCompleter
            {
                ResourceInfoResolvers = new List<IResourceInfoResolver>(2)
                {
                    new AssetInfoResolver
                    {
                        AssetIndexUriRoot = assetIndexUriRoot,
                        AssetUriRoot = assetUriRoot,
                        BasePath = GameHelper.Core.RootPath,
                        VersionInfo = version
                    },
                    new LibraryInfoResolver
                    {
                        BasePath = GameHelper.Core.RootPath,
                        LibraryUriRoot = libUriRoot,
                        VersionInfo = version
                    }
                },
                TotalRetry = 3,
                DownloadThread = SettingsHelper.Settings.DownloadThread
            };

            dRc.GameResourceInfoResolveStatus += (sender, args) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    LoadingTextTitle = LanguageHelper.GetField("PleaseWait");
                    LoadingTextContent = args.CurrentProgress;
                });
            };

            dRc.DownloadFileChangedEvent += (sender, args) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    LoadingTextContent =
                        $"{LanguageHelper.GetField("Progress")}：[{args.ProgressPercentage:P}]";
                });
            };

            var total = 0;
            dRc.DownloadFileCompletedEvent += (sender, args) =>
            {
                LoadingTextTitle =
                    $"{LanguageHelper.GetField("Completion")}{(args.File.RetryCount == 0 ? null : "<重试>")} [{args.File.FileName}...]";
            };

            var result = await dRc.CheckAndDownloadTaskAsync().ConfigureAwait(false);
            return new Tuple<TaskResultStatus, bool>(result.TaskStatus, result.Value);
        }

        private void ChangeGameRunningUiStates(bool isRunning)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                StartGameText = LanguageHelper.GetField(isRunning ? "PleaseWait" : "Start");
                IsLoadingTextVisible = isRunning;
                IsStartGameButtonIndeterminate = isRunning;
                if (isRunning)
                {
                    ViewModelLocator.HomePageViewModel.BgmControl.MusicEaseOut();
                    ViewModelLocator.HomePageViewModel.BgmControl.IsMusicMuted = true;
                }
                else
                {
                    ViewModelLocator.HomePageViewModel.BgmControl.IsMusicMuted = false;
                    ViewModelLocator.HomePageViewModel.BgmControl.MusicEaseIn();
                }
            });
        }

        private void StartGameInternal()
        {
            Task.Run(async () =>
            {
                Analytics.TrackEvent(AnalyticsEventNames.StartGame, new Dictionary<string, string>(1)
                {
                    {"Version", SettingsHelper.Settings.ChoseGame}
                });
                _isGameStarting = true;
                _stopwatch.Start();
                var game = GameHelper.Core.VersionLocator.GetGame(SettingsHelper.Settings.ChoseGame);
                if (game == null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        NotifyHelper.GetBasicMessageWithBadge(
                            LanguageHelper.GetField("PleaseSelectGameFirst"),
                            NotifyHelper.MessageType.Warning).Queue();
                    });
                    _isGameStarting = false;
                    return;
                }

                ChangeGameRunningUiStates(true);

                if (!SettingsHelper.Settings.FallBackGameSettings.ForceLaunch)
                {
                    var (taskStatus, isLibraryDownloadFailed) =
                        await LostFilesCheck(game).ConfigureAwait(true);
                    if (taskStatus == TaskResultStatus.PartialSuccess || taskStatus == TaskResultStatus.Error)
                    {
                        if (isLibraryDownloadFailed)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                NotifyHelper.GetBasicMessageWithBadge(
                                    LanguageHelper.GetField("GameResourcesDownloadFailed1"),
                                    NotifyHelper.MessageType.Error).Queue();
                                IsLoadingTextVisible = false;
                            });

                            ChangeGameRunningUiStates(false);
                            _isGameStarting = false;
                            return;
                        }

                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            NotifyHelper.GetBasicMessageWithBadge(
                                LanguageHelper.GetField("GameResourcesDownloadFailed2"),
                                NotifyHelper.MessageType.Warning).Queue();
                            IsLoadingTextVisible = false;
                        });
                    }
                }

                if (SettingsHelper.Settings.IgnoreWarning.Equals(false))
                {
                    var checkResult = await LauncherHelper.CheckEnvironment().ConfigureAwait(true);

                    if (checkResult.Any())
                    {
                        ChangeGameRunningUiStates(false);
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("WarningPage");
                            ViewModelLocator.WarningPageViewModel.Warnings = checkResult;
                        });
                        _isGameStarting = false;

                        return;
                    }
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() => { IsLoadingTextVisible = false; });

                StartGame();
            });
        }

        private void StartGame()
        {
            GameHelper.StartGame(SettingsHelper.Settings.ChoseGame, Dispatcher).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    SettingsHelper.Settings.IgnoreWarning = false;

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        ChangeGameRunningUiStates(false);
                        IsLoadingTextVisible = false;

                        NotifyHelper.GetBasicMessageWithBadge(
                            LanguageHelper.GetFields("LaunchGame|Failed"),
                            NotifyHelper.MessageType.Error).Queue();
                    });

                    NotifyHelper.ShowNotification(
                        LanguageHelper.GetFields("LaunchGame|Failed"),
                        LanguageHelper.GetField("PleaseCheckLog"), 3000, ToolTipIcon.Error);

                    LogHelper.WriteLogLine(
                        LanguageHelper.GetFields("LaunchGame|Failed"),
                        LogHelper.LogLevels.Error);
                    LogHelper.WriteError(task.Exception);
                }

                if (task.Result.Value)
                {
                    _stopwatch.Stop();
                    SettingsHelper.Settings.IgnoreWarning = false;
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        WindowState = WindowState.Minimized;
                        IsLoadingTextVisible = false;
                    });
                    LogHelper.WriteLogLine(
                        LanguageHelper.GetFields("LaunchGame|Succeeded"),
                        LogHelper.LogLevels.Info);
                    NotifyHelper.ShowNotification(LanguageHelper.GetField("GameIsRunning"),
                        $"{LanguageHelper.GetFields("LaunchGame|Succeeded")}, {_stopwatch.Elapsed.TotalMilliseconds}ms",
                        3000, ToolTipIcon.Info);
                    Analytics.TrackEvent(AnalyticsEventNames.StartGameFinished, new Dictionary<string, string>(1)
                    {
                        {"Time (s)", _stopwatch.Elapsed.TotalSeconds.ToString("F1")}
                    });
                    _stopwatch.Reset();
                }
                else
                {
                    SettingsHelper.Settings.IgnoreWarning = false;

                    ChangeGameRunningUiStates(false);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        NotifyHelper.GetBasicMessageWithBadge(
                            LanguageHelper.GetFields("LaunchGame|Succeeded"),
                            NotifyHelper.MessageType.Error).Queue();
                    });

                    NotifyHelper.ShowNotification(
                        LanguageHelper.GetFields("LaunchGame|Failed"),
                        LanguageHelper.GetFields("LaunchGame|Failed|PleaseCheckLog", ", "),
                        3000, ToolTipIcon.Error);
                    LogHelper.WriteLogLine(
                        LanguageHelper.GetFields("LaunchGame|Failed"),
                        LogHelper.LogLevels.Error);
                }

                _isGameStarting = false;
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        /// <summary>
        ///     初始化界面
        /// </summary>
        public void Initialize()
        {
            Task.Run(async () =>
            {
                await Task.Delay(2500).ConfigureAwait(true);

                if (SettingsHelper.Settings.FirstTime)
                {
                    ServiceLocator.Current.GetInstance<INavigationService>()
                        .NavigateTo("IntroPage1");
                }
                else
                {
                    if (!SettingsHelper.Settings.FirstTime)
                        ServiceLocator.Current.GetInstance<INavigationService>()
                            .NavigateTo("HomePage");
                }

#if DEBUG
                return;
#endif

                await LauncherHelper.CheckLauncherUpgrade().ContinueWith(task =>
                    {
                        if (task.Exception != null || task.Result.TaskStatus != TaskResultStatus.Success)
                        {
                            NotifyHelper.ShowNotification(
                                LanguageHelper.GetFields("UpdateCheck|Failed"),
                                LanguageHelper.GetField("PleaseCheckLog"), 3000,
                                ToolTipIcon.Error);
                            LogHelper.WriteLogLine(
                                LanguageHelper.GetFields("UpdateCheck|Failed"),
                                LogHelper.LogLevels.Error);
                            LogHelper.WriteError(task.Exception);
                            return;
                        }

                        var path = Process.GetCurrentProcess().MainModule?.FileName;

                        if (string.IsNullOrEmpty(path))
                            return;

                        var bytes = File.ReadAllBytes(path);
                        var sha256Bytes = Cryptosystem.Sha256(bytes);
                        var sha256 = BitConverter.ToString(sha256Bytes).Replace("-", string.Empty);
                        if (task.Result.Value.Sha.Equals(sha256, StringComparison.OrdinalIgnoreCase)) return;
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            LeftMessageBorderVisibility = true;
                            LeftBorderIconColor = "#00b0ff";
                        });

                        ViewModelLocator.UpgradePageViewModel.UpdateInfoModel = task.Result.Value;
                        ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("UpgradePage");
                        ViewModelLocator.UpgradePageViewModel.GetUpdateInfo();
                    }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default)
                    .ConfigureAwait(false);
            });
        }

        #region 获取BGM

        public void GetMusic()
        {
            LauncherHelper.GetMusicString().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    LogHelper.WriteLogLine($"BGM {LanguageHelper.GetField("GetFailed")}", LogHelper.LogLevels.Error);
                    LogHelper.WriteError(task.Exception);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        ViewModelLocator.HomePageViewModel.BgmControl.BgmName = LanguageHelper.GetField("No");
                    });
                    return;
                }

                if (task.Result.Value.UseLocalMusic)
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        ViewModelLocator.HomePageViewModel.BgmControl.SetMusicUri(task.Result.Value.LocalMusicPath);
                        ViewModelLocator.HomePageViewModel.BgmControl.BgmName = task.Result.Value.Name;
                        ViewModelLocator.HomePageViewModel.BgmControl.Play();
                    });
                else
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        ViewModelLocator.HomePageViewModel.BgmControl.SetMusicUri(
                            $"{SettingsHelper.Settings.LauncherWebServer}/Asset/Music/{task.Result.Value.FileName}");
                        ViewModelLocator.HomePageViewModel.BgmControl.BgmName = task.Result.Value.Name;
                        ViewModelLocator.HomePageViewModel.BgmControl.Play();
                    });
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        #endregion

        #region 登陆完成的操作

        /// <summary>
        ///     登陆完成后执行的操作
        /// </summary>
        public void LoginFinished()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                LoginInfoVisibility = false;
                LoginButtonContent = SettingsHelper.Settings.LUsername;
                LoginButtonToolTip = LanguageHelper.GetField("ClickMeToSwitchAccount");
                LoginButtonBackground = new SolidColorBrush(Color.FromRgb(0, 230, 118));
                //ChangeAvatarButton.IsEnabled = true;
                //App.SettingsPage.GameNameBox.IsEnabled = false;
                GetUserInfo();
                //App.HomePage.ServerConnectionTest();
            });
        }

        #endregion

        #region 头像获取

        /// <summary>
        ///     头像获取
        /// </summary>
        private void GetAvatar()
        {
            Task.Run(() =>
            {
                try
                {
                    if (!Directory.Exists($"{Environment.CurrentDirectory}\\CMFL\\Avatar\\"))
                        Directory.CreateDirectory($"{Environment.CurrentDirectory}\\CMFL\\Avatar\\");

                    var base64Avatar = LauncherSocketServerHelper.Send("AVATAR", new[] {"GET", "xxx"});
                    if (string.IsNullOrEmpty(base64Avatar))
                    {
                        UseAvatarFromCache();
                        LogHelper.WriteLogLine("向服务器请求用户头像时出现错误，现使用缓存", LogHelper.LogLevels.Warning);
                        return;
                    }

                    var imageBytes = Convert.FromBase64String(base64Avatar);

                    var avatarPath = $"{Environment.CurrentDirectory}\\CMFL\\Avatar\\Cache.jpg";
                    if (File.Exists(avatarPath))
                        File.Delete(avatarPath);
                    File.WriteAllBytes(avatarPath, imageBytes);

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Image.FromFile(avatarPath).DrawingImageToImageSource();
                    });
                }
                catch (Exception e)
                {
                    LogHelper.WriteLogLine("尝试删除老头像时发生错误", LogHelper.LogLevels.Error);
                    LogHelper.WriteError(e);
                    UseAvatarFromCache();
                }
            });
        }

        #endregion

        #region 更新头像缓存

        /// <summary>
        ///     更新头像缓存
        /// </summary>
        private void UseAvatarFromCache()
        {
            var avatarPath = $"{Environment.CurrentDirectory}\\CMFL\\Avatar\\Cache.jpg";
            if (!File.Exists(avatarPath))
                return;

            AvatarSource = Image.FromFile(avatarPath).DrawingImageToImageSource();
        }

        #endregion

        #region 获取用户信息

        /// <summary>
        ///     获取用户信息
        /// </summary>
        private void GetUserInfo()
        {
            Task.Run(async () =>
            {
                LogHelper.WriteLogLine("开始获取用户信息", LogHelper.LogLevels.Info);
                ServicePointManager.ServerCertificateValidationCallback = App.CheckValidationResult;
                try
                {
                    // var res = await LauncherSocketServerHelper.Send("GETINFO").ConfigureAwait(true);
                    // var dbUserData = JsonConvert.DeserializeObject<UserData.DBUserData>(res);

                    // GetAvatar();
                    LogHelper.WriteLogLine("用户信息获取完成", LogHelper.LogLevels.Info);
                    /*
                    LogHelper.WriteLogLine("开始获取用户信息", LogHelper.LogLevels.Info);

                    WebClient webClient = new WebClient
                    {
                        Encoding = Encoding.UTF8
                    };

                    string serverReply = await webClient.DownloadStringTaskAsync(new Uri(
                        "https://background.craftminefun.com:7707/User/getpinfo.php?" +
                        HttpUtility.UrlEncode("username", encodingFormat) + "=" +
                        HttpUtility.UrlEncode(SettingProxy.LUsername, encodingFormat) + "&&" +
                        HttpUtility.UrlEncode("sid", encodingFormat) + "=" +
                        HttpUtility.UrlEncode(SettingProxy.LAccessToken, encodingFormat)));

                    string[] userInfoData = serverReply.Split(',');
                    BanInfo = userInfoData[3].Split('|');

                    if (userInfoData[1] == "0")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            App.HomePage.LabelUserCategory.Content = "用户";
                            App.HomePage.LabelUserCategory.ToolTip = "然而这里并没有属于普通用户的彩蛋，别看了（"; //好像普通用户就知道有管理员彩蛋似的
                            App.HomePage.BorderUserCategory.Background =
                                new SolidColorBrush(Color.FromRgb(234, 169, 30));
                        });
                    }
                    else
                    {
                        if (userInfoData[1] == "1")
                        {
                            Dispatcher.Invoke(() =>
                            {
                                App.HomePage.LabelUserCategory.Content = "管理员";
                                App.HomePage.LabelUserCategory.ToolTip = "绿帽管理员（跑";
                                App.HomePage.BorderUserCategory.Background =
                                    new SolidColorBrush(Color.FromRgb(98, 197, 30));
                            });
                        }
                    }

                    GetAvatar();

                    LogHelper.WriteLogLine("用户信息获取完成", LogHelper.LogLevels.Info);
                    */
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLogLine("无法从服务器获取用户信息！", LogHelper.LogLevels.Error);
                    LogHelper.WriteError(ex);
                    /*
                    Stream getInfoResponseStream = ex.Response.GetResponseStream();
                    StreamReader getInfoReader = new StreamReader(getInfoResponseStream, encodingFormat);
                    String getInfoWrongMessage = getInfoReader.ReadToEnd();
                    LogHelper.WriteLogLine("无法从服务器获取用户信息！", LogHelper.LogLevels.Error);
                    LogHelper.WriteError(ex);
                    switch (getInfoWrongMessage)
                    {
                        case "ERROR0x0":
                            Dispatcher.Invoke(() =>
                            {
                                App.HomePage.GridProFileException.Visibility = Visibility.Visible;
                                App.HomePage.LabelProFileErrorInfo.Content = "原因：数据库错误";
                                App.HomePage.LabelUserCategory.Content = "错误";
                            });

                            break;
                        case "ERROR0x1":
                            Dispatcher.Invoke(() =>
                            {
                                App.HomePage.GridProFileException.Visibility = Visibility.Visible;
                                App.HomePage.LabelProFileErrorInfo.Content = "原因：数据有空格";
                                App.HomePage.LabelUserCategory.Content = "错误";
                            });

                            break;
                        case "ERROR0x2":
                            Dispatcher.Invoke(() =>
                            {
                                App.HomePage.GridProFileException.Visibility = Visibility.Visible;
                                App.HomePage.LabelProFileErrorInfo.Content = "原因：非法字符";
                                App.HomePage.LabelUserCategory.Content = "错误";
                            });

                            break;
                        case "ERROR0x3":
                            Dispatcher.Invoke(() =>
                            {
                                App.HomePage.GridProFileException.Visibility = Visibility.Visible;
                                App.HomePage.LabelProFileErrorInfo.Content = "原因：未登录或登录数据损坏";
                                App.HomePage.LabelUserCategory.Content = "错误";
                            });

                            break;
                        case "ERROR0xF":
                            Dispatcher.Invoke(() =>
                            {
                                App.HomePage.GridProFileException.Visibility = Visibility.Visible;
                                App.HomePage.LabelProFileErrorInfo.Content = "原因：参数不正确";
                                App.HomePage.LabelUserCategory.Content = "错误";
                            });

                            break;
                    }

                    getInfoReader.Close();
                    getInfoResponseStream.Close();
                    */
                }
            });
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
        ~MainWindowViewModel()
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

        #endregion
    }
}