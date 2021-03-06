﻿using ChartControls.Contracts;

namespace ChartControls.CommonModels.DataModels
{
    public sealed class Scope
    {
        public double MinX { get; private set; }
        public double MaxX { get; private set; }
        public double MinY { get; private set; }
        public double MaxY { get; private set; }


        public Scope()
        {
            MinX = MinY = double.MaxValue;
            MaxX = MaxY = double.MinValue;
        }

        public Scope(double minX, double maxX, double minY, double maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        internal void UpdateBy(ISeriesData item)
        {
            if (item.ValueX < MinX) MinX = item.ValueX;
            if (item.ValueX > MaxX) MaxX = item.ValueX;
            foreach (var dataY in item.Data)
            {
                if (dataY < MinY) MinY = dataY;
                if (dataY > MaxY) MaxY = dataY;
            }
        }

        internal void UpdateBy(Scope scope)
        {
            if (!double.IsNaN(scope.MinX))
                this.MinX = scope.MinX;
            if (!double.IsNaN(scope.MaxX))
                this.MaxX = scope.MaxX;

            if (!double.IsNaN(scope.MinY))
                this.MinY = scope.MinY;
            if (!double.IsNaN(scope.MaxY))
                this.MaxY = scope.MaxY;
        }

        public override string ToString()
        {
            return $"X: [{MinX}; {MaxX}], Y: [{MinY}; {MaxY}]";
        }
    }
}