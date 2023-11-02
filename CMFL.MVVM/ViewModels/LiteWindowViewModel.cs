using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using CMFL.MVVM.Class.Helper.Game;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.Game;
using CMFL.MVVM.Models.DataModel.Game.Account;
using Heyo.Class.Helper;

namespace CMFL.MVVM.ViewModels
{
    public class LiteWindowViewModel : PropertyChange, IDisposable
    {
        private string _avaiMem =
            $"（{LanguageHelper.GetField("AvailableMemory")}：{MemoryHelper.GetAvailablePhysicalMemory():F} MB）";

        private bool _enableVersionInsulation = SettingsHelper.Settings.VersionInsulation;

        private ObservableCollection<GameInfo> _gameInfo = new ObservableCollection<GameInfo>();

        private string _memorySize = SettingsHelper.Settings.FallBackGameSettings.MaxMemory.ToString();

        private string _userName = AccountInfo.GetSelectedAccount().DisplayName;

        private WindowState _windowState = WindowState.Normal;

        public LiteWindowViewModel()
        {
            if (IsInDesignMode)
                return;

            GetLocalGames();
        }

        private Dispatcher Dispatcher { get; }

        public ObservableCollection<GameInfo> GameInfo
        {
            get => _gameInfo;
            set
            {
                _gameInfo = value;
                OnPropertyChanged(nameof(GameInfo));
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        private WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                OnPropertyChanged(nameof(WindowState));
            }
        }

        public bool EnableVersionInsulation
        {
            get => _enableVersionInsulation;
            set
            {
                _enableVersionInsulation = value;
                OnPropertyChanged(nameof(EnableVersionInsulation));
            }
        }

        public string MemorySize
        {
            get => _memorySize;
            set
            {
                _memorySize = value;
                SettingsHelper.Settings.FallBackGameSettings.MaxMemory = int.Parse(value);
                OnPropertyChanged(nameof(MemorySize));
            }
        }

        public string AvaiMem
        {
            get => _avaiMem;
            set
            {
                _avaiMem = value;
                OnPropertyChanged(AvaiMem);
            }
        }

        public ICommand CloseAppCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    SettingsHelper.Save();
                    ViewModelLocator.CleanUp();
                    Environment.Exit(0);
                });
            }
        }

        public ICommand DisableLiteModeCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    SettingsHelper.Settings.LiteMode = false;
                    NotifyHelper.ShowNotification(LanguageHelper.GetField("ExitLiteModeSucceeded"),
                        LanguageHelper.GetField("PleaseRebootLauncher"), 3000, ToolTipIcon.Info);
                });
            }
        }

        public ICommand StartGameCommand
        {
            get { return new DelegateCommand(obj => { GameStartAsync(); }); }
        }

        /// <summary>
        ///     启动游戏
        /// </summary>
        /// <returns></returns>
        private void GameStartAsync()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var game = GameHelper.Core.VersionLocator.GetGame(SettingsHelper.Settings.ChoseGame);
            if (game == null)
            {
                NotifyHelper.ShowNotification(LanguageHelper.GetField("PleaseSelectGameFirst"), "！！！！！", 3000,
                    ToolTipIcon.Error);
                return;
            }

            GameHelper.StartGame(SettingsHelper.Settings.ChoseGame, Dispatcher).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    WindowState = WindowState.Normal;
                    NotifyHelper.GetBasicMessageWithBadge(
                        LanguageHelper.GetFields("LaunchGame|Failed"),
                        NotifyHelper.MessageType.Error).Queue();
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
                    stopwatch.Stop();
                    SettingsHelper.Settings.IgnoreWarning = false;
                    WindowState = WindowState.Minimized;
                    LogHelper.WriteLogLine(
                        LanguageHelper.GetFields("LaunchGame|Succeeded"),
                        LogHelper.LogLevels.Info);
                    NotifyHelper.ShowNotification(LanguageHelper.GetField("GameIsRunning"),
                        $"{LanguageHelper.GetFields("LaunchGame|Succeeded")}，{stopwatch.Elapsed.TotalMilliseconds}ms",
                        3000, ToolTipIcon.Info);
                    stopwatch.Reset();
                }
                else
                {
                    WindowState = WindowState.Normal;
                    NotifyHelper.GetBasicMessageWithBadge(
                        LanguageHelper.GetFields("LaunchGame|Failed"),
                        NotifyHelper.MessageType.Error).Queue();
                    NotifyHelper.ShowNotification(
                        LanguageHelper.GetFields("LaunchGame|Failed"),
                        LanguageHelper.GetField("PleaseCheckLog"), 3000, ToolTipIcon.Error);
                    LogHelper.WriteLogLine(
                        LanguageHelper.GetFields("LaunchGame|Failed"),
                        LogHelper.LogLevels.Error);
                }
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        private void GetLocalGames()
        {
            GameHelper.SearchGames(true).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    LogHelper.WriteLogLine(
                        LanguageHelper.GetFields("GameList|GetFailed"),
                        LogHelper.LogLevels.Error);
                    LogHelper.WriteError(task.Exception);
                    return;
                }

                GameInfo = new ObservableCollection<GameInfo>(task.Result.Value);
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
        ~LiteWindowViewModel()
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