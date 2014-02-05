using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Snarkdown_WPF
{
    public enum TreeItemType
    {
        None,
        /// <summary>
        /// a text file, .txt, .md, .mkd, .mmd, etc
        /// </summary>
        Text,
        /// <summary>
        /// a folder containing other items
        /// </summary>
        Folder,
        /// <summary>
        /// a meta file that describes a text file
        /// </summary>
        Meta,
        /// <summary>
        /// a project file that describes a whole project
        /// </summary>
        Project,
        Image,
        HTML,
        Ebook,
        Other
    }

    /// <summary>
    /// Meta Extractor helper class
    /// </summary>
    static public class MetaExtractor
    {
        /// <summary>
        /// find a value in a meta file
        /// </summary>
        /// <param name="inputString">the meta file to look inside</param>
        /// <param name="searchString">the meta key to look for</param>
        /// <returns>value: integer value or -1 for failure</returns>
        static public int PullNumber(string inputString, string searchString)
        {
            int n = -1;

            // get the line TODO

            // get the text number

            // parse to int

            return n;
        }
        static public List<string> TagsFromString(string s)
        {
            List<string> tags = new List<string>();
            string[] sep = { "," };
            tags = s.Replace(", ", ",").Split(sep, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            return tags;
        }
        static public String StringFromTags(List<string> tags)
        {
            string s = "";
            if (tags != null && tags.Count > 0)
            { 
                s = String.Join(", ", tags);
            }
            return s;
        }
    }
    static class DirectoryHelper
    {
        const string validFileTypes = ".md .mkdn .txt .markdown .mmd";
        /// <summary>
        /// Find the root directory of a given file
        /// </summary>
        /// <param name="startPath">file to search by</param>
        /// <returns>string: path to root folder</returns>
        static public string GetRootDir(string startPath)
        {
            string rootDir = "";
            bool hasProjectFile = false;
            string modPath = "";
            if (startPath.Length > 0)
            {
                // the initial path is big enough to check
                modPath = Path.GetFullPath(startPath);
                if (Directory.Exists(modPath))
                {
                    // the initial path is a valid, real directory on the machine
                    // set the max search depth
                    int searchDepth = 6;
                    // store the path root to check against
                    string pr = Path.GetPathRoot(modPath);

                    // check parent directories until we find "project.md.meta" or run out of depth
                    while (searchDepth > 0)
                    {
                        if (File.Exists(modPath + "project.md"))
                        {
                            // did find the project file in this directory
                            modPath = Path.GetDirectoryName(modPath);
                            hasProjectFile = true;
                            // end the search
                            searchDepth = 0;
                        }
                        else if (modPath != pr)
                        {
                            // did not find the project file in this directory
                            hasProjectFile = false;
                            // modify the directory up one.
                            modPath = Directory.GetParent(modPath).FullName;

                            searchDepth--;
                        }
                        else
                        {
                            hasProjectFile = false;
                            searchDepth = 0;
                        }
                    }
                }

                // apply path
                if (hasProjectFile)
                {
                    rootDir = modPath;
                    //projectFile = projectRoot + "project.md.meta";
                    db.w("HAD PROJECT FILE: " + rootDir);
                }
                else
                {
                    if (Path.GetExtension(startPath) != "")
                    {
                        rootDir = Path.GetDirectoryName(startPath);
                        //projectFile = projectRoot + "project.md.meta";

                    }
                    else
                    {
                        rootDir = startPath;
                    }
                    db.w("HAD NO PROJECT: " + rootDir + " - " + startPath);
                }

            }
            return rootDir;
        }

        // utilities
        /// <summary>
        /// checks to see if a file at a path is already open in the editor
        /// </summary>
        /// <param name="path">file path to check</param>
        /// <returns>int: index of file or -1 if not open</returns>
        public static int isDocOpen(string path)
        {
            int isOpen = -1;

            for (int i = 0; i < Model.Instance.docModels.Count; i++)
            {
                if (Model.Instance.docModels[i].pathFile == path)
                {
                    isOpen = i;
                }
            }

            return isOpen;
        }
        /// <summary>
        /// check to see if our path is a valid object to display in our tree
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsPathValidFile (string path)
        {
            bool r = false;
            string ex = Path.GetExtension(path);
            // check if we have an extension
            if (ex == "")
            {
                // no extension should mean a directory
                if (Directory.Exists(path))
                {
                    r = true;
                }
            }
            else
            {
                // we have an extension to check
                if (validFileTypes.Contains(ex) == true)
                {
                    // we have an extension that we found in our list of extensions
                    r = true;
                }
            }

            return r;
        }

    }
    static public class db
    {
        /// <summary>
        /// output string to debug
        /// </summary>
        /// <param name="w">debug output</param>
        static public void w(string w)
        {
            System.Diagnostics.Debug.WriteLine(w.ToUpper());
        }
    }
}
