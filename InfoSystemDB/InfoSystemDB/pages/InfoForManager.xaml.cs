using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace InfoSystemDB
{
    public partial class InfoForManager : Page
    {
        public InfoForManager()
        {
            InitializeComponent();
            ProductLoader();
        }

        private void ProductLoader()
        {
            TypeList.Visibility = Visibility.Visible;
            ColorList.Visibility = Visibility.Visible;
            SizeList.Visibility = Visibility.Visible;

            DGridOutput.Columns.Clear();

            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Тип",
                FontSize = 15,
                Binding = new Binding("ProdType.title")
            });
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Назва",
                FontSize = 15,
                Binding = new Binding("title")
            });
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Колір",
                FontSize = 15,
                Binding = new Binding("Color.title")
            });
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Розмір",
                Width = 80,
                FontSize = 15,
                Binding = new Binding("Size.title")
            });
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Матеріал",
                FontSize = 15,
                Binding = new Binding("Color.Material.title")
            });
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Ціна",
                Width = 90,
                FontSize = 15,
                Binding = new Binding("price")
            });
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Залишок",
                Width = 80,
                FontSize = 15,
                Binding = new Binding("quantity")
            });

            DGridOutput.ItemsSource = ProductAvailability();
        }

        private void ProductFill(object sender, RoutedEventArgs e)
        {
            ProductLoader();
        }

        private void DiscountFill(object sender, RoutedEventArgs e)
        {
            TypeList.Visibility = Visibility.Collapsed;
            ColorList.Visibility = Visibility.Collapsed;
            SizeList.Visibility = Visibility.Collapsed;

            DGridOutput.Columns.Clear();
            
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Назва",
                FontSize = 15,
                Binding = new Binding("title"),
            });
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Відсоток",
                FontSize = 15,
                Binding = new Binding("per"),
                Width = 100
            });

            DGridOutput.ItemsSource = VsInsideDBEntities.Content().Discount.ToList();
        }

        private List<Product> ProductAvailability()
        {
            string sql = "SELECT * FROM Product WHERE quantity > 0";

            List<Product> prodFill = new List<Product>();

            SqlDataReader reader = new DoSql(sql, new SqlParameter[]{ }).ToReadQuery();

            while (reader.Read())
            {
                foreach (Product i in VsInsideDBEntities.Content().Product.ToList())
                {
                    if (reader.GetInt32(0) == i.product_id)
                    {
                        prodFill.Add(i);
                    }
                }
            }

            return prodFill;
        }
    }
}