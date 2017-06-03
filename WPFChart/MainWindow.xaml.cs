using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            _dataGenerator = new ChartDataGenerator(50);
            _dataGenerator.OnData += _dataGenerator_OnData;
            {// add context menu
                this.ContextMenu = new ContextMenu();
                MenuItem item = new MenuItem();
                item.Header = "Start";
                item.Click += (sender, args) => _dataGenerator.Start();
                this.ContextMenu.Items.Add(item);
                item = new MenuItem();
                item.Header = "Stop";
                item.Click += (sender, args) => _dataGenerator.Stop();
                this.ContextMenu.Items.Add(item);
            }

            InitializeComponent();
            myChart.Series.Add(new LineSeries
            {
                Brush = (Brush)App.Current.Resources["PurpleBrush"],
                Width = 2
            });
            _dataGenerator.Start();
        }

        private void _dataGenerator_OnData(object sender, ISeriesData data)
        {
            var lastDataSource = myChart.Series[myChart.Series.Count - 1].Data;

            data.ValueX = (long)(lastDataSource.Count * 2);
            lastDataSource.Add(data);

            myChart.UpdateSeries();
        }
    }
}