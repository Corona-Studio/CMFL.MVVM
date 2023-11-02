using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.AuthLib
{
    public class Checksums
    {
        [JsonProperty("sha256")] public string Sha256 { get; set; }
    }


    public class AuthLibLatestJson
    {
        [JsonProperty("build_number")] public int BuildNumber { get; set; }

        [JsonProperty("version")] public string Version { get; set; }

        [JsonProperty("download_url")] public string DownloadUrl { get; set; }

        [JsonProperty("checksums")] public Checksums CheckSums { get; set; }
    }
}