using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class OrdersByManager : Page
    {
        private int manager_id;
        private List<Order> orderList;

        private string sql =
            "SELECT so.order_id, FORMAT(so.ddate, 'dd.MM.yy' ), br.nick," +
            "br.surname + ' ' + br.nname, br.tel, STRING_AGG(pt.title + ' ' + pr.title + ' ' + cl.title + ' ' + " +
            "sz.title, ', '), da.city + ' ' + da.dep + ' (' + ISNULL(da.note, '') + ')', ds.title AS discount, so.summ, " +
            "so.prepay, so.amount_due, so.invoice, ISNULL(so.stat, ''), ISNULL(so.note, ''), sp.title AS shop, mg.nname " +
            "+ ' ' + mg.surname FROM SetOrder so JOIN Delivery dl ON so.delivery_id = dl.delivery_id JOIN Buyer br " +
            "ON dl.buyer_id = br.buyer_id JOIN DelAddress da ON dl.address_id = da.address_id JOIN Discount ds ON " +
            "so.discount_id = ds.discount_id JOIN Shop sp ON so.shop_id = sp.shop_id JOIN Manager mg ON sp.manager_id " +
            "= mg.manager_id JOIN Packaging pck ON pck.order_id = so.order_id JOIN Product pr ON pck.product_id = " +
            "pr.product_id JOIN ProdType pt ON pr.prodtype_id = pt.prodtype_id JOIN Size sz ON pr.size_id = sz.size_id " +
            "JOIN Color cl ON pr.color_id = cl.color_id WHERE mg.manager_id = @id GROUP BY so.order_id, so.ddate, br.nick, " +
            "br.surname + ' ' + br.nname, br.tel, da.city + ' ' + da.dep + ' (' + ISNULL(da.note, '') + ')', ds.title, " +
            "so.summ, so.prepay, so.amount_due, so.invoice, ISNULL(so.stat, ''), ISNULL(so.note, ''), sp.title, mg.nname + ' ' + mg.surname";

        public OrdersByManager(int id)
        {
            manager_id = id;

            InitializeComponent();
            OutputList();

            amount_due.Header = "Сума до\nсплати";
            summ.Header = "Сума\nзамов.";
            prepay.Header = "Перед\nоплата";
        }

        public void OutputList()
        {
            List<SetOrder> orders = VsInsideDBEntities.Content().SetOrder.ToList();
            orderList = new List<Order>();

            SqlDataReader reader = new DoSql(sql, new SqlParameter[]
            {
                new SqlParameter("@id", manager_id)
            }).ToReadQuery();

            while (reader.Read())
            {
                foreach (SetOrder el in orders)
                {
                    if (el.order_id == reader.GetInt32(0))
                    {
                        Order item = new Order(reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
                            reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6),
                            reader.GetString(7), reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10),
                            reader.GetString(11), reader.GetString(12), reader.GetString(13), reader.GetString(14),
                            reader.GetString(15));

                        orderList.Add(item);
                    }
                }
            }

            DGridOrders.ItemsSource = orderList;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            new OrderSettings(manager_id, OutputList).Show();
        }
    }
}