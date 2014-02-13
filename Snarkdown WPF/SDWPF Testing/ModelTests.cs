﻿using System;
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
        Random rand = new Random();

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
        [TestMethod]
        public void Model_WordCountProject ()
        {
            // arrange
            int wordCount = 860;
            float margin = 0.01f;
            SetupTestUi();
            // act
            Model.Instance.CountAllWords();
            // assert
            Assert.AreEqual(wordCount, Model.Instance.wcProj, wordCount * margin, "Project returned wrong word count");
            Assert.AreEqual(wordCount, Model.Instance.wcProj, "Project returned wrong word count");
        }
        [TestMethod]
        public void Model_LoadContent ()
        {
            // arrange
            //Random rand = new Random();
            string tempFileContents = "Test File: "+rand.Next();
            string tempFileLocation = "TestProject\\temp.md";
            string returnedContents = "";
            string rtbContents = "";
            DocModel ourTestDoc;
            // act
            using (StreamWriter sw = new StreamWriter(tempFileLocation))
            {
                sw.Write(tempFileContents);
            }
            SetupTestUi();
            ourTestDoc = Model.Instance.GetDocByFilename(tempFileLocation);
            Model.Instance.CurrentDocument = ourTestDoc;
            returnedContents = ourTestDoc.textContents;
            rtbContents = new TextRange(Model.Instance.rtb.Document.ContentStart, 
                Model.Instance.rtb.Document.ContentEnd).Text.Trim();
            // assert
            Assert.IsNotNull(ourTestDoc, "Our Test Doc was null");
            Assert.AreEqual(tempFileContents, returnedContents, "Contents were not the same as loaded content");
            Assert.AreEqual(tempFileContents, Model.Instance.Content, "Contents were not the same as Model's content");
            Assert.AreEqual(tempFileContents, rtbContents, "Contents were not the same as rich text box");
            File.Delete(tempFileLocation);
        }
        [TestMethod]
        public void Model_SaveContent()
        {
            // arrange
            Random rand = new Random();
            string tempFileContents = "Test File: " + rand.Next();
            string tempFileChanges = "Test File Changes: " + rand.Next();
            string tempFileLocation = "TestProject\\temp.md";
            string returnedContents = "";
            string returnedChanges = "";
            string rtbContents = "";
            //string rtbChanges = "";
            DocModel ourTestDoc;
            //PrivateObject mw;
            // act
            using (StreamWriter sw = new StreamWriter(tempFileLocation))
            {
                sw.Write(tempFileContents);
            }
            SetupTestUi();
            //mw = new PrivateObject(Model.Instance.mw);
            ourTestDoc = Model.Instance.GetDocByFilename(tempFileLocation);
            returnedContents = ourTestDoc.textContents;
            Model.Instance.CurrentDocument = ourTestDoc;
            returnedContents = ourTestDoc.textContents;
            rtbContents = new TextRange(Model.Instance.rtb.Document.ContentStart,
                Model.Instance.rtb.Document.ContentEnd).Text.Trim();
            //mw.Invoke("Rtb_KeyUp",new object[2]);
            Model.Instance.Refresh();
            ourTestDoc.textContents = tempFileChanges;
            ourTestDoc.Save();
            using (StreamReader sr = new StreamReader(tempFileLocation))
            {
                returnedChanges = sr.ReadToEnd();
            }

            // assert
            Assert.IsNotNull(ourTestDoc, "Our Test Doc was null");
            Assert.AreEqual(tempFileContents, returnedContents, "Contents were not the same as loaded content");
            Assert.AreEqual(tempFileChanges, Model.Instance.Content, "Contents were not the same as Model's content");
            Assert.AreEqual(tempFileContents, rtbContents, "Contents were not the same as rich text box");
            Assert.AreEqual(tempFileChanges, returnedChanges, "Saved contents were not the same");
            File.Delete(tempFileLocation);
        }
        [TestMethod]
        public void Model_FswCreated()
        {
            // arrange
            string newFilePath = "TestProject\\TestFile.md";
            string contents = "";
            int fileCount = 0;
            int newFileCount = 0;
            DocModel parent;
            DocModel child;
            DocModel parentsLastChild;
            bool containsNewChild = false;
            // act
            File.Delete(newFilePath);
            SetupTestUi();
            fileCount = Model.Instance.docModels.Count();
            contents = "# new File Text: " + Environment.NewLine + rand.Next();
            using (StreamWriter sw = new StreamWriter(newFilePath))
            {
                sw.Write(contents);
            }
            System.Threading.Thread.Sleep(100);
            newFileCount = Model.Instance.docModels.Count();
            parent = Model.Instance.GetDocByFilename("TestProject");
            child = Model.Instance.GetDocByFilename(newFilePath);
            parentsLastChild = parent.children[parent.children.Count-1];

            foreach (DocModel d in parent.children)
            {
                if (d.pathFile == newFilePath)
                    containsNewChild = true;
                db.w(d.pathFile);
            }

            // assert
            Assert.AreEqual(fileCount + 1, newFileCount, "Failed to add file to the Model's list");
            Assert.IsTrue(Model.Instance.docModels.Contains(child), "New file not found in Model List");
            Assert.IsTrue(containsNewChild, "Child not added to parent's children list");
            Assert.AreEqual(contents, child.textContents, "contents differ from expected");
            File.Delete(newFilePath);
        }
        [TestMethod]
        public void Model_FswDeleted()
        {
            // arrange
            string newFilePath = "TestProject\\TestFile.md";
            string contents = "";
            int fileCount = 0;
            int newFileCount = 0;
            int parentCount = 0;
            int newParentCount = 0;
            DocModel parent;
            // act
            contents = "# new File Text: " + Environment.NewLine + rand.Next();
            using (StreamWriter sw = new StreamWriter(newFilePath))
            {
                sw.Write(contents);
            }
            System.Threading.Thread.Sleep(100);
            SetupTestUi();
            fileCount = Model.Instance.docModels.Count();
            parent = Model.Instance.GetDocByFilename("TestProject");
            parentCount = parent.children.Count();

            File.Delete(newFilePath);
            System.Threading.Thread.Sleep(100);
            newFileCount = Model.Instance.docModels.Count();
            newParentCount = parent.children.Count();
            
            // assert
            Assert.AreEqual(fileCount - 1, newFileCount, "Failed to remove file to the Model's list");
            Assert.AreEqual(parentCount - 1, newParentCount, "Failed to remove file to the Model's list");
        }

        string testProjectPath = "TestProject\\project.md";
        public void SetupTestUi ()
        {
            // arrange
            string projectPath = testProjectPath;
            MainWindow mw = new MainWindow(true);
            RichTextBox rtb = new RichTextBox();
            RichTextBox rtbmeta = new RichTextBox();
            Grid grid1 = new Grid();

            // act
            //rtb.Name = "rtb";
            mw.Content = grid1;
            //grid1.Children.Add(rtb);
            Model.Instance.rtb = rtb;
            Model.Instance.rtbmeta = rtbmeta;
            Model.Instance.mw = mw;
            Model.Instance.LoadProject(projectPath);
            Model.Instance.autoMode = true;
            Model.Instance.alwaysAcceptFsChange = true;

            Assert.IsNotNull(Model.Instance, "Model Instance is Null");
            Assert.IsNotNull(Model.Instance.ProjectPath, "Project path is Null");
            Assert.IsNotNull(Model.Instance.RootObject, "Root object is Null");
        }

    }
}
