using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InfoSystemDB.windows;

namespace InfoSystemDB
{
    public partial class ProductsPage : Page
    {

        public ProductsPage()
        {
            InitializeComponent();

            DGridProducts.ItemsSource = InfoSystemDBEntities.GetContent().Products.ToList();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            new ProductSettings(DGridProducts, 0).Show();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            int selected_id = 0;
            List<Products> selected = InfoSystemDBEntities.GetContent().Products.ToList();

            int rowInd = DGridProducts.SelectedIndex;
            if (rowInd != -1)
                selected_id = selected[rowInd].id;

            //new EditProduct(DGridProducts, selected_id).Show();
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            int id = 0;
            List<Products> selected = InfoSystemDBEntities.GetContent().Products.ToList();

            int rowIndex = DGridProducts.SelectedIndex;
            if (rowIndex != -1)
                id = selected[rowIndex].id;

            string sqlExpression = "DELETE FROM Products WHERE id = @id";
            using (SqlConnection connection = new SqlConnection("Data Source=WIN-FSJH44K4B7V;Initial Catalog=lab1DB;Integrated Security=true;"))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlExpression, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
            
            DGridProducts.ItemsSource = InfoSystemDBEntities.Reload().Products.ToList();
        }

        private void ViewTips(object sender, RoutedEventArgs e)
        {
            //MainFrame.Content = new ViewTipsPage();
        }
    }
}