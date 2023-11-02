using GalaSoft.MvvmLight.Views;

namespace CMFL.MVVM.Services
{
    public interface IFrameNavigationService : INavigationService
    {
        object Parameter { get; }
    }
}