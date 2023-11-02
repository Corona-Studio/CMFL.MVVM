using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Game;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.BMCLAPI;
using CMFL.MVVM.Models.DataModel.Forge;
using CMFL.MVVM.Models.DataModel.Game;
using CMFL.MVVM.Models.DataModel.LiteLoader;
using CMFL.MVVM.Models.DataModel.Optifine;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;

namespace CMFL.MVVM.ViewModels
{
    public class GamePageViewModel : PropertyChange, IDisposable
    {
        private readonly List<OnlineGameInfo> _pureGameSnapshot = new List<OnlineGameInfo>();
        private ObservableCollection<string> _allForgeSupportedVersion;

        private ObservableCollection<string> _allOptifineSupportedVersion;

        private bool _emptyGameListGridVisibility;

        private ObservableCollection<ForgeBuilds> _forgeBuilds = new ObservableCollection<ForgeBuilds>();

        private bool _gameDownloadGridVisibility;

        private ObservableCollection<GameInfo> _gameInfo = new ObservableCollection<GameInfo>();

        private bool _gamePathGridVisibility;

        private ObservableCollection<GamePathModel> _gamePathList =
            new ObservableCollection<GamePathModel>(SettingsHelper.Settings.GamePaths);

        private GamePathModel _gamePathWaitingForAdd = new GamePathModel();

        private Visibility _getForgeAgainButtonVisibility = Visibility.Collapsed;

        private Visibility _getOptifineAgainButtonVisibility = Visibility.Collapsed;

        private Visibility _getLiteLoaderAgainButtonVisibility = Visibility.Collapsed;
        public Visibility GetLiteLoaderAgainButtonVisibility
        {
            get => _getLiteLoaderAgainButtonVisibility;
            set
            {
                _getLiteLoaderAgainButtonVisibility = value;
                OnPropertyChanged(nameof(GetLiteLoaderAgainButtonVisibility));
            }
        }

        private Visibility _getPureGamesAgainButtonVisibility = Visibility.Collapsed;

        private bool _loadingForgePanelVisibility = true;

        private bool _loadingOptifinePanelVisibility = true;

        private bool _loadingPanel = true;

        private bool _loadingPureGamesPanelVisibility = true;

        private ObservableCollection<OnlineGameInfo> _onlineGameInfo =
            new ObservableCollection<OnlineGameInfo>();

        private ObservableCollection<OptifineBuilds> _optiFineBuilds = new ObservableCollection<OptifineBuilds>();

        private bool _pureGameBetaBranch;

        private string _pureGameKeyWord;

        private bool _pureGameReleaseBranch = true;

        private string _refreshButtonText = LanguageHelper.GetField("RefreshGameList");

        private string _selectedForgeVersion;

        private string _selectedOptifineVersion;

        public ObservableCollection<GameInfo> GameInfo
        {
            get => _gameInfo;
            set
            {
                _gameInfo = value;
                OnPropertyChanged(nameof(GameInfo));
            }
        }

        public ObservableCollection<OnlineGameInfo> OnlineGameInfo
        {
            get => _onlineGameInfo;
            set
            {
                _onlineGameInfo = value;
                OnPropertyChanged(nameof(OnlineGameInfo));
            }
        }

        public ObservableCollection<ForgeBuilds> ForgeBuilds
        {
            get => _forgeBuilds;
            set
            {
                _forgeBuilds = value;
                OnPropertyChanged(nameof(ForgeBuilds));
            }
        }

        public ObservableCollection<string> AllOptifineSupportedVersion
        {
            get => _allOptifineSupportedVersion;
            set
            {
                _allOptifineSupportedVersion = value;
                OnPropertyChanged(nameof(AllOptifineSupportedVersion));
            }
        }

        public ObservableCollection<string> AllForgeSupportedVersion
        {
            get => _allForgeSupportedVersion;
            set
            {
                _allForgeSupportedVersion = value;
                OnPropertyChanged(nameof(AllForgeSupportedVersion));
            }
        }

        public ObservableCollection<OptifineBuilds> OptiFineBuilds
        {
            get => _optiFineBuilds;
            set
            {
                _optiFineBuilds = value;
                OnPropertyChanged(nameof(OptiFineBuilds));
            }
        }

        private ObservableCollection<LiteLoaderBuildModel> _liteLoaderBuilds =
            new ObservableCollection<LiteLoaderBuildModel>();
        public ObservableCollection<LiteLoaderBuildModel> LiteLoaderBuilds
        {
            get => _liteLoaderBuilds;
            set
            {
                _liteLoaderBuilds = value;
                OnPropertyChanged(nameof(LiteLoaderBuilds));
            }
        }

        public string SelectedForgeVersion
        {
            get => _selectedForgeVersion;
            set
            {
                _selectedForgeVersion = value;
                OnPropertyChanged(nameof(SelectedForgeVersion));
            }
        }

        public string SelectedOptifineVersion
        {
            get => _selectedOptifineVersion;
            set
            {
                _selectedOptifineVersion = value;
                OnPropertyChanged(nameof(SelectedOptifineVersion));
            }
        }

        public bool GameDownloadGridVisibility
        {
            get => _gameDownloadGridVisibility;
            set
            {
                _gameDownloadGridVisibility = value;
                OnPropertyChanged(nameof(GameDownloadGridVisibility));
            }
        }

        public bool EmptyGameListGridVisibility
        {
            get => _emptyGameListGridVisibility;
            set
            {
                _emptyGameListGridVisibility = value;
                OnPropertyChanged(nameof(EmptyGameListGridVisibility));
            }
        }

        public string RefreshButtonText
        {
            get => _refreshButtonText;
            set
            {
                _refreshButtonText = value;
                OnPropertyChanged(nameof(RefreshButtonText));
            }
        }

        public bool LoadingPanel
        {
            get => _loadingPanel;
            set
            {
                _loadingPanel = value;
                OnPropertyChanged(nameof(LoadingPanel));
            }
        }

        public bool LoadingPureGamesPanelVisibility
        {
            get => _loadingPureGamesPanelVisibility;
            set
            {
                _loadingPureGamesPanelVisibility = value;
                OnPropertyChanged(nameof(LoadingPureGamesPanelVisibility));
            }
        }

        public bool LoadingForgePanelVisibility
        {
            get => _loadingForgePanelVisibility;
            set
            {
                _loadingForgePanelVisibility = value;
                OnPropertyChanged(nameof(LoadingForgePanelVisibility));
            }
        }

        public bool LoadingOptifinePanelVisibility
        {
            get => _loadingOptifinePanelVisibility;
            set
            {
                _loadingOptifinePanelVisibility = value;
                OnPropertyChanged(nameof(LoadingOptifinePanelVisibility));
            }
        }

        private bool _loadingLiteLoaderPanelVisibility;
        public bool LoadingLiteLoaderPanelVisibility
        {
            get => _loadingLiteLoaderPanelVisibility;
            set
            {
                _loadingLiteLoaderPanelVisibility = value;
                OnPropertyChanged(nameof(LoadingLiteLoaderPanelVisibility));
            }
        }

        public Visibility GetPureGamesAgainButtonVisibility
        {
            get => _getPureGamesAgainButtonVisibility;
            set
            {
                _getPureGamesAgainButtonVisibility = value;
                OnPropertyChanged(nameof(GetPureGamesAgainButtonVisibility));
            }
        }

        public Visibility GetForgeAgainButtonVisibility
        {
            get => _getForgeAgainButtonVisibility;
            set
            {
                _getForgeAgainButtonVisibility = value;
                OnPropertyChanged(nameof(GetForgeAgainButtonVisibility));
            }
        }

        public Visibility GetOptifineAgainButtonVisibility
        {
            get => _getOptifineAgainButtonVisibility;
            set
            {
                _getOptifineAgainButtonVisibility = value;
                OnPropertyChanged(nameof(GetOptifineAgainButtonVisibility));
            }
        }

        public bool PureGameReleaseBranch
        {
            get => _pureGameReleaseBranch;
            set
            {
                _pureGameReleaseBranch = value;
                OnPropertyChanged(nameof(PureGameReleaseBranch));
            }
        }

        public bool PureGameBetaBranch
        {
            get => _pureGameBetaBranch;
            set
            {
                _pureGameBetaBranch = value;
                OnPropertyChanged(nameof(PureGameBetaBranch));
            }
        }

        public string PureGameKeyWord
        {
            get => _pureGameKeyWord;
            set
            {
                _pureGameKeyWord = value;
                OnPropertyChanged(nameof(PureGameKeyWord));
            }
        }

        public ObservableCollection<GamePathModel> GamePathList
        {
            get => _gamePathList;
            set
            {
                _gamePathList = value;
                SettingsHelper.Settings.GamePaths = value.ToList();
                OnPropertyChanged(nameof(GamePathList));
            }
        }

        public bool GamePathGridVisibility
        {
            get => _gamePathGridVisibility;
            set
            {
                _gamePathGridVisibility = value;
                OnPropertyChanged(nameof(GamePathGridVisibility));
            }
        }

        public GamePathModel GamePathWaitingForAdd
        {
            get => _gamePathWaitingForAdd;
            set
            {
                _gamePathWaitingForAdd = value;
                OnPropertyChanged(nameof(GamePathWaitingForAdd));
            }
        }

        public Dictionary<string, string> GameIcons { get; } = GameIconHelper.GetAllIcons();

        public ICommand AddGamePathCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (string.IsNullOrEmpty(GamePathWaitingForAdd.Path)) return;

                    if (!GamePathWaitingForAdd.Path.EndsWith("\\", StringComparison.Ordinal))
                        GamePathWaitingForAdd.Path += "\\";

                    SettingsHelper.Settings.GamePaths.Add(GamePathWaitingForAdd);
                    GamePathList.Add(GamePathWaitingForAdd);
                    DialogHost.CloseDialogCommand.Execute(null, null);
                });
            }
        }

        public ICommand RefreshGameListCommand
        {
            get { return new DelegateCommand(obj => { GetAllLocalGames(); }); }
        }

        public ICommand OpenGameDownloadGridCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    GameDownloadGridVisibility = true;
                    GetAllVersionsOnline();
                });
            }
        }

        public ICommand OpenGamePathGridCommand
        {
            get { return new DelegateCommand(obj => { GamePathGridVisibility = true; }); }
        }

        public ICommand CloseGameDownloadGridCommand
        {
            get { return new DelegateCommand(obj => { GameDownloadGridVisibility = false; }); }
        }

        public ICommand CloseGamePathGridCommand
        {
            get { return new DelegateCommand(obj => { GamePathGridVisibility = false; }); }
        }

        public ICommand GetForgeBuildsCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    try
                    {
                        LogHelper.WriteLogLine($"{LanguageHelper.GetField("StartGetting")} Forge",
                            LogHelper.LogLevels.Info);
                        GetForgeList();
                    }
                    catch (WebException ex)
                    {
                        LogHelper.WriteLogLine($"Forge {LanguageHelper.GetField("GetFailed")}",
                            LogHelper.LogLevels.Error);
                        LogHelper.WriteError(ex);
                    }
                });
            }
        }

        public ICommand GetOptifineBuildsCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    OptiFineBuilds.Clear();
                    try
                    {
                        LogHelper.WriteLogLine($"{LanguageHelper.GetField("StartGetting")} Optifine",
                            LogHelper.LogLevels.Info);
                        GetOptifineList();
                    }
                    catch (WebException ex)
                    {
                        LogHelper.WriteLogLine($"Optifine {LanguageHelper.GetField("GetFailed")}",
                            LogHelper.LogLevels.Error);
                        LogHelper.WriteError(ex);
                    }
                });
            }
        }

        public ICommand GetLiteLoaderBuildsCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    GetLiteLoaderList();
                });
            }
        }

        public ICommand CloseDragGridCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("GamePage");
                });
            }
        }

        public ICommand OpenEulaPage
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Process.Start("https://account.mojang.com/documents/minecraft_eula");
                });
            }
        }

        public ICommand TryGetGamesAgainCommand
        {
            get { return new DelegateCommand(obj => { GetAllVersionsOnline(); }); }
        }

        public ICommand AdvancePureGameSearchCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (PureGameReleaseBranch)
                    {
                        if (string.IsNullOrEmpty(PureGameKeyWord))
                        {
                            var list = _pureGameSnapshot.Where(game => game.Type.Contains("release")).ToList();
                            OnlineGameInfo = new ObservableCollection<OnlineGameInfo>(list);
                        }
                        else
                        {
                            var list = _pureGameSnapshot.Where(game =>
                                game.Type.Contains("release") && game.Id.Contains(PureGameKeyWord)).ToList();
                            OnlineGameInfo = new ObservableCollection<OnlineGameInfo>(list);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(PureGameKeyWord))
                        {
                            var list = _pureGameSnapshot.Where(game => !game.Type.Contains("release")).ToList();
                            OnlineGameInfo = new ObservableCollection<OnlineGameInfo>(list);
                        }
                        else
                        {
                            var list = _pureGameSnapshot.Where(game =>
                                !game.Type.Contains("release") && game.Id.Contains(PureGameKeyWord)).ToList();
                            OnlineGameInfo = new ObservableCollection<OnlineGameInfo>(list);
                        }
                    }
                });
            }
        }

        private void GetForgeList()
        {
            Task.Run(async () =>
            {
                using var client = new WebClient {Encoding = Encoding.UTF8};
                try
                {
                    var serverReply = await client.DownloadStringTaskAsync(new Uri(
                            $"{SettingsHelper.BmclapiDownloadServer}forge/minecraft/{SelectedForgeVersion}"))
                        .ConfigureAwait(true);
                    if (serverReply == null || !serverReply.Any())
                    {
                        return;
                    }

                    var forgeBuilds = JsonConvert.DeserializeObject<List<ForgeBuilds>>(serverReply);
                    forgeBuilds.Reverse();

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        ForgeBuilds = new ObservableCollection<ForgeBuilds>(forgeBuilds);
                    });
                }
                catch (WebException e)
                {
                    NotifyHelper.ShowNotification($"Forge {LanguageHelper.GetField("GetFailed")}",
                        LanguageHelper.GetField("TryAgainLater"), 3000, ToolTipIcon.Error);
                    LogHelper.WriteLogLine($"Forge {LanguageHelper.GetField("GetFailed")}", LogHelper.LogLevels.Error);
                    LogHelper.WriteError(e);
                }
            });
        }

        private void GetLiteLoaderList()
        {
            BMCLHelper.GetAllLiteLoaderBuilds().ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            GetLiteLoaderAgainButtonVisibility = Visibility.Visible;
                        });

                        NotifyHelper.ShowNotification($"LiteLoader {LanguageHelper.GetField("GetFailed")}",
                            LanguageHelper.GetField("PleaseCheckLog"), 3000, ToolTipIcon.Error);
                        LogHelper.WriteLogLine($"LiteLoader {LanguageHelper.GetField("GetFailed")}", LogHelper.LogLevels.Error);
                        LogHelper.WriteError(task.Exception);
                        return;
                    }

                    foreach(var lL in task.Result.Value)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            LiteLoaderBuilds.Add(new LiteLoaderBuildModel { VersionMeta = lL });
                        });
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        LoadingLiteLoaderPanelVisibility = false;
                    });
                }, CancellationToken.None,
                TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        private void GetOptifineList()
        {
            Task.Run(async () =>
            {
                using var client = new WebClient {Encoding = Encoding.UTF8};
                var serverReply = await client.DownloadStringTaskAsync(new Uri(
                        $"{SettingsHelper.BmclapiDownloadServer}optifine/{SelectedOptifineVersion}"))
                    .ConfigureAwait(true);
                var optiFineBuilds = JsonConvert.DeserializeObject<OptifineBuilds[]>(serverReply);

                if (!optiFineBuilds.Any())
                {
                    return;
                }

                foreach (var item in optiFineBuilds)
                {
                    var suggestion = LanguageHelper.GetField("StableBranch");

                    if (item.Patch.Contains("pre") || item.Patch.Contains("beta") || item.Patch.Contains("alpha"))
                        suggestion = LanguageHelper.GetField("BetaBranch");

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        OptiFineBuilds.Add(new OptifineBuilds
                        {
                            IdLocker = item.IdLocker,
                            McVersion = item.McVersion,
                            Type = item.Type,
                            VersionLocker = item.VersionLocker,
                            Filename = item.Filename.Substring(0, item.Filename.Length - 4),
                            Patch = item.Patch,
                            Tag = string.Join("|", item.McVersion, item.Type, item.Patch),
                            Suggestion = suggestion
                        });
                    });
                }
            });
        }

        /// <summary>
        ///     获取所有本地游戏
        /// </summary>
        public void GetAllLocalGames()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                RefreshButtonText = LanguageHelper.GetField("PleaseWait");
                LoadingPanel = true;
                GameInfo.Clear();
            });


            GameHelper.SearchGames(false).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        NotifyHelper
                            .GetBasicMessageWithBadge(LanguageHelper.GetFields("RefreshGameList|Failed"),
                                NotifyHelper.MessageType.Error).Queue();
                        RefreshButtonText = LanguageHelper.GetField("RefreshGameList");
                        LoadingPanel = false;
                    });

                    LogHelper.WriteLogLine(LanguageHelper.GetFields("GameList|GetFailed"),
                        LogHelper.LogLevels.Error);
                    LogHelper.WriteError(task.Exception);
                    return;
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    EmptyGameListGridVisibility = !task.Result.Value.Any();
                    GameInfo = new ObservableCollection<GameInfo>(task.Result.Value);
                    RefreshButtonText = LanguageHelper.GetField("RefreshGameList");
                    LoadingPanel = false;
                });
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        /// <summary>
        ///     从BMCLAPI获取所有MC版本
        /// </summary>
        private void GetAllVersionsOnline()
        {
            LoadingPureGamesPanelVisibility = true;
            LoadingForgePanelVisibility = true;
            LoadingOptifinePanelVisibility = true;
            LoadingLiteLoaderPanelVisibility = true;

            AllForgeSupportedVersion?.Clear();
            AllOptifineSupportedVersion?.Clear();
            LiteLoaderBuilds?.Clear();
            OnlineGameInfo?.Clear();

            BMCLHelper.GetAllMcVersions().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        GetPureGamesAgainButtonVisibility = Visibility.Visible;
                    });

                    LogHelper.WriteLogLine(LanguageHelper.GetField("GetFailed"), LogHelper.LogLevels.Error);
                    LogHelper.WriteError(task.Exception);
                    return;
                }

                var localGames = GameHelper.GetListGames();

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    OnlineGameInfo.Clear();
                    _onlineGameInfo.Clear();
                });

                foreach (var item in task.Result.Value.Versions)
                {
                    var cnType = string.Empty;
                    var suggestion = string.Empty;
                    var isDownloadButtonEnable = true;
                    switch (item.Type)
                    {
                        case "snapshot":
                            cnType = LanguageHelper.GetField("Snapshot");
                            suggestion = LanguageHelper.GetField("SnapshotSuggestion");
                            break;
                        case "release":
                            cnType = LanguageHelper.GetField("Release");
                            suggestion = LanguageHelper.GetField("ReleaseSuggestion");
                            break;
                        case "old_beta":
                            cnType = LanguageHelper.GetField("OldBeta");
                            suggestion = LanguageHelper.GetField("OldBetaSuggestion");
                            break;
                        case "old_alpha":
                            cnType = LanguageHelper.GetField("OldAlpha");
                            suggestion = LanguageHelper.GetField("OldAlphaSuggestion");
                            break;
                    }

                    if (localGames.Any(localGame => localGame.Id == item.Id))
                        isDownloadButtonEnable = false;

                    var onlineGameInfo = new OnlineGameInfo
                    {
                        Id = item.Id,
                        Type = item.Type,
                        Time = item.Time.ToLocalTime().ToString(),
                        CnType = cnType,
                        Suggestion = suggestion,
                        IsDownloadButtonEnable = isDownloadButtonEnable
                    };

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        OnlineGameInfo.Add(onlineGameInfo);
                        _pureGameSnapshot.Add(onlineGameInfo);
                    });
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    LoadingPureGamesPanelVisibility = false;
                    GetPureGamesAgainButtonVisibility = Visibility.Collapsed;
                });
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

            BMCLHelper.GetAllSupportedForgeVersion().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        GetForgeAgainButtonVisibility = Visibility.Visible;
                    });

                    NotifyHelper.ShowNotification($"Forge {LanguageHelper.GetField("GetFailed")}",
                        LanguageHelper.GetField("PleaseCheckLog"), 3000, ToolTipIcon.Error);
                    LogHelper.WriteLogLine($"Forge {LanguageHelper.GetField("GetFailed")}", LogHelper.LogLevels.Error);
                    LogHelper.WriteError(task.Exception);
                    return;
                }

                if (task.Result.Value == null || !task.Result.Value.Any())
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        GetForgeAgainButtonVisibility = Visibility.Visible;
                    });
                else
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        AllForgeSupportedVersion = new ObservableCollection<string>(task.Result.Value);
                        LoadingForgePanelVisibility = false;
                        GetForgeAgainButtonVisibility = Visibility.Collapsed;
                    });
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

            BMCLHelper.GetAllOptiFineSupportVersions().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        GetOptifineAgainButtonVisibility = Visibility.Visible;
                    });

                    NotifyHelper.ShowNotification($"Optifine {LanguageHelper.GetField("GetFailed")}",
                        LanguageHelper.GetField("PleaseCheckLog"), 3000, ToolTipIcon.Error);
                    LogHelper.WriteLogLine($"Optifine {LanguageHelper.GetField("GetFailed")}",
                        LogHelper.LogLevels.Error);
                    LogHelper.WriteError(task.Exception);
                    return;
                }

                if (task.Result.Value == null || !task.Result.Value.Any())
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        GetOptifineAgainButtonVisibility = Visibility.Visible;
                    });
                else
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        AllOptifineSupportedVersion = new ObservableCollection<string>(task.Result.Value);
                        LoadingOptifinePanelVisibility = false;
                        GetOptifineAgainButtonVisibility = Visibility.Collapsed;
                    });
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

            GetLiteLoaderList();
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
        ~GamePageViewModel()
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