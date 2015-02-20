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
		/// 收集数据
		/// </summary>
		public void CollectData()
		{
			CollectData(AutoValidate);
		}

		/// <summary>
		/// 收集数据，仅校验指定的组
		/// </summary>
		/// <param name="validationGroup">指定的组号，如果小于0，则为全部校验</param>
		public void CollectData(int validationGroup)
		{
			ExceptionHelper.FalseThrow(this.data != null, "绑定数据源为空，Data属性不能为null。");

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
		/// 收集数据
		/// </summary>
		/// <param name="autoValidate">是否自动校验</param>
		public void CollectData(bool autoValidate)
		{
			ExceptionHelper.FalseThrow(this.data != null, "绑定数据源为空，Data属性不能为null。");

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
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "输入非法");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "整数部分的位数不能超过{0}位");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "小数部分的位数不能超过{0}位");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "数字必须在{0}和{1}之间");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "数字必须大于等于{0}");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "数字必须小于等于{0}");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "数字类型只能有一个小数点");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "数字类型只能有一个'{0}'");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "必须输入合法的数字");

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
		/// 外部验证集合
		/// </summary>
		public List<ValidateData> OnValidateDatas = new List<ValidateData>();

		[PersistenceMode(PersistenceMode.InnerProperty), Description("绑定数据的映射")]
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
		/// 自动将数据绑定到控件
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
		/// 收集数据时自动校验获取到对象上的数据
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
		/// 是否校验子对象
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
		/// PostBack后，是否自动收集数据（不校验）。此操作发生在Load和控件事件(Button Click)之间
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
		/// 是否允许客户端收集数据。决定了是否下载BindingItems到客户端
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
		/// 自动校验获取到对象上的数据
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
		/// 是否校验没有绑定的属性
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
		/// 控件创建对象
		/// </summary>
		/// <param name="sourceData">数据源</param>
		/// <param name="items">绑定定义</param>
		/// <returns>不做校验的数据</returns>
		private DataBindingItemCollection MappingControlsToData(object sourceData, DataBindingItemCollection items)
		{
			DataBindingItemCollection unValidates = new DataBindingItemCollection();

			foreach (DataBindingItem item in items)
			{
				if (!item.IsValidate)
					unValidates.Add(item);

				if ((item.Direction & BindingDirection.ControlToData) != BindingDirection.None)
				{
					//对象路径
					string targetPro = string.Empty;
					object subData;
					//对象属性
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
		/// 获取单个对象属性
		/// </summary>
		/// <param name="dataPropertyInfo">属性</param>
		/// <param name="currData">当前数据</param>
		/// <param name="dataTargetProName">目标属性名称</param>
		/// <param name="item">绑定项</param>
		private void MappingOneControlToOneData(object currData, string dataTargetProName, DataBindingItem item)
		{
			Control targetControl = FindControlByPath(this, item.ControlID);

			//控件对象路径
			string targetPro = string.Empty;
			//控件对象属性
			string targetProName = item.ControlPropertyName;
			ExceptionHelper.FalseThrow(targetControl != null, "不能找到ID为{0}的控件", item.ControlID);

			SplitPath(targetControl, item.ControlPropertyName, out targetPro, out targetProName);

			Type dataType = this.DataType;

			if (currData != null)
				dataType = currData.GetType();

			//获取目标属性值
			object targetItem = FindObjectByPath(targetControl, targetPro);
			if (targetItem != null && dataType != null)
			{
				PropertyInfo piDest = TypePropertiesCacheQueue.Instance.GetPropertyInfo(dataType,
						GetObjectPropertyName(dataType, dataTargetProName));

				if (piDest != null)
				{
					if (targetItem is CheckBoxList && piDest.PropertyType.IsEnum && FlagsAttribute.IsDefined(piDest.PropertyType, typeof(FlagsAttribute)))
					{
						//Flags型的枚举和CheckBox之间的特殊处理
						CollectDataFromCheckBoxListToFlagsEnumProperty(item, ((ListControl)targetItem).Items, piDest, currData);
					}
					else
					{
						//获取绑定属性上的信息，绑定对象属性
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
		/// 反向格式化数据
		/// </summary>
		/// <param name="sourceData">数据源</param>
		/// <param name="piDest">属性</param>
		/// <param name="item">绑定项</param>
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
		/// 控件中数据是不是为空
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
		/// 将数据绑定至控件
		/// </summary>
		/// <param name="sourceData">数据源</param>
		/// <param name="items">数据项</param>
		private void MappingDataToControls(object sourceData, DataBindingItemCollection items)
		{
			if (sourceData != null)
			{
				foreach (DataBindingItem item in items)
				{
					if ((item.Direction & BindingDirection.DataToControl) != BindingDirection.None)
					{
						//对象路径
						string targetPro = string.Empty;
						object subData;

						//对象属性
						string targetProName = item.DataPropertyName;

						SplitPath(sourceData, item.DataPropertyName, out targetPro, out targetProName);

						//获取路径上的对象

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
		/// 绑定单个控件
		/// </summary>
		/// <param name="dataPropertyInfo">属性</param>
		/// <param name="currData">当前数据</param>
		/// <param name="dataTargetProName">目标属性名称</param>
		/// <param name="item">绑定项</param>
		private void MappingOneDataToOneControl(PropertyInfo dataPropertyInfo, object currData, string dataTargetProName, DataBindingItem item)
		{
			if (currData != null)
			{
				Control targetControl = FindControlByPath(this, item.ControlID);

				//控件对象路径
				string targetPro = string.Empty;
				//控件对象属性
				string targetProName = item.ControlPropertyName;
				ExceptionHelper.FalseThrow(targetControl != null, "不能找到ID为{0}的控件", item.ControlID);

				SplitPath(targetControl, item.ControlPropertyName, out targetPro, out targetProName);

				//获取目标属性值
				object targetItem = FindObjectByPath(targetControl, targetPro);
				if (targetItem != null)
				{
					//获取目标
					PropertyInfo piDest = TypePropertiesCacheQueue.Instance.GetPropertyInfo(targetItem.GetType(),
							GetObjectPropertyName(targetItem.GetType(), targetProName));

					if (piDest != null && piDest.CanWrite)
					{
						//获取绑定属性上的信息，绑定控件属性
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
		/// 格式化数据
		/// </summary>
		/// <param name="sourceData">数据源</param>
		/// <param name="piDest">属性</param>
		/// <param name="item">绑定项</param>
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

				//除掉else，只要是设置了格式化，就应该格式化数据。 
				if (!isDefault && string.IsNullOrEmpty(item.Format) == false)
					targetValue = string.Format(item.Format, sourceData);
			}

			return targetValue;
		}

		/// <summary>
		/// 设置控件属性
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
				return binded;	//防止对ListControl进行空值绑定，导致错误。

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

			//特殊处理：如果绑定的是List控件的Items的属性，应置成选中，而不是设置它的值。
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
		/// 数据客户端验证初始化
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
				//徐磊修改，2011年建军节。。。
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

				//判断数据类型
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
					cdtData.IsValidateOnBlur = false;	//字符串禁止使用客户端焦点移开的校验，沈峥修改
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

				//this.ControlUserInput(vdtControl, cdtData, item);//新机制，客户端不需要知道校验数据类型，冯立雷修改

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
		/// 检测DataBingdingData
		/// </summary>
		/// <param name="item"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		private bool CheckDataBingdingItem(DataBindingItem item, Control control)
		{
			if (control == null)
				return false;//查找不到控件跳过

			if (control is WebControl)
			{
				if (!((WebControl)control).Enabled) return false;//如果控件无效跳过
			}

			PropertyInfo propertyinfo = TypePropertiesCacheQueue.Instance.GetPropertyInfoDirectly(control.GetType(), "ReadOnly");

			if (propertyinfo == null)
				return true;//如果控件没有ReadOnly属性返回验证

			if (((bool)propertyinfo.GetValue(control, null)))
				return false; //如果控件只读，跳过。

			if (!item.IsValidate)
				return false;//不校验跳过

			return true;
		}

		///// <summary>
		///// 控制用户输入
		///// </summary>
		///// <param name="control">控件</param>
		///// <param name="cdtData">验证数据</param>
		//private void ControlUserInput(Control control, ClientVdtData cdtData, DataBindingItem bItem)
		//{
		//    //最大长度限制
		//    if (control is TextBox)
		//    {
		//        foreach (ClientVdtAttribute item in cdtData.CvtList)
		//        {
		//            //设置最大长度
		//            if (item.DType == ValidteDataType.Length)
		//            {
		//                TextBox tb = (TextBox)control;
		//                tb.MaxLength = int.Parse(item.UpperBound);
		//                //tb.Attributes.Add("onchange", "if (this.maxLength) {while(value.replace(/[^\x00-\xff]/g,'*').length > this.maxLength) value = value.slice(0,-1);}");
		//            }

		//            //数字处理
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
		/// 是否是数字类型
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
		/// 获取对象属性名称。如果originalName为空，返回对象的DefaultProperty
		/// </summary>
		/// <param name="data">数据源</param>
		/// <param name="originalName">名称</param>
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
		/// 将枚举数据绑定至列表型控件
		/// </summary>
		/// <param name="enumType">枚举类型</param>
		/// <param name="data">当前数据</param>
		/// <param name="ddl">下拉控件</param>
		/// <param name="item">绑定数据</param>
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
		/// 绑定枚举
		/// </summary>
		/// <param name="enumType">枚举类型</param>
		/// <param name="enumUsage">枚举使用方式</param>
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
		/// 将枚举数据绑定至文本型控件
		/// </summary>
		/// <param name="enumType">枚举类型</param>
		/// <param name="data">当前数据</param>
		/// <param name="lab">控件</param>
		/// <param name="item">绑定数据</param>
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
		/// 服务端校验对象数据
		/// </summary>
		public void ValidateData()
		{
			this.CollectData();
		}

		/// <summary>
		/// 服务端校验对象数据
		/// </summary>
		/// <param name="objdata">数据对象</param>
		/// <param name="unValidates"></param>
		/// <param name="msg">错误回传消息</param>
		/// <returns>是否验证成功</returns>
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
					msg += "：";
				}

				validMsgs.Append(msg + vItem.Message + "\r\n");

				if (vItem.NestedValidationResults != null)
				{
					foreach (ValidationResult vSubItem in vItem.NestedValidationResults)
					{
						msg = vSubItem.Tag == null ? vSubItem.Key : vSubItem.Tag;

						if (msg != null && msg.Length > 0)
							msg += "：";

						validMsgs.Append(msg + vSubItem.Message + "\r\n");
					}
				}
			}

			isval = DoExValidate(validMsgs, isval);
			msg = validMsgs.ToString();

			return isval;
		}

		/// <summary>
		/// 执行扩展验证
		/// </summary>
		/// <param name="validMsgs">验证信息</param>
		/// <param name="isval">验证结果初始值</param>
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
							msg += "：";
						}
						validMsgs.Append(msg + vItem.Message + "\r\n");
						if (vItem.NestedValidationResults != null)
						{
							foreach (ValidationResult vSubItem in vItem.NestedValidationResults)
							{
								msg = vSubItem.Tag == null ? vSubItem.Key : vSubItem.Tag;
								if (msg != null && msg.Length > 0)
								{
									msg += "：";
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
		/// 根据控件路径查找控件
		/// </summary>
		/// <param name="rootControl">父控件</param>
		/// <param name="idPath">控件路径</param>
		/// <returns>所查找的控件</returns>
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
		/// 根据对象属性路径查找数据
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
		/// 拆分路径
		/// </summary>
		/// <param name="dataSource">数据对象</param>
		/// <param name="source">原始属性名称(路径)</param>
		/// <param name="objPath">对象路径</param>
		/// <param name="proPath">对象属性</param>
		internal static void SplitPath(object dataSource, string source, out string objPath, out string proPath)
		{
			//对象路径
			objPath = string.Empty;
			//对象属性
			proPath = source;

			int lastDot = source.LastIndexOf('.');

			if (lastDot >= 0)
			{
				objPath = source.Substring(0, lastDot);
				proPath = source.Substring(lastDot + 1,
					source.Length - lastDot - 1);
			}

			//如果获取不到属性，设置为默认属性。
			if (proPath.Length <= 0 && dataSource != null)
				proPath = GetObjectPropertyName(dataSource.GetType(), proPath);
		}

		/// <summary>
		/// 输出客户端交验绑定控件。
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
