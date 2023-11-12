using System.Linq;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class BuyerListPage : Page
    {
        private Frame MainFrame;
        
        public BuyerListPage(Frame MainFrame)
        {
            InitializeComponent();

            DGridBuyers.ItemsSource = VsInsideDBEntities.GetContent().Buyer.ToList();
        }
    }
}