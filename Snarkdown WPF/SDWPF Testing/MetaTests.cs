using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snarkdown_WPF;

namespace SDWPF_Testing
{
    [TestClass]
    public class MetaParsing
    {
        [TestMethod]
        public void Meta_ParseTags()
        {
            // arrange
            MetaContainer mc = new MetaContainer();
            DateTime due = new DateTime(2014, 1, 1);
            // act
            mc.ParseTagsFromString(testData);
            // assert
            Assert.AreEqual("one", mc.Characters);
            Assert.AreEqual("two", mc.Locations);
            Assert.AreEqual("three", mc.Progress);
            Assert.AreEqual("four", mc.Tags);
            Assert.AreEqual(5, mc.DocWords);
            Assert.AreEqual(6, mc.DocTarget);
            Assert.AreEqual("seven", mc.Synopsis);
            Assert.AreEqual("eight", mc.Title);
            Assert.AreEqual("nine", mc.Author);
            Assert.AreEqual(10, mc.ProjectTarget);
            Assert.AreEqual(11, mc.ProjectWords);
            Assert.AreEqual(12, mc.Daily);
            Assert.AreEqual(13, mc.DailyTarget);
            Assert.AreEqual(due, mc.DueDate);
            Assert.AreEqual(15, mc.Pace);
        }

        [TestMethod]
        public void Meta_RebuildString()
        {
            // arrange
            MetaContainer mc = new MetaContainer();
            DateTime due = new DateTime(2014, 1, 1);
            // act
            mc.ParseTagsFromString(testData);
            // assert

        }

        string testData = " * Characters: one\n * Locations: two\n * Progress: three\n * Tags: four\n * DocWords: 5\n * DocTarget: 6\n * Synopsis: seven\n * Title: eight\n * Author: nine\n * ProjectTarget: 10\n * ProjectWords: 11\n * Daily: 12\n * DailyTarget: 13\n * DueDate: 1/1/2014\n * Pace: 15\nleftovers";
    }
}
