using System;
using System.Windows.Media;

namespace Heyo.Class.Helper
{
    public static class ColorHelper
    {
        /// <summary>
        ///     将System.Drawing.Color转化为System.Windows.Media.Color
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Color ToMedia(this System.Drawing.Color c)
        {
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        /// <summary>
        ///     将System.Windows.Media.Color转化为System.Drawing.Color
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static System.Drawing.Color ToDrawing(this Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public static ColorHSV ToHSV(this Color rgb)
        {
            float min, max, tmp, H, S, V;
            float R = rgb.R * 1.0f / 255, G = rgb.G * 1.0f / 255, B = rgb.B * 1.0f / 255;
            tmp = Math.Min(R, G);
            min = Math.Min(tmp, B);
            tmp = Math.Max(R, G);
            max = Math.Max(tmp, B);
            // H  
            H = 0;
            if (max == min)
                H = 0;
            else if (max == R && G > B)
                H = 60 * (G - B) * 1.0f / (max - min) + 0;
            else if (max == R && G < B)
                H = 60 * (G - B) * 1.0f / (max - min) + 360;
            else if (max == G)
                H = H = 60 * (B - R) * 1.0f / (max - min) + 120;
            else if (max == B) H = H = 60 * (R - G) * 1.0f / (max - min) + 240;
            // S  
            if (max == 0)
                S = 0;
            else
                S = (max - min) * 1.0f / max;
            // V  
            V = max;
            return new ColorHSV((int) H, (int) (S * 255), (int) (V * 255));
        }

        public static ColorHSV ToHSV(this System.Drawing.Color rgb)
        {
            return rgb.ToMedia().ToHSV();
        }

        public static System.Drawing.Color ToRgb(this ColorHSV hsv)
        {
            if (hsv.H == 360) hsv.H = 359; // 360为全黑，原因不明  
            float R = 0f, G = 0f, B = 0f;
            if (hsv.S == 0) return System.Drawing.Color.FromArgb(hsv.A, hsv.V, hsv.V, hsv.V);
            float S = hsv.S * 1.0f / 255, V = hsv.V * 1.0f / 255;
            int H1 = (int) (hsv.H * 1.0f / 60), H = hsv.H;
            var F = H * 1.0f / 60 - H1;
            var P = V * (1.0f - S);
            var Q = V * (1.0f - F * S);
            var T = V * (1.0f - (1.0f - F) * S);
            switch (H1)
            {
                case 0:
                    R = V;
                    G = T;
                    B = P;
                    break;
                case 1:
                    R = Q;
                    G = V;
                    B = P;
                    break;
                case 2:
                    R = P;
                    G = V;
                    B = T;
                    break;
                case 3:
                    R = P;
                    G = Q;
                    B = V;
                    break;
                case 4:
                    R = T;
                    G = P;
                    B = V;
                    break;
                case 5:
                    R = V;
                    G = P;
                    B = Q;
                    break;
            }

            R = R * 255;
            G = G * 255;
            B = B * 255;
            while (R > 255) R -= 255;
            while (R < 0) R += 255;
            while (G > 255) G -= 255;
            while (G < 0) G += 255;
            while (B > 255) B -= 255;
            while (B < 0) B += 255;
            return System.Drawing.Color.FromArgb(hsv.A, (int) R, (int) G, (int) B);
        }

        /// <summary>
        ///     将含有透明度、色调、饱和度、亮度的一组颜色数据转化为System.Drawing.Color
        /// </summary>
        /// <param name="alpha">透明度</param>
        /// <param name="hue">色调</param>
        /// <param name="saturation">饱和度</param>
        /// <param name="brightness">亮度</param>
        /// <returns></returns>
        public static System.Drawing.Color FromAhsv(int alpha, float hue, float saturation, float brightness)
        {
            if (0 > alpha
                || 255 < alpha)
                throw new ArgumentOutOfRangeException(
                    "alpha",
                    alpha,
                    "Value must be within a range of 0 - 255.");

            if (0f > hue
                || 360f < hue)
                throw new ArgumentOutOfRangeException(
                    "hue",
                    hue,
                    "Value must be within a range of 0 - 360.");

            if (0f > saturation
                || 1f < saturation)
                throw new ArgumentOutOfRangeException(
                    "saturation",
                    saturation,
                    "Value must be within a range of 0 - 1.");

            if (0f > brightness
                || 1f < brightness)
                throw new ArgumentOutOfRangeException(
                    "brightness",
                    brightness,
                    "Value must be within a range of 0 - 1.");

            if (0 == saturation)
                return System.Drawing.Color.FromArgb(
                    alpha,
                    Convert.ToInt32(brightness * 255),
                    Convert.ToInt32(brightness * 255),
                    Convert.ToInt32(brightness * 255));

            float fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;

            if (0.5 < brightness)
            {
                fMax = brightness - brightness * saturation + saturation;
                fMin = brightness + brightness * saturation - saturation;
            }
            else
            {
                fMax = brightness + brightness * saturation;
                fMin = brightness - brightness * saturation;
            }

            iSextant = (int) Math.Floor(hue / 60f);
            if (300f <= hue) hue -= 360f;

            hue /= 60f;
            hue -= 2f * (float) Math.Floor((iSextant + 1f) % 6f / 2f);
            if (0 == iSextant % 2)
                fMid = hue * (fMax - fMin) + fMin;
            else
                fMid = fMin - hue * (fMax - fMin);

            iMax = Convert.ToInt32(fMax * 255);
            iMid = Convert.ToInt32(fMid * 255);
            iMin = Convert.ToInt32(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return System.Drawing.Color.FromArgb(alpha, iMid, iMax, iMin);
                case 2:
                    return System.Drawing.Color.FromArgb(alpha, iMin, iMax, iMid);
                case 3:
                    return System.Drawing.Color.FromArgb(alpha, iMin, iMid, iMax);
                case 4:
                    return System.Drawing.Color.FromArgb(alpha, iMid, iMin, iMax);
                case 5:
                    return System.Drawing.Color.FromArgb(alpha, iMax, iMin, iMid);
                default:
                    return System.Drawing.Color.FromArgb(alpha, iMax, iMid, iMin);
            }
        }
    }
}