using System;
using System.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using Heyo.Class.Helper;

namespace CMFL.MVVM.ViewModels
{
    public class SystemLoadControlViewModel : PropertyChange, IDisposable
    {
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromSeconds(5)
        };

        private double _memUsage;
        private string _memUsageText;

        public SystemLoadControlViewModel()
        {
            if (IsInDesignMode)
                return;

            _dispatcherTimer.Tick += (sender, args) => { GetMemLoad(); };
            new Thread(() => { _dispatcherTimer.Start(); })
                {IsBackground = true}.Start();
        }

        public double MemUsage
        {
            get => _memUsage;
            set
            {
                _memUsage = value;
                OnPropertyChanged(nameof(MemUsage));
            }
        }

        public string MemUsageText
        {
            get => _memUsageText;
            set
            {
                _memUsageText = value;
                OnPropertyChanged(nameof(MemUsageText));
            }
        }

        private void GetMemLoad()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                MemUsage = (MemoryHelper.GetTotalPhysicalMemory() - MemoryHelper.GetAvailablePhysicalMemory()) /
                           MemoryHelper.GetTotalPhysicalMemory();
                MemUsageText = $"{MemUsage * 100:F}%";
            });
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
        ~SystemLoadControlViewModel()
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