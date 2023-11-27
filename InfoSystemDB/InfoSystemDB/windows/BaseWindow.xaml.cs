using System.Windows;

namespace InfoSystemDB
{
    public partial class BaseWindow : Window
    {
        public BaseWindow(string login)
        {
            InitializeComponent();
            if (login == "admin")
                MainFrame.Content = new AdminMenuPage(MainFrame);
            else
                MainFrame.Content = new UserMenuPage(MainFrame, login);
        }
    }
}