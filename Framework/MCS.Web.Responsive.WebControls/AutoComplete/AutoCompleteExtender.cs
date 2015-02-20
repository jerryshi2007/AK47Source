#region
// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	AutoCompleteExtender.cs
// Remark	：  自动完成的服务器端代码
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		张曦	    20070815		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Reflection;
using System.Web.UI.WebControls.WebParts;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library.Script;

[assembly: WebResource("MCS.Web.Responsive.WebControls.AutoComplete.AutoCompleteExtender.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.AutoComplete.AutoCompleteExtender.css", "text/css")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.AutoComplete.check.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.AutoComplete.hourglass.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.AutoComplete.ou.gif", "image/gif")]

namespace MCS.Web.Responsive.WebControls
{
	/// <summary>
	/// 自动完成的服务器端代码
	/// </summary>
	/// <remarks>
	/// 自动完成的服务器端代码
	///		将各种属性输出到客户端，并实现服务器端事件
	/// </remarks>
	[RequiredScript(typeof(ControlBaseScript))]
	[Designer(typeof(AutoCompleteExtenderDesigner))]
	[TargetControlType(typeof(Control))]
    [ClientCssResource("MCS.Web.Responsive.WebControls.AutoComplete.AutoCompleteExtender.css")]
    [ClientScriptResource("MCS.Web.WebControls.AutoCompleteExtender", "MCS.Web.Responsive.WebControls.AutoComplete.AutoCompleteExtender.js")]
	public class AutoCompleteExtender : DataBoundExtenderControl
	{
		/// <summary>
		/// 回调的委托定义
		/// </summary>
		/// <remarks>
		/// 回调的委托定义
		/// </remarks>
		/// <param name="sPrefix">前缀</param>
		/// <param name="iCount">最大记录数</param>
		/// <param name="eventContext">控件回调时的上下文</param>
		/// <param name="result">返回的结果</param>
		public delegate void GetDataSourceDelegate(string sPrefix, int iCount, object eventContext, ref System.Collections.IEnumerable result);

		/// <summary>
		/// 回调的事件定义
		/// </summary>
		/// <remarks>
		/// 回调的事件定义
		/// </remarks>
		public event GetDataSourceDelegate GetDataSource;

		#region Field

		private object eventContext = null;

		/// <summary>
		/// 当Word文档中创建完毕后，触发的客户端事件
		/// </summary>
		/// <remarks>
		/// 当Word文档中创建完毕后，触发的客户端事件
		/// </remarks>
		[DefaultValue("")]
		[Bindable(true), Category("ClientEventsHandler"), Description("当选择了一个项目后，触发的客户端事件")]
		[ClientPropertyName("itemSelected")]// 对应的客户端属性
		[ScriptControlEvent]
		public string OnItemSelected
		{
			get { return this.GetPropertyValue<string>("OnItemSelected", string.Empty); }
			set { this.SetPropertyValue<string>("OnItemSelected", value); }
		}

		/// <summary>
		/// 校验后，触发的客户端事件
		/// </summary>
		[DefaultValue("")]
		[Bindable(true), Category("ClientEventsHandler"), Description("校验后，触发的客户端事件")]
		[ClientPropertyName("valueValidated")]// 对应的客户端属性
		[ScriptControlEvent]
		public string OnValueValidated
		{
			get { return this.GetPropertyValue<string>("OnValueValidated", string.Empty); }
			set { this.SetPropertyValue<string>("OnValueValidated", value); }
		}

		/// <summary>
		/// 校验后，触发的客户端事件
		/// </summary>
		[DefaultValue("")]
		[Bindable(true), Category("ClientEventsHandler"), Description("客户端发起回调之前")]
		[ClientPropertyName("beforeInvoke")]// 对应的客户端属性
		[ScriptControlEvent]
		public string OnClientBeforeInvoke
		{
			get { return this.GetPropertyValue<string>("OnClientBeforeInvoke", string.Empty); }
			set { this.SetPropertyValue<string>("OnClientBeforeInvoke", value); }
		}

		/// <summary>
		/// 是否打开自动完成功能
		/// </summary>
		/// <remarks>
		/// 是否打开自动完成功能
		///     True:打开
		///     False:关闭
		/// </remarks>
		[DefaultValue(true)]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("isAutoComplete")]// 对应的客户端属性
		public bool IsAutoComplete
		{
			get { return this.GetPropertyValue("IsAutoComplete", true); }
			set { this.SetPropertyValue("IsAutoComplete", value); }
		}

		/// <summary>
		/// 是否切换焦点时自动校验
		/// </summary>
		/// <remarks>
		/// 是否切换焦点时自动校验
		///     True:打开
		///     False:关闭
		/// </remarks>
		[DefaultValue(true)]
		[ScriptControlProperty]
		[ClientPropertyName("autoValidateOnChange")]// 对应的客户端属性
		public bool AutoValidateOnChange
		{
			get { return this.GetPropertyValue("AutoValidateOnChange", true); }
			set { this.SetPropertyValue("AutoValidateOnChange", value); }
		}

		/// <summary>
		/// 输入多少个字符后开始自动完成，默认为3
		/// </summary>
		/// <remarks>
		/// 输入多少个字符后开始自动完成，默认为3
		/// </remarks>
		[DefaultValue(3)]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("minimumPrefixLength")]// 对应的客户端属性
		public int MinimumPrefixLength
		{
			get { return this.GetPropertyValue("MinimumPrefixLength", 3); }
			set { this.SetPropertyValue("MinimumPrefixLength", value); }
		}

		/// <summary>
		/// 自动完成间隔。默认1000毫秒。输入停止后多长时间开始自动完成，单位：毫秒。1000毫秒=1秒
		/// </summary>
		/// <remarks>
		/// 自动完成间隔。默认1000毫秒。输入停止后多长时间开始自动完成，单位：毫秒。1000毫秒=1秒
		/// </remarks>
		[DefaultValue(1000)]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("completionInterval")]// 对应的客户端属性
		public int CompletionInterval
		{
			get { return this.GetPropertyValue("CompletionInterval", 1000); }
			set { this.SetPropertyValue("CompletionInterval", value); }
		}

		/// <summary>
		/// 鼠标移动到自动完成的项目上的CssClass
		/// </summary>
		/// <remarks>
		/// 鼠标移动到自动完成的项目上的CssClass
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("itemHoverCssClass")]
		public string ItemHoverCssClass
		{
			get { return this.GetPropertyValue("ItemHoverCssClass", string.Empty); }
			set { this.SetPropertyValue("ItemHoverCssClass", string.Empty); }
		}

		/// <summary>
		/// 当启用输入验证，并且输入的内容无法完全匹配到数据源中的某一项后的CssClass
		/// </summary>
		/// <remarks>
		/// 当启用输入验证，并且输入的内容无法完全匹配到数据源中的某一项后的CssClass
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("errorCssClass")]
		public string ErrorCssClass
		{
			get { return this.GetPropertyValue("ErrorCssClass", string.Empty); }
			set { this.SetPropertyValue("ErrorCssClass", value); }
		}

		/// <summary>
		/// 自动完成的项目CssClass
		/// </summary>
		/// <remarks>
		/// 自动完成的项目CssClass
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("itemCssClass")]
		public string ItemCssClass
		{
			get { return this.GetPropertyValue("ItemCssClass", string.Empty); }
			set { this.SetPropertyValue("ItemCssClass", value); }
		}

		/// <summary>
		/// 控件是否启用验证,如果启用则在输入内容无法完全匹配到数据源中的某一项
		/// 后应用ErrorStyle到控件
		/// </summary>
		/// <remarks>
		/// 控件是否启用验证,如果启用则在输入内容无法完全匹配到数据源中的某一项
		/// 后应用ErrorStyle到控件
		///     True:启用
		///     False:关闭
		/// </remarks>
		[DefaultValue(false)]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("requireValidation")]// 对应的客户端属性
		public bool RequireValidation
		{
			get { return this.GetPropertyValue("RequireValidation", false); }
			set { this.SetPropertyValue("RequireValidation", value); }
		}

		/// <summary>
		/// 事件回调时的Context
		/// </summary>
		[DefaultValue(false)]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("eventContext")]// 对应的客户端属性
		public object EventContext
		{
			get { return this.eventContext; }
			set { this.eventContext = value; }
		}

		/// <summary>
		/// 控件自动完成出来的列表中显示的最大记录数量。
		///     默认为-1，表示全部数据
		/// </summary>
		/// <remarks>
		/// 控件自动完成出来的列表中显示的最大记录数量。
		///     默认为-1，表示全部数据
		/// </remarks>
		[DefaultValue(-1)]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("maxCompletionRecordCount")]// 对应的客户端属性
		public int MaxCompletionRecordCount
		{
			get { return this.GetPropertyValue("MaxCompletionRecordCount", -1); }
			set { this.SetPropertyValue("MaxCompletionRecordCount", value); }
		}

		/// <summary>
		/// 控件自动完成弹出的选择窗口的最大高度，默认为260px。
		///     如果记录的内容小于等于这个值，弹出窗口的高度自适应，如果大于这个值，则显示滚动条
		/// </summary>
		/// <remarks>
		/// 控件自动完成弹出的选择窗口的最大高度，默认为260px。
		///     如果记录的内容小于等于这个值，弹出窗口的高度自适应，如果大于这个值，则显示滚动条
		/// </remarks>
		[DefaultValue(260)]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("maxPopupWindowHeight")]// 对应的客户端属性
		public int MaxPopupWindowHeight
		{
			get { return this.GetPropertyValue("MaxPopupWindowHeight", 260); }
			set { this.SetPropertyValue("MaxPopupWindowHeight", value); }
		}

		/// <summary>
		/// 是否自动回调
		/// </summary>
		/// <remarks>
		/// 是否自动回调
		/// </remarks>
		[DefaultValue("")]
		[ScriptControlProperty]// 设置此属性要输出到客户端
		[ClientPropertyName("autoCallBack")]// 对应的客户端属性
		public bool AutoCallBack
		{
			get { return this.GetPropertyValue("AutoCallBack", false); }
			set { this.SetPropertyValue("AutoCallBack", value); }
		}

		/// <summary>
		/// 启用客户端缓存
		/// </summary>
		/// <remarks>
		/// 启用客户端缓存
		/// </remarks>
		[DefaultValue(true)]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("enableCaching")]//对应的客户端属性
		public bool EnableCaching
		{
			get { return this.GetPropertyValue("EnableCaching", true); }
			set { this.SetPropertyValue("EnableCaching", value); }
		}

		/// <summary>
		/// 离线数据源
		/// </summary>
		/// <remarks>
		/// 离线数据源
		/// </remarks>
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("dataList")]//对应的客户端属性
		public System.Collections.IEnumerable DataList
		{
			//get { return base.DataSourceResult; }
			get
			{
				return this.DataSourceResult;
			}
		}

		/// <summary>
		/// 提供文本显示的数据源属性集合
		/// </summary>
		/// <remarks>
		/// 提供文本显示的数据源属性集合
		/// </remarks>
		[Category("Appearance")]
		[Description("选择项文本显示属性集合")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("dataTextFieldList")]//对应的客户端属性
		public string[] DataTextFieldList
		{
			get
			{
				return this.GetPropertyValue<string[]>("DataTextFieldList", new string[] { });
			}

			set
			{
				this.SetPropertyValue<string[]>("DataTextFieldList", value);
			}
		}

		/// <summary>
		/// 在哪些字段中匹配输入的项目，只要有一个字段符合条件则认为该匹配成功
		/// </summary>
		/// <remarks>
		/// 在哪些字段中匹配输入的项目，只要有一个字段符合条件则认为该匹配成功
		/// </remarks>
		[Category("Appearance")]
		[Description("制定在哪些字段中匹配输入的项目，只要有一个字段符合条件则认为该匹配成功")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("compareFieldName")]//对应的客户端属性
		public string[] CompareFieldName
		{
			get
			{
				return this.GetPropertyValue<string[]>("CompareFieldName", new string[] { });
			}

			set
			{
				this.SetPropertyValue<string[]>("CompareFieldName", value);
			}
		}

		/// <summary>
		/// 指定项目的Value值
		/// </summary>
		/// <remarks>
		/// 指定项目的Value值
		/// </remarks>
		[Category("Appearance")]
		[Description("指定项目的Value值")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("dataValueField")]//对应的客户端属性
		public string DataValueField
		{
			get
			{
				return this.GetPropertyValue<string>("DataValueField", string.Empty);
			}

			set
			{
				this.SetPropertyValue<string>("DataValueField", value);
			}
		}

		/// <summary>
		/// 显示内容格式化字符串,用来调整显示格式
		/// </summary>
		/// <remarks>
		/// 显示内容格式化字符串,用来调整显示格式
		/// </remarks>
		[Category("Appearance")]
		[Description("指定显示内容的格式化字符串")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("dataTextFormatString")]//对应的客户端属性
		public string DataTextFormatString
		{
			get
			{
				return this.GetPropertyValue<string>("DataTextFormatString", string.Empty);
			}

			set
			{
				this.SetPropertyValue<string>("DataTextFormatString", value);
			}
		}

		/// <summary>
		/// 输入的文本
		/// </summary>
		/// <remarks>
		/// 输入的文本
		/// </remarks>
		[DefaultValue("")]
		public string Text
		{
			get { return this.GetPropertyValue<string>("Text", string.Empty); }
			set { this.SetPropertyValue<string>("Text", value); }
		}

		/// <summary>
		/// 选择机构的图标
		/// </summary>
		/// <remarks>
		/// 选择机构的图标
		/// </remarks>
		[Description("检查的图标")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("checkImage")]//对应的客户端属性
		public string CheckImage
		{
			get
			{
				return this.GetPropertyValue<string>("CheckImage",
					Page.ClientScript.GetWebResourceUrl(typeof(AutoCompleteExtender),
                        "MCS.Web.Responsive.WebControls.AutoComplete.check.gif"));
			}

			set
			{
				this.SetPropertyValue<string>("CheckImage", value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Description("等待的图标")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("waitingImage")]//对应的客户端属性
		public string WaitingImage
		{
			get
			{
				return this.GetPropertyValue<string>("WaitingImage",
					Page.ClientScript.GetWebResourceUrl(typeof(AutoCompleteExtender),
                        "MCS.Web.Responsive.WebControls.AutoComplete.hourglass.gif"));
			}

			set
			{
				this.SetPropertyValue<string>("WaitingImage", value);
			}
		}

		/// <summary>
		/// 是否显示检查的图标
		/// </summary>
		[Description("是否显示检查的图标")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("showCheckImage")]//对应的客户端属性
		public bool ShowCheckImage
		{
			get
			{
				return this.GetPropertyValue("ShowCheckImage", false);
			}

			set
			{
				this.SetPropertyValue("ShowCheckImage", value);
			}
		}

		/// <summary>
		/// 如果是选择的则保存选择的值
		/// </summary>
		/// <remarks>
		/// 如果是选择的则保存选择的值
		/// </remarks>
		[DefaultValue("")]
		public string SelectValue
		{
			get { return this.GetPropertyValue<string>("SelectValue", string.Empty); }
			set { this.SetPropertyValue<string>("SelectValue", value); }
		}

		/// <summary>
		/// 回调时的上下文，由使用者提供
		/// </summary>
		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("callBackContext")]
		[Bindable(true), Description("回调时的上下文，由使用者提供")]
		public string CallBackContext
		{
			get
			{
				return this.GetPropertyValue("callBackContext", string.Empty);
			}

			set
			{
				this.SetPropertyValue("callBackContext", value);
			}
		}
		#endregion

		/// <summary>
		/// 通过这个玩意儿回调页面上的公共方法。先通过js脚本中的invoke掉到这个，然后此方法通过应用
		/// 设置的invokeMethod指定的页面上的方法，调用页面的公共方法。
		/// </summary>
		/// <remarks>
		/// 通过这个玩意儿回调页面上的公共方法。先通过js脚本中的invoke掉到这个，然后此方法通过应用
		/// 设置的invokeMethod指定的页面上的方法，调用页面的公共方法。
		/// </remarks>
		/// <param name="sPrefix">输入的内容，作为匹配的前缀</param>
		/// <param name="iCount">获取的数据记录数量最大值</param>
		/// <returns>获取的数据</returns>
		[ScriptControlMethod]
		public System.Collections.IEnumerable CallBackPageMethod(string sPrefix, int iCount)
		{
			System.Collections.IEnumerable fenumResult = null;//返回的值

			if (this.GetDataSource != null)
				this.GetDataSource(sPrefix, iCount, this.eventContext, ref fenumResult);

			return fenumResult;
		}

		#region ClientState
		/// <summary>
		/// 加载ClientState
		/// </summary>
		/// <remarks>
		/// 加载ClientState
		///     ClientState中保存的是一个长度为2的一维数组
		///         第一个为输入框中的文本
		///         第二个为选中项目的Value，如果手工输入不是选择则为 空</remarks>
		/// <param name="clientState">序列化后的clientState</param>
		protected override void LoadClientState(string clientState)
		{
			base.LoadClientState(clientState);

			object[] foArray = JSONSerializerExecute.Deserialize<object[]>(clientState);

			if (null != foArray && foArray.Length > 0)
			{
				this.Text = foArray[0].ToString();//设置Text

				//设置Value
				if (foArray.Length > 1 && null != foArray[1])
					this.SelectValue = foArray[1].ToString();
				else
					this.SelectValue = string.Empty;

				//设置DataList
				if (foArray.Length > 2 && null != foArray[2])
					this.DataSource = (System.Collections.IEnumerable)foArray[2];
				else
					this.DataSource = null;

				if (foArray.Length > 3 && null != foArray[3])
					this.eventContext = foArray[3];
				else
					this.eventContext = null;
			}
			else
			{
				this.SelectValue = string.Empty;
				this.Text = string.Empty;
				this.DataSource = null;
				this.eventContext = null;
			}
		}

		/// <summary>
		/// 保存ClientState
		/// </summary>
		/// <remarks>
		/// 保存ClientState
		///     ClientState中保存的是一个长度为2的一维数组
		///         第一个为输入框中的文本
		///         第二个为选中项目的Value，如果手工输入不是选择则为 String.Empty
		///         第三个为DataList数据源
		/// </remarks>
		/// <returns>序列化后的CLientState字符串</returns>
		protected override string SaveClientState()
		{
			object[] foArray = new object[] { this.Text, this.SelectValue, this.DataList, this.eventContext };

			return JSONSerializerExecute.Serialize(foArray);
		}
		#endregion
	}
}