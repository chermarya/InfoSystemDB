using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InfoSystemDB
{
    public partial class StatisticsPage : Page
    {
        private Frame MainFrame;
        public StatisticsPage(Frame MainFrame)
        {
            this.MainFrame = MainFrame;
            
            InitializeComponent();
            
            ContentFrame.Content = new ManagersProductivity();
        }

        private void ManagersProductivity(object sender, RoutedEventArgs e)
        {
            BeginColor1.Color = Colors.Yellow;
            BeginColor2.Color = Colors.White;
            BeginColor3.Color = Colors.White;
            BeginColor4.Color = Colors.White;
            
            ContentFrame.Content = new ManagersProductivity();
        }
        
        private void PopularProduct(object sender, RoutedEventArgs e)
        {
            BeginColor1.Color = Colors.White;
            BeginColor2.Color = Colors.Yellow;
            BeginColor3.Color = Colors.White;
            BeginColor4.Color = Colors.White;
            
            ContentFrame.Content = new PopularProductColor();
        }
        
        private void AvgSupply(object sender, RoutedEventArgs e)
        {
            BeginColor1.Color = Colors.White;
            BeginColor2.Color = Colors.White;
            BeginColor3.Color = Colors.Yellow;
            BeginColor4.Color = Colors.White;
            
            ContentFrame.Content = new AverageSupply();
        }
        
        private void RegularBuyers(object sender, RoutedEventArgs e)
        {
            BeginColor1.Color = Colors.White;
            BeginColor2.Color = Colors.White;
            BeginColor3.Color = Colors.White;
            BeginColor4.Color = Colors.Yellow;
            
            ContentFrame.Content = new RegularBuyers();
        }
        
        private void Exit(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AdminMenuPage(MainFrame);
        }
    }
}