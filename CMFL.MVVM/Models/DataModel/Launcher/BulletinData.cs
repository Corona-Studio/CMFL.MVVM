using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CMFL.MVVM.Models.DataModel.Launcher
{
    public enum BulletinColor
    {
        Black = 0,
        Grey,
        Blue,
        Green,
        Yellow,
        Red
    }

    public class BulletinData
    {
        /// <summary>
        ///     公告栏背景色
        /// </summary>
        [JsonProperty("level")]
        [JsonConverter(typeof(StringEnumConverter))]
        public BulletinColor Level { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     内容
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}