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

        #endregion

        #region Methods
        public void ConvertContentStringToXaml(string c)
        {
            // http://msdn.microsoft.com/en-us/library/system.windows.documents.flowdocument(v=vs.110).aspx
            watch = Stopwatch.StartNew();
            c = "# heading\n*italic*\n**bold**\ntest normal *test *test test _text_ text ~~text~~ test test text?\nanother _text_ para*graph*\n * list test\n + listtest2?";
            string tempXaml = "";
            // convert string to xaml markup
            string[] lines;
            if(c != null)
            {
                // break strings into lines, wrap with paragraph
                string[] sep = {"\n"};
                lines = c.Split(sep, StringSplitOptions.None);

                // foreach line
                for (int i = 0; i < lines.Length; i++ )
                {
                    bool starOpen = false;
                    bool underOpen = false;
                    bool starOpenD = false;
                    bool underOpenD = false;

                    string line = lines[i];
                    if (GfmSyntaxProvider.StringContainsTags(line) == true)
                    {
                        // break string into words by space
                        string[] splitter = { " " };
                        string[] words = line.Split(splitter, StringSplitOptions.None);
                        // foreach string in array
                        foreach (string s in words)
                        {

                        }
                        // foreach tag in list
                        // if contains tag, add xaml tag on outside
                    }
                    if (GfmSyntaxProvider.StringContainsFlags(line) == true)
                    {
                        if (line.StartsWith("#"))
                        {
                            // check for heading, then apply font size to paragraph
                            line = "<Paragraph FontSize=\"20\" FontWeight=\"Bold\" Background=\"Orange\">" + line + "</Paragraph>";
                        }
                        else
                        {
                            line = "<Paragraph Foreground=\"Navy\" Background=\"#AAFAFF\" LineHeight=\"1\">" + line + "</Paragraph>";
                        }
                    }
                    else
                    {
                        line = "<Paragraph>" + line + "</Paragraph>";
                    }

                    lines[i] = line;
                }
                // finally put to public
                tempXaml = String.Join("\t\n", lines);
                // base styling
                tempXaml = "<FlowDocument \nxmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \nxmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n" + tempXaml;
                tempXaml += "\n</FlowDocument>";
            }
            // put markup to rtb
            xamlString = tempXaml;
            watch.Stop();
            db.w("" + watch.Elapsed);
            db.w(tempXaml);
        }
        // notification stuff
        public void ConvertXamlToStringContents()
        {
            // saving xaml to string
            // http://stackoverflow.com/questions/1917489/flowdocument-contents-as-text
            // strip xaml tags and style
            // http://stackoverflow.com/questions/4629924/convert-flowdocument-to-simple-text


        }
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
