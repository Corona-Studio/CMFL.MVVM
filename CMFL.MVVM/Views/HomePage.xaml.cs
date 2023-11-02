using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CMFL.MVVM.ViewModels;

namespace CMFL.MVVM.Views
{
    /// <summary>
    ///     HomeView.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        private bool _alreadyLoaded;

        public HomePage()
        {
            InitializeComponent();

            if (ViewOperationBase.IsInDesignMode)
                return;

            ViewModelLocator.GamePageViewModel.GetAllLocalGames();
        }

        private void HomePage_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.HomePageViewModel.Monitoring = true;
            Task.Run(async () =>
            {
                if (!_alreadyLoaded)
                {
                    await Task.Delay(250).ConfigureAwait(true);
                    ViewModelLocator.HomePageViewModel.GetMcbbsNews();
                    BulletinBoard.GetBulletin();

                    _alreadyLoaded = true;
                }

                ViewModelLocator.GameTimeCounterViewModel.GetGameTime();
            });
        }

        private void HomePage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.HomePageViewModel.Monitoring = false;
        }

        private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var sv = (ScrollViewer) sender;
            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}