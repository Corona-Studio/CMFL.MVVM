using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Mojang
{
    /// <summary>
    ///     Mojang返回的用户名以及ID
    /// </summary>
    public class MojangNameAndId
    {
        /// <summary>
        ///     ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     用户名
        /// </summary>
        [JsonProperty("name")]
        [DataMember]
        public string Name { get; set; }
    }
}