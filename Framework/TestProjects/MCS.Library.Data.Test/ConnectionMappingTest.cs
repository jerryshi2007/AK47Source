using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.Data.Test
{
	/// <summary>
	/// 此测试用于验证ConnectionMapping（连接串映射）的功能
	/// </summary>
	[TestClass]
	public class ConnectionMappingTest
	{
		public ConnectionMappingTest()
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
		[Description("简单的连接名称映射测试")]
		public void SimpleConnectionMapping()
		{
			DbConnectionMappingContext.DoMappingAction("HB2008", "HB2008Test", () =>
			{
				Assert.AreEqual("HB2008Test", DbConnectionMappingContext.GetMappedConnectionName("HB2008"));
			});
		}

		[TestMethod]
		[Description("恢复嵌套的连接名称映射测试")]
		public void RestoreNestedConnectionMapping()
		{
			DbConnectionMappingContext.DoMappingAction("HB2008", "HB2008Test", () =>
			{
				Assert.AreEqual("HB2008Test", DbConnectionMappingContext.GetMappedConnectionName("HB2008"));

				DbConnectionMappingContext.DoMappingAction("HB2008", "HB2008Archive", () =>
					{
						Assert.AreEqual("HB2008Archive", DbConnectionMappingContext.GetMappedConnectionName("HB2008"));
					});

				Assert.AreEqual("HB2008Test", DbConnectionMappingContext.GetMappedConnectionName("HB2008"));
			});
		}

		[TestMethod]
		[Description("级联的连接名称映射测试")]
		public void CascadeConnectionMapping()
		{
			DbConnectionMappingContext.DoMappingAction("HB2008", "HB2008Test", () =>
			{
				DbConnectionMappingContext.DoMappingAction("PermissionCenter", "HB2008", () =>
				{
					Assert.AreEqual("HB2008Test", DbConnectionMappingContext.GetMappedConnectionName("PermissionCenter"));
				});

				Assert.AreEqual("HB2008Test", DbConnectionMappingContext.GetMappedConnectionName("HB2008"));
			});
		}

		[TestMethod]
		[Description("死锁型级联的连接名称映射测试")]
		public void CascadeDeadLockConnectionMapping()
		{
			DbConnectionMappingContext.DoMappingAction("HB2008", "PermissionCenter", () =>
			{
				DbConnectionMappingContext.DoMappingAction("PermissionCenter", "HB2008", () =>
				{
					Assert.AreEqual("PermissionCenter", DbConnectionMappingContext.GetMappedConnectionName("PermissionCenter"));
				});

				Assert.AreEqual("PermissionCenter", DbConnectionMappingContext.GetMappedConnectionName("HB2008"));
			});
		}
	}
}
