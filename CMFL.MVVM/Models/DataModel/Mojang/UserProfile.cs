using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Mojang
{
    /// <summary>
    ///     属性
    /// </summary>
    public class Properties
    {
        /// <summary>
        ///     姓名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     值
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        ///     签名
        /// </summary>
        [JsonProperty("signature")]
        public string Signature { get; set; }
    }

    /// <summary>
    ///     Mojang用户信息总成
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        ///     玩家ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     玩家名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     玩家属性
        /// </summary>
        [JsonProperty("Properties")]
        public Properties[] properties { get; set; }
    }
}