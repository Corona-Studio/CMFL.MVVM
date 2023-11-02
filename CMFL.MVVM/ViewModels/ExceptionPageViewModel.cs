using System;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.SocketServer;
using Microsoft.VisualBasic.Devices;

namespace CMFL.MVVM.ViewModels
{
    public class ExceptionPageViewModel : PropertyChange, IDisposable
    {
        private string _errorMessage = string.Empty;

        private string _sysVersion =
            $"{LanguageHelper.GetField("SystemVersion")}{new ComputerInfo().OSFullName}{new ComputerInfo().OSVersion}";

        public string SysVersion
        {
            get => _sysVersion;
            set
            {
                _sysVersion = value;
                OnPropertyChanged(nameof(SysVersion));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand CloseLauncherCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    LauncherSocketServerHelper.CloseSocket();
                    ViewModelLocator.CleanUp();
                    Environment.Exit(0);
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
        ~ExceptionPageViewModel()
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