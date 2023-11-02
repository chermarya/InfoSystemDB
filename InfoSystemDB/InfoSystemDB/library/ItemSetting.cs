﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace InfoSystemDB
{
    public class ItemSetting
    {
        private int id = 0;
        private string title = "";
        private int tip_id = 0;
        private int price = 0;
        private int size_id = 0;
        private int color_id = 0;
        private int quantity = 0;

        private DataGrid DGridProducts;
        private Products element;

        private ListItem[] tips;
        private ListItem[] sizes;
        private ListItem[] colors;
        private ListItem[] materials;

        private ComboBox TipList;
        private ComboBox ColorList;
        private ComboBox SizeList;
        private ComboBox MaterialList;
        private TextBox PriceInput;
        private TextBox TitleInput;        
        private TextBox QuantityInput;

        public ItemSetting(ComboBox tip, ComboBox color, ComboBox size, ComboBox material, TextBox price,
            TextBox title, TextBox quantity, DataGrid grid)
        {
            TipList = tip;
            ColorList = color;
            SizeList = size;
            MaterialList = material;
            PriceInput = price;
            TitleInput = title;
            QuantityInput = quantity;
            DGridProducts = grid;
        }

        public void Save(int mode)
        {
            if (Validate(mode))
            {
                string sqlExpression;
                string connection = "Data Source=WIN-FSJH44K4B7V;Initial Catalog=InfoSystemDB;Integrated Security=true;";
                if (mode == 0)
                    sqlExpression = "INSERT INTO Products (title, tip, price, size, color, quantity) VALUES " +
                                    "@title, @tip_id, @price, @size_id, @color_id, @quantity)";
                else
                    sqlExpression = "UPDATE Products SET title = @title, tip = @tip_id, price = @price, size = " +
                                    "@size_id, color = @color_id, quantity = @quantity WHERE id = @id";

                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(sqlExpression, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@title", title));
                        cmd.Parameters.Add(new SqlParameter("@tip_id", tip_id));
                        cmd.Parameters.Add(new SqlParameter("@price", price));
                        cmd.Parameters.Add(new SqlParameter("@size_id", size_id));
                        cmd.Parameters.Add(new SqlParameter("@color_id", color_id));
                        cmd.Parameters.Add(new SqlParameter("@quantity", quantity));

                        cmd.ExecuteNonQuery();
                    }
                }

                DGridProducts.ItemsSource = InfoSystemDBEntities.Reload().Products.ToList();

                Window window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                window.Close();
            }
        }

        public void Loader(int mode, int selected_id)
        {
            if (mode == 1)
                SelectedItem(selected_id);
            
            List<Tips> tip_list = InfoSystemDBEntities.GetContent().Tips.ToList();
            tips = new ListItem[tip_list.Count];

            if (mode == 0)
            {
                TipList.Items.Add("Обрати");
                TipList.SelectedIndex = 0;
            }

            for (var i = 0; i < tip_list.Count; i++)
            {
                tips[i] = new ListItem(tip_list[i].id, tip_list[i].title);
                TipList.Items.Add(tip_list[i].title);
                if (mode == 1 && tip_list[i].id == element.tip)
                    TipList.SelectedIndex = i;
            }
            
            List<Colors> color_list = InfoSystemDBEntities.GetContent().Colors.ToList();
            colors = new ListItem[color_list.Count];

            if (mode == 0)
            {
                ColorList.Items.Add("Обрати");
                ColorList.SelectedIndex = 0;
            }

            for (var i = 0; i < color_list.Count; i++)
            {
                colors[i] = new ListItem(color_list[i].id, color_list[i].title);
                ColorList.Items.Add(color_list[i].title);
                if (mode == 1 && color_list[i].id == element.color)
                    ColorList.SelectedIndex = i;
            }

            List<Sizes> size_list = InfoSystemDBEntities.GetContent().Sizes.ToList();
            sizes = new ListItem[size_list.Count];

            if (mode == 0)
            {
                SizeList.Items.Add("Обрати");
                SizeList.SelectedIndex = 0;
            }

            for (var i = 0; i < size_list.Count; i++)
            {
                sizes[i] = new ListItem(size_list[i].id, size_list[i].title);
                SizeList.Items.Add(size_list[i].title);
                if (mode == 1 && size_list[i].id == element.size)
                    SizeList.SelectedIndex = i;
            }

            List<Materials> material_list = InfoSystemDBEntities.GetContent().Materials.ToList();
            materials = new ListItem[material_list.Count];
            
            if (mode == 0)
            {
                MaterialList.Items.Add("Обрати");
                MaterialList.SelectedIndex = 0;
            }
            
            for (var i = 0; i < material_list.Count; i++)
            {
                materials[i] = new ListItem(material_list[i].id, material_list[i].title);
                MaterialList.Items.Add(material_list[i].title);
                if (mode == 1)
                    foreach (var j in color_list)
                    {
                        if (j.id == element.color && j.material == material_list[i].id)
                            MaterialList.SelectedIndex = i;
                    }
            }
        }

        private void SelectedItem(int id)
        {
            this.id = id;
            
            element = new Products();
            
            List<Products> list = InfoSystemDBEntities.GetContent().Products.ToList();
            foreach (var el in list)
            {
                if (el.id == id)
                {
                    element = el;
                    break;
                }
            }
            
            TitleInput.Text = element.title;
            PriceInput.Text = Convert.ToString(element.price);
        }

        private bool Validate(int mode)
        {
            if (mode == 0)
                if (TipList.SelectedIndex == 0 || ColorList.SelectedIndex == 0 ||
                SizeList.SelectedIndex == 0 || MaterialList.SelectedIndex == 0)
                return false;
            if (QuantityInput.Text == "" || PriceInput.Text == "" || TitleInput.Text == "")
                return false;
            
            try
            {
                price = Convert.ToInt32(PriceInput.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Невірно вказана ціна");
                return false;
            }
            
            try
            {
                quantity = Convert.ToInt32(QuantityInput.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Невірно вказаний залишок");
                return false;
            }

            int tipInd = TipList.SelectedIndex;
            int sizeInd = SizeList.SelectedIndex;
            int colorInd = ColorList.SelectedIndex;

            if (mode == 0)
            {
                tipInd--;
                sizeInd--;
                colorInd--;
            }

            title = TitleInput.Text;
            tip_id = tips[tipInd].Id;
            size_id = sizes[sizeInd].Id;
            color_id = colors[colorInd].Id;

            return true;
        }
    }
}