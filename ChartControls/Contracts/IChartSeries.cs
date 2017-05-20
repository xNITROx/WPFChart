using System.Collections.ObjectModel;
using ChartControls.CommonModels.DataModels;

namespace ChartControls.Contracts
{
    public interface IChartSeries
    {
        ObservableCollection<ISeriesData> Data { get; }
        bool IsVisible { get; set; }

        SeriesDrawingGeometry GetGeometry();
    }
}