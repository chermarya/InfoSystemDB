using System.Windows;
using VsInsideManagement.pages;
using VsInsideManagement.pages.admin;

namespace VsInsideManagement.windows
{
    public partial class BaseWindow 
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