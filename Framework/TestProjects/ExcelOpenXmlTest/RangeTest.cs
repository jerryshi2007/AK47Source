using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Office.OpenXml.Excel;

namespace ExcelOpenXmlTest
{
    [TestClass]
    public class RangeTest
    {
        [TestMethod]
        public void IsSubsetTest()
        {
            Range containerRange = Range.Parse("$A$1:$K$5");
            Range subsetRange = Range.Parse("$B$1:$E$4");

            Assert.IsTrue(subsetRange.IsSubset(containerRange));
        }

        [TestMethod]
        public void IsSubsetAndEqualTest()
        {
            Range containerRange = Range.Parse("$A$1:$K$5");
            Range subsetRange = Range.Parse("$B$1:$E$4");
            Range sameRange = Range.Parse("$B$1:$E$4");

            Assert.IsTrue(subsetRange.IsSubsetOrEqual(containerRange));
            Assert.IsTrue(subsetRange.IsSubsetOrEqual(sameRange));
        }

        [TestMethod]
        public void ContainerIsInvalidSubsetTest()
        {
            Range containerRange = new Range();
            Range subsetRange = Range.Parse("$B$1:$E$4");

            Assert.IsFalse(subsetRange.IsSubset(containerRange));
        }

        [TestMethod]
        public void SubsetIsInvalidSubsetTest()
        {
            Range containerRange = Range.Parse("$A$1:$K$5");
            Range subsetRange = new Range();

            Assert.IsFalse(subsetRange.IsSubset(containerRange));
        }
    }
}
