using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Heyo.Controls
{
    public class BlurAnimationPanel : Border, IAnimationPanel
    {
        private readonly DoubleAnimation radiusAnim = new DoubleAnimation();

        public BlurAnimationPanel()
        {
            Effect = new BlurEffect {Radius = 0};
            radiusAnim.EasingFunction = Function;
            radiusAnim.SpeedRatio = AnimationSpeed;
        }

        public double From { get; set; }
        public double To { get; set; }

        public double AnimationSpeed { get; set; } = 4;
        public EasingFunctionBase Function { get; set; } = new PowerEase {Power = 4, EasingMode = EasingMode.EaseOut};


        public void Anim()
        {
            radiusAnim.To = To;
            if (Complete != null) radiusAnim.Completed += Complete;

            (Effect as BlurEffect).BeginAnimation(BlurEffect.RadiusProperty, radiusAnim);
        }

        public event EventHandler Complete;
    }
}