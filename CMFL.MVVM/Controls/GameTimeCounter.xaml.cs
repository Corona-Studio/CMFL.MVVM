using System;
using System.Windows.Controls;

namespace CMFL.MVVM.Controls
{
    /// <summary>
    ///     GameTimeCounter.xaml 的交互逻辑
    /// </summary>
    public partial class GameTimeCounter : UserControl, IDisposable
    {
        public GameTimeCounter()
        {
            InitializeComponent();
        }

        #region IDisposible Support

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        ~GameTimeCounter()
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