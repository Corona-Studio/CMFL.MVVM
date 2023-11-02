using System;

namespace ProjCrypto
{
    internal class NoiseGen
    {
        private static double FlatNoise(int x, int y)
        {
            x %= 25;
            y %= 25;
            var n = x + y * 57;
            n = (n << 13) ^ n;
            var o = Convert.ToInt32((n * (n * n * MathLib.ConvertDateTimeInt(DateTime.Now) + 789221) + 1376312589) &
                                    0xFFFFFFFF);
            double oo = o / 1073741824.0f;
            var ooo = 1.0f - oo;
            return ooo;
        }

        /// <summary>
        ///     获取一个2维的平滑噪音
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>噪点值</returns>
        internal static double SmoothNoise(int x, int y)
        {
            var co = (FlatNoise(x - 1, y - 1) + FlatNoise(x + 1, y - 1) + FlatNoise(x - 1, y + 1) +
                      FlatNoise(x + 1, y + 1)) / 16.0f;
            var si = (FlatNoise(x - 1, y) + FlatNoise(x + 1, y) + FlatNoise(x, y - 1) + FlatNoise(x, y + 1)) / 8.0f;
            var ce = FlatNoise(x, y) / 4.0f;
            return co + si + ce;
        }
    }
}