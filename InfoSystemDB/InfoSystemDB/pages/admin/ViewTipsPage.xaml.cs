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

            DGridProductsTips.ItemsSource = VsInsideDBEntities.GetContent().Product.ToList();

            List<ProdType> type_list = VsInsideDBEntities.GetContent().ProdType.ToList();
            TypeList.Items.Add("Всі");
            TypeList.SelectedIndex = selected_id;
            
            for (var i = 0; i < type_list.Count; i++)
            {
                TypeList.Items.Add(type_list[i].title);
            }
        }

        private void Select(object sender, RoutedEventArgs e)
        {
            int pos = TypeList.SelectedIndex;

            List<Product> list = VsInsideDBEntities.GetContent().Product.ToList();
            List<Product> newList = new List<Product>();

            try
            {
                SqlConnection connection =
                    new SqlConnection(
                        "Data Source=WIN-FSJH44K4B7V;Initial Catalog=VsInsideDB;Integrated Security=true;");
                SqlCommand cmd = new SqlCommand();
                connection.Open();
                
                cmd.Connection = connection;
                
                if (pos == 0)
                    cmd.CommandText = "SELECT * FROM Product";
                else
                {
                    cmd.CommandText = "SELECT * FROM Product WHERE prodtype_id = @type";
                    cmd.Parameters.AddWithValue("@type", pos);
                }

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    foreach (Product el in list)
                    {
                        if (el.product_id == reader.GetInt32(0))
                            newList.Add(el);
                    }

                    DGridProductsTips.ItemsSource = newList.ToList();
                }
                
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}