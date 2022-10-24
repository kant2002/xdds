using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace xddswpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int Max = 500;
        int count = 0;
        int lastcount = 0;
        readonly System.Timers.Timer timer = new System.Timers.Timer(500);
        readonly Stopwatch stopwatch = new Stopwatch();
        private double width;
        private double height;
        public MainWindow()
        {
            InitializeComponent();
            timer.Elapsed += OnTimer;
            width = this.Width;
            height = this.Height;

            stopwatch.Start();
            timer.Start();
            _ = Task.Factory.StartNew(RunTest, TaskCreationOptions.LongRunning);
        }
        void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            double avg = (lastcount == 0 ? count : lastcount) / stopwatch.Elapsed.TotalSeconds;
            string text = "XDD/s: " + avg.ToString("0.00", CultureInfo.InvariantCulture);
            Dispatcher.Invoke(() => UpdateText(text));
        }

        void UpdateText(string text) => lols.Text = text;

        void RunTest()
        {
            var random = Random.Shared;
            while (count < 5000)
            {
                Dispatcher.Invoke(() =>
                {
                    Color color = Color.FromArgb(255, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                    var brush = new SolidColorBrush(color);
                    var label = new TextBlock
                    {
                        Text = "XDD?",
                        Foreground = brush,
                    };
                    // AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
                    // Cannot find a way
                    //LayoutTransformControl lt = new LayoutTransformControl();
                    //lt.Child = label;
                    //lt.RenderTransform = new RotateTransform(random.NextDouble() * 360);
                    label.RenderTransform = new RotateTransform() { Angle = random.NextDouble() * 360 };
                    // AbsoluteLayout.SetLayoutBounds(label, new Rect(random.NextDouble(), random.NextDouble(), 80, 24));
                    label.Width = 80;
                    label.Height = 24;
                    Canvas.SetLeft(label, random.NextDouble() * width);
                    Canvas.SetTop(label, random.NextDouble() * height);
                    if (absolute.Children.Count >= Max)
                        absolute.Children.RemoveAt(0);
                    absolute.Children.Add(label);
                    count++;
                });
                //NOTE: plain Android we could put 1
                if (count % 10 == 0)
                    Thread.Sleep(1);
            }

            stopwatch.Stop();
            timer.Stop();
            lastcount = count;
        }
    }
}
