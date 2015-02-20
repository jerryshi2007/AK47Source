using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MCS.Library.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.Test
{
	[TestClass]
	public class DynamicMethodTest
	{
		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void InvokePublicMethodTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			Func<object, int, int, int> func = (Func<object, int, int, int>)DynamicHelper.GetMethodInvokeDelegate(data.GetType().GetMethod("Add"));

			int result = func(data, 3, 4);

			Assert.AreEqual(7, result);
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void MethodDelegateCacheTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			Func<object, int, int, int> func1 = (Func<object, int, int, int>)DynamicHelper.GetMethodInvokeDelegate(data.GetType().GetMethod("Add"));

			Func<object, int, int, int> func2 = (Func<object, int, int, int>)DynamicHelper.GetMethodInvokeDelegate(data.GetType().GetMethod("Add"));

			Assert.AreSame(func1, func2);
		}
	}
}
