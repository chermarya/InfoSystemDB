using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace InfoSystemDB
{
    public partial class TableManagmentSettings : Window
    {
        private Dictionary<string, int> materials = new Dictionary<string, int>();
        private Action function;
        private int mode;

        public TableManagmentSettings(int md, Action func)
        {
            mode = md;
            function = func;

            InitializeComponent();

            Dictionary<int, Action> modes = new Dictionary<int, Action>()
            {
                { 0, Colors },
                { 1, Materials },
                { 2, Discounts },
                { 3, Prodtypes }
            };

            foreach (Material i in VsInsideDBEntities.Content().Material.ToList())
            {
                materials.Add(i.title, i.material_id);
                MaterialList.Items.Add(i.title);
            }

            modes[mode]();
        }

        private void Colors()
        {
            MainWindow.Height = 450;
            RowButton.Height = new GridLength(100);

            MaterialLabel.Visibility = Visibility.Visible;
            MaterialInput.Visibility = Visibility.Visible;

            CodeLabel.Content = "Код";
            CodeLabel.Visibility = Visibility.Visible;
            CodeInput.Visibility = Visibility.Visible;
            CodeLabel.VerticalAlignment = VerticalAlignment.Center;
            CodeInput.VerticalAlignment = VerticalAlignment.Center;
            CodeLabel.Margin = new Thickness(80, 30, 0, 30);
            CodeInput.Margin = new Thickness(30, 30, 30, 30);

            NameLabel.VerticalAlignment = VerticalAlignment.Top;
            NameInput.VerticalAlignment = VerticalAlignment.Top;
            NameLabel.Margin = new Thickness(80, 55, 0, 0);
            NameInput.Margin = new Thickness(30, 50, 30, 50);
        }

        private void ColorSave(object sender, RoutedEventArgs e)
        {
            new DoSql(
                "INSERT INTO Color (title, code, material_id) VALUES (@title, @code, @material)",
                new SqlParameter[]
                {
                    new SqlParameter("@title", NameInput.Text),
                    new SqlParameter("@code", CodeInput.Text),
                    new SqlParameter("@material", materials[MaterialList.Text])
                }).ToExecuteQuery();

            function();

            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
        }

        private void Materials()
        {
            MainWindow.Height = 250;
            RowButton.Height = new GridLength(80);

            MaterialLabel.Visibility = Visibility.Collapsed;
            MaterialInput.Visibility = Visibility.Collapsed;

            CodeLabel.Visibility = Visibility.Collapsed;
            CodeInput.Visibility = Visibility.Collapsed;

            NameLabel.VerticalAlignment = VerticalAlignment.Center;
            NameInput.VerticalAlignment = VerticalAlignment.Center;
            NameLabel.Margin = new Thickness(60, 0, 0, 0);
            NameInput.Margin = new Thickness(0, 20, 30, 20);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                if (MaterialList.Text == "")
                    MaterialList.Text = "Софт";

                DoSql color = new DoSql(
                    "INSERT INTO Color (title, code, material_id) VALUES (@title, @code, @material)",
                    new SqlParameter[]
                    {
                        new SqlParameter("@title", NameInput.Text),
                        new SqlParameter("@code", CodeInput.Text),
                        new SqlParameter("@material", materials[MaterialList.Text])
                    }
                );

                DoSql material = new DoSql("INSERT INTO Material (title) VALUES (@title)",
                    new SqlParameter[]
                    {
                        new SqlParameter("@title", NameInput.Text)
                    }
                );

                DoSql discount = new DoSql("INSERT INTO Discount (title, per) VALUES (@title, @per)",
                    new SqlParameter[]
                    {
                        new SqlParameter("@title", NameInput.Text),
                        new SqlParameter("@per", CodeInput.Text)
                    }
                );

                DoSql type = new DoSql("INSERT INTO ProdType (title) VALUES (@title)",
                    new SqlParameter[]
                    {
                        new SqlParameter("@title", NameInput.Text)
                    }
                );

                Dictionary<int, DoSql> expr = new Dictionary<int, DoSql>()
                {
                    { 0, color },
                    { 1, material },
                    { 2, discount },
                    { 3, type }
                };

                expr[mode].ToExecuteQuery();

                function();

                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive).Close();
            }
        }

        private bool Validate()
        {
            if (NameInput.Text == "")
            {
                MessageBox.Show("Назва не може бути пустою.");
                return false;
            }

            switch (mode)
            {
                case 0:
                    if (MaterialList.SelectedItem == null)
                    {
                        MessageBox.Show("Матеріал не може бути пустим.");
                        return false;
                    }

                    break;

                case 2:
                    if (!int.TryParse(CodeInput.Text, out int a) || CodeInput.Text == "")
                    {
                        MessageBox.Show("Відсоток введений невірно.");
                        return false;
                    }

                    break;
            }

            return true;
        }

        private void Discounts()
        {
            MainWindow.Height = 300;
            RowButton.Height = new GridLength(80);

            MaterialLabel.Visibility = Visibility.Collapsed;
            MaterialInput.Visibility = Visibility.Collapsed;

            CodeLabel.Content = "Відсоток";
            CodeLabel.Visibility = Visibility.Visible;
            CodeInput.Visibility = Visibility.Visible;
            CodeLabel.VerticalAlignment = VerticalAlignment.Bottom;
            CodeInput.VerticalAlignment = VerticalAlignment.Bottom;
            CodeLabel.Margin = new Thickness(60, 0, 0, 30);
            CodeInput.Margin = new Thickness(0, 20, 30, 30);

            NameLabel.VerticalAlignment = VerticalAlignment.Top;
            NameInput.VerticalAlignment = VerticalAlignment.Top;
            NameLabel.Margin = new Thickness(60, 30, 0, 0);
            NameInput.Margin = new Thickness(0, 30, 30, 20);
        }

        private void Prodtypes()
        {
            MainWindow.Height = 250;
            RowButton.Height = new GridLength(80);

            MaterialLabel.Visibility = Visibility.Collapsed;
            MaterialInput.Visibility = Visibility.Collapsed;

            CodeLabel.Visibility = Visibility.Collapsed;
            CodeInput.Visibility = Visibility.Collapsed;

            NameLabel.VerticalAlignment = VerticalAlignment.Center;
            NameInput.VerticalAlignment = VerticalAlignment.Center;
            NameLabel.Margin = new Thickness(60, 0, 0, 0);
            NameInput.Margin = new Thickness(0, 20, 30, 20);
        }
    }
}