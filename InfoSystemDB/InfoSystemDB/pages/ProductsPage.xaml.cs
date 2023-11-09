using System;
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
        private Frame MainFrame;

        public ProductsPage(Frame MainFrame)
        {
            this.MainFrame = MainFrame;

            InitializeComponent();

            DGridProducts.ItemsSource = VsInsideDBEntities.GetContent().Product.ToList();

            DGridProducts.SelectionChanged += Selected;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            new ProductSettings(DGridProducts).Show();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            int selected_id = 0;
            List<Product> selected = VsInsideDBEntities.GetContent().Product.ToList();

            int rowInd = DGridProducts.SelectedIndex;
            if (rowInd != -1)
                selected_id = selected[rowInd].product_id;
            else
                return;

            new ProductSettings(DGridProducts, selected_id).Show();
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            int id = 0;
            List<Product> selected = VsInsideDBEntities.GetContent().Product.ToList();

            int rowIndex = DGridProducts.SelectedIndex;
            if (rowIndex != -1)
                id = selected[rowIndex].product_id;
            else
                return;
            
            try
            {
                string sqlExpression = "DELETE FROM Product WHERE id = @id";
                
                SqlConnection connection =
                    new SqlConnection(
                        "Data Source=WIN-FSJH44K4B7V;Initial Catalog=InfoSystemDB;Integrated Security=true;");
                SqlCommand cmd = new SqlCommand(sqlExpression, connection);
                connection.Open();

                cmd.Parameters.Add(new SqlParameter("@id", id));
                
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            DGridProducts.ItemsSource = VsInsideDBEntities.Reload().Product.ToList();
        }

        private void ViewTips(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ViewTipsPage();
        }
        
        private void Selected(object sender, RoutedEventArgs e)
        {
            List<Product> sel = VsInsideDBEntities.GetContent().Product.ToList();
            int rowInd = DGridProducts.SelectedIndex;
            BoxOutput.Text = sel[rowInd].product_id.ToString();
        }
    }
}