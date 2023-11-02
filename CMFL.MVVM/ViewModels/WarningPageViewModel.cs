using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.Launcher;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Views;

namespace CMFL.MVVM.ViewModels
{
    public class WarningPageViewModel : PropertyChange, IDisposable
    {
        private ObservableCollection<ProblemList> _warnings = new ObservableCollection<ProblemList>();

        public ObservableCollection<ProblemList> Warnings
        {
            get => _warnings;
            set
            {
                _warnings = value;
                OnPropertyChanged(nameof(Warnings));
            }
        }

        public ICommand DisableAndClose
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    SettingsHelper.Settings.IgnoreWarning = true;
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("HomePage");
                });
            }
        }

        public ICommand Close
        {
            get
            {
                return new DelegateCommand(obj =>
                {
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
        ~WarningPageViewModel()
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