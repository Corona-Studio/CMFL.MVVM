using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Launcher
{
    public class CommandResultModel
    {
        [JsonProperty("result")] public string Result { get; set; }

        [JsonProperty("message")] public string Message { get; set; }
    }
}