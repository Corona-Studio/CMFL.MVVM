using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Game
{
    /// <summary>
    ///     Assets信息
    /// </summary>
    public class AssetIndex
    {
        /// <summary>
        ///     ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     sha1检验码
        /// </summary>
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        /// <summary>
        ///     大小
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        ///     链接
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        ///     总大小
        /// </summary>
        [JsonProperty("totalSize")]
        public int TotalSize { get; set; }
    }

    /// <summary>
    ///     客户端
    /// </summary>
    public class Client
    {
        /// <summary>
        ///     sha1检验码
        /// </summary>
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        /// <summary>
        ///     大小
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        ///     链接
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    /// <summary>
    ///     服务器
    /// </summary>
    public class Server
    {
        /// <summary>
        ///     sha1检验码
        /// </summary>
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        /// <summary>
        ///     大小
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        ///     链接
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    /// <summary>
    ///     下载总成
    /// </summary>
    public class Downloads
    {
        /// <summary>
        ///     客户端
        /// </summary>
        [JsonProperty("client")]
        public Client Client { get; set; }

        /// <summary>
        ///     服务器
        /// </summary>
        [JsonProperty("server")]
        public Server Server { get; set; }
    }


    /// <summary>
    ///     Artifact
    /// </summary>
    public class Artifact
    {
        /// <summary>
        ///     路径
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        ///     sha1检验码
        /// </summary>
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        /// <summary>
        ///     大小
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        ///     链接
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    /// <summary>
    ///     Libraries下载
    /// </summary>
    public class LibrariesDownloads
    {
        /// <summary>
        ///     Artifact
        /// </summary>
        [JsonProperty("artifact")]
        public Artifact Artifact { get; set; }
    }

    /// <summary>
    ///     MC Libraries
    /// </summary>
    public class McLibraries
    {
        /// <summary>
        ///     名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     下载
        /// </summary>
        [JsonProperty("downloads")]
        public LibrariesDownloads Downloads { get; set; }
    }

    /// <summary>
    ///     JSON总成
    /// </summary>
    public class McGameJson
    {
        /// <summary>
        ///     Assets总成
        /// </summary>
        [JsonProperty("assetIndex")]
        public AssetIndex AssetIndex { get; set; }

        /// <summary>
        ///     Assets版本
        /// </summary>
        [JsonProperty("assets")]
        public string Assets { get; set; }

        /// <summary>
        ///     下载总成
        /// </summary>
        [JsonProperty("downloads")]
        public Downloads Downloads { get; set; }

        /// <summary>
        ///     ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Libraries总成
        /// </summary>
        [JsonProperty("libraries")]
        public McLibraries[] Libraries { get; set; }

        /// <summary>
        ///     主类
        /// </summary>
        [JsonProperty("mainClass")]
        public string MainClass { get; set; }

        /// <summary>
        ///     启动参数
        /// </summary>
        [JsonProperty("minecraftArgument")]
        public string MinecraftArguments { get; set; }

        /// <summary>
        ///     最低启动器版本
        /// </summary>
        [JsonProperty("minimumLauncherVersion")]
        public int MinimumLauncherVersion { get; set; }

        /// <summary>
        ///     发布日期
        /// </summary>
        [JsonProperty("releaseTime")]
        public string ReleaseTime { get; set; }

        /// <summary>
        ///     时间
        /// </summary>
        [JsonProperty("time")]
        public string Time { get; set; }

        /// <summary>
        ///     类型
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}