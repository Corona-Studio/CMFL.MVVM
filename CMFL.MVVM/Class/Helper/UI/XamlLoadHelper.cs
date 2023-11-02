using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace CMFL.MVVM.Class.Helper.UI
{
    public static class XamlLoadHelper
    {
        public static DependencyObject LoadXamlFromFile(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            try
            {
                return (DependencyObject) XamlReader.Load(fs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static DependencyObject LoadXamlFromString(string content)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                using var memoryStream = new MemoryStream(bytes);
                var dependencyObject = (DependencyObject) XamlReader.Load(memoryStream);
                return dependencyObject;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}