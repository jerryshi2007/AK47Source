using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Test.WorkflowTest;
using System.Xml.Linq;
using System.IO;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	[TestClass]
	public class WfActivityInvokeServiceActionTest
	{
		[TestMethod]
		public void EnterActivityTest()
		{
			WfServiceOperationDefinition enterSvcDef = new WfServiceInvokerFactory().SvcOpDef;
			WfServiceOperationDefinition leaveSvcDef = new WfServiceInvokerFactory().SvcOpDef;
			enterSvcDef.AddressDef.RequestMethod = WfServiceRequestMethod.Get;
			enterSvcDef.OperationName = "SayHelloWorld";
			enterSvcDef.RtnXmlStoreParamName = "EnterServiceRtnXml";

			leaveSvcDef.AddressDef.RequestMethod = WfServiceRequestMethod.Get;
			leaveSvcDef.OperationName = "Add";
			leaveSvcDef.RtnXmlStoreParamName = "LeaveServiceRtnXml";
			leaveSvcDef.Params.Add(new WfServiceOperationParameter()
			{
				Name = "a",
				Type = WfSvcOperationParameterType.Int,
				Value = 50
			});
			leaveSvcDef.Params.Add(new WfServiceOperationParameter()
			{
				Name = "b",
				Type = WfSvcOperationParameterType.Int,
				Value = 5
			});

			IWfProcess process = WfProcessTestCommon.StartupProcessWithServiceDefinition(enterSvcDef, leaveSvcDef);

			Console.WriteLine("进入起始活动点");
			Console.WriteLine(process.ApplicationRuntimeParameters[enterSvcDef.RtnXmlStoreParamName]);
			Console.WriteLine(process.ApplicationRuntimeParameters[leaveSvcDef.RtnXmlStoreParamName]);

			IWfActivityDescriptor nextActivityDesp = process.CurrentActivity.Descriptor.ToTransitions[0].ToActivity;
			WfTransferParams tp = ProcessTestHelper.GetInstanceOfWfTransferParams(nextActivityDesp, OguObject.approver2);
			process.MoveTo(tp);

			Console.WriteLine("离开起始活动点");
			Console.WriteLine(process.ApplicationRuntimeParameters[enterSvcDef.RtnXmlStoreParamName]);
			Console.WriteLine(process.ApplicationRuntimeParameters[leaveSvcDef.RtnXmlStoreParamName]);
			WfRuntime.PersistWorkflows();
			process = WfRuntime.GetProcessByProcessID(process.ID);

		}

		public void Test()
		{
			var fStream = File.OpenRead(@"C:\Users\ddz\Desktop\1.txt");
			XElement xDoc = XElement.Load(fStream);
			
			XDocument doc = XDocument.Load(fStream);
			XElement firstElement = (XElement)doc.FirstNode;
			object result;
			string strValue = "";
			switch (firstElement.Name.LocalName.ToLower())
			{
				case "int":
					strValue = "5";
					result = int.Parse(strValue);
					break;
				case "decimal":
					decimal val = 500M;
					strValue = val.ToString();
					result = decimal.Parse(strValue);
					break;
				case "double":
					double dVal = 50d;
					result = double.Parse(dVal.ToString());
					break;
				case "datetime":
					result = DateTime.Parse(strValue);
					break;
				case "timespan":
					result = TimeSpan.Parse(strValue);
					break;
				case "boolean":
					result = bool.Parse(strValue);
					break;
				case "string":
					result = firstElement.Value;
					break;
				default:
					result = "";
					break;
			}
			Console.WriteLine(result.GetType().ToString());
			Console.WriteLine(result);
		}
	}
}
