using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class AddProductWindow : Window
    {
        private int mode;
        private DataGrid grid;
        private List<Product> selectedList;

        private string sql = "SELECT * FROM Product ";
        private string additionalType = "";
        private string additionalSize = "";
        private string additionalTitle = "";

        private ListItem[] types = new ListItem[VsInsideDBEntities.Content().ProdType.ToList().Count];
        private ListItem[] sizes = new ListItem[VsInsideDBEntities.Content().Size.ToList().Count];

        public AddProductWindow(DataGrid gr, List<Product> selList, int m)
        {
            mode = m;
            selectedList = selList;
            grid = gr;
            InitializeComponent();
            Loader();
            additionalTitle = $"WHERE title LIKE '%{TitleInput.Text}%' ";
        }

        private void AddProd(object sender, RoutedEventArgs e)
        {
            Product selected = (Product)DGProducts.SelectedItem;
            selectedList.Add(selected);
            SetValue();
            MessageBox.Show("Товар був додан.");
        }

        private void SetValue()
        {
            List<Product> newL = new List<Product>();

            foreach (Product i in selectedList)
            {
                if (mode == 1)
                    i.quantity = 0;
                newL.Add(i);
            }

            grid.ItemsSource = newL;
        }

        private void DoQuery()
        {
            List<Product> newList = new List<Product>();

            string expr = sql + additionalTitle + additionalType + additionalSize + " ORDER BY title";
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
                additionalType = $"AND prodtype_id = {types[TypeList.SelectedIndex - 1].Id} ";
            else
                additionalType = "";

            DoQuery();
        }

        private void SizeChanged(object sender, RoutedEventArgs e)
        {
            if (SizeList.SelectedIndex != 0)
                additionalSize = $"AND size_id = {sizes[SizeList.SelectedIndex - 1].Id} ";
            else
                additionalSize = "";

            DoQuery();
        }

        private void TitleChanged(object sender, RoutedEventArgs e)
        {
            additionalTitle = $"WHERE title LIKE '%{TitleInput.Text}%' ";

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
                types[i] = new ListItem(typeList[i].prodtype_id, typeList[i].title);
                TypeList.Items.Add(types[i].Title);
            }

            for (var i = 0; i < sizeList.Count; i++)
            {
                sizes[i] = new ListItem(sizeList[i].size_id, sizeList[i].title);
                SizeList.Items.Add(sizes[i].Title);
            }
        }
    }
}