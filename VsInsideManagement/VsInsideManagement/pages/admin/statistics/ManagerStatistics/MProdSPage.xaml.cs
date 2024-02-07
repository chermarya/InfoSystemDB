using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using VsInsideManagement.library;
using VsInsideManagement.library.statistics;

namespace VsInsideManagement.pages.admin.statistics.ManagerStatistics
{
    public partial class MProdSPage : Page
    {
        private List<MProductivity> Data;
        public SeriesCollection PieSeriesCollection { get; set; }
        private ObservableCollection<LeaderboardItem> Leaderboard = new ObservableCollection<LeaderboardItem>();
        private double sum = 0;
        
        public MProdSPage(List<MProductivity> data)
        {
            Data = data;
            
            InitializeComponent();
            
            LoadDataDiagram();
            TotalProd.Content += sum.ToString();

            LoadLiderboard();
            LeaderboardListView.ItemsSource = Leaderboard;
        }
        
        private void LoadLiderboard()
        {
            DataContext = Leaderboard;
            foreach (MProductivity i in Data)
            {
                Leaderboard.Add(new LeaderboardItem(i.Manager, i.ProductCount));
            }
        }

        private void LoadDataDiagram()
        {
            Dictionary<string, double> managers = new Dictionary<string, double>();

            sum = 0;

            foreach (MProductivity i in Data)
            {
                sum += i.ProductCount;
            }

            foreach (MProductivity i in Data)
            {
                double per = (100 * i.ProductCount) / sum;

                managers.Add(i.Manager, per);
            }

            PieSeriesCollection = new SeriesCollection();

            foreach (string i in managers.Keys)
            {
                AddPieSeries(i, managers[i]);
            }

            PieChart.Series = PieSeriesCollection;
        }

        private void AddPieSeries(string title, double value)
        {
            var newSeries = new PieSeries
            {
                Title = title,
                Values = new ChartValues<double> { value }
            };

            PieSeriesCollection.Add(newSeries);
        }
    }
}