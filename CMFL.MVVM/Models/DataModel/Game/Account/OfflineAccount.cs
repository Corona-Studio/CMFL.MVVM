using System;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Interface;
using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.Game.Account
{
    [JsonObject]
    public class OfflineAccount : AccountInfo, IAccountInfo
    {
        /// <summary>
        ///     皮肤路径
        /// </summary>
        public string SkinPath { get; set; }

        public void Save()
        {
            var index = SettingsHelper.Settings.AccountInfos.FindIndex(account =>
                account.DisplayName.Equals(DisplayName, StringComparison.Ordinal) && account is OfflineAccount);
            SettingsHelper.Settings.AccountInfos[index] = this;
        }
    }
}