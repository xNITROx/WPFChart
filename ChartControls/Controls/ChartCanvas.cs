using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChartControls
{
    internal sealed class ChartCanvas : BaseChartControl
    {
        private Grid _layersContainer;
        private readonly Path _chartPath;


        public ChartCanvas()
        {
            _chartPath = new Path();
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _layersContainer = this.GetTemplateChild("CANVAS_LAYERS_CONTAINTER") as Grid;
            if (_layersContainer != null)
            {
                _layersContainer.Children.Add(_chartPath);
            }
        }

        public void DrawChart(Geometry chartGeo)
        {
            if (_chartPath != null)
                _chartPath.Data = chartGeo;
        }
    }
}