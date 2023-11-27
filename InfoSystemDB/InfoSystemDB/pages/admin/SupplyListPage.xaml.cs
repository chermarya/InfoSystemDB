using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace InfoSystemDB
{
    public partial class SupplyListPage : Page
    {
        private Frame MainFrame;

        public SupplyListPage(Frame MainFrame)
        {
            InitializeComponent();
            FillGrid();
        }

        private void FillGrid()
        {
            List<SupplyItem> list = new List<SupplyItem>();

            string sqlExpr = "SELECT FORMAT(s.dday, 'dd.MM.yy' ) AS sup_day, pt.title + ' ' + pr.title AS product, " +
                             "m.quantity FROM Supply s JOIN  Making m ON s.supply_id = m.supply_id JOIN Product pr " +
                             "ON m.product_id = pr.product_id JOIN ProdType pt ON pr.prodtype_id = pt.prodtype_id " +
                             "GROUP BY s.dday, pt.title + ' ' + pr.title, m.quantity";
            
            SqlDataReader reader = new DoSql(sqlExpr, new SqlParameter[] { }).ToReadQuery();
            
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
            //MessageBox.Show(Calendar.SelectedDate.Value.ToString());
        }
    }
}