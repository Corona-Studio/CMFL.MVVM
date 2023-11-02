using System;
using CMFL.MVVM.Class.Helper.Launcher;

namespace CMFL.MVVM.ViewModels
{
    public class LoadingPageViewModel : PropertyChange, IDisposable
    {
        private string _tips = $"({LanguageHelper.GetField("Tips")}：{TipsHelper.Show()})";

        public string Tips
        {
            get => _tips;
            set
            {
                _tips = value;
                OnPropertyChanged(nameof(Tips));
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
        ~LoadingPageViewModel()
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