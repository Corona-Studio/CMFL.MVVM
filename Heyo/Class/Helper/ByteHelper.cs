using System.Collections.Generic;
using System.Text;

namespace Heyo.Class.Helper
{
    public static class ByteHelper
    {
        /// <summary>
        ///     把多个byte数组合并成一个
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] LinkBytes(params byte[][] bytes)
        {
            var totalLength = 0;
            for (var i = 0; i < bytes.Length; i++) totalLength += bytes[i].Length;
            var result = new byte[totalLength];

            var pointer = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                var l = bytes[i].Length;
                //int start = i == 0 ? 0 : bytes[i - 1].Length;
                for (var i2 = 0; i2 < l; i2++) result[i2 + pointer] = bytes[i][i2];
                pointer += bytes[i].Length;
            }

            return result;
        }

        /// <summary>
        ///     去掉byte[] 中特定的byte
        /// </summary>
        /// <param name="b"> 需要处理的byte[]</param>
        /// <param name="cut">byte[] 中需要除去的特定 byte (此处: byte cut = 0x00 ;) </param>
        /// <returns> 返回处理完毕的byte[] </returns>
        public static byte[] ByteCut(byte[] b, byte cut)
        {
            var list = new List<byte>();
            list.AddRange(b);
            for (var i = list.Count - 1; i >= 0; i--)
                if (list[i] == cut)
                    list.RemoveAt(i);
            var lastbyte = new byte[list.Count];
            for (var i = 0; i < list.Count; i++) lastbyte[i] = list[i];
            return lastbyte;
        }

        public static byte[] ByteCut(this byte[] b, int start, int length)
        {
            var lastbyte = new byte[length];
            for (var i = start; i < length; i++) lastbyte[i - start] = b[i];
            return lastbyte;
        }

        public static byte[] Trim(byte[] buffer)
        {
            var endPointer = 0;
            for (var i = 0; i < buffer.Length; i++)
                if (buffer[i] != 0)
                    endPointer = i;
            var resultBuffer = new byte[endPointer];
            for (var i = 0; i < resultBuffer.Length; i++) resultBuffer[i] = buffer[i];
            return resultBuffer;
        }

        public static string ToString(this byte[] buffers, Encoding encoding)
        {
            return encoding.GetString(buffers);
        }

        public static byte[] ToBytes(this string str)
        {
            byte[] ret;
            ret = Encoding.UTF8.GetBytes(str);
            return ret;
        }
    }
}