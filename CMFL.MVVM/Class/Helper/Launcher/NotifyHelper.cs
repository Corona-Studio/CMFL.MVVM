using System;
using System.Drawing;
using System.Windows.Forms;
using CMFL.MVVM.Class.Helper.Kernel;
using CMFL.MVVM.Properties;
using CMFL.MVVM.ViewModels;
using Enterwell.Clients.Wpf.Notifications;
using MaterialDesignThemes.Wpf;
using Icon = System.Drawing.Icon;

namespace CMFL.MVVM.Class.Helper.Launcher
{
    /// <summary>
    ///     通知显示
    /// </summary>
    public static class NotifyHelper
    {
        private static readonly string ProgramName = LanguageHelper.GetField("TrayIconName");

        private static readonly NotifyIcon NotifyIcon = new NotifyIcon
        {
            Icon = Icon.FromHandle(new Bitmap(Resources.logo).GetHicon()),
            Text = ProgramName,
            Visible = true
        };

        /// <summary>
        ///     显示通知（气泡）
        /// </summary>
        /// <param name="title">通知的标题</param>
        /// <param name="text">通知的内容</param>
        /// <param name="time">通知保留多长时间</param>
        /// <param name="toolTipIcon">通知的图标</param>
        /// <param name="clickEvent">气泡点击事件</param>
        public static void ShowNotification(string title, string text, int time, ToolTipIcon toolTipIcon,
            EventHandler clickEvent = null)
        {
            NotifyIcon.Click += clickEvent;
            NotifyIcon.ShowBalloonTip(time, title, text, toolTipIcon);
            ProcessHelper.RefreshTrayIcon();
        }

        #region 显示通知

        /// <summary>
        ///     消息类别
        /// </summary>
        public enum MessageType
        {
            Success = 0,
            Error = 1,
            Warning = 2,
            Info = 3
        }

        public static string GetNotifyBackgroundColor(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.Success => "#00e676",
                MessageType.Error => "#ff5252",
                MessageType.Warning => "#fbc02d",
                MessageType.Info => "#82b1ff",
                _ => "#90a4ae"
            };
        }

        [STAThread]
        public static NotificationMessageBuilder GetBasicMessageWithBadge(string message, MessageType type,
            int delay = 3000)
        {
            var badge = type switch
            {
                MessageType.Success => LanguageHelper.GetField("Succeeded"),
                MessageType.Error => LanguageHelper.GetField("Error"),
                MessageType.Warning => LanguageHelper.GetField("Warning"),
                MessageType.Info => LanguageHelper.GetField("Normal"),
                _ => LanguageHelper.GetField("None")
            };

            return ViewModelLocator.MainWindowViewModel.NotificationMessageManager.CreateMessage()
                .Accent(GetNotifyBackgroundColor(type)).Animates(true).AnimationInDuration(0.15)
                .AnimationOutDuration(0.15)
                .Background("#333").HasBadge(badge).HasMessage(message)
                .Dismiss().WithDelay(delay).WithButton(new PackIcon {Kind = PackIconKind.Close}, null);
        }

        #endregion
    }
}