using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class SupplyListPage : Page
    {
        private Frame MainFrame;

        public SupplyListPage(Frame MainFrame)
        {
            InitializeComponent();
            FillGrid("");
        }

        private void FillGrid(string where)
        {
            List<SupplyItem> list = new List<SupplyItem>();

            string select = @"
                SELECT FORMAT(s.dday, 'dd.MM.yy' ) AS sup_day,
                       pt.title + ' ' + pr.title + ' ' + cl.title + ' ' + sz.title AS product,
                       m.quantity
                FROM Supply s
                JOIN  Making m ON s.supply_id = m.supply_id
                JOIN Product pr ON m.product_id = pr.product_id
                JOIN ProdType pt ON pr.prodtype_id = pt.prodtype_id
                JOIN Size sz ON pr.size_id = sz.size_id
                JOIN Color cl ON pr.color_id = cl.color_id 
            ";

            if (where != "")
                where = $"WHERE s.dday = '{where}'";

            string group = @"
                GROUP BY s.dday,
                pt.title + ' ' + pr.title + ' ' + cl.title + ' ' + sz.title,
                m.quantity
                ORDER BY s.dday DESC
            ";
            
            SqlDataReader reader = new DoSql(select + where + group, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                list.Add(new SupplyItem(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetInt32(2)
                ));
            }

            DGridSupplies.ItemsSource = list;
        }

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DPSelected.SelectedDate != null)
            {
                string[] d = DPSelected.Text.Split('.');
                string data = d[2] + "/" + d[1] + "/" + d[0];
                FillGrid(data);
            }
        }

        private void All(object sender, RoutedEventArgs e)
        {
            FillGrid("");
            DPSelected.SelectedDate = null;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            new SupplySettings(0, FillGrid).Show();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            if (DPSelected.SelectedDate != null)
            {
                string[] date = DPSelected.SelectedDate.ToString().Split('.');
                string sqlDate = date[2].Split(' ')[0] + "-" + date[1] + "-" + date[0];

                string sql = "";
            }
            else
                MessageBox.Show("Оберіть дату партії для редагування.");
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}