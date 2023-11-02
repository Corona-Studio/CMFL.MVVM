using System;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Views;

namespace CMFL.MVVM.ViewModels
{
    public class IntroPage4ViewModel : PropertyChange, IDisposable
    {
        private bool _enableVersionInsulation;

        public bool EnableVersionInsulation
        {
            get => _enableVersionInsulation;
            set
            {
                _enableVersionInsulation = value;
                OnPropertyChanged(nameof(EnableVersionInsulation));
            }
        }


        public ICommand GoToNextCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    SettingsHelper.Settings.VersionInsulation = EnableVersionInsulation;
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("IntroPage5");
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
        ~IntroPage4ViewModel()
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