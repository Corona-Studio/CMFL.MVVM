using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Other;
using CMFL.MVVM.Models.DataModel.Mojang;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjCrypto.Class.Helper;

namespace CMFL.MVVM.Class.Helper.Game
{
    public static class MojangApi
    {
        public static async Task<Dictionary<string, ServerStatus>> ApiCheck()
        {
            using var client = new WebClient();
            client.Headers.Add("Content-Type", "application/json");
            var data =
                JArray.Parse(
                    await client.DownloadStringTaskAsync(new Uri("https://status.mojang.com/check"))
                        .ConfigureAwait(true));

            return data.SelectMany(item => ((JObject) item).Properties()).ToDictionary(property => property.Name,
                property => (ServerStatus) Enum.Parse(typeof(ServerStatus), property.Value.Value<string>()));
        }

        /// <summary>
        ///     通过游戏名获取UUID
        /// </summary>
        /// <param name="name">游戏名</param>
        /// <param name="timeStamp">时间戳</param>
        /// <returns>UUID</returns>
        public static async Task<string> GetUuidByNameAndTimeStamp(string name, long timeStamp)
        {
            try
            {
                using var client = new WebClient();
                var serverReply = await client.DownloadStringTaskAsync(new Uri(
                        $"https://api.mojang.com/users/profiles/minecraft/{name}?at={timeStamp.ToString()}"))
                    .ConfigureAwait(true);

                var mojangNameAndId = JsonConvert.DeserializeObject<MojangNameAndId>(serverReply);
                return mojangNameAndId.Id;
            }
            catch (WebException ex)
            {
                NotifyHelper.ShowNotification(LanguageHelper.GetField("GetUuidFailed"),
                    LanguageHelper.GetField("ApiLimitsReason"), 3000, ToolTipIcon.Error);
                LogHelper.WriteLogLine(
                    LanguageHelper.GetFields("GetUuidFailed|ApiLimitsReason", ", "),
                    LogHelper.LogLevels.Error);
                LogHelper.WriteError(ex);
                return null;
            }
        }

        public static async Task<string> GetSkinUrl(string uuid)
        {
            try
            {
                using var client = new WebClient();
                var serverReply = await client.DownloadStringTaskAsync(
                        new Uri($"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}"))
                    .ConfigureAwait(true);

                var userProfile = JsonConvert.DeserializeObject<UserProfile>(serverReply);
                var skinInfo =
                    JsonConvert.DeserializeObject<SkinInfo>(
                        StringEncryptHelper.Base64Decode(userProfile.properties[0].Value));
                return skinInfo.Textures.Skin.Url;
            }
            catch (WebException ex)
            {
                NotifyHelper.ShowNotification(LanguageHelper.GetField("GetSkinFailed"),
                    LanguageHelper.GetField("ApiLimitsReason"), 3000, ToolTipIcon.Error);
                LogHelper.WriteLogLine(
                    LanguageHelper.GetFields("GetUuidFailed|ApiLimitsReason", ", "),
                    LogHelper.LogLevels.Error);
                LogHelper.WriteError(ex);
                return null;
            }
        }

        /// <summary>
        ///     获取用户头像
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>头像</returns>
        public static async Task<Bitmap> GetUserAvatar(string username)
        {
            var uuid = await GetUuidByNameAndTimeStamp(username, TimeHelper.GetTimeStampTen()).ConfigureAwait(true);
            return null;
        }
    }
}