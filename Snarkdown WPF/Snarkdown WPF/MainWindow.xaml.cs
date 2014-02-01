using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrightIdeasSoftware;
//using ObjectListView;
using System.Windows.Forms;

namespace Snarkdown_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // try to set up a datagrid
            Model.currentDocument = new FileModel(@"F:\Freelance\repos\Snarkdown\SDWF\TestProject");
            /*
            foreach (FileModel fm in Model.currentDocument.children)
            {
                var item = datagrid.Items.Add(fm);
            }*/
        }

        // host our winforms tree view
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // trying to do our winforms crap
            ObjectListView olv = new ObjectListView();
            wfhost.Child = olv;
            this.grid1.Children.Add(wfhost);
            olv.Dock = DockStyle.Fill;
            olv.Columns.Add("File");
            olv.Columns.Add("Content");
            olv.Columns.Add("Meta");
            olv.RebuildColumns();
            olv.Refresh();


        }
    }
}
