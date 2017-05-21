using System.Windows;
using ChartControls.CommonModels.Series;
using ChartControls.Contracts;

namespace WPFChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChartDataGenerator _dataGenerator;


        public MainWindow()
        {
            _dataGenerator = new ChartDataGenerator(100);
            _dataGenerator.OnData += _dataGenerator_OnData;
            InitializeComponent();

            myChart.Series.Add(new LineSeries());
            _dataGenerator.Start();
        }


        private void _dataGenerator_OnData(object sender, ISeriesData data)
        {
            var lastDataSource = myChart.Series[myChart.Series.Count - 1].Data;

            data.ValueX = (long) lastDataSource.Count + 10;
            lastDataSource.Add(data);

            myChart.UpdateSeries();
        }
    }
}