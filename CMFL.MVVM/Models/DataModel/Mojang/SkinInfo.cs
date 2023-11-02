using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Mojang
{
    /// <summary>
    ///     元数据
    /// </summary>
    public class MetaData
    {
        /// <summary>
        ///     模型
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }
    }

    /// <summary>
    ///     皮肤信息
    /// </summary>
    public class Skin
    {
        /// <summary>
        ///     地址
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        ///     元数据
        /// </summary>
        [JsonProperty("metadata")]
        public MetaData Metadata { get; set; }
    }

    /// <summary>
    ///     披风
    /// </summary>
    public class Cape
    {
        /// <summary>
        ///     地址
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    /// <summary>
    ///     材质信息
    /// </summary>
    public class Textures
    {
        /// <summary>
        ///     皮肤
        /// </summary>
        [JsonProperty("SKIN")]
        public Skin Skin { get; set; }

        /// <summary>
        ///     披风
        /// </summary>
        [JsonProperty("CAPE")]
        public Cape Cape { get; set; }
    }

    /// <summary>
    ///     皮肤信息总成
    /// </summary>
    public class SkinInfo
    {
        /// <summary>
        ///     时间戳
        /// </summary>
        [JsonProperty("timestamp")]
        public string TimeStamp { get; set; }

        /// <summary>
        ///     档案ID
        /// </summary>
        [JsonProperty("profileId")]
        public string ProfileId { get; set; }

        /// <summary>
        ///     档案名称
        /// </summary>
        [JsonProperty("profileName")]
        public string ProfileName { get; set; }

        /// <summary>
        ///     是否公开
        /// </summary>
        [JsonProperty("isPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        ///     材质信息
        /// </summary>
        [JsonProperty("textures")]
        public Textures Textures { get; set; }
    }
}