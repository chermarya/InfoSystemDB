using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class ViewTipsPage : Page
    {
        private int selected_id = 0;

        public ViewTipsPage()
        {
            InitializeComponent();

            DGridProductsTips.ItemsSource = VsInsideDBEntities.Content().Product.ToList();

            List<ProdType> type_list = VsInsideDBEntities.Content().ProdType.ToList();
            TypeList.Items.Add("Усі");
            TypeList.SelectedIndex = selected_id;
            
            for (var i = 0; i < type_list.Count; i++)
            {
                TypeList.Items.Add(type_list[i].title);
            }
        }

        private void Select(object sender, RoutedEventArgs e)
        {
            int pos = TypeList.SelectedIndex;

            List<Product> list = VsInsideDBEntities.Content().Product.ToList();
            List<Product> newList = new List<Product>();

            string sql;
            
            if (pos == 0)
                sql = "SELECT * FROM Product";
            else
                sql = "SELECT * FROM Product WHERE prodtype_id = @type";
            
            
            SqlDataReader reader = new DoSql(sql, 
                new SqlParameter[]
            {
                new SqlParameter("@type", pos)
            }).ToReadQuery();

            while (reader.Read())
            {
                foreach (Product el in list)
                {
                    if (el.product_id == reader.GetInt32(0))
                        newList.Add(el);
                }
            }
            
            DGridProductsTips.ItemsSource = newList.ToList();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}