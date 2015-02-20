using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class PropertyDefineTest
	{
		public PropertyDefineTest()
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
		[TestCategory(ProcessTestHelper.Properties)]
		public void PropertyGroupConfigTest()
		{
			PropertyGroupConfigurationElementCollection groups = PropertyGroupSettings.GetConfig().Groups;

			foreach (PropertyGroupConfigurationElement group in groups)
			{
				Console.WriteLine(group.Name);

				foreach (PropertyDefineConfigurationElement property in group.AllProperties)
				{
					property.Output(Console.Out, 1);
				}
			}
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Properties)]
		public void PropertyGroupWithValidatorTest()
		{
			PropertyGroupConfigurationElementCollection groups = PropertyGroupSettings.GetConfig().Groups;

			foreach (PropertyGroupConfigurationElement group in groups)
			{
				Console.WriteLine(group.Name);

				PropertyDefineCollection propertiesDefinitions = new PropertyDefineCollection();

				propertiesDefinitions.LoadPropertiesFromConfiguration(group);

				foreach (PropertyDefine pd in propertiesDefinitions)
				{
					pd.Output(Console.Out, 1);
				}
			}
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Properties)]
		public void PropertyValueValidatorTest()
		{
			PropertyGroupConfigurationElementCollection groups = PropertyGroupSettings.GetConfig().Groups;

			foreach (PropertyGroupConfigurationElement group in groups)
			{
				PropertyDefineCollection propertiesDefinitions = new PropertyDefineCollection();

				propertiesDefinitions.LoadPropertiesFromConfiguration(group);

				PropertyValueCollection properties = new PropertyValueCollection();

				properties.AppendFromPropertyDefineCollection(propertiesDefinitions);

				ValidationResults results = properties.Validate();

				Console.WriteLine(results.ToString("\r\n"));
			}
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Properties)]
		public void EnumGroupConfigTest()
		{
			EnumGroupConfigurationElementCollection groups = EnumSettings.GetConfig().Groups;

			foreach (EnumGroupConfigurationElement group in groups)
			{
				Console.WriteLine(group.Name);

				foreach (EnumConfigurationElement property in group.Items)
				{
					Console.WriteLine("\t{0}, {1}", property.Name, property.Description);
				}
			}
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Properties)]
		public void PropertyStringEmptyValidatorTest()
		{
			PropertyDefine pd = new PropertyDefine();

			pd.Name = "UserName";

			PropertyValidatorDescriptor pvd = new PropertyValidatorDescriptor();

			pvd.MessageTemplate = "属性UserName不能为空";
			pvd.TypeDescription = "MCS.Library.Validation.StringEmptyValidator, MCS.Library";

			pd.Validators.Add(pvd);

			PropertyValue pv = new PropertyValue(pd);

			ValidationResults results = new ValidationResults();

			pv.Validate(results);

			Console.WriteLine(results);

			Assert.AreEqual(1, results.ResultCount);
			Assert.AreEqual(pvd.MessageTemplate, results.First().Message);
		}
	}
}
