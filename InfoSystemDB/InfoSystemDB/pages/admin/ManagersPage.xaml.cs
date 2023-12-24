using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class ManagersPage : Page
    {
        private List<Manager> managers_list = VsInsideDBEntities.Content().Manager.ToList();
        private int pos = 0;
        private Frame MainFrame;
        
        public ManagersPage(Frame MainFrame)
        {
            this.MainFrame = MainFrame;
            
            InitializeComponent();

            CountLabel.Content = pos + 1 + "/" + managers_list.Count;

            FillGaps(pos);
        }

        private void FillGaps(int pos)
        {
            if (pos == 0)
                BtnL.Opacity = 0;
            else
                BtnL.Opacity = 1;

            if (pos == managers_list.Count - 1)
                BtnR.Opacity = 0;
            else
                BtnR.Opacity = 1;

            CountLabel.Content = pos + 1 + "/" + managers_list.Count;

            NameOutput.Text = managers_list[pos].nname;
            SurnameOutput.Text = managers_list[pos].surname;
            LoginOutput.Text = managers_list[pos].llogin;

            ShopFilter(managers_list[pos].manager_id);
        }

        private void ShopFilter(int id)
        {
            List<Shop> list = VsInsideDBEntities.Content().Shop.ToList();
            List<Shop> newList = new List<Shop>();

            new DoSql("SELECT * FROM Shop WHERE manager_id = @id", 
                new SqlParameter[]
            {
                new SqlParameter("@id", id)
            }).ToExecuteQuery();
            
            SqlDataReader reader = new DoSql("SELECT * FROM Shop WHERE manager_id = @id",
                new SqlParameter[]
                {
                    new SqlParameter("@id", id)
                }).ToReadQuery();
            
            while (reader.Read())
            {
                foreach (Shop el in list)
                {
                    if (el.shop_id == reader.GetInt32(0))
                        newList.Add(el);
                }
            
                DGridShops.ItemsSource = newList.ToList();
            }
        }

        private void LAction(object sender, RoutedEventArgs e)
        {
            FillGaps(--pos);
        }

        private void RAction(object sender, RoutedEventArgs e)
        {
            FillGaps(++pos);
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new EditManager(pos, Update);
        }

        private void Update()
        {
            managers_list = VsInsideDBEntities.Content().Manager.ToList();
            CountLabel.Content = pos + 1 + "/" + managers_list.Count;
            FillGaps(pos);
        }
        
        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}