using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace InfoSystemDB
{
    public partial class OrderListPage : Page
    {
        private ListItem[] managers = new ListItem[VsInsideDBEntities.Content().Manager.ToList().Count];

        private string select = @"SELECT  so.order_id, 
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
                                JOIN Color cl ON pr.color_id = cl.color_id ";

        private string where = " ";

        private string group = @" GROUP BY so.order_id, so.ddate, br.nick, 
                                     br.surname + ' ' + br.nname, br.tel, 
                                     da.city + ' ' + da.dep + ' (' + ISNULL(da.note, '') + ')', ds.title, 
                                     so.summ, so.prepay, so.amount_due, so.invoice, ISNULL(so.stat, ''), 
                                     ISNULL(so.note, ''), sp.title, mg.nname + ' ' + mg.surname";

        private List<string> StatList = new List<string>
        {
            "в обробці",
            "відгружен",
            "обмін",
            "повернення",
            "відмова",
            "завершено"
        };

        private int SelIndEdit = -1;

        public OrderListPage()
        {
            InitializeComponent();

            ManagerList.SelectionChanged += FilterByManager;
            //DGridOrders.CellEditEnding += StatusLoader;

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
            StatusList.CellStyle = null;
            StatusList.IsReadOnly = true;

            StatusView.Visibility = Visibility.Visible;
            StatusList.Visibility = Visibility.Collapsed;

            EditInvoiceBtn.Visibility = Visibility.Visible;
            EditStatusBtn.Visibility = Visibility.Visible;

            SaveStatusBtn.Visibility = Visibility.Collapsed;
        }
        
        private void SaveStatus()
        {
            foreach (Order i in DGridOrders.Items)
            {
                i.mode = 0;
            }
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

            EditStatusBtn.Visibility = Visibility.Collapsed;
            EditInvoiceBtn.Visibility = Visibility.Collapsed;

            SaveStatusBtn.Visibility = Visibility.Visible;

            StatusList.ItemsSource = StatList;

            for (int i = 0; i < DGridOrders.Items.Count; i++)
            {
                Order item = (Order)DGridOrders.Items[i];
                item.mode = 1;
                
                if (item.StatusDate == null)
                {
                    DataGridCell targetCell = GetCell(DGridOrders, i, 14);
                    //targetCell.Opacity = 0.1;
                    targetCell.Style = (Style)Resources["StatusDateCell"];
                }
            }
        }

        private DataGridCell GetCell(DataGrid dataGrid, int rowIndex, int columnIndex)
        {
            DataGridRow rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
        
                if (presenter != null)
                {
                    DataGridCell cell =
                        presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    return cell;
                }
            }
        
            return null;
        }
        
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
        
                childItem childOfChild = FindVisualChild<childItem>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
        
            return null;
        }

        private void FilterByManager(object sender, SelectionChangedEventArgs e)
        {
            List<Order> newList = new List<Order>();

            if (ManagerList.SelectedIndex == 0)
                newList = OutputList(select + group);
            else
            {
                where =
                    $" WHERE so.shop_id IN (SELECT shop_id FROM Shop WHERE manager_id = {managers[ManagerList.SelectedIndex - 1].Id}) ";
                newList = OutputList(select + where + group);
            }

            List<Order> content = new List<Order>();
            foreach (var i in newList)
            {
                content.Add(i);
            }

            DGridOrders.ItemsSource = content;
        }

        private List<Order> OutputList(string sql)
        {
            List<SetOrder> orders = VsInsideDBEntities.Content().SetOrder.ToList();
            List<Order> newList = new List<Order>();

            SqlDataReader reader = new DoSql(sql, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                foreach (SetOrder el in orders)
                {
                    if (el.order_id == reader.GetInt32(0))
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
                }
            }

            return newList;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}