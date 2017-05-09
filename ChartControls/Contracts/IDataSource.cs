using System;
using System.Collections.Generic;

namespace ChartControls.Contracts
{
    public interface IDataSource
    {
        event EventHandler<IEnumerable<IChartData>> OnDataAdded;

        void Add(IChartData dataItem);
        void Add(IEnumerable<IChartData> dataItems);
    }
}