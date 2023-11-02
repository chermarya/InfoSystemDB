using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB.windows
{
    public partial class ProductSettings : Window
    {
        //private ItemSettings item;

        public ProductSettings(DataGrid DGridProducts, int mode)
        {
            InitializeComponent();

            if (mode == 0)
            {
                //item = new ItemSettings(TipList, ColorList, SizeList, MaterialList, PriceInput, TitleInput, QuantityInput, DGridProducts);
                    

                //item.Loader(0, 0);
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            //item.Save(0);
        }
    }
}