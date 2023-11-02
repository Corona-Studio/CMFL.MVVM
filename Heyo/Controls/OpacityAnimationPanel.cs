using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Heyo.Controls
{
    public class OpacityAnimationPanel : Border, IAnimationPanel
    {
        private readonly DoubleAnimation opacityAnim = new DoubleAnimation();

        public OpacityAnimationPanel()
        {
            opacityAnim.EasingFunction = Function;
            opacityAnim.SpeedRatio = AnimationSpeed;
        }

        public double From { get; set; } = 0;
        public double To { get; set; } = 1;

        public double AnimationSpeed { get; set; } = 4;
        public EasingFunctionBase Function { get; set; } = new PowerEase {Power = 2, EasingMode = EasingMode.EaseOut};

        public void Anim()
        {
            opacityAnim.From = From;
            opacityAnim.To = To;
            if (Completed != null) opacityAnim.Completed += Completed;

            BeginAnimation(OpacityProperty, opacityAnim);
        }

        public event EventHandler Completed;
    }
}