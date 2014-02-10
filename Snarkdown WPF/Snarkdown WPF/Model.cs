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
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static readonly Model instance = new Model();
        // ctor
        private Model()
        {
            
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
                    instance.Meta = CurrentDocument.Meta;
                    NotifyPropertyChanged();
                    Instance.rtb.Document = new FlowDocument();
                    Instance.rtb.AppendText(Instance.Content);
                    Model.Instance.gfm.CheckAllBlocks();
                    //Instance.rtb.Document = Instance.Content;
                }
            }
        }

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
                if (Instance.currentDocument != null && Instance.currentDocument.textContents != null)
                {
                    t = Instance.currentDocument.meta;
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
        }
        public void SaveProjectData()
        {
            using (StreamWriter sw = new StreamWriter(rootObject.pathFile + "\\project.md"))
            {
                sw.Write(rootObject.textContents);
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
                NotifyPropertyChanged();
            }
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
        public void SetOpenDoc()
        {


        }
        #endregion
    }
}
