using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class BuyerSettings : Window
    {
        private DataGrid DGBuyers;
        private Buyer currentBuyer;

        private Action Function;
        
        public BuyerSettings(DataGrid grid, Buyer buyer, Action func)
        {
            Function = func;
            DGBuyers = grid;
            currentBuyer = buyer;
            
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                string nick = NickInput.Text;
                string sur = SurnameInput.Text;
                string name = NameInput.Text;
                string tel = "0" + TelInput.Text;

                string sql = "INSERT INTO Buyer (nick, nname, surname, tel) VALUES (@nick, @name, @sur, @tel)";
                
                new DoSql(sql, new SqlParameter[]
                    {
                        new SqlParameter("@nick", nick),
                        new SqlParameter("@name", name),
                        new SqlParameter("@sur", sur),
                        new SqlParameter("@tel", tel),
                        new SqlParameter("@id", currentBuyer.buyer_id)
                    }
                ).ToExecuteQuery();

                DGBuyers.ItemsSource = VsInsideDBEntities.Content().Buyer.ToList();
                
                DGBuyers.SelectedIndex = VsInsideDBEntities.Content().Buyer.ToList().Count - 1;
                
                SqlDataReader reader = new DoSql("SELECT TOP 1 buyer_id FROM Buyer ORDER BY buyer_id DESC",
                    new SqlParameter[] { }).ToReadQuery();

                int buyer_id = 0;
                while (reader.Read())
                {
                    buyer_id = reader.GetInt32(0);
                }

                Window wind = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                
                new AddressSetings(buyer_id, Function).Show();
                
                wind.Close();
            }
        }

        private bool Validate()
        {
            if (SurnameInput.Text == "")
            {
                MessageBox.Show("Прізвище не може бути пустим.");
                return false;
            }
            
            if (NameInput.Text == "")
            {
                MessageBox.Show("Ім'я не може бути пустим.");
                return false;
            }
            
            if (TelInput.Text.Length != 9 || !int.TryParse(TelInput.Text, out int a))
            {
                MessageBox.Show("Телефон введено невірно.");
                return false;
            }

            return true;
        }

        private void MaskTel(object sender, TextChangedEventArgs e)
        {
            TelOutput.Text = ApplyMask(TelInput.Text);
        }
        
        private string ApplyMask(string input)
        {
            string mask = "+380(##)-###-##-##";
            int maskIndex = 0;
            string result = "";

            foreach (char c in mask)
            {
                if (c == '#')
                {
                    if (maskIndex < input.Length)
                    {
                        result += input[maskIndex];
                        maskIndex++;
                    }
                    else
                    {
                        result += '_';
                    }
                }
                else
                {
                    result += c;
                }
            }

            return result;
        }
    }
}