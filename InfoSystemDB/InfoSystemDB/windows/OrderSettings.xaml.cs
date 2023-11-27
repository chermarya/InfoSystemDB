using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
        private ListItem[] addresses;

        private int address_id;
        private int delivery_id;
        private int discount_id;
        private int shop_id;

        private Buyer selectedBuyer;

        public OrderSettings(int id)
        {
            manager_id = id;

            InitializeComponent();

            DGBuyers.ItemsSource = VsInsideDBEntities.Content().Buyer.ToList();

            PrepayInput.TextChanged += PrepayChanged;
            DiscountList.SelectionChanged += DiscountSelected;

            Loader();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                string[] dataArr = DateInput.Text.Split('.');
                string data = $"{dataArr[2]}-{dataArr[1]}-{dataArr[0]}";
                string sum = OrderSumOutput.Text;
                string prepay = PrepayInput.Text;
                string amount_due = PaySumOutput.Text;
                string invoice = "";
                string stat = "в обробці";
                string note = NoteInput.Text;

                ReadValues();

                string expr =
                    "INSERT INTO SetOrder (ddate, delivery_id, summ, discount_id, prepay, amount_due, invoice, " +
                    "stat, note, shop_id) VALUES (@ddate, @delivery_id, @summ, @discount_id, @prepay, " +
                    "@amount_due, @invoice, @stat, @note, @shop_id)";

                new DoSql(expr, new SqlParameter[]
                {
                    new SqlParameter("@ddate", data),
                    new SqlParameter("@delivery_id", delivery_id),
                    new SqlParameter("@summ", sum),
                    new SqlParameter("@discount_id", discount_id),
                    new SqlParameter("@prepay", prepay),
                    new SqlParameter("@amount_due", amount_due),
                    new SqlParameter("@invoice", invoice),
                    new SqlParameter("@stat", stat),
                    new SqlParameter("@note", note),
                    new SqlParameter("@shop_id", shop_id),
                }).ToExecuteQuery();

                SetProducts();

                MessageBox.Show("Замовлення сформовано успішно.");
                
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
            }
        }

        private void ReadValues()
        {
            string selAddressStr = AddressList.SelectedItem.ToString();
            foreach (ListItem i in addresses)
            {
                if (i.Title == selAddressStr)
                    address_id = i.Id;
            }

            string selDiscountStr = DiscountList.SelectedItem.ToString();
            foreach (ListItem i in discounts)
            {
                if (i.Title == selDiscountStr)
                    discount_id = i.Id;
            }

            string selShopStr = ShopList.SelectedItem.ToString();
            foreach (ListItem i in shops)
            {
                if (i.Title == selShopStr)
                    shop_id = i.Id;
            }

            SqlDataReader reader = new DoSql(
                "SELECT delivery_id FROM Delivery WHERE buyer_id = @buyer AND address_id = @address",
                new SqlParameter[]
                {
                    new SqlParameter("@buyer", selectedBuyer.buyer_id),
                    new SqlParameter("@address", address_id)
                }).ToReadQuery();

            while (reader.Read())
            {
                foreach (Delivery i in VsInsideDBEntities.Content().Delivery.ToList())
                {
                    if (i.delivery_id == reader.GetInt32(0))
                        delivery_id = reader.GetInt32(0);
                }
            }
        }

        private void SetProducts()
        {
            SqlDataReader reader = new DoSql("SELECT TOP 1 order_id FROM SetOrder ORDER BY order_id DESC", new SqlParameter[] { }).ToReadQuery();

            int order_id = 0;
            int[] prod_ids = new int[selectedList.Count];

            while (reader.Read())
            {
                order_id = reader.GetInt32(0);
            }

            for (int i = 0; i < prod_ids.Length; i++)
            {
                prod_ids[i] = selectedList[i].product_id;
            }

            foreach (Product i in selectedList)
            {
                new DoSql("INSERT INTO Packaging (order_id, product_id) VALUES (@order, @prod)", new SqlParameter[]
                {
                    new SqlParameter("@order", order_id),
                    new SqlParameter("@prod", i.product_id)
                }).ToExecuteQuery();
            }
        }

        private bool Validate()
        {
            if (DateInput.Text == "" || ShopList.SelectedItem == null || AddressList.SelectedItem == null)
                return false;

            return true;
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
            string selStr = DiscountList.SelectedItem.ToString();

            foreach (ListItem i in discounts)
            {
                if (i.Title == selStr)
                    per = Convert.ToDouble(i.AddInfo.Title);
            }

            order -= order * (per / 100);
            OrderSumOutput.Text = Convert.ToInt32(order).ToString();
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
            if (DGBuyers.SelectedIndex == -1)
                return;

            SurnameLabel.Visibility = Visibility.Collapsed;
            SurnameInput.Visibility = Visibility.Collapsed;
            DGBuyers.Visibility = Visibility.Collapsed;
            TBOutputBuyer.Visibility = Visibility.Visible;

            selectedBuyer = (Buyer)DGBuyers.SelectedItem;

            TBOutputBuyer.Text = "Нікнейм:\t" + selectedBuyer.nick + "\nПрізвище:\t" + selectedBuyer.surname +
                                 "\nІм'я:\t\t" +
                                 selectedBuyer.nname + "\nТелефон:\t" + selectedBuyer.tel;

            SetAddresses();
        }

        private void DeleteBtn(object sender, RoutedEventArgs e)
        {
            if (DGSelectedProducts.SelectedIndex == -1)
                return;

            for (int i = 0; i < selectedList.Count; i++)
            {
                if (((Product)DGSelectedProducts.SelectedItem).product_id == selectedList[i].product_id)
                    selectedList.RemoveAt(i);
            }

            DGSelectedProducts.ItemsSource = selectedList;

            int sum = 0;

            foreach (Product i in selectedList)
            {
                sum += i.price;
            }

            OrderSumOutput.Text = sum.ToString();
            DiscountCounter();
        }

        private void SetAddresses()
        {
            List<DelAddress> addressList = new List<DelAddress>();

            SqlDataReader reader = new DoSql(
                "SELECT da.address_id, city, dep, note FROM DelAddress da  JOIN Delivery d ON d.address_id = " +
                "da.address_id JOIN Buyer ON d.buyer_id = Buyer.buyer_id WHERE d.buyer_id = @buyer_id",
                new SqlParameter[]
                {
                    new SqlParameter("@buyer_id", selectedBuyer.buyer_id)
                }).ToReadQuery();

            while (reader.Read())
            {
                foreach (DelAddress i in VsInsideDBEntities.Content().DelAddress.ToList())
                {
                    if (reader.GetInt32(0) == i.address_id)
                        addressList.Add(i);
                }
            }


            addresses = new ListItem[addressList.Count];

            for (int i = 0; i < addressList.Count; i++)
            {
                addresses[i] = new ListItem(addressList[i].address_id, addressList[i].city + " "
                    + addressList[i].dep + " (" + addressList[i].note + ")");
                AddressList.Items.Add(addresses[i].Title);
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