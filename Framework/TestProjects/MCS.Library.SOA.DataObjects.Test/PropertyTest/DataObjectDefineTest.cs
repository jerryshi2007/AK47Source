using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test
{
	/// <summary>
	/// Summary description for DataObjectDefineTest
	/// </summary>
	[TestClass]
	public class DataObjectDefineTest
	{
		public DataObjectDefineTest()
		{
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		[TestCategory(ProcessTestHelper.Data)]
		public void CreateDataObjectTest()
		{
			DataObjectDefine dod = new DataObjectDefine();

			dod.Name = "数据实体";
			dod.Description = "测试用的数据实体";

			dod.Fields.Add(new DataFieldDefine("UserName", PropertyDataType.String));
			dod.Fields.Add(new DataFieldDefine("Age", PropertyDataType.Integer));

			dod.Output(Console.Out, 0);
		}
	}
}
