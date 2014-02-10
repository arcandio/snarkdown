using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GfmSyntax;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Windows.Markup;

namespace SDWPF_Testing
{
    [TestClass]
    public class GfmTests
    {
        [TestMethod]
        public void GfmSyntax__CheckAllBlocks()
        {
            // arrange
            RichTextBox rtb = new RichTextBox();
            using (StreamReader sr = new StreamReader("test.md"))
            {
                rtb.AppendText( sr.ReadToEnd());
            }
            GfmSyntaxProvider gfm = new GfmSyntaxProvider();
            // act
            gfm.CheckAllBlocks(rtb.Document, rtb);
            // assert

        }
        [TestMethod]
        public void GfmSyntax_ItalicAsterisk()
        {
            CheckSyntaxHighlighting(" *italic*", "Background");
        }
        [TestMethod]
        public void GfmSyntax_ItalicUnderscore()
        {
            CheckSyntaxHighlighting(" _italic_", "Background");
        }
        [TestMethod]
        public void GfmSyntax_BoldAsterisk()
        {
            CheckSyntaxHighlighting(" **bold**", "Background");
        }
        [TestMethod]
        public void GfmSyntax_BoldUnderline()
        {
            CheckSyntaxHighlighting(" __bold__", "Background");
        }
        [TestMethod]
        public void GfmSyntax_Strikeout()
        {
            CheckSyntaxHighlighting(" ~~strike~~", "Background");
        }
        [TestMethod]
        public void GfmSyntax_Monospace()
        {
            CheckSyntaxHighlighting(" `monospace`", "Background");
        }
        [TestMethod]
        public void GfmSyntax_InlineHeading()
        {
            CheckSyntaxHighlighting("# A Heading", "Background");
        }
        [TestMethod]
        public void GfmSyntax_Link()
        {
            CheckSyntaxHighlighting(" http://voidspiral.com", "Background");
        }
        [TestMethod]
        public void GfmSyntax_InlineComment()
        {
            CheckSyntaxHighlighting(" <!-- comment -->", "Background");
        }

        [TestMethod]
        public void GfmSyntax_MultilineHeader()
        {
            CheckSyntaxHighlighting("A Heading\n\r======", "Background");
        }

        public void CheckSyntaxHighlighting (string example, string check)
        {
            // arrange
            RichTextBox rtb = new RichTextBox();
            RichTextBox rtb2 = new RichTextBox();
            string content = example;
            GfmSyntaxProvider gfm = new GfmSyntaxProvider();
            string text;
            string text2;

            // act
            rtb.AppendText(content);
            rtb2.AppendText(content);
            gfm.CheckAllBlocks(rtb.Document, rtb);
            text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
            text2 = new TextRange(rtb2.Document.ContentStart, rtb2.Document.ContentEnd).Text;
            string serializedOutput = XamlWriter.Save(rtb.Document);

            // assert
            Assert.AreEqual(text2, text, "Didn't transport text correctly");
            Assert.IsTrue(serializedOutput.Contains(check),"Did not contain check: "+check);
        }

        // end of test methods
    }
}
