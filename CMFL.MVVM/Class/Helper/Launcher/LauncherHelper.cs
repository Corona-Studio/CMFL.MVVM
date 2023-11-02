using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMFL.MVVM.Class.Helper.Kernel;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.Launcher.SocketServer;
using CMFL.MVVM.Class.Helper.Web;
using CMFL.MVVM.Models.DataModel.Game.Account;
using CMFL.MVVM.Models.DataModel.Launcher;
using CMFL.MVVM.Models.DataModel.Launcher.Feedback;
using CMFL.MVVM.Models.DataModel.News;
using IWshRuntimeLibrary;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ProjBobcat.Class.Model;
using File = System.IO.File;
using FileInfo = System.IO.FileInfo;

namespace CMFL.MVVM.Class.Helper.Launcher
{
    /// <summary>
    ///     启动助手
    /// </summary>
    public static class LauncherHelper
    {
        /// <summary>
        ///     文件类型
        /// </summary>
        public enum FileType
        {
            /// <summary>
            ///     材质包
            /// </summary>
            ResourcePack = 0,

            /// <summary>
            ///     MOD
            /// </summary>
            Mod,

            /// <summary>
            ///     光影包
            /// </summary>
            ShaderPack,

            /// <summary>
            ///     截图
            /// </summary>
            ScreenShot,

            /// <summary>
            ///     存档
            /// </summary>
            Save
        }

        /// <summary>
        ///     获取启动器公告
        /// </summary>
        /// <returns></returns>
        public static async Task<TaskResult<List<BulletinData>>> GetBulletin()
        {
            return await Task.Run(() =>
            {
                var bulletinJson = LauncherSocketServerHelper.Send("ANNOUNCEMENT");

                if (string.IsNullOrEmpty(bulletinJson))
                    return new TaskResult<List<BulletinData>>(TaskResultStatus.Error);

                return new TaskResult<List<BulletinData>>(TaskResultStatus.Success,
                    value: JsonConvert.DeserializeObject<List<BulletinData>>(bulletinJson));
            }).ConfigureAwait(true);
        }

        /// <summary>
        ///     获取移动文件的路径
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        public static string GetMoveFilePath(FileType type)
        {
            var path = string.Empty;
            if (!SettingsHelper.Settings.VersionInsulation)
                path = type switch
                {
                    FileType.ResourcePack => $@"{SettingsHelper.Settings.ChoseGamePath}\resourcepacks\",
                    FileType.Mod => $@"{SettingsHelper.Settings.ChoseGamePath}\mods\",
                    FileType.Save => $@"{SettingsHelper.Settings.ChoseGamePath}\saves\",
                    FileType.ScreenShot => $@"{SettingsHelper.Settings.ChoseGamePath}\screenshots\",
                    FileType.ShaderPack => $@"{SettingsHelper.Settings.ChoseGamePath}\shaderpacks\",
                    _ => path
                };
            else
                path = type switch
                {
                    FileType.ResourcePack =>
                    $@"{SettingsHelper.Settings.ChoseGamePath}\versions\{SettingsHelper.Settings.ChoseGame}\resourcepacks\",
                    FileType.Mod =>
                    $@"{SettingsHelper.Settings.ChoseGamePath}\versions\{SettingsHelper.Settings.ChoseGame}\mods\",
                    FileType.Save =>
                    $@"{SettingsHelper.Settings.ChoseGamePath}\versions\{SettingsHelper.Settings.ChoseGame}\saves\",
                    FileType.ScreenShot =>
                    $@"{SettingsHelper.Settings.ChoseGamePath}\versions\{SettingsHelper.Settings.ChoseGame}\screenshots\",
                    FileType.ShaderPack =>
                    $@"{SettingsHelper.Settings.ChoseGamePath}\versions\{SettingsHelper.Settings.ChoseGame}\shaderpacks\",
                    _ => path
                };

            return path;
        }

        /// <summary>
        ///     获取BGM
        /// </summary>
        /// <returns></returns>
        public static async Task<TaskResult<BgmInfoModel>> GetMusicString()
        {
            return await Task.Run(async () =>
            {
                if (SettingsHelper.Settings.UseLocalMusic &&
                    !string.IsNullOrEmpty(SettingsHelper.Settings.MusicFilePath))
                {
                    var musicInfo = new BgmInfoModel
                    {
                        UseLocalMusic = true,
                        LocalMusicPath = SettingsHelper.Settings.MusicFilePath,
                        Name = SettingsHelper.Settings.MusicFilePath.Split('\\').Last()
                    };

                    return new TaskResult<BgmInfoModel>(TaskResultStatus.Success, value: musicInfo);
                }

                LogHelper.WriteLogLine(LanguageHelper.GetField("StartingGetBgm"), LogHelper.LogLevels.Info);
                using var webClient = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                var bgmJson = await webClient
                    .DownloadStringTaskAsync($"{SettingsHelper.Settings.LauncherWebServer}/api/music")
                    .ConfigureAwait(true);
                return new TaskResult<BgmInfoModel>(TaskResultStatus.Success,
                    value: JsonConvert.DeserializeObject<BgmInfoModel>(bgmJson));
            }).ConfigureAwait(true);
        }

        public static async Task<TaskResult<List<McbbsNews>>> GetMcBbsNews()
        {
            return await Task.Run(async () =>
            {
                var newsClient = new WebClient
                {
                    Encoding = Encoding.UTF8
                };

                LogHelper.WriteLogLine(LanguageHelper.GetFields("StartGetting|McbbsNews"),
                    LogHelper.LogLevels.Info);
                var serverReply = await newsClient.DownloadStringTaskAsync(new Uri(SettingsHelper.McBbsNewsServer))
                    .ConfigureAwait(true);
                var news = JsonConvert.DeserializeObject<List<McbbsNews>>(serverReply);

                return new TaskResult<List<McbbsNews>>(TaskResultStatus.Success, value: news);
            }).ConfigureAwait(true);
        }

        public static async Task<string> CheckLauncherAuthInfo(string userName, string password)
        {
            return await Task.Run(() =>
            {
                var resultJson = LauncherSocketServerHelper
                    .Send("LOGIN", new[] {userName, password});

                if (string.IsNullOrEmpty(resultJson)) return string.Empty;

                var result =
                    JsonConvert.DeserializeObject<CommandResultModel>(resultJson);
                return result.Result;
            }).ConfigureAwait(true);
        }

        public static async Task<TaskResult<List<FeedbackBindingModel>>> GetFeedBacks(string user)
        {
            return await Task.Run(() =>
            {
                var feedBacks = LauncherSocketServerHelper.Send("GETFEEDBACK", new[] {user ?? "ALLFEEDBACK"});

                if (string.IsNullOrEmpty(feedBacks))
                    return new TaskResult<List<FeedbackBindingModel>>(TaskResultStatus.Error);

                var feedBackJson = JsonConvert.DeserializeObject<List<FeedbackModel>>(feedBacks);
                var result = new List<FeedbackBindingModel>();

                feedBackJson.ForEach(feedback =>
                {
                    var feedbackModel = new FeedbackBindingModel
                    {
                        Date = feedback.Date,
                        AdminReply = feedback.AdminReply,
                        Content = feedback.Content,
                        Tags = new List<TagModel>(),
                        Title = feedback.Title,
                        User = feedback.User
                    };
                    var tags = new List<FeedbackTagType>
                    {
                        (FeedbackTagType) feedback.UserTag
                    };
                    feedback.AdminTag?.Split('|').ToList()
                        .ForEach(tag => { tags.Add((FeedbackTagType) int.Parse(tag)); });

                    tags.ForEach(tagType =>
                    {
                        var text = "";
                        var color = "";

                        switch (tagType)
                        {
                            case FeedbackTagType.FeatureRequest:
                                text = LanguageHelper.GetField("FeedBackTag1");
                                color = "#00e676";
                                break;
                            case FeedbackTagType.BugReport:
                                text = LanguageHelper.GetField("FeedBackTag2");
                                color = "#ff5252";
                                break;
                            case FeedbackTagType.Question:
                                text = LanguageHelper.GetField("FeedBackTag3");
                                color = "#42a5f5";
                                break;
                            case FeedbackTagType.WaitingForResponse:
                                text = LanguageHelper.GetField("FeedBackTag4");
                                color = "#a7c0cd";
                                break;
                            case FeedbackTagType.FurtherInfoRequired:
                                text = LanguageHelper.GetField("FeedBackTag5");
                                color = "#fbc02d";
                                break;
                            case FeedbackTagType.Closed:
                                text = LanguageHelper.GetField("FeedBackTag6");
                                color = "#ef5350";
                                break;
                        }

                        feedbackModel.Tags.Add(new TagModel
                        {
                            Color = color,
                            Text = text
                        });
                    });

                    result.Add(feedbackModel);
                });

                return new TaskResult<List<FeedbackBindingModel>>(TaskResultStatus.Success, value: result);
            }).ConfigureAwait(true);
        }

        /// <summary>
        ///     获取CraftMineFun建立至今的天数
        /// </summary>
        /// <returns>天数</returns>
        public static string GetFoundationTime()
        {
            var timeWhenCmfFounded = new DateTime(2016, 7, 26, 00, 00, 00);
            var timeNow = DateTime.Now;
            var time = timeNow.Subtract(timeWhenCmfFounded);
            return
                $"{time.Days}{LanguageHelper.GetField("Day")}{time.Hours}{LanguageHelper.GetField("Hours")}{time.Minutes}{LanguageHelper.GetField("Minutes")}{time.Seconds}{LanguageHelper.GetField("Seconds")}";
        }

        /// <summary>
        ///     复制文件
        /// </summary>
        /// <param name="sourcePath">源目录</param>
        /// <param name="destinationPath">目的地</param>
        /// <param name="overwriteExisting">是否覆盖已存在的文件</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static bool CopyDirectory(string sourcePath, string destinationPath, bool overwriteExisting)
        {
            if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destinationPath))
                throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            bool ret;
            try
            {
                sourcePath = sourcePath.EndsWith("\\", StringComparison.Ordinal) ? sourcePath : $"{sourcePath}\\";
                destinationPath = destinationPath.EndsWith("\\", StringComparison.Ordinal)
                    ? destinationPath
                    : $"{destinationPath}\\";

                if (Directory.Exists(sourcePath))
                {
                    if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);

                    foreach (var fls in Directory.GetFiles(sourcePath))
                    {
                        var fileInfo = new FileInfo(fls);
                        fileInfo.CopyTo($"{destinationPath}{fileInfo.Name}", overwriteExisting);
                    }

                    foreach (var drs in Directory.GetDirectories(sourcePath))
                    {
                        var directoryInfo = new DirectoryInfo(drs);
                        if (!CopyDirectory(drs, $"{destinationPath}{directoryInfo.Name}", overwriteExisting))
                            ret = false;
                    }
                }

                ret = true;
            }
            catch (ArgumentException)
            {
                ret = false;
            }
            catch (IOException)
            {
                ret = false;
            }
            catch (SecurityException)
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        ///     游戏环境检测
        /// </summary>
        /// <returns>问题列表</returns>
        public static Task<ObservableCollection<ProblemList>> CheckEnvironment()
        {
            return Task.Run(() =>
            {
                var iconColor = string.Empty;
                var problemLists = new ObservableCollection<ProblemList>();
                var listDictionary = new Dictionary<string, PackIconKind>();

                if (NetworkHelper.SystemProxyCheck() && AccountInfo.GetSelectedAccount().IsOnlineAccount())
                    listDictionary.Add(LanguageHelper.GetField("EnvProblem1"), PackIconKind.Alert);

                if (!SettingsHelper.Settings.AccountInfos.Any())
                    listDictionary.Add(LanguageHelper.GetField("EnvProblem2"), PackIconKind.CloseCircle);

                if (string.IsNullOrEmpty(SettingsHelper.Settings.FallBackGameSettings.JavaPath))
                {
                    listDictionary.Add("没有找到java!", PackIconKind.CloseCircle);
                }
                else
                {
                    if (SettingsHelper.Settings.FallBackGameSettings.JavaPath?.Contains("Program Files") != true)
                        listDictionary.Add(LanguageHelper.GetField("EnvProblem3"), PackIconKind.Information);

                    if (SystemInfoHelper.SysInfo["OperatingSystemInfo"].Equals("64Bits", StringComparison.Ordinal) &&
                        SettingsHelper.Settings.FallBackGameSettings.JavaPath.Contains("x86"))
                        listDictionary.Add(LanguageHelper.GetField("EnvProblem4"), PackIconKind.Information);
                }

                if (SettingsHelper.Settings.FallBackGameSettings.MaxMemory <= 512)
                    listDictionary.Add(LanguageHelper.GetField("EnvProblem5"), PackIconKind.Alert);

                if (!listDictionary.Any()) return problemLists;
                foreach (var item in listDictionary)
                {
                    var text = item.Key.Split('|');

                    iconColor = item.Value switch
                    {
                        PackIconKind.CheckboxMarkedCircle => "#DD02CD02",
                        PackIconKind.Information => "#DD0078F0",
                        PackIconKind.Alert => "#DDFFDC00",
                        PackIconKind.CloseCircle => "#DDFF3434",
                        _ => iconColor
                    };

                    problemLists.Add(new ProblemList
                    {
                        Title = text.First(),
                        Context = text.Last(),
                        IconColor = iconColor,
                        Kind = item.Value
                    });
                }

                return problemLists;
            }, CancellationToken.None);
        }

        public static Task<TaskResult<UpdateInfoModel>> CheckLauncherUpgrade()
        {
            return Task.Run(async () =>
            {
                using var wc = new WebClient {Encoding = Encoding.UTF8};
                var str = await wc.DownloadStringTaskAsync(
                    new Uri($"{SettingsHelper.Settings.LauncherWebServer}/api/update")).ConfigureAwait(true);

                var updateInfo = JsonConvert.DeserializeObject<UpdateInfoModel>(str);

                return new TaskResult<UpdateInfoModel>(TaskResultStatus.Success, value: updateInfo);
            });
        }

        /// <summary>
        ///     删除程序自身
        /// </summary>
        public static void BeginKillSelf(string currentStr, string newStr)
        {
            if (!Directory.Exists($"{Environment.CurrentDirectory}\\CMFL\\Temp\\"))
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\CMFL\\Temp\\");

            var vBatFile = $"{Environment.CurrentDirectory}\\CMFL\\Temp\\Update.bat";

            using (var vStreamWriter = new StreamWriter(vBatFile, false, Encoding.Default))
            {
                var sb = new StringBuilder();
                sb
                    .Append(":del").Append(Environment.NewLine)
                    .AppendFormat(" del \"{0}\"", Application.ExecutablePath).Append(Environment.NewLine)
                    .AppendFormat("if exist \"{0}\" goto del", Application.ExecutablePath).Append(Environment.NewLine)
                    .AppendFormat("ren {0} {1}", currentStr, newStr).Append(Environment.NewLine)
                    .Append($"{Environment.CurrentDirectory}\\{newStr}").Append(Environment.NewLine)
                    .Append("del %0").Append(Environment.NewLine);
                vStreamWriter.Write(sb);
            }

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = vBatFile,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };
            Process.Start(startInfo);
        }

        public static void CreateShortcutOnDesktop(string path)
        {
            var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                "CMFL.lnk");
            if (File.Exists(shortcutPath)) return;

            // 获取当前应用程序目录地址
            var exePath = path;
            var shell = new WshShell();
            // 确定是否已经创建的快捷键被改名了
            if (Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "*.lnk")
                .Select(item => (WshShortcut) shell.CreateShortcut(item))
                .Any(tempShortcut => tempShortcut.TargetPath == exePath)) return;

            Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "*.lnk")
                .Select(item => (WshShortcut) shell.CreateShortcut(item))
                .Where(tempShortcut => tempShortcut.TargetPath == Application.ExecutablePath)
                .ToList().ForEach(shortCut => { File.Delete(shortCut.TargetPath); });

            var shortcut = (WshShortcut) shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = exePath;
            shortcut.Arguments = ""; // 参数  
            shortcut.Description = LanguageHelper.GetField("LauncherShortCut");
            shortcut.WorkingDirectory = Environment.CurrentDirectory; //程序所在文件夹，在快捷方式图标点击右键可以看到此属性  
            shortcut.IconLocation = exePath; //图标，该图标是应用程序的资源文件  
            shortcut.WindowStyle = 1;
            shortcut.Save();
        }
    }
}