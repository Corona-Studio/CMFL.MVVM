using System;
using System.Security.Cryptography;
using System.Text;

namespace Heyo.Class.Helper.Encryption
{
    public class SHA1
    {
        /// <summary>
        ///     将字符串进行SHA1加密，返回8位无符号数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns>返回8位无符号数组</returns>
        public static byte[] EncryptToSHA1(string str)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var str1 = Encoding.UTF8.GetBytes(str);
            var str2 = sha1.ComputeHash(str1);
            sha1.Clear();
            (sha1 as IDisposable).Dispose();
            return str2;
        }

        /// <summary>
        ///     将字符串进行SHA1加密，返回字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns>返回字符串</returns>
        public static string EncryptToSHA1string(string str)
        {
            var bytHash = EncryptToSHA1(str);
            var sTemp = "";
            for (var i = 0; i < bytHash.Length; i++) sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            return sTemp.ToLower();
        }
    }
}