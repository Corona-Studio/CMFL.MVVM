using System.Runtime.Serialization;

namespace CMFL.MVVM.Models.DataModel.Launcher
{
    /// <summary>
    ///     封禁信息
    /// </summary>
    [DataContract]
    public class BanInfo
    {
        /// <summary>
        ///     BC封禁状态
        /// </summary>
        [DataMember]
        public bool IsBannedBc { get; set; }

        /// <summary>
        ///     工业封禁状态
        /// </summary>
        [DataMember]
        public bool IsBannedGy { get; set; }
    }

    /// <summary>
    ///     用户信息总成
    /// </summary>
    [DataContract]
    public class UsersInfo
    {
        /// <summary>
        ///     用户名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        ///     密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        ///     是否为OP
        /// </summary>
        [DataMember]
        public bool IsOp { get; set; }

        /// <summary>
        ///     邮箱
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        ///     封禁数据
        /// </summary>
        [DataMember]
        public BanInfo BanInfo { get; set; }
    }
}