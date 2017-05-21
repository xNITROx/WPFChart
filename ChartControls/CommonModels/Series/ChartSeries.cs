using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media;
using ChartControls.Contracts;

namespace ChartControls.CommonModels.Series
{
    public abstract class ChartSeries : IChartSeries
    {
        private bool _isUpdated = false;
        private Drawing _geometry;


        public ObservableCollection<ISeriesData> Data { get; } = new ObservableCollection<ISeriesData>();
        public bool IsVisible { get; set; } = true;


        protected ChartSeries()
        {
            Data.CollectionChanged += Data_CollectionChanged;
        }


        public Drawing GetGeometry()
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

            return new GeometryDrawing();
        }

        protected abstract Drawing DrawGeometry();

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _isUpdated = false;
        }
    }
}