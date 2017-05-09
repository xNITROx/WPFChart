using ChartControls.Contracts;
using System.Windows.Media;
using System.Collections.Generic;
using ChartControls.CommonModels;

namespace ChartControls
{
    public sealed class Chart : BaseChartControl
    {
        private IDataSource _dataSource;
        private ChartCanvas _canvas;


        public IDataSource DataSource
        {
            get { return _dataSource; }
            private set
            {
                if (_dataSource != null)
                    _dataSource.OnDataAdded -= DataSource_OnDataAdded;
                if (value != null)
                    value.OnDataAdded += DataSource_OnDataAdded;

                _dataSource = value;
            }
        }

        public ChartPainters ChartPainters { get; set; }


        public Chart()
        {
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = this.GetTemplateChild("CHART_CANVAS") as ChartCanvas;
        }

        private void DataSource_OnDataAdded(object sender, IEnumerable<IChartData> newData)
        {
            if (ChartPainters != null && ChartPainters.Count > 0 && _canvas != null)
            {
                IDataSource dataSource = (IDataSource)sender;

                Geometry chartGeo = null;
                foreach (var painter in ChartPainters)
                    if (chartGeo == null)
                        chartGeo = painter.Draw(dataSource);
                    else
                        chartGeo = new CombinedGeometry(chartGeo, painter.Draw(dataSource));

                _canvas.DrawChart(chartGeo);
            }
        }
    }
}