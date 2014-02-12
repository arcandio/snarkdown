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
using System.Windows;

namespace GfmSyntax
{
    public class GfmSyntaxProvider
    {
        // http://msdn.microsoft.com/en-us/library/system.windows.documents.flowdocument(v=vs.110).aspx
        // http://msdn.microsoft.com/en-us/library/system.windows.documents.textrange(v=vs.110).aspx
        // http://msdn.microsoft.com/en-us/library/system.windows.documents.inline(v=vs.110).aspx
        // http://msdn.microsoft.com/en-us/library/system.windows.documents.block(v=vs.110).aspx
        // http://msdn.microsoft.com/en-us/library/system.windows.documents.paragraph(v=vs.110).aspx
        // http://msdn.microsoft.com/en-us/library/system.windows.controls.richtextbox.caretposition(v=vs.110).aspx
        // http://msdn.microsoft.com/en-us/library/system.windows.documents.textpointer.getinsertionposition(v=vs.110).aspx
        // http://msdn.microsoft.com/en-us/library/ms745683(v=vs.110).aspx
        // http://msdn.microsoft.com/en-us/library/system.windows.style(v=vs.110).aspx
        // http://msdn.microsoft.com/en-us/library/system.text.regularexpressions.match(v=vs.110).aspx

        GfmColorTheme theme;
        List<string> parsCache;
         
        public GfmSyntaxProvider()
        {
            theme = new GfmColorTheme();
        }
        /*
        public void CheckAllBlocks()
        {
            if (Model.Instance.rtb.Document == null)
            {
                Model.Instance.rtb.Document = new FlowDocument();
            }
            CheckAllBlocks(Model.Instance.rtb.Document, Model.Instance.rtb);
        }*/
        public void CheckAllBlocks(RichTextBox rtb)
        {
            FlowDocument fd = rtb.Document;
            Stopwatch st = Stopwatch.StartNew();
            TextPointer tp = rtb.CaretPosition;
            int pointer = rtb.CaretPosition.GetOffsetToPosition(rtb.CaretPosition.DocumentStart) * -1;
            LogicalDirection dir = rtb.CaretPosition.LogicalDirection;
            TextRange cursor = new TextRange(rtb.Document.ContentStart, rtb.CaretPosition);
            // replace rtb? http://msdn.microsoft.com/en-us/library/system.windows.controls.flowdocumentscrollviewer(v=vs.110).aspx
            // http://stackoverflow.com/questions/2068120/c-sharp-cast-entire-array
            Paragraph[] pars = Array.ConvertAll<Block,Paragraph>(fd.Blocks.ToArray<Block>(), item => (Paragraph)item);
            List<string> tempCache = new List<string>();

            for (int r = 0; r < pars.Length; r++)
            {
                // get current paragraph and current paragraph text
                Paragraph p = pars[r];
                string ptext = new TextRange(p.ContentStart, p.ContentEnd).Text;
                // check to see if our paragraphtext is unchanged, and should we skip parsing
                if (parsCache != null && r < parsCache.Count && ptext == parsCache[r])
                {
                    continue;
                }
                tempCache.Add(ptext);
                TextRange tr = new TextRange(p.ContentStart, p.ContentEnd);
                tr.ClearAllProperties();
                foreach (GfmSyntaxRule rule in theme.rules)
                {
                    if (rule.isParagraph != true)
                    {
                        rule.matches = Regex.Matches(ptext, rule.regex);
                        rule.markups = new TextRange[rule.matches.Count];

                        for (int i = 0; i < rule.matches.Count; i++)
                        {
                            Match match = rule.matches[i];

                            TextPointer start = tr.Start.GetPositionAtOffset(match.Captures[0].Index);
                            TextPointer end = tr.Start.GetPositionAtOffset(match.Captures[0].Index + match.Captures[0].Length);

                            TextRange matchRange = new TextRange(start, end);
                            rule.markups[i] = matchRange;
                        }
                    }
                }
                foreach (GfmSyntaxRule rule in theme.rules)
                {
                    if (rule.isParagraph != true)
                    {
                        for (int i = 0; i < rule.matches.Count; i++)
                        {
                            SolidColorBrush myColor = new SolidColorBrush(rule.color);
                            SolidColorBrush myColor2 = new SolidColorBrush(rule.color);
                            myColor2.Opacity = .5f;
                            rule.markups[i].ApplyPropertyValue(TextElement.BackgroundProperty, myColor2);
                            //rule.markups[i].ApplyPropertyValue(TextElement.ForegroundProperty, myColor);
                            //rule.markups[i].ApplyPropertyValue(TextElement.FontWeightProperty, myColor);
                            //rule.markups[i].ApplyPropertyValue(TextElement., "markup");
                        }
                    }
                }
                // done parsing styles INSIDE paragraphs
            }
            // place our newly constructed paragraph cache
            parsCache = tempCache;
            // reset our document, just to be sure it updates in the view
            rtb.Document = fd;
            // restore our caret position
            rtb.CaretPosition = tp;
            // stop stopwatch for performance check
            st.Stop();
            //db.w(" elapsed " + st.Elapsed);
        }
        
    }
    internal class GfmColorTheme
    {
        
        internal Color appBg = Color.FromArgb(255, 127, 127, 127);
        internal Color textBg = Color.FromArgb(127, 255, 255, 255);

        internal List<GfmSyntaxRule> rules = new List<GfmSyntaxRule>();

        internal GfmColorTheme ()
        {
            // emphasis/italic
            rules.Add(new GfmSyntaxRule(@"((?<=\s)(\*[\w ]+?\*))|((?<=\s)(_[\w ]+?_))", Colors.Cyan, false));
            //rules.Add(new GfmSyntaxRule(@"(?<=\s)(_[\w ]+_)",  Colors.Cyan));
            // strong/bold
            rules.Add(new GfmSyntaxRule(@"((?<=\s)(\*\*[\w ]+?\*\*))|((?<=\s)(__[\w ]+?__))",  Colors.Blue, false));
            //rules.Add(new GfmSyntaxRule(@"(?<=\s)(__[\w ]+__)",  Colors.Blue));
            // monospace
            rules.Add(new GfmSyntaxRule(@"(?<=\s)(`[\w ]+?`)",  Colors.Green, false));
            // strike
            rules.Add(new GfmSyntaxRule(@"(?<=\s)(~~[\w ]+?~~)",  Colors.Red, false));
            // comment
            rules.Add(new GfmSyntaxRule(@"(<!--.+?-->)",  Colors.Green, false));
            // header
            rules.Add(new GfmSyntaxRule(@"((?<=^{2,})#+.+)",  Colors.Orange, false));
            //rules.Add(new GfmSyntaxRule(@"(.+[\r\r\n\n][=-]+[\r\r\n\n])|((?<=[\r\r\n\n]{2,})#+.+)",  Colors.Orange));
            // math
            rules.Add(new GfmSyntaxRule(@"(?<=\s)(\$.+?\$)",  Colors.Red, false));
            // link
            // http://blog.mattheworiordan.com/post/13174566389/url-regular-expression-for-links-with-or-without-the
            rules.Add(new GfmSyntaxRule(@"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?)",  Colors.Blue, false));

            // header
            rules.Add(new GfmSyntaxRule(@"(?m)(^.+$)\W(^[=-]+$)",  Colors.Orange, false));
            // list
            rules.Add(new GfmSyntaxRule(@"((?<=[\r\r\n\n])[ \t]+.*)+([\r\r\n\n][ \t]+.*)*",  Colors.Red, true));
            // block quote
            rules.Add(new GfmSyntaxRule(@"((?<=[\r\r\n\n])>.*)([\r\r\n\n]>.*)*",  Colors.Red, true));
            // code block
            rules.Add(new GfmSyntaxRule(@"(?<=[\r\r\n\n])((```[\d\D]+?```)|(~~~[\d\D]+?~~~))",  Colors.Red, true));
            // table
            rules.Add(new GfmSyntaxRule(@"((?<=[\r\r\n\n]).+)([\r\r\n\n][-]+[- ]*)([\r\r\n\n].+)*",  Colors.Red, true));
        }
    }
    internal class GfmSyntaxRule
    {
        internal string regex;
        //internal Style style;
        internal Color color;
        internal MatchCollection matches;
        internal TextRange[] markups;
        internal bool isParagraph;

        //internal GfmSyntaxRule() { }
        internal GfmSyntaxRule (string reg, /*Style sty,*/ Color col, bool isPara)
        {
            regex = reg;
            //style = sty;
            color = col;
            isParagraph = isPara;
        }
    }
}
