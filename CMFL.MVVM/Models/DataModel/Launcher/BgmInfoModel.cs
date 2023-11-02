using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Launcher
{
    public class BgmInfoModel
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("filename")] public string FileName { get; set; }

        [JsonProperty("author")] public string Author { get; set; }

        [JsonIgnore] public bool UseLocalMusic { get; set; }

        [JsonIgnore] public string LocalMusicPath { get; set; }
    }
}