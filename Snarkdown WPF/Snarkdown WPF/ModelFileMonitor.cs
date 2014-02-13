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
                fsw = new FileSystemWatcher(Model.Instance.projectPath);
                fsw.IncludeSubdirectories = true;
                fsw.EnableRaisingEvents = true;
                fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                fsw.Created += new FileSystemEventHandler(Fs_FileWasCreated);
                fsw.Deleted += new FileSystemEventHandler(Fs_FileWasDeleted);
                fsw.Changed += new FileSystemEventHandler(Fs_FileWasChanged);
                fsw.Renamed += new RenamedEventHandler(Fs_FileWasRenamed);
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
                    string text = "A file has been added to the project folder: " + Environment.NewLine;
                    text += e.FullPath + Environment.NewLine;
                    text += "Do you want to update them ?";
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
            }
        }
        private void Fs_FileWasDeleted(object sender, FileSystemEventArgs e)
        {
            db.w("File Event: " + e.FullPath + " " + e.ChangeType);

            // remove docmodel from list
        }
        private void Fs_FileWasChanged(object sender, FileSystemEventArgs e)
        {
            db.w("File Event: " + e.FullPath + " " + e.ChangeType);

            // update contents of file/meta
        }
        private void Fs_FileWasRenamed(object sender, RenamedEventArgs e)
        {
            db.w("File Event: " + e.FullPath + " " + e.ChangeType);
            
            // get new and old paths
            // change docmodel paths and name
        }

    }
}
