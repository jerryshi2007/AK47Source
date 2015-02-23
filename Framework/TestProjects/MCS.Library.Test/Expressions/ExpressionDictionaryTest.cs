using MCS.Library.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.Test.Expressions
{
    [TestClass]
    public class ExpressionDictionaryTest
    {
        [TestMethod]
        public void ExpressionDictionaryInfoTest()
        {
            ExpressionDictionaryCollection dictionaryInfo = new ExpressionDictionaryCollection();

            dictionaryInfo.InitFromConfiguration(ExpressionDictionarySettings.GetConfig());

            Assert.IsTrue(dictionaryInfo.Count > 0);
            Assert.IsNotNull(dictionaryInfo["Users"]);
            Assert.IsTrue(dictionaryInfo["Users"].Items.Count > 0);

            Assert.AreEqual("Name", dictionaryInfo["Users"].Items["Name"].Name);
            Assert.AreEqual("年龄", dictionaryInfo["Users"].Items["Age"].Description);
            Assert.AreEqual(ExpressionDataType.DateTime, dictionaryInfo["Users"].Items["Birthday"].DataType);
        }

        [TestMethod]
        public void CalculateUserDictionaryTest()
        {
            object age = ExpressionParser.Calculate("Users(\"Age\")");

            Console.WriteLine(age);

            Assert.AreEqual(42, age);
        }
    }
}
