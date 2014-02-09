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
        public DocModel currentDocument;
        public DocModel CurrentDocument
        {
            get { return currentDocument; }
            set
            {
                if (value != null && currentDocument != value)
                {
                    currentDocument = value;
                    instance.Content = currentDocument.textContents;
                    instance.Meta = currentDocument.meta;
                    NotifyPropertyChanged();
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
        Stopwatch watch;
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
            using (StreamWriter sw = new StreamWriter(rootObject.pathFile + "project.md"))
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
                //db.w(projectPath);
                rootObject = new DocModel(projectPath, true);
                currentDocument = rootObject;
                db.w("loaded: " + projectPath);
                // we did not actually load a project, we tried to load a file
                if (path != projectPath)
                {
                    currentDocument = GetDocByFilename(path);
                }
                Content = currentDocument.textContents;
                Meta = currentDocument.meta;
                /*
                Model.Instance.ConvertContentStringToXaml(" ");
                StringReader sr = new StringReader(Model.Instance.xamlString);
                XmlReader xr = XmlReader.Create(sr);
                mw.rtb.Document = (FlowDocument)XamlReader.Load(xr);
                */
                mw.rtb.Document.Blocks.Clear();
                mw.rtb.AppendText(Content);
                //gfm.CheckAllBlocks();
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
