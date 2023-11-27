using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace InfoSystemDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Dictionary<string, string> managers = new Dictionary<string, string>()
        {
            { "admin", "Sn0e1BRHTkAzrCnMuGU9mw==" }
        };
        private string login;

        public MainWindow()
        {
            InitializeComponent();

            foreach (Manager i in VsInsideDBEntities.Content().Manager.ToList())
            {
                managers.Add(i.llogin, i.pass);
            }
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                Window window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                new BaseWindow(login).Show();
                window.Close();
            }
            else
            {
                MessageBox.Show("Incorrect login or password.");
            }
        }

        private bool Validate()
        {
            if (logInput.Text == "" || PassInput.Password == "")
                return false;
            
            login = logInput.Text;
            string pass = GetHash(PassInput.Password);
            
            if (managers.ContainsKey(login) && managers[login] == pass)
                return true;

            return false;
        }

        private string GetHash(string input)
        {
            byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}