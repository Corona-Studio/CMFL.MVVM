using System;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Interface;
using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Game.Account
{
    [JsonObject]
    public class OnlineAccount : AccountInfo, IAccountInfo
    {
        /// <summary>
        ///     邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     认证服务器地址
        /// </summary>
        public string AuthServer { get; set; }

        /// <summary>
        ///     登陆成功得到的UUID
        /// </summary>
        public Guid Uuid { get; set; }

        /// <summary>
        ///     上一次认证时间
        /// </summary>
        public DateTime LastAuthTime { get; set; }

        /// <summary>
        ///     上一次认证状态（true为成功）
        /// </summary>
        public bool LastAuthState { get; set; }

        public void Save()
        {
            var index = SettingsHelper.Settings.AccountInfos.IndexOf(this);
            SettingsHelper.Settings.AccountInfos[index] = this;
            SettingsHelper.Save();
        }

        public void UpdateAuthTime(DateTime dt)
        {
            LastAuthTime = dt;
        }
    }
}