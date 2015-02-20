using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using MCS.Library.Expression;

namespace MCS.Library.Test
{
    [TestClass]
    public class ExpressionTest
    {
        [TestMethod]
        public void EnumBuiltInFunctionInfoTest()
        {
            BuiltInFunctionInfoCollection funcsInfo = BuiltInFunctionHelper.GetBuiltInFunctionsInfo(typeof(BuiltInFuncWrapper));

            funcsInfo.ForEach(f => Console.WriteLine(f.FunctionName));

            Assert.IsTrue(funcsInfo.Contains("add"));
            Assert.IsTrue(funcsInfo.Contains("sub"));
        }

        [TestMethod]
        public void CalculateAddFunctionTest()
        {
            object result = ExpressionParser.Calculate("Add(3, 4)", BuiltInFuncWrapper.Instance, "context");

            Console.WriteLine(result);

            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void CalculateCombineFunctionTest()
        {
            object result = ExpressionParser.Calculate("Combine(\"Hello \")", BuiltInFuncWrapper.Instance, "World");

            Console.WriteLine(result);

            Assert.AreEqual("Hello World", result);
        }

        [TestMethod]
        public void CalculateTypeNameFunctionTest()
        {
            object result = ExpressionParser.Calculate("TypeName", BuiltInFuncWrapper.Instance, "World");

            Console.WriteLine(result);

            Assert.AreEqual(typeof(BuiltInFuncWrapper).FullName, result);
        }

        [TestMethod]
        public void CalculateStaticAddFunctionTest()
        {
            object result = ExpressionParser.Calculate("StaticAdd(3, 4)", BuiltInFuncWrapper.Instance, null);

            Console.WriteLine(result);

            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void CalculateGenderValueFunctionTest()
        {
            object result = ExpressionParser.Calculate("GenderValue(\"Female\")", BuiltInFuncWrapper.Instance, null);

            Console.WriteLine(result);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void CalculateGenderNameFunctionTest()
        {
            object result = ExpressionParser.Calculate("GenderName(2)", BuiltInFuncWrapper.Instance, null);

            Console.WriteLine(result);

            Assert.AreEqual("Female", result);
        }
    }
}
