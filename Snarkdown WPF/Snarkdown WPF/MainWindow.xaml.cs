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

            // try to set up a datagrid
            //Model.Instance.currentDocument = new DocModel(@"TestProject");
            /*
            foreach (DocModel fm in Model.currentDocument.children)
            {
                var item = datagrid.Items.Add(fm);
            }*/
            Model.Instance.LoadProject(@"SampleProject");
        }

        private void NewDoc_Click(object sender, RoutedEventArgs e)
        {
            Model.Instance.currentDocument = new DocModel();
        }

        private void OpenDoc_Click(object sender, RoutedEventArgs e)
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
                Model.Instance.docModels = new System.Collections.ObjectModel.ObservableCollection<DocModel>();
                Model.Instance.rootObject = null;
                Model.Instance.LoadProject(returnedPath);
                
                //Model.Instance.rootObject = new DocModel(returnedPath);
            }
        }

        private void SaveDoc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewProj_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenProj_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExportProj_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackupProj_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
