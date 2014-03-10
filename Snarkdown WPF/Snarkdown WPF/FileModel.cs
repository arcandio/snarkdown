using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace Snarkdown_WPF
{
    public partial class DocModel
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
        public MetaContainer metaData;
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
        public DocModel(string path, bool isRoot)
        {
                Initialize(path, isRoot);
        }

        /// <summary>
        /// initialize an instance of DocModel that was not constructed with a path string.
        /// </summary>
        /// <param name="path">filename to initialize</param>
        public void Initialize(string path, bool isRoot)
        {
            Model.Instance.docModels.Add(this);
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
            GetMetaTags();
            GetSummary();
            GetEditDate();
            GetWordCount();

            if (isRoot == true)
            {
                pathRelative = "ROOT DIR";
                isVisible = false;
            }
        }

        public int GetWordCount()
        {
            if (Type == TreeItemType.Text && textContents != null && textContents.Length > 0)
            {
                wordCount = WordCounter.CountWordsInString(textContents);
                wordCountTarget = metaData.DocTarget;
                metaData.DocWords = wordCount;
            }
            else if (Type == TreeItemType.Folder)
            {
                wordCount = 0;
                wordCountTarget = 0;
                foreach (DocModel d in children)
                {
                    d.GetWordCount();
                    wordCount += d.wordCount;
                    wordCountTarget += d.wordCountTarget;
                }
            }
            else
            {
                wordCount = 0;
                wordCountTarget = 0;
            }
            return wordCount;
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
            GetContents();
            GetSnippet();
            GetKindleFileSize();
            GetMetaFile();
            GetMetaTags();
            GetSummary();
            GetEditDate();
            GetWordCount();

        }

        private void GetChildren()
        {
            if (metaItemType == TreeItemType.Folder && /*CheckFilePath(true)*/Directory.Exists(pathFile))
            {
                string[] s = Directory.GetFileSystemEntries(pathFile);
                List<string> myChildren = Directory.EnumerateFileSystemEntries(pathFile).ToList<string>();
                for (int i = 0; i < myChildren.Count; i++)
                {
                    // check if this is a valid file object to show
                    if (DirectoryHelper.IsPathValidFile(myChildren[i]) == true)
                    {
                        // create new child
                        DocModel fm = new DocModel(myChildren[i], false);
                        // add to my children
                        if (children == null)
                        {
                            children = new ObservableCollection<DocModel>();
                        }
                        children.Add(fm);
                        // set the new instance's model
                        //fm.myProject = myProject;

                        // add to list
                        if (Model.Instance.docModels == null)
                        {
                            Model.Instance.docModels = new ObservableCollection<DocModel>();
                        }
                        //Model.Instance.docModels.Add(fm);
                        //db.w("doc models: " + Model.Instance.docModels.Count);
                    }
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
                metaPath = pathFile + "meta";
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
        private void GetMetaTags ()
        {
            // do gathering of meta info from here
            if (metaData == null)
            {
                metaData = new MetaContainer();
            }
            metaData.ParseTagsFromString(meta);
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
                    //case ".meta":
                    case ".mdmeta":
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
                if (fileName == "project.md")
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
            if (CheckFilePath(true) && (metaItemType == TreeItemType.Text || metaItemType == TreeItemType.Project))
            {
                // check item type
                if (metaItemType == TreeItemType.None)
                {
                    GetItemType();
                }

                // load file
                if (metaItemType == TreeItemType.Text || metaItemType == TreeItemType.Project)
                {
                    //using (StreamReader sr = new StreamReader(pathFile))
                    FileStream fs = DirectoryHelper.GetStream(pathFile, 20);
                    if (fs != null)
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            textContents = sr.ReadToEnd();
                        }
                        fs.Close();
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
        private void GetSummary ()
        {
            string sum = "";
            if (meta != null && meta.Length > 50)
            {
                sum = meta.Substring(0, 49);
            }
            else
            {
                sum = meta;
            }
            metaSynopsis = sum;
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
            // save the project data.
            //Model.Instance.SaveProjectData();
            if (CheckFilePath(false))
            {
                using (StreamWriter sw = new StreamWriter(pathFile))
                {
                    sw.Write(textContents);
                }
                if (metaPath != null && meta != null && meta.Length > 0)
                {
                    if (metaPath != null || metaPath.Length < 1)
                    {
                        metaPath = pathFile+"meta";
                    }
                    using (StreamWriter sw = new StreamWriter(metaPath))
                    {
                        sw.Write(meta);
                        //db.w("wrote to meta");
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
