using System;
using System.Linq;
using System.Windows.Forms;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.ViewModels;

namespace CMFL.MVVM.Models.DataModel.Game.Account
{
    public abstract class AccountInfo
    {
        /// <summary>
        ///     账户标识符
        /// </summary>
        public string Guid { get; set; }

        public string SearchGuid { get; set; }

        /// <summary>
        ///     游戏名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     是否为选定账户
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        ///     选定账户用作登录
        /// </summary>
        public void SelectAccount()
        {
            try
            {
                Selected = true;
                ViewModelLocator.MainWindowViewModel.WelcomeText =
                    $"{LanguageHelper.GetField("Welcome")}，{DisplayName}";
                SettingsHelper.Settings.AccountInfos.ForEach(o => o.Selected = false);
                SettingsHelper.Settings.AccountInfos.First(o => o.SearchGuid == SearchGuid).Selected = true;
                SettingsHelper.Save();
            }
            catch (Exception e)
            {
                NotifyHelper.ShowNotification("选择失败！", "请检查启动器日志！", 3000, ToolTipIcon.Error);
                LogHelper.WriteLogLine("选择游戏账户失败！", LogHelper.LogLevels.Error);
                LogHelper.WriteError(e);
            }
        }

        public void Delete()
        {
            try
            {
                var index = SettingsHelper.Settings.AccountInfos.FindIndex(account =>
                    account.SearchGuid.Equals(SearchGuid, StringComparison.Ordinal));
                SettingsHelper.Settings.AccountInfos.RemoveAt(index);
                SettingsHelper.Save();
            }
            catch (Exception e)
            {
                NotifyHelper.ShowNotification("删除失败！", "请检查启动器日志！", 3000, ToolTipIcon.Error);
                LogHelper.WriteLogLine("删除游戏账户失败！", LogHelper.LogLevels.Error);
                LogHelper.WriteError(e);
            }
        }

        /// <summary>
        ///     返回是否为在线账户
        /// </summary>
        /// <returns></returns>
        public bool IsOnlineAccount()
        {
            return this is OnlineAccount;
        }

        public static AccountInfo GetSelectedAccount()
        {
            return SettingsHelper.Settings.AccountInfos.FirstOrDefault(o => o.Selected);
        }
    }
}