using System.Linq;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class ColorsPage : Page
    {
        public ColorsPage()
        {
            InitializeComponent();
            
            DGridColours.ItemsSource = VsInsideDBEntities.GetContent().Color.ToList();
            DGridMaterials.ItemsSource = VsInsideDBEntities.GetContent().Material.ToList();
        }
    }
}