using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using GalaSoft.MvvmLight.Threading;
using LiveCharts;
using LiveCharts.Wpf;

namespace CMFL.MVVM.ViewModels
{
    public class GameTimeCounterViewModel : PropertyChange, IDisposable
    {
        private Func<double, string> _formatter = value =>
            value.ToString("F1");

        private List<string> _labels = new List<string>();

        private SeriesCollection _seriesCollection = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = LanguageHelper.GetField("Duration"),
                Values = new ChartValues<long>()
            }
        };

        private double _totalGameTime;

        public double TotalGameTime
        {
            get => _totalGameTime;
            set
            {
                _totalGameTime = value;
                OnPropertyChanged(nameof(TotalGameTime));
            }
        }

        public List<string> Labels
        {
            get => _labels;
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }


        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        public Func<double, string> Formatter
        {
            get => _formatter;
            set
            {
                _formatter = value;
                OnPropertyChanged(nameof(Formatter));
            }
        }

        public ICommand Refresh
        {
            get { return new DelegateCommand(obj => { GetGameTime(); }); }
        }

        public void GetGameTime()
        {
            long temp = 0;
            Labels.Clear();
            SeriesCollection.First().Values.Clear();

            Task.Run(() =>
            {
                foreach (var gameTime in SettingsHelper.Settings.GameTimes)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Labels.Add(gameTime.Key.Length > 10 ? gameTime.Key.Substring(0, 10) : gameTime.Key);
                        SeriesCollection.First().Values.Add(gameTime.Value / (60 * 1000));
                    });

                    temp += gameTime.Value;
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() => { TotalGameTime = (double) temp / (60 * 1000); });
            });
        }

        #region IDisposable Support

        private bool disposedValue; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~GameTimeCounterViewModel()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}