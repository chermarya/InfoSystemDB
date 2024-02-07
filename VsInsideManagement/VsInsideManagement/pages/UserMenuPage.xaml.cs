using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VsInsideManagement.pages.admin;

namespace VsInsideManagement.pages
{
    public partial class UserMenuPage : Page
    {
        private Frame MainFrame;
        private Manager currUser;
        
        public UserMenuPage(Frame MainFrame, string login)
        {
            this.MainFrame = MainFrame;
            
            InitializeComponent();
            
            foreach (Manager i in VsInsideDBEntities.Content().Manager.ToList())
            {
                if (i.llogin == login)
                    currUser = i;
            }

            NameLabel.Content = currUser.nname + " " + currUser.surname;
        }

        private void Order(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new OrdersByManager(currUser.manager_id);
        }
        
        private void Buyers(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new BuyerListPage(MainFrame);
        }
        
        private void Info(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new InfoForManager();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Window wind = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            new MainWindow().Show();
            wind.Close();
        }
    }
}