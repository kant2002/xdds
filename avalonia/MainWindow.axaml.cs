using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace xdds
{
    public partial class MainWindow : Window
    {
        const int Max = 500;
        volatile int count = 0;
        int lastcount = 0;
        readonly System.Timers.Timer timer = new System.Timers.Timer(500);
        readonly Stopwatch stopwatch = new Stopwatch();
        private double width;
        private double height;

        public MainWindow()
        {
            InitializeComponent();
            timer.Elapsed += OnTimer;
        }
        void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            double avg = (lastcount == 0 ? count : lastcount) / stopwatch.Elapsed.TotalSeconds;
            string text = "XDD/s: " + avg.ToString("0.00", CultureInfo.InvariantCulture);
            Dispatcher.UIThread.Post(() => UpdateText(text), DispatcherPriority.MaxValue);
        }

        void UpdateText(string text) => lols.Text = text;

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            width = this.Width;
            height = this.Height;

            stopwatch.Start();
            timer.Start();
            _ = Task.Factory.StartNew(RunTest, TaskCreationOptions.LongRunning);
        }

        void RunTest()
        {
            var random = Random.Shared;
            while (count < 5000)
            {
                Color color = new Color(255, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                var brush = new ImmutableSolidColorBrush(color);
                Dispatcher.UIThread.Post(() =>
                {
                    var label = new TextBlock
                    {
                        Text = "XDD?",
                        Foreground = brush,
                    };
                    // AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
                    // Cannot find a way
                    LayoutTransformControl lt = new LayoutTransformControl();
                    lt.Child = label;
                    lt.RenderTransform = new RotateTransform(random.NextDouble() * 360);
                    // AbsoluteLayout.SetLayoutBounds(label, new Rect(random.NextDouble(), random.NextDouble(), 80, 24));
                    label.Width = 80;
                    label.Height = 24;
                    Canvas.SetLeft(lt, random.NextDouble() * width);
                    Canvas.SetTop(lt, random.NextDouble()* height);
                    if (absolute.Children.Count >= Max)
                        absolute.Children.RemoveAt(0);
                    //absolute.Children.Add(label);
                    absolute.Children.Add(lt);
                    count++;
                });
                Thread.Sleep(1);
            }

            stopwatch.Stop();
            timer.Stop();
            lastcount = count;
        }

    }
}