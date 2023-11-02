using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Heyo.Controls
{
    public class ClippingBorder : Border
    {
        private readonly RectangleGeometry _clipRect = new RectangleGeometry();
        private object _oldClip;

        public override UIElement Child
        {
            get => base.Child;
            set
            {
                if (Child != value)
                {
                    if (Child != null)
                        // Restore original clipping  
                        Child.SetValue(ClipProperty, _oldClip);

                    if (value != null)
                        _oldClip = value.ReadLocalValue(ClipProperty);
                    else
                        // If we dont set it to null we could leak a Geometry object  
                        _oldClip = null;

                    base.Child = value;
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            OnApplyChildClip();
            base.OnRender(dc);
        }

        protected virtual void OnApplyChildClip()
        {
            var child = Child;
            if (child != null)
            {
                _clipRect.RadiusX =
                    _clipRect.RadiusY = Math.Max(0.0, CornerRadius.TopLeft - BorderThickness.Left * 0.5);
                var rect = new Rect(RenderSize);
                rect.Height -= BorderThickness.Top + BorderThickness.Bottom;
                rect.Width -= BorderThickness.Left + BorderThickness.Right;
                _clipRect.Rect = rect;
                child.Clip = _clipRect;
            }
        }

        public void Update()
        {
            OnApplyChildClip();
        }
    }
}