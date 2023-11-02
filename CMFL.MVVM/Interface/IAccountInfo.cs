namespace CMFL.MVVM.Interface
{
    public interface IAccountInfo
    {
        /// <summary>
        ///     游戏名
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        ///     是否为选定账户
        /// </summary>
        bool Selected { get; set; }

        /// <summary>
        ///     保存
        /// </summary>
        /// <param name="accounts"></param>
        void Save();
    }
}