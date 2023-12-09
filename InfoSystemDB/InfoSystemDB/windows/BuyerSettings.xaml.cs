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
        private int mode;
        private Buyer currentBuyer;
        private Action Function;
        public BuyerSettings(DataGrid grid, int mode, Buyer buyer, Action func)
        {
            Function = func;
            DGBuyers = grid;
            this.mode = mode;
            currentBuyer = buyer;
            InitializeComponent();

            if (mode == 1)
                LoadValues();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            int row = DGBuyers.SelectedIndex;
            if (Validate())
            {
                string nick = NickInput.Text;
                string sur = SurnameInput.Text;
                string name = NameInput.Text;
                string tel = "0" + TelInput.Text;

                string sql;
                if (mode == 0)
                    sql = "INSERT INTO Buyer (nick, nname, surname, tel) VALUES (@nick, @name, @sur, @tel)";
                else 
                    sql = "UPDATE Buyer SET nick = @nick, nname = @name, surname = @sur, tel = @tel WHERE buyer_id = @id";
                
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
                
                if (mode == 0)
                    DGBuyers.SelectedIndex = VsInsideDBEntities.Content().Buyer.ToList().Count - 1;
                else 
                    DGBuyers.SelectedIndex = row;
                
                SqlDataReader reader = new DoSql("SELECT TOP 1 buyer_id FROM Buyer ORDER BY buyer_id DESC",
                    new SqlParameter[] { }).ToReadQuery();

                int buyer_id = 0;
                while (reader.Read())
                {
                    buyer_id = reader.GetInt32(0);
                }

                Window wind = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                
                if (mode == 0)
                    new AddressSetings(buyer_id, Function).Show();
                
                wind.Close();
            }
        }

        private void LoadValues()
        {
            NickInput.Text = currentBuyer.nick;
            SurnameInput.Text = currentBuyer.surname;
            NameInput.Text = currentBuyer.nname;
            TelInput.Text = currentBuyer.tel.Substring(1);
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