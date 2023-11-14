using System.Data.SqlClient;
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

            SqlDataReader reader = new DoSql("SELECT title FROM Material", new SqlParameter[]{}).ToReadQuery();

            while (reader.Read())
            {
                MaterialList.Items.Add(reader.GetString(0));
            }
        }
    }
}