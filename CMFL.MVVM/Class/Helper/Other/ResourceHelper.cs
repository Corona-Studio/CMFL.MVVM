using System;
using System.IO;
using System.Reflection;
using ProjBobcat.Class.Helper;

namespace CMFL.MVVM.Class.Helper.Other
{
    public static class ResourceHelper
    {
        /// <summary>
        ///     从资源文件中抽取资源文件
        /// </summary>
        /// <param name="resFileName">资源文件名称（资源文件名称必须包含目录，目录间用“.”隔开,最外层是项目默认命名空间）</param>
        /// <param name="outputFile">输出文件</param>
        public static void ExtractResFile(string resFileName, string outputFile)
        {
            var asm = Assembly.GetExecutingAssembly(); //读取嵌入式资源
            using var inStream = new BufferedStream(asm.GetManifestResourceStream(resFileName) ??
                                                    throw new InvalidOperationException());
            FileHelper.SaveBinaryFile(inStream, outputFile);
        }

        public static string FormatResourceName(this string resourceName)
        {
            return resourceName.Replace(" ", "_")
                .Replace("\\", ".")
                .Replace("/", ".");
        }
    }
}