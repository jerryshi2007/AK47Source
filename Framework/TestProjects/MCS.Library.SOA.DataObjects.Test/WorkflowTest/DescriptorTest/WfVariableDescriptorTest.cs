using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.DescriptorTest
{
	[TestClass]
	public class WfVariableDescriptorTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		public void WfVariableDescriptorValueTest()
		{
			WfVariableDescriptor variable = new WfVariableDescriptor("Data", "Hello", DataType.String);

			Assert.AreEqual("Hello", variable.ActualValue);

			variable = new WfVariableDescriptor("Data", "1024", DataType.Int);

			Assert.AreEqual(1024, variable.ActualValue);

			variable = new WfVariableDescriptor("Data", "1024.00", DataType.Double);

			Assert.AreEqual(1024.00, variable.ActualValue);

			variable = new WfVariableDescriptor("Data", "1024.00", DataType.Float);

			Assert.AreEqual((Single)1024.00, variable.ActualValue);

			variable = new WfVariableDescriptor("Data", "True", DataType.Boolean);

			Assert.AreEqual(true, variable.ActualValue);

			DateTime now = DateTime.Now;

			variable = new WfVariableDescriptor("Data", now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DataType.DateTime);

			DateTime deserilizedDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);

			Assert.AreEqual(deserilizedDate, variable.ActualValue);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		public void WfVariableDescriptorArrayValueTest()
		{
			WfVariableDescriptor variable = new WfVariableDescriptor("Data", "['Hello', 'World']", DataType.StringArray);

			string[] actualValue = (string[])variable.ActualValue;

			Assert.AreEqual("Hello", actualValue[0]);
			Assert.AreEqual("World", actualValue[1]);

			variable = new WfVariableDescriptor("Data", "", DataType.StringArray);

			actualValue = (string[])variable.ActualValue;

			Assert.IsNull(actualValue);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		public void WfVariableDescriptorIntValueTest()
		{
			WfVariableDescriptor variable = new WfVariableDescriptor("Data", "[1, 3]", DataType.IntArray);

			int[] actualValue = (int[])variable.ActualValue;

			Assert.AreEqual(1, actualValue[0]);
			Assert.AreEqual(3, actualValue[1]);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		public void WfVariableDescriptorBooleanValueTest()
		{
			WfVariableDescriptor variable = new WfVariableDescriptor("Data", "[true, false]", DataType.BooleanArray);

			Boolean[] actualValue = (Boolean[])variable.ActualValue;

			Assert.AreEqual(true, actualValue[0]);
			Assert.AreEqual(false, actualValue[1]);
		}
	}
}
