using System.Windows;
using System.Windows.Controls;

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