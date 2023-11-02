namespace CMFL.MVVM.Class.Helper.Launcher
{
    /// <summary>
    ///     彩蛋23333
    /// </summary>
    public static class EasterEgg
    {
        /// <summary>
        ///     彩蛋类型
        /// </summary>
        public enum EasterEggType
        {
            /// <summary>
            ///     ToolTip彩蛋
            /// </summary>
            ToolTipEgg,

            /// <summary>
            ///     其他类型
            /// </summary>
            Other
        }

        /// <summary>
        ///     彩蛋顺序
        /// </summary>
        private static int _times = -1;

        private static readonly string[] ToolTipsEasterEgg =
            new string[int.Parse(LanguageHelper.GetField("ToolTipEasterEggCount"))];

        static EasterEgg()
        {
            for (var i = 1; i <= int.Parse(LanguageHelper.GetField("ToolTipEasterEggCount")); i++)
                ToolTipsEasterEgg[i - 1] = LanguageHelper.GetField($"ToolTipEasterEgg{i}");
        }

        /// <summary>
        ///     显示一个彩蛋
        /// </summary>
        /// <returns></returns>
        public static string Show()
        {
            if (_times.Equals(ToolTipsEasterEgg.Length - 1))
                _times = 0;
            else
                _times++;

            return ToolTipsEasterEgg[_times];
        }
    }
}