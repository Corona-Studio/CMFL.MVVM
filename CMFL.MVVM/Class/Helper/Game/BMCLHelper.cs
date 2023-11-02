using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Models.DataModel.BMCLAPI;
using CMFL.MVVM.Models.DataModel.LiteLoader;
using CMFL.MVVM.Models.DataModel.Optifine;
using Newtonsoft.Json;
using ProjBobcat.Class.Model;

namespace CMFL.MVVM.Class.Helper.Game
{
    /// <summary>
    ///     BMCLAPI实现
    /// </summary>
    public static class BMCLHelper
    {
        /// <summary>
        ///     获取Java的下载路径
        /// </summary>
        /// <returns>返回Java下载链接</returns>
        public static Task<TaskResult<Dictionary<string, string>>> GetJavaDownloadLinks()
        {
            return Task.Run(async () =>
            {
                using var client = new WebClient();
                var javaLinks = new Dictionary<string, string>();
                var serverReply =
                    await client
                        .DownloadStringTaskAsync(new Uri($"{SettingsHelper.BmclapiDownloadServer}java/list"))
                        .ConfigureAwait(true);
                var javas = JsonConvert.DeserializeObject<List<JavaDownloadLinks>>(serverReply);
                foreach (var item in javas)
                    if (item.Title.Contains("Windows") && !item.Title.Contains("64"))
                    {
                        javaLinks.Add("32", $"{SettingsHelper.BmclapiDownloadServer}java/" + item.File);
                    }
                    else
                    {
                        if (item.Title.Contains("Windows") && item.Title.Contains("64"))
                            javaLinks.Add("64", $"{SettingsHelper.BmclapiDownloadServer}java/" + item.File);
                    }

                return new TaskResult<Dictionary<string, string>>(TaskResultStatus.Success, value: javaLinks);
            });
        }

        /// <summary>
        ///     从BMCLAPI获取原版MC信息
        /// </summary>
        /// <returns></returns>
        public static Task<TaskResult<McVersions>> GetAllMcVersions()
        {
            return Task.Run(async () =>
            {
                using var client = new WebClient();
                var serverReply =
                    await client.DownloadStringTaskAsync(
                            new Uri($"{SettingsHelper.BmclapiDownloadServer}mc/game/version_manifest.json"))
                        .ConfigureAwait(true);
                var versions = JsonConvert.DeserializeObject<McVersions>(serverReply);
                return new TaskResult<McVersions>(TaskResultStatus.Success, value: versions);
            });
        }

        /// <summary>
        ///     获取所有OptiFine构建
        /// </summary>
        /// <returns></returns>
        public static Task<TaskResult<List<string>>> GetAllOptiFineSupportVersions()
        {
            return Task.Run(async () =>
            {
                using var client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                var serverReply =
                    await client.DownloadStringTaskAsync(
                            new Uri($"{SettingsHelper.BmclapiDownloadServer}optifine/versionList"))
                        .ConfigureAwait(true);
                var data = JsonConvert.DeserializeObject<List<OptifineBuilds>>(serverReply);
                var versions = data.Select(item => item.McVersion).ToList();
                return new TaskResult<List<string>>(TaskResultStatus.Success, value: versions.Distinct().ToList());
            });
        }

        /// <summary>
        ///     获取所有Forge支持的MC版本
        /// </summary>
        /// <returns>Mc版本 List string </returns>
        public static Task<TaskResult<List<string>>> GetAllSupportedForgeVersion()
        {
            return Task.Run(async () =>
            {
                using var client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                var serverReply =
                    await client
                        .DownloadStringTaskAsync(new Uri($"{SettingsHelper.BmclapiDownloadServer}forge/minecraft"))
                        .ConfigureAwait(true);
                var data = JsonConvert.DeserializeObject<List<string>>(serverReply);
                data.Reverse();
                return new TaskResult<List<string>>(TaskResultStatus.Success, value: data);
            });
        }

        /// <summary>
        ///     获取所有LiteLoader支持的MC版本
        /// </summary>
        /// <returns>Mc版本 List string </returns>
        public static Task<TaskResult<List<LiteLoaderVersionMetaModel>>> GetAllLiteLoaderBuilds()
        {
            return Task.Run(async () =>
            {
                using var client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                var serverReply =
                    await client
                        .DownloadStringTaskAsync(new Uri($"{SettingsHelper.BmclapiDownloadServer}liteloader/list"))
                        .ConfigureAwait(true);
                var data = JsonConvert.DeserializeObject<List<LiteLoaderVersionMetaModel>>(serverReply);
                data.Reverse();
                return new TaskResult<List<LiteLoaderVersionMetaModel>>(TaskResultStatus.Success, value: data);
            });
        }
    }
}