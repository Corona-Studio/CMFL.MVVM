using System.Windows.Media;

namespace Heyo.Controls.Class
{
    /// <summary>
    ///     粒子类
    /// </summary>
    public class GrainBase
    {
        public double? X { get; set; }
        public double? Y { get; set; }

        public double Xa { get; set; }

        public double Ya { get; set; }

        public double Size { get; set; }
        public double Max { get; set; }

        public Color Color { get; set; }

        public RectangleGeometry Area { get; set; }
    }
}