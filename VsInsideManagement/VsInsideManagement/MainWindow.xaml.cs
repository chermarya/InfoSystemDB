using System.IO;
using VsInsideManagement.pages.enter;

namespace VsInsideManagement
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            string filePath = "ServerName.txt";

            string content = File.ReadAllText(filePath);

            if (content.Contains("#"))
            {
                EnterWindow.Width = 330;
                EnterWindow.Height = 260;
                EnterWindow.Title = "Server";
                EnterFrame.Content = new NewServerPage(EnterWindow, 0);
            }
            else
            {
                EnterWindow.Width = 830;
                EnterWindow.Height = 630;
                EnterWindow.Title = "Login";
                EnterFrame.Content = new LoginPage(EnterWindow, EnterFrame);
            }
        }
    }
}
