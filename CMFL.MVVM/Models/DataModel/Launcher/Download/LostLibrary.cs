using CMFL.MVVM.Interface;

namespace CMFL.MVVM.Models.DataModel.Launcher.Download
{
    public class LostLibrary : ILostFile
    {
        /// <summary>
        ///     下载进度
        /// </summary>
        public double DownloadProgress { get; set; }

        /// <summary>
        ///     下载目录
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     文件类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Url
        /// </summary>
        public string Url { get; set; }
    }
}