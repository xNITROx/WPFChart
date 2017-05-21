using System.Collections.ObjectModel;
using System.Windows.Media;

namespace ChartControls.Contracts
{
    public interface IChartSeries
    {
        ObservableCollection<ISeriesData> Data { get; }
        bool IsVisible { get; set; }

        Drawing GetGeometry();
    }
}