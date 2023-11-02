namespace CMFL.MVVM.Interface
{
    public interface IAsset
    {
        string Name { get; set; }
        string AbsolutelyDirectory { get; set; }
        string AbsolutelyFilePath { get; set; }
        string FileLocation { get; set; }
    }
}