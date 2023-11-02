using System;
using System.Drawing;
using System.Runtime.InteropServices;
using CMFL.MVVM.Class.Helper.Launcher;
using CMFL.MVVM.Class.Helper.Other;

namespace CMFL.MVVM.Class.Helper.Graphic
{
    public static class ImageEffects
    {
        private static readonly Guid BlurEffectGuid = new Guid("{633C80A4-1843-482B-9EF2-BE2834C5FDD4}");
        private static readonly Guid UsmSharpenEffectGuid = new Guid("{63CBF3EE-C526-402C-8F71-62C540BF5142}");

        //[DllImport("GaussBlur.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true)]
        //private static extern void GaussBlur(byte* Src, byte* Dest, int Width, int Height, int Stride, float Radius);

        //[DllImport("GaussBlur.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true)]
        //private static extern void GaussBlur_SSE(byte* Src, byte* Dest, int Width, int Height, int Stride, float Radius);

        private static IntPtr NativeHandle(this Bitmap bmp)
        {
            return bmp.GetPrivateField<IntPtr>("nativeImage");
            /*  用Reflector反编译System.Drawing.Dll可以看到Image类有如下的私有字段
                internal IntPtr nativeImage;
                private byte[] rawData;
                private object userData;
                然后还有一个 SetNativeImage函数
                internal void SetNativeImage(IntPtr handle)
                {
                    if (handle == IntPtr.Zero)
                    {
                        throw new ArgumentException(SR.GetString("NativeHandle0"), "handle");
                    }
                    this.nativeImage = handle;
                }
                这里在看看FromFile等等函数，其实也就是调用一些例如GdipLoadImageFromFile之类的GDIP函数，并把返回的GDIP图像句柄
                通过调用SetNativeImage赋值给变量nativeImage，因此如果我们能获得该值，就可以调用VS2010暂时还没有封装的GDIP函数
                进行相关处理了，并且由于.NET肯定已经初始化过了GDI+，我们也就无需在调用GdipStartup初始化他了。
             */
        }

        /// <summary>
        ///     对图像进行高斯模糊,参考：http://msdn.microsoft.com/en-us/library/ms534057(v=vs.85).aspx
        /// </summary>
        /// <param name="bmp">Bmp</param>
        /// <param name="rect">需要模糊的区域，会对该值进行边界的修正并返回.</param>
        /// <param name="radius">指定高斯卷积核的半径，有效范围[0，255],半径越大，图像变得越模糊.</param>
        /// <param name="expandEdge">指定是否对边界进行扩展，设置为True，在边缘处可获得较为柔和的效果. </param>
        public static void SlowGaussianBlur(this Bitmap bmp, ref Rectangle rect, float radius = 10,
            bool expandEdge = false)
        {
            BlurParameters blurPara;
            if (radius < 0 || radius > 255)
                throw new ArgumentOutOfRangeException(LanguageHelper.GetField("ArgumentNotCorrect"));

            blurPara.Radius = radius;
            blurPara.ExpandEdges = expandEdge;
            var result = NativeMethods.GdipCreateEffect(BlurEffectGuid, out var blurEffect);
            if (result == 0)
            {
                var handle = Marshal.AllocHGlobal(Marshal.SizeOf(blurPara));
                Marshal.StructureToPtr(blurPara, handle, true);
                _ = NativeMethods.GdipSetEffectParameters(blurEffect, handle, (uint) Marshal.SizeOf(blurPara));
                _ = NativeMethods.GdipBitmapApplyEffect(bmp.NativeHandle(), blurEffect, ref rect, false,
                    IntPtr.Zero,
                    0);
                // 使用GdipBitmapCreateApplyEffect函数可以不改变原始的图像，而把模糊的结果写入到一个新的图像中
                _ = NativeMethods.GdipDeleteEffect(blurEffect);
                Marshal.FreeHGlobal(handle);
            }
            else
            {
                throw new ExternalException(LanguageHelper.GetField("GDIVersionNotCorrect"));
            }
        }

        /// <summary>
        ///     对图像进行锐化,参考：http://msdn.microsoft.com/en-us/library/ms534073(v=vs.85).aspx
        /// </summary>
        /// <param name="bmp">Bmp</param>
        /// <param name="rect">需要锐化的区域，会对该值进行边界的修正并返回.</param>
        /// <param name="radius">指定高斯卷积核的半径，有效范围[0，255],因为这个锐化算法是以高斯模糊为基础的，所以他的速度肯定比高斯模糊慢</param>
        /// <param name="amount"></param>
        [Obsolete]
        public static void UsmSharpen(this Bitmap bmp, ref Rectangle rect, float radius = 10, float amount = 50f)
        {
            SharpenParams sharpenParams;
            if (radius < 0 || radius > 255)
                throw new ArgumentOutOfRangeException(LanguageHelper.GetField("ArgumentNotCorrect"));

            if (amount < 0 || amount > 100)
                throw new ArgumentOutOfRangeException(LanguageHelper.GetField("ArgumentNotCorrect"));

            sharpenParams.Radius = radius;
            sharpenParams.Amount = amount;
            var result = NativeMethods.GdipCreateEffect(UsmSharpenEffectGuid, out var unSharpMaskEffect);
            if (result == 0)
            {
                var handle = Marshal.AllocHGlobal(Marshal.SizeOf(sharpenParams));
                Marshal.StructureToPtr(sharpenParams, handle, true);
                _ = NativeMethods.GdipSetEffectParameters(unSharpMaskEffect, handle,
                    (uint) Marshal.SizeOf(sharpenParams));
                _ = NativeMethods.GdipBitmapApplyEffect(bmp.NativeHandle(), unSharpMaskEffect, ref rect, false,
                    IntPtr.Zero,
                    0);
                _ = NativeMethods.GdipDeleteEffect(unSharpMaskEffect);
                Marshal.FreeHGlobal(handle);
            }
            else
            {
                throw new ExternalException(LanguageHelper.GetField("GDIVersionNotCorrect"));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BlurParameters
        {
            internal float Radius;
            internal bool ExpandEdges;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SharpenParams
        {
            internal float Radius;
            internal float Amount;
        }

        internal enum PaletteType // GDI+1.1还可以针对一副图像获取某种特殊的调色
        {
            PaletteTypeCustom = 0,
            PaletteTypeOptimal = 1,
            PaletteTypeFixedBW = 2,
            PaletteTypeFixedHalftone8 = 3,
            PaletteTypeFixedHalftone27 = 4,
            PaletteTypeFixedHalftone64 = 5,
            PaletteTypeFixedHalftone125 = 6,
            PaletteTypeFixedHalftone216 = 7,
            PaletteTypeFixedHalftone252 = 8,
            PaletteTypeFixedHalftone256 = 9
        }

        internal enum DitherType // 这个主要用于将真彩色图像转换为索引图像，并尽量减低颜色损失
        {
            DitherTypeNone = 0,
            DitherTypeSolid = 1,
            DitherTypeOrdered4x4 = 2,
            DitherTypeOrdered8x8 = 3,
            DitherTypeOrdered16x16 = 4,
            DitherTypeOrdered91x91 = 5,
            DitherTypeSpiral4x4 = 6,
            DitherTypeSpiral8x8 = 7,
            DitherTypeDualSpiral4x4 = 8,
            DitherTypeDualSpiral8x8 = 9,
            DitherTypeErrorDiffusion = 10
        }
    }
}