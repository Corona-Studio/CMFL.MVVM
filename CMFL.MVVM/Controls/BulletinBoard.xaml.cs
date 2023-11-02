using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Models.DataModel.Launcher;
using CMFL.MVVM.ViewModels;
using GalaSoft.MvvmLight.Threading;
using Heyo.Class.Helper;
using Newtonsoft.Json;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model;

namespace CMFL.MVVM.Controls
{
    /// <summary>
    ///     BulletinBoard.xaml 的交互逻辑
    /// </summary>
    public partial class BulletinBoard : UserControl, IDisposable
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromSeconds(5)
        };

        private List<BulletinData> _bulletinData;
        private int _index;
        private string _previousColor = string.Empty;

        public BulletinBoard()
        {
            InitializeComponent();
        }

        private Dispatcher CurrentDispatcher { get; } = Dispatcher.CurrentDispatcher;

        public void GetBulletin()
        {
            LauncherHelper.GetBulletin().ContinueWith(task =>
            {
                if (!Directory.Exists($"{Environment.CurrentDirectory}\\CMFL\\Temp\\"))
                    Directory.CreateDirectory($"{Environment.CurrentDirectory}\\CMFL\\Temp\\");

                if (task.Exception != null || task.Result.TaskStatus != TaskResultStatus.Success)
                {
                    if (File.Exists($"{Environment.CurrentDirectory}\\CMFL\\Temp\\BulletinCache.json"))
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            Show(JsonConvert.DeserializeObject<List<BulletinData>>(
                                File.ReadAllText(
                                    $"{Environment.CurrentDirectory}\\CMFL\\Temp\\BulletinCache.json")));
                        });

                        LogHelper.WriteLogLine(LanguageHelper.GetField("UsingCachedBulletin"),
                            LogHelper.LogLevels.Info);
                        LogHelper.WriteError(task.Exception);
                        return;
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        MainGrid.Background = new SolidColorBrush(ColorTranslator.FromHtml("#7F000000").ToMedia());
                        Title.Text = $"{LanguageHelper.GetField("Error")}：";
                        Content.Text = LanguageHelper.GetField("GetBulletinFailed");
                    });
                    LogHelper.WriteLogLine(LanguageHelper.GetField("GetBulletinFailed"), LogHelper.LogLevels.Error);
                    LogHelper.WriteError(task.Exception);
                    return;
                }

                FileHelper.Write($"{Environment.CurrentDirectory}\\CMFL\\Temp\\BulletinCache.json",
                    JsonConvert.SerializeObject(task.Result.Value));
                Show(task.Result.Value);
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        private void Show(List<BulletinData> data)
        {
            if (data.Count <= 1)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    MainGrid.Background =
                        new SolidColorBrush(ColorTranslator.FromHtml(GetBulletinColor(data[0].Level)).ToMedia());
                    Title.Text = $"{data[0].Name}：";
                    Content.Text = data[0].Content;
                });
            }
            else
            {
                _bulletinData = data;
                ShowBulletins();
            }
        }

        private string GetBulletinColor(BulletinColor color)
        {
            return color switch
            {
                BulletinColor.Black => "#B2000000",
                BulletinColor.Grey => "#B2636363",
                BulletinColor.Blue => "#B22962FF",
                BulletinColor.Green => "#B200C853",
                BulletinColor.Yellow => "#B2FFA726",
                BulletinColor.Red => "#B2F44336",
                _ => "#B2000000"
            };
        }

        private void ShowBulletins()
        {
            _timer.Tick += (sender, args) =>
            {
                if (!ViewModelLocator.HomePageViewModel.Monitoring)
                    return;

                if (string.IsNullOrEmpty(_previousColor))
                    _previousColor = GetBulletinColor(_bulletinData[_index].Level);

                if (_index + 1 > _bulletinData.Count)
                    _index = 0;

                CurrentDispatcher.Invoke(() =>
                {
                    MainGrid.Background =
                        new SolidColorBrush(ColorTranslator.FromHtml(GetBulletinColor(_bulletinData[_index].Level))
                            .ToMedia());
                    Title.Text = $"{_bulletinData[_index].Name}：";
                    Content.Text = HttpUtility.HtmlDecode(_bulletinData[_index].Content);
                });
                _previousColor = GetBulletinColor(_bulletinData[_index].Level);
                _index++;
            };
            _timer.IsEnabled = true;
            new Thread(() => { _timer.Start(); }) {IsBackground = true}.Start();
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
        ~BulletinBoard()
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