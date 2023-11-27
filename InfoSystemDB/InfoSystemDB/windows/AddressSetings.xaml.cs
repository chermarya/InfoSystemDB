using System.Windows;

namespace InfoSystemDB
{
    public partial class AddressSetings : Window
    {
        public AddressSetings()
        {
            InitializeComponent();
        }
        
        private void CDSchecked(object sender, RoutedEventArgs e)
        {
            CBNew.IsChecked = false;
            ChooseGrid.Visibility = Visibility.Visible;
            NewGrid.Visibility = Visibility.Collapsed;
        }

        private void CDNchecked(object sender, RoutedEventArgs e)
        {
            CBSelect.IsChecked = false;
            ChooseGrid.Visibility = Visibility.Collapsed;
            NewGrid.Visibility = Visibility.Visible;
        }
    }
}