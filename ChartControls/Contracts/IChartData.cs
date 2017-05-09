using System;

namespace ChartControls.Contracts
{
    public interface IChartData
    {
        DateTime DateTime { get; }
        long Ticks { get; }
        double[] Data { get; }
    }
}