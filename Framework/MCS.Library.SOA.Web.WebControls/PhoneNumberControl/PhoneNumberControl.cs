using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library.Script;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using MCS.Web.Library;
using System.Web.UI.HtmlControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Library.Principal;

[assembly: WebResource("MCS.Web.WebControls.PhoneNumberControl.PhoneNumberControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.PhoneNumberControl.PhoneNumberControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.PhoneNumberControl.PhoneNumberControl.css", "text/css")]
namespace MCS.Web.WebControls
{
	/// <summary>
	/// 电话号码控件
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 3)]
	[RequiredScript(typeof(PhoneNumberScript), 5)]
	[ClientScriptResource("MCS.Web.WebControls.PhoneNumberControl", "MCS.Web.WebControls.PhoneNumberControl.PhoneNumberControl.js")]
	[ClientCssResource("MCS.Web.WebControls.PhoneNumberControl.PhoneNumberControl.css")]
	public partial class PhoneNumberControl : ScriptControlBase
	{
		public PhoneNumberControl()
			: base(true, HtmlTextWriterTag.Span)
		{
			JSONSerializerExecute.RegisterConverter(typeof(PhoneNumberConverter));
		}

		#region  Fields

	    private const  int EXTNUMBER_FLAG = 1;
	    private const  int PHONENUMBER_FLAG = 2;
	    private const  int AREANUMBER_FLAG = 4;
	    private const  int STATENUMBER_FLAG = 8;

		private HBDropDownList _stateDropdownList;
		private TextBox _areaInputBox;
		private TextBox _mainInputBox;
		private TextBox _extInputBox;
		private PhoneNumber _phonenumber = new PhoneNumber();
		private CustomValidator submitValidator = new CustomValidator();

		#endregion

		#region  Properties
		/// <summary>
		/// 电话号码控件使用模式
		/// </summary>
		[DefaultValue(PhoneNumberUseMode.OnlyPhoneNumber), Category("Appearance"), ScriptControlProperty, ClientPropertyName("phoneNumberUseMode"), Description("电话的显示模式")]
		public PhoneNumberUseMode PhoneNumberUseMode
		{
			get
			{
				return WebControlUtility.GetViewStateValue(this.ViewState, "PhoneNumberUseMode", PhoneNumberUseMode.OnlyPhoneNumber);
			}
			set
			{
				WebControlUtility.SetViewStateValue(this.ViewState, "PhoneNumberUseMode", value);
			}
		}

		/// <summary>
		/// 电话的显示类型
		/// </summary>
		[DefaultValue(PhoneNumberCategory.Cellphone), Category("Appearance"), ScriptControlProperty, ClientPropertyName("phoneNumberCategory"), Description("电话的显示类型")]
		public PhoneNumberCategory PhoneNumberCategory
		{
			get
			{
				return WebControlUtility.GetViewStateValue(this.ViewState, "PhoneNumberCategory", PhoneNumberCategory.Cellphone);
			}
			set
			{
				WebControlUtility.SetViewStateValue(this.ViewState, "PhoneNumberCategory", value);
			}
		}

		/// <summary>
		/// stateDropdownList控件ID
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("stateDropdownListID")]
		private string stateDropdownListID
		{
			get
			{
				return _stateDropdownList.ClientID;
			}
		}

		/// <summary>
		/// areaInputBox控件ID
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("areaInputBoxID")]
		private string AreaInputBoxID
		{
			get
			{
				return _areaInputBox.ClientID;
			}
		}

		/// <summary>
		/// mainInputBox控件ID
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("mainInputBoxID")]
		private string MainInputBoxID
		{
			get
			{
				return _mainInputBox.ClientID;
			}
		}

		/// <summary>
		/// extInputBox控件ID
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("extInputBoxID")]
		private string ExtInputBoxID
		{
			get
			{
				return _extInputBox.ClientID;
			}
		}

		/// <summary>
		/// 国别控件宽度
		/// </summary>
		[DefaultValue(typeof(Unit), "100%")]
		[Bindable(true), Category("Appearance")]
		public Unit StateWidth
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "StateWidth", new Unit("50px"));
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "StateWidth", value);
			}
		}

		/// <summary>
		/// 区号控件宽度
		/// </summary>
		[DefaultValue(typeof(Unit), "100%")]
		[Bindable(true), Category("Appearance")]
		public Unit AreaWidth
		{
			get
			{

				return WebControlUtility.GetViewStateValue(ViewState, "AreaWidth", new Unit("50px"));
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AreaWidth", value);
			}
		}

		/// <summary>
		/// 电话控件宽度
		/// </summary>
		[DefaultValue(typeof(Unit), "100%")]
		[Bindable(true), Category("Appearance")]
		public Unit MainWidth
		{
			get
			{

				return WebControlUtility.GetViewStateValue(ViewState, "MainWidth", new Unit("70px"));
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "MainWidth", value);
			}
		}

		/// <summary>
		/// 分机控件宽度
		/// </summary>
		[DefaultValue(typeof(Unit), "100%")]
		[Bindable(true), Category("Appearance")]
		public Unit ExtWidth
		{
			get
			{

				return WebControlUtility.GetViewStateValue(ViewState, "ExtWidth", new Unit("50px"));
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ExtWidth", value);
			}
		}

		/// <summary>
		/// 电话号码输入框的样式
		/// </summary>        
		[DefaultValue("contact-telephone-mainnumber")]
		[Browsable(true), Description("电话号码输入框的样式"), Category("Appearance")]
		public string TelephoneCss
		{
			get { return GetPropertyValue("TelephoneCss", "contact-telephone-mainnumber"); }
			set { SetPropertyValue("TelephoneCss", value); }
		}

		/// <summary>
		/// 国别号、区号、分机号输入框的样式
		/// </summary>        
		[DefaultValue("contact-telephone-number")]
		[Browsable(true), Description("国别号、区号、分机号输入框的样式"), Category("Appearance")]
		public string InputCss
		{
			get { return GetPropertyValue("InputCss", "contact-telephone-number"); }
			set { SetPropertyValue("InputCss", value); }
		}

		#endregion

		#region 暴露的属性

		/// <summary>
        /// 是否自动校验
		/// </summary>
        /// <remarks>是否自动校验</remarks>
		[Category("Default")]
		[ScriptControlProperty]
		[Description("是否自动校验")]
		[ClientPropertyName("autoVaildate")]
		public bool AutoVaildate
		{
			get { return GetPropertyValue("AutoVaildate", true); }
			set { SetPropertyValue("AutoVaildate", value); }
		}

		/// <summary>
		/// 是否可为空
		/// </summary>
		/// <remarks>是否可为空</remarks>
		[Category("Default")]
		[ScriptControlProperty]
		[Description("是否可为空")]
        [ClientPropertyName("allowEmpty")]
		public bool AllowEmpty
		{
            get { return GetPropertyValue("AllowEmpty", true); }
            set { SetPropertyValue("AllowEmpty", value); }
		}

		/// <summary>
		/// 用户自定义错误提示信息
		/// </summary>
		/// <remarks>用户自定义错误提示信息</remarks>
		[Category("Appearance")]
		[ScriptControlProperty]
        [ClientPropertyName("errorMessage")]
		[Description("用户自定义错误提示信息")]
		public string ErrorMessage
		{
            get { return GetPropertyValue("ErrorMessage", string.Empty); }
            set { SetPropertyValue("ErrorMessage", value); }
		}

		/// <summary>
		///	区号用户自定义正则表达式
		/// </summary>
		/// <remarks>用户自定义正则表达式</remarks>
		[Category("Appearance")]
		[ScriptControlProperty]
		[Description("用户自定义正则表达式")]
		[ClientPropertyName("customAreaRegularExpression")]
		public string CustomAreaRegularExpression
		{
			get { return GetPropertyValue("CustomAreaRegularExpression", string.Empty); }
			set { SetPropertyValue("CustomAreaRegularExpression", value); }
		}

		/// <summary>
		///	电话用户自定义正则表达式
		/// </summary>
		/// <remarks>用户自定义正则表达式</remarks>
		[Category("Appearance")]
		[ScriptControlProperty]
		[Description("用户自定义正则表达式")]
		[ClientPropertyName("customMainRegularExpression")]
		public string CustomMainRegularExpression
		{
			get { return GetPropertyValue("CustomMainRegularExpression", string.Empty); }
			set { SetPropertyValue("CustomMainRegularExpression", value); }
		}

		/// <summary>
		///	分机用户自定义正则表达式
		/// </summary>
		/// <remarks>用户自定义正则表达式</remarks>
		[Category("Appearance")]
		[ScriptControlProperty]
		[Description("用户自定义正则表达式")]
		[ClientPropertyName("customExtRegularExpression")]
		public string CustomExtRegularExpression
		{
			get { return GetPropertyValue("CustomExtRegularExpression", string.Empty); }
			set { SetPropertyValue("CustomExtRegularExpression", value); }
		}

		/// <summary>
        /// 数据变化后触发的客户端事件
		/// </summary>
        /// <remarks>数据变化后触发的客户端事件</remarks>
		[DefaultValue("")]
		[Category("Action")]
		[ScriptControlEvent]
		[ClientPropertyName("onClientValueChanged")]
		[Description("数据变化后触发的客户端事件")]
		public string OnClientValueChanged
		{
			get { return GetPropertyValue("OnClientValueChanged", string.Empty); }
			set { SetPropertyValue("OnClientValueChanged", value); }
		}

		/// <summary>
		/// 用户自定义格式
		/// </summary>
		/// <remarks>用户自定义正则表达式</remarks>
		public string FormatString
		{
			get { return GetPropertyValue("FormatString", string.Empty); }
			set { SetPropertyValue("FormatString", value); }
		}

		[ScriptControlProperty]
		[ClientPropertyName("currentStateCode")]
		private string CurrentStateCode
		{
			get 
			{
				return this.DefaultStateCode == "" ? this.UserDefaultStateCode : this.DefaultStateCode;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("currentAreaCode")]
		private string CurrentAreaCode
		{
			get
			{
				return this.DefaultAreaCode == "" ? this.UserDefaultAreaCode : this.DefaultAreaCode;
			}
		}

		/// <summary>
		/// 默认国别号
		/// </summary>
		/// <remarks>默认国别号</remarks>
		[Category("Appearance")]
		[ScriptControlProperty]
		[Description("默认国别号")]
		[ClientPropertyName("defaultStateCode")]
		public string DefaultStateCode
		{
			get { return GetPropertyValue("DefaultStateCode", string.Empty); }
			set { SetPropertyValue("DefaultStateCode", value); }
		}

		/// <summary>
		/// 默认区号
		/// </summary>
		/// <remarks>默认区号</remarks>
		[Category("Appearance")]
		[ScriptControlProperty]
		[Description("默认区号")]
		[ClientPropertyName("defaultAreaCode")]
		public string DefaultAreaCode
		{
			get { return GetPropertyValue("DefaultAreaCode", string.Empty); }
			set { SetPropertyValue("DefaultAreaCode", value); }
		}

		private UserSettings _userSettings = null;
		//用户默认国别号
		[Browsable(false)]
		private string UserDefaultStateCode
		{
			get
			{
				if (_userSettings == null)
				{
					if (DeluxePrincipal.IsAuthenticated)
					{
						_userSettings = UserSettings.GetSettings(DeluxeIdentity.CurrentUser.ID);
					}
					else
						return null;
				}

				return _userSettings.GetPropertyValue("CommonSettings", "CountryCode", "86");
			}
		}

		//用户默认区号
		[Browsable(false)]
		private string UserDefaultAreaCode
		{
			get
			{
				if (_userSettings == null)
				{
					if (DeluxePrincipal.IsAuthenticated)
					{
						_userSettings = UserSettings.LoadSettings(DeluxeIdentity.CurrentUser.ID);
					}
					else
						return null;
				}

				return _userSettings.GetPropertyValue("CommonSettings", "AreaCode", "010");
			}
		}

		/// <summary>
		/// 电话对象
		/// </summary>
		[Browsable(false)]
		public PhoneNumber PhoneNumber
		{
			get
			{
				if (this._phonenumber != null)
				{
					if (this._phonenumber.StateCode.IsNullOrEmpty())
					{
						this._phonenumber.StateCode = this.CurrentStateCode;
					}
					if (this._phonenumber.AreaCode.IsNullOrEmpty())
					{
						this._phonenumber.AreaCode = this.CurrentAreaCode;
					}
				}

				return this._phonenumber;
			}
			set
			{
				ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "PhoneNumber");
				this._phonenumber = value;
			}
		}

		/// <summary>
		/// 是否为只读
		/// </summary>
		[Category("readOnly")]
		[ScriptControlProperty]
		[Description("是否为只读")]
		[ClientPropertyName("readOnly")]
		public override bool ReadOnly
		{
			get
			{
				return base.ReadOnly;
			}
			set
			{
				base.ReadOnly = value;
			}
		}

		#endregion

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			_stateDropdownList = new HBDropDownList { ID = "stateDropdownList" };
			_areaInputBox = new TextBox { ID = "areaInputBox" };
			_mainInputBox = new TextBox { ID = "mainInputBox" };
			_extInputBox = new TextBox { ID = "extInputBox" };

			if (PhoneNumberCategory == PhoneNumberCategory.Cellphone)
			{
				(((int)this.PhoneNumberUseMode & AREANUMBER_FLAG) > 0 || ((int)this.PhoneNumberUseMode & EXTNUMBER_FLAG) > 0).TrueThrow("PhoneNumberCategory为手机时，PhoneNumberUseMode不可包含区号和分机。");
			}

            if (!this.ReadOnly)
			{
				if (((int)this.PhoneNumberUseMode & STATENUMBER_FLAG) > 0)
				{
					var lineSpan = new HtmlGenericControl() { TagName = "span", InnerText = "—" };
					Controls.Add(this._stateDropdownList);
					Controls.Add(lineSpan);
					BindListControl();
				}

				if (((int)this.PhoneNumberUseMode & AREANUMBER_FLAG) > 0)
				{
					var lineSpan = new HtmlGenericControl() { TagName = "span", InnerText = "—" };
					Controls.Add(this._areaInputBox);
					Controls.Add(lineSpan);
				}

				if (((int)this.PhoneNumberUseMode & PHONENUMBER_FLAG) > 0)
				{
					Controls.Add(this._mainInputBox);

					if (((int)this.PhoneNumberUseMode & EXTNUMBER_FLAG) > 0)
					{
						var lineSpan = new HtmlGenericControl() { TagName = "span", InnerText = "—" };
						Controls.Add(lineSpan);
					}					
				}

				if (((int)this.PhoneNumberUseMode & EXTNUMBER_FLAG) > 0)
				{
					Controls.Add(this._extInputBox);
				}

				//如果没有添加Validator则添加Validator
				if (this.Visible && this.Page.Items.Contains("telephoneClientValidate") == false &&
					!RenderMode.OnlyRenderSelf)
				{
					this.Controls.Add(this.submitValidator);
					this.Page.Items.Add("telephoneClientValidate", "exist");
				}
			}
		}

		//填充国别信息
		private void BindListControl()
		{
			CountrycodeCollection countrycodes = CountrycodeAdapter.Instance.LoadAllFromCache();
			foreach (var country in countrycodes)
			{
				ListItem item = new ListItem(country.CnName, country.Code);
				item.Attributes.Add("title", country.Code);
				_stateDropdownList.Items.Add(item);
			}
		}

		protected override void LoadClientState(string clientState)
		{
			if (!string.IsNullOrEmpty(clientState))
				this._phonenumber = JSONSerializerExecute.Deserialize<PhoneNumber>(clientState);
		}

		protected override string SaveClientState()
		{
			return JSONSerializerExecute.Serialize(this._phonenumber);
		}

		//页面呈现事件
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (this.ReadOnly)
			{
				Label lbnumber = new Label();

				if (!this.FormatString.IsNullOrEmpty())
				{
					lbnumber.Text = this._phonenumber.ToString(this.FormatString);
				}
				else
					lbnumber.Text = this._phonenumber.ToString("C-A-T-E");

				Controls.Add(lbnumber);
			}
            else
            {
					_stateDropdownList.Width = this.StateWidth;
					_areaInputBox.Width = this.AreaWidth;
					_extInputBox.Width = this.ExtWidth;
					_mainInputBox.Width = this.MainWidth;

				    if (this.PhoneNumberCategory == WebControls.PhoneNumberCategory.Cellphone)
					{
						_mainInputBox.Width = new Unit("100px");
					}
				    
					_stateDropdownList.EnableViewState = false;
					_areaInputBox.EnableViewState = false;
                    _mainInputBox.EnableViewState = false;
				    _extInputBox.EnableViewState = false;
            }

			this.submitValidator.ClientValidationFunction = "$HBRootNS.PhoneNumberControl.telephoneClientValidate";
		}
	}
}
