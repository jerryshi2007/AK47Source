using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using System.Collections.Specialized;

namespace MCS.Library.Test
{
	/// <summary>
	/// Summary description for ArgumentParserTest
	/// </summary>
	[TestClass]
	public class ArgumentParserTest
	{
		public ArgumentParserTest()
		{
			//
			// TODO: Add constructor logic here
			//
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
		public void ParseArgumentsTest()
		{
			string[] arguments = new string[] { "Service", "/serviceName=Hello", "-port=8189", "=1024", "params = Face=Book" };

			StringDictionary parameters = ArgumentsParser.Parse(arguments);

			Assert.IsTrue(parameters.ContainsKey("Service"));
			Assert.AreEqual("Hello", parameters["servicename"]);
			Assert.AreEqual("8189", parameters["port"]);
			Assert.AreEqual("1024", parameters[""]);
			Assert.AreEqual("Face=Book", parameters["params"]);
		}
	}
}
