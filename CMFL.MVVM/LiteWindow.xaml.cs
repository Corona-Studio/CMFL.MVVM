using System.Windows;
using System.Windows.Input;
using Heyo;

namespace CMFL.MVVM
{
    /// <summary>
    ///     LiteWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LiteWindow : WindowPlus
    {
        public LiteWindow()
        {
            InitializeComponent();
        }

        private void CurrentWindowMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void MinizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}