using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class BuyerListPage : Page
    {
        //private Buyer selectedBuyer;

        public BuyerListPage()
        {
            InitializeComponent();

            DGridBuyers.ItemsSource = VsInsideDBEntities.Content().Buyer.ToList();
            DGridBuyers.SelectionChanged += AddressLoad;
            DGridBuyers.SelectedIndex = 0;

            UpBtn.Visibility = Visibility.Hidden;
        }

        private void AddressLoad(object sender, RoutedEventArgs e)
        {
            if (DGridBuyers.SelectedIndex == 0)
            {
                UpBtn.Visibility = Visibility.Hidden;
                DownBtn.Visibility = Visibility.Visible;
            }
            else
            {
                if (DGridBuyers.SelectedIndex == VsInsideDBEntities.Content().Buyer.ToList().Count - 1)
                {
                    DownBtn.Visibility = Visibility.Hidden;
                    UpBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    UpBtn.Visibility = Visibility.Visible;
                    DownBtn.Visibility = Visibility.Visible;
                }
            }
            
            int buyer_id = ((Buyer)DGridBuyers.SelectedItem).buyer_id;

            List<DelAddress> addr = new List<DelAddress>();
            string expr = "SELECT da.address_id, city, dep, note FROM DelAddress da JOIN Delivery d ON d.address_id " +
                          "= da.address_id WHERE d.buyer_id = @buyer_id";

            SqlDataReader reader = new DoSql(expr, new SqlParameter[]
            {
                new SqlParameter("@buyer_id", buyer_id)
            }).ToReadQuery();

            while (reader.Read())
            {
                foreach (DelAddress i in VsInsideDBEntities.Content().DelAddress.ToList())
                {
                    if (i.address_id == reader.GetInt32(0))
                        addr.Add(i);
                }
            }

            DGridAddresses.ItemsSource = addr;
        }

        private void UpBtnClick(object sender, RoutedEventArgs e)
        {
            DGridBuyers.SelectedIndex--;

            if (DGridBuyers.SelectedIndex == 0)
                UpBtn.Visibility = Visibility.Hidden;

            DownBtn.Visibility = Visibility.Visible;
        }

        private void DownBtnClick(object sender, RoutedEventArgs e)
        {
            DGridBuyers.SelectedIndex++;

            if (DGridBuyers.SelectedIndex == VsInsideDBEntities.Content().Buyer.ToList().Count - 1)
                DownBtn.Visibility = Visibility.Hidden;

            UpBtn.Visibility = Visibility.Visible;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            new BuyerSettings(DGridBuyers).Show();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (DGridBuyers.SelectedIndex == -1)
                return;

            int row = DGridBuyers.SelectedIndex;

            int id = ((Buyer)DGridBuyers.SelectedItem).buyer_id;

            new DoSql("DELETE FROM Buyer WHERE buyer_id = @id",
                new SqlParameter[]
                {
                    new SqlParameter("@id", id)
                }).ToExecuteQuery();

            DGridBuyers.ItemsSource = VsInsideDBEntities.Content().Buyer.ToList();
            DGridBuyers.SelectedIndex = row - 1;
        }

        private void AddAddress(object sender, RoutedEventArgs e)
        {
            new AddressSetings().Show();
        }
    }
}