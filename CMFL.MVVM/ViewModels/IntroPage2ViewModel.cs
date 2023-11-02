using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Game;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.Game.Account;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using ProjBobcat.Authenticator;
using ProjBobcat.Class.Model;
using ProjCrypto.Class.Helper;

namespace CMFL.MVVM.ViewModels
{
    public class IntroPage2ViewModel : PropertyChange, IDisposable
    {
        private string _mcEmail;

        private string _mcPwd;

        private string _offlineUserName;

        public string McEmail
        {
            get => _mcEmail;
            set
            {
                _mcEmail = value;
                OnPropertyChanged(nameof(McEmail));
            }
        }

        public string McPwd
        {
            get => _mcPwd;
            set
            {
                _mcPwd = value;
                OnPropertyChanged(nameof(McPwd));
            }
        }

        public string OfflineUserName
        {
            get => _offlineUserName;
            set
            {
                _offlineUserName = value;
                OnPropertyChanged(nameof(OfflineUserName));
            }
        }

        public ICommand LoginToMojang
        {
            get { return new DelegateCommand(obj => { UseOnline(); }); }
        }

        public ICommand UseOffline
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (string.IsNullOrEmpty(OfflineUserName))
                    {
                        NotifyHelper.ShowNotification(LanguageHelper.GetField("PleaseInputUserName"),
                            LanguageHelper.GetField("PleaseInputUserName"), 3000, ToolTipIcon.Error);
                        return;
                    }

                    var offlineAccount = new OfflineAccount
                    {
                        DisplayName = OfflineUserName,
                        Guid = Guid.NewGuid().ToString("N")
                    };

                    SettingsHelper.Settings.AccountInfos.Add(offlineAccount);
                    offlineAccount.SelectAccount();

                    ViewModelLocator.MainWindowViewModel.WelcomeText =
                        $"{LanguageHelper.GetField("Welcome")}，{OfflineUserName}";
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("IntroPage3");
                });
            }
        }

        private void UseOnline()
        {
            var cancellationToken = new CancellationToken();
            if (string.IsNullOrEmpty(McEmail) || string.IsNullOrEmpty(McPwd))
            {
                NotifyHelper.GetBasicMessageWithBadge(LanguageHelper.GetField("InfoCanNotBeEmpty"),
                    NotifyHelper.MessageType.Warning).Queue();
                NotifyHelper.ShowNotification(LanguageHelper.GetField("InputError"),
                    LanguageHelper.GetField("InfoCanNotBeEmpty"), 3000, ToolTipIcon.Error);
            }
            else
            {
                NotifyHelper.GetBasicMessageWithBadge(LanguageHelper.GetField("StartVerifying"),
                    NotifyHelper.MessageType.Info).Queue();
                LogHelper.WriteLogLine(LanguageHelper.GetField("StartVerifying"), LogHelper.LogLevels.Info);

                Task.Run(async () =>
                {
                    var yggdrasilLogin = new YggdrasilAuthenticator
                    {
                        Email = McEmail,
                        Password = McPwd,
                        LauncherProfileParser = GameHelper.Core.VersionLocator.LauncherProfileParser
                    };
                    var authInfo = await yggdrasilLogin.AuthTaskAsync().ConfigureAwait(true);

                    if (authInfo.AuthStatus == AuthStatus.Succeeded)
                    {
                        LogHelper.WriteLogLine(LanguageHelper.GetFields("Auth|Succeeded"),
                            LogHelper.LogLevels.Info);
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            NotifyHelper.GetBasicMessageWithBadge(
                                LanguageHelper.GetFields("Auth|Succeeded"),
                                NotifyHelper.MessageType.Success).Queue();
                        });
                        NotifyHelper.ShowNotification(
                            LanguageHelper.GetFields("Auth|Succeeded"),
                            LanguageHelper.GetFields("Verify|Succeeded"), 3000,
                            ToolTipIcon.Info);
                        try
                        {
                            var onlineAccount = new OnlineAccount
                            {
                                DisplayName = authInfo.SelectedProfile.Name,
                                Email = StringEncryptHelper.StringEncrypt(McEmail),
                                Password = StringEncryptHelper.StringEncrypt(McPwd),
                                Uuid = new Guid(authInfo.SelectedProfile.Id),
                                LastAuthTime = new DateTime(),
                                LastAuthState = true,
                                Guid = new Guid(authInfo.SelectedProfile.Id).ToString("N")
                            };

                            var offlineAccount = new OfflineAccount
                            {
                                DisplayName = authInfo.SelectedProfile.Name,
                                Guid = Guid.NewGuid().ToString("N")
                            };

                            SettingsHelper.Settings.AccountInfos.Add(onlineAccount);
                            SettingsHelper.Settings.AccountInfos.Add(offlineAccount);
                            onlineAccount.SelectAccount();
                            SettingsHelper.Save();

                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                ViewModelLocator.MainWindowViewModel.WelcomeText =
                                    $"{LanguageHelper.GetField("Welcome")}，{authInfo.SelectedProfile.Name}";
                                ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("IntroPage3");
                            });
                        }
                        catch (ArgumentException ex)
                        {
                            LogHelper.WriteLogLine(LanguageHelper.GetFields("Encrypt|Failed"),
                                LogHelper.LogLevels.Error);
                            LogHelper.WriteError(ex);
                        }
                    }
                    else
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            NotifyHelper.GetBasicMessageWithBadge(
                                LanguageHelper.GetFields("Verify|Failed"),
                                NotifyHelper.MessageType.Error).Queue();
                        });
                        NotifyHelper.ShowNotification(
                            LanguageHelper.GetFields("Verify|Failed"),
                            LanguageHelper.GetFields("PleaseCheckLog"), 3000, ToolTipIcon.Error);
                        LogHelper.WriteLogLine(
                            $"{LanguageHelper.GetFields("Verify|Failed")}：{authInfo.Error.Error}{Environment.NewLine}{authInfo.Error.ErrorMessage}{Environment.NewLine}{authInfo.Error.Cause}",
                            LogHelper.LogLevels.Error);
                    }
                }, cancellationToken);
            }
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
        ~IntroPage2ViewModel()
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