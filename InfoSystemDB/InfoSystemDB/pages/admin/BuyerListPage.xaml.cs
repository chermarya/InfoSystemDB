using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class BuyerListPage : Page
    {
        private Frame MainFrame;
        
        public BuyerListPage(Frame MainFrame)
        {
            InitializeComponent();

            DGridBuyers.ItemsSource = VsInsideDBEntities.GetContent().Buyer.ToList();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            
        }
        
        private void Edit(object sender, RoutedEventArgs e)
        {
            
        }
        
        private void Delete(object sender, RoutedEventArgs e)
        {
            
        }
    }
}