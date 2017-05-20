using System;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Media;
using ChartControls.CommonModels.DataModels;
using ChartControls.Contracts;

namespace ChartControls.CommonModels.Series
{
    public sealed class LineSeries : ChartSeries
    {
        private Pen _pen = new Pen(Brushes.Black, 1);
        private readonly StringBuilder _data = new StringBuilder("M");
        public IFormatProvider Format { get; } = Extentions.DefaultFormat;


        public LineSeries()
        {
            Data.CollectionChanged += Data_CollectionChanged;
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ISeriesData item in e.NewItems)
                    _data.Append($" {item.ValueX.ToString(Format)},{item.Data[0].ToString(Format)}");
            }
        }


        protected override SeriesDrawingGeometry DrawGeometry()
        {
            Geometry geometry = Geometry.Parse(_data.ToString());
            if (geometry.CanFreeze)
                geometry.Freeze();

            return new SeriesDrawingGeometry(Brushes.Transparent, _pen, geometry);
        }
    }
}