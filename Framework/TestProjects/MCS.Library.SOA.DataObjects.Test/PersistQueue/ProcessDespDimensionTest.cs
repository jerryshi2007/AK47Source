using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.PersistQueue
{
	[TestClass]
	public class ProcessDespDimensionTest
	{
		[TestMethod]
		[Description("流程模板信息变成持久化的XML信息的测试")]
		public void NormalProcessDimensionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateProcessDescriptorForXElementSerialization();

			XElement processElem = XElement.Parse("<Process/>");
			((ISimpleXmlSerializer)processDesp).ToXElement(processElem, string.Empty);

			Console.WriteLine(processElem.ToString());
		}
	}
}
