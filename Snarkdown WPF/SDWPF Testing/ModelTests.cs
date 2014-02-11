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
        string testProjectPath = "TestProject\\project.md";
        [TestMethod]
        public void Model_LoadProject()
        {
            SetupTestUi();

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
        [TestMethod]
        public void Model_NewProject ()
        {
            // arrange
            string newProjectFileContents = "";
            // act
            if (File.Exists(testProjectPath) == true)
                File.Delete(testProjectPath);
            Assert.IsFalse(File.Exists(testProjectPath), "Failed to delete project file");
            Model.Instance.NewProject(testProjectPath);
            using (StreamReader sr = new StreamReader(testProjectPath))
            {
                newProjectFileContents = sr.ReadToEnd();
            }
            // assert
            Assert.IsTrue(File.Exists(testProjectPath), "Failed to create new project file" );
            Assert.IsTrue(newProjectFileContents.ToLower().Contains("project"),"Did not place text in new project file");
            Assert.IsNotNull(Model.Instance, "Model Instance is Null");
            Assert.IsNotNull(Model.Instance.ProjectPath, "Project path is Null");
            Assert.IsNotNull(Model.Instance.RootObject, "Root object is Null");
        }
        [TestMethod]
        public void Model_ExportProject()
        {
            // arrange
            string outputContents = "";
            SetupTestUi();
            Model.Instance.exportPath = "TestProject\\test.html";

            // act
            Model.Instance.ExportProject();
            // assert
            using (StreamReader sr = new StreamReader(Model.Instance.exportPath))
            {
                outputContents = sr.ReadToEnd();
            }
            Assert.IsTrue(File.Exists(Model.Instance.exportPath), "Export file does not exist");
            Assert.IsTrue(outputContents.Length > 0, "Export file blank");
        }
        [TestMethod]
        public void Model_BackupProject()
        {
            // arrange
            SetupTestUi();
            string backupPath = "";
            long size = 0;
            // act
            backupPath = Model.Instance.BackupProject();
            size = new FileInfo(backupPath).Length;
            // assert
            
            Assert.IsTrue(File.Exists(backupPath), "Did not create backup");
            Assert.IsTrue(size > 30, "Zip file is blank");
            File.Delete(backupPath);
        }

        public void SetupTestUi ()
        {
            // arrange
            string projectPath = testProjectPath;
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

            Assert.IsNotNull(Model.Instance, "Model Instance is Null");
            Assert.IsNotNull(Model.Instance.ProjectPath, "Project path is Null");
            Assert.IsNotNull(Model.Instance.RootObject, "Root object is Null");
        }

    }
}
