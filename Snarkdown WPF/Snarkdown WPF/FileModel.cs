using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace Snarkdown_WPF
{
    public class FileList : ObservableCollection<DocModel>
    {
        public FileList () : base ()
        {
            Model.currentDocument = new DocModel(@"F:\Freelance\repos\Snarkdown\SDWF\TestProject");
            foreach (DocModel fm in Model.docModels)
            {
                Add(fm);
            }
        }
    }
    /// <summary>
    /// the model of all data in the program
    /// </summary>
    static class Model
    {
        /// <summary>
        /// open documents
        /// </summary>
        static public ObservableCollection<DocModel> docModels = new ObservableCollection<DocModel>();

        static public DocModel currentDocument;

        static public bool useDocWc = true;
        static public bool useProjWc = true;
        static public bool useDayWc = false;

        /// <summary>
        /// form1 instance for access
        /// </summary>
        static public MainWindow f1;
        /// <summary>
        /// the path to the project folder
        /// </summary>
        static public string projectPath = @"F:\Freelance\repos\Snarkdown\SDWF\TestProject";

        /// <summary>
        /// Try to open a document by index
        /// </summary>
        /// <param name="index">the number of the document on the stack</param>
        /// <returns>bool: was it successful?</returns>
        static public bool GetOpenDocument(int index)
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
    }
    /*
    public class ProjectModel
    {

        // replacing File Model
        public List<DocModel> fileModelItems;
        public string projectRoot;
        public string projectFile;
        public DocModel rootObject;

        public int totalWC;
        public int totalT;
        public int dailyWC;
        public int dailyT;

        public bool isBlank = true;

        //private int maxFiles = 100;
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public ProjectModel()
        {
            // at least set up the lists and add it to the model
            fileModelItems = new List<DocModel>();
            Model.projectModels.Add(this);
            Model.currentProject = this;
            // blank constructor, so blank instance
            isBlank = true;
        }
        /// <summary>
        /// creates a project at the directory
        /// </summary>
        /// <param name="pathToFile">a path to a file or folder target</param>
        public ProjectModel(string pathToFile)
        {
            // it's blank until we finish initializing it, just in case
            isBlank = true;
            // first convert to a folder, then to init
            Initialize(Path.GetDirectoryName(pathToFile));
        }

        private void Initialize(string pathToFolder)
        {
            // get the root dir for this path, to see if there's a project there
            string pathToRoot = DirectoryHelper.GetRootDir(pathToFolder);
            projectRoot = pathToRoot;
            // initialize a list of files
            fileModelItems = new List<DocModel>();
            rootObject = new DocModel(pathToRoot, this);
            //rootObject.myProject = this;
            fileModelItems.Add(rootObject);
            // add this to the model's list
            Model.projectModels.Add(this);
            Model.currentProject = this;
            // we've finished, so make sure this is marked as not blank
            isBlank = false;
        }
        /// <summary>
        /// sets the project's directory up and resets the item list
        /// </summary>
        /// <param name="pathToFolder">a path to a file or folder target</param>
        private void Reinitialize(string pathToFolder)
        {
            // store old file list
            List<DocModel> old;
            if (fileModelItems.Count > 0)
            {
                old = fileModelItems;
            }
            // get the root dir for this path, to see if there's a project there
            string pathToRoot = DirectoryHelper.GetRootDir(pathToFolder);
            projectRoot = pathToRoot;
            // initialize a list of files
            fileModelItems = new List<DocModel>();
            rootObject = new DocModel(pathToRoot, this);
            //rootObject.myProject = this;
            fileModelItems.Add(rootObject);
            // add this to the model's list
            //Model.projectModels.Add(this);
            Model.currentProject = this;
        }
        /// <summary>
        /// update the info for the project
        /// </summary>
        public void Update()
        {
            // todo
        }

        /// <summary>
        /// retrieve file from the project's list
        /// </summary>
        /// <param name="path">path to try to match</param>
        /// <returns>Reference to the appropriate DocModel</returns>
        public DocModel GetFile(string path)
        {
            DocModel gf = null;
            for (int i = 0; i < fileModelItems.Count; i++)
            {
                if (path == fileModelItems[i].pathFile)
                {
                    gf = fileModelItems[i];
                }
            }
            return gf;
        }
    }
    */
    public class DocModel
    {
        #region fields


        /// <summary>
        /// name of file, including extension
        /// </summary>
        public string fileName;
        /// <summary>
        /// full path to file
        /// </summary>
        public string pathFile;
        /// <summary>
        /// An Enum describing the type of item
        /// </summary>
        public TreeItemType metaItemType = TreeItemType.None;
        /// <summary>
        /// the path to this file relative to the project folder
        /// </summary>
        public string pathRelative;
        /// <summary>
        /// path to the adjacent meta file
        /// </summary>
        public string metaPath;
        /// <summary>
        /// does the meta file exist
        /// </summary>
        public bool metaExists;
        /// <summary>
        /// the textContents of the adjacent meta file
        /// </summary>
        public string meta;
        /// <summary>
        /// words in this document item
        /// </summary>
        public int wordCount;
        /// <summary>
        /// target words for this document item
        /// </summary>
        public int wordCountTarget;
        /// <summary>
        /// byte length adjusted for Kindle
        /// </summary>
        public int metaByteLength;
        /// <summary>
        /// the edit date
        /// </summary>
        public DateTime metaLastEdit;
        /// <summary>
        /// the string that contains the PLAIN TEXT textContents of the file
        /// </summary>
        public string textContents;
        /// <summary>
        /// the first few words of the Contents
        /// </summary>
        public string textSnippet;
        /// <summary>
        /// the user-written metaSynopsis stored in the meta file
        /// </summary>
        public string metaSynopsis;
        /// <summary>
        /// the output of styling the text for caching
        /// </summary>
        public string textContentsStyled;
        /// <summary>
        /// does this item have children?
        /// </summary>
        public bool hasChildren = false;

        /// <summary>
        /// children items of this folder, if it is one
        /// </summary>
        public ObservableCollection<DocModel> children;

        /// <summary>
        /// is this document open in our editor?
        /// </summary>
        public bool isOpen = false;

        /// <summary>
        /// is the document dirty?
        /// </summary>
        public bool isChanged = false;

        /// <summary>
        /// is this object visible in the file tree?
        /// </summary>
        public bool isVisible = true;
        /*
        public bool isReadOnly = false;
        public bool isFsHidden = false;
         */
        public bool isNew = false;

        public List<string> tagCharacters;
        public List<string> tagLocations;
        public List<string> tagProgress;
        public List<string> tagOther;

        #endregion

        /// <summary>
        /// Empty constructor, blank item
        /// </summary>
        public DocModel()
        {
            // Blank Constructor
            metaItemType = TreeItemType.None;
        }
        /// <summary>
        /// constructs and initializes by a file path and project model
        /// </summary>
        /// <param name="path">filename to initialize</param>
        /// <param name="creator">the Project that this file belongs to</param>
        public DocModel(string path)
        {
            Initialize(path);
        }

        /// <summary>
        /// initialize an instance of DocModel that was not constructed with a path string.
        /// </summary>
        /// <param name="path">filename to initialize</param>
        public void Initialize(string path)
        {
            //db.w("INIT FM: " + path);
            // set up and calculate all variables
            pathFile = path;
            //myProject = creator;
            GetFileName();
            GetItemType();
            GetItemVisibility();
            GetChildren();
            GetContents();
            GetSnippet();
            GetKindleFileSize();
            GetMetaFile();
            GetEditDate();
        }

        public void Update()
        {
            // do I still exist?
            // have my textContents changed?
            // should I save my textContents? (isChanged)
            // should I load my textContents? (textContents different than loaded string)
            // get summary
            // get edit date
            // do I have any new children?
            // update children (recusion)

        }

        private void GetChildren()
        {
            if (metaItemType == TreeItemType.Folder && CheckFilePath(true))
            {
                string[] s = Directory.GetFileSystemEntries(pathFile);
                List<string> myChildren = Directory.EnumerateFileSystemEntries(pathFile).ToList<string>();
                for (int i = 0; i < myChildren.Count; i++)
                {
                    // create new child
                    DocModel fm = new DocModel(myChildren[i]);
                    // add to my children
                    if (children == null)
                    {
                        children = new ObservableCollection<DocModel>();
                    }
                    children.Add(fm);
                    // set the new instance's model
                    //fm.myProject = myProject;

                    // add to list
                    if (Model.docModels == null)
                    {
                        Model.docModels = new ObservableCollection<DocModel>();
                    }
                    Model.docModels.Add(fm);
                }
                hasChildren = true;
            }
        }

        private void GetEditDate()
        {
            if (CheckFilePath(true))
            {
                metaLastEdit = File.GetLastWriteTime(pathFile);
            }
        }

        private void GetMetaFile()
        {
            if (CheckFilePath(false) && metaItemType == TreeItemType.Text)
            {
                metaPath = pathFile + ".meta";
                metaExists = CheckFilePath(metaPath, true);
                // connect file to meta
                if (metaExists)
                {
                    using (StreamReader sr = new StreamReader(metaPath))
                    {
                        meta = sr.ReadToEnd();
                    }
                }
            }
            else if (CheckFilePath(false) && metaItemType == TreeItemType.Meta)
            {
                //metaPath = pathFile;
                //metaExists = CheckFilePath(metaPath);
            }
        }

        private void GetFileName()
        {
            if (CheckFilePath(true))
            {
                fileName = Path.GetFileName(pathFile);
            }
        }
        /// <summary>
        /// Figure out what kind of item we are from the extension
        /// </summary>
        private void GetItemType()
        {
            metaItemType = TreeItemType.None;
            if (CheckFilePath(true))
            {
                string ex = Path.GetExtension(fileName);

                switch (ex)
                {
                    case ".txt":
                    case ".md":
                    case ".mmd":
                    case ".mkdn":
                    case ".markdown":
                        metaItemType = TreeItemType.Text;
                        break;
                    case ".meta":
                        metaItemType = TreeItemType.Meta;
                        break;
                    case ".mobi":
                    case ".epub":
                        metaItemType = TreeItemType.Ebook;
                        break;
                    case ".jpg":
                    case ".png":
                    case ".bmp":
                    case ".gif":
                        metaItemType = TreeItemType.Image;
                        break;
                    case ".html":
                    case ".xhtml":
                    case ".xml":
                        metaItemType = TreeItemType.HTML;
                        break;
                    case "":
                        metaItemType = TreeItemType.Folder;
                        break;
                    default:
                        metaItemType = TreeItemType.Other;
                        break;
                }
                if (fileName == "project.md.meta")
                {
                    metaItemType = TreeItemType.Project;
                }
            }
            else
            {
                db.w("DID NOT PASS FILE CHECK ON TYPE: " + pathFile);
            }
        }

        private void GetItemVisibility()
        {
            // does the file exist for us to check?
            /*
            if (File.Exists(pathFile))
            {
                // check to see if it's read only or hidden
                string at = File.GetAttributes(pathFile).ToString().ToLower();
                db.w("File Attributes: " + pathFile + at);
                if (at.Contains("readonly"))
                {
                    isReadOnly = true;
                }
                if (at.Contains("hidden"))
                {
                    isFsHidden = true;
                }
            }*/
            /*else
            {
                isVisible = false;
            }*/
            if (metaItemType == TreeItemType.Meta || (metaItemType == TreeItemType.Other && (File.Exists(pathFile) != true)))
            {
                isVisible = false;
            }
        }

        /// <summary>
        /// Retrieve textContents of file to the textContents string.
        /// </summary>
        private void GetContents()
        {
            if (CheckFilePath(true) && metaItemType == TreeItemType.Text)
            {
                // check item type
                if (metaItemType == TreeItemType.None)
                {
                    GetItemType();
                }

                // load file
                if (metaItemType == TreeItemType.Text)
                {
                    using (StreamReader sr = new StreamReader(pathFile))
                    {
                        textContents = sr.ReadToEnd();
                    }
                }
            }
        }
        private void GetSnippet()
        {
            string sum = "";
            if (textContents != null && textContents.Length > 50)
            {
                sum = textContents.Substring(0, 49);
            }
            else
            {
                sum = textContents;
            }
            textSnippet = sum;
        }

        /// <summary>
        /// calculate the size of the item based on theoretical kindle formatting
        /// </summary>
        private void GetKindleFileSize()
        {
            // Is this what we want? Do we store the max locations, or do we store the number of bytes?
            WordCounter.GetKindleLocation(textContents);
        }

        /// <summary>
        /// do we have a valid pathFile that contains a real file?
        /// </summary>
        /// <returns></returns>
        private bool CheckFilePath(bool needsToExist)
        {
            if (pathFile != null && pathFile.Length > 0 && (needsToExist == false || (File.Exists(pathFile) || Directory.Exists(pathFile))))
            {
                return true;
            }
            else
            {
                db.w("DOES NOT EXIST: " + pathFile);
                return false;
            }
        }

        /// <summary>
        /// overload passing in a path string checks the string instead
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CheckFilePath(string path, bool needsToExist)
        {

            if (path != null && path.Length > 0 && (needsToExist == false || File.Exists(path) == true))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// saves the content and meta from this item's paths
        /// </summary>
        public void Save()
        {
            if (CheckFilePath(false))
            {
                using (StreamWriter sw = new StreamWriter(pathFile))
                {
                    sw.Write(textContents);
                }
                if (meta.Length > 0)
                {
                    using (StreamWriter sw = new StreamWriter(metaPath))
                    {
                        sw.Write(meta);
                    }
                }
            }
            else
            {
                db.w("could not save file to path: " + pathFile);
            }
        }
        /// <summary>
        /// loads the content and meta from this item's paths
        /// </summary>
        public void Load()
        {
            if (CheckFilePath(true))
            {
                using (StreamReader sr = new StreamReader(pathFile))
                {
                    textContents = sr.ReadToEnd();
                }
                using (StreamReader sr = new StreamReader(metaPath))
                {
                    meta = sr.ReadToEnd();
                }
            }
            else
            {
                db.w("could not load file from path: " + pathFile);
            }
        }
    }
}
