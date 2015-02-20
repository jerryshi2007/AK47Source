using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;

namespace MCS.Web.Responsive.WebControls
{
	/// <summary>
	/// AutoCompleteControl是对AutoCompleteExtender的一个包装，AutoCompleteExtender仅起到辅助输入的作用，没有记录和控制输入的值和文本
	/// 这个控件就是为了添加这些控制
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript))]
	[ToolboxData("<{0}:AutoCompleteControl runat=server></{0}:AutoCompleteControl>")]
    [ClientScriptResource("MCS.Web.WebControls.AutoCompleteControl", "MCS.Web.Responsive.WebControls.AutoComplete.AutoCompleteExtender.js")]
	public class AutoCompleteControl : ScriptControlBase
	{
		private AutoCompleteExtender _autoCompleteExtender = new AutoCompleteExtender() { CompareFieldName = new string[] { "Value" }, DataTextFieldList = new string[] { "Value", "Text" } };
		private Control _targetControl = null;

		/// <summary>
		/// 
		/// </summary>
		public event AutoCompleteExtender.GetDataSourceDelegate GetDataSource;

		/// <summary>
		/// 构造方法
		/// </summary>
		public AutoCompleteControl()
			: base(true, HtmlTextWriterTag.Div)
		{
		}

		#region Properties
		/// <summary>
		/// 目标控件的ID
		/// </summary>
		[DefaultValue(""), IDReferenceProperty()]
		[Category("Appearance")]
		public string TargetControlID
		{
			get
			{
				return this._autoCompleteExtender.TargetControlID;
			}

			set
			{
				this._autoCompleteExtender.TargetControlID = value;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("targetControlClientID")]
		private string TargetControlClientID
		{
			get
			{
				string result = null;

				if (this.TargetControl != null)
					result = this.TargetControl.ClientID;

				return result;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("autoCompleteExtenderClientID")]
		private string AutoCompleteExtenderClientID
		{
			get
			{
				return this._autoCompleteExtender.ClientID;
			}
		}

		/// <summary>
		/// 目标控件
		/// </summary>
		[Browsable(false)]
		public Control TargetControl
		{
			get
			{
				if (this._targetControl == null)
				{
					if (string.IsNullOrEmpty(this.TargetControlID) == false)
						this._targetControl = WebUtility.GetCurrentPage().FindControlByID(this.TargetControlID, true);
				}

				return this._targetControl;
			}

			set
			{
				this._targetControl = value;
			}
		}

		/// <summary>
		/// 控件输入的值
		/// </summary>
		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("value")]
		public string Value
		{
			get
			{
				return this.GetPropertyValue("Value", string.Empty);
			}

			set
			{
				this.SetPropertyValue("Value", value);
			}
		}

		/// <summary>
		/// 控件输入的值
		/// </summary>
		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("text")]
		public string Text
		{
			get
			{
				return this._autoCompleteExtender.Text;
			}

			set
			{
				this._autoCompleteExtender.Text = value;
			}
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
		public bool IsAutoComplete
		{
			get { return this._autoCompleteExtender.IsAutoComplete; }
			set { this._autoCompleteExtender.IsAutoComplete = value; }
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
		public bool AutoValidateOnChange
		{
			get { return this._autoCompleteExtender.AutoValidateOnChange; }
			set { this._autoCompleteExtender.AutoValidateOnChange = value; }
		}

		/// <summary>
		/// 输入多少个字符后开始自动完成，默认为3
		/// </summary>
		/// <remarks>
		/// 输入多少个字符后开始自动完成，默认为3
		/// </remarks>
		[DefaultValue(3)]
		public int MinimumPrefixLength
		{
			get { return this._autoCompleteExtender.MinimumPrefixLength; }
			set { this._autoCompleteExtender.MinimumPrefixLength = value; }
		}

		/// <summary>
		/// 自动完成间隔。默认1000毫秒。输入停止后多长时间开始自动完成，单位：毫秒。1000毫秒=1秒
		/// </summary>
		/// <remarks>
		/// 自动完成间隔。默认1000毫秒。输入停止后多长时间开始自动完成，单位：毫秒。1000毫秒=1秒
		/// </remarks>
		[DefaultValue(1000)]
		public int CompletionInterval
		{
			get { return this._autoCompleteExtender.CompletionInterval; }
			set { this._autoCompleteExtender.CompletionInterval = value; }
		}

		/// <summary>
		/// 鼠标移动到自动完成的项目上的CssClass
		/// </summary>
		/// <remarks>
		/// 鼠标移动到自动完成的项目上的CssClass
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		public string ItemHoverCssClass
		{
			get { return this._autoCompleteExtender.ItemHoverCssClass; }
			set { this._autoCompleteExtender.ItemHoverCssClass = value; }
		}

		/// <summary>
		/// 当启用输入验证，并且输入的内容无法完全匹配到数据源中的某一项后的CssClass
		/// </summary>
		/// <remarks>
		/// 当启用输入验证，并且输入的内容无法完全匹配到数据源中的某一项后的CssClass
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		public string ErrorCssClass
		{
			get { return this._autoCompleteExtender.ErrorCssClass; }
			set { this._autoCompleteExtender.ErrorCssClass = value; }
		}

		/// <summary>
		/// 自动完成的项目CssClass
		/// </summary>
		/// <remarks>
		/// 自动完成的项目CssClass
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		public string ItemCssClass
		{
			get { return this._autoCompleteExtender.ItemCssClass; }
			set { this._autoCompleteExtender.ItemCssClass = value; }
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
		public bool RequireValidation
		{
			get { return this._autoCompleteExtender.RequireValidation; }
			set { this._autoCompleteExtender.RequireValidation = value; }
		}

		/// <summary>
		/// 事件回调时的Context
		/// </summary>
		[DefaultValue(false)]
		public object EventContext
		{
			get { return this._autoCompleteExtender.EventContext; }
			set { this._autoCompleteExtender.EventContext = value; }
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
		public int MaxCompletionRecordCount
		{
			get { return this._autoCompleteExtender.MaxCompletionRecordCount; }
			set { this._autoCompleteExtender.MaxCompletionRecordCount = value; }
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
		public int MaxPopupWindowHeight
		{
			get { return this._autoCompleteExtender.MaxPopupWindowHeight; }
			set { this._autoCompleteExtender.MaxPopupWindowHeight = value; }
		}

		/// <summary>
		/// 是否自动回调
		/// </summary>
		/// <remarks>
		/// 是否自动回调
		/// </remarks>
		[DefaultValue("")]
		public bool AutoCallBack
		{
			get { return this._autoCompleteExtender.AutoCallBack; }
			set { this._autoCompleteExtender.AutoCallBack = value; }
		}

		/// <summary>
		/// 启用客户端缓存
		/// </summary>
		/// <remarks>
		/// 启用客户端缓存
		/// </remarks>
		[DefaultValue(true)]
		public bool EnableCaching
		{
			get { return this._autoCompleteExtender.EnableCaching; }
			set { this._autoCompleteExtender.EnableCaching = value; }
		}

		/// <summary>
		/// 指定项目的Value值
		/// </summary>
		/// <remarks>
		/// 指定项目的Value值
		/// </remarks>
		[Category("Appearance")]
		[Description("指定项目的Value值")]
		public string DataValueField
		{
			get
			{
				return this._autoCompleteExtender.DataValueField;
			}

			set
			{
				this._autoCompleteExtender.DataValueField = value;
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
		public string DataTextFormatString
		{
			get
			{
				return this._autoCompleteExtender.DataTextFormatString;
			}

			set
			{
				this._autoCompleteExtender.DataTextFormatString = value;
			}
		}

		/// <summary>
		/// 选择机构的图标
		/// </summary>
		/// <remarks>
		/// 选择机构的图标
		/// </remarks>
		[Description("检查的图标")]
		public string CheckImage
		{
			get
			{
				return this._autoCompleteExtender.CheckImage;
			}

			set
			{
				this._autoCompleteExtender.CheckImage = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Description("等待的图标")]
		public string WaitingImage
		{
			get
			{
				return this._autoCompleteExtender.WaitingImage;
			}

			set
			{
				this._autoCompleteExtender.WaitingImage = value;
			}
		}

		/// <summary>
		/// 是否显示检查的图标
		/// </summary>
		[Description("是否显示检查的图标")]
		public bool ShowCheckImage
		{
			get
			{
				return this._autoCompleteExtender.ShowCheckImage;
			}

			set
			{
				this._autoCompleteExtender.ShowCheckImage = value;
			}
		}

		/// <summary>
		/// 回调时的上下文，由使用者提供
		/// </summary>
		[DefaultValue("")]
		[Bindable(true), Description("回调时的上下文，由使用者提供")]
		public string CallBackContext
		{
			get
			{
				return this._autoCompleteExtender.CallBackContext;
			}

			set
			{
				this._autoCompleteExtender.CallBackContext = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue("")]
		[Bindable(true), Category("ClientEventsHandler"), Description("数据变化后，触发的客户端事件")]
		[ClientPropertyName("valueChanged")]
		[ScriptControlEvent]
		public string OnValueChanged
		{
			get { return this.GetPropertyValue<string>("OnValueChanged", string.Empty); }
			set { this.SetPropertyValue<string>("OnValueChanged", value); }
		}
		#endregion

		/// <summary>
		/// 创建子控件
		/// </summary>
		protected override void CreateChildControls()
		{
			this.Controls.Add(this._autoCompleteExtender);
			this._autoCompleteExtender.GetDataSource += this.OnGetDataSource;

			base.CreateChildControls();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clientState"></param>
		protected override void LoadClientState(string clientState)
		{
			if (clientState.IsNotEmpty())
			{
				string[] states = JSONSerializerExecute.Deserialize<string[]>(clientState);

				this.Value = states[0];
				this.Text = states[1];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
		    IEnumerable<ScriptDescriptor> result = null;

			if (this.TargetControl != null)
		    {
				if (!this.IsRenderScript) return null;

		        this.EnsureID();

		        List<ScriptDescriptor> descriptors = new List<ScriptDescriptor>();

				ScriptControlDescriptor descriptor = new ScriptControlDescriptor(this.ClientControlType, this.TargetControlClientID);
		        this.DescribeComponent(descriptor);
		        descriptors.Add(descriptor);

		        result = descriptors;
		    }
		    else
		        result = base.GetScriptDescriptors();

		    return result;
		}

		private void OnGetDataSource(string sPrefix, int iCount, object eventContext, ref IEnumerable result)
		{
			if (this.GetDataSource != null)
				this.GetDataSource(sPrefix, iCount, eventContext, ref result);
		}
	}
}
