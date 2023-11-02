using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.Launcher;
using GalaSoft.MvvmLight.Threading;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;

namespace CMFL.MVVM.Models.DataModel.Optifine
{
    /// <summary>
    ///     OptiFine构建数据模型
    /// </summary>
    public class OptifineBuilds : PropertyChange
    {
        private string _installMessage;

        private bool _installSnackBarActive;

        /// <summary>
        ///     ID
        /// </summary>
        [JsonProperty("_id")]
        public string IdLocker { get; set; }

        /// <summary>
        ///     MC版本
        /// </summary>
        [JsonProperty("mcversion")]
        public string McVersion { get; set; }

        /// <summary>
        ///     类型
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        ///     修订
        /// </summary>
        [JsonProperty("patch")]
        public string Patch { get; set; }

        /// <summary>
        ///     文件名
        /// </summary>
        [JsonProperty("filename")]
        public string Filename { get; set; }

        /// <summary>
        ///     版本锁
        /// </summary>
        [JsonProperty("__v")]
        public int VersionLocker { get; set; }

        /// <summary>
        ///     Tag标签（按钮用）
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        ///     建议
        /// </summary>
        public string Suggestion { get; set; }

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

        public ICommand DownloadOptifineCommand
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    var info = Tag.Split('|');

                    InstallSnackBarActive = true;
                    InstallMessage =
                        $"{LanguageHelper.GetFields("Start|Download")}Optifine：{string.Join("_", info[0], info[1], info[2])}";

                    Analytics.TrackEvent(AnalyticsEventNames.DownloadOptifine, new Dictionary<string, string>(1)
                    {
                        {"Version", Tag}
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
                                        $"{LanguageHelper.GetFields("Start|Download")}{string.Join("_", info[0], info[1], info[2])} ({args.ProgressPercentage:P1})";
                                });
                            },
                            Completed = (sender, args) =>
                            {
                                try
                                {
                                    Process.Start($@"{Environment.CurrentDirectory}\CMFL\Download\");

                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        InstallMessage =
                                            $"Optifine{LanguageHelper.GetFields("Download|Succeeded")}";
                                        InstallSnackBarActive = false;
                                    });
                                }
                                catch (Win32Exception ex)
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        InstallMessage =
                                            $"Optifine{LanguageHelper.GetFields("Download|Failed")}";
                                        InstallSnackBarActive = false;
                                    });

                                    LogHelper.WriteLogLine($"Optifine {LanguageHelper.GetField("Access")}",
                                        LogHelper.LogLevels.Error);
                                    LogHelper.WriteError(ex);
                                }
                            },
                            DownloadUri =
                                $"{SettingsHelper.BmclapiDownloadServer}optifine/{info[0]}/{info[1]}/{info[2]}",
                            DownloadPath =
                                $"{Environment.CurrentDirectory}\\CMFL\\Download\\{string.Join("_", info[0], info[1], info[2])}.jar",
                            FileName = $"{string.Join("_", info[0], info[1], info[2])}.jar"
                        };

                        await DownloadHelper.MultiPartDownloadTaskAsync(dF).ConfigureAwait(false);
                    }
                    catch (WebException e)
                    {
                        NotifyHelper.GetBasicMessageWithBadge(LanguageHelper.GetFields("Download|Failed"),
                            NotifyHelper.MessageType.Error).Queue();
                        LogHelper.WriteLogLine(
                            $"{LanguageHelper.GetFields("Download|Failed")}",
                            LogHelper.LogLevels.Error);
                        LogHelper.WriteError(e);
                    }
                });
            }
        }
    }
}