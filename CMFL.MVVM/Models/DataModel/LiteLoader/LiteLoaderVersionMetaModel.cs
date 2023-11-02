using System.Collections.Generic;
using Newtonsoft.Json;
using ProjBobcat.Class.Model;

namespace CMFL.MVVM.Models.DataModel.LiteLoader
{
    public class BuildInfo
    {
        [JsonProperty("tweakClass")]
        public string TweakClass { get; set; }

        [JsonProperty("libraries")]
        public List<Library> Libraries { get; set; }

        [JsonProperty("stream")]
        public string Stream { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("build")]
        public string Build { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        [JsonProperty("timestamp")]
        public string TimeStamp { get; set; }

        [JsonProperty("lastSuccessfulBuild")]
        public int LastSuccessfulBuild { get; set; }
    }

    public class LiteLoaderVersionMetaModel
    {
        [JsonProperty("_id")]
        public string IdLocker { get; set; }

        [JsonProperty("mcversion")]
        public string McVersion { get; set; }

        [JsonProperty("build")]
        public BuildInfo Build { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("__v")]
        public int VersionLocker { get; set; }
    }
}