using System.Windows;

namespace InfoSystemDB
{
    public partial class BaseWindow : Window
    {
        public BaseWindow()
        {
            InitializeComponent();
            MainFrame.Content = new AdminMenuPage(MainFrame);
        }
    }
}