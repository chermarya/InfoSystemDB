using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using InfoSystemDB.windows;

namespace InfoSystemDB
{
    public partial class ProductsPage : Page
    {
        private Frame MainFrame;

        private Dictionary<string, string> orderMode = new Dictionary<string, string>()
        {
            {"назвою (А -> Я)", "title ASC"},
            {"назвою (Я -> А)", "title DESC"},
            {"ціною (зростання)", "price ASC"},
            {"ціною (зменшення)", "price DESC"},
            {"наявністю (зростання)", "quantity ASC"},
            {"наявністю (зменшення)", "quantity DESC"},
        };

        public ProductsPage(Frame MainFrame)
        {
            this.MainFrame = MainFrame;

            InitializeComponent();

            OrderByList.SelectionChanged += GridChanging;
            titleInput.TextChanged += TitleChanging;
            QuantitySelect.Click += LowQuantity;
            
            foreach (string i in orderMode.Keys)
            {
                OrderByList.Items.Add(i);
            }
            
            OrderByList.SelectedIndex = 0;
        }

        private void LowQuantity(object sender, RoutedEventArgs e)
        {
            FillGrid(orderMode[OrderByList.SelectedValue.ToString()]);
        }

        private void TitleChanging(object sender, TextChangedEventArgs e)
        {
            FillGrid(orderMode[OrderByList.SelectedValue.ToString()]);
        }

        private void GridChanging(object sender, SelectionChangedEventArgs e)
        {
            FillGrid(orderMode[OrderByList.SelectedValue.ToString()]);
        }

        private void FillGrid(string order)
        {
            List<Product> content = new List<Product>();

            string select = "SELECT product_id, prodtype_id, title, size_id, color_id, price, quantity FROM Product ";
            string where = $" WHERE title LIKE '{titleInput.Text}%' ";
            order = " ORDER BY " + order;
            
            if (QuantitySelect.IsChecked == true)
                where = $" WHERE title LIKE '{titleInput.Text}%' AND quantity <= 5 ";

            SqlDataReader reader = new DoSql(select + where + order, new SqlParameter[]{ }).ToReadQuery();

            while (reader.Read())
            {
                foreach (Product i in VsInsideDBEntities.Content().Product.ToList())
                {
                    if (i.product_id == reader.GetInt32(0))
                        content.Add(i);
                }
            }

            DGridProducts.ItemsSource = content;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            new ProductSettings(DGridProducts).Show();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            int selected_id = 0;
            List<Product> selected = VsInsideDBEntities.Content().Product.ToList();

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
            List<Product> selected = VsInsideDBEntities.Content().Product.ToList();

            int rowIndex = DGridProducts.SelectedIndex;
            if (rowIndex != -1)
                id = selected[rowIndex].product_id;
            else
                return;

            new DoSql("DELETE FROM Product WHERE product_id = @id",
                new SqlParameter[]
                {
                    new SqlParameter("@id", id)
                }).ToExecuteQuery();

            DGridProducts.ItemsSource = VsInsideDBEntities.Content().Product.ToList();
        }
        
        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}