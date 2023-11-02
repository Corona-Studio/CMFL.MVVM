using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Heyo.Controls
{
    public class MaterialRipples : ClippingBorder
    {
        private readonly ColorAnimationUsingKeyFrames backgroundAnim = new ColorAnimationUsingKeyFrames();
        private readonly PowerEase easeFunction = new PowerEase {EasingMode = EasingMode.EaseInOut, Power = 4};
        private readonly Ellipse ellipse = new Ellipse();
        private readonly DoubleAnimation ellipseOpacityAnim = new DoubleAnimation();
        private readonly DoubleAnimation ellipseSizeAnim = new DoubleAnimation {From = 5};
        private readonly ThicknessAnimation fixAnim = new ThicknessAnimation(); //你不知道这是什么的，不用看了
        private readonly ThicknessAnimation marginAnim = new ThicknessAnimation();

        private readonly DoubleAnimation radiusAnim = new DoubleAnimation();

        public MaterialRipples()
        {
            UseLayoutRounding = true; //防止模糊

            marginAnim.EasingFunction
                = ellipseSizeAnim.EasingFunction
                    = fixAnim.EasingFunction
                        = ellipseOpacityAnim.EasingFunction
                            = easeFunction;

            radiusAnim.EasingFunction = new PowerEase {EasingMode = EasingMode.EaseOut, Power = 3};

            ellipseOpacityAnim.SpeedRatio = AnimationSpeed / 2;

            MouseDown += MaterialCard_MouseDown;


            ellipseSizeAnim.From = 0;
            ellipseOpacityAnim.From = 1;
            ellipseOpacityAnim.To = 0;
            fixAnim.From = new Thickness(0);


            ellipse.HorizontalAlignment = HorizontalAlignment.Left;
            ellipse.VerticalAlignment = VerticalAlignment.Top;
            ellipse.Width = ellipse.Height = 0;
            ellipse.Effect = new BlurEffect {Radius = 20};
            var grid = new Grid();
            grid.Children.Add(ellipse);
            base.Child = grid;
        }

        /// <summary>
        ///     动画速度
        /// </summary>
        public double AnimationSpeed { get; set; } = 2;

        /// <summary>
        ///     水波反馈
        /// </summary>
        public bool WaveFeedback { get; set; } = true;

        public new UIElement Child
        {
            get
            {
                if ((base.Child as Grid).Children.Count > 1) return (base.Child as Grid).Children[1];
                return null;
            }
            set
            {
                if ((base.Child as Grid).Children.Count < 2)
                    (base.Child as Grid).Children.Add(value);
                else
                    (base.Child as Grid).Children[1] = value;
            }
        }

        private void MaterialCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WaveFeedback)
            {
                var radius = Math.Max(ActualHeight, ActualWidth) * 2;
                ellipseSizeAnim.To = radius;
                var p = e.GetPosition((IInputElement) sender);
                fixAnim.To = new Thickness(-radius / 2, -radius / 2, 0, 0);
                if (Background is SolidColorBrush)
                {
                    var c = (Background as SolidColorBrush).Color;
                    ellipse.Fill = new SolidColorBrush(new Color
                        {A = 255, R = (byte) (c.R * 0.80), B = (byte) (c.B * 0.80), G = (byte) (c.G * 0.80)});
                }

                ellipse.RenderTransform = new TranslateTransform(p.X, p.Y);
                ellipse.BeginAnimation(WidthProperty, ellipseSizeAnim);
                ellipse.BeginAnimation(HeightProperty, ellipseSizeAnim);
                ellipse.BeginAnimation(OpacityProperty, ellipseOpacityAnim);
                ellipse.BeginAnimation(MarginProperty, fixAnim);

                if (Background is SolidColorBrush)
                {
                    var background = Background as SolidColorBrush;
                    var originBrush = background.Clone();
                    var c = originBrush.Color;
                    var c2 = Color.FromArgb(Math.Min((byte) (c.A * 1.4), (byte) 255), (byte) (c.R * 0.96),
                        (byte) (c.G * 0.96), (byte) (c.B * 0.96));
                    backgroundAnim.KeyFrames.Clear();
                    backgroundAnim.KeyFrames.Add(new EasingColorKeyFrame(c2) {EasingFunction = easeFunction});
                    backgroundAnim.KeyFrames.Add(new EasingColorKeyFrame(c) {EasingFunction = easeFunction});
                    background = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B)); //防冻结
                    background.BeginAnimation(SolidColorBrush.ColorProperty, backgroundAnim);
                }
            }
        }
    }
}