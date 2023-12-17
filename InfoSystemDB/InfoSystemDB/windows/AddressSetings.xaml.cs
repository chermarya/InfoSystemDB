using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace InfoSystemDB
{
    public partial class AddressSetings : Window
    {
        private int buyer_id;
        private Action Function;

        public AddressSetings(int id, Action func)
        {
            buyer_id = id;
            Function = func;
            InitializeComponent();

            DGAddress.ItemsSource = VsInsideDBEntities.Content().DelAddress.ToList();

            CityInput.TextChanged += Filter;
            DepInput.TextChanged += Filter;
        }

        private void CDSchecked(object sender, RoutedEventArgs e)
        {
            CBNew.IsChecked = false;
            ChooseGrid.Visibility = Visibility.Visible;
            NewGrid.Visibility = Visibility.Collapsed;
        }

        private void CDNchecked(object sender, RoutedEventArgs e)
        {
            CBSelect.IsChecked = false;
            ChooseGrid.Visibility = Visibility.Collapsed;
            NewGrid.Visibility = Visibility.Visible;
        }
        
        private void CityChecked(object sender, RoutedEventArgs e)
        {
            if (KhCity.IsChecked == true)
            {
                AllCity.Visibility = Visibility.Collapsed;
                KhCityList.Visibility = Visibility.Visible;

                if (KhCityList.Items.IsEmpty)
                {
                    new Parser(KhCityList);
                    KhCityList.SelectedIndex = 0;
                }
            }
            else
            {
                AllCity.Visibility = Visibility.Visible;
                KhCityList.Visibility = Visibility.Collapsed;
            }
        }

        private void Filter(object sender, RoutedEventArgs e)
        {
            string sql =
                $"SELECT * FROM DelAddress WHERE city LIKE '%{CityInput.Text}%' AND dep LIKE '%{DepInput.Text}%'";

            List<DelAddress> newList = new List<DelAddress>();

            SqlDataReader reader = new DoSql(sql, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                foreach (DelAddress i in VsInsideDBEntities.Content().DelAddress.ToList())
                {
                    if (i.address_id == reader.GetInt32(0))
                        newList.Add(i);
                }
            }

            DGAddress.ItemsSource = newList;
        }

        private void Choose(object sender, RoutedEventArgs e)
        {
            string sql = "INSERT INTO Delivery (buyer_id, address_id) VALUES (@buyer, @address)";

            new DoSql(sql, new SqlParameter[]
            {
                new SqlParameter("@buyer", buyer_id),
                new SqlParameter("@address", ((DelAddress)DGAddress.SelectedItem).address_id)
            }).ToExecuteQuery();

            Function();
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                string city = NewCityInput.Text;
                string dep = NewDepInput.Text;
                string note = NoteInput.Text;

                string create = "INSERT INTO DelAddress (city, dep, note) VALUES (@city, @dep, @note)";

                new DoSql(create, new SqlParameter[]
                {
                    new SqlParameter("@city", city),
                    new SqlParameter("@dep", dep),
                    new SqlParameter("@note", note)
                }).ToExecuteQuery();

                SqlDataReader reader = new DoSql("SELECT TOP 1 address_id FROM DelAddress ORDER BY address_id DESC",
                    new SqlParameter[] { }).ToReadQuery();

                int address_id = 0;
                while (reader.Read())
                {
                    address_id = reader.GetInt32(0);
                }

                string add = "INSERT INTO Delivery (buyer_id, address_id) VALUES (@buyer, @address)";

                new DoSql(add, new SqlParameter[]
                {
                    new SqlParameter("@buyer", buyer_id),
                    new SqlParameter("@address", address_id)
                }).ToExecuteQuery();

                Function();
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
            }
        }

        private bool Validate()
        {
            if (KhCity.IsChecked == false)
            {
                if (NewCityInput.Text == "")
                {
                    MessageBox.Show("Місто не заповнено.");
                    return false;
                }

                if (NewDepInput.Text == "")
                {
                    MessageBox.Show("Відділення не заповнено.");
                    return false;
                }
            }

            return true;
        }
    }
}