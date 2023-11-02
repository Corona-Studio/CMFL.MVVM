using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.Launcher;
using GalaSoft.MvvmLight.Threading;

namespace CMFL.MVVM.ViewModels
{
    public class PlazaPageViewModel : PropertyChange, IDisposable
    {
        private bool _contentGridVisibility;

        private List<LRImageBoxModel> _imageUriList = new List<LRImageBoxModel>
        {
            new LRImageBoxModel
            {
                ImageUri = new Uri("https://craftmine.fun/wp-content/uploads/2019/12/1.png"),
                Link = "https://craftmine.fun/",
                Title = "点击访问官网"
            },
            new LRImageBoxModel
            {
                ImageUri = new Uri("https://craftmine.fun/wp-content/uploads/2019/12/2.png"),
                Link = "https://jq.qq.com/?_wv=1027&k=53ZREmi",
                Title = "点我加群喵！"
            }
        };

        private bool _loadingGridVisibility = true;

        private string _loadingMessage;

        private ObservableCollection<string> _news = new ObservableCollection<string>();

        private bool _requireLoginGridVisibility;

        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                _loadingMessage = value;
                OnPropertyChanged(nameof(LoadingMessage));
            }
        }

        public bool LoadingGridVisibility
        {
            get => _loadingGridVisibility;
            set
            {
                _loadingGridVisibility = value;
                OnPropertyChanged(nameof(LoadingGridVisibility));
            }
        }

        public bool RequireLoginGridVisibility
        {
            get => _requireLoginGridVisibility;
            set
            {
                _requireLoginGridVisibility = value;
                OnPropertyChanged(nameof(RequireLoginGridVisibility));
            }
        }

        public bool ContentGridVisibility
        {
            get => _contentGridVisibility;
            set
            {
                _contentGridVisibility = value;
                OnPropertyChanged(nameof(ContentGridVisibility));
            }
        }

        public List<LRImageBoxModel> ImageUriList
        {
            get => _imageUriList;
            set
            {
                _imageUriList = value;
                OnPropertyChanged(nameof(ImageUriList));
            }
        }

        public ObservableCollection<string> News
        {
            get => _news;
            set
            {
                _news = value;
                OnPropertyChanged(nameof(News));
            }
        }

        public ICommand GoToMcBbs
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Process.Start("http://www.mcbbs.net/forum.php?mod=viewthread&tid=791406");
                });
            }
        }

        public ICommand GoToBilibili
        {
            get { return new DelegateCommand(obj => { Process.Start("https://www.bilibili.com/video/av36150998"); }); }
        }

        public ICommand GoToMainSite
        {
            get
            {
                return new DelegateCommand(obj => { Process.Start("https://www.craftminefun.com/index.php/cmfl/"); });
            }
        }

        public void LoadContent()
        {
            if (!SettingsHelper.Settings.LoggedInToCMFL || !SettingsHelper.Settings.IsConnectedToCMF)
            {
                ContentGridVisibility = false;
                LoadingGridVisibility = false;
                RequireLoginGridVisibility = true;
                return;
            }

            LoadingGridVisibility = true;
            RequireLoginGridVisibility = false;
            ContentGridVisibility = true;

            GetNewsAsync();
        }

        private void GetNewsAsync()
        {
            Task.Run(async () =>
            {
                using var client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                try
                {
                    var newsStringArray =
                        (await client.DownloadStringTaskAsync(
                                "https://background.craftminefun.com:7707/Others/AnnSys.php?srv=6")
                            .ConfigureAwait(true))
                        .Split('|');
                    foreach (var news in newsStringArray)
                        News.Add(news);
                }
                catch (WebException e)
                {
                    LogHelper.WriteLogLine("获取广场新闻失败", LogHelper.LogLevels.Error);
                    LogHelper.WriteError(e);
                }
                finally
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { LoadingGridVisibility = false; });
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
        ~PlazaPageViewModel()
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