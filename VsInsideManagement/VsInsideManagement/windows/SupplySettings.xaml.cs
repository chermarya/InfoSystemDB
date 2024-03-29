﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using VsInsideManagement.library;

namespace VsInsideManagement.windows
{
    public partial class SupplySettings : Window
    {
        private List<Product> SelProdList = new List<Product>();
        public delegate void Function(string where);

        private Function func;
        private int mode;
        public SupplySettings(int m, Function func)
        {
            mode = m;
            this.func = func;
            
            InitializeComponent();
        }
        
        public SupplySettings(int m, Function func, List<Product> sel)
        {
            SelProdList = sel;
            mode = m;
            this.func = func;
            
            InitializeComponent();
        }

        private void AddProduct(object sender, RoutedEventArgs e)
        {
            new AddProductWindow(DGProducts, SelProdList, 1).Show();
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (DGProducts.SelectedIndex == -1)
                return;

            for (int i = 0; i < SelProdList.Count; i++)
            {
                if (((Product)DGProducts.SelectedItem).product_id == SelProdList[i].product_id)
                    SelProdList.RemoveAt(i);
            }

            DGProducts.ItemsSource = SelProdList;

            if (SelProdList.Count == 0)
                DGProducts.ItemsSource = new List<Product>();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                string[] dataArr = DateInput.Text.Split('.');
                string data = $"{dataArr[2]}-{dataArr[1]}-{dataArr[0]}";

                new DoSql("INSERT INTO Supply (dday) VALUES (@day)", new SqlParameter[]
                {
                    new SqlParameter("@day", data)
                }).ToExecuteQuery();

                SqlDataReader reader = new DoSql("SELECT TOP 1 supply_id FROM Supply ORDER BY supply_id DESC",
                    new SqlParameter[] { }).ToReadQuery();

                int sup_id = 0;

                while (reader.Read())
                {
                    sup_id = reader.GetInt32(0);
                }

                foreach (Product i in SelProdList)
                {
                    new DoSql("INSERT INTO Making (supply_id, product_id, quantity) VALUES (@sup, @prod, @quant)",
                        new SqlParameter[]
                        {
                            new SqlParameter("@sup", sup_id),
                            new SqlParameter("@prod", i.product_id),
                            new SqlParameter("@quant", i.quantity)
                        }).ToExecuteQuery();

                    SqlDataReader rd = new DoSql("SELECT quantity FROM Product WHERE product_id = @prod",
                        new SqlParameter[]
                        {
                            new SqlParameter("@prod", i.product_id)
                        }).ToReadQuery();

                    int quan = 0;
                    while (rd.Read())
                    {
                        quan = rd.GetInt32(0);
                    }

                    new DoSql("UPDATE Product SET quantity = @quantity WHERE product_id = @prod", new SqlParameter[]
                    {
                        new SqlParameter("@quantity", quan + i.quantity),
                        new SqlParameter("@prod", i.product_id)
                    }).ToExecuteQuery();
                }

                MessageBox.Show("Партія додана успішно.");

                func("");
                
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
            }
        }

        private bool Validate()
        {
            foreach (var i in SelProdList)
            {
                if (i.quantity <= 0)
                {
                    MessageBox.Show($"{i.ProdType.title} {i.title}: введена невірна кількість.");
                    return false;
                }
            }
            
            if (DateInput.Text == "")
            {
                MessageBox.Show("Не вказана дата.");
                return false;
            }

            if (SelProdList.Count == 0)
            {
                MessageBox.Show("Список товарів пуст.");
                return false;
            }

            return true;
        }
    }
}