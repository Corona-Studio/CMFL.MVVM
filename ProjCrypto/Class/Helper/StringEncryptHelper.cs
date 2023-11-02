using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjCrypto.Class.Helper
{
    public class StringEncryptHelper
    {
        //默认密钥
        private const string AesKey = "yCsZ#-4a*!XFNuMogmjvdrb?";
        private const string DesKey = "K+DNEnbs";
        private const string EncryptKey = "ILRcMfl2";

        #region 加密字符串

        /// <summary>
        ///     /// 加密字符串
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string StringEncrypt(string str)
        {
            var descsp = new DESCryptoServiceProvider(); //实例化加/解密类对象   

            var key = Encoding.UTF8.GetBytes(EncryptKey); //定义字节数组，用来存储密钥    

            var data = Encoding.UTF8.GetBytes(str); //定义字节数组，用来存储要加密的字符串  

            var mStream = new MemoryStream(); //实例化内存流对象      

            //使用内存流实例化加密流对象   
            using var cStream = new CryptoStream(mStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);

            cStream.Write(data, 0, data.Length); //向加密流中写入数据      

            cStream.FlushFinalBlock(); //释放加密流      

            return Convert.ToBase64String(mStream.ToArray()); //返回加密后的字符串  
        }

        #endregion

        #region 解密字符串

        /// <summary>
        ///     解密字符串
        /// </summary>
        /// <param name="str">要解密的字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string StringDecrypt(string str)
        {
            try
            {
                var descsp = new DESCryptoServiceProvider(); //实例化加/解密类对象    

                var key = Encoding.UTF8.GetBytes(EncryptKey); //定义字节数组，用来存储密钥    

                var data = Convert.FromBase64String(str); //定义字节数组，用来存储要解密的字符串  

                var MStream = new MemoryStream(); //实例化内存流对象      

                //使用内存流实例化解密流对象       
                using var cStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);

                cStream.Write(data, 0, data.Length); //向解密流中写入数据     

                cStream.FlushFinalBlock(); //释放解密流      

                return Encoding.UTF8.GetString(MStream.ToArray()); //返回解密后的字符串  
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

        /// <summary>
        ///     AES加密
        /// </summary>
        /// <param name="value">字符串内容</param>
        /// <param name="_aeskey">AES密钥</param>
        /// <returns></returns>
        public static string AesEncrypt(string value, string _aeskey = null)
        {
            if (string.IsNullOrEmpty(_aeskey)) _aeskey = AesKey;

            var keyArray = Encoding.UTF8.GetBytes(_aeskey);
            var toEncryptArray = Encoding.UTF8.GetBytes(value);

            var rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rDel.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            rDel.Dispose();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///     AES解密
        /// </summary>
        /// <param name="value">字符串内容</param>
        /// <param name="_aeskey">AES密钥</param>
        /// <returns></returns>
        public static string AesDecrypt(string value, string _aeskey = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_aeskey)) _aeskey = AesKey;

                var keyArray = Encoding.UTF8.GetBytes(_aeskey);
                var toEncryptArray = Convert.FromBase64String(value);

                using var rDel = new RijndaelManaged
                {
                    Key = keyArray,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                var cTransform = rDel.CreateDecryptor();
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     DES加密
        /// </summary>
        /// <param name="value">字符串内容</param>
        /// <param name="_deskey">DES密钥</param>
        /// <returns></returns>
        public static string DesEncrypt(string value, string _deskey = null)
        {
            if (string.IsNullOrEmpty(_deskey)) _deskey = DesKey;

            var keyArray = Encoding.UTF8.GetBytes(_deskey);
            var toEncryptArray = Encoding.UTF8.GetBytes(value);

            using var rDel = new DESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rDel.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///     DES解密
        /// </summary>
        /// <param name="value">字符串内容</param>
        /// <param name="deskey">DES密钥</param>
        /// <returns></returns>
        public static string DesDecrypt(string value, string deskey = null)
        {
            try
            {
                if (string.IsNullOrEmpty(deskey)) deskey = DesKey;

                var keyArray = Encoding.UTF8.GetBytes(deskey);
                var toEncryptArray = Convert.FromBase64String(value);

                using var rDel = new DESCryptoServiceProvider
                {
                    Key = keyArray,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                var cTransform = rDel.CreateDecryptor();
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string MD5(string value)
        {
            var result = Encoding.UTF8.GetBytes(value);
            using var md5 = new MD5CryptoServiceProvider();
            var output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }

        public static string HMACMD5(string value, string hmacKey)
        {
            using var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(hmacKey));
            var result = Encoding.UTF8.GetBytes(value);
            var output = hmacsha1.ComputeHash(result);

            return BitConverter.ToString(output).Replace("-", "");
        }

        /// <summary>
        ///     base64编码
        /// </summary>
        /// <returns></returns>
        public static string Base64Encode(string value)
        {
            var result = Convert.ToBase64String(Encoding.Default.GetBytes(value));
            return result;
        }

        /// <summary>
        ///     base64解码
        /// </summary>
        /// <returns></returns>
        public static string Base64Decode(string value)
        {
            var result = Encoding.Default.GetString(Convert.FromBase64String(value));
            return result;
        }
    }
}