using System.IO;
using System.Linq;
using System.Windows;

namespace InfoSystemDB
{
    public partial class NewServerWindow : Window
    {
        public NewServerWindow()
        {
            InitializeComponent();
        }

        private void SaveBtn(object sender, RoutedEventArgs e)
        {
            string serverPath = "ServerName.txt";
            string[] serv = File.ReadAllLines(serverPath);

            serv[0] = ServerInput.Text;
            
            File.WriteAllLines(serverPath, serv);
            
            string configPath = "../../App.config";
            string[] conf = File.ReadAllLines(configPath);

            conf[10] = conf[10].Replace("WIN-FSJH44K4B7V", ServerInput.Text);
            conf[11] = conf[11].Replace("WIN-FSJH44K4B7V", ServerInput.Text);

            File.WriteAllLines(configPath, conf);
            
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
        }
    }
}