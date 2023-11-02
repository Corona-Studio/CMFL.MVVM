using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.Launcher;
using GalaSoft.MvvmLight.Threading;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;
using ProjCrypto;

namespace CMFL.MVVM.ViewModels
{
    public class UpgradePageViewModel : PropertyChange, IDisposable
    {
        // private ObservableCollection<string> _confirmContent = new ObservableCollection<string>();

        private bool _createLinkOnDesktop = true;

        private string _downloadText = LanguageHelper.GetField("DownloadNow");

        /*
        public ObservableCollection<string> ConfirmContent
        {
            get => _confirmContent;
            set
            {
                _confirmContent = value;
                OnPropertyChanged(nameof(ConfirmContent));
            }
        }
        */

        private bool _isDownloading;

        // private bool _isConfirmDialogOpen;

        // private bool _isConfirmed;
        private UpdateInfoModel _updateInfoModel;

        private ObservableCollection<string> _upgradeContent;

        public UpdateInfoModel UpdateInfoModel
        {
            get => _updateInfoModel;
            set
            {
                _updateInfoModel = value;
                OnPropertyChanged(nameof(UpdateInfoModel));
            }
        }

        public ObservableCollection<string> UpgradeContent
        {
            get => _upgradeContent;
            set
            {
                _upgradeContent = value;
                OnPropertyChanged(nameof(UpgradeContent));
            }
        }

        public string DownloadText
        {
            get => _downloadText;
            set
            {
                _downloadText = value;
                OnPropertyChanged(nameof(DownloadText));
            }
        }

        /*
        public bool IsConfirmDialogOpen
        {
            get => _isConfirmDialogOpen;
            set
            {
                _isConfirmDialogOpen = value;
                OnPropertyChanged(nameof(IsConfirmDialogOpen));
            }
        }
        */

        public bool CreateLinkOnDesktop
        {
            get => _createLinkOnDesktop;
            set
            {
                _createLinkOnDesktop = value;
                OnPropertyChanged(nameof(CreateLinkOnDesktop));
            }
        }

        public ICommand DownloadUpgrade
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    if (_isDownloading) return;

                    _isDownloading = true;
                    var dF = new DownloadFile
                    {
                        Changed = (sender, args) =>
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                DownloadText = args.ProgressPercentage.ToString("P1");
                            });
                        },
                        Completed = (sender, args) =>
                        {
                            _isDownloading = false;
                            var bytes = File.ReadAllBytes($"{Environment.CurrentDirectory}\\{UpdateInfoModel.Name}");
                            var sha256Bytes = Cryptosystem.Sha256(bytes);
                            var sha256 = BitConverter.ToString(sha256Bytes).Replace("-", string.Empty);
                            if (!sha256.Equals(UpdateInfoModel.Sha, StringComparison.OrdinalIgnoreCase))
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    NotifyHelper.GetBasicMessageWithBadge(LanguageHelper.GetFields("Update|Failed"),
                                        NotifyHelper.MessageType.Error).Queue();
                                    DownloadText = LanguageHelper.GetField("DownloadNow");
                                });
                                return;
                            }

                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                DownloadText = LanguageHelper.GetFields("Download|Succeeded");
                            });

                            SettingsHelper.Save();

                            try
                            {
                                if (CreateLinkOnDesktop)
                                    LauncherHelper.CreateShortcutOnDesktop(
                                        $"{Environment.CurrentDirectory}\\CMFL.MVVM.exe");

                                DirectoryHelper.CleanDirectory($"{Environment.CurrentDirectory}\\CMFL\\Temp\\",
                                    false);
                            }
                            catch (UnauthorizedAccessException)
                            {
                            }
                            catch (IOException)
                            {
                            }

                            LauncherHelper.BeginKillSelf($"{Environment.CurrentDirectory}\\{UpdateInfoModel.Name}",
                                "CMFL.MVVM.exe");

                            Environment.Exit(0);
                        },
                        DownloadPath = $"{Environment.CurrentDirectory}\\{UpdateInfoModel.Name}",
                        DownloadUri = $"{SettingsHelper.Settings.LauncherWebServer}/Update/{UpdateInfoModel.Name}",
                        FileName = UpdateInfoModel.Name
                    };

                    await DownloadHelper.MultiPartDownloadTaskAsync(dF, 16).ConfigureAwait(false);
                });
            }
        }

        public ICommand ConfirmCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    /*
                    IsConfirmDialogOpen = false;
                    _isConfirmed = true;
                    */
                });
            }
        }

        public void GetUpdateInfo()
        {
            UpgradeContent = new ObservableCollection<string>(UpdateInfoModel.Description.Split('|').ToList());
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
        ~UpgradePageViewModel()
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