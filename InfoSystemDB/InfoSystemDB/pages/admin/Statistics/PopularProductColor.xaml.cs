﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class PopularProductColor : Page
    {
        private List<MProductivity> Data;
        private string dateString;
        
        public PopularProductColor(string date)
        {
            if (date != "")
                dateString = " AND " + date;
            else
                dateString = date;
            
            InitializeComponent();
        }
        
        private void ReadData()
        {
            Data = new List<MProductivity>();
            
            string sel1 = @"
                SELECT
                    mg.surname + ' ' + mg.nname AS Manager,
                    COALESCE(COUNT(DISTINCT so.order_id), 0) AS OrderCount,
                    COALESCE(TotalOrderSum, 0) AS TotalSum,
                    COALESCE(COUNT(pk.product_id), 0) AS ProductCount
                FROM Manager mg
                LEFT JOIN (
                    SELECT
	                sp.manager_id,
                    SUM(so.summ) AS TotalOrderSum
                    FROM SetOrder so
                    LEFT JOIN Shop sp ON so.shop_id = sp.shop_id
                    WHERE so.stat <> 'відмова'
            ";
            
            string sel2 = @"
                    GROUP BY sp.manager_id
                ) q ON q.manager_id = mg.manager_id
                LEFT JOIN Shop sp ON sp.manager_id = mg.manager_id
                LEFT JOIN SetOrder so ON so.shop_id = sp.shop_id 
                AND so.stat <> 'відмова'
              ";
            
            string and = @"  
                LEFT JOIN Packaging pk ON pk.order_id = so.order_id
                GROUP BY mg.surname + ' ' + mg.nname, TotalOrderSum
                ORDER BY OrderCount DESC, TotalSum DESC;
            ";

            string sql = sel1 + dateString + sel2 + dateString + and;
                
            SqlDataReader reader = new DoSql(sql, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                Data.Add(new MProductivity(
                    reader.GetString(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2),
                    reader.GetInt32(3)
                ));
            }
        }
    }
}