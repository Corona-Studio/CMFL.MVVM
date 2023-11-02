using Newtonsoft.Json;

namespace CMFL.MVVM.Models.DataModel.BMCLAPI
{
    /// <summary>
    ///     Java下载地址数据模型
    /// </summary>
    public class JavaDownloadLinks
    {
        /// <summary>
        ///     文件标题
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     文件名
        /// </summary>
        [JsonProperty("file")]
        public string File { get; set; }
    }
}