using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ProjCrypto
{
    /// <summary>
    ///     加密系统
    /// </summary>
    public static class Cryptosystem
    {
        private const string salt = "Er1cStEVeNsSB23333D0nTEd1TtH1S";

        private static string MD5(string str)
        {
            var textBytes = Encoding.Default.GetBytes(str);
            try
            {
                using var cryptHandler = new MD5CryptoServiceProvider();
                var hash = cryptHandler.ComputeHash(textBytes);
                var ret = "";
                foreach (var a in hash)
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");

                return ret;
            }
            catch
            {
                throw new NullReferenceException();
            }
        }

        public static string PasswordEncryption(string pwd)
        {
            return MD5(pwd + salt);
        }


        public static string ByteArrayToToHexString(IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        public static byte[] Sha512(byte[] content)
        {
            using var sha512 = SHA512.Create();
            var computedHash = sha512.ComputeHash(content);
            return computedHash;
        }

        public static byte[] Sha256(byte[] content)
        {
            using var sha256 = SHA256.Create();
            var computedHash = sha256.ComputeHash(content);
            return computedHash;
        }

        public static byte[] Md5(byte[] content)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var computedHash = md5.ComputeHash(content);
            return computedHash;
        }

        /// <summary>
        ///     静态加密
        /// </summary>
        /// <param name="dataList">含有所有数据的数组（最好事先经过一次md5）</param>
        /// <param name="SALT">盐，不填则自动使用默认值</param>
        /// <returns></returns>
        public static string StaticCrypProcess(string[] dataList, string SALT = "DEf4u1tS417")
        {
            var mixedData = string.Join(SALT, dataList);
            return md5.GetMD5FromText(mixedData);
        }

        private static int SplitSCP(string SCPData)
        {
            var resultStr = string.Empty;
            foreach (var ch in SCPData)
                if (char.IsNumber(ch))
                    resultStr += ch;
                else
                    resultStr += 0;

            return Convert.ToInt32(resultStr);
        }

        public static string DynamicCrypProcess(string SCPData)
        {
            var sScp = SplitSCP(SCPData);
            var sScpL = sScp.ToString().Length;
            return Convert.ToString(NoiseGen.SmoothNoise(sScp, sScpL));
        }
    }
}