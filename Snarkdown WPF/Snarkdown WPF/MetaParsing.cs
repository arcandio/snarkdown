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

    class MetaContainer
    {
        const string[] validTags = {
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
        private int words;
        public int Words
        {
            get { return words; }
            set { words = value; }
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
                    if (line.StartsWith(tag))
                    {
                        isTag = true;
                        foundTag = tag;
                    }
                }
                // parse tags
                if (isTag)
                {
                    // we have a tag, so parse it
                    string value = line.Replace(foundTag, "");
                    // switch and parse
                    
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
