using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using ChartControls.CommonModels.DataModels;
using ChartControls.CommonModels.Series;
using ChartControls.Contracts;
using ChartControls.Controls;

namespace ChartControls
{
    public sealed class Chart : BaseChartControl
    {
        private ChartCanvas _canvas;
        private readonly ChartSettings _settings;


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
            var chart = (Chart)d;
            var oldCollection = e.OldValue as ChartSeriesCollection;
            if (oldCollection != null)
                oldCollection.CollectionChanged -= chart.SeriesOnCollectionChanged;
            var newCollection = e.NewValue as ChartSeriesCollection;
            if (newCollection != null)
                newCollection.CollectionChanged += chart.SeriesOnCollectionChanged;
        }

        #endregion


        public Chart()
        {
            _settings = new ChartSettings();
            Series = new ChartSeriesCollection();
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

            List<Drawing> seriesDrawings = new List<Drawing>();
            foreach (var series in Series)
            {
                var newGeo = series.GetGeometry();
                if (newGeo == null)
                    continue;
                seriesDrawings.Add(newGeo);
            }

            _canvas.DrawSeries(seriesDrawings.ToArray());
        }

        private void SeriesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (IChartSeries series in e.OldItems)
                    series.Data.CollectionChanged -= SeriesData_CollectionChanged;

            if (e.NewItems != null)
                foreach (IChartSeries series in e.NewItems)
                    series.Data.CollectionChanged += SeriesData_CollectionChanged;
        }

        private void SeriesData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ISeriesData data in e.NewItems)
                    _settings.Scope.UpdateBy(data);
        }
    }
}