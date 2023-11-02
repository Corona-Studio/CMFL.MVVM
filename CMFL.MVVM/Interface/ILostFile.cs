namespace CMFL.MVVM.Interface
{
    public interface ILostFile
    {
        /// <summary>
        ///     下载进度
        /// </summary>
        double DownloadProgress { get; set; }

        /// <summary>
        ///     下载目录
        /// </summary>
        string Path { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        ///     文件类型
        /// </summary>
        string Type { get; set; }

        /// <summary>
        ///     Url
        /// </summary>
        string Url { get; set; }
    }
}