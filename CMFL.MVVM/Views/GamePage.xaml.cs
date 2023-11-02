using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.ViewModels;
using ProjBobcat.Class.Helper;
using Button = System.Windows.Controls.Button;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;

namespace CMFL.MVVM.Views
{
    /// <summary>
    ///     GamePage.xaml 的交互逻辑
    /// </summary>
    public partial class GamePage : Page
    {
        private int _confirmDelete;

        private List<string> _path = new List<string>();

        public GamePage()
        {
            InitializeComponent();

            if (ViewOperationBase.IsInDesignMode)
                return;

            DragEnter += OnDragEnterOrOver;
            //this.DragOver += OnDragEnterOrOver;
            DragLeave += OnDragLeave;
            Drop += OnDrop;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e != null)
                _path = ((string[]) e.Data.GetData(DataFormats.FileDrop) ?? throw new InvalidOperationException())
                    .ToList(); //获得路径
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            DragCheckGrid.BeginStoryboard((Storyboard) FindResource("GridDragCheckOpacityDrop"));
            Dispatcher.Invoke(() =>
            {
                DragCheckGrid.Visibility = Visibility.Hidden;
                FileChooseBar.IsActive = false;
            });
        }

        private void OnDragEnterOrOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.All
                : DragDropEffects.None;
            Dispatcher.Invoke(() =>
            {
                DragCheckGrid.Visibility = Visibility.Visible;
                FileChooseBar.IsActive = true;
            });
            DragCheckGrid.BeginStoryboard((Storyboard) FindResource("GridDragCheckOpacityRaise"));
        }

        private void MoveGameFile(object sender, RoutedEventArgs e)
        {
            try
            {
                var typeNum = int.TryParse(((Button) sender).Tag.ToString(), out var type) ? type : -1;

                if (typeNum.Equals(-1))
                {
                    NotifyHelper.ShowNotification("移动文件失败！", "请提交反馈！！", 3000, ToolTipIcon.Error);
                    return;
                }

                var fileType =
                    (LauncherHelper.FileType) Enum.Parse(typeof(LauncherHelper.FileType), typeNum.ToString());

                if (!Directory.Exists(LauncherHelper.GetMoveFilePath(fileType)))
                {
                    var directoryInfo = new DirectoryInfo(LauncherHelper.GetMoveFilePath(fileType));
                    directoryInfo.Create();
                }

                Dispatcher.Invoke(() => { FileChooseBar.IsActive = false; });

                foreach (var files in _path)
                    if (File.Exists(files))
                    {
                        File.Move(files,
                            LauncherHelper.GetMoveFilePath(fileType) + files.Split('\\').Last());
                    }
                    else
                    {
                        if (Directory.Exists(files))
                            LauncherHelper.CopyDirectory(files,
                                LauncherHelper.GetMoveFilePath(fileType) + files.Split('\\').Last(),
                                true);
                    }

                NotifyHelper.ShowNotification("咩！！", "移动成功！", 3000, ToolTipIcon.Info);
            }
            catch (IOException exception)
            {
                NotifyHelper.ShowNotification("移动文件失败！", "无法移动游戏文件，请检查目录权限！", 3000, ToolTipIcon.Error);
                LogHelper.WriteLogLine("移动文件失败！", LogHelper.LogLevels.Error);
                LogHelper.WriteError(exception);
            }

            DragCheckGrid.BeginStoryboard((Storyboard) FindResource("GridDragCheckOpacityDrop"));
            Dispatcher.Invoke(() =>
            {
                DragCheckGrid.Visibility = Visibility.Hidden;
                FileChooseBar.IsActive = false;
            });

            _path.Clear();
        }

        private void DeleteGame(object sender, RoutedEventArgs e)
        {
            switch (_confirmDelete)
            {
                case 0:
                    _confirmDelete++;
                    break;
                case 1:
                    try
                    {
                        var button = (Button) sender;
                        DirectoryHelper.CleanDirectory(
                            $"{Environment.CurrentDirectory}\\.minecraft\\versions\\{button.Tag}\\", true);
                        NotifyHelper.ShowNotification("删除成功", "成功删除游戏", 3000, ToolTipIcon.Info);
                        _confirmDelete = 0;
                        ViewModelLocator.GamePageViewModel.GetAllLocalGames();
                    }
                    catch (IOException exception)
                    {
                        NotifyHelper.ShowNotification("删除失败！", "请查看日志", 3000, ToolTipIcon.Error);
                        LogHelper.WriteLogLine("删除游戏失败", LogHelper.LogLevels.Error);
                        LogHelper.WriteError(exception);
                        _confirmDelete = 0;
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }

                    break;
            }
        }

        private void CancelDeleteGame(object sender, RoutedEventArgs e)
        {
            _confirmDelete = 0;
        }

        private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var sv = (ScrollViewer) sender;
            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}