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
using System.Windows;

namespace Snarkdown_WPF
{
    public sealed partial class Model : INotifyPropertyChanged
    {
        public bool autoMode = false;
        public bool alwaysAcceptFsChange = false;
        public FileSystemWatcher fsw;

        private void SetupFsw()
        {
            if (Model.Instance.projectPath != null && Model.Instance.projectPath.Length > 0)
            {
                /*
                fsw = new FileSystemWatcher(Model.Instance.projectPath);
                fsw.IncludeSubdirectories = true;
                fsw.EnableRaisingEvents = true;
                fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                fsw.Created += new FileSystemEventHandler(Fs_FileWasCreated);
                fsw.Deleted += new FileSystemEventHandler(Fs_FileWasDeleted);
                fsw.Changed += new FileSystemEventHandler(Fs_FileWasChanged);
                fsw.Renamed += new RenamedEventHandler(Fs_FileWasRenamed);
                 * */
            }
        }

        private void Fs_FileWasCreated(object sender, FileSystemEventArgs e)
        {
            db.w("File Event: " + e.FullPath + " " + e.ChangeType);
            string ext = Path.GetExtension(e.FullPath);
            if (DirectoryHelper.validFileTypes.Contains(ext) || ext == ".mdmeta")
            {
                bool acceptChange = alwaysAcceptFsChange;
                if (autoMode == false)
                {
                    string text = "A " + ext + " file has been added to the project folder: " + Environment.NewLine;
                    text += e.FullPath + Environment.NewLine;
                    text += "Do you want to add it to the project?";
                    string caption = "File Added";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxResult result = MessageBox.Show(text, caption, button);
                    if (result == MessageBoxResult.Yes)
                        acceptChange = true;
                    else
                        acceptChange = false;
                }
                if (acceptChange == true)
                {
                    if (ext != ".mdmeta")
                    {
                        // find parent
                        string directory = Path.GetDirectoryName(e.FullPath);
                        DocModel parent = GetDocByFilename(directory);
                        // create docmodel
                        DocModel newFile = new DocModel(e.FullPath, false);
                        // add to parent
                        parent.children.Add(newFile);
                        // add to list
                        //DocModels.Add(newFile);
                        // refresh list
                        Model.Instance.NotifyPropertyChanged();
                    }
                    else
                    {
                        string nonMetaPath = e.FullPath.Substring(0, e.FullPath.LastIndexOf("meta"));
                        DocModel nonMetaObject = GetDocByFilename(nonMetaPath);
                        nonMetaObject.Update();
                        Model.Instance.NotifyPropertyChanged();
                    }
                }
            }
        }
        private void Fs_FileWasDeleted(object sender, FileSystemEventArgs e)
        {
            db.w("File Event: " + e.FullPath + " " + e.ChangeType);
            string ext = Path.GetExtension(e.FullPath);
            if (DirectoryHelper.validFileTypes.Contains(ext) || ext == ".mdmeta")
            {
                bool acceptChange = alwaysAcceptFsChange;
                if (autoMode == false)
                {
                    string text = "A " + ext + " file has been removed to the project folder: " + Environment.NewLine;
                    text += e.FullPath + Environment.NewLine;
                    text += "Do you want to remove it from the project?";
                    string caption = "File Deleted";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxResult result = MessageBox.Show(text, caption, button);
                    if (result == MessageBoxResult.Yes)
                        acceptChange = true;
                    else
                        acceptChange = false;
                }
                if (acceptChange == true)
                {
                    if (ext != ".mdmeta")
                    {
                        // find parent
                        string directory = Path.GetDirectoryName(e.FullPath);
                        DocModel parent = GetDocByFilename(directory);
                        // find object
                        DocModel delFile = GetDocByFilename(e.FullPath);
                        // remove from to parent
                        parent.children.Remove(delFile);
                        // Remvove from list
                        DocModels.Remove(delFile);
                        //delFile
                        // refresh list
                        Model.Instance.NotifyPropertyChanged();
                    }
                    else
                    {
                        string nonMetaPath = e.FullPath.Substring(0, e.FullPath.LastIndexOf("meta"));
                        DocModel nonMetaObject = GetDocByFilename(nonMetaPath);
                        nonMetaObject.Update();
                        Model.Instance.NotifyPropertyChanged();
                    }
                }
            }
        }
        private void Fs_FileWasChanged(object sender, FileSystemEventArgs e)
        {
            db.w("File Event: " + e.FullPath + " " + e.ChangeType);
            string ext = Path.GetExtension(e.FullPath);
            if (DirectoryHelper.validFileTypes.Contains(ext) || ext == ".mdmeta")
            {
                bool acceptChange = alwaysAcceptFsChange;
                if (autoMode == false)
                {
                    string text = "A "+ext+" file has been changed in the project folder: " + Environment.NewLine;
                    text += e.FullPath + Environment.NewLine;
                    text += "Do you want to update it?";
                    string caption = "File Changed";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxResult result = MessageBox.Show(text, caption, button);
                    if (result == MessageBoxResult.Yes)
                        acceptChange = true;
                    else
                        acceptChange = false;
                }
                if (acceptChange == true)
                {
                    if (ext != ".mdmeta")
                    {
                        // find object
                        DocModel changeFile = GetDocByFilename(e.FullPath);
                        changeFile.Update();
                        // refresh list
                        Model.Instance.NotifyPropertyChanged();
                    }
                    else
                    {
                        string nonMetaPath = e.FullPath.Substring(0,e.FullPath.LastIndexOf("meta"));
                        DocModel fileObject = GetDocByFilename(nonMetaPath);
                        db.w(fileObject.pathFile + " --- "+ e.FullPath);
                        fileObject.Update();
                    }
                }
            }

            // update contents of file/meta
        }
        private void Fs_FileWasRenamed(object sender, RenamedEventArgs e)
        {
            db.w("File Event: " + e.FullPath + " " + e.ChangeType);
            string ext = Path.GetExtension(e.FullPath);
            if (DirectoryHelper.validFileTypes.Contains(ext) || ext == ".mdmeta")
            {
                bool acceptChange = alwaysAcceptFsChange;
                if (autoMode == false)
                {
                    string text = "A " + ext + " file has been renamed in the project folder: " + Environment.NewLine;
                    text += e.FullPath + Environment.NewLine;
                    text += "Do you want to update it?";
                    string caption = "File Renamed";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxResult result = MessageBox.Show(text, caption, button);
                    if (result == MessageBoxResult.Yes)
                        acceptChange = true;
                    else
                        acceptChange = false;
                }
                if (acceptChange == true)
                {
                    if (ext != ".mdmeta")
                    {
                        // find object
                        DocModel changeFile = GetDocByFilename(e.OldFullPath);
                        changeFile.pathFile = e.FullPath;
                        changeFile.Update();
                        // refresh list
                        Model.Instance.NotifyPropertyChanged();
                    }
                    else
                    {
                        string nonMetaPath = e.FullPath.Substring(0, e.OldFullPath.LastIndexOf("meta"));
                        DocModel fileObject = GetDocByFilename(nonMetaPath);
                        db.w(fileObject.pathFile + " --- " + e.FullPath);
                        fileObject.Update();
                    }
                }
            }

            // get new and old paths
            // change docmodel paths and name
        }

    }
}
