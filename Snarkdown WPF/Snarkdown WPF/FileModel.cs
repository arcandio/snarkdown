using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace Snarkdown_WPF
{
    public class FileList : ObservableCollection<FileModel>
    {
        public FileList () : base ()
        {
            Model.currentDocument = new FileModel(@"F:\Freelance\repos\Snarkdown\SDWF\TestProject");
            foreach (FileModel fm in Model.docModels)
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
        /// the list of open projects accessible
        /// </summary>
        //static public List<ProjectModel> projectModels = new List<ProjectModel>();

        /// <summary>
        /// open documents
        /// </summary>
        static public ObservableCollection<FileModel> docModels = new ObservableCollection<FileModel>();

        static public FileModel currentDocument;
        //static public ProjectModel currentProject;

        static public bool useDocWc = true;
        static public bool useProjWc = true;
        static public bool useDayWc = false;

        /// <summary>
        /// form1 instance for access
        /// </summary>
        static public MainWindow f1;

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
        public List<FileModel> fileModelItems;
        public string projectRoot;
        public string projectFile;
        public FileModel rootObject;

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
            fileModelItems = new List<FileModel>();
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
            fileModelItems = new List<FileModel>();
            rootObject = new FileModel(pathToRoot, this);
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
            List<FileModel> old;
            if (fileModelItems.Count > 0)
            {
                old = fileModelItems;
            }
            // get the root dir for this path, to see if there's a project there
            string pathToRoot = DirectoryHelper.GetRootDir(pathToFolder);
            projectRoot = pathToRoot;
            // initialize a list of files
            fileModelItems = new List<FileModel>();
            rootObject = new FileModel(pathToRoot, this);
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
        /// <returns>Reference to the appropriate FileModel</returns>
        public FileModel GetFile(string path)
        {
            FileModel gf = null;
            for (int i = 0; i < fileModelItems.Count; i++)
            {
                if (path == fileModelItems[i].filePath)
                {
                    gf = fileModelItems[i];
                }
            }
            return gf;
        }
    }
    */
    public class FileModel
    {
        #region FileModel Variables

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        /// <summary>
        /// name of file, including extension
        /// </summary>
        public string fileName;
        //public string fileName { get; set; }
        /// <summary>
        /// full path to file
        /// </summary>
        public string filePath;
        //public string filePath { get; set; }
        /// <summary>
        /// An Enum describing the type of item
        /// </summary>
        public TreeItemType itemType = TreeItemType.None;

        /// <summary>
        /// path to the adjacent meta file
        /// </summary>
        public string metaPath;

        /// <summary>
        /// does the meta file exist
        /// </summary>
        public bool metaExists;
        /// <summary>
        /// the contents of the adjacent meta file
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
        public int bytes;

        /// <summary>
        /// the edit date
        /// </summary>
        public DateTime lastEdit;

        /// <summary>
        /// the string that contains the PLAIN TEXT contents of the file
        /// </summary>
        public string contents;

        /// <summary>
        /// the first few words of the Contents
        /// </summary>
        public string summary;

        /// <summary>
        /// the output of styling the text
        /// </summary>
        public string contentsStyled;

        /// <summary>
        /// does this item have children?
        /// </summary>
        public bool hasChildren = false;

        /// <summary>
        /// children items of this folder, if it is one
        /// </summary>
        public ObservableCollection<FileModel> children;

        /// <summary>
        /// the path to this file's root folder
        /// </summary>
        public string projectRoot;

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

        /// <summary>
        /// reference to this file's project model
        /// </summary>
        //public ProjectModel myProject;

        #endregion

        /// <summary>
        /// Empty constructor, blank item
        /// </summary>
        public FileModel()
        {
            // Blank Constructor
            itemType = TreeItemType.None;
        }
        /// <summary>
        /// constructs and initializes by a file path and project model
        /// </summary>
        /// <param name="path">filename to initialize</param>
        /// <param name="creator">the Project that this file belongs to</param>
        public FileModel(string path)
        {
            Initialize(path);
        }

        /// <summary>
        /// initialize an instance of FileModel that was not constructed with a path string.
        /// </summary>
        /// <param name="path">filename to initialize</param>
        public void Initialize(string path)
        {
            //db.w("INIT FM: " + path);
            // set up and calculate all variables
            filePath = path;
            //myProject = creator;
            GetFileName();
            GetItemType();
            GetItemVisibility();
            GetChildren();
            GetContents();
            GetSummary();
            GetKindleFileSize();
            GetMetaFile();
            GetEditDate();
        }

        public void Update()
        {
            // do I still exist?
            // have my contents changed?
            // should I save my contents? (isChanged)
            // should I load my contents? (contents different than loaded string)
            // get summary
            // get edit date
            // do I have any new children?
            // update children (recusion)

        }

        private void GetChildren()
        {
            if (itemType == TreeItemType.Folder && CheckFilePath(true))
            {
                string[] s = Directory.GetFileSystemEntries(filePath);
                List<string> myChildren = Directory.EnumerateFileSystemEntries(filePath).ToList<string>();
                for (int i = 0; i < myChildren.Count; i++)
                {
                    // create new child
                    FileModel fm = new FileModel(myChildren[i]);
                    // add to my children
                    if (children == null)
                    {
                        children = new ObservableCollection<FileModel>();
                    }
                    children.Add(fm);
                    // set the new instance's model
                    //fm.myProject = myProject;

                    // add to list
                    if (Model.docModels == null)
                    {
                        Model.docModels = new ObservableCollection<FileModel>();
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
                lastEdit = File.GetLastWriteTime(filePath);
            }
        }

        private void GetMetaFile()
        {
            if (CheckFilePath(false) && itemType == TreeItemType.Text)
            {
                metaPath = filePath + ".meta";
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
            else if (CheckFilePath(false) && itemType == TreeItemType.Meta)
            {
                //metaPath = filePath;
                //metaExists = CheckFilePath(metaPath);
            }
        }

        private void GetFileName()
        {
            if (CheckFilePath(true))
            {
                fileName = Path.GetFileName(filePath);
            }
        }
        /// <summary>
        /// Figure out what kind of item we are from the extension
        /// </summary>
        private void GetItemType()
        {
            itemType = TreeItemType.None;
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
                        itemType = TreeItemType.Text;
                        break;
                    case ".meta":
                        itemType = TreeItemType.Meta;
                        break;
                    case ".mobi":
                    case ".epub":
                        itemType = TreeItemType.Ebook;
                        break;
                    case ".jpg":
                    case ".png":
                    case ".bmp":
                    case ".gif":
                        itemType = TreeItemType.Image;
                        break;
                    case ".html":
                    case ".xhtml":
                    case ".xml":
                        itemType = TreeItemType.HTML;
                        break;
                    case "":
                        itemType = TreeItemType.Folder;
                        break;
                    default:
                        itemType = TreeItemType.Other;
                        break;
                }
                if (fileName == "project.md.meta")
                {
                    itemType = TreeItemType.Project;
                }
            }
            else
            {
                db.w("DID NOT PASS FILE CHECK ON TYPE: " + filePath);
            }
        }

        private void GetItemVisibility()
        {
            // does the file exist for us to check?
            /*
            if (File.Exists(filePath))
            {
                // check to see if it's read only or hidden
                string at = File.GetAttributes(filePath).ToString().ToLower();
                db.w("File Attributes: " + filePath + at);
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
            if (itemType == TreeItemType.Meta || (itemType == TreeItemType.Other && (File.Exists(filePath) != true)))
            {
                isVisible = false;
            }
        }

        /// <summary>
        /// Retrieve contents of file to the contents string.
        /// </summary>
        private void GetContents()
        {
            if (CheckFilePath(true) && itemType == TreeItemType.Text)
            {
                // check item type
                if (itemType == TreeItemType.None)
                {
                    GetItemType();
                }

                // load file
                if (itemType == TreeItemType.Text)
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        contents = sr.ReadToEnd();
                    }
                }
            }
        }
        private void GetSummary()
        {
            string sum = "";
            if (contents != null && contents.Length > 50)
            {
                sum = contents.Substring(0, 49);
            }
            else
            {
                sum = contents;
            }
            summary = sum;
        }

        /// <summary>
        /// calculate the size of the item based on theoretical kindle formatting
        /// </summary>
        private void GetKindleFileSize()
        {
            // Is this what we want? Do we store the max locations, or do we store the number of bytes?
            WordCounter.GetKindleLocation(contents);
        }

        /// <summary>
        /// do we have a valid filePath that contains a real file?
        /// </summary>
        /// <returns></returns>
        private bool CheckFilePath(bool needsToExist)
        {
            if (filePath != null && filePath.Length > 0 && (needsToExist == false || (File.Exists(filePath) || Directory.Exists(filePath))))
            {
                return true;
            }
            else
            {
                db.w("DOES NOT EXIST: " + filePath);
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
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.Write(contents);
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
                db.w("could not save file to path: " + filePath);
            }
        }
        /// <summary>
        /// loads the content and meta from this item's paths
        /// </summary>
        public void Load()
        {
            if (CheckFilePath(true))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    contents = sr.ReadToEnd();
                }
                using (StreamReader sr = new StreamReader(metaPath))
                {
                    meta = sr.ReadToEnd();
                }
            }
            else
            {
                db.w("could not load file from path: " + filePath);
            }
        }
    }
}
