using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Snarkdown_WPF
{
    /*
    static public class MetaParsing
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
         * */

    public class MetaContainer
    {
        string[] validTags = {
                                        " * Characters:",
                                        " * Locations:",
                                        " * Progress:",
                                        " * Tags:",
                                        " * DocWords:",
                                        " * DocTarget:",
                                        " * Synopsis:",
                                        " * Title:",
                                        " * Author:",
                                        " * ProjectTarget:",
                                        " * ProjectWords:",
                                        " * Daily:",
                                        " * DailyTarget:",
                                        " * DueDate:",
                                        " * Pace:"
                                   };

        public string MetaContents { get; set; }
        private List<string> metaLeftovers = new List<string>();


        private string characters;
        public string Characters
        {
            get { return characters; }
            set { characters = value; }
        }
        private string locations;
        public string Locations
        {
            get { return locations; }
            set { locations = value; }
        }
        private string progress;
        public string Progress
        {
            get { return progress; }
            set { progress = value; }
        }
        private string tags;
        public string Tags
        {
            get { return tags; }
            set { tags = value; }
        }
        private int docWords;
        public int DocWords
        {
            get { return docWords; }
            set { docWords = value; }
        }
        private int docTarget;
        public int DocTarget
        {
            get { return docTarget; }
            set { docTarget = value; }
        }
        private string synopsis;
        public string Synopsis
        {
            get { return synopsis; }
            set { synopsis = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private string author;
        public string Author
        {
            get { return author; }
            set { author = value; }
        }
        private int projectTarget;
        public int ProjectTarget
        {
            get { return projectTarget; }
            set { projectTarget = value; }
        }
        private int projectWords;
        public int ProjectWords
        {
            get { return projectWords; }
            set { projectWords = value; }
        }
        private int daily;
        public int Daily
        {
            get { return daily; }
            set { daily = value; }
        }
        private int dailyTarget;
        public int DailyTarget
        {
            get { return dailyTarget; }
            set { dailyTarget = value; }
        }
        private DateTime dueDate;
        public DateTime DueDate
        {
            get { return dueDate; }
            set { dueDate = value; }
        }
        private int pace;
        public int Pace
        {
            get { return pace; }
            set { pace = value; }
        }

        public void ParseTagsFromString (string metaInput)
        {
            if (metaInput == null || metaInput.Length < 1)
            {
                return;
            }
            string[] lines = metaInput.Split(new char[] { '\n' });
            List<string> leftovers = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                // check for tags
                bool isTag = false;
                string foundTag = "";
                foreach (string tag in validTags)
                {
                    if (line.ToLower().StartsWith(tag.ToLower()))
                    {
                        isTag = true;
                        foundTag = tag;
                    }
                }
                // parse tags
                if (isTag)
                {
                    // we have a tag, so parse it
                    string value = line.Replace(foundTag, "").Trim();
                    // switch and parse
                    switch (foundTag.ToLower())
                    {
                        case " * characters:":
                            this.Characters = value;
                            break;
                        case " * locations:":
                            this.Locations = value;
                            break;
                        case " * progress:":
                            this.Progress = value;
                            break;
                        case " * tags:":
                            this.Tags = value;
                            break;
                        case " * docwords:":
                            this.DocWords = value.ParseIntSilent();
                            break;
                        case " * doctarget:":
                            this.DocTarget = value.ParseIntSilent();
                            break;
                        case " * synopsis:":
                            this.Synopsis = value;
                            break;
                        case " * title:":
                            this.Title = value;
                            break;
                        case " * author:":
                            this.Author = value;
                            break;
                        case " * projecttarget:":
                            this.ProjectTarget = value.ParseIntSilent();
                            break;
                        case " * projectwords:":
                            this.ProjectWords = value.ParseIntSilent();
                            break;
                        case " * daily:":
                            this.Daily = value.ParseIntSilent();
                            break;
                        case " * dailytarget:":
                            this.DailyTarget = value.ParseIntSilent();
                            break;
                        case " * duedate:":
                            this.DueDate = value.ParseDtSilent();
                            break;
                        case " * pace:":
                            this.Pace = value.ParseIntSilent();
                            break;
                        default:
                            db.w("dropped a tag.");
                            break;
                    }
                    
                    // end switch
                }
                else
                {
                    // line is not a tag, store it
                    leftovers.Add(line);
                }
            }
            metaLeftovers = leftovers;
        }
        public string BuildTagsToString ()
        {
            string build = "";

            // regex replace tag values?
            // rebuild meta completely?
            build = build + String.Join(Environment.NewLine, metaLeftovers);

            return build;
        }

        // do our notify implementation 
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
