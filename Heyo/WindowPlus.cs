using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Heyo.Class.Helper;

namespace Heyo
{
    public class WindowPlus : Window
    {
        public static readonly DependencyProperty AeroProperty =
            DependencyProperty.Register(
                "Aero",
                typeof(bool),
                typeof(WindowPlus),
                new PropertyMetadata(false)
            );

        public static readonly DependencyProperty ShadowProperty =
            DependencyProperty.Register(
                "Shadow",
                typeof(bool),
                typeof(WindowPlus),
                new PropertyMetadata(true)
            );

        public static readonly DependencyProperty ShadowRadiusProperty =
            DependencyProperty.Register(
                "ShadowRadius",
                typeof(int),
                typeof(WindowPlus),
                new PropertyMetadata(14)
            );


        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register(
                "ShadowOpacity",
                typeof(double),
                typeof(WindowPlus),
                new PropertyMetadata(0.2)
            );

        public static readonly DependencyProperty WindowEntranceProperty =
            DependencyProperty.Register(
                "WindowEntrance",
                typeof(WindowEntrance),
                typeof(WindowPlus),
                new PropertyMetadata(WindowEntrance.None)
            );

        public static readonly DependencyProperty WindowEntranceTimeProperty =
            DependencyProperty.Register(
                "WindowEntranceTime",
                typeof(double),
                typeof(WindowPlus),
                new PropertyMetadata(0.4)
            );

        private readonly Window aeroLayer = new Window
        {
            Opacity = 0,
            Background = new SolidColorBrush(Colors.Transparent),
            AllowsTransparency = true,
            WindowStyle = WindowStyle.None
        };

        private readonly Stopwatch s = new Stopwatch();

        private readonly Border shadowBorder = new Border
        {
            BorderThickness = new Thickness(4),
            Background = new SolidColorBrush(Colors.Transparent),
            BorderBrush = new SolidColorBrush(Colors.Black),
            Effect = new BlurEffect()
        };

        /*private Image imageCache = new Image();
        private Window entranceWindow = new Window()
        {
            AllowsTransparency = true,
            WindowStyle = WindowStyle.None,
            Background = new SolidColorBrush(Colors.Transparent),
        };*/
        private readonly Window shadowLayer = new Window
        {
            Opacity = 0,
            Focusable = false,
            Background = new SolidColorBrush(Colors.Transparent),
            //ShowInTaskbar = false,
            AllowsTransparency = true,
            WindowStyle = WindowStyle.None
        };

        public WindowPlus()
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                Opacity = 0;
                Loaded += WindowPlus_Loaded;
                Handle = new WindowInteropHelper(this).Handle;
                ShowInTaskbar = false;
                shadowLayer.Content = shadowBorder;
            });

            //entranceWindow.Content = imageCache;
            //entranceWindow.Show();
        }

        public IntPtr Handle { get; private set; }

        /// <summary>
        ///     是否启用Aero
        /// </summary>
        [Category("样式")]
        public bool Aero
        {
            get => (bool) GetValue(AeroProperty);
            set
            {
                SetValue(AeroProperty, value);
                if ( /*SystemHelper.IsWindow10*/true)
                {
                    if (value)
                        AeroHelper.EnableAero(this, true);
                    else
                        AeroHelper.DisableAero(this);
                }
            }
        }

        /// <summary>
        ///     是否启用Aero
        /// </summary>
        [Category("样式")]
        public bool Shadow
        {
            get => (bool) GetValue(ShadowProperty);
            set => SetValue(ShadowProperty, value);
        }

        /// <summary>
        ///     阴影的半径
        /// </summary>
        [Category("样式")]
        public int ShadowRadius
        {
            get => (int) GetValue(ShadowRadiusProperty);
            set
            {
                SetValue(ShadowRadiusProperty, value);
                SetShadowRadius(value);
            }
        }

        /// <summary>
        ///     阴影的浓度
        /// </summary>
        [Category("样式")]
        public double ShadowOpacity
        {
            get => (double) GetValue(ShadowOpacityProperty);
            set
            {
                SetValue(ShadowOpacityProperty, value);
                SetShadowOpacity(value);
            }
        }

        /// <summary>
        ///     入场方式
        /// </summary>
        [Category("样式")]
        public WindowEntrance WindowEntrance
        {
            get => (WindowEntrance) GetValue(WindowEntranceProperty);
            set => SetValue(WindowEntranceProperty, value);
        }

        /// <summary>
        ///     入场动画时间
        /// </summary>
        [Category("样式")]
        public double WindowEntranceTime
        {
            get => (double) GetValue(WindowEntranceTimeProperty);
            set => SetValue(WindowEntranceTimeProperty, value);
        }

        private void WindowPlus_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLayerSize();
            UpdateLayerLocation();
            SizeChanged += WindowPlus_SizeChanged;
            //WindowStyleHelper.SetOpacity(this, 0.6);
            if (SystemHelper.IsWindow10 && Aero)
            {
                aeroLayer.Show();
                Owner = aeroLayer;
                AeroHelper.EnableAero(aeroLayer, Shadow);
            }
            else
            {
                SetShadowRadius(ShadowRadius);
                SetShadowOpacity(ShadowOpacity);
                shadowLayer.Show();
                Owner = shadowLayer;
            }

            if (WindowEntrance == WindowEntrance.Fade)
            {
                var showAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(WindowEntranceTime)));
                showAnimation.CurrentTimeInvalidated += (s, e2) =>
                {
                    WindowStyleHelper.SetOpacity(aeroLayer, Opacity);
                };
                showAnimation.Completed += (s, e2) => { WindowStyleHelper.SetOpacity(aeroLayer, Opacity); };
                BeginAnimation(OpacityProperty, showAnimation);
            }
            else if (WindowEntrance == WindowEntrance.None)
            {
                Opacity = 1;
                shadowLayer.Opacity = 1;
            }


            Focus();
        }

        private void WindowPlus_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLayerSize();
            OnLocationChanged(new EventArgs());
        }

        private void UpdateLayerLocation()
        {
            s.Restart();
            aeroLayer.Left = Left;
            aeroLayer.Top = Top;
            shadowLayer.Left = Left - ShadowRadius;
            shadowLayer.Top = Top - ShadowRadius;
            Console.WriteLine(s.ElapsedMilliseconds);
        }

        protected void UpdateLayerSize()
        {
            aeroLayer.Width = ActualWidth;
            aeroLayer.Height = ActualHeight;
            shadowLayer.Width = ActualWidth + ShadowRadius * 2;
            shadowLayer.Height = ActualHeight + ShadowRadius * 2;
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            UpdateLayerLocation();
            base.OnLocationChanged(e);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            shadowLayer.Opacity = Opacity;
            base.OnPropertyChanged(e);
        }

        private void SetShadowRadius(int radius)
        {
            shadowBorder.Margin = new Thickness(ShadowRadius);
            (shadowBorder.Effect as BlurEffect).Radius = radius;
        }

        private void SetShadowOpacity(double opacity)
        {
            shadowBorder.Opacity = opacity;
        }

        public new void Close()
        {
            if (WindowEntrance == WindowEntrance.Fade)
            {
                var closeAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(WindowEntranceTime)));
                closeAnimation.CurrentTimeInvalidated += (s, e) =>
                {
                    WindowStyleHelper.SetOpacity(aeroLayer, Opacity);
                };
                closeAnimation.Completed += (s, e) =>
                {
                    base.Close();
                    shadowLayer.Close();
                    aeroLayer.Close();
                };
                BeginAnimation(OpacityProperty, closeAnimation);
            }
            else if (WindowEntrance == WindowEntrance.None)
            {
                base.Close();
                shadowLayer.Close();
                aeroLayer.Close();
            }
        }
    }
}