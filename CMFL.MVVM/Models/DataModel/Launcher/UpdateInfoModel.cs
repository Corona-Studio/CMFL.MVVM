using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Launcher
{
    public class UpdateInfoModel
    {
        [JsonProperty("version")] public string Version { get; set; }

        [JsonProperty("description")] public string Description { get; set; }

        [JsonProperty("time")] public string Time { get; set; }

        [JsonProperty("sha")] public string Sha { get; set; }

        [JsonProperty("name")] public string Name { get; set; }
    }
}