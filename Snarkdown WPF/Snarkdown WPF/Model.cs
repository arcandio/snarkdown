using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
            set { currentDocument = value; NotifyPropertyChanged(); }
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void LoadProject(string path)
        {
            // check if a file exists at the path
            if (File.Exists(path) == true || Directory.Exists(path))
            {
                // if so, find a project path for it
                projectPath = DirectoryHelper.GetRootDir(path);
                //db.w(projectPath);
                rootObject = new DocModel(projectPath);
                currentDocument = rootObject;
                db.w("loaded: " + projectPath);
                // we did not actually load a project, we tried to load a file
                if (path != projectPath)
                {
                    currentDocument = GetDocByFilename(path);
                }
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
    }
}
