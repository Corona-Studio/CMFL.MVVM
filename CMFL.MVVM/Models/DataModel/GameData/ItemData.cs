using CMFL.MVVM.Interface;

namespace CMFL.MVVM.Models.DataModel.GameData
{
    public class ItemData : IAsset
    {
        private string _absolutelyFilePath;

        /// <summary>
        ///     文件名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     目录绝对路径
        /// </summary>
        public string AbsolutelyDirectory { get; set; }

        /// <summary>
        ///     文件绝对路径
        /// </summary>
        public string AbsolutelyFilePath
        {
            get => _absolutelyFilePath;
            set
            {
                _absolutelyFilePath = value;
                var jarName = value.Substring(value.LastIndexOf('\\') + 1);
                Name = jarName.Substring(0, jarName.Length - 4);
                AbsolutelyDirectory = value.Replace(jarName, "");
            }
        }

        public string FileLocation { get; set; }
    }
}