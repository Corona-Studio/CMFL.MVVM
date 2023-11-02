using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CMFL.MVVM.Class.Helper.Game;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Interface;
using CMFL.MVVM.Models.DataModel.GameData;
using CMFL.MVVM.ViewModels;
using MaterialDesignThemes.Wpf;
using ProjBobcat.Class.Model.LauncherProfile;

namespace CMFL.MVVM.Models.DataModel.Game
{
    /// <summary>
    ///     截图
    /// </summary>
    public class ScreenShot : IScreenShot
    {
        /// <summary>
        ///     截图（BitmapSource）
        /// </summary>
        public BitmapSource ScreenShotSource { get; set; }

        /// <summary>
        ///     截图路径
        /// </summary>
        public string ScreenShotPath { get; set; }

        public ICommand OpenScreenShotCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    try
                    {
                        Process.Start(ScreenShotPath);
                    }
                    catch (FileNotFoundException ex)
                    {
                        LogHelper.WriteLogLine(
                            LanguageHelper.GetFields("OpenScreenShotFailedReason|FileNotFound", ": "),
                            LogHelper.LogLevels.Error);
                        LogHelper.WriteError(ex);
                    }
                });
            }
        }
    }

    /// <summary>
    ///     本地游戏列表数据模型
    /// </summary>
    public class GameInfo : PropertyChange, IGameInfo
    {
        private int _selectedGcTypeIndex;

        /// <summary>
        ///     游戏设置
        /// </summary>
        public GameSettings GameSettings { get; set; }

        /// <summary>
        ///     游戏图标
        /// </summary>
        public string Icon { get; set; }

        public int SelectedGcTypeIndex
        {
            get => _selectedGcTypeIndex;
            set
            {
                _selectedGcTypeIndex = value;
                OnPropertyChanged(nameof(SelectedGcTypeIndex));
            }
        }

        /// <summary>
        ///     选择的图标索引
        /// </summary>
        public int SelectedIconIndex { get; set; }

        public ICommand BrowseJavaCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using var javaFileDialog = new OpenFileDialog
                    {
                        InitialDirectory = "C:\\",
                        Filter = "javaw.exe|javaw.exe",
                        RestoreDirectory = true,
                        FilterIndex = 1
                    };
                    if (javaFileDialog.ShowDialog() != DialogResult.OK) return;

                    GameSettings.JavaPath = javaFileDialog.FileName;
                });
            }
        }

        public ICommand SaveGameSettingsCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    var key = SettingsHelper.Settings.GameSettings.FirstOrDefault(s =>
                        s.Value.VersionId.Equals(Version, StringComparison.Ordinal));
                    var profileKey = GameHelper.Core.VersionLocator.LauncherProfileParser.LauncherProfile.Profiles
                        .FirstOrDefault(p =>
                            p.Value.LastVersionId.Equals(Version, StringComparison.Ordinal));

                    if (key.Equals(default(KeyValuePair<string, GameSettings>)) ||
                        profileKey.Equals(default(KeyValuePair<string, GameProfileModel>)))
                    {
                        NotifyHelper.GetBasicMessageWithBadge("解析游戏配置文件失败", NotifyHelper.MessageType.Error).Queue();
                        return;
                    }

                    GameHelper.Core.VersionLocator.LauncherProfileParser.LauncherProfile.Profiles[profileKey.Key]
                        .Resolution = GameSettings.ScreenSize;
                    GameHelper.Core.VersionLocator.LauncherProfileParser.LauncherProfile.Profiles[profileKey.Key]
                        .JavaDir = GameSettings.JavaPath;

                    SettingsHelper.Settings.GameSettings[key.Key] = GameSettings;
                    SettingsHelper.Save();
                    GameHelper.Core.VersionLocator.LauncherProfileParser.SaveProfile();

                    DialogHost.CloseDialogCommand.Execute(null, null);
                    ViewModelLocator.GamePageViewModel.GetAllLocalGames();
                });
            }
        }

        public ICommand SaveGameNameAndIconCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    var key = SettingsHelper.Settings.GameSettings.FirstOrDefault(s =>
                        s.Value.VersionId.Equals(Version, StringComparison.Ordinal));
                    var profileKey = GameHelper.Core.VersionLocator.LauncherProfileParser.LauncherProfile.Profiles
                        .FirstOrDefault(p =>
                            p.Value.LastVersionId.Equals(Version, StringComparison.Ordinal));
                    if (key.Equals(default(KeyValuePair<string, GameSettings>)) ||
                        profileKey.Equals(default(KeyValuePair<string, GameProfileModel>)))
                    {
                        NotifyHelper.GetBasicMessageWithBadge("解析游戏配置文件失败", NotifyHelper.MessageType.Error).Queue();
                        return;
                    }

                    GameSettings.Icon = GameIconHelper.GetIconByIndex(SelectedIconIndex);

                    GameHelper.Core.VersionLocator.LauncherProfileParser.LauncherProfile.Profiles[profileKey.Key].Icon =
                        GameSettings.Icon;
                    GameHelper.Core.VersionLocator.LauncherProfileParser.LauncherProfile.Profiles[profileKey.Key].Name =
                        Name;

                    SettingsHelper.Settings.GameSettings.Remove(key.Key);
                    SettingsHelper.Settings.GameSettings.Add(Name, GameSettings);
                    SettingsHelper.Save();
                    GameHelper.Core.VersionLocator.LauncherProfileParser.SaveProfile();

                    DialogHost.CloseDialogCommand.Execute(null, null);
                    ViewModelLocator.GamePageViewModel.GetAllLocalGames();
                });
            }
        }

        /// <summary>
        ///     版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     游戏名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     材质包名称
        /// </summary>
        public IEnumerable<ResPack> ResPack { get; set; }

        /// <summary>
        ///     截图文件路径
        /// </summary>
        public IEnumerable<ScreenShot> ScreenShotUrl { get; set; }

        /// <summary>
        ///     光影包路径
        /// </summary>
        public IEnumerable<string> ShaderPackUrl { get; set; }

        /// <summary>
        ///     Mod路径
        /// </summary>
        public IEnumerable<string> ModUrl { get; set; }

        /// <summary>
        ///     存档路径
        /// </summary>
        public IEnumerable<string> SavesUrl { get; set; }

        /// <summary>
        ///     是否选择
        /// </summary>
        public bool IsChecked { get; set; }

        public ICommand ChooseGameCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ViewModelLocator.MainWindowViewModel.ChoseGame = Version;
                    SettingsHelper.Settings.ChoseGame = Version;
                    SettingsHelper.Save();
                });
            }
        }

        public ICommand OpenGameFolderCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    try
                    {
                        Process.Start($"{SettingsHelper.Settings.ChoseGamePath}\\versions\\{Version}\\");
                    }
                    catch (Win32Exception exception)
                    {
                        NotifyHelper.ShowNotification(LanguageHelper.GetFields("Open|Failed"),
                            LanguageHelper.GetField("OpenVersionFolderFailed"), 3000, ToolTipIcon.Error);
                        LogHelper.WriteLogLine(LanguageHelper.GetField("OpenVersionFolderFailed"),
                            LogHelper.LogLevels.Error);
                        LogHelper.WriteError(exception);
                    }
                });
            }
        }
    }
}