using System;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Media;
using ChartControls.Contracts;

namespace ChartControls.CommonModels.Series
{
    public sealed class LineSeries : ChartSeries
    {
        private readonly StringBuilder _data = new StringBuilder("M");


        public Brush Brush { get; set; } = Brushes.Black;
        public double Width { get; set; } = 1;



        public LineSeries()
        {
            Data.CollectionChanged += Data_CollectionChanged;
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var format = Extentions.DefaultFormat;
                foreach (ISeriesData item in e.NewItems)
                    _data.Append($" {item.ValueX.ToString(format)},{item.Data[0].ToString(format)}");
            }
        }


        protected override Drawing DrawGeometry()
        {
            Geometry geometry = null;
            try
            {
                geometry = Geometry.Parse(_data.ToString());
                if (geometry.CanFreeze)
                    geometry.Freeze();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            Pen pen = new Pen(Brush, Width);
            return new GeometryDrawing(Brushes.Transparent, pen, geometry);
        }
    }
}