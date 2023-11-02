using CMFL.MVVM.Class.Helper.Game;
using ProjBobcat.Class.Model.LauncherProfile;

namespace CMFL.MVVM.Class.Helper.Launcher.Settings
{
    public class GameSettings
    {
        /// <summary>
        ///     是否启用CGC
        /// </summary>
        public bool EnableGc { get; set; }

        /// <summary>
        ///     Gc类型
        /// </summary>
        public int GcType { get; set; }

        /// <summary>
        ///     是否使用AuthLib-Injector
        /// </summary>
        public bool UseAuthLib { get; set; }

        /// <summary>
        ///     高级启动参数
        /// </summary>
        public string AdvancedArguments { get; set; }

        /// <summary>
        ///     是否强制启动
        /// </summary>
        public bool ForceLaunch { get; set; }

        /// <summary>
        ///     最小内存大小
        /// </summary>
        public int MinMemory { get; set; }

        /// <summary>
        ///     最大内存大小
        /// </summary>
        public int MaxMemory { get; set; }

        /// <summary>
        ///     选择的Java路径
        /// </summary>
        public string JavaPath { get; set; }

        /// <summary>
        ///     是否自动设置内存
        /// </summary>
        public bool AutoMemorySize { get; set; }

        /// <summary>
        ///     版本ID
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        ///     屏幕大小
        /// </summary>
        public ResolutionModel ScreenSize { get; set; }

        /// <summary>
        ///     需要自动加入的服务器地址
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        ///     游戏图标
        /// </summary>
        public string Icon { get; set; }

        public static GameSettings GetDefaultGameSettings()
        {
            return new GameSettings
            {
                EnableGc = true,
                GcType = 1,
                Icon = GameIconHelper.RandomIcon(true)
            };
        }
    }
}