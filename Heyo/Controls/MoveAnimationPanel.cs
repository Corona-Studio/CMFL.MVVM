using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Heyo.Controls
{
    public class MoveAnimationPanel : Border, IAnimationPanel
    {
        private readonly ThicknessAnimation moveAnimMargin = new ThicknessAnimation();
        private readonly DoubleAnimation moveAnimX = new DoubleAnimation();
        private readonly DoubleAnimation moveAnimY = new DoubleAnimation();
        private readonly TranslateTransform transform;
        private Point originPoint;


        public MoveAnimationPanel()
        {
            if (!(RenderTransform is TranslateTransform)) RenderTransform = new TranslateTransform(0, 0);
            transform = RenderTransform as TranslateTransform;
            originPoint = new Point(transform.X, transform.Y);
            moveAnimMargin.EasingFunction = moveAnimX.EasingFunction = moveAnimY.EasingFunction = Function;
            moveAnimMargin.SpeedRatio = moveAnimX.SpeedRatio = moveAnimY.SpeedRatio = AnimationSpeed;
            MouseDown += (s, e) =>
            {
                if (MouseDownAnimation && !Lock) MoveTo(new Point(), MouseDownMoveTo, MouseDownAnimationComplete);
            };
            MouseEnter += (s, e) =>
            {
                if (MouseEnterAnimation && !Lock) MoveTo(new Point(), MouseEnterMoveTo, MouseEnterAnimationComplete);
            };
            MouseLeave += (s, e) =>
            {
                if (MouseEnterAnimation && !Lock) Back();
            };
        }

        public Point From { get; set; }
        public Point To { get; set; }
        public bool MouseEnterAnimation { get; set; } = true;
        public bool MouseDownAnimation { get; set; } = false;
        public Point MouseEnterMoveTo { get; set; }
        public Point MouseDownMoveTo { get; set; }
        public bool Lock { get; set; } = false;
        public double AnimationSpeed { get; set; } = 2;
        public EasingFunctionBase Function { get; set; } = new PowerEase {Power = 4, EasingMode = EasingMode.EaseOut};

        public void Anim()
        {
            if (To != null)
            {
                moveAnimX.From = From.X;
                moveAnimY.From = From.Y;
                moveAnimX.To = To.X;
                moveAnimY.To = To.Y;
                if (Math.Abs(originPoint.X - To.X) > Math.Abs(originPoint.Y - To.Y))
                    moveAnimX.Completed += (s, e) => { Complete?.Invoke(this, new EventArgs()); };
                else
                    moveAnimY.Completed += (s, e) => { Complete?.Invoke(this, new EventArgs()); };
                transform.BeginAnimation(TranslateTransform.XProperty, moveAnimX);
                transform.BeginAnimation(TranslateTransform.YProperty, moveAnimY);
            }
        }

        public event EventHandler Complete;

        public event EventHandler MouseEnterAnimationComplete;
        public event EventHandler MouseDownAnimationComplete;

        public void MoveTo(Point from, Point to, EventHandler completed)
        {
            if (to != null)
            {
                moveAnimX.From = from.X;
                moveAnimY.From = from.Y;
                moveAnimX.To = to.X;
                moveAnimY.To = to.Y;
                if (Math.Abs(originPoint.X - to.X) > Math.Abs(originPoint.Y - to.Y))
                    moveAnimX.Completed += (s, e) => { completed?.Invoke(this, new EventArgs()); };
                else
                    moveAnimY.Completed += (s, e) => { completed?.Invoke(this, new EventArgs()); };
                transform.BeginAnimation(TranslateTransform.XProperty, moveAnimX);
                transform.BeginAnimation(TranslateTransform.YProperty, moveAnimY);
            }
        }

        public void MoveTo(Thickness to, EventHandler completed)
        {
            if (to != null)
            {
                moveAnimMargin.To = to;
                if (completed != null) moveAnimMargin.Completed += completed;

                BeginAnimation(MarginProperty, moveAnimMargin);
            }
        }

        public void Back()
        {
            if (originPoint != null)
            {
                moveAnimX.To = originPoint.X;
                moveAnimY.To = originPoint.Y;
                transform.BeginAnimation(TranslateTransform.XProperty, moveAnimX);
                transform.BeginAnimation(TranslateTransform.YProperty, moveAnimY);
            }
        }
    }
}