using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snarkdown_WPF;

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
    public class ModelTests
    {
        [TestMethod]
        public void Model_LoadProject()
        {
            SetupTestUi();
            // assert
            Assert.IsNotNull(Model.Instance, "Model Instance is Null");
            Assert.IsNotNull(Model.Instance.ProjectPath, "Project path is Null");
            Assert.IsNotNull(Model.Instance.RootObject, "Root object is Null");
        }
        [TestMethod]
        public void Model_CorrectNumberOfDocuments ()
        {
            SetupTestUi();
            // act
            int numberOfFiles = 6;
            // assert
            Assert.AreEqual(numberOfFiles, Model.Instance.DocModels.Count, "Wrong number of files");
        }

        public void SetupTestUi ()
        {
            // arrange
            string projectPath = "TestProject\\project.md";
            MainWindow mw = new MainWindow(true);
            RichTextBox rtb = new RichTextBox();
            Grid grid1 = new Grid();

            // act
            rtb.Name = "rtb";
            mw.Content = grid1;
            grid1.Children.Add(rtb);
            Model.Instance.rtb = rtb;
            Model.Instance.mw = mw;
            Model.Instance.LoadProject(projectPath);
        }

    }
}
