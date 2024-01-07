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
            string filePath = "ServerName.txt";
            string[] lines = File.ReadAllLines(filePath);

            lines[0] = ServerInput.Text;
            
            File.WriteAllLines(filePath, lines);

            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
        }
    }
}