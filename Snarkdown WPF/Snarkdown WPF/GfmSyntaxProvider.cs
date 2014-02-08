using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Media;
using Snarkdown_WPF;

namespace GfmSyntax
{
    public class GfmSyntaxProvider
    {
        static public FlowDocument CheckAllBlocks(FlowDocument sec)
        {
            Stopwatch st = Stopwatch.StartNew();
            // replace rtb? http://msdn.microsoft.com/en-us/library/system.windows.controls.flowdocumentscrollviewer(v=vs.110).aspx
            // http://stackoverflow.com/questions/2068120/c-sharp-cast-entire-array
            Paragraph[] pars = Array.ConvertAll<Block,Paragraph>(sec.Blocks.ToArray<Block>(), item => (Paragraph)item);
            db.w("found blocks x" + sec.Blocks.Count + " pars x" + pars.Length);
            Random rand = new Random();
            foreach (Paragraph p in pars)
            {
                string ptext = new TextRange(p.ContentStart, p.ContentEnd).Text;
                p.Inlines.Clear();
                string[] words = ptext.Split(new char[] { ' ' });

                foreach (string w in words)
                {
                    Run newInline = new Run(w+" ");
                    BrushConverter conv = new BrushConverter();
                    string color = "#" + ("" + rand.Next()).Substring(0, 6);
                    string color2 = "#" + ("" + rand.Next()).Substring(0, 6);
                    SolidColorBrush brush = conv.ConvertFromString(color) as SolidColorBrush;
                    SolidColorBrush brush2 = conv.ConvertFromString(color2) as SolidColorBrush;
                    newInline.Foreground = brush;
                    newInline.Background = brush2;
                    p.Inlines.Add(newInline);
                }
            }
            st.Stop();
            db.w(" elapsed " + st.Elapsed);
            return sec;
        }
        public void CheckBlock(Block block)
        {

        }

        static List<string> tags = new List<string>
        {
            " _",
            " *",
            " __",
            " **",
            " ~~",
            "<!--",
            "-->"
        };
        static List<string> flags = new List<string>
        {
            "#",
            " * ",
            " + ",
            " - ",
            "\t"
        };
        public static bool StringContainsTags (string s)
        {
            bool r = false;
            foreach (string t in tags)
            {
                if (s.Contains(t) == true)
                    r = true;
            }
            return r;
        }
        public static bool StringContainsFlags (string s)
        {
            bool r = false;
            // remove tabs down to a max of 1 tab, to cover nested lists
            while (s.Contains("\t\t"))
            {
                s.Replace("\t\t", "\t");
            }
            foreach (string f in flags)
            {
                if (s.StartsWith(f) == true)
                    r = true;
            }
            return r;
        }
    }
    class GsFolder
    {
        public object analog;
        public GsFolder childFolders;
        public GsSpan childSpans;
        public string text;

        GsFolder ()
        {

        }
        GsFolder (FlowDocument fd)
        {

        }
        GsFolder (Section sec)
        {

        }
    }
    class GsSpan
    {
        public object analog;
        public string text;
    }
}
