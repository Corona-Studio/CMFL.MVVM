using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.Launcher;
using CMFL.MVVM.ViewModels;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Threading;
using Microsoft.AppCenter.Analytics;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;

namespace CMFL.MVVM.Models.DataModel.BMCLAPI
{
    /// <summary>
    ///     BMCLAPI返回的游戏序列化数据结构
    /// </summary>
    public class OnlineGameInfo : PropertyChange
    {
        private string _installMessage;

        private bool _installSnackBarActive;

        private bool _isDownloadButtonEnable;

        /// <summary>
        ///     游戏ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     类型（中文）
        /// </summary>
        public string CnType { get; set; }

        /// <summary>
        ///     下载建议
        /// </summary>
        public string Suggestion { get; set; }

        /// <summary>
        ///     类型（英文）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     发布时间
        /// </summary>
        public string Time { get; set; }

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

        public bool IsDownloadButtonEnable
        {
            get => _isDownloadButtonEnable;
            set
            {
                _isDownloadButtonEnable = value;
                OnPropertyChanged(nameof(IsDownloadButtonEnable));
            }
        }


        public ICommand DownloadGameCommand
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    Analytics.TrackEvent(AnalyticsEventNames.DownloadMineCraft, new Dictionary<string, string>(1)
                    {
                        {"Version", Id}
                    });
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        InstallSnackBarActive = true;
                        InstallMessage = $"开始下载游戏：{Id}";
                        IsDownloadButtonEnable = false;
                    });

                    if (!Directory.Exists($@"{SettingsHelper.Settings.ChoseGamePath}\versions\{Id}"))
                        Directory.CreateDirectory($@"{SettingsHelper.Settings.ChoseGamePath}\versions\{Id}");

                    try
                    {
                        var dF = new DownloadFile
                        {
                            Changed = (sender, args) =>
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    InstallMessage = $"正在下载{Id}的核心JAR（{args.ProgressPercentage:P1}）";
                                });
                            },
                            Completed = (sender, args) =>
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() => { InstallMessage = "核心文件下载完成"; });
                            },
                            DownloadPath = $"{SettingsHelper.Settings.ChoseGamePath}\\versions\\{Id}\\{Id}.jar",
                            DownloadUri = $"{SettingsHelper.BmclapiDownloadServer}version/{Id}/client",
                            FileName = $"{Id}.jar"
                        };

                        await DownloadHelper.MultiPartDownloadTaskAsync(dF).ConfigureAwait(false);

                        //下载Json
                        await DownloadHelper.DownloadSingleFileAsyncWithEvent(
                                new Uri($"{SettingsHelper.BmclapiDownloadServer}version/{Id}/json"),
                                $@"{SettingsHelper.Settings.ChoseGamePath}\versions\{Id}\", $"{Id}.json",
                                (o, args) =>
                                {
                                    ServiceLocator.Current.GetInstance<GamePageViewModel>().GetAllLocalGames();
                                    InstallMessage = "Json文件下载完成";
                                    InstallSnackBarActive = false;
                                    IsDownloadButtonEnable = true;
                                },
                                (o, args) => { InstallMessage = $"正在下载{Id}的Json（{args.ProgressPercentage:D}%）"; })
                            .ConfigureAwait(false);
                    }
                    catch (WebException e)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            InstallSnackBarActive = false;
                            NotifyHelper.GetBasicMessageWithBadge("下载失败，可能是下载服务器出现了异常。", NotifyHelper.MessageType.Error)
                                .Queue();
                        });

                        LogHelper.WriteLogLine("原版游戏下载失败", LogHelper.LogLevels.Error);
                        LogHelper.WriteError(e);
                    }
                    catch (UriFormatException ex)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            InstallSnackBarActive = false;
                            NotifyHelper.GetBasicMessageWithBadge("下载失败，原因：Uri不合法", NotifyHelper.MessageType.Error)
                                .Queue();
                        });

                        LogHelper.WriteLogLine("原版游戏下载失败", LogHelper.LogLevels.Error);
                        LogHelper.WriteError(ex);
                    }
                });
            }
        }
    }
}