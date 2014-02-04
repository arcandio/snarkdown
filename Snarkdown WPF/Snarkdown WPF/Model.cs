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
        public bool useProjWc = true;
        public bool useDayWc = false;

        /// <summary>
        /// form1 instance for access
        /// </summary>
        public MainWindow f1;
        /// <summary>
        /// the path to the project folder
        /// </summary>
        public string projectPath = "";

        public DocModel rootObject;
        public DocModel RootObject
        {
            get { return rootObject; }
            set { currentDocument = value; NotifyPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /*
        /// <summary>
        /// Try to open a document by index
        /// </summary>
        /// <param name="index">the number of the document on the stack</param>
        /// <returns>bool: was it successful?</returns>
        public bool GetOpenDocument(int index)
        {
            bool worked = false;

            if (index <= 0 && index > docModels.Count)
            {
                //openDocIndex = index;
                currentDocument = docModels[index];
                worked = true;
            }

            return worked;
        }
        */
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
                NotifyPropertyChanged();
            }

        }
    }
}
