using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace Heyo.Class.Helper
{
    public class ComponentHelper
    {
        /// <summary>
        ///     加载UserControl的XAML文件并返回对应UserControl类型的对象
        /// </summary>
        /// <param name="resourceLocator">XMAL文件的地址(绝对路径)</param>
        /// <returns>对应的UserControl</returns>
        public static T LoadComponent<T>(string resourceLocator) where T : UIElement
        {
            var readerXaml = new StringReader(resourceLocator);

            var fs = new FileStream(resourceLocator, FileMode.Open);
            object comt = LoadComponent<T>(fs);
            fs.Close();
            return comt as T;
        }

        /// <summary>
        ///     加载UserControl的XAML并返回对应UserControl类型的对象
        /// </summary>
        /// <param name="stream">含有XMAL内容的流</param>
        /// <returns>对应的UserControl</returns>
        public static T LoadComponent<T>(Stream stream) where T : UIElement
        {
            //sSystem.Xaml.XamlObjectWriterSettings xows =new System.Xaml.XamlObjectWriterSettings();
            //xows.RootObjectInstance
            var comt = XamlReader.Load(stream);
            return comt as T;
        }
    }
}