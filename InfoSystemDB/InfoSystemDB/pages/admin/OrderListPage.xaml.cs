using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class OrderListPage : Page
    {
        private ListItem[] managers = new ListItem[VsInsideDBEntities.Content().Manager.ToList().Count];

        private string select =
            @"
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
	        ";

        private string where = " ";

        private string group =
            @"
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

        private List<string> StatList = new List<string>
        {
            "в обробці",
            "терміново",
            "відгружен",
            "відмова",
            "завершено"
        };

        public OrderListPage()
        {
            InitializeComponent();

            ManagerList.SelectionChanged += FilterByManager;

            DGridOrders.ItemsSource = OutputList(select + group);

            ManagerList.Items.Add("Усі");
            ManagerList.SelectedIndex = 0;

            List<Manager> mngrs = VsInsideDBEntities.Content().Manager.ToList();
            for (int i = 0; i < managers.Length; i++)
            {
                managers[i] = new ListItem(mngrs[i].manager_id, mngrs[i].nname + " " + mngrs[i].surname);
                ManagerList.Items.Add(managers[i].Title);
            }

            StatusList.ItemsSource = StatList;

            where = $" WHERE so.order_id LIKE '%{OrderNumInput.Text}%' ";
        }

        private void OnlyOrder(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }

        private void OrderNumFilter(object sender, TextChangedEventArgs e)
        {
            FillGrid();
        }

        private void SaveInvoiceClick(object sender, RoutedEventArgs e)
        {
            InvoiceCell.CellStyle = null;
            InvoiceCell.IsReadOnly = true;

            SaveInvoiceBtn.Visibility = Visibility.Collapsed;

            EditStatusBtn.Visibility = Visibility.Visible;
            EditInvoiceBtn.Visibility = Visibility.Visible;

            SaveInvoice();
        }

        private void SaveInvoice()
        {
            string sql = "UPDATE SetOrder SET invoice = @invoice WHERE order_id = @id";

            foreach (Order i in DGridOrders.Items)
            {
                new DoSql(sql, new SqlParameter[]
                {
                    new SqlParameter("@invoice", i.Invoice),
                    new SqlParameter("@id", i.ID)
                }).ToExecuteQuery();
            }

            OutputList(select + where + group);
        }

        private void SaveStatusClick(object sender, RoutedEventArgs e)
        {
            SaveStatus();

            StatusList.CellStyle = null;
            StatusList.IsReadOnly = true;

            StatusView.Visibility = Visibility.Visible;
            StatusList.Visibility = Visibility.Collapsed;

            DateStatusCell.Visibility = Visibility.Collapsed;
            EditInvoiceBtn.Visibility = Visibility.Visible;
            EditStatusBtn.Visibility = Visibility.Visible;

            SaveStatusBtn.Visibility = Visibility.Collapsed;
        }

        private void SaveStatus()
        {
            string newStat;
            string sql = "UPDATE SetOrder SET stat = @stat WHERE order_id = @id";

            foreach (Order i in DGridOrders.Items)
            {
                i.mode = 0;

                newStat = i.Status;
                if (i.Status == "відгружен")
                    newStat += " " + i.StatusDate.ToString().Split(' ')[0];

                new DoSql(sql, new SqlParameter[]
                {
                    new SqlParameter("@stat", newStat),
                    new SqlParameter("@id", i.ID)
                }).ToExecuteQuery();

                DGridOrders.ItemsSource = OutputList(select + where + group);
            }
        }

        private void SelDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;

            if (DGridOrders.SelectedIndex != -1)
                ((Order)DGridOrders.SelectedItem).StatusDate = datePicker.SelectedDate;
        }

        private void EditInvoice(object sender, RoutedEventArgs e)
        {
            InvoiceCell.CellStyle = (Style)Resources["EditCell"];
            InvoiceCell.IsReadOnly = false;

            EditStatusBtn.Visibility = Visibility.Collapsed;
            EditInvoiceBtn.Visibility = Visibility.Collapsed;

            SaveInvoiceBtn.Visibility = Visibility.Visible;
        }

        private void EditStatus(object sender, RoutedEventArgs e)
        {
            StatusList.CellStyle = (Style)Resources["EditCell"];
            StatusList.IsReadOnly = false;

            StatusView.Visibility = Visibility.Collapsed;
            StatusList.Visibility = Visibility.Visible;
            DateStatusCell.Visibility = Visibility.Visible;

            EditStatusBtn.Visibility = Visibility.Collapsed;
            EditInvoiceBtn.Visibility = Visibility.Collapsed;

            SaveStatusBtn.Visibility = Visibility.Visible;

            StatusList.ItemsSource = StatList;

            for (int i = 0; i < DGridOrders.Items.Count; i++)
            {
                Order item = (Order)DGridOrders.Items[i];
                item.mode = 1;
            }
        }

        private void FilterByManager(object sender, SelectionChangedEventArgs e)
        {
            FillGrid();
        }

        private void FillGrid()
        {
            List<Order> newList = new List<Order>();

            int OrderNum;
            string num;

            if (int.TryParse(OrderNumInput.Text, out OrderNum) && OrderNum != 0)
                num = OrderNum.ToString();
            else
                num = "";

            where = $" WHERE so.order_id LIKE '%{num}%' ";

            if (ManagerList.SelectedIndex != 0)
                where += $" AND sp.manager_id = {managers[ManagerList.SelectedIndex - 1].Id} ";

            if (OnlyOrderCheck.IsChecked == true)
                where += " AND (stat LIKE 'в обробці' OR stat LIKE 'терміново') ";

            newList = OutputList(select + where + group);

            List<Order> content = new List<Order>();
            foreach (var i in newList)
            {
                content.Add(i);
            }

            DGridOrders.ItemsSource = content;
        }

        private List<Order> OutputList(string sql)
        {
            List<Order> newList = new List<Order>();

            SqlDataReader reader = new DoSql(sql, new SqlParameter[] { }).ToReadQuery();

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

                newList.Add(item);
            }

            return newList;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}