using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB.windows
{
    public partial class ProductSettings : Window
    {
        private ItemSettings item;
        private int mode;

        public ProductSettings(DataGrid DGridProducts)
        {
            InitializeComponent();

            mode = 0;
            item = new ItemSettings(mode, 0, DGridProducts, TipList, TitleInput, MaterialList, ColorList, SizeList,
                PriceInput, QuantityInput);

            MaterialList.SelectionChanged += MaterialChanged;
        }

        public ProductSettings(DataGrid DGridProducts, int selected_id)
        {
            InitializeComponent();

            mode = 1;
            item = new ItemSettings(mode, selected_id, DGridProducts, TipList, TitleInput, MaterialList, ColorList,
                SizeList, PriceInput, QuantityInput);

            MaterialList.SelectionChanged += MaterialChanged;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            item.Save();
        }

        private void MaterialChanged(object sender, RoutedEventArgs e)
        {
            item.SetColorSample();
        }
    }
}