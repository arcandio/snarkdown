using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snarkdown_WPF;
using System.IO;

namespace SDWPF_Testing
{
    [TestClass]
    public class SdwpfTests
    {
        /* THINGS TO TEST
         * 
         * DOCMODEL
         * checkfilepath
         * 
         * 
         * new project
         * new document
         * saving files
         * loading files
         * getting content
         * getting meta
         * editing content
         * editing meta
         * exporting
         * backing up
         * 
         * */
        
        [TestMethod]
        public void DocModel_CheckFilePathExists()
        {
            // arrange
            string file = "test.md";
            // act
            DocModel docmodel = new DocModel(file, false);
            PrivateObject obj = new PrivateObject(docmodel);
            bool fileCheck = (bool)obj.Invoke("CheckFilePath",new object[]{true});
            // assert
            Assert.AreEqual(fileCheck, true, "Did not find file");
        }
        [TestMethod]
        public void DocModel_CheckFilePathDoesNotExist()
        {
            // arrange
            string file = "gibberish.123123123";
            // act
            DocModel docmodel = new DocModel(file, false);
            PrivateObject obj = new PrivateObject(docmodel);
            bool fileCheck = (bool)obj.Invoke("CheckFilePath", new object[] { true });
            // assert
            Assert.AreEqual(fileCheck, false, "Falsely returned file");
        }
        [TestMethod]
        public void DocModel_CheckContents()
        {
            // arrange
            string file = "test.md";
            string contents;
            using (StreamReader sr = new StreamReader(file))
            {
                contents = sr.ReadToEnd();
            }
            // act
            DocModel docmodel = new DocModel(file, false);
            PrivateObject obj = new PrivateObject(docmodel);
            // assert
            Assert.AreEqual(docmodel.TextContents, contents, "Contents did not match");
        }
        [TestMethod]
        public void DocModel_CheckMeta()
        {
            // arrange
            string file = "test.md";
            string meta;
            using (StreamReader sr = new StreamReader(file+"meta"))
            {
                meta = sr.ReadToEnd();
            }
            // act
            DocModel docmodel = new DocModel(file, false);
            PrivateObject obj = new PrivateObject(docmodel);
            // assert
            Assert.AreEqual(docmodel.Meta, meta, "Contents did not match");
        }

        // end of test cases
    }
}
