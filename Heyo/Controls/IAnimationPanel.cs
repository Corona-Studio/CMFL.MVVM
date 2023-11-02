using System.Windows.Media.Animation;

namespace Heyo.Controls
{
    public interface IAnimationPanel
    {
        double AnimationSpeed { get; set; }
        EasingFunctionBase Function { get; set; }
        void Anim();
    }
}