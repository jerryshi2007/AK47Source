using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections;
using System.Drawing.Design;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI.Design.WebControls;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Web.Library.Script;
using MCS.Library.Validation;
using MCS.Library.Data.Mapping;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.DataBindingControl.DataBindingControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.Images.error.gif", "image/gif")]
namespace MCS.Web.WebControls
{
	public delegate ValidationResult ValidateData(object sender, EventArgs e);

	[ParseChildren(true)]
	[DefaultProperty("ItemBindings")]
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.DataBindingControl", "MCS.Web.WebControls.DataBindingControl.DataBindingControl.js")]
	[ToolboxData("<{0}:DataBindingControl runat=server></{0}:DataBindingControl>")]
	public class DataBindingControl : ScriptControlBase, IWfApplicationRuntimeParametersCollector
	{
		private DataBindingItemCollection bindingData = new DataBindingItemCollection();
		private object data = null;
		private List<ClientVdtData> validatorAttributes = null;
		private Type _dataType = null;
		private DataBindingInnerValidationControl _innerPageValidator = new DataBindingInnerValidationControl();
		private TextBox _innerValidateTextBox = new TextBox();

		public DataBindingControl()
			: base(true, HtmlTextWriterTag.Div)
		{
			validatorAttributes = new List<ClientVdtData>();
		}

		/// <summary>
		/// �ռ�����
		/// </summary>
		public void CollectData()
		{
			CollectData(AutoValidate);
		}

		/// <summary>
		/// �ռ����ݣ���У��ָ������
		/// </summary>
		/// <param name="validationGroup">ָ������ţ����С��0����Ϊȫ��У��</param>
		public void CollectData(int validationGroup)
		{
			ExceptionHelper.FalseThrow(this.data != null, "������ԴΪ�գ�Data���Բ���Ϊnull��");

			if (validationGroup < 0)
			{
				CollectData(true);
			}
			else
			{
				string validatemessage = string.Empty;

				DataBindingItemCollection unValidates = MappingControlsToData(this.data, this.ItemBindings);

				foreach (DataBindingItem item in this.ItemBindings)
				{
					if ((item.ValidationGroup & validationGroup) == 0)
						unValidates.Add(item);
				}

				ExceptionHelper.FalseThrow(ValidateData(this.data, unValidates, out validatemessage), validatemessage);

				CollectApplicationData();
			}
		}

		/// <summary>
		/// �ռ�����
		/// </summary>
		/// <param name="autoValidate">�Ƿ��Զ�У��</param>
		public void CollectData(bool autoValidate)
		{
			ExceptionHelper.FalseThrow(this.data != null, "������ԴΪ�գ�Data���Բ���Ϊnull��");

			string validatemessage = string.Empty;

			DataBindingItemCollection unValidates = MappingControlsToData(this.data, this.ItemBindings);

			if (autoValidate)
				ExceptionHelper.FalseThrow(ValidateData(this.data, unValidates, out validatemessage), validatemessage);

			CollectApplicationData();
		}

		protected override void OnInit(EventArgs e)
		{
			WfApplicationRuntimeParametersCollector.RegisterCollector(this);
			base.OnInit(e);
		}

		protected override void OnDataBinding(EventArgs e)
		{
			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("DataBindingControl-RegCheckScript",
				() => this.RegCheckScript());

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("DataBindingControl-MappingDataToControls",
				() => MappingDataToControls(this.data, this.ItemBindings));

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("DataBindingControl-MappingDataToValidate",
				() => MappingDataToValidate());

			base.OnDataBinding(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			Page.PreRender += new EventHandler(Page_PreRender);

			base.OnLoad(e);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this._innerPageValidator.BindingControl = this;
			this.Controls.Add(this._innerPageValidator);

			this._innerValidateTextBox.ID = "_innerValidateTextBox";
			this._innerValidateTextBox.Style["display"] = "none";

			this.Controls.Add(this._innerValidateTextBox);

			this._innerPageValidator.ControlToValidate = this._innerValidateTextBox.ID;
		}

		protected override void Render(HtmlTextWriter output)
		{
			if (this.DesignMode)
			{
				output.Write(string.Format("<div>Data Binding begin</div>"));
				foreach (DataBindingItem item in ItemBindings)
				{
					output.Write(string.Format("<div>{0}:{1} mapping to {2}</div>",
						HttpUtility.HtmlEncode(item.ControlID),
						HttpUtility.HtmlEncode(item.ControlPropertyName),
						HttpUtility.HtmlEncode(item.DataPropertyName)));
				}
				output.Write(string.Format("<div>Data Binding end</div>"));
			}
			else
			{
				base.Render(output);
			}
		}

		private void Page_PreRender(object sender, EventArgs e)
		{
			if (AutoBinding)
			{
				if (this.data != null)
					DataBind();
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "����Ƿ�");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "�������ֵ�λ�����ܳ���{0}λ");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "С�����ֵ�λ�����ܳ���{0}λ");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "���ֱ�����{0}��{1}֮��");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "���ֱ�����ڵ���{0}");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "���ֱ���С�ڵ���{0}");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "��������ֻ����һ��С����");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "��������ֻ����һ��'{0}'");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "��������Ϸ�������");

			base.OnPreRender(e);
		}

		protected override string SaveClientState()
		{
			if (!this.Page.IsCallback)
			{
				List<ClientDataBindingItem> clientBindings = new List<ClientDataBindingItem>();

				FillClientDataBindingItemCollection(clientBindings);

				object[] state = new object[] { this.validatorAttributes, clientBindings };

				return JSONSerializerExecute.Serialize(state);
			}
			else
				return string.Empty;
		}

		private void FillClientDataBindingItemCollection(List<ClientDataBindingItem> clientBindings)
		{
			if (AllowClientCollectData)
			{
				foreach (DataBindingItem srcItem in ItemBindings)
				{
					ClientDataBindingItem newItem = new ClientDataBindingItem(srcItem);

					if (string.IsNullOrEmpty(srcItem.DataPropertyName))
					{
						if (this.Data != null)
							newItem.DataPropertyName = GetObjectPropertyName(this.Data.GetType(), srcItem.DataPropertyName);
					}

					if (string.IsNullOrEmpty(newItem.ClientDataPropertyName))
						newItem.ClientDataPropertyName = newItem.DataPropertyName;

					if (srcItem.ClientDataType == ClientBindingDataType.None)
					{
						srcItem.ClientDataType = ClientBindingDataType.String;
					}

					if (string.IsNullOrEmpty(srcItem.ClientSetPropName))
						newItem.ClientSetPropName = srcItem.ClientPropName;

					Control control = this.FindControlByPath(this, srcItem.ControlID);

					if (control != null)
						newItem.ControlID = control.ClientID;

					clientBindings.Add(newItem);
				}
			}
		}

		#region Properties
		/// <summary>
		/// �ⲿ��֤����
		/// </summary>
		public List<ValidateData> OnValidateDatas = new List<ValidateData>();

		[PersistenceMode(PersistenceMode.InnerProperty), Description("�����ݵ�ӳ��")]
		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue((string)null)]
		[Editor(typeof(DataBindingItemsEditor), typeof(UITypeEditor))]
		[Browsable(false)]
		public DataBindingItemCollection ItemBindings
		{
			get
			{
				return bindingData;
			}
		}

		[Browsable(false)]
		public object Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		/// <summary>
		/// �Զ������ݰ󶨵��ؼ�
		/// </summary>
		[DefaultValue(true)]
		public bool AutoBinding
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AutoBinding", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AutoBinding", value);
			}
		}

		[DefaultValue("")]
		public string DataTypeName
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "DataTypeName", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "DataTypeName", value);
				this._dataType = null;
			}
		}

		private Type DataType
		{
			get
			{
				if (this._dataType == null && DataTypeName.IsNotEmpty())
					this._dataType = TypeCreator.GetTypeInfo(DataTypeName);

				return this._dataType;
			}
		}

		/// <summary>
		/// �ռ�����ʱ�Զ�У���ȡ�������ϵ�����
		/// </summary>
		[DefaultValue(true)]
		public bool AutoValidate
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AutoValidate", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AutoValidate", value);
			}
		}

		/// <summary>
		/// �Ƿ�У���Ӷ���
		/// </summary>
		[DefaultValue(false)]
		public bool IsValidateChildren
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "IsValidateChildren", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "IsValidateChildren", value);
			}
		}

		/// <summary>
		/// PostBack���Ƿ��Զ��ռ����ݣ���У�飩���˲���������Load�Ϳؼ��¼�(Button Click)֮��
		/// </summary>
		[DefaultValue(true)]
		public bool AutoCollectDataWhenPostBack
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AutoCollectDataWhenPostBack", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AutoCollectDataWhenPostBack", value);
			}
		}

		/// <summary>
		/// �Ƿ�����ͻ����ռ����ݡ��������Ƿ�����BindingItems���ͻ���
		/// </summary>
		[DefaultValue(false)]
		public bool AllowClientCollectData
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AllowClientCollectData", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AllowClientCollectData", value);
			}
		}

		/// <summary>
		/// �Զ�У���ȡ�������ϵ�����
		/// </summary>
		[DefaultValue(false)]
		public bool IsValidateOnSubmit
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "IsValidateOnSubmit", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "IsValidateOnSubmit", value);
			}
		}

		/// <summary>
		/// �Ƿ�У��û�а󶨵�����
		/// </summary>
		[DefaultValue(true)]
		public bool ValidateUnbindProperties
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ValidateUnbindProperties", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ValidateUnbindProperties", value);
			}
		}

		[ScriptControlProperty, ClientPropertyName("errorImg"), Browsable(false)]
		private string ErrorImg
		{
			get
			{
				return this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.DataBindingControl),
					"MCS.Web.WebControls.Images.error.gif");
			}
		}
		#endregion Properties

		#region Client Events
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("clientDataBinding")]
		public string OnClientDataBinding
		{
			get
			{
				return GetPropertyValue("OnClientDataBinding", string.Empty);
			}
			set
			{
				SetPropertyValue("OnClientDataBinding", value);
			}
		}

		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("clientCollectData")]
		public string OnClientCollectData
		{
			get
			{
				return GetPropertyValue("OnClientCollectData", string.Empty);
			}
			set
			{
				SetPropertyValue("OnClientCollectData", value);
			}
		}
		#endregion Client Events

		#region ControlsToData

		/// <summary>
		/// �ؼ���������
		/// </summary>
		/// <param name="sourceData">����Դ</param>
		/// <param name="items">�󶨶���</param>
		/// <returns>����У�������</returns>
		private DataBindingItemCollection MappingControlsToData(object sourceData, DataBindingItemCollection items)
		{
			DataBindingItemCollection unValidates = new DataBindingItemCollection();

			foreach (DataBindingItem item in items)
			{
				if (!item.IsValidate)
					unValidates.Add(item);

				if ((item.Direction & BindingDirection.ControlToData) != BindingDirection.None)
				{
					//����·��
					string targetPro = string.Empty;
					object subData;
					//��������
					string targetProName = item.DataPropertyName;

					SplitPath(sourceData, item.DataPropertyName, out targetPro, out targetProName);

					if (item.SubBindings.Count > 0)
					{
						subData = FindObjectByPath(sourceData, item.DataPropertyName);
						MappingControlsToData(subData, item.SubBindings);
					}
					else
					{
						subData = FindObjectByPath(sourceData, targetPro);

						MappingOneControlToOneData(subData, targetProName, item);
					}
				}
			}

			return unValidates;
		}

		/// <summary>
		/// ��ȡ������������
		/// </summary>
		/// <param name="dataPropertyInfo">����</param>
		/// <param name="currData">��ǰ����</param>
		/// <param name="dataTargetProName">Ŀ����������</param>
		/// <param name="item">����</param>
		private void MappingOneControlToOneData(object currData, string dataTargetProName, DataBindingItem item)
		{
			Control targetControl = FindControlByPath(this, item.ControlID);

			//�ؼ�����·��
			string targetPro = string.Empty;
			//�ؼ���������
			string targetProName = item.ControlPropertyName;
			ExceptionHelper.FalseThrow(targetControl != null, "�����ҵ�IDΪ{0}�Ŀؼ�", item.ControlID);

			SplitPath(targetControl, item.ControlPropertyName, out targetPro, out targetProName);

			Type dataType = this.DataType;

			if (currData != null)
				dataType = currData.GetType();

			//��ȡĿ������ֵ
			object targetItem = FindObjectByPath(targetControl, targetPro);
			if (targetItem != null && dataType != null)
			{
				PropertyInfo piDest = TypePropertiesCacheQueue.Instance.GetPropertyInfo(dataType,
						GetObjectPropertyName(dataType, dataTargetProName));

				if (piDest != null)
				{
					if (targetItem is CheckBoxList && piDest.PropertyType.IsEnum && FlagsAttribute.IsDefined(piDest.PropertyType, typeof(FlagsAttribute)))
					{
						//Flags�͵�ö�ٺ�CheckBox֮������⴦��
						CollectDataFromCheckBoxListToFlagsEnumProperty(item, ((ListControl)targetItem).Items, piDest, currData);
					}
					else
					{
						//��ȡ�������ϵ���Ϣ���󶨶�������
						object targetValue = FindObjectByPath(targetItem, targetProName);

						if (piDest.CanWrite)
						{
							targetValue = UnformatControlProperty(targetValue, piDest, item);

							SetSimpleValueToData(item, piDest, currData, targetValue);
						}
						else
						{
							ProcessCollectionProperty(piDest, currData, targetValue);
						}
					}
				}
			}
		}

		private static void SetSimpleValueToData(DataBindingItem bindingItem, PropertyInfo piDest, object graph, object targetValue)
		{
			if (graph != null)
				piDest.SetValue(graph, targetValue, null);

			if (bindingItem.CollectToProcessParameters && CurrentProcess != null)
			{
				string parameterName = bindingItem.ProcessParameterName;

				if (parameterName.IsNullOrEmpty())
				{
					if (graph != null)
						parameterName = GetObjectPropertyName(graph.GetType(), bindingItem.DataPropertyName);
					else
						parameterName = bindingItem.DataPropertyName;
				}

				if ((bindingItem.ProcessParameterEvalMode & ProcessParameterEvalMode.CurrentProcess) == ProcessParameterEvalMode.CurrentProcess)
				{
					CurrentProcess.ApplicationRuntimeParameters[parameterName] = targetValue;
				}

				if ((bindingItem.ProcessParameterEvalMode & ProcessParameterEvalMode.ApprovalRootProcess) == ProcessParameterEvalMode.ApprovalRootProcess)
				{
					CurrentProcess.ApprovalRootProcess.ApplicationRuntimeParameters[parameterName] = targetValue;
				}

				if ((bindingItem.ProcessParameterEvalMode & ProcessParameterEvalMode.RootProcess) == ProcessParameterEvalMode.RootProcess)
				{
					CurrentProcess.RootProcess.ApplicationRuntimeParameters[parameterName] = targetValue;
				}

				if ((bindingItem.ProcessParameterEvalMode & ProcessParameterEvalMode.SameResourceRootProcess) == ProcessParameterEvalMode.SameResourceRootProcess)
				{
					CurrentProcess.RootProcess.ApplicationRuntimeParameters[parameterName] = targetValue;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="piDest"></param>
		/// <param name="currData"></param>
		/// <param name="targetValue"></param>
		private static void ProcessCollectionProperty(PropertyInfo piDest, object currData, object targetValue)
		{
			if (targetValue != null && targetValue is IEnumerable && currData != null)
			{
				object dataPropertyValue = piDest.GetValue(currData, null);

				if (dataPropertyValue != null && dataPropertyValue is IList)
				{
					IList listValue = (IList)dataPropertyValue;

					listValue.Clear();

					IEnumerable enumerableData = (IEnumerable)targetValue;

					foreach (object data in enumerableData)
						listValue.Add(data);
				}
			}
		}

		private static void CollectDataFromCheckBoxListToFlagsEnumProperty(DataBindingItem bindingItem, ListItemCollection items, PropertyInfo piDest, object currData)
		{
			if (piDest.CanWrite)
			{
				int targetValue = 0;

				foreach (ListItem item in items)
				{
					int n = 0;

					if (int.TryParse(item.Value, out n))
					{
						if (item.Selected)
							targetValue |= n;
					}
				}

				SetSimpleValueToData(bindingItem, piDest, currData, targetValue);
			}
		}

		/// <summary>
		/// �����ʽ������
		/// </summary>
		/// <param name="sourceData">����Դ</param>
		/// <param name="piDest">����</param>
		/// <param name="item">����</param>
		/// <returns></returns>
		private object UnformatControlProperty(object sourceData, PropertyInfo piDest, DataBindingItem item)
		{
			object targetValue = sourceData;

			if (ControlDataIsNull(sourceData))
			{
				if (piDest.PropertyType.IsValueType)
					targetValue = TypeCreator.CreateInstance(piDest.PropertyType);
			}
			else
			{
				if (IsNumericType(piDest.PropertyType))
				{
					if (sourceData.GetType() == typeof(string))
						sourceData = ((string)sourceData).Replace(",", string.Empty);
				}

				targetValue = DataConverter.ChangeType(sourceData, GetRealDataType(piDest.PropertyType));

				if (piDest.PropertyType == typeof(string))
				{
					switch (item.ItemTrimBlankType)
					{
						case TrimBlankType.Right:
							{
								targetValue = targetValue.ToString().TrimEnd(new char[] { ' ' });
								break;
							}
						case TrimBlankType.Left:
							{
								targetValue = targetValue.ToString().TrimStart(new char[] { ' ' });
								break;
							}
						case TrimBlankType.None:
							{
								break;
							}
						case TrimBlankType.ALL:
							{
								targetValue = new Regex(@"\s").Replace(targetValue.ToString(), "");
								break;
							}
						default:
							{
								targetValue = targetValue.ToString().Trim();
								break;
							}
					}

				}
			}

			return targetValue;
		}

		private static Type GetRealDataType(Type type)
		{
			Type result = type;

			if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1")
				result = type.GetGenericArguments()[0];

			return result;
		}

		/// <summary>
		/// �ؼ��������ǲ���Ϊ��
		/// </summary>
		/// <param name="sourceData"></param>
		/// <returns></returns>
		private bool ControlDataIsNull(object sourceData)
		{
			bool result = false;

			if (sourceData == null)
				result = true;
			else
				if (sourceData is string)
					result = (string)sourceData == string.Empty;

			return result;
		}

		#endregion

		#region DataToControls

		/// <summary>
		/// �����ݰ����ؼ�
		/// </summary>
		/// <param name="sourceData">����Դ</param>
		/// <param name="items">������</param>
		private void MappingDataToControls(object sourceData, DataBindingItemCollection items)
		{
			if (sourceData != null)
			{
				foreach (DataBindingItem item in items)
				{
					if ((item.Direction & BindingDirection.DataToControl) != BindingDirection.None)
					{
						//����·��
						string targetPro = string.Empty;
						object subData;

						//��������
						string targetProName = item.DataPropertyName;

						SplitPath(sourceData, item.DataPropertyName, out targetPro, out targetProName);

						//��ȡ·���ϵĶ���

						if (item.SubBindings.Count > 0)
						{
							subData = FindObjectByPath(sourceData, item.DataPropertyName);

							if (subData != null)
							{
								MappingDataToControls(subData, item.SubBindings);
							}
						}
						else
						{
							subData = FindObjectByPath(sourceData, targetPro);

							if (subData != null)
							{
								MappingOneDataToOneControl(TypePropertiesCacheQueue.Instance.GetPropertyInfo(subData.GetType(),
										GetObjectPropertyName(subData.GetType(), targetProName)), subData, targetProName, item);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// �󶨵����ؼ�
		/// </summary>
		/// <param name="dataPropertyInfo">����</param>
		/// <param name="currData">��ǰ����</param>
		/// <param name="dataTargetProName">Ŀ����������</param>
		/// <param name="item">����</param>
		private void MappingOneDataToOneControl(PropertyInfo dataPropertyInfo, object currData, string dataTargetProName, DataBindingItem item)
		{
			if (currData != null)
			{
				Control targetControl = FindControlByPath(this, item.ControlID);

				//�ؼ�����·��
				string targetPro = string.Empty;
				//�ؼ���������
				string targetProName = item.ControlPropertyName;
				ExceptionHelper.FalseThrow(targetControl != null, "�����ҵ�IDΪ{0}�Ŀؼ�", item.ControlID);

				SplitPath(targetControl, item.ControlPropertyName, out targetPro, out targetProName);

				//��ȡĿ������ֵ
				object targetItem = FindObjectByPath(targetControl, targetPro);
				if (targetItem != null)
				{
					//��ȡĿ��
					PropertyInfo piDest = TypePropertiesCacheQueue.Instance.GetPropertyInfo(targetItem.GetType(),
							GetObjectPropertyName(targetItem.GetType(), targetProName));

					if (piDest != null && piDest.CanWrite)
					{
						//��ȡ�������ϵ���Ϣ���󶨿ؼ�����
						object targetValue = FindObjectByPath(currData, dataTargetProName);
						if (targetValue != null)
						{
							if (SetControlsAttributesByDataType(dataPropertyInfo, targetValue, targetItem, item, targetProName) == false)
							{
								targetValue = FormatDataProperty(targetValue, piDest, item);
								piDest.SetValue(targetItem, targetValue, null);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// ��ʽ������
		/// </summary>
		/// <param name="sourceData">����Դ</param>
		/// <param name="piDest">����</param>
		/// <param name="item">����</param>
		/// <returns></returns>
		private object FormatDataProperty(object sourceData, PropertyInfo piDest, DataBindingItem item)
		{
			object targetValue = null;

			if (sourceData == null)
				return string.Empty;

			if (sourceData.GetType().IsEnum && item.EnumUsage == EnumUsageTypes.UseEnumValue)
			{
				targetValue = DataConverter.ChangeType((int)sourceData, GetRealDataType(piDest.PropertyType));
			}
			else
			{
				if (string.IsNullOrEmpty(item.Format))
					targetValue = DataConverter.ChangeType(sourceData, GetRealDataType(piDest.PropertyType));
				else
					targetValue = sourceData;
			}

			if (piDest.PropertyType == typeof(string))
			{
				bool isDefault = false;

				if (item.FormatDefaultValueToEmpty)
				{
					Type srcDataType = sourceData.GetType();

					if (srcDataType.IsValueType && srcDataType != typeof(Boolean))
					{
						object defaultValue = TypeCreator.CreateInstance(srcDataType);

						if (sourceData.Equals(defaultValue) && string.IsNullOrEmpty(item.Format))
						{
							targetValue = string.Empty;
							isDefault = true;
						}
					}
				}

				//����else��ֻҪ�������˸�ʽ������Ӧ�ø�ʽ�����ݡ� 
				if (!isDefault && string.IsNullOrEmpty(item.Format) == false)
					targetValue = string.Format(item.Format, sourceData);
			}

			return targetValue;
		}

		/// <summary>
		/// ���ÿؼ�����
		/// </summary>
		/// <param name="dataPropertyInfo"></param>
		/// <param name="data"></param>
		/// <param name="control"></param>
		/// <param name="item"></param>
		/// <param name="targetProName"></param>
		/// <returns></returns>
		private bool SetControlsAttributesByDataType(PropertyInfo dataPropertyInfo, object data, object control, DataBindingItem item, string targetProName)
		{
			bool binded = false;
			if (data == null || (control is ListControl && data.ToString().Length <= 0))
				return binded;	//��ֹ��ListControl���п�ֵ�󶨣����´���

			if (dataPropertyInfo.PropertyType.IsEnum)
			{
				if (control is ListControl)
				{
					BindEnumToListControl(dataPropertyInfo.PropertyType, data, (ListControl)control, item);
					binded = true;
				}
				else if (control is ITextControl)
				{
					BindEnumToTextControl(data, (ITextControl)control, item);
					binded = true;
				}
			}

			//���⴦������󶨵���List�ؼ���Items�����ԣ�Ӧ�ó�ѡ�У���������������ֵ��
			if (control is ListItem)
			{
				control = this.FindControl(item.ControlID);

				if (targetProName.ToLower() == "text")
				{
					foreach (ListItem litem in ((ListControl)control).Items)
					{
						if (litem.Text.ToLower() == data.ToString().ToLower())
						{
							litem.Selected = true;
						}
						else
						{
							litem.Selected = false;
						}
					}
				}
				else if (targetProName.ToLower() == "value")
				{
					((ListControl)control).SelectedValue = data.ToString();
				}
				binded = true;
			}

			return binded;
		}

		/// <summary>
		/// ���ݿͻ�����֤��ʼ��
		/// </summary>
		private void MappingDataToValidate()
		{
			foreach (DataBindingItem item in this.ItemBindings)
			{
				ClientVdtData cdtData = new ClientVdtData();
				cdtData.IsValidateOnSubmit = this.IsValidateOnSubmit;
				cdtData.FormatString = item.Format;
				cdtData.ValidationGroup = item.ValidationGroup;
				cdtData.ClientIsHtmlElement = item.ClientIsHtmlElement;

				Control vdtControl = this.FindControlByPath(this, item.ControlID);

				if (!this.CheckDataBingdingItem(item, vdtControl))
					continue;

				PropertyInfo propertyinfo = this.data.GetType().GetProperty(item.DataPropertyName);
				//�����޸ģ�2011�꽨���ڡ�����
				if (propertyinfo == null && this.IsValidateChildren)
				{
					string[] splitstr = item.DataPropertyName.Split('.');
					Type t = this.Data.GetType();

					for (int i = 0; i < splitstr.Length; i++)
					{
						propertyinfo = t.GetProperty(splitstr[i]);
						if (propertyinfo != null)
						{
							t = propertyinfo.PropertyType;
						}
					}
				}

				if (propertyinfo == null)
					continue;

				object[] vobjects = propertyinfo.GetCustomAttributes(typeof(ValidatorCompositionAttribute), true);
				cdtData.ControlID = vdtControl.ClientID;

				if (vobjects.Length > 0)
				{
					cdtData.IsAnd =
					((ValidatorCompositionAttribute)vobjects[0]).CompositionType == CompositionType.And ? true : false;
				}

				//�ж���������
				if (propertyinfo.PropertyType == typeof(int) || propertyinfo.PropertyType == typeof(Int64))
				{
					cdtData.IsOnlyNum = true;
				}
				else if (propertyinfo.PropertyType == typeof(double) || propertyinfo.PropertyType == typeof(float) || propertyinfo.PropertyType == typeof(decimal))
				{
					cdtData.IsFloat = true;
				}

				cdtData.ValidateProp = item.ClientPropName.Length > 0 ? item.ClientPropName : "value";

				if (propertyinfo.PropertyType == typeof(string))
					cdtData.IsValidateOnBlur = false;	//�ַ�����ֹʹ�ÿͻ��˽����ƿ���У�飬����޸�
				else
					cdtData.IsValidateOnBlur = item.IsValidateOnBlur;

				vobjects = propertyinfo.GetCustomAttributes(typeof(ValidatorAttribute), true);

				if (vobjects.Length <= 0 && !cdtData.IsFloat && !cdtData.IsOnlyNum)
					continue;

				foreach (ValidatorAttribute info in vobjects)
				{
					var targetType = info.GetType();
					var curValidator = info.CreateValidator(targetType, targetType.ReflectedType);
					if (curValidator is IClientValidatable)//add By Fenglilei,2011/11/7,modified by Fenglilei,2012/2/27
					{
						ClientVdtAttribute cvArt = new ClientVdtAttribute(info, propertyinfo);
						cdtData.CvtList.Add(cvArt);

						if (!string.IsNullOrEmpty(cvArt.ClientValidateMethodName))
						{
							Page.ClientScript.RegisterStartupScript(this.GetType(),
																		cvArt.ClientValidateMethodName,
																		((IClientValidatable)curValidator).
																			GetClientValidateScript(), true);
							cvArt.AdditionalData =
								((IClientValidatable)curValidator).GetClientValidateAdditionalData(propertyinfo);
						}
					}
				}

				//this.ControlUserInput(vdtControl, cdtData, item);//�»��ƣ��ͻ��˲���Ҫ֪��У���������ͣ��������޸�

				//var pType = propertyinfo.PropertyType;
				//if (item.Format.Length > 0 && (pType == typeof(System.Decimal) || pType == typeof(System.Int32) ||
				//    pType == typeof(System.Double) || pType == typeof(System.Single)))
				//{
				//    cdtData.FormatString = item.Format;
				//}

				this.validatorAttributes.Add(cdtData);
			}
		}

		/// <summary>
		/// ���DataBingdingData
		/// </summary>
		/// <param name="item"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		private bool CheckDataBingdingItem(DataBindingItem item, Control control)
		{
			if (control == null)
				return false;//���Ҳ����ؼ�����

			if (control is WebControl)
			{
				if (!((WebControl)control).Enabled) return false;//����ؼ���Ч����
			}

			PropertyInfo propertyinfo = TypePropertiesCacheQueue.Instance.GetPropertyInfoDirectly(control.GetType(), "ReadOnly");

			if (propertyinfo == null)
				return true;//����ؼ�û��ReadOnly���Է�����֤

			if (((bool)propertyinfo.GetValue(control, null)))
				return false; //����ؼ�ֻ����������

			if (!item.IsValidate)
				return false;//��У������

			return true;
		}

		///// <summary>
		///// �����û�����
		///// </summary>
		///// <param name="control">�ؼ�</param>
		///// <param name="cdtData">��֤����</param>
		//private void ControlUserInput(Control control, ClientVdtData cdtData, DataBindingItem bItem)
		//{
		//    //��󳤶�����
		//    if (control is TextBox)
		//    {
		//        foreach (ClientVdtAttribute item in cdtData.CvtList)
		//        {
		//            //������󳤶�
		//            if (item.DType == ValidteDataType.Length)
		//            {
		//                TextBox tb = (TextBox)control;
		//                tb.MaxLength = int.Parse(item.UpperBound);
		//                //tb.Attributes.Add("onchange", "if (this.maxLength) {while(value.replace(/[^\x00-\xff]/g,'*').length > this.maxLength) value = value.slice(0,-1);}");
		//            }

		//            //���ִ���
		//            if (bItem.Format.Length > 0 && (item.DType == ValidteDataType.Int || item.DType == ValidteDataType.Float))
		//            {
		//                cdtData.FormatString = bItem.Format;
		//            }
		//        }
		//    }
		//}

		#endregion

		#region Helper
		/// <summary>
		/// �Ƿ�����������
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private bool IsNumericType(System.Type type)
		{
			bool result = false;

			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					result = true;
					break;
			}

			return result;
		}

		/// <summary>
		/// ��ȡ�����������ơ����originalNameΪ�գ����ض����DefaultProperty
		/// </summary>
		/// <param name="data">����Դ</param>
		/// <param name="originalName">����</param>
		/// <returns></returns>
		private static string GetObjectPropertyName(Type dataType, string originalName)
		{
			string result = originalName;

			if (string.IsNullOrEmpty(originalName) == true)
			{
				DefaultPropertyAttribute attr = AttributeHelper.GetCustomAttribute<DefaultPropertyAttribute>(dataType);

				if (attr != null)
					result = attr.Name;
			}

			return result;
		}

		/// <summary>
		/// ��ö�����ݰ����б��Ϳؼ�
		/// </summary>
		/// <param name="enumType">ö������</param>
		/// <param name="data">��ǰ����</param>
		/// <param name="ddl">�����ؼ�</param>
		/// <param name="item">������</param>
		private void BindEnumToListControl(System.Type enumType, object data, ListControl lControl, DataBindingItem item)
		{
			if (item.EnumAutoBinding)
			{
				EnumItemDescriptionList listEnum = EnumItemDescriptionAttribute.GetDescriptionList(enumType);

				List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();

				foreach (EnumItemDescription enumItem in listEnum)
				{
					KeyValuePair<int, string> kp = new KeyValuePair<int, string>(enumItem.EnumValue,
						string.IsNullOrEmpty(enumItem.Description) ? enumItem.Name : enumItem.Description);

					list.Add(kp);
				}

				lControl.DataSource = BuildEnumList(enumType, item.EnumUsage);

				lControl.DataValueField = "Key";
				lControl.DataTextField = "Value";
				lControl.DataBind();
			}

			int intData = Convert.ToInt32(data);

			if ((lControl is CheckBoxList) && FlagsAttribute.IsDefined(enumType, typeof(FlagsAttribute)))
			{
				foreach (ListItem lItem in lControl.Items)
				{
					int v = int.Parse(lItem.Value);

					if ((v & intData) != 0)
						lItem.Selected = true;
				}
			}
			else
			{
				string strValue = intData.ToString();

				foreach (ListItem lItem in lControl.Items)
				{
					if (lItem.Value == strValue)
					{
						lControl.SelectedValue = strValue;
						break;
					}
				}
			}
		}

		/// <summary>
		/// ��ö��
		/// </summary>
		/// <param name="enumType">ö������</param>
		/// <param name="enumUsage">ö��ʹ�÷�ʽ</param>
		/// <returns></returns>
		private IList BuildEnumList(System.Type enumType, EnumUsageTypes enumUsage)
		{
			IList result = null;

			EnumItemDescriptionList listEnum = EnumItemDescriptionAttribute.GetDescriptionList(enumType);

			if (enumUsage == EnumUsageTypes.UseEnumValue)
			{
				List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();

				foreach (EnumItemDescription enumItem in listEnum)
				{
					KeyValuePair<int, string> kp = new KeyValuePair<int, string>(enumItem.EnumValue,
						string.IsNullOrEmpty(enumItem.Description) ? enumItem.Name : enumItem.Description);

					list.Add(kp);
				}

				result = list;
			}
			else
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

				foreach (EnumItemDescription enumItem in listEnum)
				{
					KeyValuePair<string, string> kp = new KeyValuePair<string, string>(enumItem.Name,
						string.IsNullOrEmpty(enumItem.Description) ? enumItem.Name : enumItem.Description);

					list.Add(kp);
				}

				result = list;
			}

			return result;
		}

		/// <summary>
		/// ��ö�����ݰ����ı��Ϳؼ�
		/// </summary>
		/// <param name="enumType">ö������</param>
		/// <param name="data">��ǰ����</param>
		/// <param name="lab">�ؼ�</param>
		/// <param name="item">������</param>
		private void BindEnumToTextControl(object data, ITextControl tContrl, DataBindingItem item)
		{
			if (item.EnumAutoBinding)
			{
				string description = EnumItemDescriptionAttribute.GetDescription((Enum)data);

				string defaultDataValue = string.Empty;

				switch (item.EnumUsage)
				{
					case EnumUsageTypes.UseEnumValue:
						defaultDataValue = ((int)data).ToString();
						break;
					case EnumUsageTypes.UseEnumString:
						defaultDataValue = data.ToString();
						break;
				}

				tContrl.Text = string.IsNullOrEmpty(description) ? defaultDataValue : description;
			}
		}


		/// <summary>
		/// �����У���������
		/// </summary>
		public void ValidateData()
		{
			this.CollectData();
		}

		/// <summary>
		/// �����У���������
		/// </summary>
		/// <param name="objdata">���ݶ���</param>
		/// <param name="unValidates"></param>
		/// <param name="msg">����ش���Ϣ</param>
		/// <returns>�Ƿ���֤�ɹ�</returns>
		private bool ValidateData(object objdata, DataBindingItemCollection unValidates, out string msg)
		{
			bool isval = false;

			msg = string.Empty;

			StringBuilder validMsgs = new StringBuilder();
			List<string> unValidatePropName = new List<string>();

			foreach (DataBindingItem bItem in unValidates)
			{
				unValidatePropName.Add(bItem.DataPropertyName);
			}

			if (ValidateUnbindProperties == false)
			{
				PropertyInfo[] pInfos = objdata.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

				foreach (PropertyInfo pi in pInfos)
				{
					if (this.ItemBindings.Exists(item => item.DataPropertyName == pi.Name) == false)
						unValidatePropName.Add(pi.Name);
				}
			}

			Validator validator = ValidationFactory.CreateValidator(objdata.GetType(), unValidatePropName);
			ValidationResults valResults = validator.Validate(objdata);
			isval = valResults.IsValid();

			foreach (ValidationResult vItem in valResults)
			{
				msg = vItem.Tag == null ? vItem.Key : vItem.Tag;

				if (msg != null && msg.Length > 0)
				{
					msg += "��";
				}

				validMsgs.Append(msg + vItem.Message + "\r\n");

				if (vItem.NestedValidationResults != null)
				{
					foreach (ValidationResult vSubItem in vItem.NestedValidationResults)
					{
						msg = vSubItem.Tag == null ? vSubItem.Key : vSubItem.Tag;

						if (msg != null && msg.Length > 0)
							msg += "��";

						validMsgs.Append(msg + vSubItem.Message + "\r\n");
					}
				}
			}

			isval = DoExValidate(validMsgs, isval);
			msg = validMsgs.ToString();

			return isval;
		}

		/// <summary>
		/// ִ����չ��֤
		/// </summary>
		/// <param name="validMsgs">��֤��Ϣ</param>
		/// <param name="isval">��֤�����ʼֵ</param>
		/// <returns></returns>
		private bool DoExValidate(StringBuilder validMsgs, bool isval)
		{
			string msg = string.Empty;
			if (this.OnValidateDatas.Count > 0)
			{
				foreach (ValidateData validatedata in this.OnValidateDatas)
				{
					ValidationResult vItem = validatedata(this.data, null);
					if (vItem != null)
					{
						msg = vItem.Tag == null ? vItem.Key : vItem.Tag;
						if (msg != null && msg.Length > 0)
						{
							msg += "��";
						}
						validMsgs.Append(msg + vItem.Message + "\r\n");
						if (vItem.NestedValidationResults != null)
						{
							foreach (ValidationResult vSubItem in vItem.NestedValidationResults)
							{
								msg = vSubItem.Tag == null ? vSubItem.Key : vSubItem.Tag;
								if (msg != null && msg.Length > 0)
								{
									msg += "��";
								}
								validMsgs.Append(msg + vSubItem.Message + "\r\n");
							}
						}
						isval = false;
					}
				}
			}
			return isval;
		}

		/// <summary>
		/// ���ݿؼ�·�����ҿؼ�
		/// </summary>
		/// <param name="rootControl">���ؼ�</param>
		/// <param name="idPath">�ؼ�·��</param>
		/// <returns>�����ҵĿؼ�</returns>
		private Control FindControlByPath(Control rootControl, string idPath)
		{
			int depth = 0;
			idPath = idPath.Trim();

			if (idPath.Length > 0)
			{
				string[] idPaths = idPath.Split('.');
				while (idPaths.Length > depth)
				{
					rootControl = rootControl.FindControl(idPaths[depth]);
					depth++;
				}
			}

			return rootControl;
		}

		/// <summary>
		/// ���ݶ�������·����������
		/// </summary>
		/// <param name="sourceData"></param>
		/// <param name="proPath"></param>
		/// <returns></returns>
		internal static object FindObjectByPath(object sourceData, string proPath)
		{
			int depth = 0;

			proPath = proPath.Trim();

			if (proPath.Length > 0 && sourceData != null)
			{
				string[] proPaths = proPath.Split('.');

				while (proPaths.Length > depth)
				{
					sourceData = TypePropertiesCacheQueue.Instance.GetObjectPropertyValue(
										sourceData,
										GetObjectPropertyName(sourceData.GetType(), proPaths[depth]));
					depth++;
				}
			}

			return sourceData;
		}

		/// <summary>
		/// ���·��
		/// </summary>
		/// <param name="dataSource">���ݶ���</param>
		/// <param name="source">ԭʼ��������(·��)</param>
		/// <param name="objPath">����·��</param>
		/// <param name="proPath">��������</param>
		internal static void SplitPath(object dataSource, string source, out string objPath, out string proPath)
		{
			//����·��
			objPath = string.Empty;
			//��������
			proPath = source;

			int lastDot = source.LastIndexOf('.');

			if (lastDot >= 0)
			{
				objPath = source.Substring(0, lastDot);
				proPath = source.Substring(lastDot + 1,
					source.Length - lastDot - 1);
			}

			//�����ȡ�������ԣ�����ΪĬ�����ԡ�
			if (proPath.Length <= 0 && dataSource != null)
				proPath = GetObjectPropertyName(dataSource.GetType(), proPath);
		}

		/// <summary>
		/// ����ͻ��˽���󶨿ؼ���
		/// </summary>
		private void RegCheckScript()
		{
			if (this.IsValidateOnSubmit)
			{
				CustomValidator vcontrol = new CustomValidator();
				vcontrol.ClientValidationFunction = "$HBRootNS.DataBindingControl.checkBindingControlData";
				this.Controls.Add(vcontrol);
			}
		}

		#endregion

		private static IWfProcess CurrentProcess
		{
			get
			{
				//return (IWfProcess)HttpContext.Current.Items["CurrentProcess"];
				IWfProcess process = null;

				if (WfClientContext.Current.OriginalActivity != null)
					process = WfClientContext.Current.OriginalActivity.Process;

				return process;
			}
			//set
			//{
			//    HttpContext.Current.Items["CurrentProcess"] = value;
			//}
		}

		public void CollectApplicationData(IWfProcess process)
		{
			/*
            CurrentProcess = process;

            MappingControlsToData(null, this.ItemBindings);

            CurrentProcess = null;
			*/
		}

		private void CollectApplicationData()
		{
			/*
            if (WfClientContext.Current.OriginalActivity != null)
            {
                CollectApplicationData(WfClientContext.Current.OriginalActivity.Process.ApprovalRootProcess);
            }*/
		}
	}
}
