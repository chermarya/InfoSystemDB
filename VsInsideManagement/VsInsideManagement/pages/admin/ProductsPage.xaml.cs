﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VsInsideManagement.library;
using VsInsideManagement.windows;

namespace VsInsideManagement.pages.admin
{
    public partial class ProductsPage 
    {
        private Dictionary<string, string> _orderMode = new Dictionary<string, string>()
        {
            {"назвою (А -> Я)", "title ASC"},
            {"назвою (Я -> А)", "title DESC"},
            {"ціною (зростання)", "price ASC"},
            {"ціною (зменшення)", "price DESC"},
            {"наявністю (зростання)", "quantity ASC"},
            {"наявністю (зменшення)", "quantity DESC"},
        };

        public ProductsPage()
        {
            InitializeComponent();

            OrderByList.SelectionChanged += GridChanging;
            titleInput.TextChanged += TitleChanging;
            QuantitySelect.Click += LowQuantity;
            
            foreach (string i in _orderMode.Keys)
            {
                OrderByList.Items.Add(i);
            }
            
            OrderByList.SelectedIndex = 0;
        }

        private void LowQuantity(object sender, RoutedEventArgs e)
        {
            FillGrid(_orderMode[OrderByList.SelectedValue.ToString()]);
        }

        private void TitleChanging(object sender, TextChangedEventArgs e)
        {
            FillGrid(_orderMode[OrderByList.SelectedValue.ToString()]);
        }

        private void GridChanging(object sender, SelectionChangedEventArgs e)
        {
            FillGrid(_orderMode[OrderByList.SelectedValue.ToString()]);
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
            new ProductSettings(DGridProducts, FillGrid, _orderMode[OrderByList.SelectedValue.ToString()]).Show();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            if (DGridProducts.SelectedIndex == -1)
                return;

            new ProductSettings(DGridProducts, ((Product)DGridProducts.SelectedItem).product_id, FillGrid, _orderMode[OrderByList.SelectedValue.ToString()]).Show();
            
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (DGridProducts.SelectedIndex == -1)
                return;

            new DoSql("DELETE FROM Product WHERE product_id = @id",
                new SqlParameter[]
                {
                    new SqlParameter("@id", ((Product)DGridProducts.SelectedItem).product_id)
                }).ToExecuteQuery();
            
            FillGrid(_orderMode[OrderByList.SelectedValue.ToString()]);
        }
        
        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}