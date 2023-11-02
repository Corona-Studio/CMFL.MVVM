using System;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;

namespace CMFL.MVVM.ViewModels
{
    public class AboutPageViewModel : PropertyChange, IDisposable
    {
        private string _bgPath = SettingsHelper.Settings.BgPath;

        private string _copyright =
            $" CraftMineFun 2016 - {DateTime.Now.Year} ，{LanguageHelper.GetField("AllRightReserved")}。";

        private string _foundTime = string.Format(LanguageHelper.GetField("FoundationTimeText"),
            LauncherHelper.GetFoundationTime());

        public string FoundTime
        {
            get => _foundTime;
            set
            {
                _foundTime = value;
                OnPropertyChanged(nameof(FoundTime));
            }
        }

        public string CopyRight
        {
            get => _copyright;
            set
            {
                _copyright = value;
                OnPropertyChanged(nameof(CopyRight));
            }
        }

        public string BgPath
        {
            get => _bgPath;
            set
            {
                _bgPath = value;
                OnPropertyChanged(nameof(BgPath));
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
        ~AboutPageViewModel()
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