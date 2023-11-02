using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.Launcher.SocketServer;
using CMFL.MVVM.Models.DataModel.Launcher;
using CMFL.MVVM.Models.DataModel.Launcher.Feedback;
using GalaSoft.MvvmLight.Threading;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ProjBobcat.Class.Model;

namespace CMFL.MVVM.ViewModels
{
    public class FeedbackPageViewModel : PropertyChange, IDisposable
    {
        private ObservableCollection<FeedbackBindingModel> _feedbackList;

        private bool _loadingPanelVisibility = true;

        private string _preFeedBackContent;

        private string _preFeedBackTitle;

        private int _selectedTagIndex = -1;

        public IEnumerable<string> Tags { get; } = new List<string>
        {
            "特性提议",
            "BUG反馈"
        };

        public int SelectedTagIndex
        {
            get => _selectedTagIndex;
            set
            {
                _selectedTagIndex = value;
                OnPropertyChanged(nameof(SelectedTagIndex));
            }
        }

        public string PreFeedBackTitle
        {
            get => _preFeedBackTitle;
            set
            {
                _preFeedBackTitle = value;
                OnPropertyChanged(nameof(PreFeedBackTitle));
            }
        }

        public string PreFeedBackContent
        {
            get => _preFeedBackContent;
            set
            {
                _preFeedBackContent = value;
                OnPropertyChanged(nameof(PreFeedBackContent));
            }
        }

        public ObservableCollection<FeedbackBindingModel> FeedbackList
        {
            get => _feedbackList;
            set
            {
                _feedbackList = value;
                OnPropertyChanged(nameof(FeedbackList));
            }
        }

        public bool LoadingPanelVisibility
        {
            get => _loadingPanelVisibility;
            set
            {
                _loadingPanelVisibility = value;
                OnPropertyChanged(nameof(LoadingPanelVisibility));
            }
        }


        public ICommand GetSentFeedBacksCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    LoadingPanelVisibility = true;
                    GetFeedBack(SettingsHelper.Settings.LUsername);
                });
            }
        }

        public ICommand SendFeedBacksCommand
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

                    if (string.IsNullOrWhiteSpace(PreFeedBackContent) ||
                        string.IsNullOrWhiteSpace(PreFeedBackTitle)) return;

                    var result = LauncherSocketServerHelper.Send("FEEDBACK",
                        new[]
                        {
                            DateTime.Now.ToString("U"),
                            SettingsHelper.Settings.LUsername,
                            PreFeedBackTitle,
                            PreFeedBackContent,
                            $"{(SelectedTagIndex.Equals(-1) ? 0 : SelectedTagIndex)}",
                            "3"
                        });

                    if (string.IsNullOrEmpty(result))
                    {
                        NotifyHelper.GetBasicMessageWithBadge("获取反馈失败!", NotifyHelper.MessageType.Error).Queue();
                        return;
                    }

                    var resultJson = JsonConvert.DeserializeObject<CommandResultModel>(result);
                    if (resultJson.Message.Equals("FEEDBACKATMAXIUM", StringComparison.Ordinal))
                    {
                        NotifyHelper.GetBasicMessageWithBadge("您目前的反馈数量已达到了管理员所允许的最大反馈数量（3条），请联系管理员处理！",
                            NotifyHelper.MessageType.Error).Queue();
                        return;
                    }

                    if (resultJson.Message.Equals("FEEDBACK ADDED", StringComparison.Ordinal))
                    {
                        NotifyHelper.GetBasicMessageWithBadge("反馈发送成功！",
                            NotifyHelper.MessageType.Success).Queue();
                        GetFeedBack(null);
                    }
                    else
                    {
                        NotifyHelper.GetBasicMessageWithBadge($"反馈发送失败，服务器返回：{result}",
                            NotifyHelper.MessageType.Error).Queue();
                    }

                    DialogHost.CloseDialogCommand.Execute(null, null);
                    PreFeedBackTitle = string.Empty;
                    PreFeedBackContent = string.Empty;
                    SelectedTagIndex = -1;
                });
            }
        }

        public void GetFeedBack(string user)
        {
            LauncherHelper.GetFeedBacks(user).ContinueWith(task =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { LoadingPanelVisibility = false; });
                if (task.Exception != null || task.Result.TaskStatus != TaskResultStatus.Success)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        NotifyHelper.GetBasicMessageWithBadge(LanguageHelper.GetField("LoadFeedBackFailed"),
                            NotifyHelper.MessageType.Error).Queue();
                    });
                    LogHelper.WriteLogLine(LanguageHelper.GetField("LoadFeedBackFailed"), LogHelper.LogLevels.Error);
                    LogHelper.WriteError(task.Exception);
                    return;
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    task.Result.Value.Reverse();
                    FeedbackList = new ObservableCollection<FeedbackBindingModel>(task.Result.Value);
                });
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
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
        ~FeedbackPageViewModel()
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