using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Launcher
{
    public class ServerTestJson
    {
        [JsonProperty("icon")] public string Icon { get; set; }

        [JsonProperty("message")] public string Message { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("status")] public string Status { get; set; }
    }
}