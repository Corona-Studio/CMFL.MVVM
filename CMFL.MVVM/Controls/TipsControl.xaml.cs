using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CMFL.MVVM.Class.Helper.Launcher;

namespace CMFL.MVVM.Controls
{
    /// <summary>
    ///     TipsControl.xaml 的交互逻辑
    /// </summary>
    public partial class TipsControl : UserControl, IDisposable
    {
        private readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromSeconds(2.5)
        };

        public TipsControl()
        {
            InitializeComponent();
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            var tips = TipsHelper.Show();
            var times = 1;

            timer.Tick += (s, ee) =>
            {
                if (times == 2)
                {
                    tips = TipsHelper.Show();
                    Tips.Text = tips;
                    times = 1;
                }
                else
                {
                    Tips.Text = tips;
                    times++;
                }
            };

            new Thread(() => { timer.Start(); }) {IsBackground = true}.Start();
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
        ~TipsControl()
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