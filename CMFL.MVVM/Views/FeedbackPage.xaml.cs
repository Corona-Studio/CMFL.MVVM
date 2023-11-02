using System.Windows.Controls;
using System.Windows.Input;

namespace CMFL.MVVM.Views
{
    /// <summary>
    ///     FeedbackPage.xaml 的交互逻辑
    /// </summary>
    public partial class FeedbackPage : Page
    {
        public FeedbackPage()
        {
            InitializeComponent();
        }

        private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var sv = (ScrollViewer) sender;
            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}