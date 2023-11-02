using System;
using System.Windows;
using System.Windows.Controls;

namespace Heyo.Controls
{
    internal class MaterialFloatingBox : MaterialCard
    {
        private readonly Viewbox viewbox;
        private double originWidth, originHeight;

        public MaterialFloatingBox()
        {
            viewbox = new Viewbox();
            base.Child = viewbox;
            ShadowAnim = true;
            Opacity = 0;
            Visibility = Visibility.Collapsed;
            AnimationSpeed = 8;
            DownRadius = 10;
            ShadowOpacity = 0.2;
            Loaded += (s, e) =>
            {
                originWidth = Width;
                originHeight = Height;
            };
        }

        public new UIElement Child
        {
            get => viewbox.Child;
            set => viewbox.Child = value;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
            SetSizeWithOutAnim(0, 0);
            Width = originWidth;
            Height = originHeight;
            Opacity = 1;
        }

        public void Hide()
        {
            Width = 0;
            Height = 0;
            Opacity = 0;
            SizeAnim.Completed += SizeAnim_Completed;
        }

        private void SizeAnim_Completed(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;
            SizeAnim.Completed -= SizeAnim_Completed;
        }
    }
}