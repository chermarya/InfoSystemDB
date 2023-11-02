using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class AdminMenuPage : Page
    {
        private Frame MainFrame;
        
        public AdminMenuPage(Frame MainFrame)
        {
            this.MainFrame = MainFrame;
            
            InitializeComponent();
        }
    }
}