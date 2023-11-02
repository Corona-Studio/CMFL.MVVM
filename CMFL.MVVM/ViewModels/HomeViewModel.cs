using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.Launcher;
using CMFL.MVVM.Models.DataModel.News;
using GalaSoft.MvvmLight.Threading;
using ProjBobcat.Class.Model;

namespace CMFL.MVVM.ViewModels
{
    public class HomeViewModel : PropertyChange, IDisposable
    {
        private BgmControl _bgmControl = new BgmControl();

        private bool _enableAnimation = SettingsHelper.Settings.EnableAnimation;

        private bool _gridExceptionNewsVisibility;

        private ObservableCollection<McbbsNews> _mcbbsNews = new ObservableCollection<McbbsNews>();

        private bool _newsProcessRingVisibility = true;
        public bool Monitoring { get; set; }

        public BgmControl BgmControl
        {
            get => _bgmControl;
            set
            {
                _bgmControl = value;
                OnPropertyChanged(nameof(BgmControl));
            }
        }

        public ObservableCollection<McbbsNews> McbbsNews
        {
            get => _mcbbsNews;
            set
            {
                _mcbbsNews = value;
                OnPropertyChanged(nameof(McbbsNews));
            }
        }

        public bool NewsProcessRingVisibility
        {
            get => _newsProcessRingVisibility;
            set
            {
                _newsProcessRingVisibility = value;
                OnPropertyChanged(nameof(NewsProcessRingVisibility));
            }
        }

        public bool GridExceptionNewsVisibility
        {
            get => _gridExceptionNewsVisibility;
            set
            {
                _gridExceptionNewsVisibility = value;
                OnPropertyChanged(nameof(GridExceptionNewsVisibility));
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

        public ICommand ResetMusicCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    SettingsHelper.Settings.UseLocalMusic = false;
                    SettingsHelper.Settings.MusicFilePath = string.Empty;
                    SettingsHelper.Save();
                    ViewModelLocator.MainWindowViewModel.GetMusic();
                    NotifyHelper.GetBasicMessageWithBadge(LanguageHelper.GetField("ResetSucceeded"),
                        NotifyHelper.MessageType.Success).Queue();
                });
            }
        }

        public ICommand RetryGetNewsCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    GridExceptionNewsVisibility = false;
                    NewsProcessRingVisibility = true;
                    GetMcbbsNews();
                });
            }
        }

        public ICommand ChangeBgmCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    var musicFileDialog = new OpenFileDialog
                    {
                        InitialDirectory = "C:\\",
                        Filter = $"{LanguageHelper.GetField("AudioFile")}|*.wav;*.mp3;*.ogg;*.flac;*.asf;*.wma;*.wav",
                        RestoreDirectory = true,
                        FilterIndex = 1
                    };

                    if (musicFileDialog.ShowDialog() != DialogResult.OK) return;

                    BgmControl.BgmName =
                        musicFileDialog.FileName.Split('\\').Last().Split('.').First();
                    BgmControl.SetMusicUri(musicFileDialog.FileName);
                    SettingsHelper.Settings.UseLocalMusic = true;
                    SettingsHelper.Settings.MusicFilePath = musicFileDialog.FileName;
                });
            }
        }

        public void GetMcbbsNews()
        {
            LauncherHelper.GetMcBbsNews().ContinueWith(task =>
            {
                if (task.Exception != null || task.Result.TaskStatus != TaskResultStatus.Success)
                {
                    LogHelper.WriteLogLine(LanguageHelper.GetFields("McbbsNews|GetFailed", " "),
                        LogHelper.LogLevels.Error);
                    LogHelper.WriteError(task.Exception);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        NewsProcessRingVisibility = false;
                        GridExceptionNewsVisibility = true;
                    });
                    return;
                }

                if (!task.Result.Value.Any())
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        NewsProcessRingVisibility = false;
                        GridExceptionNewsVisibility = true;
                    });
                    return;
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    McbbsNews = new ObservableCollection<McbbsNews>(task.Result.Value);
                });

                LogHelper.WriteLogLine(
                    LanguageHelper.GetFields("McbbsNews|GetSuccessful", " "),
                    LogHelper.LogLevels.Info);
                DispatcherHelper.CheckBeginInvokeOnUI(() => { NewsProcessRingVisibility = false; });
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
        ~HomeViewModel()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                // free managed resources
                McbbsNews = null;
        }

        #endregion
    }
}