using System;
using System.Windows.Forms;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Game;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Views;

namespace CMFL.MVVM.ViewModels
{
    public class IntroPage3ViewModel : PropertyChange, IDisposable
    {
        private int _memSize = 1024;

        public int MemSize
        {
            get => _memSize;
            set
            {
                _memSize = value;
                OnPropertyChanged(nameof(MemSize));
            }
        }

        public ICommand AutoMemCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    var memSize = GameHelper.AutoMemory();
                    if (!string.IsNullOrEmpty(memSize))
                    {
                        MemSize = int.Parse(memSize);
                        SettingsHelper.Settings.FallBackGameSettings.AutoMemorySize = true;
                    }
                    else
                    {
                        NotifyHelper.ShowNotification(
                            LanguageHelper.GetFields("AutoMemory|Failed"),
                            LanguageHelper.GetField("AutoMemoryFailedDetail"), 3000, ToolTipIcon.Error);
                    }
                });
            }
        }


        public ICommand GoToNextCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    SettingsHelper.Settings.FallBackGameSettings.MaxMemory = MemSize;
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("IntroPage4");
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
        ~IntroPage3ViewModel()
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