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
using Microsoft.Win32;
using System.IO;
using System.IO.Compression;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;

/*
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes; // causes System.IO.Path to return nothing on directory queries
*/

namespace Snarkdown_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //bool isFullscreen = false;
        public bool IsFullscreen { get; set; }
        GridLength columnWidth;
        GridLength rowHeight;

        public MainWindow()
        {
            InitializeComponent();
            //Model.Instance.LoadProject(@"SampleProject"); // debuggery
            columnWidth = collapseCol.Width;
            rowHeight = collapseRow.Height;
            Model.Instance.mw = this;
        }

        private void NewDoc_Click(object sender, RoutedEventArgs e)
        {
            Model.Instance.CurrentDocument = new DocModel();
            Model.Instance.DocModels.Add(Model.Instance.CurrentDocument);
        }
        private void SaveDoc_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Instance.CurrentDocument.pathFile != null
                || Model.Instance.CurrentDocument.pathFile != ""
                || Model.Instance.CurrentDocument.pathFile.Length > 0)
            {
                // ask user for new path for this DocModel
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Markdown Files (.md)|*.md|Text Files (.txt)|*.txt|All Files (*.*)|*.*";
                sfd.FilterIndex = 1;
                sfd.Title = "Save As";
                Nullable<bool> result = sfd.ShowDialog();
                if (result == true)
                {
                    Model.Instance.CurrentDocument.pathFile = sfd.FileName;

                }
            }
            if (Model.Instance.CurrentDocument.pathFile != null 
                && Model.Instance.CurrentDocument.pathFile != ""
                && Model.Instance.CurrentDocument.pathFile.Length > 0)
            {
                Model.Instance.CurrentDocument.Save();
            }
        }

        private void NewProj_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Snarkdown Project|*.*";
            sfd.FilterIndex = 1;
            sfd.Title = "Project Title";
            sfd.FileName = "New Project Title";
            //sfd.ShowNewFolderButton = true;
            Nullable<bool> result = sfd.ShowDialog();
            string returnedPath = "";
            string projectTitle = "";
            string projectPath = "";
            string rootPath = "";
            if (result == true)
            {
                returnedPath = sfd.FileName;
                //projectPath = System.IO.Path.GetFullPath(returnedPath);
                projectPath = System.IO.Path.GetDirectoryName(returnedPath);
                projectTitle = System.IO.Path.GetFileNameWithoutExtension(returnedPath);
                // create the file
                rootPath = projectPath + "\\project.md";
                try
                {
                    using (StreamWriter sw = new StreamWriter(rootPath))
                    {
                        sw.Write(" * Project Name: " + projectTitle);
                    }
                }
                catch (Exception ex)
                {
                    db.w("exception: " + ex);
                }
                // load the blank project in
                if (File.Exists(rootPath))
                {
                    // clear all the documents and fields on Model.
                    Model.Instance.LoadProject(rootPath);
                }
            }

        }

        private void OpenProj_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Markdown Files (.md, .txt)|*.md;*.txt;project.md.meta|Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;
            ofd.Title = "Open File or Project";
            Nullable<bool> result = ofd.ShowDialog();
            string returnedPath = "";
            if (result == true)
            {
                returnedPath = ofd.FileName;
            }
            if (File.Exists(returnedPath))
            {
                // clear all the documents and fields on Model.
                Model.Instance.LoadProject(returnedPath);
            }
        }

        private void ExportProj_Click(object sender, RoutedEventArgs e)
        {
            // https://github.com/danielwertheim/kiwi/wiki/Use-with-Asp.Net-MVC
            // http://stackoverflow.com/questions/8210974/markdownsharp-github-syntax-for-c-sharp-code
            // compile all markdown files together
            string compiledMD = "";
            foreach (DocModel dm in Model.Instance.docModels)
            {
                // check that we should include this document
                compiledMD += dm.textContents;
                compiledMD += "\n";
            }
            // ask for a place to save
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "HTML file (.html|*.html";
            sfd.FilterIndex = 1;
            sfd.Title = "Export Project";
            Nullable<bool> result =sfd.ShowDialog(); 
            if ( result == true )
            {
                Model.Instance.exportPath = sfd.FileName;
            }
            if (Model.Instance.exportPath.Length > 0)
            {
                // set up our service and render to html
                MarkdownService mds = new MarkdownService(new FileContentProvider(Model.Instance.ProjectPath));
                //MarkdownService mds = new MarkdownService();
                //db.w(mds.ToHtml(compiledMD));
                // save html content to file
                
                using (StreamWriter sw = new StreamWriter(Model.Instance.exportPath))
                {
                    sw.Write(mds.ToHtml(compiledMD));
                }
            }
        }

        private void BackupProj_Click(object sender, RoutedEventArgs e)
        {
            string projectPathParent = Directory.GetParent(Model.Instance.projectPath).ToString();
            string backupTitle = Path.GetFileName(Model.Instance.projectPath); // TODO: should be project name
            backupTitle += DateTime.Now.ToString("-yyyy-MMdd-HHmm");
            string backupPath = projectPathParent + @"\" + backupTitle + ".zip";
            if (File.Exists(backupPath))
            {
                backupPath = backupPath.Replace(".zip", "-" + DateTime.Now.Second + ".zip");
            }
            ZipFile.CreateFromDirectory(Model.Instance.projectPath, backupPath, CompressionLevel.Fastest, true);
        }

        private void Fullscreen_Click(object sender, ExecutedRoutedEventArgs e)
        {
            IsFullscreen = !IsFullscreen;
            if(IsFullscreen == true)
            {
                collapseCol.Width = new GridLength(0, GridUnitType.Star);
                collapseRow.Height = new GridLength(0, GridUnitType.Star);
            }
            else
            {
                collapseCol.Width = columnWidth;
                collapseRow.Height = rowHeight;
            }
            if (fullscreenMenuItem.IsChecked != IsFullscreen)
            {
                fullscreenMenuItem.IsChecked = IsFullscreen;
            }
        }

        private void datagrid_CurrentCellChanged(object sender, EventArgs e)
        {
            // throws an error if you click on the last row...
            if (datagrid.CurrentItem != null)
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
    public static class Commands
    {
        // http://stackoverflow.com/questions/5329292/why-doesnt-setting-menuitem-inputgesturetext-cause-the-menuitem-to-activate-whe
        public static readonly RoutedCommand NewProject = new RoutedCommand("New Project", typeof(Commands));
        public static readonly RoutedCommand BackupProject = new RoutedCommand("Backup Project", typeof(Commands));
        public static readonly RoutedCommand ExportProject = new RoutedCommand("Export Project", typeof(Commands));
        public static readonly RoutedCommand FullScreen = new RoutedCommand("Fullscreen", typeof(Commands));

        static Commands()
        {
            NewProject.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Alt));
            BackupProject.InputGestures.Add(new KeyGesture(Key.B, ModifierKeys.Control | ModifierKeys.Alt));
            ExportProject.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control | ModifierKeys.Alt));
            FullScreen.InputGestures.Add(new KeyGesture(Key.F11));
        }
    }
}
