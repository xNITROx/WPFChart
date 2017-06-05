using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ChartControls.CommonModels.Series
{
    public sealed class LineSeries : ChartSeries
    {
        public Brush Fill { get; set; } = Brushes.Transparent;
        public Brush Brush { get; set; } = Brushes.Black;
        public double Width { get; set; } = 1;


        public LineSeries()
        {
        }


        protected override Drawing DrawGeometry()
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < Data.Count; i++)
            {
                var seriesData = Data[i];
                if (ChartSettings.ViewContains(seriesData))
                    points.Add(ChartSettings.ConvertToPoint(seriesData));
            }

            if (points.Count == 0)
                return new DrawingGroup();

            var geometry = GetGeometry(points);
            Pen pen = new Pen(Brush, Width);
            return new GeometryDrawing(Fill, pen, geometry);
        }

        private Geometry GetGeometry(List<Point> points)
        {
            PathFigure figure = new PathFigure();
            figure.IsFilled = true;
            // add Start and End points to correct Fill
            figure.StartPoint = new Point(-10, ChartSettings.Size.Height);
            points.Add(new Point(ChartSettings.Size.Width + 10, ChartSettings.Size.Height));

            PolyLineSegment lineSegments = new PolyLineSegment(points, true);
            figure.Segments.Add(lineSegments);

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(figure);
            pathGeometry.Freeze();

            return pathGeometry;
        }
    }
}