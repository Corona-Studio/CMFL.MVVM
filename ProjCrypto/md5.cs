using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjCrypto
{
    internal class md5
    {
        /// <summary>
        ///     取文件的MD5值
        /// </summary>
        /// <param name="fileData">文件数据流</param>
        /// <returns>MD5</returns>
        public static string GetMD5FromFile(FileStream fileData)
        {
            using var md5 = new MD5CryptoServiceProvider();
            var retVal = md5.ComputeHash(fileData);
            fileData.Close();
            var sb = new StringBuilder();
            foreach (var t in retVal)
                sb.Append(t.ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        ///     取字符的MD5值
        /// </summary>
        /// <param name="text">字符数据流</param>
        /// <returns>MD5</returns>
        public static string GetMD5FromText(string text)
        {
            using var md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(Encoding.Default.GetBytes(text));
            return Encoding.Default.GetString(result);
        }
    }
}