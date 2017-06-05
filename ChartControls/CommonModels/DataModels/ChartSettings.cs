using System;
using System.Windows;
using ChartControls.Contracts;

namespace ChartControls.CommonModels.DataModels
{
    public sealed class ChartSettings : ICloneable
    {
        public Scope Scope { get; }
        public Scope ViewScope { get; }
        public Size Size { get; set; }


        public ChartSettings()
        {
            Scope = new Scope();
            ViewScope = new Scope();
        }


        public bool ViewContains(ISeriesData item)
        {
            return item.ValueX >= ViewScope.MinX && item.ValueX <= ViewScope.MaxX;
        }

        public Point ConvertToPoint(ISeriesData item, int yIndex = 0)
        {
            double x = ConvertToX(item.ValueX);
            double y = ConvertToY(item.Data[yIndex]);

            return new Point(x, y);
        }

        public double ConvertToX(double dataX)
        {
            double x1 = (ViewScope.MaxX - ViewScope.MinX) / Size.Width;
            double x = (dataX - ViewScope.MinX) / x1;
            return x;
        }

        public double ConvertToY(double dataY)
        {
            double y1 = (Scope.MaxY - Scope.MinY) / Size.Height;
            double y = (dataY - Scope.MinY) / y1;

            // reverse Y coordinate to usability view
            y = Size.Height - y;
            return y;
        }

        public ChartSettings Clone()
        {
            return (ChartSettings)MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}