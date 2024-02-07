using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VsInsideManagement.library;

namespace VsInsideManagement.windows
{
    public partial class AddProductWindow
    {
        private int _mode;
        private DataGrid _grid;
        private List<Product> _selectedList;

        private string _sql = "SELECT * FROM Product ";
        private string _additionalType = "";
        private string _additionalSize = "";
        private string _additionalTitle;
        private string _additionalQuantity = "";

        private ListItem[] _types = new ListItem[VsInsideDBEntities.Content().ProdType.ToList().Count];
        private ListItem[] _sizes = new ListItem[VsInsideDBEntities.Content().Size.ToList().Count];

        public AddProductWindow(DataGrid gr, List<Product> selList, int m)
        {
            _mode = m;
            _selectedList = selList;
            _grid = gr;

            InitializeComponent();
            Loader();

            _additionalTitle = $"WHERE title LIKE '%{TitleInput.Text}%' ";
            
            if (_mode == 0)
            {
               _additionalQuantity = " AND quantity > 0 ";
                DGProducts.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Залишок",
                    Width = 75,
                    FontSize = 15,
                    Binding = new Binding("quantity"),
                });
            }

            DoQuery();
        }

        private void AddProd(object sender, RoutedEventArgs e)
        {
            Product selected = (Product)DGProducts.SelectedItem;
            _selectedList.Add(selected);
            SetValue();
            MessageBox.Show("Товар був додан.");
        }

        private void SetValue()
        {
            List<Product> newL = new List<Product>();

            foreach (Product i in _selectedList)
            {
                if (_mode == 1)
                    i.quantity = 0;
                newL.Add(i);
            }

            _grid.ItemsSource = newL;
        }

        private void DoQuery()
        {
            List<Product> newList = new List<Product>();

            string expr = _sql + _additionalTitle + _additionalQuantity + _additionalType + _additionalSize + " ORDER BY title";
            SqlDataReader reader = new DoSql(expr, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                foreach (Product i in VsInsideDBEntities.Content().Product.ToList())
                {
                    if (reader.GetInt32(0) == i.product_id)
                        newList.Add(i);
                }
            }

            DGProducts.ItemsSource = newList;
        }

        private void TypeChanged(object sender, RoutedEventArgs e)
        {
            if (TypeList.SelectedIndex != 0)
            {
                _additionalType = $"AND prodtype_id = {_types[TypeList.SelectedIndex - 1].Id} ";
            }
            else
                _additionalType = "";

            DoQuery();
        }

        private void SizeChanged(object sender, RoutedEventArgs e)
        {
            if (SizeList.SelectedIndex != 0)
                _additionalSize = $"AND size_id = {_sizes[SizeList.SelectedIndex - 1].Id} ";
            else
                _additionalSize = "";

            DoQuery();
        }

        private void TitleChanged(object sender, RoutedEventArgs e)
        {
            _additionalTitle = $"WHERE title LIKE '%{TitleInput.Text}%' ";

            DoQuery();
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
                _types[i] = new ListItem(typeList[i].prodtype_id, typeList[i].title);
                TypeList.Items.Add(_types[i].Title);
            }

            for (var i = 0; i < sizeList.Count; i++)
            {
                _sizes[i] = new ListItem(sizeList[i].size_id, sizeList[i].title);
                SizeList.Items.Add(_sizes[i].Title);
            }
        }
    }
}