using System.Windows;

namespace Heyo.Controls
{
    /// <summary>
    ///     MaterialBoard.xaml 的交互逻辑
    /// </summary>
    public partial class MaterialBoard : MaterialButton
    {
        public MaterialBoard()
        {
            InitializeComponent();

            SizeChanged += MaterialBoard_SizeChanged;
        }

        public object Title
        {
            get => titleLabel.Content;
            set => titleLabel.Content = value;
        }

        public string Intruduction
        {
            get => titleBlcok.Text;
            set => titleBlcok.Text = value;
        }

        private void MaterialBoard_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((Height == double.NaN ? ActualHeight : Height) < 200)
            {
                userLabel.Visibility = Visibility.Collapsed;
                userAvatar.Height = userAvatar.Width = 24;
                Row1.Height = new GridLength(46);
            }
            else
            {
                userLabel.Visibility = Visibility.Visible;
                userAvatar.Height = userAvatar.Width = 36;
                Row1.Height = new GridLength(64);
            }
        }
    }
}