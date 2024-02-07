using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace VsInsideManagement.library
{
    public class ItemSettings
    {
        private int mode;
        private DataGrid DGridProducts;
        private Product curr_element = new Product();

        private int id;
        private int type_id;
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

        private ListItem[] tips = new ListItem[VsInsideDBEntities.Content().ProdType.ToList().Count];
        public ListItem[] colors = new ListItem[VsInsideDBEntities.Content().Color.ToList().Count];
        private ListItem[] sizes = new ListItem[VsInsideDBEntities.Content().Size.ToList().Count];
        public ListItem[] materials = new ListItem[VsInsideDBEntities.Content().Material.ToList().Count];

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

                string sqlQuery;

                if (mode == 0)
                    sqlQuery =
                        "INSERT INTO Product (prodtype_id, title, size_id, color_id, price, quantity) VALUES (@prodtype, " +
                        "@title, @size, @color, @price, @quantity)";
                else
                    sqlQuery =
                        "UPDATE Product SET prodtype_id = @prodtype, title = @title, size_id = @size, color_id = " +
                        "@color, price = @price, quantity = @quantity WHERE product_id = @id";

                new DoSql(sqlQuery,
                    new SqlParameter[]
                    {
                        new SqlParameter("@prodtype", type_id),
                        new SqlParameter("@title", title),
                        new SqlParameter("@size", size_id),
                        new SqlParameter("@color", color_id),
                        new SqlParameter("@price", price),
                        new SqlParameter("@quantity", quantity),
                        new SqlParameter("@id", id)
                    }).ToExecuteQuery();

                DGridProducts.ItemsSource = VsInsideDBEntities.Content().Product.ToList();
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
            }
        }

        private bool Validate(int mode)
        {
            if (mode == 0)
            {
                if (TipList.SelectedIndex == 0)
                {
                    MessageBox.Show("Тип виробу не обрано.");
                    return false;
                }

                if (ColorList.SelectedIndex == 0)
                {
                    MessageBox.Show("Колір не обрано.");
                    return false;
                }

                if (SizeList.SelectedIndex == 0)
                {
                    MessageBox.Show("Розмір не обрано.");
                    return false;
                }
            }

            if (TitleInput.Text == "")
            {
                MessageBox.Show("Назва не може бути пустою.");
                return false;
            }

            if (PriceInput.Text == "" || !int.TryParse(PriceInput.Text, out price))
            {
                MessageBox.Show("Невірно введена ціна.");
                return false;
            }

            if (QuantityInput.Text == "" || !int.TryParse(QuantityInput.Text, out quantity))
            {
                MessageBox.Show("Невірно введений залишок.");
                return false;
            }

            if (ColorList.SelectedIndex <= 0)
            {
                MessageBox.Show("Колір не обрано.");
                return false;
            }

            return true;
        }

        private void SetValues()
        {
            int tipInd = TipList.SelectedIndex;
            int sizeInd = SizeList.SelectedIndex;

            if (mode == 0)
            {
                tipInd--;
                sizeInd--;
            }

            type_id = tips[tipInd].Id;
            size_id = sizes[sizeInd].Id;
            title = TitleInput.Text;
            price = Convert.ToInt32(PriceInput.Text);
            quantity = Convert.ToInt32(QuantityInput.Text);

            for (int i = 0; i < colors.Length; i++)
            {
                if (ColorList.SelectedItem.ToString() == $"{colors[i].Title}\t{colors[i].AddInfo.Title}")
                    color_id = colors[i].Id;
            }
        }

        public void Loader(int mode)
        {
            List<ProdType> tip_list = VsInsideDBEntities.Content().ProdType.ToList();
            List<Color> color_list = VsInsideDBEntities.Content().Color.ToList();
            List<Size> size_list = VsInsideDBEntities.Content().Size.ToList();
            List<Material> material_list = VsInsideDBEntities.Content().Material.ToList();

            if (mode == 1)
                SelectedItem();

            MaterialList.Items.Add("Усі");
            if (mode == 0)
            {
                TipList.Items.Add("Обрати");
                TipList.SelectedIndex = 0;

                SizeList.Items.Add("Обрати");
                SizeList.SelectedIndex = 0;

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
                tips[i] = new ListItem(tip_list[i].prodtype_id, tip_list[i].title);
                TipList.Items.Add(tips[i].Title);
                if (mode == 1 && tips[i].Id == curr_element.prodtype_id)
                    TipList.SelectedIndex = i;
            }

            for (var i = 0; i < material_list.Count; i++)
            {
                materials[i] = new ListItem(material_list[i].material_id, material_list[i].title);
                MaterialList.Items.Add(materials[i].Title);
            }

            for (var i = 0; i < color_list.Count; i++)
            {
                colors[i] = new ListItem(color_list[i].color_id, color_list[i].title,
                    new ListItem(color_list[i].Material.material_id, color_list[i].Material.title));
                ColorList.Items.Add($"{colors[i].Title}\t{colors[i].AddInfo.Title}");
            }

            for (var i = 0; i < color_list.Count; i++)
            {
                if (mode == 1 && colors[i].Id == curr_element.color_id)
                {
                    for (int j = 0; j < materials.Length; j++)
                    {
                        if (colors[i].AddInfo.Id == materials[j].Id)
                        {
                            MaterialList.SelectedIndex = j + 1;
                        }
                    }

                    SetColorSample();

                    for (int j = 0; j < ColorList.Items.Count; j++)
                    {
                        if (ColorList.Items[j].Equals($"{colors[i].Title}\t{colors[i].AddInfo.Title}"))
                        {
                            ColorList.SelectedIndex = j;
                            break;
                        }
                    }
                }
            }

            if (mode == 0)
                SetColorSample();


            for (var i = 0; i < size_list.Count; i++)
            {
                sizes[i] = new ListItem(size_list[i].size_id, size_list[i].title);
                SizeList.Items.Add(size_list[i].title);
                if (mode == 1 && size_list[i].size_id == curr_element.size_id)
                    SizeList.SelectedIndex = i;
            }
        }

        private void SelectedItem()
        {
            List<Product> products_list = VsInsideDBEntities.Content().Product.ToList();
            foreach (Product i in products_list)
            {
                if (id == i.product_id)
                {
                    curr_element = i;
                    break;
                }
            }
        }

        public void SetColorSample()
        {
            int mat = 0;

            if (MaterialList.SelectedIndex != 0)
                mat = materials[MaterialList.SelectedIndex - 1].Id;

            ColorList.Items.Clear();

            ColorList.Items.Add("Обрати");
            ColorList.SelectedIndex = 0;

            string sql;

            if (MaterialList.SelectedIndex == 0)
                sql = "SELECT color_id FROM Color ORDER BY title";
            else
                sql = "SELECT color_id FROM Color WHERE material_id = @mat ORDER BY title";

            SqlDataReader reader = new DoSql(sql,
                new SqlParameter[]
                {
                    new SqlParameter("@mat", mat)
                }).ToReadQuery();

            while (reader.Read())
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    if (reader.GetInt32(0) == colors[i].Id)
                        ColorList.Items.Add($"{colors[i].Title}\t{colors[i].AddInfo.Title}");
                }
            }
        }
    }
}