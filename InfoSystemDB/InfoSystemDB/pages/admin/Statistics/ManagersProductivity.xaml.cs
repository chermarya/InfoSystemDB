using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

namespace InfoSystemDB
{
    public partial class ManagersProductivity : Page
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }

        public ManagersProductivity()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "2019",
                    Values = new ChartValues<double> { 10, 50, 39, 50 }
                },
                new ColumnSeries
                {
                    Title = "2020",
                    Values = new ChartValues<double> { 11, 56, 42, 48 }
                }
            };
            
            Labels = new[] { "Apple", "Banana", "Orange", "Grapes" };

            DataContext = this;
        }
    }
}