using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;

namespace CMFL.MVVM.Class.Helper.UI
{
    public static class FontHelper
    {
        private static readonly List<string> InstalledFontList = GetInstalledFont();

        private static List<string> GetInstalledFont()
        {
            using var installedFonts = new InstalledFontCollection();
            var result = installedFonts.Families.Select(font => font.Name).ToList();
            return result;
        }

        public static int GetFontIndex(string fontName)
        {
            return InstalledFontList.IndexOf(fontName);
        }

        public static List<string> GetInstalledFontList()
        {
            return InstalledFontList;
        }
    }
}