using System;
using System.Windows.Input;
using System.Windows.Media;
using CMFL.MVVM.Class.Helper.Graphic;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.RandomAvatarHelper;
using CMFL.MVVM.ViewModels;
using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Game
{
    public class GamePathModel
    {
        [JsonIgnore]
        public ImageSource Icon { get; } =
            RandomAvatarBuilder.Build(100).SetPadding(10).ToBytes().ToBitmap().ToImageSource();

        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsChecked { get; set; }

        [JsonIgnore]
        public ICommand ChooseGamePathCommand
        {
            get { return new DelegateCommand(obj => { SettingsHelper.Settings.ChoseGamePath = Path; }); }
        }

        [JsonIgnore]
        public ICommand DeletePathCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (SettingsHelper.Settings.GamePaths.Count == 1)
                    {
                        NotifyHelper.GetBasicMessageWithBadge("这是仅存的一个游戏目录!您无法将它删除", NotifyHelper.MessageType.Warning)
                            .Queue();
                        return;
                    }

                    var index = SettingsHelper.Settings.GamePaths.FindIndex(p =>
                        p.Name.Equals(Name, StringComparison.Ordinal));
                    ViewModelLocator.GamePageViewModel.GamePathList.RemoveAt(index);
                    SettingsHelper.Settings.GamePaths.RemoveAt(index);
                    SettingsHelper.Save();
                    NotifyHelper.GetBasicMessageWithBadge("删除成功！", NotifyHelper.MessageType.Success)
                        .Queue();
                });
            }
        }
    }
}