using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Heyo.Controls.Class;

namespace Heyo.Controls
{
    public class GrainsBackground : Canvas
    {
        private readonly List<GrainBase> grains = new List<GrainBase>();

        //List<GrainBase> grainsEqual = new List<GrainBase>();
        private readonly GrainBase mousePoint = new GrainBase();
        private readonly Random rand = new Random();
        private readonly DispatcherTimer updateTimer;

        public GrainsBackground()
        {
            UseLayoutRounding = false;
            SnapsToDevicePixels = false;
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255));
            updateTimer = new DispatcherTimer();
            updateTimer.Tick += DrawingAY;
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 40);
            MouseDown += Cav_MouseDown;
            MouseMove += Cav_MouseMove;
            MouseLeave += Cav_MouseLeave;
            Loaded += GrainsBackground_Loaded;
            mousePoint.Max = 20000;
            grains.Add(mousePoint);
        }

        public int GrainsCount { get; set; } = 30;
        public double Acceleration { get; set; } = 4;

        public int MaxLineLength { get; set; } = 8000;

        public void Stop()
        {
            updateTimer.Stop();
        }

        public void Start()
        {
            updateTimer.Start();
        }

        private void GrainsBackground_Loaded(object sender, RoutedEventArgs e)
        {
            //// 添加粒子
            //// x，y为粒子坐标，xa, ya为粒子xy轴加速度，max为连线的最大距离     
            for (var i = 0; i < GrainsCount; i++)
            {
                var gb = new GrainBase();
                gb.X = rand.NextDouble() * ActualWidth;
                gb.Y = rand.NextDouble() * ActualHeight;
                gb.Xa = rand.NextDouble() * Acceleration - 1;
                gb.Ya = rand.NextDouble() * Acceleration - 1;
                gb.Max = MaxLineLength;
                grains.Add(gb);
            }
        }

        private void DrawingAY(object sender, EventArgs e)
        {
            Next();
        }

        private void Cav_MouseMove(object sender, MouseEventArgs e)
        {
            var ui = e.GetPosition(this);
            mousePoint.X = ui.X;
            mousePoint.Y = ui.Y;
        }

        private void Cav_MouseLeave(object sender, MouseEventArgs e)
        {
            mousePoint.X = null;
            mousePoint.Y = null;
        }

        public void Next()
        {
            Children.Clear();
            var lineColor = Color.FromRgb(255, 255, 255);
            if (OpacityMask is SolidColorBrush)
                lineColor = (OpacityMask as SolidColorBrush).Color;
            else if (OpacityMask is LinearGradientBrush)
                lineColor = (OpacityMask as LinearGradientBrush).GradientStops[0].Color;

            for (var i = 0; i < grains.Count; i++)
            {
                var dot = grains[i];
                if (dot.X == null || dot.Y == null) continue;

                #region 创建碰撞粒子

                // 粒子位移
                dot.X += dot.Xa;
                dot.Y += dot.Ya;
                // 遇到边界将加速度反向
                dot.Xa *= dot.X.Value > ActualWidth || dot.X.Value < 0 ? -1 : 1;
                dot.Ya *= dot.Y.Value > ActualHeight || dot.Y.Value < 0 ? -1 : 1;
                // 绘制点
                var elip = new Ellipse();
                elip.Width = 2;
                elip.Height = 2;
                SetLeft(elip, dot.X.Value - 0.5);
                SetTop(elip, dot.Y.Value - 0.5);
                elip.Fill = new SolidColorBrush(Colors.Transparent);
                Children.Add(elip);

                #endregion


                //判断是不是最后一个，就不用两两比较了
                var endIndex = i + 1;
                if (endIndex == grains.Count) continue;
                for (var j = endIndex; j < grains.Count; j++)
                {
                    var d2 = grains[j];
                    var xc = dot.X.Value - d2.X.Value;
                    var yc = dot.Y.Value - d2.Y.Value;
                    // 两个粒子之间的距离
                    var dis = xc * xc + yc * yc;
                    // 距离比
                    double ratio;
                    // 如果两个粒子之间的距离小于粒子对象的max值，则在两个粒子间画线
                    if (dis < d2.Max)
                    {
                        elip.Fill = new SolidColorBrush(lineColor);
                        // 计算距离比
                        ratio = (d2.Max - dis) / d2.Max;
                        var line = new Line();
                        var opacity = ratio + 0.2;
                        if (opacity > 1) opacity = 1;
                        var ar = (byte) (opacity * 255);
                        line.Stroke = new SolidColorBrush(Color.FromArgb(ar, lineColor.R, lineColor.G, lineColor.B));
                        line.StrokeThickness = ratio / 2;
                        line.X1 = dot.X.Value;
                        line.Y1 = dot.Y.Value;
                        line.X2 = d2.X.Value;
                        line.Y2 = d2.Y.Value;
                        Children.Add(line);
                    }
                }
            }
        }

        private void Cav_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                grains.RemoveAt(rand.Next(0, grains.Count));
                var gb = new GrainBase();
                var ui = e.GetPosition(this);
                gb.X = ui.X;
                gb.Y = ui.Y;

                gb.Xa = rand.NextDouble() * Acceleration - 1;
                gb.Ya = rand.NextDouble() * Acceleration - 1;
                gb.Max = MaxLineLength;
                grains.Add(gb);
            }
        }
    }
}