using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VsInsideManagement.library;
using VsInsideManagement.pages.admin.statistics;
using VsInsideManagement.windows;

namespace VsInsideManagement.pages.admin
{
    public partial class AdminMenuPage
    {
        private Frame MainFrame;

        public AdminMenuPage(Frame MainFrame)
        {
            this.MainFrame = MainFrame;

            InitializeComponent();
        }

        private void Orders(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new OrderListPage();
        }

        private void Managers(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManagersPage(MainFrame);
        }

        private void Buyers(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new BuyerListPage(MainFrame);
        }

        private void Products(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ProductsPage();
        }

        private void Supplies(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new SupplyListPage();
        }

        private void Tables(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TableManagmentPage();
        }

        private void Statistics(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new StatisticsPage(MainFrame);
        }

        private void Stock(object sender, RoutedEventArgs e)
        {
            List<Product> cont = new List<Product>();

            SqlDataReader reader = new DoSql("SELECT product_id FROM Product WHERE quantity > 0 ORDER BY title",
                new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                foreach (Product i in VsInsideDBEntities.Content().Product.ToList())
                {
                    if (i.product_id == reader.GetInt32(0))
                        cont.Add(i);
                }
            }

            new DocumentPreviewWindow(cont).ShowDialog();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Window wind = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            new MainWindow().Show();
            wind.Close();
        }
    }
}