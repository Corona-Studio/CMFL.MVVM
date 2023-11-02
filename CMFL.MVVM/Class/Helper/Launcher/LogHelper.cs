using System;
using System.IO;
using System.Security;
using System.Text;
using System.Windows;
using CMFL.MVVM.Class.Helper.Launcher.Settings;
using CMFL.MVVM.Class.Helper.Other;

namespace CMFL.MVVM.Class.Helper.Launcher
{
    public static class LogHelper
    {
        public enum LogLevels
        {
            Error,
            Warning,
            Info
        }

        private static readonly object LogLock = new object();
        private static readonly object GameLogLock = new object();

        private static readonly string LauncherLogPath =
            Environment.CurrentDirectory + @"\CMFL\LauncherLog\" + DateTime.Now.ToString("yyyy_MM_dd") +
            " CMFL_Log.txt";

        private static readonly string GameLogPath = Environment.CurrentDirectory + @"\CMFL\GameLog\" +
                                                     DateTime.Now.ToString("yyyy_MM_dd") + " Game_Log.txt";

        #region 向启动器日志文件写入报告（写入一行后换行）

        /// <summary>
        ///     向启动器日志文件写入报告
        /// </summary>
        /// <param name="log"></param>
        /// <param name="levels">日志等级</param>
        /// <param name="logTimeAndLevel">是否记录时间和等级</param>
        public static void WriteLogLine(string log, LogLevels levels, bool logTimeAndLevel = true)
        {
            var fi = new FileInfo(LauncherLogPath);
            if (!fi.Directory.Exists) fi.Directory.Create();

            if (logTimeAndLevel)
                log = levels switch
                {
                    LogLevels.Info => string.Join(" ", "[", LanguageHelper.GetField("Info"), "]", ":", log),
                    LogLevels.Warning => string.Join(" ", "[", LanguageHelper.GetField("Warning"), "]", ":", log),
                    LogLevels.Error => string.Join(" ", "[", LanguageHelper.GetField("Error"), "]", ":", log),
                    _ => log
                };

            try
            {
                lock (LogLock)
                {
                    File.AppendAllText(LauncherLogPath,
                        $"{(logTimeAndLevel ? $"({DateTime.Now:yyyy-MM-dd hh:mm:ss})" : " ")}{log}{Environment.NewLine}");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(LanguageHelper.GetField("WriteLogFileFailedReason1"),
                    LanguageHelper.GetField("LauncherName"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ShowMsgError(ex);
                Environment.Exit(0);
            }
            catch (IOException ex)
            {
                MessageBox.Show(LanguageHelper.GetField("WriteLogFileFailedReason1"),
                    LanguageHelper.GetField("LauncherName"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ShowMsgError(ex);
                Environment.Exit(0);
            }
            catch (SecurityException ex)
            {
                MessageBox.Show(LanguageHelper.GetField("WriteLogFileFailedReason3"),
                    LanguageHelper.GetField("LauncherName"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ShowMsgError(ex);
                Environment.Exit(0);
            }
        }

        #endregion

        #region 游戏日志写入

        /// <summary>
        ///     写入游戏日志
        /// </summary>
        /// <param name="log"></param>
        public static void WriteGameLog(string log)
        {
            var fi = new FileInfo(GameLogPath);
            if (!fi.Directory.Exists) fi.Directory.Create();

            try
            {
                lock (GameLogLock)
                {
                    File.AppendAllText(GameLogPath,
                        DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss    ") + log + Environment.NewLine);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(LanguageHelper.GetField("WriteLogFileFailedReason1"),
                    LanguageHelper.GetField("LauncherName"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ShowMsgError(ex);
                Environment.Exit(0);
            }
            catch (IOException ex)
            {
                MessageBox.Show(LanguageHelper.GetField("WriteLogFileFailedReason2"),
                    LanguageHelper.GetField("LauncherName"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ShowMsgError(ex);
                Environment.Exit(0);
            }
            catch (SecurityException ex)
            {
                MessageBox.Show(LanguageHelper.GetField("WriteLogFileFailedReason3"),
                    LanguageHelper.GetField("LauncherName"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ShowMsgError(ex);
                Environment.Exit(0);
            }
        }

        #endregion

        #region 启动器异常写入

        /// <summary>
        ///     通过WriteLogLine方法向启动器日志文件写入错误
        /// </summary>
        /// <param name="e">异常</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void WriteError(Exception e)
        {
            if (e == null) return;
#if DEBUG
            var s = new StringBuilder();
            s.Append("Error-");
            e.GetAllExceptionString(ref s);
            WriteLogLine(s.ToString(), LogLevels.Error);
            return;
#endif
            if (!SettingsHelper.Settings.DebugMode) return;
            var sb = new StringBuilder();
            sb.Append("Error-");
            e.GetAllExceptionString(ref sb);
            WriteLogLine(sb.ToString(), LogLevels.Error);
        }

        #endregion

        #region 通过MessageBox.Show()方法向用户显示错误信息

        /// <summary>
        ///     通过MessageBox.Show()方法向用户显示错误信息
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="e">异常</param>
        public static void ShowMsgError(Exception e)
        {
            if (e == null) throw new ArgumentNullException(LanguageHelper.GetField("ArgumentCanNotBeNull"));

            MessageBox.Show(e.TargetSite.Name, LanguageHelper.GetField("LauncherName"), MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
            MessageBox.Show(e.Message, LanguageHelper.GetField("LauncherName"), MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
            MessageBox.Show(e.StackTrace, LanguageHelper.GetField("LauncherName"), MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
        }

        #endregion
    }
}