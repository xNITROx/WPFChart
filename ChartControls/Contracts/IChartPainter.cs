using System.Windows.Media;

namespace ChartControls.Contracts
{
    public interface IChartPainter
    {
        Geometry Draw(IDataSource dataSource);
    }
}