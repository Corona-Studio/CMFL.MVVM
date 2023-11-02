using System.Collections.Generic;
using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Launcher
{
    public static class UserData
    {
        public enum OnlineState
        {
            Online = 0,
            InGame = 1,
            WantsToPlay = 2,
            AFK = 3
        }

        /// <summary>
        ///     Data of an online user.
        /// </summary>
        public class OnlineUserData
        {
            [JsonProperty("InGame")] public string InGame;

            [JsonProperty("onlineState")] public OnlineState OnlineState;

            [JsonProperty("sid")] public string Sid;

            [JsonProperty("threadId")] public int ThreadId;

            [JsonProperty("username")] public string Username;
        }

        /// <summary>
        ///     User's database data
        /// </summary>
        public class DBUserData
        {
            /// <summary>
            ///     邮箱
            /// </summary>
            [JsonProperty("email")] public string Email;

            /// <summary>
            ///     朋友
            /// </summary>
            [JsonProperty("friends")] public List<string> Friends;

            /// <summary>
            ///     emmm
            /// </summary>
            [JsonProperty("idiograph")] public string Idiograph;

            /// <summary>
            ///     邀请
            /// </summary>
            [JsonProperty("invitation")] public string Invitation;

            /// <summary>
            ///     管理员
            /// </summary>
            [JsonProperty("isAdmin")] public bool IsAdmin;

            /// <summary>
            ///     密码
            /// </summary>
            [JsonProperty("password")] public string Password;

            /// <summary>
            ///     用户名
            /// </summary>
            [JsonProperty("username")] public string Username;
        }
    }
}