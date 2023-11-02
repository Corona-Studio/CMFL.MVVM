using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjAurora.Utils
{
    public static class ExtensionsMethods
    {
        public static string ReplaceByDic(this string str, Dictionary<string, string> dic)
        {
            return str == null ? null : dic.Aggregate(str, (tempStr, paraDic) => tempStr.Replace(paraDic.Key, paraDic.Value));
        }

        public static void AppendWithWhitespace(this StringBuilder sb, string content)
        {
            sb.Append($"{content} ");
        }
    }
}