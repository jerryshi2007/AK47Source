using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;

namespace MCS.Library.Test
{
	using System.Linq.Expressions;
	using System.Reflection;

	[TestClass]
	public class DynamicPropertyTest
	{
		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void SimplePropertyGetTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			string originalID = data.ID;

			Func<object, string> reader = (Func<object, string>)DynamicHelper.GetPropertyGetterDelegate(typeof(PropertyTestObject).GetProperty("ID"));

			string idRead = reader(data);

			Assert.AreEqual(originalID, idRead);
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void InterfacePropertyGetTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			IUser originalUser = data.User;

			Func<object, IUser> reader = (Func<object, IUser>)DynamicHelper.GetPropertyGetterDelegate(typeof(PropertyTestObject).GetProperty("User"));

			IUser userRead = reader(data);

			Assert.AreSame(originalUser, userRead);
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void InterfacePropertySetNullTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			var writer = (Func<object, IUser, IUser>)DynamicHelper.GetPropertySetterDelegate(typeof(PropertyTestObject).GetProperty("User"));

			writer(data, null);

			Assert.IsNull(data.User);
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void InterfacePropertySetUserTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			var writer = (Func<object, IUser, IUser>)DynamicHelper.GetPropertySetterDelegate(typeof(PropertyTestObject).GetProperty("User"));

			IUser user = TestUser.PrepareTestData();

			writer(data, user);

			Assert.AreSame(data.User, user);
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void PrivatePropertyGetTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			Func<object, int> reader = (Func<object, int>)DynamicHelper.GetPropertyGetterDelegate(typeof(PropertyTestObject).GetProperty("PrivateInt", BindingFlags.Instance | BindingFlags.NonPublic));

			int dataRead = (int)reader(data);

			Assert.AreEqual(data.GetPrivateInt(), dataRead);
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void PrivatePropertySetTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			var writer = (Func<object, int, int>)DynamicHelper.GetPropertySetterDelegate(typeof(PropertyTestObject).GetProperty("PrivateInt", BindingFlags.Instance | BindingFlags.NonPublic));

			writer(data, 2048);

			Assert.AreEqual(2048, data.GetPrivateInt());
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void PropertyGetMethodDelegateCacheTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			Func<object, object> reader1 = (Func<object, object>)DynamicHelper.GetPropertyGetterDelegate(typeof(PropertyTestObject).GetProperty("PrivateInt", BindingFlags.Instance | BindingFlags.NonPublic), typeof(object));
			Func<object, object> reader2 = (Func<object, object>)DynamicHelper.GetPropertyGetterDelegate(typeof(PropertyTestObject).GetProperty("PrivateInt", BindingFlags.Instance | BindingFlags.NonPublic), typeof(object));

			Assert.AreSame(reader1, reader2);
		}

		[TestMethod]
		[TestCategory("Dynamic Invoke")]
		public void PropertySetMethodDelegateCacheTest()
		{
			PropertyTestObject data = PropertyTestObject.PrepareTestData();

			var writer1 = (Func<object, int, int>)DynamicHelper.GetPropertySetterDelegate(typeof(PropertyTestObject).GetProperty("PrivateInt", BindingFlags.Instance | BindingFlags.NonPublic));
			var writer2 = (Func<object, int, int>)DynamicHelper.GetPropertySetterDelegate(typeof(PropertyTestObject).GetProperty("PrivateInt", BindingFlags.Instance | BindingFlags.NonPublic));

			Assert.AreSame(writer1, writer2);
		}
	}
}
