using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class BuyerSettings : Window
    {
        private DataGrid DGBuyers;
        
        public BuyerSettings(DataGrid grid)
        {
            DGBuyers = grid;
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                string nick = NickInput.Text;
                string sur = SurnameInput.Text;
                string name = NameInput.Text;
                string tel = TelInput.Text;

                new DoSql("INSERT INTO Buyer (nick, nname, surname, tel) VALUES (@nick, @name, @sur, @tel)",
                    new SqlParameter[]
                    {
                        new SqlParameter("@nick", nick),
                        new SqlParameter("@name", name),
                        new SqlParameter("@sur", sur),
                        new SqlParameter("@tel", tel)
                    }
                ).ToExecuteQuery();

                DGBuyers.ItemsSource = VsInsideDBEntities.Content().Buyer.ToList();
                
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
            }
        }

        private bool Validate()
        {
            if (NickInput.Text == "" || SurnameInput.Text == "" || NameInput.Text == "" || TelInput.Text == "")
                return false;
            return true;
        }
    }
}