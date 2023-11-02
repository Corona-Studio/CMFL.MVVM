using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjCrypto.Class.Helper
{
    public static class RsaEncrypt
    {
        /// <summary>
        ///     Decrypt the data using the private key
        /// </summary>
        /// <param name="data">The data to decrypt</param>
        /// <param name="keyString"></param>
        /// <param name="keyLength"></param>
        /// <returns>The decrypted data in string format</returns>
        public static string Decrypt(byte[] data, string keyString, int keyLength)
        {
            var rsa = new RSACryptoServiceProvider(1024);
            rsa.FromXmlString(keyString);

            var buffer = new byte[keyLength / 8];
            var pos = 0;
            var copyLength = buffer.Length;

            /*
            var realLength = (int)Math.Ceiling((double)data.Length / keyLength);
            var fixedData = new byte[realLength * keyLength];
            */

            using var ms = new MemoryStream(data.Length);
            while (true)
            {
                Array.Copy(data, pos, buffer, 0, copyLength);

                pos += copyLength;

                var resp = rsa.PublicDecryption(buffer);
                ms.Write(resp, 0, resp.Length);

                Array.Clear(resp, 0, resp.Length);
                Array.Clear(buffer, 0, copyLength);

                // pos >= data.Length
                if (data.Length - pos <= 0)
                    break;
            }

            //Return the decoded data
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static byte[] PublicDecryption(this RSACryptoServiceProvider rsa, byte[] cipherData)
        {
            if (cipherData == null)
                throw new ArgumentNullException("cipherData");
            var numEncData = new BigInteger(cipherData);
            var rsaParams = rsa.ExportParameters(false);
            var exponent = new BigInteger(rsaParams.Exponent);
            var modulus = new BigInteger(rsaParams.Modulus);

            var decData2 = numEncData.modPow(exponent, modulus);
            var data = decData2.getBytes();
            var first = false;
            var bl = new List<byte>();
            foreach (var t in data)
                if (!first && t == 0x00)
                {
                    first = true;
                }
                else if (first)
                {
                    if (t == 0x00) return bl.ToArray();

                    bl.Add(t);
                }

            return bl.Count > 0 ? bl.ToArray() : Array.Empty<byte>();
        }

        public static string EncryptString(string inputString, int dwKeySize,
            string xmlString, RSAEncryptionPadding padding)
        {
            using var rsaCryptoServiceProvider =
                new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            var keySize = dwKeySize / 8;
            var bytes = Encoding.UTF8.GetBytes(inputString);
            var maxLength = keySize - 42;
            var dataLength = bytes.Length;
            var iterations = dataLength / maxLength;
            var preResult = new List<byte>();

            for (var i = 0; i <= iterations; i++)
            {
                var tempBytes = new byte[
                    dataLength - maxLength * i > maxLength ? maxLength : dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0,
                    tempBytes.Length);

                var encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, padding);
                preResult.AddRange(encryptedBytes);
            }

            return Convert.ToBase64String(preResult.ToArray());
        }

        public static string DecryptString(string inputString, int dwKeySize,
            string xmlString, RSAEncryptionPadding padding)
        {
            using var rsaCryptoServiceProvider
                = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            var base64BlockSize = dwKeySize / 8 % 3 != 0 ? dwKeySize / 8 / 3 * 4 + 4 : dwKeySize / 8 / 3 * 4;
            var iterations = inputString.Length / base64BlockSize;
            var arrayList = new ArrayList();
            for (var i = 0; i < iterations; i++)
            {
                var encryptedBytes = Convert.FromBase64String(
                    inputString.Substring(base64BlockSize * i, base64BlockSize));
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(
                    encryptedBytes, padding));
            }

            return Encoding.UTF8.GetString(arrayList.ToArray(typeof(byte)) as byte[]);
        }
    }
}