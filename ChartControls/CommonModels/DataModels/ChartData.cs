using ChartControls.Contracts;
using System;

namespace ChartControls
{
    public struct ChartData : IChartData
    {
        public DateTime DateTime { get; private set; }
        public long Ticks => DateTime.Ticks;

        public double[] Data { get; private set; }


        public ChartData(DateTime dateTime, params double[] data)
        {
            DateTime = dateTime;
            Data = data ?? new double[] { };
        }

        public ChartData(long dateTime, params double[] data)
            : this(new DateTime(dateTime), data)
        {
        }
    }
}