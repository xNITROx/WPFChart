using ChartControls.Contracts;

namespace ChartControls.CommonModels.DataModels
{
    public struct SeriesValue : ISeriesData
    {
        public long ValueX { get; }

        public double[] Data { get; }
        public string[] Labels { get; }


        public SeriesValue(long valueX, double[] data, string[] labels = null)
        {
            ValueX = valueX;
            Data = data ?? new double[] { };
            Labels = labels ?? new string[] { };
        }

        public SeriesValue(long valueX, double data, string label = null)
        {
            ValueX = valueX;
            Data = new double[] { data };
            if (label != null)
                Labels = new string[] { label };
            else
                Labels = new string[] { };
        }
    }
}