using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using System.Web.Services;
using System.Web.Services.Discovery;
using System.Net;
using System.Collections;
using System.Web.Services.Description;

namespace WorkflowDesigner.ModalDialog
{
	public partial class WfServiceOperationDefEditor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WfConverterHelper.RegisterConverters();

			WfServiceOperationDefinition svcOpDef = new WfServiceOperationDefinition();
			hiddenSvcOperationTemplate.Value = JSONSerializerExecute.Serialize(svcOpDef);

			var dataTypeList = EnumItemDescriptionAttribute.GetDescriptionList(typeof(WfSvcOperationParameterType));
			dataTypeDropDownList.DataSource = dataTypeList;
			dataTypeDropDownList.DataTextField = "Name";

			dataTypeDropDownList.DataValueField = "EnumValue";
			dataTypeDropDownList.DataBind();

			if (Request["hasRtn"] == "false")
			{
				this.trRtn.Style["display"] = "none";
			}

			if (!string.IsNullOrEmpty(Request["initPara"]))
			{
				var initData = new WfServiceOperationParameterCollection();
				initData.Add(new WfServiceOperationParameter()
				{
					Name = Request["initPara"],
					Type = WfSvcOperationParameterType.RuntimeParameter,
					Value = ""
				});
				detailGrid.InitialData = initData;
			}
		}

		protected void btnRefresh_Click(object sender, EventArgs e)
		{

		}

		[WebMethod()]
		public static string[] DiscoverMethods(string address, string httpMethod)
		{
			ServiceDescription desc = GetServiceDescription(address);
			return DiscoverOperations(desc);
		}

		[WebMethod()]
		public static WfServiceOperationParameterCollection DiscoverParameters(string address, string httpMethod, string method)
		{
			WfServiceOperationParameterCollection rst = new WfServiceOperationParameterCollection();

			ServiceDescription desc = GetServiceDescription(address);

			var xMessage = DiscoverMethodMessage(method, httpMethod, desc);
			if (xMessage != null)
			{
				var msg = desc.Messages[xMessage.Name];
				if (httpMethod == "Soap")
				{
					ExtractSoapParameters(rst, desc, msg);
				}
				else
				{
					foreach (MessagePart pt in msg.Parts)
					{
						rst.Add(new WfServiceOperationParameter() { Name = pt.Name, Type = SolveType(pt.Type) });
					}
				}
			}
			else if (httpMethod != "Soap")
			{
				//尝试作为SOAP处理
				xMessage = DiscoverMethodMessage(method, "Soap", desc);
				if (xMessage != null)
				{
					var msg = desc.Messages[xMessage.Name];
					ExtractSoapParameters(rst, desc, msg);

				}
			}

			return rst;
		}

		private static void ExtractSoapParameters(WfServiceOperationParameterCollection rst, ServiceDescription desc, Message msg)
		{
			if (msg.Parts["parameters"] != null)
			{
				var ppsElemName = msg.Parts["parameters"].Element;
				var schema = desc.Types.Schemas[ppsElemName.Namespace];
				System.Xml.Schema.XmlSchemaElement innerTypes = (System.Xml.Schema.XmlSchemaElement)schema.Elements[ppsElemName];
				System.Xml.Schema.XmlSchemaComplexType xType = innerTypes.SchemaType as System.Xml.Schema.XmlSchemaComplexType;
				if (xType != null)
				{
					System.Xml.Schema.XmlSchemaSequence xSeq = xType.Particle as System.Xml.Schema.XmlSchemaSequence;
					if (xSeq != null)
					{
						foreach (var xItem in xSeq.Items)
						{
							if (xItem is System.Xml.Schema.XmlSchemaElement)
							{
								System.Xml.Schema.XmlSchemaElement xElem = xItem as System.Xml.Schema.XmlSchemaElement;
								rst.Add(new WfServiceOperationParameter() { Name = xElem.Name, Type = SolveType(xElem.SchemaTypeName) });
							}
						}
					}
				}
			}
		}

		private static ServiceDescription GetServiceDescription(string address)
		{
			DiscoveryClientProtocol client = new DiscoveryClientProtocol();
			client.Credentials = CredentialCache.DefaultCredentials;

			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(address + "?wsdl");
			var resp = req.GetResponse();
			ServiceDescription desc;
			using (System.IO.Stream source = resp.GetResponseStream())
			{
				desc = ServiceDescription.Read(source);
			}

			resp.Close();
			return desc;
		}

		private static WfSvcOperationParameterType SolveType(System.Xml.XmlQualifiedName xmlQualifiedName)
		{
			switch (xmlQualifiedName.Name)
			{
				case "int":
				case "integer":
				case "byte":
				case "short":
					return WfSvcOperationParameterType.Int;
				case "datetime":
					return WfSvcOperationParameterType.DateTime;
				case "string":
					return WfSvcOperationParameterType.String;
				default:
					return WfSvcOperationParameterType.RuntimeParameter;
			}
		}

		private static string[] DiscoverOperations(ServiceDescription sd)
		{
			HashSet<string> set = new HashSet<string>();
			foreach (PortType pt in sd.PortTypes)
			{
				foreach (Operation opItem in pt.Operations)
				{
					set.Add(opItem.Name);
				}
			}

			return set.ToArray();
		}

		private static System.Xml.XmlQualifiedName DiscoverMethodMessage(string method, string httpHint, ServiceDescription sd)
		{
			string hint = method + httpHint + "In";
			System.Xml.XmlQualifiedName xMessage = null;

			string serviceName = sd.Services[0].Name;
			PortType pt = sd.PortTypes[serviceName + httpHint];

			if (pt == null)
				return null;

			foreach (Operation opItem in pt.Operations)
			{
				if (opItem.Name == method)
				{
					foreach (OperationMessage mss in opItem.Messages)
					{
						OperationInput inputMessage = mss as OperationInput;
						if (inputMessage != null)
						{
							xMessage = inputMessage.Message;
							break;
						}
					}
				}
			}

			return xMessage;
		}
	}
}