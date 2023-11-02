using System;
using Newtonsoft.Json;

namespace ProjAurora.Models
{
    #region Libraries

    public class Artifact
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("sha1")]
        public string SHA1 { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class LibraryDownloadInfo
    {
        [JsonProperty("artifact")]
        public Artifact Artifact { get; set; }
    }

    public class Library
    {
        [JsonProperty("downloads")]
        public LibraryDownloadInfo Downloads { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    #endregion

    #region Downloads

    public class DownloadInfo
    {
        [JsonProperty("sha1")]
        public string SHA1 { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Downloads
    {
        [JsonProperty("client")]
        public DownloadInfo Client { get; set; }

        [JsonProperty("server")]
        public DownloadInfo Server { get; set; }
    }

    #endregion

    #region AssetIndex

    public class AssetIndex
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sha1")]
        public string SHA1 { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("totalSize")]
        public long TotalSize { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    #endregion

    #region Arguments

    public class Arguments
    {
        [JsonProperty("game")]
        public object[] Game { get; set; }

        [JsonProperty("jvm")]
        public object[] Jvm { get; set; }
    }

    #endregion

    #region Logging

    public class File
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sha1")]
        public string SHA1 { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Client
    {
        [JsonProperty("argument")]
        public string Argument { get; set; }

        [JsonProperty("file")]
        public File File { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Logging
    {
        [JsonProperty("client")]
        public Client Client { get; set; }
    }

    #endregion

    public class Version
    {
        [JsonProperty("arguments")]
        public Arguments Arguments { get; set; }

        [JsonProperty("assetIndex")]
        public AssetIndex AssetIndex { get; set; }

        [JsonProperty("assets")]
        public string Assets { get; set; }

        [JsonProperty("downloads")]
        public Downloads Downloads { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("libraries")]
        public Library[] Libraries { get; set; }

        [JsonProperty("logging")]
        public Logging Logging { get; set; }

        [JsonProperty("mainClass")]
        public string MainClass { get; set; }

        [JsonProperty("minimumLauncherVersion")]
        public int MinimumLauncherVersion { get; set; }

        [JsonProperty("releaseTime")]
        public DateTime ReleaseTime { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } 
    }
}