using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using CMFL.MVVM.Class.Exceptions;
using CMFL.MVVM.Class.Helper.Graphic;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.Other;
using CMFL.MVVM.Models.DataModel.AuthLib;
using CMFL.MVVM.Models.DataModel.Game;
using CMFL.MVVM.Models.DataModel.Game.Account;
using CMFL.MVVM.Models.DataModel.GameData;
using CMFL.MVVM.ViewModels;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Heyo.Class.Helper;
using Newtonsoft.Json;
using ProjBobcat.Authenticator;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;
using ProjBobcat.Class.Model.LauncherProfile;
using ProjBobcat.DefaultComponent.Launch;
using ProjBobcat.Event;
using ProjBobcat.Interface;
using ProjCrypto.Class.Helper;
using SharpCompress.Archives;
using SharpCompress.Common;
using SystemInfoHelper = CMFL.MVVM.Class.Helper.Kernel.SystemInfoHelper;

namespace CMFL.MVVM.Class.Helper.Game
{
    /// <inheritdoc />
    /// <summary>
    ///     游戏启动、扫描、修复
    /// </summary>
    public class GameHelper : FrameworkElement
    {
        private static readonly Dictionary<string, VersionInfo> FoundVersionInfos =
            new Dictionary<string, VersionInfo>();

        private static readonly Stopwatch GameTimeStopWatch = new Stopwatch();
        private new static Dispatcher Dispatcher { get; set; }
        private static string RunningGame { get; set; }

        public static IGameCore Core { get; private set; }

        public static void InitGameCore()
        {
            var rootPath = SettingsHelper.Settings.ChoseGamePath.TrimEnd('\\');
            var clientToken = new Guid(SettingsHelper.Settings.ClientToken);

            Core = new DefaultGameCore
            {
                ClientToken = clientToken,
                RootPath = rootPath,
                VersionLocator = new DefaultVersionLocator(rootPath, clientToken)
                {
                    LauncherProfileParser = new DefaultLauncherProfileParser(rootPath, clientToken)
                }
            };

            LogHelper.WriteLogLine(LanguageHelper.GetField("RegisterGameExitEvent"), LogHelper.LogLevels.Info);
            Core.GameExitEventDelegate += GameExit;
            LogHelper.WriteLogLine(LanguageHelper.GetField("RegisterGameExitEvent"), LogHelper.LogLevels.Info);
            Core.GameLogEventDelegate += GameLog;
            Core.LaunchLogEventDelegate += CoreOnLaunchLogEventDelegate;
        }

        private static void CoreOnLaunchLogEventDelegate(object sender, LaunchLogEventArgs e)
        {
            LogHelper.WriteLogLine($"[ProjBobcat Launch Engine]{e.Item}, {e.ItemRunTime.Milliseconds} ms.",
                LogHelper.LogLevels.Info);
        }

        #region 游戏退出事件

        /// <summary>
        ///     游戏退出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">退出代码，0为正常</param>
        private static void GameExit(object sender, GameExitEventArgs e)
        {
            GameTimeStopWatch.Stop();
            if (SettingsHelper.Settings.GameTimes.ContainsKey(RunningGame))
                SettingsHelper.Settings.GameTimes[RunningGame] += GameTimeStopWatch.ElapsedMilliseconds;
            else
                SettingsHelper.Settings.GameTimes.Add(RunningGame, GameTimeStopWatch.ElapsedMilliseconds);
            SettingsHelper.Save();

            if (SettingsHelper.Settings.ResumeMusic)
            {
                ViewModelLocator.HomePageViewModel.BgmControl.IsMusicMuted = false;
                ViewModelLocator.HomePageViewModel.BgmControl.MusicEaseIn();
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ViewModelLocator.MainWindowViewModel.IsStartGameButtonIndeterminate = false;
                ViewModelLocator.MainWindowViewModel.StartGameText = LanguageHelper.GetField("Start");
            });

            ResumeWindow();

            if (e.ExitCode != 0 || e.Exception != null)
            {
                LogHelper.WriteLogLine(LanguageHelper.GetField("McHasCrashedSeeLogs"), LogHelper.LogLevels.Error);
                LogHelper.WriteError(e.Exception);
                NotifyHelper.ShowNotification(LanguageHelper.GetField("McCrashed"),
                    LanguageHelper.GetField("McCrashed2"), 3000, ToolTipIcon.Warning);
                return;
            }

            NotifyHelper.ShowNotification(LanguageHelper.GetField("McExited"),
                LanguageHelper.GetField("McExited"), 3000, ToolTipIcon.Info);
            LogHelper.WriteLogLine(LanguageHelper.GetField("McExited"), LogHelper.LogLevels.Info);
            NotifyHelper.ShowNotification(LanguageHelper.GetField("McExited"), LanguageHelper.GetField("McExited2"),
                3000, ToolTipIcon.Info);
        }

        #endregion

        #region 恢复窗口方法

        private static void ResumeWindow()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ViewModelLocator.MainWindowViewModel.WindowState = WindowState.Normal;
                ViewModelLocator.MainWindowViewModel.WindowTopMost = true;
            });
        }

        #endregion

        #region 游戏日志记录事件

        /// <summary>
        ///     游戏日志记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">日志</param>
        private static void GameLog(object sender, GameLogEventArgs e)
        {
            LogHelper.WriteGameLog($"[{e.LogType}]{e.Content}");
        }

        #endregion

        #region 从配置文件启动游戏

        private static async Task<bool> StartGameFromConfig(LaunchSettings launchSettings)
        {
            return await Task.Run(async () =>
            {
                LogHelper.WriteLogLine(
                    string.Format("{0}{2}{0} 丨{1}            丨{0}{2}", Environment.NewLine,
                        LanguageHelper.GetField("StartingGameUseList"), "+----------------------------------+"),
                    LogHelper.LogLevels.Info);

                GameTimeStopWatch.Start();
                var result = await Core.LaunchTaskAsync(launchSettings).ConfigureAwait(true);

                if (result.Error == null && result.ErrorType == LaunchErrorType.None) return true;

                var solution = string.Empty;
                if (result.Error?.Exception != null) LogHelper.WriteError(result.Error.Exception);
                LogHelper.WriteLogLine(
                    string.Format("{0}：{1}{0}{1}：{2}{0}{1}：{3}", Environment.NewLine, result.Error?.Error,
                        result.ErrorType, result.Error?.ErrorMessage), LogHelper.LogLevels.Info);

                solution = result.ErrorType switch
                {
                    LaunchErrorType.AuthFailed => LanguageHelper.GetField("CrashSolution1"),
                    LaunchErrorType.NoJava => LanguageHelper.GetField("CrashSolution2"),
                    LaunchErrorType.DecompressFailed => LanguageHelper.GetField("CrashSolution3"),
                    // LaunchErrorType.OperatorException => LanguageHelper.GetField("CrashSolution4"),
                    LaunchErrorType.Unknown => LanguageHelper.GetField("UnknownError"),
                    _ => solution
                };

                LogHelper.WriteLogLine(solution, LogHelper.LogLevels.Info);

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("GameErrorPage");
                    if (result.Error.Exception == null)
                    {
                        ViewModelLocator.GameErrorPageViewModel.ErrorText = string.Format("{1}{0}{2}{0}{3}{0}{4}",
                            Environment.NewLine, result.ErrorType, result.Error.ErrorMessage, solution,
                            result.Error.Cause);
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        result.Error.Exception.GetAllExceptionString(ref sb);
                        ViewModelLocator.GameErrorPageViewModel.ErrorText = sb.ToString();
                    }
                });

                return false;
            }).ConfigureAwait(false);
        }

        #endregion

        #region 本地游戏列表获取

        /// <summary>
        ///     获取本地游戏列表
        /// </summary>
        /// <returns>游戏列表</returns>
        public static List<VersionInfo> GetListGames()
        {
            var games = Core.VersionLocator.GetAllGames();
            var versions = games.ToList();

            FoundVersionInfos.Clear();
            foreach (var version in versions) FoundVersionInfos.Add(version.Id, version);

            return versions;
        }

        #endregion

        #region 自动设置内存

        /// <summary>
        ///     自动设置内存
        /// </summary>
        public static string AutoMemory()
        {
            var available = (int) MemoryHelper.GetAvailablePhysicalMemory();
            var memory =
                SystemInfoHelper.SysInfo["OperatingSystemInfo"].Equals("64Bits", StringComparison.Ordinal) &&
                available * 0.8 >= 1024
                    ? (int) (available * 0.9)
                    : 1024;
            if (memory >= 10240) memory = 9216;
            SettingsHelper.Settings.FallBackGameSettings.MaxMemory = memory;
            return memory.ToString();
        }

        #endregion

        #region 获取本地游戏文件信息

        /// <summary>
        ///     获取游戏
        /// </summary>
        public static Task<TaskResult<List<GameInfo>>> SearchGames(bool isLiteMode)
        {
            return Task.Run(async () =>
            {
                var gameInfos = new List<GameInfo>();

                if (Directory.Exists($@"{Environment.CurrentDirectory}\CMFL\Temp\"))
                {
                    DirectoryHelper.CleanDirectory($@"{Environment.CurrentDirectory}\CMFL\Temp\", false);
                }
                else
                {
                    if (!Directory.Exists($@"{Environment.CurrentDirectory}\CMFL\Temp\"))
                        Directory.CreateDirectory($@"{Environment.CurrentDirectory}\CMFL\Temp\");
                }

                var games = GetListGames();
                if (games == null) throw new GameNotFoundException();

                await SettingsHelper.GetGamesInfo().ConfigureAwait(true);

                #region 获取游戏文件（光影、MOD、截图、材质包、地图）

                foreach (var game in games)
                {
                    var resPacks = new List<ResPack>();
                    var savesList = new List<string>();
                    var modList = new List<string>();
                    var shaderPackList = new List<string>();
                    var screenShotList = new List<ScreenShot>();
                    var isChecked = game.Id.Equals(SettingsHelper.Settings.ChoseGame, StringComparison.Ordinal);

                    if (!isLiteMode)
                    {
                        #region 光影包列表获取

                        if (SettingsHelper.ShaderPack[game.Id].Any())
                            shaderPackList.AddRange(SettingsHelper.ShaderPack[game.Id].Select(item => item.Name));

                        #endregion

                        #region 截图列表获取

                        if (SettingsHelper.ScreenShot[game.Id].Any())
                            screenShotList.AddRange(SettingsHelper.ScreenShot[game.Id].Select(item => new ScreenShot
                            {
                                ScreenShotSource = BitmapHelper.GetImage(item.AbsolutelyFilePath, 80, 120),
                                ScreenShotPath = item.AbsolutelyFilePath
                            }));

                        #endregion

                        #region 材质包列表获取

                        if (SettingsHelper.ResPack[game.Id].Any())
                            foreach (var item in SettingsHelper.ResPack[game.Id])
                            {
                                string resPackImageUrl;
                                try
                                {
                                    using (var archive = ArchiveFactory.Open(item.AbsolutelyFilePath))
                                    {
                                        foreach (var file in archive.Entries)
                                            if (!file.IsDirectory && file.Key.Equals("pack.png",
                                                StringComparison.Ordinal))
                                                file.WriteToDirectory(
                                                    $@"{Environment.CurrentDirectory}\CMFL\Temp\{game.Id}\{item.Name}\",
                                                    new ExtractionOptions
                                                    {
                                                        ExtractFullPath = true,
                                                        Overwrite = true
                                                    });
                                    }

                                    resPackImageUrl =
                                        File.Exists(
                                            $@"{Environment.CurrentDirectory}\CMFL\Temp\{game.Id}\{item.Name}\pack.png")
                                            ? $@"{Environment.CurrentDirectory}\CMFL\Temp\{game.Id}\{item.Name}\pack.png"
                                            : "/CMFL.MVVM;component/Assets/Images/ResourcePack.jpg";
                                }
                                catch (ArgumentException ex)
                                {
                                    resPackImageUrl = "/CMFL.MVVM;component/Assets/Images/ResourcePack.jpg";
                                    LogHelper.WriteLogLine(
                                        $"{LanguageHelper.GetField("ResourcesPackLoadFailed")}：{item.Name}，{LanguageHelper.GetField("ResourcesPackLoadFailedReason1")}",
                                        LogHelper.LogLevels.Error);
                                    LogHelper.WriteError(ex);
                                }
                                catch (InvalidOperationException ex)
                                {
                                    resPackImageUrl = "/CMFL.MVVM;component/Assets/Images/ResourcePack.jpg";
                                    LogHelper.WriteLogLine(
                                        $"{LanguageHelper.GetField("ResourcesPackLoadFailed")}：{item.Name}，{LanguageHelper.GetField("ResourcesPackLoadFailedReason2")}",
                                        LogHelper.LogLevels.Error);
                                    LogHelper.WriteError(ex);
                                }

                                resPacks.Add(new ResPack
                                {
                                    ResPackName = item.Name,
                                    ResPackImageUrl = resPackImageUrl
                                });
                            }

                        #endregion

                        #region Mod列表获取

                        if (SettingsHelper.Mods[game.Id].Any())
                            modList.AddRange(SettingsHelper.Mods[game.Id].Select(item => item.Name));

                        #endregion

                        #region 存档列表获取

                        if (SettingsHelper.Saves[game.Id].Any())
                            savesList.AddRange(SettingsHelper.Saves[game.Id]
                                .Select(item => item.AbsolutelyDirectory.Split('\\').Last()));

                        #endregion
                    }

                    var gameInfo = new GameInfo
                    {
                        Name = game.Name,
                        Version = game.Id,
                        IsChecked = isChecked,
                        ShaderPackUrl = shaderPackList,
                        SavesUrl = savesList,
                        ModUrl = modList,
                        ScreenShotUrl = screenShotList,
                        ResPack = resPacks
                    };
                    var gameSettings = SettingsHelper.Settings.GameSettings.FirstOrDefault(g =>
                        g.Key.Equals(game.Name, StringComparison.Ordinal));

                    if (gameSettings.Equals(default(KeyValuePair<string, GameSettings>)))
                    {
                        var defaultGameSettings = GameSettings.GetDefaultGameSettings();
                        defaultGameSettings.VersionId = game.Id;

                        gameInfo.Icon = GameIconHelper.GetIcon(defaultGameSettings.Icon);
                        gameInfo.SelectedIconIndex = GameIconHelper.GetIconIndex(defaultGameSettings.Icon);
                        gameInfo.GameSettings = defaultGameSettings;

                        SettingsHelper.Settings.GameSettings.Add(game.Name, defaultGameSettings);
                    }
                    else
                    {
                        gameInfo.Name = gameSettings.Key;
                        gameInfo.GameSettings = gameSettings.Value;
                        gameInfo.Icon = GameIconHelper.GetIcon(gameSettings.Value.Icon);
                        gameInfo.SelectedIconIndex = GameIconHelper.GetIconIndex(gameSettings.Value.Icon);
                    }

                    gameInfos.Add(gameInfo);
                }

                SettingsHelper.Save();

                #endregion

                if (!SettingsHelper.Settings.FirstTime)
                    return new TaskResult<List<GameInfo>>(TaskResultStatus.Success, value: gameInfos);

                gameInfos.First().IsChecked = true;
                SettingsHelper.Settings.ChoseGame = gameInfos.First().Name;

                return new TaskResult<List<GameInfo>>(TaskResultStatus.Success, value: gameInfos);
            });
        }

        #endregion

        #region 启动游戏

        /// <summary>
        ///     启动游戏
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <param name="dispatcher">Dispatcher</param>
        public static Task<TaskResult<bool>> StartGame(string versionId, Dispatcher dispatcher)
        {
            RunningGame = versionId;
            GameTimeStopWatch.Reset();
            return Task.Run(async () =>
            {
                Dispatcher = dispatcher;

                #region 启动配置

                LogHelper.WriteLogLine(
                    string.Format("{1}{0}丨 {2}             丨{0}{1}", Environment.NewLine,
                        " +----------------------------------+", LanguageHelper.GetField("StartingGenerateLaunchList")),
                    LogHelper.LogLevels.Info, false);

                // var gameVersion = Core.VersionLocator.GetGame(versionId);

                LogHelper.WriteLogLine(
                    string.Format("{1}{0}   {2}{0} 1.{3}：{4}", Environment.NewLine,
                        " +----------------------------------+ ", LanguageHelper.GetField("ConfigureList"),
                        LanguageHelper.GetField("GameVersion"), versionId),
                    LogHelper.LogLevels.Info, false);

                var preGameSettings = SettingsHelper.Settings.GameSettings.FirstOrDefault(s =>
                    s.Value.VersionId.Equals(versionId, StringComparison.Ordinal));
                var flag = false;

                var path = SettingsHelper.Settings.VersionInsulation
                    ? GamePathHelper.GetGamePath(Core.RootPath, versionId)
                    : Core.RootPath;
                var launchSettings = new LaunchSettings
                {
                    FallBackGameArguments = new GameArguments
                    {
                        GcType = Enum.TryParse(SettingsHelper.Settings.FallBackGameSettings.GcType.ToString(),
                            out GcType type)
                            ? type
                            : GcType.G1Gc,
                        JavaExecutable = SettingsHelper.Settings.FallBackGameSettings.JavaPath,
                        Resolution = new ResolutionModel
                        {
                            Height = SettingsHelper.Settings.FallBackGameSettings.ScreenSize.Height,
                            Width = SettingsHelper.Settings.FallBackGameSettings.ScreenSize.Width
                        },
                        MinMemory = SettingsHelper.Settings.FallBackGameSettings.MinMemory,
                        MaxMemory = SettingsHelper.Settings.FallBackGameSettings.MaxMemory
                    },
                    Version = versionId,
                    VersionInsulation = SettingsHelper.Settings.VersionInsulation,
                    GameResourcePath = Core.RootPath,
                    GamePath = path,
                    VersionLocator = Core.VersionLocator
                };

                if (!preGameSettings.Equals(default(KeyValuePair<string, GameSettings>)))
                {
                    launchSettings.GameArguments = new GameArguments
                    {
                        AdvanceArguments = preGameSettings.Value.AdvancedArguments,
                        GcType = (GcType) preGameSettings.Value.GcType,
                        JavaExecutable = preGameSettings.Value.JavaPath,
                        Resolution = preGameSettings.Value.ScreenSize,
                        MinMemory = preGameSettings.Value.MinMemory,
                        MaxMemory = preGameSettings.Value.MaxMemory
                    };
                    flag = true;
                    launchSettings.GameName = preGameSettings.Key;
                }

                var accountInfo = AccountInfo.GetSelectedAccount();
                launchSettings.Authenticator = accountInfo switch
                {
                    OnlineAccount online => new YggdrasilAuthenticator
                    {
                        AuthServer = online.AuthServer?.TrimEnd('/'),
                        Email = StringEncryptHelper.StringDecrypt(online.Email),
                        Password = StringEncryptHelper.StringDecrypt(online.Password),
                        LauncherProfileParser = Core.VersionLocator.LauncherProfileParser
                    },
                    OfflineAccount offline => new OfflineAuthenticator
                    {
                        Username = SettingsHelper.Settings.LoggedInToCMFL &&
                                   !string.IsNullOrEmpty(SettingsHelper.Settings.LUsername)
                            ? SettingsHelper.Settings.LUsername
                            : offline.DisplayName,
                        LauncherProfileParser = Core.VersionLocator.LauncherProfileParser
                    },
                    _ => launchSettings.Authenticator
                };

                LogHelper.WriteLogLine(
                    $"2.{LanguageHelper.GetField("OnlineMode")}：{(AccountInfo.GetSelectedAccount().IsOnlineAccount() ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                    LogHelper.LogLevels.Info, false);

                LogHelper.WriteLogLine(
                    $" 3.{LanguageHelper.GetField("VersionInsulationSetting")}：{(launchSettings.VersionInsulation ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                    LogHelper.LogLevels.Info, false);

                launchSettings.LauncherName = "CraftMineFun-ProjBobcat";

                if (flag)
                {
                    if (preGameSettings.Value.MaxMemory > 0)
                        launchSettings.GameArguments.MaxMemory = preGameSettings.Value.MaxMemory;

                    if (preGameSettings.Value.MinMemory > 0)
                        launchSettings.GameArguments.MinMemory = preGameSettings.Value.MinMemory;

                    LogHelper.WriteLogLine(
                        $" 4.{LanguageHelper.GetField("MaxMem")}：{SettingsHelper.Settings.FallBackGameSettings.MaxMemory} | {LanguageHelper.GetField("MinMem")}：{SettingsHelper.Settings.FallBackGameSettings.MinMemory}",
                        LogHelper.LogLevels.Info, false);

                    if (!string.IsNullOrEmpty(preGameSettings.Value.ServerIp))
                    {
                        LogHelper.WriteLogLine(
                            $" 5.{LanguageHelper.GetField("JoinServerAfterLaunch")}：{preGameSettings.Value.ServerIp}",
                            LogHelper.LogLevels.Info, false);
                        var server = preGameSettings.Value.ServerIp.Split(':');
                        var serverPort = server.Length > 1
                            ? int.TryParse(server[1], out var port) ? port : 25565
                            : 25565;
                        launchSettings.GameArguments.ServerSettings = new ServerSettings
                        {
                            Address = server[0],
                            Port = serverPort
                        };
                    }

                    if (preGameSettings.Value.ScreenSize != null)
                    {
                        launchSettings.GameArguments.Resolution = preGameSettings.Value.ScreenSize;
                        LogHelper.WriteLogLine(
                            $" 6.{LanguageHelper.GetField("ScreenResolution")}：{preGameSettings.Value.ScreenSize.Width}*{preGameSettings.Value.ScreenSize.Height}",
                            LogHelper.LogLevels.Info, false);
                        LogHelper.WriteLogLine(
                            $" 7.{LanguageHelper.GetField("FullScreen")}：{(SettingsHelper.Settings.FallBackGameSettings.ScreenSize.FullScreen ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                            LogHelper.LogLevels.Info, false);
                    }
                }

                LogHelper.WriteLogLine(
                    $" 8.{LanguageHelper.GetField("EnableGc")}：{(SettingsHelper.Settings.FallBackGameSettings.EnableGc ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                    LogHelper.LogLevels.Info, false);

                LogHelper.WriteLogLine(
                    $" 9.{LanguageHelper.GetField("AuthLib")}：{(SettingsHelper.Settings.FallBackGameSettings.UseAuthLib ? LanguageHelper.GetField("True") : LanguageHelper.GetField("False"))}",
                    LogHelper.LogLevels.Info, false);

                LogHelper.WriteLogLine(" +----------------------------------+ ",
                    LogHelper.LogLevels.Info, false);

                #endregion

                #region 启动游戏

                if (!flag)
                    return new TaskResult<bool>(TaskResultStatus.Success,
                        value: await StartGameFromConfig(launchSettings).ConfigureAwait(true));
                if (!preGameSettings.Value.UseAuthLib)
                    return new TaskResult<bool>(TaskResultStatus.Success,
                        value: await StartGameFromConfig(launchSettings).ConfigureAwait(true));

                LogHelper.WriteLogLine(LanguageHelper.GetField("UsingAuthlibWarning"),
                    LogHelper.LogLevels.Info, false);

                LogHelper.WriteLogLine(
                    $" +----------------------------------+{Environment.NewLine} 丨 {LanguageHelper.GetField("StartDownloadAuthlibBuild")}               丨{Environment.NewLine} +----------------------------------+",
                    LogHelper.LogLevels.Info, false);

                try
                {
                    if (!Directory.Exists(Environment.CurrentDirectory + @"\CMFL\Temp\AuthLib-Injector\"))
                        Directory.CreateDirectory(Environment.CurrentDirectory +
                                                  @"\CMFL\Temp\AuthLib-Injector\");
                    else
                        DirectoryHelper.CleanDirectory(
                            Environment.CurrentDirectory + @"\CMFL\Temp\AuthLib-Injector\",
                            false);

                    await DownloadHelper.DownloadSingleFileAsyncWithEvent(SettingsHelper.Settings.EnableBMCLAPI
                                ? new Uri(
                                    $"{SettingsHelper.BmclapiDownloadServer}mirrors/authlib-injector/artifact/latest.json")
                                : new Uri($"{SettingsHelper.AuthLibInjectorDownloadServer}artifact/latest.json"),
                            Environment.CurrentDirectory + @"\CMFL\Temp\AuthLib-Injector\", "latest.json")
                        .ConfigureAwait(true);

                    LogHelper.WriteLogLine(LanguageHelper.GetField("StartDownloadAuthlibBuild"),
                        LogHelper.LogLevels.Info, false);
                    var latestJson =
                        File.ReadAllText(Environment.CurrentDirectory +
                                         @"\CMFL\Temp\AuthLib-Injector\latest.json");
                    var authLibLatestJson = JsonConvert.DeserializeObject<AuthLibLatestJson>(latestJson);

                    await DownloadHelper.DownloadSingleFileAsyncWithEvent(new Uri(authLibLatestJson.DownloadUrl),
                        Environment.CurrentDirectory + @"\CMFL\Temp\AuthLib-Injector\",
                        authLibLatestJson.DownloadUrl.Split('/').Last()).ConfigureAwait(true);

                    if (AccountInfo.GetSelectedAccount() is OnlineAccount online1)
                    {
                        launchSettings.GameArguments.AgentPath =
                            $"{Environment.CurrentDirectory}\\CMFL\\Temp\\AuthLib-Injector\\{authLibLatestJson.DownloadUrl.Split('/').Last()}";
                        launchSettings.GameArguments.JavaAgentAdditionPara = online1.AuthServer;
                    }

                    LogHelper.WriteLogLine(
                        $" +----------------------------------+{Environment.NewLine} 丨 {LanguageHelper.GetField("DownloadAuthlibBuildSuccess")}               丨{Environment.NewLine} +----------------------------------+",
                        LogHelper.LogLevels.Info, false);

                    return new TaskResult<bool>(TaskResultStatus.Success,
                        value: await StartGameFromConfig(launchSettings).ConfigureAwait(true));
                }
                catch (WebException e)
                {
                    LogHelper.WriteLogLine(
                        $" +----------------------------------+{Environment.NewLine} 丨 {LanguageHelper.GetField("DownloadAuthlibBuildFailed")}         丨{Environment.NewLine} +----------------------------------+",
                        LogHelper.LogLevels.Info, false);
                    LogHelper.WriteError(e);
                    return new TaskResult<bool>(TaskResultStatus.Error);
                }

                #endregion
            });
        }

        #endregion
    }
}