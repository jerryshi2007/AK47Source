using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Collections;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest
{
	public class WfServiceInvokerFactory
	{
		public string Url = "";
		public string Operation = "";
		public WfServiceOperationParameterCollection ParaList;
		public WfNetworkCredential Credential;
		public WfServiceAddressDefinition AddressDef;
		/// <summary>
		/// A empty service definition
		/// </summary>
		public WfServiceOperationDefinition SvcOpDef;
		public int Timeout = 10000;

		public WfServiceInvokerFactory()
		{
			Url = @"http://localhost/MCSWebApp/WebTestProject/Services/ProcessTestService.asmx";
			ParaList = new WfServiceOperationParameterCollection();
			Credential = new WfNetworkCredential();
			AddressDef = new WfServiceAddressDefinition(WfServiceRequestMethod.Get,
				Credential, Url);
			AddressDef.Key = Guid.NewGuid().ToString();
			WfGlobalParameters.Default.ServiceAddressDefs.Add(AddressDef);
			AddressDef.ServiceNS = @"http://tempuri.org/";
			SvcOpDef = new WfServiceOperationDefinition(AddressDef.Key, Operation, ParaList, "");
		}
	}

	[TestClass]
	public class WfServiceInvokerTest
	{
		public WfServiceOperationDefinition SvcOpDef;
		[TestInitialize]
		public void Setup()
		{
			SvcOpDef = new WfServiceInvokerFactory().SvcOpDef;
		}

		[TestMethod]
		public void GetMethodTest()
		{
			SvcOpDef.AddressKey = "";

			//SvcOpDef.AddressDef.RequestMethod = WfServiceRequestMethod.Get;
			InvokeService();
		}

		[TestMethod]
		public void PostMethodTest()
		{
			SvcOpDef.AddressDef.RequestMethod = WfServiceRequestMethod.Post;
			InvokeService();
		}

		[TestMethod]
		public void SoapMethodTest()
		{
			SvcOpDef.AddressDef.RequestMethod = WfServiceRequestMethod.Soap;
			SimpleData paraValue = new SimpleData()
			{
				DateTimeField = DateTime.Now,
				IntField = 1000,
				EnumField = SimpleEnum.S1
			};//StringField = "aaaaaaa",
			InvokeComplexService(paraValue);
		}

		public class SimpleData
		{
			public string StringField { get; set; }
			public int IntField { get; set; }
			public DateTime DateTimeField { get; set; }
			public SimpleEnum EnumField { get; set; }
		}

		public enum SimpleEnum
		{
			S1,
			S2
		}

		[TestMethod]
		public void CreateSoapEnvelopeTest()
		{
			//SvcOpDef.OperationName = "Method1";
			//SimpleData data = new SimpleData() { IntField = 100, StringField = "test data" };
			//WfServiceInvoker svcInvoker = new WfServiceInvoker(SvcOpDef);
			//string result = svcInvoker.CreateSoapEnvelope(data, "");
			//Console.WriteLine(result);
		}

		[TestMethod]
		public void WcfServiceTest()
		{
			WfNetworkCredential credential = new WfNetworkCredential();
			string url = @"http://localhost/MCSWebApp/WcfServiceDemo/Service1.svc";
			string addressKey = "testaddress";
			WfServiceAddressDefinition addressDef =
				new WfServiceAddressDefinition(WfServiceRequestMethod.Post, credential, url);
			addressDef.Key = addressKey;
			addressDef.ContentType = WfServiceContentType.Json;

			WfGlobalParameters.Default.ServiceAddressDefs.Add(addressDef);

			string paraVal = @"{""BoolValue"":""true"",""StringValue"":""test""}";
			WfServiceOperationParameter operationParam =
				new WfServiceOperationParameter() { Name = "composite", Type = WfSvcOperationParameterType.String, Value = paraVal };
			WfServiceOperationDefinition operationDef =
				new WfServiceOperationDefinition(addressKey, "PostContract", new WfServiceOperationParameterCollection() { operationParam }, "");
			WfServiceInvoker invoker = new WfServiceInvoker(operationDef);
			var result = invoker.Invoke();
			Console.WriteLine(result.ToString());
		}

		private void InvokeComplexService(object paraValue)
		{

		}

		private void InvokeService()
		{
			WfServiceInvoker svcInvoker = new WfServiceInvoker(SvcOpDef);
			SvcOpDef.OperationName = "Add";
			SvcOpDef.Params.Clear();
			SvcOpDef.Params.Add(new WfServiceOperationParameter()
			{
				Name = "a",
				Type = WfSvcOperationParameterType.Int,
				Value = 50
			});
			SvcOpDef.Params.Add(new WfServiceOperationParameter()
			{
				Name = "b",
				Type = WfSvcOperationParameterType.Int,
				Value = 5
			});
			var result = svcInvoker.Invoke();
			Console.WriteLine(result.GetType().ToString() + result.ToString());

			SvcOpDef.OperationName = "AddDecimal";
			result = svcInvoker.Invoke();
			Console.WriteLine(result.GetType().ToString() + result.ToString());

			SvcOpDef.OperationName = "AddDouble";
			result = svcInvoker.Invoke();
			Console.WriteLine(result.GetType().ToString() + result.ToString());

			SvcOpDef.Params.Clear();

			SvcOpDef.OperationName = "GetDate";
			result = svcInvoker.Invoke();
			Console.WriteLine(result.GetType().ToString() + result.ToString());
			SvcOpDef.OperationName = "IsTrue";
			result = svcInvoker.Invoke();
			Console.WriteLine(result.GetType().ToString() + result.ToString());
			SvcOpDef.OperationName = "SayHelloWorld";
			result = svcInvoker.Invoke();
			Console.WriteLine(result.GetType().ToString() + result.ToString());

			SvcOpDef.OperationName = "GetDayTimeSpan";
			SvcOpDef.AddressDef.RequestMethod = WfServiceRequestMethod.Soap;
			result = svcInvoker.Invoke();
			Console.WriteLine(result.GetType().ToString() + result.ToString());
		}
	}
}
