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
    public partial class DocModel : INotifyPropertyChanged
    {
        // do our notify implementation 
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public string RelativePath
        {
            get
            {
                if (pathFile != null && pathFile.Length > 0 && Model.Instance.projectPath != null && Model.Instance.projectPath.Length > 0)
                {
                    pathRelative = pathFile.Replace(Model.Instance.projectPath, string.Empty);
                }
                else
                {
                    pathRelative = pathFile;
                }
                return pathRelative;
            }
            set
            {
                pathRelative = value;
                pathFile = Model.Instance.projectPath + pathRelative;
                NotifyPropertyChanged();
            }
        }
        public TreeItemType Type
        {
            get { return metaItemType; }
            set { NotifyPropertyChanged(); }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set { NotifyPropertyChanged(); }
        }
        public bool IsChanged
        {
            get { return isChanged; }
            set { isChanged = value;  NotifyPropertyChanged(); }
        }
        public string Snippet
        {
            get { return textSnippet; }
            set { textSnippet = value; NotifyPropertyChanged(); }
        }
        public string Synopsis
        {
            get { return metaSynopsis; }
            set { metaSynopsis = value; NotifyPropertyChanged(); }
        }


        public string TextContents
        {
            get { return textContents + "test"; }
            set { textContents = value; NotifyPropertyChanged(); }
        }
        public string TextContentsStyled
        {
            get { return textContentsStyled; }
            set { textContentsStyled = value; NotifyPropertyChanged(); }
        }
        public string Meta
        {
            get { return meta; }
            set { meta = value; NotifyPropertyChanged(); }
        }

        public string TagCharacters
        {
            get { return MetaExtractor.StringFromTags(tagCharacters); }
            set
            {
                tagCharacters = MetaExtractor.TagsFromString(value);
                NotifyPropertyChanged();
            }
        }
        public string TagLocations
        {
            get { return MetaExtractor.StringFromTags(tagLocations); }
            set
            {
                tagLocations = MetaExtractor.TagsFromString(value);
                NotifyPropertyChanged();
            }
        }
        public string TagProgress
        {
            get { return MetaExtractor.StringFromTags(tagProgress); }
            set
            {
                tagProgress = MetaExtractor.TagsFromString(value);
                NotifyPropertyChanged();
            }
        }
        public string TagOther
        {
            get { return MetaExtractor.StringFromTags(tagOther); }
            set
            {
                tagOther = MetaExtractor.TagsFromString(value);
                NotifyPropertyChanged();
            }
        }
        
    }
}