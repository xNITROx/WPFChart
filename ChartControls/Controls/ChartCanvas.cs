using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ChartControls.CommonModels;
using ChartControls.CommonModels.DataModels;

namespace ChartControls.Controls
{
    internal sealed class ChartCanvas : UIElement
    {
        private readonly GeometryDrawing _pointerDrawing;
        private SeriesDrawingGeometry[] _seriesDrawingGeometrys;


        public ChartCanvas()
        {
            _pointerDrawing = new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Black, 0.11) { DashStyle = new DashStyle(new double[] { 50 }, 0) }, Geometry.Empty);
            this.MouseMove += ChartCanvas_MouseMove;
            this.MouseLeave += ChartCanvas_MouseLeave;
        }

        public void DrawSeries(SeriesDrawingGeometry[] seriesGeometry)
        {
            _seriesDrawingGeometrys = seriesGeometry;
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Gray, new Pen(), new Rect(this.RenderSize));

            if (_seriesDrawingGeometrys != null && _seriesDrawingGeometrys.Length > 0)
                foreach (var geo in _seriesDrawingGeometrys)
                    drawingContext.DrawGeometry(geo.Brush, geo.Pen, geo.Geometry);

            drawingContext.DrawDrawing(_pointerDrawing);

            base.OnRender(drawingContext);
        }

        private void ChartCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point startPoint, endPoint, mouse = e.GetPosition(this);
            string lines = string.Empty;

            startPoint = new Point(0, mouse.Y);
            endPoint = new Point(this.RenderSize.Width, mouse.Y);
            lines += $"M {startPoint.GetString()} {endPoint.GetString()} z";
            startPoint = new Point(mouse.X, 0);
            endPoint = new Point(mouse.X, this.RenderSize.Height);
            lines += $"M {startPoint.GetString()} {endPoint.GetString()} z";

            _pointerDrawing.Geometry = Geometry.Parse(lines);
        }

        private void ChartCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            _pointerDrawing.Geometry = Geometry.Empty;
        }
    }
}