using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class BuyerListPage : Page
    {
        public BuyerListPage()
        {
            InitializeComponent();
            
            DGridBuyers.ItemsSource = VsInsideDBEntities.Content().Buyer.ToList();
            DGridBuyers.SelectedIndex = 0;
            
            DGridBuyers.SelectionChanged += AddressLoad;
            SurnameInput.TextChanged += BuyerLoad;
            DeleteBtn.Click += Delete;
            AddBtn.Click += Add;

            UpBtn.Visibility = Visibility.Hidden;
        }

        private void BuyerLoad(object sender, TextChangedEventArgs e)
        {
            BuyersFilter();
        }
        
        private void BuyersFilter()
        {
            List<Buyer> content = new List<Buyer>();
            string sql = $"SELECT * FROM Buyer WHERE surname LIKE '%{SurnameInput.Text}%' ORDER BY surname";

            SqlDataReader reader = new DoSql(sql, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                foreach (Buyer i in VsInsideDBEntities.Content().Buyer.ToList())
                {
                    if (i.buyer_id == reader.GetInt32(0))
                        content.Add(i);
                }
            }

            if (SurnameInput.Text == "")
                DGridBuyers.ItemsSource = VsInsideDBEntities.Content().Buyer.ToList();
            else
            {
                List<Buyer> newList = new List<Buyer>();
                
                foreach (Buyer i in content)
                {
                    newList.Add(i);
                }

                DGridBuyers.ItemsSource = newList;

                if (newList.Count == 0 || newList.Count == 1)
                {
                    DownBtn.Visibility = Visibility.Hidden;
                    UpBtn.Visibility = Visibility.Hidden;
                }
            }
            
            DGridBuyers.SelectedIndex = 0;
            AddressFilter();
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
                if (DGridBuyers.SelectedIndex == DGridBuyers.Items.Count - 1)
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
            
            AddressFilter();
        }

        private void AddressFilter()
        {
            int buyer_id = 0;
            try
            {
                buyer_id = ((Buyer)DGridBuyers.SelectedItem).buyer_id;
            }
            catch
            {
            }

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
            new BuyerSettings(DGridBuyers, 0, new Buyer(), AddressFilter).Show();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            if (DGridBuyers.SelectedIndex == -1)
                return;

            new BuyerSettings(DGridBuyers, 1, (Buyer)DGridBuyers.SelectedItem, AddressFilter).Show();
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
                }
            ).ToExecuteQuery();

            BuyersFilter();
            DGridBuyers.SelectedIndex = row - 1;
        }

        private void AddAddress(object sender, RoutedEventArgs e)
        {
            if (DGridBuyers.SelectedIndex == -1)
                return;
            new AddressSetings(((Buyer)DGridBuyers.SelectedItem).buyer_id, AddressFilter).Show();
        }

        private void DelAddress(object sender, RoutedEventArgs e)
        {
            if (DGridAddresses.SelectedIndex == -1)
                return;

            int del_id = 0;
            string sql = "SELECT delivery_id FROM Delivery d JOIN Buyer b ON d.buyer_id = b.buyer_id JOIN DelAddress " +
                         "da ON da.address_id = d.address_id WHERE d.buyer_id = @buyer AND d.address_id = @address";

            int buyer = ((Buyer)DGridBuyers.SelectedItem).buyer_id;
            int address = ((DelAddress)DGridAddresses.SelectedItem).address_id;
            
            SqlDataReader reader = new DoSql(sql, new SqlParameter[]
            {
                new SqlParameter("@buyer", buyer),
                new SqlParameter("@address", address)
            }).ToReadQuery();

            while (reader.Read())
            {
                del_id = reader.GetInt32(0);
            }

            new DoSql("DELETE FROM Delivery WHERE delivery_id = @id",
                new SqlParameter[]
                {
                    new SqlParameter("@id", del_id)
                }
            ).ToExecuteQuery();

            AddressFilter();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}