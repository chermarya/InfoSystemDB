using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class RegularBuyers : Page
    {
        private List<RBuyer> Data;
        private string dateString;
        
        public RegularBuyers(string date)
        {
            if (date != "")
                dateString = " AND " + date;
            else
                dateString = date;
            
            InitializeComponent();
            ReadData();

            DGBuyers.ItemsSource = Data;
        }
        
        private void ReadData()
        {
            Data = new List<RBuyer>();
            
            string sel1 = @"
                SELECT
                    b.surname + ' ' + b.nname AS Buyer,
                    COALESCE(COUNT(DISTINCT so.order_id), 0) AS OrderCount,
					COALESCE(COUNT(pk.product_id), 0) AS ProductCount,
                    COALESCE(TotalOrderSum, 0) AS TotalSum
                FROM Buyer b
                LEFT JOIN (
                    SELECT
	                    d.buyer_id,
                        SUM(so.summ) AS TotalOrderSum
                    FROM SetOrder so
                    LEFT JOIN Delivery d ON d.delivery_id = so.delivery_id
                    WHERE so.stat <> 'відмова'
            ";
            
            string sel2 = @"
                    GROUP BY d.buyer_id
                ) q ON q.buyer_id = b.buyer_id
                LEFT JOIN Delivery d ON d.buyer_id = b.buyer_id
                LEFT JOIN SetOrder so ON so.delivery_id = d.delivery_id 
                AND so.stat <> 'відмова'
              ";
            
            string and = @"  
                LEFT JOIN Packaging pk ON pk.order_id = so.order_id
                GROUP BY b.surname + ' ' + b.nname, TotalOrderSum
                ORDER BY OrderCount DESC, TotalSum DESC, ProductCount DESC;
            ";

            string sql = sel1 + dateString + sel2 + dateString + and;
                
            SqlDataReader reader = new DoSql(sql, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                Data.Add(new RBuyer(
                    reader.GetString(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2),
                    reader.GetInt32(3)
                ));
            }
        }
    }
}