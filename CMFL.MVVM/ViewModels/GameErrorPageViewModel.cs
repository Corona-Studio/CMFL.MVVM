using System;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.Game.Account;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Views;

namespace CMFL.MVVM.ViewModels
{
    public class GameErrorPageViewModel : PropertyChange, IDisposable
    {
        private string _choseGame = $"{LanguageHelper.GetField("LaunchGame")}：{SettingsHelper.Settings.ChoseGame}";

        private string _errorText;

        private string _gameId =
            AccountInfo.GetSelectedAccount().IsOnlineAccount()
                ? $"{LanguageHelper.GetField("GameName")}：{(AccountInfo.GetSelectedAccount() as OnlineAccount)?.Email}"
                : SettingsHelper.Settings.LoggedInToCMFL
                    ? $"{LanguageHelper.GetField("GameName")}：{SettingsHelper.Settings.LUsername}"
                    : $"{LanguageHelper.GetField("GameName")}：{AccountInfo.GetSelectedAccount().DisplayName}";

        private string _javaPath =
            $"{LanguageHelper.GetField("JavaPath")}：{SettingsHelper.Settings.FallBackGameSettings.JavaPath}";

        public string JavaPath
        {
            get => _javaPath;
            set
            {
                _javaPath = value;
                OnPropertyChanged(nameof(JavaPath));
            }
        }

        public string GameId
        {
            get => _gameId;
            set
            {
                _gameId = value;
                OnPropertyChanged(nameof(GameId));
            }
        }

        public string ChoseGame
        {
            get => _choseGame;
            set
            {
                _choseGame = value;
                OnPropertyChanged(nameof(ChoseGame));
            }
        }

        public string ErrorText
        {
            get => _errorText;
            set
            {
                _errorText = value;
                OnPropertyChanged(nameof(ErrorText));
            }
        }


        public ICommand Close
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("SettingPage");
                });
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
        ~GameErrorPageViewModel()
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