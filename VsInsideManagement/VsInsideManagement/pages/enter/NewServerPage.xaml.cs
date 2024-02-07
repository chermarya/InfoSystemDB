using System.IO;
using System.Linq;
using System.Windows;

namespace VsInsideManagement.pages.enter
{
    public partial class NewServerPage
    {
        private int _mode;
        string[] serv;

        public NewServerPage(Window EnterWindow, int m)
        {
            _mode = m;

            EnterWindow.Width = 330;
            EnterWindow.Height = 260;
            EnterWindow.Title = "Server";

            InitializeComponent();

            string[] serv = File.ReadAllLines("ServerName.txt");

            if (_mode == 1)
                ServerInput.Text = serv[0];
        }

        private void SaveBtn(object sender, RoutedEventArgs e)
        {
            string serverPath = "ServerName.txt";
            string[] serv = File.ReadAllLines(serverPath);

            string configPath = "../../App.config";
            string[] conf = File.ReadAllLines(configPath);

            conf[10] = conf[10].Replace(serv[0], ServerInput.Text);

            serv[0] = ServerInput.Text;
            File.WriteAllLines(serverPath, serv);

            File.WriteAllLines(configPath, conf);

            MessageBox.Show("Назву сервера збережено успішно.\nПерезапустіть програму для подальшого користування.");

            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
        }
    }
}