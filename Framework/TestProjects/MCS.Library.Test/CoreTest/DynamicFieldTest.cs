using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.Test
{
	[TestClass]
	public class DynamicFieldTest
	{
		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void SimpleFieldGetTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			string originalValue = data.PublicField;

			Func<object, string> reader = (Func<object, string>)DynamicHelper.GetFiledGetterDelegate(typeof(PropertyTestObject).GetField("PublicField"));

			string idRead = reader(data);

			Assert.AreEqual(originalValue, idRead);
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void SimpleFieldSetTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			Func<object, string, string> writer = (Func<object, string, string>)DynamicHelper.GetFieldSetterDelegate(typeof(PropertyTestObject).GetField("PublicField"));

			writer(data, "新字段值");

			Assert.AreEqual("新字段值", data.PublicField);
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void FieldGetMethodDelegateCacheTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			Func<object, object> reader1 = (Func<object, object>)DynamicHelper.GetFiledGetterDelegate(typeof(PropertyTestObject).GetField("PublicField"), typeof(object));
			Func<object, object> reader2 = (Func<object, object>)DynamicHelper.GetFiledGetterDelegate(typeof(PropertyTestObject).GetField("PublicField"), typeof(object));

			Assert.AreSame(reader1, reader2);
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void FieldSetMethodDelegateCacheTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			Func<object, string, string> writer1 = (Func<object, string, string>)DynamicHelper.GetFieldSetterDelegate(typeof(PropertyTestObject).GetField("PublicField"));
			Func<object, string, string> writer2 = (Func<object, string, string>)DynamicHelper.GetFieldSetterDelegate(typeof(PropertyTestObject).GetField("PublicField"));

			Assert.AreSame(writer1, writer2);
		}
	}
}
