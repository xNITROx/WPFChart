using ChartControls.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace ChartControls.CommonModels
{
    public sealed class ChartPainters : List<IChartPainter>
    {
        internal List<T> GetProviders<T>()
            where T : IChartPainter
        {
            return this.Where(x => x is T).Cast<T>().ToList();
        }
    }
}