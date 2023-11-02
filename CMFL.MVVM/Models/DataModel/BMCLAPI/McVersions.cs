using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.BMCLAPI
{
    /// <summary>
    ///     最新版本
    /// </summary>
    public class Latest
    {
        /// <summary>
        ///     快照版本
        /// </summary>
        [JsonProperty("snapshot")]
        public string Snapshot { get; set; }

        /// <summary>
        ///     发布版本
        /// </summary>
        [JsonProperty("release")]
        public string Release { get; set; }
    }

    /// <summary>
    ///     版本信息
    /// </summary>
    public class Versions
    {
        /// <summary>
        ///     ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     类型
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        ///     时间
        /// </summary>
        [JsonProperty("time")]
        public DateTime Time { get; set; }

        /// <summary>
        ///     发布时间
        /// </summary>
        [JsonProperty("releaseTime")]
        public string ReleaseTime { get; set; }

        /// <summary>
        ///     地址
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    /// <summary>
    ///     MC版本
    /// </summary>
    public class McVersions
    {
        /// <summary>
        ///     最新
        /// </summary>
        [JsonProperty("latest")]
        public Latest Latest { get; set; }

        /// <summary>
        ///     版本号
        /// </summary>
        [JsonProperty("versions")]
        public List<Versions> Versions { get; set; }
    }
}