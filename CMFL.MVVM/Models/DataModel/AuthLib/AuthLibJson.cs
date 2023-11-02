using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.AuthLib
{
    public class Artifacts
    {
        [JsonProperty("build_number")] public int BuildNumber { get; set; }

        [JsonProperty("version")] public string Version { get; set; }
    }

    public class AuthLibJson
    {
        [JsonProperty("latest_build_number")] public string LatestBuildNumber { get; set; }

        [JsonProperty("artifacts")] public Artifacts[] Artifacts { get; set; }
    }
}