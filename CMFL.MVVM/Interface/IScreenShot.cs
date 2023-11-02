using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CMFL.MVVM.Interface
{
    public interface IScreenShot
    {
        BitmapSource ScreenShotSource { get; set; }
        string ScreenShotPath { get; set; }
        ICommand OpenScreenShotCommand { get; }
    }
}