using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VsInsideManagement.library;

namespace VsInsideManagement.pages
{
    public partial class InfoForManager : Page
    {
        private string select = "SELECT * FROM Product WHERE quantity > 0";
        private ListItem[] types = new ListItem[VsInsideDBEntities.Content().ProdType.ToList().Count];
        private ListItem[] sizes = new ListItem[VsInsideDBEntities.Content().Size.ToList().Count];
        public InfoForManager()
        {
            InitializeComponent();
            
            ProductLoader();
            Loader();
            
            TypeList.SelectionChanged += ListSelect;
            SizeList.SelectionChanged += ListSelect;
            TitleInput.TextChanged += TitleChanged;
        }

        private void TitleChanged(object sender, TextChangedEventArgs e)
        {
            SqlCreate();
        }

        private void ListSelect(object sender, SelectionChangedEventArgs e)
        {
            SqlCreate();
        }

        private void SqlCreate()
        {
            select = $"SELECT * FROM Product WHERE quantity > 0 AND title LIKE '%{TitleInput.Text}%' ";

            if (TypeList.SelectedIndex != 0)
                select += $" AND prodtype_id = {types[TypeList.SelectedIndex - 1].Id} ";

            if (SizeList.SelectedIndex != 0)
                select += $" AND size_id = {sizes[SizeList.SelectedIndex - 1].Id} ";

            DGridOutput.ItemsSource = ProductAvailability(select);
        }
        
        private void Loader()
        {
            List<ProdType> typeList = VsInsideDBEntities.Content().ProdType.ToList();
            List<Size> sizeList = VsInsideDBEntities.Content().Size.ToList();

            TypeList.Items.Add("Усі");
            TypeList.SelectedIndex = 0;

            SizeList.Items.Add("Усі");
            SizeList.SelectedIndex = 0;

            for (var i = 0; i < typeList.Count; i++)
            {
                types[i] = new ListItem(typeList[i].prodtype_id, typeList[i].title);
                TypeList.Items.Add(types[i].Title);
            }

            for (var i = 0; i < sizeList.Count; i++)
            {
                sizes[i] = new ListItem(sizeList[i].size_id, sizeList[i].title);
                SizeList.Items.Add(sizes[i].Title);
            }
        }

        private void ProductLoader()
        {
            TypeList.Visibility = Visibility.Visible;
            SizeList.Visibility = Visibility.Visible;
            TitleInput.Visibility = Visibility.Visible;
            TypeProdLabel.Visibility = Visibility.Visible;
            SizeLabel.Visibility = Visibility.Visible;
            NameLabel.Visibility = Visibility.Visible;

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

            DGridOutput.ItemsSource = ProductAvailability(select);
        }

        private void ProductFill(object sender, RoutedEventArgs e)
        {
            ProductLoader();
        }

        private void DiscountFill(object sender, RoutedEventArgs e)
        {
            TypeList.Visibility = Visibility.Collapsed;
            SizeList.Visibility = Visibility.Collapsed;
            TitleInput.Visibility = Visibility.Collapsed;
            TypeProdLabel.Visibility = Visibility.Collapsed;
            SizeLabel.Visibility = Visibility.Collapsed;
            NameLabel.Visibility = Visibility.Collapsed;

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

        private List<Product> ProductAvailability(string sql)
        {
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
        
        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}