using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Launcher
{
    public class SocketCommandModel
    {
        [JsonProperty("arguments")] public string[] Arguments;
        [JsonProperty("command")] public string Command;

        public SocketCommandModel(string command, string[] arguments)
        {
            Command = command;
            Arguments = arguments;
        }
    }
}