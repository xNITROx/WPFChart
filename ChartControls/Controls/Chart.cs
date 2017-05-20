using System.Collections.Generic;
using System.Windows;
using ChartControls.CommonModels.DataModels;
using ChartControls.CommonModels.Series;
using ChartControls.Controls;

namespace ChartControls
{
    public sealed class Chart : BaseChartControl
    {
        private ChartCanvas _canvas;


        #region Dependency Properties

        public ChartSeriesCollection Series
        {
            get => (ChartSeriesCollection)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeriesCollection), typeof(Chart), new PropertyMetadata(default(ChartSeriesCollection), SeriesChangedCallback));
        private static void SeriesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           // TODO: handle series changed and change update logic
        }

        #endregion


        public Chart()
        {
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = this.GetTemplateChild("CHART_CANVAS") as ChartCanvas;
        }

        public void UpdateSeries()
        {
            if (_canvas == null || Series == null || Series.Count == 0)
                return;

            List<SeriesDrawingGeometry> seriesDrawingGeometrys = new List<SeriesDrawingGeometry>();
            foreach (var series in Series)
            {
                var newGeo = series.GetGeometry();
                if (newGeo == null)
                    continue;
                seriesDrawingGeometrys.Add(newGeo);
            }

            _canvas.DrawSeries(seriesDrawingGeometrys.ToArray());
        }
    }
}