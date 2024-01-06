using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace InfoSystemDB
{
    public partial class AverageSupply : Page
    {
        private List<SDate> Data;
        private string dateString;
        private int avgProd = 0;
        private int avgQuan = 0;
        
        public AverageSupply(string date)
        {
            if (date != "")
            {
                dateString = " WHERE " + date;
                dateString = dateString.Replace("ddate", "dday");
            }
            else
                dateString = date;
            
            InitializeComponent();
            ReadData();
            CreatePlotQuan();
            CreatePlotProd();
            
            NumLabel.Content = $"Середній обсяг партії: {avgQuan}";
        }
        
        private void ReadData()
        {
            Data = new List<SDate>();
            
            string select = @"
                SELECT 
	                so.dday,
	                COALESCE(COUNT(mk.product_id), 0) AS ProductCount,
	                COALESCE(SUM(quantity), 0) AS TotalQuantity
                FROM 
	                Supply so
                LEFT JOIN Making mk ON mk.supply_id = so.supply_id             
            ";
            
            string group = " GROUP BY so.dday ";

            string sql = select + dateString + group;
                
            SqlDataReader reader = new DoSql(sql, new SqlParameter[] { }).ToReadQuery();

            while (reader.Read())
            {
                Data.Add(new SDate(
                    reader.GetDateTime(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2)
                ));
            }
        }

        private void ProdBtn(object sender, RoutedEventArgs e)
        {
            NumLabel.Content = $"Середній обсяг партії: {avgProd}";
            QuanView.Visibility = Visibility.Collapsed;
            ProdView.Visibility = Visibility.Visible;
        }
        
        private void QuanBtn(object sender, RoutedEventArgs e)
        {
            NumLabel.Content = $"Середній обсяг партії: {avgQuan}";
            QuanView.Visibility = Visibility.Visible;
            ProdView.Visibility = Visibility.Collapsed;
        }
        
        private void CreatePlotQuan()
        {
            var plotModel = new PlotModel { Title = "Обсяг партії (вироби)" };

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Дата"
            };
            plotModel.Axes.Add(categoryAxis);

            var lineSeries = new LineSeries
            {
                MarkerType = MarkerType.Circle
            };

            int sum = 0;
            
            for (int i = 0; i < Data.Count; i++)
            {
                string year = Data[i].Date.Year.ToString();
                string mon = Data[i].Date.Month.ToString();
                string day = Data[i].Date.Day.ToString();
                
                int quan = Data[i].Quantity;
                sum += quan;
                
                lineSeries.Points.Add(new DataPoint(i, quan));
                categoryAxis.Labels.Add($"{day}.{mon}.{year.Substring(1)}");
            }
            
            avgQuan = sum / Data.Count;

            plotModel.Series.Add(lineSeries);

            QuanView.Model = plotModel;
        }
        
        private void CreatePlotProd()
        {
            var plotModel = new PlotModel { Title = "Обсяг партії (товари)" };

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Дата"
            };
            plotModel.Axes.Add(categoryAxis);

            var lineSeries = new LineSeries
            {
                MarkerType = MarkerType.Circle
            };

            int sum = 0;
            
            for (int i = 0; i < Data.Count; i++)
            {
                string year = Data[i].Date.Year.ToString();
                string mon = Data[i].Date.Month.ToString();
                string day = Data[i].Date.Day.ToString();
                
                int prod = Data[i].Product;
                sum += prod;
                
                lineSeries.Points.Add(new DataPoint(i, prod));
                categoryAxis.Labels.Add($"{day}.{mon}.{year.Substring(1)}");
            }

            avgProd = sum / Data.Count;

            plotModel.Series.Add(lineSeries);

            ProdView.Model = plotModel;
        }
    }
}