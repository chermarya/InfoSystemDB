using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VsInsideManagement.library;

namespace VsInsideManagement.pages.admin
{
    public partial class EditManager : Page
    {
        private int pos;
        private Manager current;
        private Action func;

        public EditManager(int pos, Action func)
        {
            this.pos = pos;
            this.func = func;

            InitializeComponent();
            SetValues();
        }

        private void SetValues()
        {
            List<Manager> managers_list = VsInsideDBEntities.Content().Manager.ToList();
            current = managers_list[pos];

            NameOutput.Text = current.nname;
            SurnameOutput.Text = current.surname;
            LoginOutput.Text = current.llogin;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            string sql;

            if (PassInput.Password == "")
                sql =
                    "UPDATE Manager SET nname = @name, surname = @surname, llogin = @login WHERE manager_id = @id";
            else
                sql = "UPDATE Manager SET nname = @name, surname = @surname, llogin = @login, pass = @pass WHERE manager_id = @id";

            new DoSql(sql,
                new SqlParameter[]
                {
                    new SqlParameter("@name", NameOutput.Text),
                    new SqlParameter("@surname", SurnameOutput.Text),
                    new SqlParameter("@login", LoginOutput.Text),
                    new SqlParameter("@pass", GetHash(PassInput.Password)),
                    new SqlParameter("@id", current.manager_id)
                }
            ).ToExecuteQuery();

            MessageBox.Show("Changes saved successfully");

            func();
            NavigationService.GoBack();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private string GetHash(string input)
        {
            byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}