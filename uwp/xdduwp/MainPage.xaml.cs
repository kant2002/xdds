using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Timers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace xdduwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const int Max = 500;
        int count = 0;
        int lastcount = 0;
        readonly System.Timers.Timer timer = new System.Timers.Timer(500);
        readonly Stopwatch stopwatch = new Stopwatch();
        private double width;
        private double height;
        public MainPage()
        {
            this.InitializeComponent();
            timer.Elapsed += OnTimer;
            this.SizeChanged += (_, __) =>
            {
            };
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            width = 800;
            height = 800;

            stopwatch.Start();
            timer.Start();
            _ = Task.Factory.StartNew(RunTest, TaskCreationOptions.LongRunning);
        }
        async void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            double avg = (lastcount == 0 ? count : lastcount) / stopwatch.Elapsed.TotalSeconds;
            string text = "XDD/s: " + avg.ToString("0.00", CultureInfo.InvariantCulture);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateText(text));
        }

        void UpdateText(string text) => lols.Text = text;

        async Task RunTest()
        {
            var random = new Random();
            while (count < 5000)
            {
                double left = random.NextDouble() * width;
                double top = random.NextDouble() * height;
                Color color = Color.FromArgb(255, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var brush = new SolidColorBrush(color);
                    var label = new TextBlock
                    {
                        Text = "XDD?",
                        Foreground = brush,
                    };
                    // AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
                    // Cannot find a way
                    label.RenderTransform = new RotateTransform() { Angle = random.NextDouble() * 360 };
                    // AbsoluteLayout.SetLayoutBounds(label, new Rect(random.NextDouble(), random.NextDouble(), 80, 24));
                    label.Width = 80;
                    label.Height = 24;

                    if (absolute.Children.Count >= Max)
                        absolute.Children.RemoveAt(0);
                    absolute.Children.Add(label);
                    var current = Canvas.GetLeft(label);
                    Canvas.SetLeft(label, left);
                    Canvas.SetTop(label, top);
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

    public class ThreadSafeRandom
    {
        private static readonly Random _global = new Random();
        private static readonly ThreadLocal<Random> _local = new ThreadLocal<Random>(() =>
        {
            int seed;
            lock (_global)
            {
                seed = _global.Next();
            }
            return new Random(seed);
        });

        public static Random Instance => _local.Value;
    }

}
