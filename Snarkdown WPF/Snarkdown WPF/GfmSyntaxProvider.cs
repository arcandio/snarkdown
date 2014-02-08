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
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace GfmSyntax
{
    public class GfmSyntaxProvider
    {
        /* http://msdn.microsoft.com/en-us/library/system.windows.documents.flowdocument(v=vs.110).aspx
         * http://msdn.microsoft.com/en-us/library/system.windows.documents.textrange(v=vs.110).aspx
         * http://msdn.microsoft.com/en-us/library/system.windows.documents.inline(v=vs.110).aspx
         * http://msdn.microsoft.com/en-us/library/system.windows.documents.block(v=vs.110).aspx
         * http://msdn.microsoft.com/en-us/library/system.windows.documents.paragraph(v=vs.110).aspx
         * http://msdn.microsoft.com/en-us/library/system.windows.controls.richtextbox.caretposition(v=vs.110).aspx
         * http://msdn.microsoft.com/en-us/library/system.windows.documents.textpointer.getinsertionposition(v=vs.110).aspx
         * 
         */
        static public void CheckAllBlocks()
        {
            if (Model.Instance.mw.rtb.Document == null)
            {
                Model.Instance.mw.rtb.Document = new FlowDocument();
            }
            CheckAllBlocks(Model.Instance.mw.rtb.Document, Model.Instance.mw.rtb);
        }
        static public void CheckAllBlocks(FlowDocument fd, RichTextBox rtb)
        {
            Stopwatch st = Stopwatch.StartNew();
            int pointer = rtb.CaretPosition.GetOffsetToPosition(rtb.CaretPosition.DocumentStart) * -1;
            // replace rtb? http://msdn.microsoft.com/en-us/library/system.windows.controls.flowdocumentscrollviewer(v=vs.110).aspx
            // http://stackoverflow.com/questions/2068120/c-sharp-cast-entire-array
            Paragraph[] pars = Array.ConvertAll<Block,Paragraph>(fd.Blocks.ToArray<Block>(), item => (Paragraph)item);
            //db.w("found blocks x" + fd.Blocks.Count + " pars x" + pars.Length);
            Random rand = new Random();
            foreach (Paragraph p in pars)
            {
                string ptext = new TextRange(p.ContentStart, p.ContentEnd).Text;
                p.Inlines.Clear();
                //string[] words = ptext.Split(new char[] { ' ' });
                string[] words = Regex.Split(ptext, @"(?<= )");

                foreach (string w in words)
                {
                    //Run newInline = new Run(w + " ");
                    Run newInline = new Run(w);
                    BrushConverter conv = new BrushConverter();
                    string color = "#" + ("" + rand.Next()).Substring(0, 6);
                    string color2 = "#" + ("33" + rand.Next()).Substring(0, 8);
                    SolidColorBrush brush = conv.ConvertFromString(color) as SolidColorBrush;
                    SolidColorBrush brush2 = conv.ConvertFromString(color2) as SolidColorBrush;
                    newInline.Foreground = brush;
                    newInline.Background = brush2;
                    p.Inlines.Add(newInline);
                }
            }
            st.Stop();
            db.w(" elapsed " + st.Elapsed);
            rtb.Document = fd;
            rtb.CaretPosition = rtb.CaretPosition.DocumentStart;
            rtb.CaretPosition = rtb.CaretPosition.GetPositionAtOffset(pointer, LogicalDirection.Forward);
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
