using System.Collections.Generic;
using System.IO;
using CMFL.MVVM.Models.DataModel.Game;
using CMFL.MVVM.Models.DataModel.Game.Account;
using Newtonsoft.Json;

namespace CMFL.MVVM.Class.Helper.Launcher.Settings
{
    /// <summary>
    ///     设置代理
    /// </summary>
    public class SettingProxy
    {
        [JsonIgnore] private string _choseGamePath;

        /// <summary>
        ///     保存的游戏路径
        /// </summary>
        public List<GamePathModel> GamePaths { get; set; }

        /// <summary>
        ///     是否使用本地音乐
        /// </summary>
        public bool UseLocalMusic { get; set; }

        /// <summary>
        ///     是否自动确定最快的下载服务器
        /// </summary>
        public bool AutoDetectBestDownloadServer { get; set; }

        /// <summary>
        ///     是否使用本地音乐
        /// </summary>
        public double BgmVolume { get; set; }

        /// <summary>
        ///     是否随机应用壁纸
        /// </summary>
        public bool RandomWallPaper { get; set; }

        /// <summary>
        ///     是否关闭游戏后恢复音乐
        /// </summary>
        public bool ResumeMusic { get; set; }

        /// <summary>
        ///     是否登陆到启动器
        /// </summary>
        public bool LoggedInToCMFL { get; set; }

        /// <summary>
        ///     是否连接到CMFL服务器主框架
        /// </summary>
        public bool IsConnectedToCMF { get; set; }

        /// <summary>
        ///     启动器动画FPS
        /// </summary>
        public int AnimationFps { get; set; }

        /// <summary>
        ///     账户数据
        /// </summary>
        public List<AccountInfo> AccountInfos { get; set; }

        /// <summary>
        ///     性能模式
        /// </summary>
        public bool BoostMode { get; set; }

        /// <summary>
        ///     选中的游戏路径
        /// </summary>
        public string ChoseGamePath
        {
            get => Path.GetFullPath(_choseGamePath);
            set => _choseGamePath = value;
        }

        /// <summary>
        ///     是否为第一次使用启动器
        /// </summary>
        public bool FirstTime { get; set; }

        /// <summary>
        ///     是否忽略警告
        /// </summary>
        public bool IgnoreWarning { get; set; }

        /// <summary>
        ///     是否启用BMCLAPI
        /// </summary>
        public bool EnableBMCLAPI { get; set; }

        /// <summary>
        ///     是否启用LiteMode
        /// </summary>
        public bool LiteMode { get; set; }

        /// <summary>
        ///     下载线程
        /// </summary>
        public int DownloadThread { get; set; }

        /// <summary>
        ///     启动器背景模糊类型
        /// </summary>
        public string BlurType { get; set; }

        /// <summary>
        ///     左侧栏模糊强度
        /// </summary>
        public double LeftBorderBlurRadius { get; set; }

        /// <summary>
        ///     左侧半透明叠层
        /// </summary>
        public bool LeftBorderOpacityLayerVisibility { get; set; }

        /// <summary>
        ///     启动器BGM路径
        /// </summary>
        public string MusicFilePath { get; set; }

        /// <summary>
        ///     Java路径集合
        /// </summary>
        public List<string> JavasPath { get; set; }

        /// <summary>
        ///     选择的java
        /// </summary>
        public int SelectedJavaIndex { get; set; }

        /// <summary>
        ///     启动器背景图片路径
        /// </summary>
        public string BgPath { get; set; }

        /// <summary>
        ///     启动器背景图片模糊半径
        /// </summary>
        public double BlurRadius { get; set; }

        /// <summary>
        ///     是否启用调试模式
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        ///     是否自动清理内存
        /// </summary>
        public bool AutoCleanMemory { get; set; }

        /// <summary>
        ///     选择的游戏
        /// </summary>
        public string ChoseGame { get; set; }

        /// <summary>
        ///     启用版本隔离
        /// </summary>
        public bool VersionInsulation { get; set; }

        /// <summary>
        ///     是否保存启动器的用户名和密码
        /// </summary>
        public bool IsRememberMe { get; set; }

        /// <summary>
        ///     是否开启动画
        /// </summary>
        public bool EnableAnimation { get; set; }

        /// <summary>
        ///     启动器用户名
        /// </summary>
        public string LUsername { get; set; }

        /// <summary>
        ///     启动器密码
        /// </summary>
        public string LPassword { get; set; }

        /// <summary>
        ///     是否启用模糊效果
        /// </summary>
        public bool EnableBlur { get; set; }

        /// <summary>
        ///     已保存的游戏时长
        /// </summary>
        public Dictionary<string, long> GameTimes { get; set; }

        /// <summary>
        ///     使用BMCLAPI时启用HTTPS
        /// </summary>
        public bool UseHttpsForBmclapi { get; set; }

        /// <summary>
        ///     使用Mojang服务器
        /// </summary>
        public bool UseMojangServer { get; set; }

        /// <summary>
        ///     选择的界面语言索引
        /// </summary>
        public int SelectedLanguageIndex { get; set; }

        /// <summary>
        ///     选择的界面字体
        /// </summary>
        public string SelectedInterfaceFont { get; set; }

        /// <summary>
        ///     保存的游戏设置
        /// </summary>
        public Dictionary<string, GameSettings> GameSettings { get; set; }

        /// <summary>
        ///     默认的设置缺省值
        /// </summary>
        public GameSettings FallBackGameSettings { get; set; }

        /// <summary>
        ///     客户端Guid
        /// </summary>
        public string ClientToken { get; set; }

        /// <summary>
        ///     启动器Socket服务器
        /// </summary>
        public string LauncherSocketServer { get; set; }

        /// <summary>
        ///     启动器Web服务器
        /// </summary>
        public string LauncherWebServer { get; set; }
    }
}