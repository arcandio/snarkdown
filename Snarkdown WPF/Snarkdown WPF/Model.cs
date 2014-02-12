using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using GfmSyntax;
using System.Windows.Controls;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;
using System.IO.Compression;

namespace Snarkdown_WPF
{

   /* public class FileList : ObservableCollection<DocModel>
    {
        public FileList() : base()
        {
            // debug constructor automatically loads the sample project. TODO: Do not ship!
            Model.Instance.currentDocument = new DocModel(@"SampleProject");
            foreach (DocModel fm in Model.Instance.docModels)
            {
                Add(fm);
            }
        }
    }*/
    /// <summary>
    /// the model of all data in the program
    /// </summary>
    public sealed class Model : INotifyPropertyChanged
    {
        #region Fields
        public MainWindow mw;
        public RichTextBox rtb;
        private bool isProjectBlank;
        public int wcDay = 0;
        public int wcDoc = 0;
        public int wcProj = 0;
        
        public bool IsProjectBlank
        {
            get { return isProjectBlank; }
            set { isProjectBlank = value; NotifyPropertyChanged(); }
        }
        //public bool isProjectSaved;

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static readonly Model instance = new Model();
        // ctor
        private Model()
        {
            IsProjectBlank = true;
        }
        // singleton pattern
        public static Model Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// open documents
        /// </summary>
        public ObservableCollection<DocModel> docModels = new ObservableCollection<DocModel>();
        public ObservableCollection<DocModel> DocModels
        {
            get { return docModels; }
            set
            { 
                docModels = value;
                NotifyPropertyChanged();
            }
        }
        public DocModel currentDocument = new DocModel();
        public DocModel CurrentDocument
        {
            get { return currentDocument; }
            set
            {
                if (value != null && currentDocument != value)
                {
                    currentDocument = value;
                    instance.Content = CurrentDocument.TextContents;
                    //instance.Meta = CurrentDocument.Meta;
                    NotifyPropertyChanged();
                    Instance.rtb.Document = new FlowDocument();
                    Instance.rtb.AppendText(Instance.Content);
                    Model.Instance.gfm.CheckAllBlocks();
                    //Instance.rtb.Document = Instance.Content;
                }
                /*
                if (instance.Meta != null && instance.Meta.Length == 0 && CurrentDocument.Meta != instance.Meta)
                {
                    instance.Meta = CurrentDocument.Meta;
                    NotifyPropertyChanged();
                }*/
                //db.w("Set CurrentDocument");
            }
        }
        /*
        public bool useDocWc = true;
        public bool UseDocWc
        {
            get { return useDocWc; }
            set { useDocWc = value; NotifyPropertyChanged(); }
        }
        public bool useProjWc = true;
        public bool UseProjWc
        {
            get { return useProjWc; }
            set { useProjWc = value; NotifyPropertyChanged(); }
        }
        public bool useDayWc = false;
        public bool UseDayWc
        {
            get { return useDayWc; }
            set { useDayWc = value; NotifyPropertyChanged(); }
        }
        */
        /// <summary>
        /// the path to the project folder
        /// </summary>
        public string projectPath = "";
        public string ProjectPath
        {
            get { return projectPath; }
            set { projectPath = value; NotifyPropertyChanged(); }
        }

        public DocModel rootObject;
        public DocModel RootObject
        {
            get { return rootObject; }
            set { Instance.currentDocument = value; NotifyPropertyChanged(); }
        }
        public string Content
        {
            get
            {
                string t ="";
                if (Instance.currentDocument != null && Instance.currentDocument.textContents != null)
                {
                    t = Instance.currentDocument.textContents;
                }
                return t; 
            }
            set 
            {
                if (Instance.currentDocument != null)
                {
                    Instance.currentDocument.textContents = value;
                }
                NotifyPropertyChanged(); 
            } 
        }
        public string Meta
        {
            get
            {
                string t = "";
                if (Instance.CurrentDocument != null && Instance.CurrentDocument.meta != null)
                {
                    t = Instance.CurrentDocument.meta;
                }
                return t;
            }
            set
            {
                if (Instance.currentDocument != null)
                {
                    Instance.currentDocument.meta = value;
                }
                NotifyPropertyChanged();
                db.w(" meta changed ");
            }
        }
        public string exportPath = "";

        public string xamlString = "";
        //Stopwatch watch;
        public GfmSyntaxProvider gfm;

        #endregion

        #region Methods
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void ClearProject()
        {
            DocModels = new ObservableCollection<DocModel>();
            CurrentDocument = new DocModel();
            Content = "";
            Meta = "";
            RootObject = new DocModel();
            ProjectPath = "";
            IsProjectBlank = true;
        }
        public void SaveProjectData()
        {
            // check to see if we have a valid project
            if (rootObject != null && rootObject.pathFile != null && rootObject.pathFile.Length > 0)
            {
                using (StreamWriter sw = new StreamWriter(rootObject.pathFile + "\\project.md"))
                {
                    sw.Write(rootObject.textContents);
                }
            }
            else
            {
                db.w("no valid project path to save.");
            }
        }
        public void LoadProject(string path)
        {
            // check if a file exists at the path
            if (File.Exists(path) == true || Directory.Exists(path))
            {
                // clear
                Model.Instance.ClearProject();
                // if so, find a project path for it
                projectPath = DirectoryHelper.GetRootDir(path);
                rootObject = new DocModel(projectPath, true);
                currentDocument = rootObject;
                db.w("loaded: " + projectPath);
                // we did not actually load a project, we tried to load a file
                if (path != projectPath)
                {
                    currentDocument = GetDocByFilename(path);
                }
                // get contents
                Content = currentDocument.textContents;
                Meta = currentDocument.meta;
                // put contents to text box
                rtb.Document.Blocks.Clear();
                rtb.AppendText(Content);
                
                // highlight text
                gfm.CheckAllBlocks();
                CountAllWords();
                IsProjectBlank = false;
                NotifyPropertyChanged();
            }
        }
        public void NewProject (string path)
        {
            //string returnedPath = "";
            string projectTitle = "";
            string projectPath = "";
            string rootPath = "";
            //returnedPath = sfd.FileName;
            //projectPath = System.IO.Path.GetFullPath(returnedPath);
            projectPath = System.IO.Path.GetDirectoryName(path);
            projectTitle = System.IO.Path.GetFileNameWithoutExtension(path);
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
        public void ExportProject ()
        {
            if (Model.Instance.exportPath.Length > 0)
            {
                // https://github.com/danielwertheim/kiwi/wiki/Use-with-Asp.Net-MVC
                // http://stackoverflow.com/questions/8210974/markdownsharp-github-syntax-for-c-sharp-code
                // compile all markdown files together
                string compiledMD = "";
                foreach (DocModel dm in Model.Instance.docModels)
                {
                    // check that we should include this document
                    compiledMD += dm.textContents;
                    compiledMD += Environment.NewLine;
                }
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
        public string BackupProject ()
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
            return backupPath;
        }
        
        public DocModel GetDocByFilename(string path)
        {
            DocModel dm = new DocModel();
            foreach (DocModel di in instance.docModels)
            {
                if (di.pathFile == path)
                {
                    dm = di;
                }
            }
            NotifyPropertyChanged();
            return dm;
        }
        public void CountAllWords ()
        {
            instance.wcProj = 0;
            rootObject.GetWordCount();
            /*foreach (DocModel d in docModels)
            {
                if (d.Type == TreeItemType.Text)
                {
                    instance.wcProj += d.WordCount;
                }
            }*/
            instance.wcDoc = currentDocument.wordCount;
            instance.wcDay = 777;
            if (instance.mw != null && instance.mw.labelWcProj != null)
            {
                instance.mw.labelWcProj.Content = "Project: "+ wcProj + "w"; //works, but not testable
                instance.mw.labelWcDay.Content = "Today: " + wcDay + "w";
                instance.mw.labelWcDoc.Content = "Document:" + wcDoc + "w";
                //string Model.Instance.WcProj;
                //NotifyPropertyChanged();
            }
        }

        #endregion
    }
}
