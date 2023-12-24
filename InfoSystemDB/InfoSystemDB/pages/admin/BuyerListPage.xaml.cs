using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class BuyerListPage : Page
    {
        private Frame MainFrame;
        public BuyerListPage(Frame MainFrame)
        {
            this.MainFrame = MainFrame;
            
            InitializeComponent();
            
            BuyersFilter();
            DGridBuyers.SelectedIndex = 0;
            
            SurnameInput.TextChanged += BuyerLoad;
            DeleteBtn.Click += Delete;
            AddBtn.Click += Add;
        }

        private void BuyerLoad(object sender, TextChangedEventArgs e)
        {
            BuyersFilter();
        }
        
        private void BuyersFilter()
        {
            List<Buyer> content = new List<Buyer>();
            string sql = $"SELECT * FROM Buyer WHERE surname LIKE '%{SurnameInput.Text}%' ORDER BY surname";

            SqlDataReader reader = new DoSql(sql, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                foreach (Buyer i in VsInsideDBEntities.Content().Buyer.ToList())
                {
                    if (i.buyer_id == reader.GetInt32(0))
                        content.Add(i);
                }
            }

            DGridBuyers.ItemsSource = content;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            new BuyerSettings(DGridBuyers, new Buyer(), BuyersFilter).Show();
        }

        private void DetailInfo(object sender, RoutedEventArgs e)
        {
            if (DGridBuyers.SelectedIndex == -1)
                return;

            MainFrame.Content = new BuyerInfoPage((Buyer)DGridBuyers.SelectedItem, BuyersFilter);
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (DGridBuyers.SelectedIndex == -1)
                return;

            int id = ((Buyer)DGridBuyers.SelectedItem).buyer_id;

            new DoSql("DELETE FROM Buyer WHERE buyer_id = @id",
                new SqlParameter[]
                {
                    new SqlParameter("@id", id)
                }
            ).ToExecuteQuery();

            BuyersFilter();
        }
        
        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}