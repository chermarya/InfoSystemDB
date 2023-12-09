using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace InfoSystemDB
{
    public partial class TableManagmentPage : Page
    {
        private Frame MainFrame;
        private int loadMode = 0;
        private List<Color> newList = new List<Color>();

        public TableManagmentPage(Frame MainFrame)
        {
            InitializeComponent();
        }

        private void Materials(object sender, RoutedEventArgs e)
        {
            loadMode = 1;
            MatList.Visibility = Visibility.Hidden;

            DGridOutput.Columns.Clear();

            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Назва",
                FontSize = 15,
                Binding = new Binding("title")
            });

            DGridOutput.ItemsSource = VsInsideDBEntities.Content().Material.ToList();
        }

        private void Discounts(object sender, RoutedEventArgs e)
        {
            loadMode = 2;
            MatList.Visibility = Visibility.Hidden;

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

        private void Prodtypes(object sender, RoutedEventArgs e)
        {
            loadMode = 3;
            MatList.Visibility = Visibility.Hidden;

            DGridOutput.Columns.Clear();

            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Назва",
                FontSize = 15,
                Binding = new Binding("title")
            });

            DGridOutput.ItemsSource = VsInsideDBEntities.Content().ProdType.ToList();
        }

        private void Colors(object sender, RoutedEventArgs e)
        {
            loadMode = 0;
            List<Material> material_list = VsInsideDBEntities.Content().Material.ToList();
            ListItem[] materials = new ListItem[VsInsideDBEntities.Content().Material.ToList().Count];

            SetColorGrid(VsInsideDBEntities.Content().Color.ToList());

            MatList.Visibility = Visibility.Visible;

            MatList.Items.Add("Усі");
            MatList.SelectedIndex = 0;

            for (var i = 0; i < material_list.Count; i++)
            {
                materials[i] = new ListItem(material_list[i].material_id, material_list[i].title);
                MatList.Items.Add(materials[i].Title);
            }
        }

        private void SetFilter(object sender, RoutedEventArgs e)
        {
            ColorFilter();
        }

        private void ColorFilter()
        {
            int mat = 0;
            string sql;
            newList = new List<Color>();
            List<Color> list = VsInsideDBEntities.Content().Color.ToList();

            if (MatList.SelectedIndex == 0)
                sql = "SELECT * FROM Color ORDER BY title";
            else
            {
                mat = VsInsideDBEntities.Content().Material.ToList()[MatList.SelectedIndex - 1].material_id;
                sql = "SELECT * FROM Color WHERE material_id = @mat ORDER BY title";
            }

            SqlDataReader reader = new DoSql(sql,
                new SqlParameter[]
                {
                    new SqlParameter("@mat", mat)
                }).ToReadQuery();

            while (reader.Read())
            {
                foreach (var i in list)
                {
                    if (i.color_id == reader.GetInt32(0))
                        newList.Add(i);
                }
            }

            SetColorGrid(newList);
        }

        private void SetColorGrid(List<Color> list)
        {
            DGridOutput.Columns.Clear();

            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Матеріал",
                FontSize = 15,
                Binding = new Binding("Material.title")
            });
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Назва",
                FontSize = 15,
                Binding = new Binding("title")
            });
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Код",
                FontSize = 15,
                Binding = new Binding("code"),
                Width = 100
            });

            DGridOutput.ItemsSource = list;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            Dictionary<int, Action> func = new Dictionary<int, Action>()
            {
                { 0, ColorFilter },
                { 1, MatAction },
                { 2, DiscountAction },
                { 3, TypeAction },
            };

            new TableManagmentSettings(loadMode, func[loadMode]).Show();
        }

        private void MatAction()
        {
            DGridOutput.ItemsSource = VsInsideDBEntities.Content().Material.ToList();
        }

        private void DiscountAction()
        {
            DGridOutput.ItemsSource = VsInsideDBEntities.Content().Discount.ToList();
        }

        private void TypeAction()
        {
            DGridOutput.ItemsSource = VsInsideDBEntities.Content().ProdType.ToList();
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (DGridOutput.SelectedIndex == -1)
                return;

            int id = 0;
            Action func = ColorFilter;
            string[] tab = new string[2];

            switch (loadMode)
            {
                case 0:
                    id = ((Color)DGridOutput.SelectedItem).color_id;
                    func = ColorFilter;
                    tab = new[] { "Color", "color_id" };
                    break;
                
                case 1:
                    id = ((Material)DGridOutput.SelectedItem).material_id;
                    func = MatAction;
                    tab = new[] { "Material", "material_id" };
                    break;
                
                case 2:
                    id = ((Discount)DGridOutput.SelectedItem).discount_id;
                    func = DiscountAction;
                    tab = new[] { "Discount", "discount_id" };
                    break;
                
                case 3:
                    id = ((ProdType)DGridOutput.SelectedItem).prodtype_id;
                    func = TypeAction;
                    tab = new[] { "ProdType", "prodtype_id" };
                    break;
            }
            
            Del(id, func, tab);
        }

        private void Del(int id, Action func, string[] tabl)
        {
            new DoSql($"DELETE FROM {tabl[0]} WHERE {tabl[1]} = @id",
                new SqlParameter[]
                {
                    new SqlParameter("@id", id)
                }
            ).ToExecuteQuery();

            func();
        }
    }
}