using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows.Controls;
using VsInsideManagement.library;
using VsInsideManagement.library.statistics;

namespace VsInsideManagement.pages.admin.statistics
{
    public partial class PopularProductColor : Page
    {
        private ObservableCollection<ProdStat> LeaderboardP = new ObservableCollection<ProdStat>();
        private ObservableCollection<ProdStat> LeaderboardS = new ObservableCollection<ProdStat>();
        private ObservableCollection<ProdStat> LeaderboardC = new ObservableCollection<ProdStat>();
        private List<ProdStat> Data;
        private string dateString;
        
        public PopularProductColor(string date)
        {
            if (date != "")
                dateString = " AND " + date;
            else
                dateString = date;
            
            InitializeComponent();
            
            LoadLiderboard(LeaderboardP, "Prod");
            LoadLiderboard(LeaderboardS, "Size");
            LoadLiderboard(LeaderboardC, "Color");
            LBProduct.ItemsSource = LeaderboardP;
            LBSize.ItemsSource = LeaderboardS;
            LBColor.ItemsSource = LeaderboardC;
        }
        
        private void LoadLiderboard(ObservableCollection<ProdStat> board, string mode)
        {
            DataContext = board;

            ReadData(mode);
            
            foreach (ProdStat i in Data)
            {
                board.Add(i);
            }
        }
        
        private void ReadData(string mode)
        {
            Data = new List<ProdStat>();
            
            string selectProd = @"
                SELECT
                    pt.title + ' ' + pd.title AS Product,
                    COUNT(*) AS OrderCount
                FROM SetOrder so
                JOIN Packaging pk ON so.order_id = pk.order_id
                JOIN Product pd ON pk.product_id = pd.product_id
                JOIN ProdType pt ON pd.prodtype_id = pt.prodtype_id
                WHERE so.stat <> 'відмова'
            ";
            
            string groupProd = @"
                GROUP BY pt.title, pd.title
                ORDER BY OrderCount DESC, Product
            ";
            
            string selectSize = @"
                SELECT
                    s.title,
                    COUNT(*) AS SizeCount
                FROM SetOrder so
                JOIN Packaging pk ON so.order_id = pk.order_id
                JOIN Product pd ON pk.product_id = pd.product_id
                JOIN Size s ON pd.size_id = s.size_id
                WHERE so.stat <> 'відмова'
            ";
            
            string groupSize = @"
                GROUP BY s.title
                ORDER BY SizeCount DESC
            ";
            
            string selectColor = @"
                SELECT
                    c.title,
                    COUNT(*) AS ColorCount
                FROM SetOrder so
                JOIN Packaging pa ON so.order_id = pa.order_id
                JOIN Product pd ON pa.product_id = pd.product_id
                JOIN Color c ON pd.color_id = c.color_id
                WHERE so.stat <> 'відмова'
            ";
            
            string groupColor = @"
                GROUP BY c.title
                ORDER BY ColorCount DESC
            ";

            string sql = "";
            switch (mode)
            {
                case "Prod":
                    sql = selectProd + dateString + groupProd;
                    break;
                
                case "Size":
                    sql = selectSize + dateString + groupSize;
                    break;
                
                case "Color":
                    sql = selectColor + dateString + groupColor;
                    break;
            }
                
                
            SqlDataReader reader = new DoSql(sql, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                Data.Add(new ProdStat(
                    reader.GetString(0),
                    reader.GetInt32(1)
                ));
            }
        }
    }
}