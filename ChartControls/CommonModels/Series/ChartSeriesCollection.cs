using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChartControls.Contracts;

namespace ChartControls.CommonModels.Series
{
    public sealed class ChartSeriesCollection : ObservableCollection<IChartSeries>
    {
        internal List<T> GetProviders<T>()
            where T : IChartSeries
        {
            return this.Where(x => x is T).Cast<T>().ToList();
        }
    }
}