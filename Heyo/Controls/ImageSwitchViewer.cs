using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Heyo.Controls
{
    public class ImageSwitchViewer : Control
    {
        public static readonly DependencyProperty ImageItemsProperty =
            DependencyProperty.Register(
                "ImageItems",
                typeof(List<Image>),
                typeof(ImageSwitchViewer),
                new PropertyMetadata(new List<Image>())
            );

        public static readonly DependencyProperty HyperlinkItemsProperty =
            DependencyProperty.Register(
                "HyperlinkItems",
                typeof(List<Hyperlink>),
                typeof(ImageSwitchViewer),
                new PropertyMetadata(new List<Hyperlink>())
            );

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(ImageSwitchViewer),
                new PropertyMetadata(0)
            );

        public static readonly DependencyProperty SwitchCycleProperty =
            DependencyProperty.Register(
                "SwitchCycle",
                typeof(int),
                typeof(ImageSwitchViewer),
                new PropertyMetadata(5000)
            );

        public static readonly DependencyProperty AutoSwitchProperty =
            DependencyProperty.Register(
                "AutoSwitch",
                typeof(bool),
                typeof(ImageSwitchViewer),
                new PropertyMetadata(true)
            );

        public static readonly DependencyProperty RadioButtonStyleProperty =
            DependencyProperty.Register(
                "RadioButtonStyle",
                typeof(Style),
                typeof(ImageSwitchViewer)
            );

        private readonly DispatcherTimer switchTimer = new DispatcherTimer();

        private Grid imageHost;
        private int lastIndex = -1;
        private StackPanel radioButtonHost;

        static ImageSwitchViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageSwitchViewer),
                new FrameworkPropertyMetadata(typeof(ImageSwitchViewer)));
        }

        //private Image image1 = new Image() { Stretch = Stretch.UniformToFill }, image2 = new Image() { Stretch = Stretch.UniformToFill };
        public ImageSwitchViewer()
        {
            Loaded += ImageViewer_Loaded;
        }

        /// <summary>
        ///     所要切换的图片集合
        /// </summary>
        public List<Image> ImageItems
        {
            get => (List<Image>) GetValue(ImageItemsProperty);
            set => SetValue(ImageItemsProperty, value);
        }

        /// <summary>
        ///     图片对应的超链接集合
        /// </summary>
        public List<Hyperlink> HyperlinkItems
        {
            get => (List<Hyperlink>) GetValue(HyperlinkItemsProperty);
            set => SetValue(HyperlinkItemsProperty, value);
        }

        /// <summary>
        ///     当前显示的图片
        /// </summary>
        public int SelectedIndex
        {
            get => (int) GetValue(SelectedIndexProperty);
            set
            {
                SetValue(SelectedIndexProperty, value);
                SetIndex(value);
                (radioButtonHost.Children[value] as RadioButton).IsChecked = true;
            }
        }

        /// <summary>
        ///     切换周期，毫秒为单位
        /// </summary>
        public int SwitchCycle
        {
            get => (int) GetValue(SwitchCycleProperty);
            set => SetValue(SwitchCycleProperty, value);
        }

        /// <summary>
        ///     是否自动切换
        /// </summary>
        public bool AutoSwitch
        {
            get => (bool) GetValue(AutoSwitchProperty);
            set
            {
                SetValue(AutoSwitchProperty, value);
                if (value)
                    switchTimer.Start();
                else
                    switchTimer.Stop();
            }
        }

        public Style RadioButtonStyle
        {
            get => (Style) GetValue(RadioButtonStyleProperty);
            set => SetValue(RadioButtonStyleProperty, value);
        }

        private void ImageViewer_Loaded(object sender, RoutedEventArgs e)
        {
            radioButtonHost = Template.FindName("radioButtonHost", this) as StackPanel;
            imageHost = Template.FindName("imageHost", this) as Grid;
            //imageHost.Children.Add(image1);
            //mageHost.Children.Add(image2);
            for (var i = 0; i < ImageItems?.Count; i++)
            {
                var radioButton = new RadioButton {GroupName = Uid, Tag = i, Style = RadioButtonStyle};
                radioButton.Checked += RadioButton_Checked;
                radioButtonHost.Children.Add(radioButton);
                ImageItems[i].Visibility = Visibility.Collapsed;
                imageHost.Children.Add(ImageItems[i]);
            }

            SelectedIndex = 0;
            switchTimer.Interval = TimeSpan.FromMilliseconds(SwitchCycle);
            switchTimer.Tick += SwitchTimer_Tick;
            if (AutoSwitch) switchTimer.Start();
        }

        private void SwitchTimer_Tick(object sender, EventArgs e)
        {
            SelectedIndex = SelectedIndex < ImageItems.Count - 1 ? SelectedIndex + 1 : 0;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            switchTimer.Stop();
            switchTimer.Start();
            SetIndex((int) radioButton.Tag);
        }

        /// <summary>
        ///     设置所显示的图片
        /// </summary>
        /// <param name="index">图片引索</param>
        private void SetIndex(int index)
        {
            if (index < 0 || index > ImageItems.Count - 1 || lastIndex == index) return;
            /*以下代码通过image1,image2交替切换实现淡出淡入的效果*/
            var showAnim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                EasingFunction = new PowerEase {Power = 4, EasingMode = EasingMode.EaseOut},
                SpeedRatio = 1
            };
            var hideAnim = new DoubleAnimation
            {
                From = 1,
                To = 0,
                EasingFunction = new PowerEase {Power = 4, EasingMode = EasingMode.EaseOut},
                SpeedRatio = 1
            };
            ImageItems[index].Visibility = Visibility.Visible;
            ImageItems[index].BeginAnimation(OpacityProperty, showAnim);
            if (lastIndex > -1)
            {
                hideAnim.Completed += (s, e) =>
                {
                    if (index == lastIndex) return;

                    ImageItems[lastIndex].Visibility = Visibility.Collapsed;
                    lastIndex = index;
                };
                ImageItems[lastIndex].BeginAnimation(OpacityProperty, hideAnim);
            }
            else
            {
                lastIndex = index;
            }
        }
    }
}