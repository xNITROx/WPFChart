using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ChartControls.CommonModels.DataModels;
using ChartControls.Contracts;

namespace ChartControls.CommonModels.Series
{
    public abstract class ChartSeries : IChartSeries
    {
        private bool _isUpdated = false;
        private SeriesDrawingGeometry _geometry;


        public ObservableCollection<ISeriesData> Data { get; } = new ObservableCollection<ISeriesData>();
        public bool IsVisible { get; set; } = true;


        protected ChartSeries()
        {
            Data.CollectionChanged += Data_CollectionChanged;
        }


        public SeriesDrawingGeometry GetGeometry()
        {
            if (IsVisible)
            {
                if (!_isUpdated)
                {
                    _geometry = DrawGeometry();
                    _isUpdated = true;
                }

                if (_geometry != null)
                    return _geometry;
            }

            return new SeriesDrawingGeometry();
        }

        protected abstract SeriesDrawingGeometry DrawGeometry();

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _isUpdated = false;
        }
    }
}