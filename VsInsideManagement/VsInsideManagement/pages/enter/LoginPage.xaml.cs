using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VsInsideManagement.windows;

namespace VsInsideManagement.pages.enter
{
    public partial class LoginPage
    {
        private Frame MainFrame;
        private Window EnterWindow;

        private Dictionary<string, string> _managers = new Dictionary<string, string>()
        {
            { "admin", "Sn0e1BRHTkAzrCnMuGU9mw==" }
        };

        private string _login;

        public LoginPage(Window EnterWindow, Frame MainFrame)
        {
            this.EnterWindow = EnterWindow;
            this.MainFrame = MainFrame;

            EnterWindow.Width = 830;
            EnterWindow.Height = 630;
            EnterWindow.Title = "Login";

            if (!IfBaseExist())
            {
                string filePath = "ServerName.txt";
                string content = File.ReadAllText(filePath);

                try
                {
                    string scriptFilePath = "CreateBase.sql";
                    string connectionString = $"Data Source={content};Initial Catalog=master;Integrated Security=true;";

                    string script = File.ReadAllText(scriptFilePath);

                    SqlConnection connection = new SqlConnection(connectionString);

                    connection.Open();

                    string[] queries = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string query in queries)
                    {
                        try
                        {
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }

                    connection.Close();

                    MessageBox.Show("Database created successfully.", "Success", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            
            InitializeComponent();
        }

        private bool IfBaseExist()
        {
            string filePath = "ServerName.txt";
            string content = File.ReadAllText(filePath);

            try
            {
                string sqlExpression = "SELECT name FROM master.dbo.sysdatabases";

                SqlConnection connection =
                    new SqlConnection($"Data Source={content};Initial Catalog=master;Integrated Security=true;");
                SqlCommand cmd = new SqlCommand(sqlExpression, connection);
                connection.Open();

                cmd.Connection = connection;
                cmd.CommandText = sqlExpression;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetString(0) == "VsInsideDB")
                    {
                        connection.Close();
                        return true;
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            if (_managers.Count == 1)
            {
                foreach (Manager i in VsInsideDBEntities.Content().Manager.ToList())
                {
                    _managers.Add(i.llogin, i.pass);
                }
            }

            if (Validate())
            {
                Window window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                new BaseWindow(_login).Show();
                window?.Close();
            }
            else
            {
                MessageBox.Show("Incorrect login or password.");
            }
        }

        private void Server(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new NewServerPage(EnterWindow, 1);
        }

        private bool Validate()
        {
            if (LogInput.Text == "" || PassInput.Password == "")
                return false;

            _login = LogInput.Text;
            string pass = GetHash(PassInput.Password);

            if (_managers.ContainsKey(_login) && _managers[_login] == pass)
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