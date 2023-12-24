using System;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Series;

namespace InfoSystemDB
{
    public partial class AverageSupply : Page
    {
        public AverageSupply()
        {
            InitializeComponent();
            CreatePlot();
        }
        
        private void CreatePlot()
        {
            var plotModel = new PlotModel { Title = "Пример графика" };

            var lineSeries = new LineSeries
            {
                Title = "График",
                MarkerType = MarkerType.Circle
            };

            // Генерация данных для графика (пример)
            for (double x = 0; x < 10; x += 0.1)
            {
                double y = Math.Sin(x);
                lineSeries.Points.Add(new DataPoint(x, y));
            }

            plotModel.Series.Add(lineSeries);

            plotView.Model = plotModel;
        }
    }
}