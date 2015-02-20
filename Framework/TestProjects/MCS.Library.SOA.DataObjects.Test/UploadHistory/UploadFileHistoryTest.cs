using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Test.UploadHistory
{
	/// <summary>
	/// Summary description for UploadFileHistoryTest
	/// </summary>
	[TestClass]
	public class UploadFileHistoryTest
	{
		public UploadFileHistoryTest()
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
		[TestCategory("UploadFileHistory")]
		public void Insert()
		{
			UploadFileHistory history = new UploadFileHistory();

			history.Operator = null;
			history.OriginalFileName = UuidHelper.NewUuidString() + ".txt";

			history.ApplicationName = "App";
			history.ProgramName = "Prog";
			history.StatusText = "一切正常";
			history.Status = UploadFileHistoryStatus.Success;
			history.Operator = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

			using (Stream stream = PrepareFileStream(history.OriginalFileName, history.OriginalFileName))
			{
				UploadFileHistoryAdapter.Instance.Insert(history, stream);
			}

			using(Stream stream = history.GetMaterialContentStream())
			{
				using (StreamReader sr = new StreamReader(history.GetMaterialContentStream()))
				{
					string content = sr.ReadToEnd();

					Assert.AreEqual(history.OriginalFileName, content);
				}
			}
		}

		private static Stream PrepareFileStream(string fileName, string content)
		{
			using (Stream fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
			{
				using (StreamWriter sw = new StreamWriter(fileStream))
				{
					sw.Write(content);
				}
			}

			return new FileStream(fileName, FileMode.Open, FileAccess.Read);
		}
	}
}
