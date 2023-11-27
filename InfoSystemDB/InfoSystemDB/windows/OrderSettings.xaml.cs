using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class OrderSettings : Window
    {
        private int manager_id;

        private List<Product> selectedList = new List<Product>();
        private ListItem[] discounts = new ListItem[VsInsideDBEntities.Content().Discount.ToList().Count];
        private ListItem[] shops;

        public OrderSettings(int id)
        {
            manager_id = id;

            InitializeComponent();

            DGBuyers.ItemsSource = VsInsideDBEntities.Content().Buyer.ToList();

            PrepayInput.TextChanged += PrepayChanged;
            DiscountList.SelectionChanged += DiscountSelected;

            Loader();
        }

        private void Loader()
        {
            List<Discount> discList = VsInsideDBEntities.Content().Discount.ToList();
            for (int i = 0; i < discList.Count; i++)
            {
                discounts[i] = new ListItem(discList[i].discount_id, discList[i].title + ", " + discList[i].per + "%",
                    new ListItem(discList[i].discount_id, discList[i].per.ToString()));
                DiscountList.Items.Add(discounts[i].Title);
            }

            DiscountList.SelectedIndex = 0;

            SqlDataReader reader = new DoSql(
                $"SELECT * FROM Shop WHERE manager_id = {manager_id}",
                new SqlParameter[] { }
            ).ToReadQuery();

            List<Shop> newShops = new List<Shop>();
            while (reader.Read())
            {
                foreach (Shop i in VsInsideDBEntities.Content().Shop.ToList())
                {
                    if (reader.GetInt32(0) == i.shop_id)
                        newShops.Add(i);
                }
            }

            shops = new ListItem[newShops.Count];

            for (int i = 0; i < newShops.Count; i++)
            {
                shops[i] = new ListItem(newShops[i].shop_id, newShops[i].title);
                ShopList.Items.Add(shops[i].Title);
            }
        }

        private void DiscountSelected(object sender, RoutedEventArgs e)
        {
            DiscountCounter();
        }

        private void DiscountCounter()
        {
            double order = Convert.ToDouble(OrderSumOutput.Text);
            double per = 0;
            var selStr = DiscountList.SelectedItem.ToString();
            
            foreach (ListItem i in discounts)
            {
                if (i.Title == selStr)
                    per = Convert.ToDouble(i.AddInfo.Title);
            }

            order -= order * (per / 100);
            OrderSumOutput.Text = order.ToString();
            AmountDueCounter();
        }

        private void DGContextChanged(object sender, DataGridRowEventArgs e)
        {
            int sum = 0;

            foreach (Product i in selectedList)
            {
                sum += i.price;
            }

            OrderSumOutput.Text = sum.ToString();
            DiscountCounter();
            //AmountDueCounter();
        }

        private void AmountDueCounter()
        {
            int order = Convert.ToInt32(OrderSumOutput.Text);
            int prepay = Convert.ToInt32(PrepayInput.Text);
            PaySumOutput.Text = (order - prepay).ToString();
        }

        private void PrepayChanged(object sender, RoutedEventArgs e)
        {
            AmountDueCounter();
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

        private void SelectBtn(object sender, RoutedEventArgs e)
        {
            SurnameLabel.Visibility = Visibility.Collapsed;
            SurnameInput.Visibility = Visibility.Collapsed;
            DGBuyers.Visibility = Visibility.Collapsed;
            TBOutputBuyer.Visibility = Visibility.Visible;

            Buyer selected = (Buyer)DGBuyers.SelectedItem;

            TBOutputBuyer.Text = "Нікнейм:\t" + selected.nick + "\nПрізвище:\t" + selected.surname + "\nІм'я:\t\t" +
                                 selected.nname + "\nТелефон:\t" + selected.tel;

            SetAddresses(selected);
        }

        private void SetAddresses(Buyer buyer)
        {
            List<DelAddress> addressList = new List<DelAddress>();

            SqlDataReader reader = new DoSql(
                "SELECT da.address_id, city, dep, note FROM DelAddress da  JOIN Delivery d ON d.address_id = " +
                "da.address_id JOIN Buyer ON d.buyer_id = Buyer.buyer_id WHERE d.buyer_id = @buyer_id",
                new SqlParameter[]
                {
                    new SqlParameter("@buyer_id", buyer.buyer_id)
                }).ToReadQuery();

            while (reader.Read())
            {
                foreach (DelAddress i in VsInsideDBEntities.Content().DelAddress.ToList())
                {
                    if (reader.GetInt32(0) == i.address_id)
                        addressList.Add(i);
                }
            }

            foreach (DelAddress i in addressList)
            {
                AddressList.Items.Add(i.city + " " + i.dep + " (" + i.note + ")");
            }

            AddressList.SelectedIndex = 0;
        }

        private void ChangeBtn(object sender, RoutedEventArgs e)
        {
            SurnameLabel.Visibility = Visibility.Visible;
            SurnameInput.Visibility = Visibility.Visible;
            DGBuyers.Visibility = Visibility.Visible;
            TBOutputBuyer.Visibility = Visibility.Collapsed;

            AddressList.Items.Clear();
        }

        private void SurnameChanged(object sender, RoutedEventArgs e)
        {
            List<Buyer> newList = new List<Buyer>();

            string sql = $"SELECT * FROM Buyer WHERE surname LIKE '%{SurnameInput.Text}%'";

            SqlDataReader reader = new DoSql(sql, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                foreach (Buyer i in VsInsideDBEntities.Content().Buyer.ToList())
                {
                    if (i.buyer_id == reader.GetInt32(0))
                        newList.Add(i);
                }
            }

            DGBuyers.ItemsSource = newList;
        }

        private void AddProduct(object sender, RoutedEventArgs e)
        {
            new AddProductWindow(DGSelectedProducts, selectedList).Show();
        }
    }
}