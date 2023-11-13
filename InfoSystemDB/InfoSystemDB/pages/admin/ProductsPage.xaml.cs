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

            new DoSql("DELETE FROM Product WHERE product_id = @id",
                new SqlParameter[]
                {
                    new SqlParameter("@id", id)
                }).ToExecuteQuery();

            DGridProducts.ItemsSource = VsInsideDBEntities.Reload().Product.ToList();
        }

        private void ViewTips(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ViewTipsPage();
        }
    }
}