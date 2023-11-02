using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using static Heyo.Class.Helper.ColorHelper;

namespace Heyo.Controls
{
    public class MaterialCard : ClippingBorder
    {
        private const double UpDepth = 8; //抬升时高度
        private const double UpRadius = 3; //抬升时阴影半径倍率
        private const double DownDepth = 1; //下降时高度
        private const int ShadowDireection = 260;

        public static readonly DependencyProperty MouseEnterAnimationProperty =
            DependencyProperty.Register(
                "MouseEnterAnimation",
                typeof(bool),
                typeof(MaterialCard),
                new PropertyMetadata(true)
            );

        public static readonly DependencyProperty ShadowProperty =
            DependencyProperty.Register(
                "Shadow",
                typeof(bool),
                typeof(MaterialCard),
                new PropertyMetadata(true, ShadowPropertyChanged)
            );

        private readonly ColorAnimation _backgroundAnim = new ColorAnimation();
        private readonly ColorAnimationUsingKeyFrames _backgroundAnimUsingKey = new ColorAnimationUsingKeyFrames();
        private readonly DoubleAnimation _depthAnim = new DoubleAnimation();
        private readonly PowerEase _easeFunction = new PowerEase {EasingMode = EasingMode.EaseInOut, Power = 4};
        private readonly Ellipse _ellipse = new Ellipse();
        private readonly Ellipse _ellipse2 = new Ellipse();
        private readonly DoubleAnimation _ellipse2OpacityAnim = new DoubleAnimation();
        private readonly DoubleAnimation _ellipse2SizeAnim = new DoubleAnimation {From = 5};
        private readonly ThicknessAnimation _ellipseAnim = new ThicknessAnimation();
        private readonly ThicknessAnimation _fixAnim = new ThicknessAnimation(); //你不知道这是什么的，不用看了
        private readonly ThicknessAnimation _marginAnim = new ThicknessAnimation();
        private readonly DoubleAnimation _opacityAnim = new DoubleAnimation();
        private readonly DoubleAnimation _radiusAnim = new DoubleAnimation();
        protected double DownRadius = 3; //下降时阴影半径(会随自适应改变,默认值为3)

        private SolidColorBrush originBrush;
        protected double ShadowOpacity = 0.3;

        //private bool _shadow = false;
        protected DoubleAnimation SizeAnim = new DoubleAnimation();

        public MaterialCard()
        {
            Loaded += MaterialCard_Loaded;
            UseLayoutRounding = true;

            _backgroundAnim.EasingFunction
                = SizeAnim.EasingFunction
                    = _opacityAnim.EasingFunction
                        = _marginAnim.EasingFunction
                            = _ellipse2SizeAnim.EasingFunction
                                = _fixAnim.EasingFunction
                                    = _ellipse2OpacityAnim.EasingFunction
                                        = _easeFunction;

            _ellipseAnim.EasingFunction =
                _depthAnim.EasingFunction =
                    _radiusAnim.EasingFunction = new PowerEase {EasingMode = EasingMode.EaseOut, Power = 3};

            _ellipse2OpacityAnim.SpeedRatio = AnimationSpeed / 2;
            _opacityAnim.SpeedRatio
                = _ellipse2SizeAnim.SpeedRatio
                    = _fixAnim.SpeedRatio
                        = _backgroundAnimUsingKey.SpeedRatio
                            = SizeAnim.SpeedRatio
                                = _marginAnim.SpeedRatio
                                    = _backgroundAnim.SpeedRatio
                                        = AnimationSpeed;

            _depthAnim.SpeedRatio = _radiusAnim.SpeedRatio = AnimationSpeed * 2;
            _ellipseAnim.SpeedRatio = AnimationSpeed * 1.5;


            _marginAnim.Completed += (s, e) => { UseLayoutRounding = true; };
            SizeAnim.Completed += (s, e) => { UseLayoutRounding = true; };


            MouseEnter += (s, e) => { BeginMouseEnterAnim(); };
            MouseLeave += (s, e) =>
            {
                //Point mouse = Mouse.GetPosition(this);
                //if (mouse.X < 4 || mouse.X > ActualWidth - 4 || mouse.Y < 4 || mouse.Y > ActualHeight - 4)
                //{
                BeginMouseLeaveAnim();
                //}
            };
            MouseDown += (s, e) =>
            {
                if (Shadow && ShadowAnim) BeginShadowAnimation(DownDepth, DownRadius);
            };
            MouseUp += (s, e) =>
            {
                if (Shadow && ShadowAnim) BeginShadowAnimation(UpDepth, DownRadius * UpRadius);
            };
            MouseDown += MaterialCard_MouseDown;


            _ellipse2SizeAnim.From = 3;
            _ellipse2OpacityAnim.From = 1;
            _ellipse2OpacityAnim.To = 0;
            _fixAnim.From = new Thickness(0);

            _ellipse.HorizontalAlignment = HorizontalAlignment.Center;
            _ellipse.VerticalAlignment = VerticalAlignment.Top;
            _ellipse.Margin = new Thickness(0, ActualHeight, 0, 0);
            _ellipse.Effect = new BlurEffect {Radius = 20};
            _ellipse2.HorizontalAlignment = HorizontalAlignment.Left;
            _ellipse2.VerticalAlignment = VerticalAlignment.Top;
            _ellipse2.Width = _ellipse2.Height = 0;
            //ellipse2.Effect = new BlurEffect() { Radius = 20 };
            var grid = new Grid();
            grid.Children.Add(_ellipse);
            grid.Children.Add(_ellipse2);
            base.Child = grid;
            //Shadow = Shadow;
        }

        /// <summary>
        ///     动画速度
        /// </summary>
        public double AnimationSpeed { get; set; } = 2;

        /// <summary>
        ///     阴影动画
        /// </summary>
        public bool ShadowAnim { get; set; } = true;

        /// <summary>
        ///     水波反馈
        /// </summary>
        public bool WaveFeedback { get; set; } = true;

        /// <summary>
        ///     开启阴影自适应
        /// </summary>
        public bool OpenShadowSelfAdaption { get; set; } = true;

        public bool AutoColor { get; set; } = false;
        public Color ActiveColor { get; set; } = Color.FromArgb(64, 0, 0, 0);
        public Color WaveColor { get; set; } = Color.FromArgb(128, 0, 0, 0);

        /// <summary>
        ///     鼠标移入移出动画
        /// </summary>
        public bool MouseEnterAnimation
        {
            get => (bool) GetValue(MouseEnterAnimationProperty);
            set
            {
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(MouseEnterAnimationProperty, Shadow, value));
                SetValue(MouseEnterAnimationProperty, value);
            }
        }

        public bool Shadow
        {
            get => (bool) GetValue(ShadowProperty);
            set => SetValue(ShadowProperty, value);
        }

        public new Brush Background
        {
            get => base.Background;
            set
            {
                if (value is SolidColorBrush)
                {
                    if (base.Background == null) base.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                    _backgroundAnim.To = (value as SolidColorBrush).Color;
                    _backgroundAnim.Completed += (s, e) => { ShadowSelfAdaption(); };
                    base.Background.BeginAnimation(SolidColorBrush.ColorProperty, _backgroundAnim);
                }
            }
        }

        public new Thickness Margin
        {
            get => Margin;
            set
            {
                _marginAnim.To = value;
                UseLayoutRounding = false;
                BeginAnimation(MarginProperty, _marginAnim);
            }
        }

        public new double Height
        {
            get => base.Height;
            set
            {
                SizeAnim.To = value;
                UseLayoutRounding = false;
                BeginAnimation(HeightProperty, SizeAnim);
            }
        }

        public new double Width
        {
            get => base.Width;
            set
            {
                SizeAnim.To = value;
                UseLayoutRounding = false;
                BeginAnimation(WidthProperty, SizeAnim);
            }
        }

        public new UIElement Child
        {
            get
            {
                if (((Grid) base.Child).Children.Count > 2) return ((Grid) base.Child).Children[2];
                return null;
            }
            set
            {
                if (((Grid) base.Child).Children.Count < 3)
                    ((Grid) base.Child).Children.Add(value);
                else
                    ((Grid) base.Child).Children[2] = value;
            }
        }

        public new double Opacity
        {
            get => base.Opacity;
            set
            {
                _opacityAnim.To = value;
                BeginAnimation(OpacityProperty, _opacityAnim);
            }
        }

        private void MaterialCard_Loaded(object sender, RoutedEventArgs e)
        {
            if (Background is SolidColorBrush)
                originBrush = (base.Background as SolidColorBrush).Clone();
            else if (Background == null) originBrush = new SolidColorBrush(Colors.Transparent);
            ShadowSelfAdaption();
        }

        /// <summary>
        ///     阴影自适应
        /// </summary>
        private void ShadowSelfAdaption()
        {
            if (Effect == null && Shadow)
                Effect = new DropShadowEffect
                {
                    BlurRadius = DownRadius,
                    Opacity = ShadowOpacity,
                    ShadowDepth = DownDepth,
                    Direction = ShadowDireection
                };
            if (Background != null && OpenShadowSelfAdaption)
            {
                Color c;
                if (Background is SolidColorBrush)
                    c = ((SolidColorBrush) Background).Color;
                else if (Background is LinearGradientBrush)
                    c = ((LinearGradientBrush) Background).GradientStops[0].Color;
                else
                    return;
                var opacity = 0.0004 * (765D - c.R - c.G - c.B) + 0.1;
                DownRadius = 0.01 * (765D - c.R - c.G - c.B) + 3;
                if (Effect is DropShadowEffect effect)
                {
                    effect.Opacity = opacity;
                    effect.BlurRadius = DownRadius;
                }

                //Console.WriteLine(c + "," + DownRadius + "," + opacity);
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        private void BeginMouseEnterAnim()
        {
            if (Effect == null && Shadow && ShadowAnim)
                Effect = new DropShadowEffect
                {
                    BlurRadius = DownRadius,
                    Opacity = ShadowOpacity,
                    ShadowDepth = DownDepth,
                    Direction = ShadowDireection
                };
            if (Shadow && ShadowAnim) BeginShadowAnimation(UpDepth, DownRadius * UpRadius);

            if (MouseEnterAnimation)
            {
                var c = new Color();
                var flag = false;
                var ca = new ColorAnimation
                {
                    SpeedRatio = AnimationSpeed * 1.5,
                    EasingFunction = new PowerEase {EasingMode = EasingMode.EaseOut, Power = 4}
                };
                if (AutoColor)
                {
                    if (Background is SolidColorBrush brush)
                    {
                        flag = true;
                        c = brush.Color;
                        base.Background = new SolidColorBrush(new Color {A = c.A, R = c.R, G = c.G, B = c.B});
                    }
                    else if (BorderBrush is SolidColorBrush colorBrush && Background == null)
                    {
                        flag = true;
                        c = colorBrush.Color;
                        Background = new SolidColorBrush();
                    }
                }
                else if (ActiveColor != null)
                {
                    c = ActiveColor;
                    flag = true;
                }


                if (flag)
                {
                    var cHsv = c.ToHSV();
                    cHsv.S = (int) Math.Min(255, cHsv.S * 1.4);
                    var cTo = cHsv.ToRgb().ToMedia();
                    cTo.A = c.A;
                    ca.To = cTo;
                    if (Background == null) return;
                    if (Background is SolidColorBrush background)
                    {
                        if (background.IsFrozen) background = new SolidColorBrush(background.Color);
                        background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                    }

                    //originBrush = Background.Clone() as SolidColorBrush;
                }

                /*ellipse.Fill = new SolidColorBrush(new Color() { A = (byte)(Math.Min(((byte)1.2 * c.A) + 40, 255)), R = (byte)(c.R * 0.93), B = (byte)(c.B * 0.93), G = (byte)(c.G * 0.93) });

                ellipse.Width = ellipse.Height = Math.Max(ActualWidth, ActualHeight) * 1.6;
                //ellipseAnim.From = new Thickness(0, ActualHeight, 0, 0);
                ellipseAnim.To = new Thickness(0, -(Math.Max(ActualHeight, ActualWidth) * 0.8 - ActualHeight * 0.5), 0, 0);
                ellipse.BeginAnimation(MarginProperty, ellipseAnim);*/
            }
        }

        private void BeginMouseLeaveAnim()
        {
            if (Shadow && ShadowAnim) BeginShadowAnimation(DownDepth, DownRadius);
            if (MouseEnterAnimation)
            {
                /*ellipse.Fill = new SolidColorBrush(new Color() { A = (byte)(Math.Min(((byte)1.2 * c.A) + 40, 255)), R = (byte)(c.R * 0.93), B = (byte)(c.B * 0.93), G = (byte)(c.G * 0.93) });
                ellipse.Width = ellipse.Height = Math.Max(ActualWidth, ActualHeight) * 1.6;
                ellipseAnim.To = new Thickness(0, ActualHeight, 0, 0);
                //ellipseAnim.From = new Thickness(0, -(Math.Max( ActualHeight,ActualWidth)*0.8- ActualHeight * 0.5), 0, 0);
                ellipse.BeginAnimation(MarginProperty, ellipseAnim);*/
                if (originBrush == null) return;
                if (Background == null) return;
                var ca = new ColorAnimation
                {
                    To = originBrush.Color,
                    SpeedRatio = AnimationSpeed * 2,
                    EasingFunction = new PowerEase {EasingMode = EasingMode.EaseOut, Power = 4}
                };
                if (Background is SolidColorBrush background)
                {
                    if (background.IsFrozen) background = new SolidColorBrush(background.Color);
                    background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                }
            }
        }

        private void BeginShadowAnimation(double depth, double radius)
        {
            _depthAnim.To = depth;
            _radiusAnim.To = radius;
            ((DropShadowEffect) Effect).BeginAnimation(DropShadowEffect.ShadowDepthProperty, _depthAnim);
            ((DropShadowEffect) Effect).BeginAnimation(DropShadowEffect.BlurRadiusProperty, _radiusAnim);
        }

        public void SetSizeWithOutAnim(double width, double height)
        {
            base.Height = height;
            base.Width = width;
        }

        private void MaterialCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WaveFeedback)
            {
                var radius = Math.Max(ActualHeight, ActualWidth) * 2;
                _ellipse2SizeAnim.To = radius;
                var p = e.GetPosition((IInputElement) sender);
                _fixAnim.To = new Thickness(-radius / 2, -radius / 2, 0, 0);
                var c = new Color();

                if (!AutoColor)
                {
                    c = WaveColor;
                }
                else
                {
                    if (Background is SolidColorBrush brush)
                    {
                        var background = brush;
                        c = background.Color;
                        background = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
                        /*SolidColorBrush background = (Background as SolidColorBrush);
                        originBrush = background.Clone();
                        Color c3 = originBrush.Color;
                        Color c2 = Color.FromArgb(255, (byte)(c.R * 0.96), (byte)(c.G * 0.96), (byte)(c.B * 0.96));
                        backgroundAnimUsingKey.KeyFrames.Clear();
                        backgroundAnimUsingKey.KeyFrames.Add(new EasingColorKeyFrame(c2) { EasingFunction = easeFunction });
                        backgroundAnimUsingKey.KeyFrames.Add(new EasingColorKeyFrame(c) { EasingFunction = easeFunction });
                        background = new SolidColorBrush(Color.FromArgb(c3.A, c3.R, c3.G, c3.B));//防冻结
                        background.BeginAnimation(SolidColorBrush.ColorProperty, backgroundAnimUsingKey);*/
                    }
                    else if (BorderBrush is SolidColorBrush colorBrush)
                    {
                        c = colorBrush.Color;
                    }

                    var cHsv = c.ToHSV();
                    cHsv.V = (int) (cHsv.V * 0.9);
                    c = cHsv.ToRgb().ToMedia();
                }


                //ellipse2.Fill = new SolidColorBrush(new Color() { A = (byte)(Math.Min(((byte)1.2 * c.A), 255) + 40), R = (byte)(c.R * 0.5), B = (byte)(c.B * 0.5), G = (byte)(c.G * 0.5) });
                _ellipse2.Fill = new SolidColorBrush(c);
                _ellipse2.RenderTransform = new TranslateTransform(p.X, p.Y);
                _ellipse2.BeginAnimation(WidthProperty, _ellipse2SizeAnim);
                _ellipse2.BeginAnimation(HeightProperty, _ellipse2SizeAnim);
                _ellipse2.BeginAnimation(OpacityProperty, _ellipse2OpacityAnim);
                _ellipse2.BeginAnimation(MarginProperty, _fixAnim);
            }
        }

        private static void ShadowPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var card = (MaterialCard) sender;
            if (!(bool) args.NewValue)
                card.Effect = null;
            else
                card.ShadowSelfAdaption();
        }
    }
}