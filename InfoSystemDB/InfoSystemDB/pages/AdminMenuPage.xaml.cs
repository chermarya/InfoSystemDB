using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class AdminMenuPage : Page
    {
        private Frame MainFrame;
        
        public AdminMenuPage(Frame MainFrame)
        {
            this.MainFrame = MainFrame;
            
            InitializeComponent();
        }

        private void ProdShow(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ProductsPage();
        }
        
        private void ManagersShow(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManagersPage();
        }
        
        private void ColorsShow(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ColorsPage();
        }
        
        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}