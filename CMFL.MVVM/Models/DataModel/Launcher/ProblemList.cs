using MaterialDesignThemes.Wpf;

namespace CMFL.MVVM.Models.DataModel.Launcher
{
    /// <summary>
    ///     （CraftMineFun_Launcher游戏环境检测服务）问题列表
    /// </summary>
    public class ProblemList
    {
        public string Title { get; set; }
        public PackIconKind Kind { get; set; }
        public string IconColor { get; set; }
        public string Context { get; set; }
    }
}