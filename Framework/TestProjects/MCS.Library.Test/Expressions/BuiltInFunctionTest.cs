using System;
using System.Collections.Generic;
using MCS.Library.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.Test.Expressions
{
    /// <summary>
    /// 表达式引擎内置的函数测试
    /// </summary>
    [TestClass]
    public class BuiltInFunctionTest
    {
        [TestMethod]
        public void SimpleInFunctionTest()
        {
            Assert.IsTrue((bool)ExpressionParser.Calculate("IN(3, 1, 3, 5, 7, 9)"));
        }

        [TestMethod]
        public void SimpleNotInFunctionTest()
        {
            Assert.IsFalse((bool)ExpressionParser.Calculate("IN(6, 1, 3, 5, 7, 9)"));
        }

        [TestMethod]
        [Description("测试In函数，匹配第一个自定义函数，不会执行第二个函数")]
        public void InFunctionMatchFirstTest()
        {
            List<int> list = new List<int>();

            Assert.IsTrue((bool)ExpressionParser.Calculate("IN(1, data1, data2, 5, 7, 9)",
                InternalCalculateFunction, list));

            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        [Description("测试In函数，匹配第二个自定义函数，data1和data2两个自定义函数都会执行")]
        public void InFunctionMatchSecondTest()
        {
            List<int> list = new List<int>();

            Assert.IsTrue((bool)ExpressionParser.Calculate("IN(2, data1, data2, 5, 7, 9)",
                InternalCalculateFunction, list));

            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        [Description("测试IIF函数，当条件满足时，返回第二个参数，不会执行第三个参数")]
        public void IIFMatchFirstTest()
        {
            List<int> list = new List<int>();

            Assert.AreEqual(1, ExpressionParser.Calculate("IIF(3 < 4, data1, data2)",
                InternalCalculateFunction, list));

            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        [Description("测试IIF函数，当条件不满足时，返回第三个参数，不会执行第二个参数")]
        public void IIFMatchSecondTest()
        {
            List<int> list = new List<int>();

            Assert.AreEqual(2, ExpressionParser.Calculate("IIF(3 > 4, data1, data2)",
                InternalCalculateFunction, list));

            Assert.AreEqual(1, list.Count);
        }

        private static object InternalCalculateFunction(string funcName, ParamObjectCollection arrParams, object callerContext)
        {
            List<int> list = (List<int>)callerContext;

            object result = false;

            switch (funcName.ToLower())
            {
                case "data1":
                    result = 1;
                    list.Add(1);
                    break;
                case "data2":
                    result = 2;
                    list.Add(2);
                    break;
            }

            return result;
        }
    }
}
