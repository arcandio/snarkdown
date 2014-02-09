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

        GfmColorTheme theme;
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
         
        public GfmSyntaxProvider()
        {
            theme = new GfmColorTheme();
        }
        public void CheckAllBlocks()
        {
            if (Model.Instance.mw.rtb.Document == null)
            {
                Model.Instance.mw.rtb.Document = new FlowDocument();
            }
            CheckAllBlocks(Model.Instance.mw.rtb.Document, Model.Instance.mw.rtb);
        }
        public void CheckAllBlocks(FlowDocument fd, RichTextBox rtb)
        {
            Stopwatch st = Stopwatch.StartNew();
            int pointer = rtb.CaretPosition.GetOffsetToPosition(rtb.CaretPosition.DocumentStart) * -1;
            LogicalDirection dir = rtb.CaretPosition.LogicalDirection;
            TextRange cursor = new TextRange(rtb.Document.ContentStart, rtb.CaretPosition);
            // replace rtb? http://msdn.microsoft.com/en-us/library/system.windows.controls.flowdocumentscrollviewer(v=vs.110).aspx
            // http://stackoverflow.com/questions/2068120/c-sharp-cast-entire-array
            Paragraph[] pars = Array.ConvertAll<Block,Paragraph>(fd.Blocks.ToArray<Block>(), item => (Paragraph)item);
            
            foreach (Paragraph p in pars)
            {
                string ptext = new TextRange(p.ContentStart, p.ContentEnd).Text;
                TextRange tr = new TextRange(p.ContentStart, p.ContentEnd);
                tr.ClearAllProperties();
                foreach (GfmSyntaxRule rule in theme.rules)
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
                foreach (GfmSyntaxRule rule in theme.rules)
                {

                    for (int i = 0; i < rule.matches.Count; i++)
                    {

                        rule.markups[i].ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(rule.color));
                    }
                }
            }
            
            st.Stop();
            db.w(" elapsed " + st.Elapsed);
            rtb.Document = fd;

            rtb.CaretPosition = rtb.CaretPosition.DocumentStart;
            pointer = cursor.End.GetOffsetToPosition(rtb.Document.ContentStart) * -1;
            rtb.CaretPosition = rtb.CaretPosition.GetPositionAtOffset(pointer, dir);
        }
        
    }
    internal class GfmColorTheme
    {
        
        internal Color appBg = Color.FromArgb(255, 127, 127, 127);
        internal Color textBg = Color.FromArgb(127, 255, 255, 255);

        internal List<GfmSyntaxRule> rules = new List<GfmSyntaxRule>();

        internal GfmColorTheme ()
        {

            rules.Add(new GfmSyntaxRule(@" (\*[\w\s]+\*)", new Style(), Colors.Red));
            rules.Add(new GfmSyntaxRule(@" (_\w+_)", new Style(), Colors.Blue));
            rules.Add(new GfmSyntaxRule(@" (\*\*\w+\*\*)", new Style(), Colors.Green));
        }
    }
    internal class GfmSyntaxRule
    {
        internal string regex;
        internal Style style;
        internal Color color;
        internal MatchCollection matches;
        internal TextRange[] markups;

        internal GfmSyntaxRule() { }
        internal GfmSyntaxRule (string reg, Style sty, Color col)
        {
            regex = reg;
            style = sty;
            color = col;
        }
    }
}
