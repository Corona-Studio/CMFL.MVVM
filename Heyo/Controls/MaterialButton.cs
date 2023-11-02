using System.Windows;
using System.Windows.Controls.Primitives;

namespace Heyo.Controls
{
    public class MaterialButton : ButtonBase
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                "CornerRadius",
                typeof(int),
                typeof(MaterialButton),
                new PropertyMetadata(2)
            );

        public static readonly DependencyProperty ShadowProperty =
            DependencyProperty.Register(
                "Shadow",
                typeof(bool),
                typeof(MaterialButton),
                new PropertyMetadata(true)
            );

        public static readonly DependencyProperty MouseEnterAnimationProperty =
            DependencyProperty.Register(
                "MouseEnterAnimation",
                typeof(bool),
                typeof(MaterialButton),
                new PropertyMetadata(true)
            );

        static MaterialButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaterialButton),
                new FrameworkPropertyMetadata(typeof(MaterialButton)));
        }

        public int CornerRadius
        {
            get => (int) GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public bool Shadow
        {
            get => (bool) GetValue(ShadowProperty);
            set
            {
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(ShadowProperty, Shadow, value));
                SetValue(ShadowProperty, value);
            }
        }

        public bool MouseEnterAnimation
        {
            get => (bool) GetValue(MouseEnterAnimationProperty);
            set
            {
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(MouseEnterAnimationProperty, Shadow, value));
                SetValue(MouseEnterAnimationProperty, value);
            }
        }
    }
}