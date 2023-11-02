using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Heyo.Controls
{
    /// <summary>
    ///     CircularProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class ArcProgressBar : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(double),
                typeof(ArcProgressBar),
                new PropertyMetadata(0.0, (d, e) => { UpdateView((ArcProgressBar) d); })
            );

        public ArcProgressBar()
        {
            InitializeComponent();
        }

        public double IndicatorThickness
        {
            get => Indicator.ArcThickness;
            set => Indicator.ArcThickness = value;
        }

        public double TrackThickness
        {
            get => Track.ArcThickness;
            set => Track.ArcThickness = value;
        }

        public double TrackRadius
        {
            get => Track.Width;
            set => Track.Width = Track.Height = value;
        }

        public double IndicatorRadius
        {
            get => Indicator.Width;
            set => Indicator.Width = Indicator.Height = value;
        }

        public Brush TrackBrush
        {
            get => Track.Fill;
            set => Track.Fill = value;
        }

        public Brush IndicatorBrush
        {
            get => Indicator.Fill;
            set => Indicator.Fill = value;
        }

        public double Minimun { get; set; } = 0;

        public double Maxmum { get; set; } = 100;

        //private double _value = 0;
        public double Value
        {
            get => (double) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void UpdateView(ArcProgressBar bar)
        {
            var angle = 360 * bar.Value * 100 / (bar.Maxmum - bar.Minimun);
            bar.Indicator.EndAngle = angle > 360 ? 360 : angle;
        }
    }
}