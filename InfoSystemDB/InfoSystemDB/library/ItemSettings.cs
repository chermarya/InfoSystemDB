using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public class ItemSettings
    {
        private string sqlQuery;
        private int mode;
        private string conStr = "Data Source=WIN-FSJH44K4B7V;Initial Catalog=InfoSystemDB;Integrated Security=true;";
        private DataGrid DGridProducts;
        private Products curr_element = new Products();

        private int id;
        private int tip_id;
        private string title;
        private int size_id;
        private int color_id;
        private int price;
        private int quantity;

        private ComboBox TipList;
        private TextBox TitleInput;
        private ComboBox MaterialList;
        private ComboBox ColorList;
        private ComboBox SizeList;
        private TextBox PriceInput;
        private TextBox QuantityInput;

        private ListItem[] tips = new ListItem[InfoSystemDBEntities.GetContent().Tips.ToList().Count];
        public ListItem[] colors = new ListItem[InfoSystemDBEntities.GetContent().Colors.ToList().Count];
        private ListItem[] sizes = new ListItem[InfoSystemDBEntities.GetContent().Sizes.ToList().Count];
        public ListItem[] materials = new ListItem[InfoSystemDBEntities.GetContent().Materials.ToList().Count];

        public ItemSettings(ComboBox material, ComboBox color)
        {
            MaterialList = material;
            ColorList = color;
        }
        
        public ItemSettings(int m, int sel_id, DataGrid grid, ComboBox tip, TextBox title, ComboBox material,
            ComboBox color, ComboBox size, TextBox price, TextBox quantity)
        {
            mode = m;
            id = sel_id;
            DGridProducts = grid;
            TipList = tip;
            TitleInput = title;
            MaterialList = material;
            ColorList = color;
            SizeList = size;
            PriceInput = price;
            QuantityInput = quantity;

            Loader(mode);
        }

        public void Save()
        {
            if (Validate(mode))
            {
                SetValues();

                if (mode == 0)
                    sqlQuery = "INSERT INTO Products (tip, title, size, color, price, quantity) VALUES (@tip, " +
                               "@title, @size, @color, @price, @quantity)";
                else
                    sqlQuery = "UPDATE Products SET tip = @tip, title = @title, size = @size, color = " +
                               "@color, price = @price, quantity = @quantity WHERE id = @id";

                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.Add(new SqlParameter("@tip", tip_id));
                        cmd.Parameters.Add(new SqlParameter("@title", title));
                        cmd.Parameters.Add(new SqlParameter("@size", size_id));
                        cmd.Parameters.Add(new SqlParameter("@color", color_id));
                        cmd.Parameters.Add(new SqlParameter("@price", price));
                        cmd.Parameters.Add(new SqlParameter("@quantity", quantity));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                DGridProducts.ItemsSource = InfoSystemDBEntities.Reload().Products.ToList();
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
            }
        }

        private bool Validate(int mode)
        {
            if (mode == 0)
                if (TipList.SelectedIndex == 0 || MaterialList.SelectedIndex == 0 ||
                    ColorList.SelectedIndex == 0 || SizeList.SelectedIndex == 0)
                    return false;

            if (TitleInput.Text == "" || PriceInput.Text == "" || QuantityInput.Text == "" ||
                !int.TryParse(PriceInput.Text, out price) || !int.TryParse(QuantityInput.Text, out quantity))
                return false;
            return true;
        }

        private void SetValues()
        {
            int tipInd = TipList.SelectedIndex;
            int sizeInd = SizeList.SelectedIndex;
            int colorInd = ColorList.SelectedIndex;
            
            if (mode == 0)
            {
                tipInd--;
                sizeInd--;
                colorInd--;
            }

            tip_id = tips[tipInd].Id;
            size_id = sizes[sizeInd].Id;
            color_id = colors[colorInd].Id;
            title = TitleInput.Text;
            price = Convert.ToInt32(PriceInput.Text);
            quantity = Convert.ToInt32(QuantityInput.Text);
        }

        public void Loader(int mode)
        {
            List<Tips> tip_list = InfoSystemDBEntities.GetContent().Tips.ToList();
            List<Colors> color_list = InfoSystemDBEntities.GetContent().Colors.ToList();
            List<Sizes> size_list = InfoSystemDBEntities.GetContent().Sizes.ToList();
            List<Materials> material_list = InfoSystemDBEntities.GetContent().Materials.ToList();

            if (mode == 1)
                SelectedItem();

            if (mode == 0)
            {
                TipList.Items.Add("Обрати");
                TipList.SelectedIndex = 0;

                ColorList.Items.Add("Обрати");
                ColorList.SelectedIndex = 0;

                SizeList.Items.Add("Обрати");
                SizeList.SelectedIndex = 0;

                MaterialList.Items.Add("Обрати");
                MaterialList.SelectedIndex = 0;
            }
            else
            {
                TitleInput.Text = curr_element.title;
                PriceInput.Text = curr_element.price.ToString();
                QuantityInput.Text = curr_element.quantity.ToString();
            }

            for (var i = 0; i < tip_list.Count; i++)
            {
                tips[i] = new ListItem(tip_list[i].id, tip_list[i].title);
                TipList.Items.Add(tips[i].Title);
                if (mode == 1 && tips[i].Id == curr_element.tip)
                    TipList.SelectedIndex = i;
            }

            for (var i = 0; i < material_list.Count; i++)
            {
                materials[i] = new ListItem(material_list[i].id, material_list[i].title);
                MaterialList.Items.Add(materials[i].Title);
            }

            for (var i = 0; i < color_list.Count; i++)
            {
                colors[i] = new ListItem(color_list[i].id, color_list[i].title, color_list[i].Materials.title);
                ColorList.Items.Add($"{colors[i].Title}\t\t{colors[i].AddInfo}");

                if (mode == 1 && colors[i].Id == curr_element.color)
                {
                    ColorList.SelectedIndex = i;
                    for (int j = 0; j < materials.Length; j++)
                    {
                        if (colors[i].AddInfo == materials[j].Title)
                            MaterialList.SelectedIndex = j;
                    }
                }
            }

            for (var i = 0; i < size_list.Count; i++)
            {
                sizes[i] = new ListItem(size_list[i].id, size_list[i].title);
                SizeList.Items.Add(size_list[i].title);
                if (mode == 1 && size_list[i].id == curr_element.size)
                    SizeList.SelectedIndex = i;
            }
        }

        private void SelectedItem()
        {
            List<Products> products_list = InfoSystemDBEntities.GetContent().Products.ToList();
            foreach (Products i in products_list)
            {
                if (id == i.id)
                {
                    curr_element = i;
                    break;
                }
            }
        }

        public void SetColorSample()
        {
            int mat;
            
            if (mode == 0 && MaterialList.SelectedIndex != 0)
                mat = materials[MaterialList.SelectedIndex - 1].Id;
            else
                mat = materials[MaterialList.SelectedIndex].Id;
            
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Connection = connection;
                    
                    ColorList.Items.Clear();

                    if (mode == 0)
                    {
                        ColorList.Items.Add("Обрати");
                        ColorList.SelectedIndex = 0;
                        if (MaterialList.SelectedIndex == 0)
                            cmd.CommandText = "SELECT id FROM Colors";
                        else
                            cmd.CommandText = "SELECT id FROM Colors WHERE material = @mat ORDER BY title";
                    }
                    else
                        cmd.CommandText = "SELECT id FROM Colors WHERE material = @mat ORDER BY title";
                    
                    cmd.Parameters.AddWithValue("@mat", mat);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < colors.Length; i++)
                            {
                                if (reader.GetInt32(0) == colors[i].Id)
                                    ColorList.Items.Add($"{colors[i].Title}\t\t{colors[i].AddInfo}");
                            }
                        }
                    }
                }
        
                connection.Close();
            }
        }
    }
}