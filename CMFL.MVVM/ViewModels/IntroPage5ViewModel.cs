using System;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Views;

namespace CMFL.MVVM.ViewModels
{
    public class IntroPage5ViewModel : IDisposable
    {
        public ICommand GoToHomeCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    SettingsHelper.Settings.FirstTime = false;
                    ViewModelLocator.MainWindowViewModel.LeftMessageBorderVisibility = false;
                    SettingsHelper.Save();
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("HomePage");
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
        ~IntroPage5ViewModel()
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