﻿using System;
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

            List<ProdType> tip_list = VsInsideDBEntities.GetContent().ProdType.ToList();
            TipList.SelectedIndex = 0;
            for (var i = 0; i < tip_list.Count; i++)
            {
                TipList.Items.Add(tip_list[i].title);
            }
        }

        private void Select(object sender, RoutedEventArgs e)
        {
            int pos = TipList.SelectedIndex + 1;

            List<Product> list = VsInsideDBEntities.GetContent().Product.ToList();
            List<Product> newList = new List<Product>();

            try
            {
                SqlConnection connection =
                    new SqlConnection(
                        "Data Source=WIN-FSJH44K4B7V;Initial Catalog=InfoSystemDB;Integrated Security=true;");
                SqlCommand cmd = new SqlCommand();
                connection.Open();
                
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM Product WHERE tip = @tip";
                cmd.Parameters.AddWithValue("@tip", pos);

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