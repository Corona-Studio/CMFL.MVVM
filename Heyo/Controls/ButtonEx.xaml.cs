using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Heyo.Controls
{
    /// <summary>
    ///     Button.xaml 的交互逻辑
    /// </summary>
    public partial class ButtonEx : ClippingBorder
    {
        private Brush backgroundTemp, borderTemp;

        private Thickness marginTemp, paddingTemp;

        //private Storyboard changeTo1, changeTo2, changeTo3;
        public ButtonEx()
        {
            InitializeComponent();
            Loaded += ShapeButton_Loaded;
        }

        public new Brush Background
        {
            get => base.Background;
            set
            {
                base.Background = value;
                backgroundTemp = value.Clone();
            }
        }

        public double ImageHeight
        {
            get => ImageHost.Height;
            set => ImageHost.Height = value;
        }

        public double ImageWidth
        {
            get => ImageHost.Width;
            set => ImageHost.Width = value;
        }

        public Thickness TextPadding
        {
            get => ContentView.Margin;
            set => ContentView.Margin = value;
        }

        //public bool AutoColour { get; set; } = true;
        public Thickness TextMargin
        {
            get => TextHost.Margin;
            set => TextHost.Margin = value;
        }

        public UIElementCollection Content
        {
            get => grid.Children;
            set
            {
                foreach (var item in value) grid.Children.Add((UIElement) item);
            }
        }

        public bool MarginAnimation { get; set; } = false;
        public bool PaddingAnimation { get; set; } = false;
        public bool BackgroundAnimation { get; set; } = false;
        public bool BorderAnimation { get; set; } = false;
        public double EllipseSize { get; set; } = 500;

        public Brush EllipseColor
        {
            get => ellipse.Fill;
            set => ellipse.Fill = value;
        }

        public int AnimationSpeed { get; set; } = 8;
        public int Power { get; set; } = 10;

        public Brush Foreground
        {
            get => Line1.Foreground;
            set => Line2.Foreground = Line1.Foreground = value;
        }

        public string TextLine1
        {
            get => Line1.Text;
            set
            {
                Line1.Text = value;
                if (value.Length < 1)
                    Line1.Visibility = Visibility.Collapsed;
                else
                    Line2.Visibility = Visibility.Visible;
            }
        }

        public string TextLine2
        {
            get => Line2.Text;
            set
            {
                Line2.Text = value;
                if (value.Length < 1)
                    Line2.Visibility = Visibility.Collapsed;
                else
                    Line2.Visibility = Visibility.Visible;
            }
        }

        public ImageSource Image
        {
            get => ImageHost.Source;
            set => ImageHost.Source = value;
        }

        public double FontSize1
        {
            get => Line1.FontSize;
            set => Line1.FontSize = value;
        }

        public double FontSize2
        {
            get => Line2.FontSize;
            set => Line2.FontSize = value;
        }

        public FontFamily FontFamily
        {
            get => Line1.FontFamily;
            set => Line2.FontFamily = Line1.FontFamily = value;
        }

        public Brush MouseOnBackground { get; set; }

        //public Brush MouseOnForeground { get; set; }
        public Brush MouseOnBorder { get; set; }
        public Thickness MouseOnMargin { get; set; }
        public Thickness MouseOnPadding { get; set; }
        public event RoutedEventHandler Click;

        private void ShapeButton_Loaded(object sender, RoutedEventArgs e)
        {
            backgroundTemp = Background == null ? null : Background.Clone();
            borderTemp = BorderBrush;
            marginTemp = Margin;
            paddingTemp = Padding;

            if (Line2.Text.Length < 1)
                Line2.Visibility = Visibility.Collapsed;
            if (Line1.Text.Length < 1)
                Line1.Visibility = Visibility.Collapsed;
        }

        public void ClickAnimation()
        {
            var da = new DoubleAnimation();
            da.SpeedRatio = 3;
            da.To = EllipseSize;
            da.From = 0;
            var pe = new PowerEase();
            pe.EasingMode = EasingMode.EaseIn;
            pe.Power = 1.2;
            da.EasingFunction = pe;
            if (ellipse.Fill == null)
            {
                if (Background is SolidColorBrush)
                {
                    var c = (Background as SolidColorBrush).Color;
                    ellipse.Fill = new SolidColorBrush(new Color
                        {A = 255, R = (byte) (c.R * 0.8), B = (byte) (c.B * 0.8), G = (byte) (c.G * 0.8)});
                }
                else
                {
                    return;
                }
            }

            ellipse.BeginAnimation(WidthProperty, da);
            ellipse.BeginAnimation(HeightProperty, da);
            var dakf = new DoubleAnimationUsingKeyFrames();
            dakf.KeyFrames.Add(new EasingDoubleKeyFrame(0.1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            dakf.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));
            dakf.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300))));
            ellipse.BeginAnimation(OpacityProperty, dakf);
        }

        private void button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClickAnimation();
            Click?.Invoke(this, new RoutedEventArgs());
        }

        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
            /*if(AutoColour)
            {
                if(Background is SolidColorBrush)
                {
                    //SolidColorBrush scb = Background as SolidColorBrush;
                    //MouseOnBackground =new SolidColorBrush( Color.FromArgb(scb.Color.A, (byte)Math.Min(scb.Color.R*1, 255), (byte)Math.Min(scb.Color.G*1.1,255), (byte)Math.Min(scb.Color.B*0.9, 255)));
                    //EllipseColor = new SolidColorBrush(Color.FromArgb(255, (byte)Math.Min(scb.Color.R*1, 255), (byte)Math.Min(scb.Color.G*1.1, 255), (byte)Math.Min(scb.Color.B*0.9, 255)));
                }
                
            }*/

            if (Background is SolidColorBrush && BackgroundAnimation)
            {
                var ca = new ColorAnimation();
                ca.SpeedRatio = AnimationSpeed;
                if (MouseOnBackground is SolidColorBrush)
                {
                    ca.To = (MouseOnBackground as SolidColorBrush).Color;
                    //backgroundTemp = Background == null ? null : Background.Clone();
                }
                else if (MouseOnBackground == null)
                {
                    var c = (Background as SolidColorBrush).Color;
                    ca.To = new Color
                        {A = c.A, G = (byte) (c.G * 0.87), R = (byte) (c.R * 0.87), B = (byte) (c.B * 0.87)};
                }

                var c1 = (Background as SolidColorBrush).Clone().Color;
                Background = new SolidColorBrush(c1); //防止被冻结
                Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }

            if (BorderBrush is SolidColorBrush && BorderAnimation)
            {
                var ca = new ColorAnimation();
                ca.SpeedRatio = AnimationSpeed;
                if (MouseOnBorder is SolidColorBrush)
                {
                    ca.To = (MouseOnBorder as SolidColorBrush).Color;
                }
                else if (MouseOnBorder == null)
                {
                    var c = (BorderBrush as SolidColorBrush).Color;
                    ca.To = new Color
                        {A = c.A, G = (byte) (c.G * 0.87), R = (byte) (c.R * 0.87), B = (byte) (c.B * 0.87)};
                }

                BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }

            if (MouseOnPadding != null && PaddingAnimation)
            {
                var t = new ThicknessAnimation();
                t.To = MouseOnPadding;
                t.SpeedRatio = AnimationSpeed / 2;
                var pe = new PowerEase();
                pe.Power = Power;
                t.EasingFunction = new PowerEase();
                BeginAnimation(PaddingProperty, t);
            }

            if (MouseOnMargin != null && MarginAnimation)
            {
                var t = new ThicknessAnimation();
                t.To = MouseOnMargin;
                t.SpeedRatio = AnimationSpeed / 2;
                var pe = new PowerEase();
                pe.Power = Power;
                t.EasingFunction = new PowerEase();
                BeginAnimation(MarginProperty, t);
            }
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (BackgroundAnimation && backgroundTemp is SolidColorBrush)
            {
                var ca = new ColorAnimation();
                ca.To = (backgroundTemp as SolidColorBrush).Color;
                ca.SpeedRatio = AnimationSpeed;
                Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }

            if (BorderAnimation && borderTemp is SolidColorBrush)
            {
                var ca2 = new ColorAnimation();
                ca2.To = (borderTemp as SolidColorBrush).Color;
                ca2.SpeedRatio = AnimationSpeed;
                BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca2);
            }

            if (PaddingAnimation && paddingTemp != null)
            {
                var t = new ThicknessAnimation();
                t.To = paddingTemp;
                t.SpeedRatio = AnimationSpeed / 2;
                var pe = new PowerEase();
                pe.Power = Power;
                t.EasingFunction = new PowerEase();
                BeginAnimation(PaddingProperty, t);
            }

            if (MarginAnimation && marginTemp != null)
            {
                var t2 = new ThicknessAnimation();
                t2.To = marginTemp;
                t2.SpeedRatio = AnimationSpeed / 2;
                var pe = new PowerEase();
                pe.Power = Power;
                t2.EasingFunction = new PowerEase();
                BeginAnimation(MarginProperty, t2);
            }
        }
    }
}