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
            }
        }
        public TreeItemType Type
        {
            get { return metaItemType; }
            set { }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set { }
        }
        public bool IsChanged
        {
            get { return isChanged; }
            set { }
        }
        public string Snippet
        {
            get { return textSnippet; }
            set { }
        }
        public string Synopsis
        {
            get { return meta; }
            set { metaSynopsis = value; }
        }
        public string TagCharacters
        {
            get { return MetaExtractor.StringFromTags(tagCharacters); }
            set
            {
                tagCharacters = MetaExtractor.TagsFromString(value);
            }
        }
        public string TagLocations
        {
            get { return MetaExtractor.StringFromTags(tagLocations); }
            set
            {
                tagLocations = MetaExtractor.TagsFromString(value);
            }
        }
        public string TagProgress
        {
            get { return MetaExtractor.StringFromTags(tagProgress); }
            set
            {
                tagProgress = MetaExtractor.TagsFromString(value);
            }
        }
        public string TagOther
        {
            get { return MetaExtractor.StringFromTags(tagOther); }
            set
            {
                tagOther = MetaExtractor.TagsFromString(value);
            }
        }
    }
}