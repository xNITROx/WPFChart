using System;
using System.Timers;
using System.Windows.Media;
using ChartControls.CommonModels.DataModels;
using ChartControls.Contracts;

namespace WPFChart
{
    public sealed class ChartDataGenerator
    {
        public event EventHandler<ISeriesData> OnData;

        private int lastRandVal = 0;
        private readonly Timer _timer;
        private readonly Random _random;

        public double Interval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        public ChartDataGenerator(double interval = 100)
        {
            _random = new Random();
            _timer = new Timer(interval);
            _timer.Elapsed += _timer_Elapsed;
        }


        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.Current?.Dispatcher.Invoke(() => OnOnData(new SeriesValue((long)GetRandom(), GetRandom())));
        }

        private double GetRandom()
        {
            int value = _random.Next(lastRandVal - 10, lastRandVal + 10);
            lastRandVal = value;
            if (lastRandVal < 0)
                lastRandVal = 0;
            return value;
        }

        private Brush GetRandomBrush()
        {
            byte
                a = (byte)_random.Next(90, 255),
                r = (byte)_random.Next(0, 255),
                g = (byte)_random.Next(0, 255),
                b = (byte)_random.Next(0, 255);
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        private void OnOnData(ISeriesData e)
        {
            OnData?.Invoke(this, e);
        }
    }
}
