using System.Windows.Media;

namespace ChartControls.CommonModels.DataModels
{
    public sealed class SeriesDrawingGeometry
    {
        public Brush Brush { get; }
        public Pen Pen { get; }
        public Geometry Geometry { get; }


        public SeriesDrawingGeometry()
            : this(Brushes.Transparent, new Pen(), Geometry.Empty)
        {
        }

        public SeriesDrawingGeometry(Brush brush, Pen pen, Geometry geometry)
        {
            Brush = brush;
            Pen = pen;
            Geometry = geometry;
        }
    }
}