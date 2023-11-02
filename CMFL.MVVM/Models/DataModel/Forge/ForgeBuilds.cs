using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.Launcher;
using CMFL.MVVM.ViewModels;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Threading;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;

namespace CMFL.MVVM.Models.DataModel.Forge
{
    /// <summary>
    ///     文件信息
    /// </summary>
    public class Files
    {
        /// <summary>
        ///     格式
        /// </summary>
        [JsonProperty("format")]
        public string Format { get; set; }

        /// <summary>
        ///     分类
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        ///     Hash值
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        ///     _id锁
        /// </summary>
        [JsonProperty("_id")]
        public string IdLocker { get; set; }
    }

    /// <summary>
    ///     ForgeBuild
    /// </summary>
    public class ForgeBuilds : PropertyChange
    {
        private string _installMessage;

        private bool _installSnackBarActive;

        /// <summary>
        ///     分支
        /// </summary>
        [JsonProperty("branch")]
        public string Branch { get; set; }

        /// <summary>
        ///     Build
        /// </summary>
        [JsonProperty("build")]
        public int Build { get; set; }

        /// <summary>
        ///     MC版本
        /// </summary>
        [JsonProperty("mcversion")]
        public string McVersion { get; set; }

        /// <summary>
        ///     修改日期
        /// </summary>
        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

        /// <summary>
        ///     版本
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        ///     ID锁
        /// </summary>
        [JsonProperty("_id")]
        public string IdLocker { get; set; }

        /// <summary>
        ///     文件信息
        /// </summary>
        [JsonProperty("files")]
        public Files[] Files { get; set; }

        public string InstallMessage
        {
            get => _installMessage;
            set
            {
                _installMessage = value;
                OnPropertyChanged(nameof(InstallMessage));
            }
        }

        public bool InstallSnackBarActive
        {
            get => _installSnackBarActive;
            set
            {
                _installSnackBarActive = value;
                OnPropertyChanged(nameof(InstallSnackBarActive));
            }
        }

        public ICommand DownloadForgeCommand
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    var selectedForgeVersion =
                        ServiceLocator.Current.GetInstance<GamePageViewModel>().SelectedForgeVersion;
                    Analytics.TrackEvent(AnalyticsEventNames.DownloadForge, new Dictionary<string, string>(1)
                    {
                        {"Version", selectedForgeVersion}
                    });
                    var downloadServer = SettingsHelper.Settings.EnableBMCLAPI
                        ? $"{SettingsHelper.BmclapiDownloadServer}forge/download/{Version.Split('.').Last()}"
                        : $"{SettingsHelper.ForgeDownloadServer}maven/net/minecraftforge/forge/{selectedForgeVersion}-{Version}/forge-{selectedForgeVersion}-{Version}-installer.jar";

                    if (!Directory.Exists(Environment.CurrentDirectory + $@"\CMFL\Temp\{Version.Split('.').Last()}\"))
                        Directory.CreateDirectory(Environment.CurrentDirectory +
                                                  $@"\CMFL\Temp\{Version.Split('.').Last()}\");

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        InstallSnackBarActive = true;
                        InstallMessage =
                            $"{LanguageHelper.GetFields("Start|Download")} Forge：{Version}";
                    });

                    try
                    {
                        var dF = new DownloadFile
                        {
                            Changed = (sender, args) =>
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    InstallMessage =
                                        $"{LanguageHelper.GetFields("Start|Download")}{Version.Split('.').Last()}（{args.ProgressPercentage:P1}）";
                                });
                            },
                            Completed = (sender, args) =>
                            {
                                try
                                {
                                    Process.Start(
                                        $@"{Environment.CurrentDirectory}\CMFL\Temp\{Version.Split('.').Last()}\{Version}.jar");
                                }
                                catch (FileNotFoundException ex)
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        NotifyHelper
                                            .GetBasicMessageWithBadge(
                                                LanguageHelper.GetFields("Download|Failed", "Forge"),
                                                NotifyHelper.MessageType.Error).Queue();
                                    });
                                    LogHelper.WriteLogLine(LanguageHelper.GetFields("Decompress|Failed"),
                                        LogHelper.LogLevels.Error);
                                    LogHelper.WriteError(ex);
                                }
                            },
                            DownloadPath =
                                $"{Environment.CurrentDirectory}\\CMFL\\Temp\\{Version.Split('.').Last()}\\{Version}.jar",
                            DownloadUri = downloadServer,
                            FileName = $"{Version}.jar"
                        };

                        await DownloadHelper.MultiPartDownloadTaskAsync(dF).ConfigureAwait(false);
                    }
                    catch (WebException e)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            NotifyHelper
                                .GetBasicMessageWithBadge(LanguageHelper.GetFields("Download|Failed"),
                                    NotifyHelper.MessageType.Error).Queue();
                        });
                        LogHelper.WriteLogLine(LanguageHelper.GetFields("Download|Failed"),
                            LogHelper.LogLevels.Error);
                        LogHelper.WriteError(e);
                    }
                });
            }
        }
    }
}