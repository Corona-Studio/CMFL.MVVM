using System.Collections.Generic;
using System.Windows.Input;
using CMFL.MVVM.Models.DataModel.Game;
using CMFL.MVVM.Models.DataModel.GameData;

namespace CMFL.MVVM.Interface
{
    public interface IGameInfo
    {
        string Version { get; set; }
        string Name { get; set; }
        IEnumerable<ResPack> ResPack { get; set; }
        IEnumerable<ScreenShot> ScreenShotUrl { get; set; }
        IEnumerable<string> ShaderPackUrl { get; set; }
        IEnumerable<string> ModUrl { get; set; }
        IEnumerable<string> SavesUrl { get; set; }
        bool IsChecked { get; set; }
        ICommand ChooseGameCommand { get; }
        ICommand OpenGameFolderCommand { get; }
    }
}