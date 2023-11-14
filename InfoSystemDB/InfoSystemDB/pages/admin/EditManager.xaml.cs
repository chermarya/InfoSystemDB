using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public partial class EditManager : Page
    {
        private Frame MainFrame;
        private int pos;
        public EditManager(Frame MainFrame, int pos)
        {
            this.MainFrame = MainFrame;
            this.pos = pos;
            
            InitializeComponent();
            SetValues();
        }

        private void SetValues()
        {
            List<Manager> managers_list = VsInsideDBEntities.GetContent().Manager.ToList();
            NameOutput.Text = managers_list[pos].nname;
            SurnameOutput.Text = managers_list[pos].surname;
            LoginOutput.Text = managers_list[pos].llogin;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}