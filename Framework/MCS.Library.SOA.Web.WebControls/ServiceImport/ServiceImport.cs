using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using MCS.Web.Library.Script;
using MCS.Library.Caching;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using System.Data;
using System.Net;
using System.IO;
using System.Web.Services.Description;
using System.Xml.Schema;
using MCS.Library.Core;

[assembly: WebResource("MCS.Web.WebControls.ServiceImport.ServiceImport.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.ServiceImport", "MCS.Web.WebControls.ServiceImport.ServiceImport.js")]
	[DialogContent("MCS.Web.WebControls.ServiceImport.ServiceImportTemplate.htm", "MCS.Library.SOA.Web.WebControls")]
	[ToolboxData("<{0}:ServiceImport runat=server></{0}:ServiceImport>")]
	public class ServiceImport : DialogControlBase<ServiceImportParams>
	{
		#region private
		private static readonly string ControlKey = "ServiceImportControlKey";
		private bool isInitFlag = false;
		private HtmlInputButton _confirmButton = null;
		private DropDownList _ddlServiceAddress = new DropDownList() { ID = "select_serviceAddresse_clientID" };
		private DropDownList _ddlServiceOperation = new DropDownList() { ID = "select_serviceOperation_clientID" };
		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		public ServiceImport()
		{
		}

		#region 属性
		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("confirmButtonClientID")]
		private string confirmButtonClientID
		{
			get
			{
				if (this._confirmButton != null)
					return _confirmButton.ClientID;
				else return "confirmButtonClientID";
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("serviceAddresseSelectClientID")]
		private string serviceAddresseSelectClientID
		{
			get
			{
				return this._ddlServiceAddress.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("serviceOperationSelectClientID")]
		private string serviceOperationSelectClientID
		{
			get
			{
				return this._ddlServiceOperation.ClientID;
			}
		}
		#endregion

		#region 重写的方法
		protected override void OnInit(EventArgs e)
		{

			if (!isInitFlag)
			{
				ExceptionHelper.TrueThrow(HttpContext.Current.Items.Contains(ControlKey), "页面中只能有一个ServiceImport控件！");
				HttpContext.Current.Items.Add(ControlKey, true);
				isInitFlag = true;
			}

			base.OnInit(e);

			if (this.Page.IsCallback)
				EnsureChildControls();
		}

		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();
			base.OnPagePreLoad(sender, e);

			this.DialogTitle = "ServiceImport";
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}

		protected override void InitDialogContent(Control container)
		{
			base.InitDialogContent(container);
			this.ID = "ServiceImportDialog";

			HtmlForm form = (HtmlForm)WebControlUtility.FindParentControl(this, typeof(HtmlForm), true);
			if (form != null)
			{
				form.Style["width"] = "100%";
				form.Style["height"] = "100%";
			}
			this.DialogTitle = "ServiceImport";
			this.Width = Unit.Percentage(100);
			this.Height = Unit.Percentage(100);

			InitChildControls(container);
		}

		private void InitChildControls(Control container)
		{
			Control addressDiv = WebControlUtility.FindControlByHtmlIDProperty(container, "div_serviceImport_serviceAddress", true);
			if (addressDiv != null)
			{
				this._ddlServiceAddress.Items.Clear();
				this._ddlServiceAddress.Style["width"] = "300px";
				this._ddlServiceAddress.Attributes.Add("onchange", "bindOperation()");

				this._ddlServiceAddress.DataSource = WfGlobalParameters.Default.ServiceAddressDefs;
				this._ddlServiceAddress.DataTextField = "Address";
				this._ddlServiceAddress.DataValueField = "Key";
				this._ddlServiceAddress.DataBind();

				addressDiv.Controls.Add(this._ddlServiceAddress);
			}

			Control operationDiv = WebControlUtility.FindControlByHtmlIDProperty(container, "div_serviceImport_serviceOperation", true);
			if (operationDiv != null && !string.IsNullOrEmpty(this._ddlServiceAddress.SelectedValue))
			{
				this._ddlServiceOperation.Items.Clear();
				this._ddlServiceOperation.Style["width"] = "300px";
				this._ddlServiceOperation.Attributes.Add("onchange", "analysisResult()");
				try
				{
					this._ddlServiceOperation.DataSource = GetOperationList(this._ddlServiceAddress.SelectedValue);
				}
				catch { }
				this._ddlServiceOperation.DataBind();
				this._ddlServiceOperation.Items.Insert(0, new ListItem() { Selected = true, Text = "请选择", Value = "" });
				operationDiv.Controls.Add(this._ddlServiceOperation);
			}
		}

		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 470;
			feature.Height = 210;
			feature.Resizable = false;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			base.InitConfirmButton(confirmButton);

			confirmButton.Attributes["onclick"] = "onConfirmButtonClick();";

			this._confirmButton = confirmButton;
		}
		#endregion

		#region 私有方法
		private WfServiceOperationDefinition GetWfServiceOperationDefinition(string functionName, string servicAddressKey)
		{
			WfServiceAddressDefinition serviceAddressDef = WfGlobalParameters.Default.ServiceAddressDefs[servicAddressKey];
			string addressUri = GetWsdlUrl(serviceAddressDef.Address);

			WebClient wc = new WebClient();
			using (Stream stream = wc.OpenRead(addressUri))
			{
				ServiceDescription sd = ServiceDescription.Read(stream);
				Binding targetBinding = GetBinding(sd, serviceAddressDef.RequestMethod);

				if (targetBinding == null && serviceAddressDef.RequestMethod == WfServiceRequestMethod.Post)
				{
					targetBinding = GetPostableBinding(sd);
				}

				if (targetBinding == null)
				{
					throw new NotImplementedException("服务不支持此种操作类型" + serviceAddressDef.RequestMethod.ToString());
				}

				Operation operation = GetOperation(sd, targetBinding, functionName);

				WfServiceOperationParameterCollection reqParameters = GetOperationParams(sd, operation);

				WfServiceOperationDefinition operationDef =
					new WfServiceOperationDefinition(serviceAddressDef.Key, functionName, reqParameters, string.Empty);

				return operationDef;
			}
		}

		private Binding GetBinding(ServiceDescription sd, WfServiceRequestMethod method)
		{
			Binding result = null;

			string reqMethodStr = method.ToString().ToLower();

			foreach (Binding binding in sd.Bindings)
			{
				foreach (var ext in binding.Extensions)
				{
					var sextension = ext as HttpBinding;
					if (sextension != null)
					{
						if (sextension.Verb.ToLower() == reqMethodStr)
						{
							result = binding;
							break;
						}
					}
					if ((ext is SoapBinding || ext is Soap12Binding)
						&& method == WfServiceRequestMethod.Soap)
					{
						result = binding;
						break;
					}
				}
			}

			return result;
		}

		private Operation GetOperation(ServiceDescription sd, Binding binding, string operationName)
		{
			Operation result = null;

			foreach (Operation op in sd.PortTypes[binding.Type.Name].Operations)
			{
				if (op.Name.ToLower() == operationName.ToLower())
				{
					result = op;
					break;
				}
			}

			return result;
		}

		private WfServiceOperationParameterCollection GetOperationParams(ServiceDescription sd, Operation operation)
		{
			WfServiceOperationParameterCollection result = new WfServiceOperationParameterCollection();

			if (operation == null)
			{
				return result;
			}

			foreach (OperationMessage opMessage in operation.Messages)
			{
				if (opMessage is OperationInput)
				{
					Message message = sd.Messages[opMessage.Message.Name];
					foreach (MessagePart part in message.Parts)
					{
						if (!string.IsNullOrEmpty(part.Type.Name))
						{
							result.Add(new WfServiceOperationParameter() { Name = part.Name, Type = WfSvcOperationParameterType.String });
							continue;
						}

						if (!string.IsNullOrEmpty(part.Element.Name))
						{
							XmlSchemaElement element = GetSchemaElement(sd, part.Element.Name);
							if (element == null)
							{
								continue;
							}
							if (element.ElementSchemaType is XmlSchemaComplexType)
							{
								List<XmlSchemaElement> members = GetComplexElementMember(element);
								members.ForEach(p =>
								{
									result.Add(new WfServiceOperationParameter() { Name = p.Name, Type = MappingWsdlType(p.ElementSchemaType.TypeCode) });
								});
							}
							else
							{
								result.Add(new WfServiceOperationParameter() { Name = element.Name, Type = MappingWsdlType(element.ElementSchemaType.TypeCode) });
							}
						}
					}
				}
			}

			return result;
		}

		private Binding GetPostableBinding(ServiceDescription sd)
		{
			foreach (Binding binding in sd.Bindings)
			{
				foreach (var ext in binding.Extensions)
				{
					if ((ext is SoapBinding) == false)
						continue;

					PortType portType = sd.PortTypes[binding.Type.Name];

					foreach (Operation operation in portType.Operations)
					{
						bool isPostable = true;
						foreach (OperationMessage opMessage in operation.Messages)
						{
							if (!(opMessage is OperationInput))
							{
								continue;
							}

							Message message = sd.Messages[opMessage.Message.Name];
							foreach (MessagePart part in message.Parts)
							{
								if (part.Element.Name.IsNullOrEmpty())	//说明参数是基本类型
									continue;

								XmlSchemaElement schElement = GetSchemaElement(sd, part.Element.Name);

								if (schElement.ElementSchemaType is XmlSchemaComplexType)
								{
									List<XmlSchemaElement> paramElements = GetComplexElementMember(schElement);
									paramElements.ForEach(p =>
									{
										if (p.ElementSchemaType is XmlSchemaComplexType)
										{
											isPostable = false;
										}
									});
								}
							}
						}

						if (isPostable)
						{
							return binding;
						}
					}
				}
			}

			return null;
		}

		private XmlSchemaElement GetSchemaElement(ServiceDescription sd, string elementName)
		{
			XmlSchemaElement result = null;

			foreach (XmlSchema schema in sd.Types.Schemas)
			{
				CompileSchema(schema);
				foreach (XmlSchemaElement element in schema.Elements.Values)
				{
					if (element.Name == elementName)
					{
						result = element;
						break;
					}
				}
			}

			return result;
		}

		private List<XmlSchemaElement> GetComplexElementMember(XmlSchemaElement element)
		{
			List<XmlSchemaElement> result = new List<XmlSchemaElement>();
			XmlSchemaComplexType sElement = element.ElementSchemaType as XmlSchemaComplexType;

			if (sElement != null)
			{
				XmlSchemaSequence sequence = sElement.ContentTypeParticle as XmlSchemaSequence;

				if (sequence != null)
				{
					foreach (XmlSchemaElement item in sequence.Items)
						result.Add(item);
				}
			}

			return result;
		}

		private void CompileSchema(XmlSchema schema)
		{
			if (schema.IsCompiled == false)
			{
				XmlSchemaSet schemaSet = new XmlSchemaSet();
				schemaSet.Add(schema);
				schemaSet.Compile();
			}
		}

		private WfSvcOperationParameterType MappingWsdlType(XmlTypeCode code)
		{
			WfSvcOperationParameterType result = WfSvcOperationParameterType.String;
			switch (code)
			{
				case XmlTypeCode.AnyAtomicType:
					break;
				case XmlTypeCode.AnyUri:
					break;
				case XmlTypeCode.Attribute:
					break;
				case XmlTypeCode.Base64Binary:
					break;
				case XmlTypeCode.Boolean:
					break;
				case XmlTypeCode.Byte:
					break;
				case XmlTypeCode.Comment:
					break;
				case XmlTypeCode.Date:
					break;
				case XmlTypeCode.DateTime:
					result = WfSvcOperationParameterType.DateTime;
					break;
				case XmlTypeCode.DayTimeDuration:
					break;
				case XmlTypeCode.Decimal:
					result = WfSvcOperationParameterType.Int;
					break;
				case XmlTypeCode.Document:
					break;
				case XmlTypeCode.Double:
					result = WfSvcOperationParameterType.Int;
					break;
				case XmlTypeCode.Duration:
					break;
				case XmlTypeCode.Element:
					break;
				case XmlTypeCode.Entity:
					break;
				case XmlTypeCode.Float:
					result = WfSvcOperationParameterType.Int;
					break;
				case XmlTypeCode.GDay:
					break;
				case XmlTypeCode.GMonth:
					break;
				case XmlTypeCode.GMonthDay:
					break;
				case XmlTypeCode.GYear:
					break;
				case XmlTypeCode.GYearMonth:
					break;
				case XmlTypeCode.HexBinary:
					break;
				case XmlTypeCode.Id:
					break;
				case XmlTypeCode.Idref:
					break;
				case XmlTypeCode.Int:
					result = WfSvcOperationParameterType.Int;
					break;
				case XmlTypeCode.Integer:
					result = WfSvcOperationParameterType.Int;
					break;
				case XmlTypeCode.Item:
					break;
				case XmlTypeCode.Language:
					break;
				case XmlTypeCode.Long:
					result = WfSvcOperationParameterType.Int;
					break;
				case XmlTypeCode.NCName:
					break;
				case XmlTypeCode.Name:
					break;
				case XmlTypeCode.Namespace:
					break;
				case XmlTypeCode.NegativeInteger:
					break;
				case XmlTypeCode.NmToken:
					break;
				case XmlTypeCode.Node:
					break;
				case XmlTypeCode.NonNegativeInteger:
					break;
				case XmlTypeCode.NonPositiveInteger:
					break;
				case XmlTypeCode.None:
					break;
				case XmlTypeCode.NormalizedString:
					break;
				case XmlTypeCode.Notation:
					break;
				case XmlTypeCode.PositiveInteger:
					break;
				case XmlTypeCode.ProcessingInstruction:
					break;
				case XmlTypeCode.QName:
					break;
				case XmlTypeCode.Short:
					result = WfSvcOperationParameterType.Int;
					break;
				case XmlTypeCode.String:
					result = WfSvcOperationParameterType.String;
					break;
				case XmlTypeCode.Text:
					break;
				case XmlTypeCode.Time:
					break;
				case XmlTypeCode.Token:
					break;
				case XmlTypeCode.UnsignedByte:
					break;
				case XmlTypeCode.UnsignedInt:
					break;
				case XmlTypeCode.UnsignedLong:
					break;
				case XmlTypeCode.UnsignedShort:
					break;
				case XmlTypeCode.UntypedAtomic:
					break;
				case XmlTypeCode.YearMonthDuration:
					break;
				default:
					break;
			}
			return result;
		}

		private string GetWsdlUrl(string url)
		{
			if (url == null) return "";
			string addressSuffix = "?wsdl";
			return url.EndsWith(addressSuffix) ? url : url + addressSuffix;
		}

		private List<string> GetOperationList(string addressKey)
		{
			List<string> result = new List<string>();
			WfServiceAddressDefinition serviceAddressDef = WfGlobalParameters.Default.ServiceAddressDefs[addressKey];
			string addressUri = GetWsdlUrl(serviceAddressDef.Address);

			WebClient wc = new WebClient();

			if (serviceAddressDef.Credential != null)
				wc.Credentials = (NetworkCredential)serviceAddressDef.Credential;

			using (Stream stream = wc.OpenRead(addressUri))
			{
				ServiceDescription sd = ServiceDescription.Read(stream);
				Binding targetBinding = GetBinding(sd, serviceAddressDef.RequestMethod);

				if (targetBinding == null && serviceAddressDef.RequestMethod == WfServiceRequestMethod.Post)
				{
					targetBinding = GetPostableBinding(sd);
				}

				if (targetBinding == null)
				{
					return result;
				}

				PortType portType = sd.PortTypes[targetBinding.Type.Name];
				foreach (Operation op in portType.Operations)
				{
					result.Add(op.Name);
				}
				return result;
			}
		}
		#endregion

		#region ScriptControlMethods
		[ScriptControlMethod]
		public string getAnalysisResult(string functionName, string servicAddress)
		{
			WfServiceOperationDefinition state = GetWfServiceOperationDefinition(functionName, servicAddress);
			if (state != null)
				return JSONSerializerExecute.Serialize(state, state.GetType());
			else
				return "";
		}

		[ScriptControlMethod]
		public string getOperationList(string addressKey)
		{
			List<string> opList = GetOperationList(addressKey);
			return JSONSerializerExecute.Serialize(opList);
		}
		#endregion
	}
}
