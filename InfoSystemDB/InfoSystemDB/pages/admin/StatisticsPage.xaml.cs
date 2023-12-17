using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class StatisticsPage : Page
    {
        private Frame MainFrame;
        public StatisticsPage(Frame MainFrame)
        {
            this.MainFrame = MainFrame;
            
            InitializeComponent();
        }
    }
}