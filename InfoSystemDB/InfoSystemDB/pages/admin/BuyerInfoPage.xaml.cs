using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class BuyerInfoPage : Page
    {
        private Buyer currentBuyer;
        private Action Function;

        public BuyerInfoPage(Buyer buyer, Action func)
        {
            currentBuyer = buyer;
            this.Function = func;

            InitializeComponent();

            Loader();
        }

        private void Loader()
        {
            NicknameTB.Text = currentBuyer.nick;
            SurnameTB.Text = currentBuyer.surname;
            NameTB.Text = currentBuyer.nname;
            TelephoneTB.Text = currentBuyer.tel.Substring(1);

            AddressLoader();
            OrderLoader();
        }

        private void OrderLoader()
        {
            string sql = @"
            SELECT  
	            so.order_id, 
	            FORMAT(so.ddate, 'dd.MM.yy' ), 
	            br.nick, 
	            br.surname + ' ' + br.nname, br.tel, 
	            STRING_AGG(pt.title + ' ' + pr.title + ' ' + cl.title + ' ' +  sz.title, ','), 
	            da.city + ' ' + da.dep + ' (' + ISNULL(da.note, '') + ')', 
	            ds.title AS discount, so.summ, 
	            so.prepay, so.amount_due, 
	            so.invoice, 
	            ISNULL(so.stat, ''), 
	            ISNULL(so.note, ''), 
	            sp.title AS shop, 
	            mg.nname + ' ' + mg.surname 
            FROM SetOrder so 
	            JOIN Delivery dl ON so.delivery_id = dl.delivery_id 
	            JOIN Buyer br ON dl.buyer_id = br.buyer_id 
	            JOIN DelAddress da ON dl.address_id = da.address_id 
	            JOIN Discount ds ON so.discount_id = ds.discount_id 
	            JOIN Shop sp ON so.shop_id = sp.shop_id 
	            JOIN Manager mg ON sp.manager_id = mg.manager_id 
	            JOIN Packaging pck ON pck.order_id = so.order_id 
	            JOIN Product pr ON pck.product_id = pr.product_id 
	            JOIN ProdType pt ON pr.prodtype_id = pt.prodtype_id 
	            JOIN Size sz ON pr.size_id = sz.size_id 
	            JOIN Color cl ON pr.color_id = cl.color_id
            WHERE dl.buyer_id = @buyer_id
            GROUP BY 
	            so.order_id, 
	            so.ddate, 
	            br.nick, 
	            br.surname + ' ' + br.nname, br.tel, 
	            da.city + ' ' + da.dep + ' (' + ISNULL(da.note, '') + ')', 
	            ds.title, 
	            so.summ, 
	            so.prepay, 
	            so.amount_due, 
	            so.invoice, 
	            ISNULL(so.stat, ''), 
	            ISNULL(so.note, ''), 
	            sp.title, 
	            mg.nname + ' ' + mg.surname
            ";

            List<Order> content = new List<Order>();

            SqlDataReader reader = new DoSql(sql, new SqlParameter[]
            {
                new SqlParameter("@buyer_id", currentBuyer.buyer_id)
            }).ToReadQuery();

            while (reader.Read())
            {
                string id = reader.GetInt32(0).ToString();

                id = string.Concat(Enumerable.Repeat("0", 5 - id.Length)) + id;

                Order item = new Order(0, id, reader.GetString(1), reader.GetString(2),
                    reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6),
                    reader.GetString(7), reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10),
                    reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14),
                    reader.GetString(15)
                );

                content.Add(item);
            }

            LabelHistory.Content = $"Історія замовлень ({content.Count})";
            DGridOrders.ItemsSource = content;
        }

        private void AddressLoader()
        {
            List<DelAddress> content = new List<DelAddress>();
            string expr = "SELECT da.address_id, city, dep, note FROM DelAddress da JOIN Delivery d ON d.address_id " +
                          "= da.address_id WHERE d.buyer_id = @buyer_id";

            SqlDataReader reader = new DoSql(expr, new SqlParameter[]
            {
                new SqlParameter("@buyer_id", currentBuyer.buyer_id)
            }).ToReadQuery();

            while (reader.Read())
            {
                foreach (DelAddress i in VsInsideDBEntities.Content().DelAddress.ToList())
                {
                    if (i.address_id == reader.GetInt32(0))
                        content.Add(i);
                }
            }
            
            DGridAddresses.ItemsSource = content;
        }

        private void AddAddress(object sender, RoutedEventArgs e)
        {
            new AddressSetings(currentBuyer.buyer_id, AddressLoader).Show();
        }

        private void DeleteAddress(object sender, RoutedEventArgs e)
        {
            if (DGridAddresses.SelectedIndex == -1)
                return;

            int del_id = 0;
            string sql = @"
                SELECT delivery_id FROM Delivery d 
                JOIN Buyer b ON d.buyer_id = b.buyer_id 
                JOIN DelAddress da ON da.address_id = d.address_id 
                WHERE d.buyer_id = @buyer AND d.address_id = @address
            ";

            SqlDataReader reader = new DoSql(sql, new SqlParameter[]
            {
                new SqlParameter("@buyer", currentBuyer.buyer_id),
                new SqlParameter("@address", ((DelAddress)DGridAddresses.SelectedItem).address_id)
            }).ToReadQuery();

            while (reader.Read())
            {
                del_id = reader.GetInt32(0);
            }

            var i = NotFreeAddress();
            var y = NotFreeAddress().Contains(del_id);
            
            if (!NotFreeAddress().Contains(del_id))
            {
                new DoSql("DELETE FROM Delivery WHERE delivery_id = @id",
                    new SqlParameter[]
                    {
                        new SqlParameter("@id", del_id)
                    }
                ).ToExecuteQuery();
            }
            else
                MessageBox.Show("Ви не можете видалити цю адресу. На неї було оформлено замовлення.");

            AddressLoader();
        }

        private List<int> NotFreeAddress()
        {
            string sql = @"
                SELECT DISTINCT del.delivery_id
                FROM SetOrder so
                JOIN Delivery del ON del.delivery_id = so.delivery_id
                JOIN Buyer b ON b.buyer_id = del.buyer_id
                WHERE b.buyer_id = @buyer
            ";

            List<int> addIDs = new List<int>();
            SqlDataReader reader = new DoSql(sql, new SqlParameter[]
            {
                new SqlParameter("@buyer", currentBuyer.buyer_id)
            }).ToReadQuery();

            while (reader.Read())
            {
                addIDs.Add(reader.GetInt32(0));
            }

            return addIDs;
        }
        
        private void Save()
        {
            if (Validate())
            {
                string nick = NicknameTB.Text;
                string sur = SurnameTB.Text;
                string name = NameTB.Text;

                string[] telephone = TelOutput.Text.Substring(5).Split('-');
                string tel = "0" + telephone[0].Substring(0, 2) + telephone[1] + telephone[2] + telephone[3];

                string sql =
                    "UPDATE Buyer SET nick = @nick, nname = @name, surname = @sur, tel = @tel WHERE buyer_id = @id";

                new DoSql(sql, new SqlParameter[]
                    {
                        new SqlParameter("@nick", nick),
                        new SqlParameter("@name", name),
                        new SqlParameter("@sur", sur),
                        new SqlParameter("@tel", tel),
                        new SqlParameter("@id", currentBuyer.buyer_id)
                    }
                ).ToExecuteQuery();
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Save();
            Function();
            NavigationService.GoBack();
        }

        private bool Validate()
        {
            if (SurnameTB.Text == "")
            {
                MessageBox.Show("Прізвище не може бути пустим.");
                return false;
            }

            if (NameTB.Text == "")
            {
                MessageBox.Show("Ім'я не може бути пустим.");
                return false;
            }

            if (TelephoneTB.Text.Length != 9 || !int.TryParse(TelephoneTB.Text, out int a))
            {
                MessageBox.Show("Телефон введено невірно.");
                return false;
            }

            return true;
        }

        private void MaskTel(object sender, TextChangedEventArgs e)
        {
            TelOutput.Text = ApplyMask(TelephoneTB.Text);
        }

        private string ApplyMask(string input)
        {
            string mask = "+380(##)-###-##-##";
            int maskIndex = 0;
            string result = "";

            foreach (char c in mask)
            {
                if (c == '#')
                {
                    if (maskIndex < input.Length)
                    {
                        result += input[maskIndex];
                        maskIndex++;
                    }
                    else
                        result += '_';
                }
                else
                    result += c;
            }

            return result;
        }
    }
}