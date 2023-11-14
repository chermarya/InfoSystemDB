using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace InfoSystemDB
{
    public partial class TableManagmentPage : Page
    {
        private Frame MainFrame;

        public TableManagmentPage(Frame MainFrame)
        {
            InitializeComponent();
        }

        private void Colors(object sender, RoutedEventArgs e)
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
            });;

            DGridOutput.ItemsSource = VsInsideDBEntities.GetContent().Color.ToList();
        }
        
        private void Materials(object sender, RoutedEventArgs e)
        {
            DGridOutput.Columns.Clear();
            
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Назва",
                FontSize = 15,
                Binding = new Binding("title")
            });

            DGridOutput.ItemsSource = VsInsideDBEntities.GetContent().Material.ToList();
        }
        
        private void Discounts(object sender, RoutedEventArgs e)
        {
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

            DGridOutput.ItemsSource = VsInsideDBEntities.GetContent().Discount.ToList();
        }
        
        private void Prodtypes(object sender, RoutedEventArgs e)
        {
            DGridOutput.Columns.Clear();
            
            DGridOutput.Columns.Add(new DataGridTextColumn()
            {
                Header = "Назва",
                FontSize = 15,
                Binding = new Binding("title")
            });

            DGridOutput.ItemsSource = VsInsideDBEntities.GetContent().ProdType.ToList();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}