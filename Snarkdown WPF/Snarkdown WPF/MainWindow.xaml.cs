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
using Microsoft.Win32;
using System.IO;
//using BrightIdeasSoftware;
//using ObjectListView;
//using System.Windows.Forms;

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
            //Model.Instance.LoadProject(@"SampleProject"); // debuggery
        }

        private void NewDoc_Click(object sender, RoutedEventArgs e)
        {
            Model.Instance.currentDocument = new DocModel();
        }

        private void OpenDoc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveDoc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewProj_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenProj_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Markdown Files (.md, .txt)|*.md;*.txt|Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;
            Nullable<bool> result = ofd.ShowDialog();
            string returnedPath = "";
            if (result == true)
            {
                returnedPath = ofd.FileName;
            }
            if (File.Exists(returnedPath))
            {
                Model.Instance.DocModels = new System.Collections.ObjectModel.ObservableCollection<DocModel>();
                Model.Instance.RootObject = null;
                Model.Instance.LoadProject(returnedPath);
            }
        }

        private void ExportProj_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackupProj_Click(object sender, RoutedEventArgs e)
        {

        }

        private void datagrid_CurrentCellChanged(object sender, EventArgs e)
        {
            // throws an error if you click on the last row...
            if (datagrid.CurrentItem.ToString() != "{NewItemPlaceholder}")
            {
                DocModel i = (DocModel)datagrid.CurrentItem;
                if (i != null)
                {
                    Model.Instance.CurrentDocument = i;
                }
                //db.w(""+i);
            }
        }
    }
}
