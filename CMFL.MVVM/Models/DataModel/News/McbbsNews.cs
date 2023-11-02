using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Models.DataModel.Launcher;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.News
{
    /// <summary>
    ///     MCBBS新闻数据模型
    /// </summary>
    public class McbbsNews
    {
        /// <summary>
        ///     标题
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     分类
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        ///     时间
        /// </summary>
        [JsonProperty("date")]
        public string Date { get; set; }

        /// <summary>
        ///     地址
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; }

        /// <summary>
        ///     作者
        /// </summary>
        [JsonProperty("author")]
        public string Author { get; set; }

        /// <summary>
        ///     颜色分类
        /// </summary>
        public string ColorClassify => Type switch
        {
            "周边消息" => "#FFFF6666",
            "快讯" => "#FF7043",
            "PE版本资讯" => "#FF44C177",
            "PE快讯" => "#FF37ADFF",
            "逸闻" => "#00E676",
            "Java版本资讯" => "#FDD835",
            "基岩版本资讯" => "#64DD17",
            "主机资讯" => "#F8BBD0",
            "周边产品" => "#FFB2EA1E",
            _ => "#FFBFBFBF"
        };

        public ICommand ViewNews
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    try
                    {
                        Analytics.TrackEvent(AnalyticsEventNames.ClickNewsLink, new Dictionary<string, string>(1)
                        {
                            {"Title", Title},
                            {"Type", Type}
                        });
                        Process.Start(Link);
                    }
                    catch (Win32Exception ex)
                    {
                        NotifyHelper.GetBasicMessageWithBadge(LanguageHelper.GetField("AccessFailed"),
                            NotifyHelper.MessageType.Error).Queue();
                        LogHelper.WriteLogLine(LanguageHelper.GetField("AccessFailed"), LogHelper.LogLevels.Error);
                        LogHelper.WriteError(ex);
                    }
                });
            }
        }
    }
}