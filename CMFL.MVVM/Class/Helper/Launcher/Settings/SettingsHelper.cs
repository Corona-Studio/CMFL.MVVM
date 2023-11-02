using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMFL.MVVM.Class.Exceptions;
using CMFL.MVVM.Class.Helper.Game;
using CMFL.MVVM.Models.DataModel.Game;
using CMFL.MVVM.Models.DataModel.Game.Account;
using CMFL.MVVM.Models.DataModel.GameData;
using Newtonsoft.Json;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model.LauncherProfile;

namespace CMFL.MVVM.Class.Helper.Launcher.Settings
{
    public static class SettingsHelper
    {
        /// <summary>
        ///     启动器版本号（变更集编号）
        /// </summary>
        public const int LauncherMajor = 1;

        public const int LauncherMinor = 76;
        public const int LauncherMirror = 40;

        public const string MojangAssetDownloadServer = "http://resources.download.minecraft.net/";
        public const string MojangDownloadServer = "https://libraries.minecraft.net/";
        public const string AuthLibInjectorDownloadServer = "https://authlib-injector.yushi.moe/";
        public const string ForgeDownloadServer = "http://files.minecraftforge.net/";
        public const string McBbsNewsServer = "http://news.api.backstage.craftmine.fun:3001/rest/api/news";

        private static readonly string SettingsPath = $"{Environment.CurrentDirectory}\\CMFL.json";
        private static readonly object Locker = new object();

        /// <summary>
        ///     Mod
        /// </summary>
        public static readonly Dictionary<string, List<ModData>> Mods = new Dictionary<string, List<ModData>>();

        /// <summary>
        ///     材质包
        /// </summary>
        public static readonly Dictionary<string, List<ResPackData>> ResPack =
            new Dictionary<string, List<ResPackData>>();

        /// <summary>
        ///     截图
        /// </summary>
        public static readonly Dictionary<string, List<ScreenshotData>> ScreenShot =
            new Dictionary<string, List<ScreenshotData>>();

        /// <summary>
        ///     光影包
        /// </summary>
        public static readonly Dictionary<string, List<ShaderPackData>> ShaderPack =
            new Dictionary<string, List<ShaderPackData>>();

        /// <summary>
        ///     存档
        /// </summary>
        public static readonly Dictionary<string, List<SaveData>> Saves = new Dictionary<string, List<SaveData>>();

        private static SettingProxy _settings;

        private static readonly object LockObject = new object();

        public static string BmclapiDownloadServer =>
            Settings?.UseHttpsForBmclapi ?? false
                ? "https://download.mcbbs.net/"
                : "http://download.mcbbs.net/";

        public static SettingProxy Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                Save();
            }
        }

        public static void GetSettingsFromDisk()
        {
            lock (Locker)
            {
                try
                {
                    _settings = JsonConvert.DeserializeObject<SettingProxy>(File.ReadAllText(SettingsPath),
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All
                        });
                    LogHelper.WriteLogLine(LanguageHelper.GetField("FoundLauncherConfigureFile"),
                        LogHelper.LogLevels.Info);
                }
                catch (FileNotFoundException)
                {
                    LogHelper.WriteLogLine(LanguageHelper.GetField("NotFoundLauncherConfigureFile"),
                        LogHelper.LogLevels.Info);
                    _settings = new SettingProxy
                    {
                        FirstTime = true,
                        AccountInfos = new List<AccountInfo>(),
                        AnimationFps = 60,
                        AutoCleanMemory = true,
                        AutoDetectBestDownloadServer = true,
                        BgmVolume = 0.5,
                        BgPath = "/CMFL.MVVM;component/Assets/Images/bg.jpg",
                        BlurRadius = 0.0,
                        ChoseGamePath = ".minecraft/",
                        EnableAnimation = true,
                        EnableBMCLAPI = false,
                        UseMojangServer = false,
                        GamePaths = new List<GamePathModel>
                        {
                            new GamePathModel
                            {
                                Name = LanguageHelper.GetField("DefaultPath"),
                                Path = ".minecraft/",
                                IsChecked = true
                            }
                        },
                        GameTimes = new Dictionary<string, long>(),
                        ResumeMusic = true,
                        SelectedLanguageIndex = 0,
                        SelectedInterfaceFont = "Microsoft YaHei UI",
                        ClientToken = Guid.NewGuid().ToString("N"),
                        GameSettings = new Dictionary<string, GameSettings>(),
                        FallBackGameSettings = new GameSettings
                        {
                            AutoMemorySize = true,
                            EnableGc = true,
                            GcType = 1,
                            MinMemory = 512,
                            MaxMemory = 1024,
                            ScreenSize = new ResolutionModel
                            {
                                FullScreen = true,
                                Height = 600,
                                Width = 800
                            }
                        },
                        LeftBorderBlurRadius = 50,
                        LeftBorderOpacityLayerVisibility = true,
                        LauncherSocketServer = "socks.api.backstage.craftmine.fun",
                        LauncherWebServer = "http://web.api.backstage.craftmine.fun:3000",
                        DownloadThread = ProcessorHelper.GetPhysicalProcessorCount() * 2
                    };
                    Save();
                    LogHelper.WriteLogLine(LanguageHelper.GetField("LauncherConfigureFileInitialized"),
                        LogHelper.LogLevels.Info);
                }
                catch (ArgumentException e)
                {
                    NotifyHelper.ShowNotification(LanguageHelper.GetField("LauncherConfigureFileLoadingFailed"),
                        LanguageHelper.GetField("PleaseCheckLog"), 3000, ToolTipIcon.Error);
                    LogHelper.WriteLogLine(LanguageHelper.GetField("LauncherConfigureFileLoadingFailed"),
                        LogHelper.LogLevels.Error);
                    LogHelper.WriteError(e);
                    return;
                }

                LogHelper.WriteLogLine(LanguageHelper.GetField("LauncherConfigureFileLoaded"),
                    LogHelper.LogLevels.Info);
            }
        }

        public static async void Save()
        {
            await Task.Run(() =>
            {
                lock (Locker)
                {
                    var settingsStr = JsonConvert.SerializeObject(_settings, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                    FileHelper.Write(SettingsPath, settingsStr);
                }
            }).ConfigureAwait(true);
        }

        public static async Task GetGamesInfo()
        {
            await Task.Run(() =>
            {
                lock (LockObject)
                {
                    if (Mods.Any()) Mods.Clear();

                    if (ResPack.Any()) ResPack.Clear();

                    if (ScreenShot.Any()) ScreenShot.Clear();

                    if (ShaderPack.Any()) ShaderPack.Clear();

                    if (Saves.Any()) Saves.Clear();

                    var games = GameHelper.GetListGames();

                    if (games == null)
                    {
                        LogHelper.WriteLogLine(LanguageHelper.GetField("GameNotFound"), LogHelper.LogLevels.Warning);
                        LogHelper.WriteError(new GameNotFoundException());
                        return;
                    }

                    foreach (var game in games)
                    {
                        var gamePath = Settings.VersionInsulation switch
                        {
                            true => $@"{Settings.ChoseGamePath}\versions\{game.Id}\",
                            false => Settings.ChoseGamePath
                        };

                        Mods.Add(game.Id, Searcher.SearchItem<ModData>(gamePath, false));
                        ResPack.Add(game.Id, Searcher.SearchItem<ResPackData>(gamePath, false));
                        ScreenShot.Add(game.Id, Searcher.SearchItem<ScreenshotData>(gamePath, false));
                        ShaderPack.Add(game.Id, Searcher.SearchItem<ShaderPackData>(gamePath, false));
                        Saves.Add(game.Id, Searcher.SearchItem<SaveData>(gamePath, true));
                    }
                }
            }).ConfigureAwait(true);
        }
    }
}