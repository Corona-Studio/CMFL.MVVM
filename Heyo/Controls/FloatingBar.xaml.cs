using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Heyo.Controls
{
    /// <summary>
    ///     FloatingBar.xaml 的交互逻辑
    /// </summary>
    public partial class FloatingBar : UserControl
    {
        public enum Direction
        {
            Top,
            Buttom,
            Left,
            Right
        }

        private readonly Storyboard hide;
        private readonly Storyboard show;

        private bool isShow;

        public FloatingBar()
        {
            InitializeComponent();
            show = FindResource("Show") as Storyboard;
            hide = FindResource("Hide") as Storyboard;
            MouseEnter += (s, e) => { Show(); };
        }

        public UIElementCollection Grid => grid.Children;

        public Thickness TipMargin
        {
            get => TopTip.Margin;
            set => TopTip.Margin = ButtomTip.Margin = LeftTip.Margin = RightTip.Margin = value;
        }

        public Direction TipDirection { get; set; }

        public bool ShowTip { get; set; } = true;

        public void Show()
        {
            if (!isShow)
            {
                isShow = true;
                PreDirection();
                BeginStoryboard(show);
            }
        }

        public void Hide()
        {
            if (isShow)
            {
                isShow = false;
                PreDirection();
                BeginStoryboard(hide);
            }
        }

        private void PreDirection()
        {
            TopTip.Visibility = Visibility.Collapsed;
            ButtomTip.Visibility = Visibility.Collapsed;
            LeftTip.Visibility = Visibility.Collapsed;
            RightTip.Visibility = Visibility.Collapsed;

            Point start, end;
            switch (TipDirection)
            {
                default:
                {
                    start = new Point(0.5, 0);
                    end = new Point(0.5, 1);
                    if (!ShowTip)
                        break;
                    TopTip.Visibility = Visibility.Visible;
                    break;
                }
                case Direction.Top:
                {
                    start = new Point(0.5, 0);
                    end = new Point(0.5, 1);
                    if (!ShowTip)
                        break;
                    TopTip.Visibility = Visibility.Visible;
                    break;
                }
                case Direction.Buttom:
                {
                    start = new Point(0.5, 1);
                    end = new Point(0.5, 0);
                    if (!ShowTip)
                        break;
                    ButtomTip.Visibility = Visibility.Visible;
                    break;
                }
                case Direction.Left:
                {
                    start = new Point(0, 0.5);
                    end = new Point(1, 0.5);
                    if (!ShowTip)
                        break;
                    LeftTip.Visibility = Visibility.Visible;
                    break;
                }
                case Direction.Right:
                {
                    start = new Point(1, 0.5);
                    end = new Point(0, 0.5);
                    if (!ShowTip)
                        break;
                    RightTip.Visibility = Visibility.Visible;
                    break;
                }
            }


            (grid1.OpacityMask as LinearGradientBrush).StartPoint = start;
            (grid1.OpacityMask as LinearGradientBrush).EndPoint = end;
        }
    }
}