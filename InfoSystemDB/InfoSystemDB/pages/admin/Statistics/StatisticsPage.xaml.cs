using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InfoSystemDB
{
    public partial class StatisticsPage : Page
    {
        private Frame MainFrame;
        private string dateString = "";

        public StatisticsPage(Frame MainFrame)
        {
            this.MainFrame = MainFrame;

            InitializeComponent();

            DPBegin.SelectedDateChanged += BeginChanged;
            DPEnd.SelectedDateChanged += EndChanged;

            ContentFrame.Content = new ManagersProductivity(dateString);
        }

        private void BeginChanged(object sender, SelectionChangedEventArgs e)
        {
            DateChanged();
        }

        private void EndChanged(object sender, SelectionChangedEventArgs e)
        {
            DateChanged();
        }

        private void DateChanged()
        {
            DateCount();

            if (BeginColor1.Color == Colors.Yellow)
                ContentFrame.Content = new ManagersProductivity(dateString);

            if (BeginColor2.Color == Colors.Yellow)
                ContentFrame.Content = new PopularProductColor(dateString);

            if (BeginColor3.Color == Colors.Yellow)
                ContentFrame.Content = new AverageSupply(dateString);

            if (BeginColor4.Color == Colors.Yellow)
                ContentFrame.Content = new RegularBuyers(dateString);
        }

        private void DateCount()
        {
            if (DPBegin.SelectedDate == null && DPEnd.SelectedDate == null)
                dateString = "";

            if (DPBegin.SelectedDate != null && DPEnd.SelectedDate == null)
                dateString = $" so.ddate >= '{DateConverter(DPBegin.Text)}'";

            if (DPBegin.SelectedDate == null && DPEnd.SelectedDate != null)
                dateString = $" so.ddate <= '{DateConverter(DPEnd.Text)}'";

            if (DPBegin.SelectedDate != null && DPEnd.SelectedDate != null)
                dateString =
                    $" so.ddate BETWEEN '{DateConverter(DPBegin.Text)}' AND '{DateConverter(DPEnd.Text)}' ";
        }

        private string DateConverter(string date)
        {
            string[] d = date.Split('.');
            return d[2] + "/" + d[1] + "/" + d[0];
        }

        private void ManagersProductivity(object sender, RoutedEventArgs e)
        {
            BeginColor1.Color = Colors.Yellow;
            BeginColor2.Color = Colors.White;
            BeginColor3.Color = Colors.White;
            BeginColor4.Color = Colors.White;

            ContentFrame.Content = new ManagersProductivity(dateString);
        }

        private void PopularProduct(object sender, RoutedEventArgs e)
        {
            BeginColor1.Color = Colors.White;
            BeginColor2.Color = Colors.Yellow;
            BeginColor3.Color = Colors.White;
            BeginColor4.Color = Colors.White;

            ContentFrame.Content = new PopularProductColor(dateString);
        }

        private void AvgSupply(object sender, RoutedEventArgs e)
        {
            BeginColor1.Color = Colors.White;
            BeginColor2.Color = Colors.White;
            BeginColor3.Color = Colors.Yellow;
            BeginColor4.Color = Colors.White;

            ContentFrame.Content = new AverageSupply(dateString);
        }

        private void RegularBuyers(object sender, RoutedEventArgs e)
        {
            BeginColor1.Color = Colors.White;
            BeginColor2.Color = Colors.White;
            BeginColor3.Color = Colors.White;
            BeginColor4.Color = Colors.Yellow;

            ContentFrame.Content = new RegularBuyers(dateString);
        }

        private void Drop(object sender, RoutedEventArgs e)
        {
            DPBegin.SelectedDate = null;
            DPEnd.SelectedDate = null;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AdminMenuPage(MainFrame);
        }
    }
}