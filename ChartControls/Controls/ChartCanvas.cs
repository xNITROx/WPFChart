using System;
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
        private DrawingGroup _seriesDrawingGroup;


        public ChartCanvas()
        {
            _pointerDrawing = new GeometryDrawing(Brushes.Transparent,
                new Pen(Brushes.DimGray, 0.11) { DashStyle = new DashStyle(new double[] { 50 }, 0) }, null);
            _seriesDrawingGroup = new DrawingGroup();
            this.MouseMove += ChartCanvas_MouseMove;
            this.MouseLeave += ChartCanvas_MouseLeave;
        }

        public void DrawSeries(Drawing[] seriesDrawings)
        {
            _seriesDrawingGroup.Children.Clear();
            foreach (var drawing in seriesDrawings)
                _seriesDrawingGroup.Children.Add(drawing);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // draw background layer for mouse handle
            drawingContext.DrawRectangle(Brushes.Transparent, new Pen(), new Rect(this.RenderSize));

            // draw series
            if (_seriesDrawingGroup != null)
                drawingContext.DrawDrawing(_seriesDrawingGroup);

            // draw cross lines
            drawingContext.DrawDrawing(_pointerDrawing);

            base.OnRender(drawingContext);
        }

        private void ChartCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point startPoint, endPoint, mouse = Mouse.GetPosition(this);
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
            _pointerDrawing.Geometry = null;
        }
    }
}