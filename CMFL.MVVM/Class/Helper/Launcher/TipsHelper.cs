using ProjBobcat.Class.Helper;

namespace CMFL.MVVM.Class.Helper.Launcher
{
    public static class TipsHelper
    {
        private static readonly string[] TipsArray = new string[int.Parse(LanguageHelper.GetField("TipsCount"))];

        static TipsHelper()
        {
            for (var i = 1; i <= int.Parse(LanguageHelper.GetField("TipsCount")); i++)
                TipsArray[i - 1] = LanguageHelper.GetField($"Tips{i}");
        }

        public static string Show()
        {
            return TipsArray.RandomSample();
        }
    }
}