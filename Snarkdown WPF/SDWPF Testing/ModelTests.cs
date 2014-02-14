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
        Random rand = new Random();

        [TestMethod]
        public void Model_LoadProject()
        {
            SetupTestUi();

        }
        [TestMethod]
        public void Model_CorrectNumberOfDocuments ()
        {
            // arrange
            int numberOfFiles = 8;
            int actual = 0;
            // act
            Wait();
            SetupTestUi();
            Wait();
            actual = Model.Instance.DocModels.Count;
            // assert
            Assert.AreEqual(numberOfFiles, actual, "Wrong number of files");
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

            string outputContents = ""+rand.Next();
            SetupTestUi();
            Model.Instance.exportPath = "TestProject\\testExport.html";
            File.Delete(Model.Instance.exportPath);
            // act
            Model.Instance.ExportProject();
            // assert
            using (StreamReader sr = new StreamReader(Model.Instance.exportPath))
            {
                outputContents = sr.ReadToEnd();

            }
            Assert.IsTrue(File.Exists(Model.Instance.exportPath), "Export file does not exist");
            File.Delete(Model.Instance.exportPath);
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
            int countedManually = 0;
            SetupTestUi();
            foreach (DocModel d in Model.Instance.docModels)
            {
                if (d.Type == TreeItemType.Text)
                    countedManually += d.wordCount;
            }
            // act
            Model.Instance.CountAllWords();
            // assert
            //Assert.AreEqual(countedManually, Model.Instance.wcProj, wordCount * margin, "Project returned wrong word count");
            Assert.AreEqual(countedManually, Model.Instance.wcProj, "Project returned wrong word count");
        }
        [TestMethod]
        public void Model_LoadContent ()
        {
            // arrange
            //Random rand = new Random();
            string tempFileContents = "Test File: "+rand.Next();
            string tempFileLocation = "TestProject\\tempLoad.md";
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
            File.Delete(tempFileLocation);
            Assert.IsNotNull(ourTestDoc, "Our Test Doc was null");
            Assert.AreEqual(tempFileContents, returnedContents, "Contents were not the same as loaded content");
            Assert.AreEqual(tempFileContents, Model.Instance.Content, "Contents were not the same as Model's content");
            Assert.AreEqual(tempFileContents, rtbContents, "Contents were not the same as rich text box");
            
        }
        [TestMethod]
        public void Model_SaveContent()
        {
            Wait();
            // arrange
            string tempFileContents = "Test File: " + rand.Next();
            string tempFileChanges = "Test File Changes: " + rand.Next();
            string tempFileLocation = "TestProject\\tempSave.md";
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
            Wait();
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
            Wait();
            FileStream fs = DirectoryHelper.GetStream(tempFileLocation, 20);
            using (StreamReader sr = new StreamReader(fs))
            {
                returnedChanges = sr.ReadToEnd();

            }

            // assert
            File.Delete(tempFileLocation);
            Assert.IsNotNull(ourTestDoc, "Our Test Doc was null");
            Assert.AreEqual(tempFileContents, returnedContents, "Contents were not the same as loaded content");
            Assert.AreEqual(tempFileChanges, Model.Instance.Content, "Contents were not the same as Model's content");
            Assert.AreEqual(tempFileContents, rtbContents, "Contents were not the same as rich text box");
            Assert.AreEqual(tempFileChanges, returnedChanges, "Saved contents were not the same");
            
        }
        [TestMethod]
        public void Model_FswCreated()
        {
            // arrange
            string newFilePath = "TestProject\\TestFileCreated.md";
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
            Wait();
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
            Wait();
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
            string newFilePath = "TestProject\\TestFileDeleted.md";
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
            Wait();
            SetupTestUi();
            fileCount = Model.Instance.docModels.Count();
            parent = Model.Instance.GetDocByFilename("TestProject");
            parentCount = parent.children.Count();

            File.Delete(newFilePath);
            Wait();
            newFileCount = Model.Instance.docModels.Count();
            newParentCount = parent.children.Count();
            
            // assert
            Assert.AreEqual(fileCount - 1, newFileCount, "Failed to remove file to the Model's list");
            Assert.AreEqual(parentCount - 1, newParentCount, "Failed to remove file to the Model's list");
        }
        [TestMethod]
        public void Model_MetaChanged()
        {
            // assemble
            string filePath = "TestProject\\MetaTestFileChanged.md";
            string metaContents = "meta contents: " + rand.Next();
            string metaReturned ="";
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write("testfile");
            }
            using (StreamWriter sw = new StreamWriter(filePath+"meta"))
            {
                sw.Write("");
            }
            Wait();
            // act
            SetupTestUi();
            using (StreamWriter sw = new StreamWriter(filePath + "meta"))
            {
                sw.Write(metaContents);
            }
            Wait();
            metaReturned = Model.Instance.GetDocByFilename(filePath).Meta;
            Wait();
            File.Delete(filePath);
            File.Delete(filePath + "meta");
            Wait();
            // assert
            Assert.AreEqual(metaContents, metaReturned, "Contents not reloaded");
        }
        [TestMethod]
        public void Model_ContentChanged()
        {
            // assemble
            string filePath = "TestProject\\MetaTestFileChanged.md";
            string newContents = "meta contents: " + rand.Next();
            string contentsReturned = "";
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write("testfile");
            }
            Wait();
            // act
            SetupTestUi();
            using (StreamWriter sw = new StreamWriter(filePath + "meta"))
            {
                sw.Write(newContents);
            }
            Wait();
            contentsReturned = Model.Instance.GetDocByFilename(filePath).Meta;
            Wait();
            File.Delete(filePath);
            File.Delete(filePath + "meta");
            Wait();
            // assert
            Assert.AreEqual(newContents, contentsReturned, "Contents not reloaded");
        }
        /*
        [TestCleanup]
        private void Cleanup()
        {
            //System.Threading.Thread.Sleep(1000);
            db.w("cleaned");
        }
        */
        private void Wait()
        {
            System.Threading.Thread.Sleep(100);
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
