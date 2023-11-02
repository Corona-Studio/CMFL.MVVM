using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Biliplus
{
    /// <summary>
    ///     分集
    /// </summary>
    public class Parts
    {
        /// <summary>
        ///     视频长度
        /// </summary>
        [JsonProperty("length")]
        public string Length { get; set; }

        /// <summary>
        ///     下载地址
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    /// <summary>
    ///     视频数据
    /// </summary>
    public class Data
    {
        /// <summary>
        ///     类型
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        ///     名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     信息
        /// </summary>
        [JsonProperty("info")]
        public string Info { get; set; }

        /// <summary>
        ///     分集
        /// </summary>
        [JsonProperty("parts")]
        public Parts[] Parts { get; set; }
    }

    /// <summary>
    ///     存储信息
    /// </summary>
    public class Storage
    {
        /// <summary>
        ///     访问权限
        /// </summary>
        [JsonProperty("access")]
        public int Access { get; set; }
    }

    /// <summary>
    ///     视频信息总成
    /// </summary>
    public class VideoInfo
    {
        /// <summary>
        ///     类型
        /// </summary>
        [JsonProperty("mode")]
        public string Mode { get; set; }

        /// <summary>
        ///     分集
        /// </summary>
        [JsonProperty("part")]
        public int Part { get; set; }

        /// <summary>
        ///     视频AV号
        /// </summary>
        [JsonProperty("cid")]
        public long Cid { get; set; }

        /// <summary>
        ///     视频长度
        /// </summary>
        [JsonProperty("length")]
        public string Length { get; set; }

        [JsonProperty("data")] public Data[] Data { get; set; }

        /// <summary>
        ///     有效时间
        /// </summary>
        [JsonProperty("expire")]
        public int Expire { get; set; }

        /// <summary>
        ///     提示
        /// </summary>
        [JsonProperty("warn")]
        public string Warn { get; set; }

        /// <summary>
        ///     存储信息
        /// </summary>
        [JsonProperty("storage")]
        public Storage Storage { get; set; }
    }
}