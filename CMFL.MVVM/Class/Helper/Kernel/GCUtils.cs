using System;
using System.Windows.Threading;
using Heyo.Class.Helper;

namespace CMFL.MVVM.Class.Helper.Kernel
{
    public class GcUtils
    {
        private static int _count;

        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Background);

        public GcUtils(double interval = 20)
        {
#if DEBUG
            return;
#endif
            _timer.Interval = TimeSpan.FromSeconds(interval);
            _timer.Tick += Collect;
        }

        private static void Collect(object sender, EventArgs e)
        {
#if DEBUG
            return;
#endif
            GC.Collect();

            if (_count == 5)
            {
                MemoryHelper.CleanAllMemory();
                _count = 0;
                return;
            }

            _count++;
        }

        public void Start()
        {
#if DEBUG
            return;
#endif
            new System.Threading.Thread(() =>
                {
                    Collect(null, null);
                    _timer.Start();
                })
                {IsBackground = true}.Start();
        }
    }
}