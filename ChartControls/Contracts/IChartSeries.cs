using System.Collections.ObjectModel;
using System.Windows.Media;
using ChartControls.CommonModels.DataModels;

namespace ChartControls.Contracts
{
    public interface IChartSeries
    {
        ObservableCollection<ISeriesData> Data { get; }
        bool IsVisible { get; set; }

        Drawing GetGeometry(ChartSettings chartSettings);
    }
}