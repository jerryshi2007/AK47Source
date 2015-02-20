#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Web.WebControls
// FileName	��	DeluxeDate.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ��ΰ	    20070720		����
// -------------------------------------------------
#endregion

using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.WebControls.WebParts;
using MCS.Web.Library.Script;
using System.Web;
using MCS.Library.Globalization;
using System.Web.UI.HtmlControls;
#region [ Resources ]

[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.DeluxeCalendar.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.DeluxeCalendar.css", "text/css", PerformSubstitution = true)]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.arrow-left.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.arrow-right.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.caption.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.datePicker.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.updown.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.checked.gif", "image/gif")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeCalendar.Images.today.gif", "image/gif")]

#endregion

namespace MCS.Web.WebControls
{
	/// <summary>
	/// ���ڿؼ���
	/// </summary>
	[DefaultProperty("Value")]
	[RequiredScript(typeof(AnimationsScript), 4)]
	[ClientCssResource("MCS.Web.WebControls.DeluxeCalendar.DeluxeCalendar.css")]
	[ClientScriptResource("MCS.Web.WebControls.DeluxeCalendar", "MCS.Web.WebControls.DeluxeCalendar.DeluxeCalendar.js")]
	public class DeluxeCalendar : Web.Library.Script.ScriptControlBase
	{
		private string setTextBoxValue;
		
		/// <summary>
		/// DeluxeCalendar���캯��
		/// </summary>
		/// <remarks></remarks>
		public DeluxeCalendar()
			: base(false, HtmlTextWriterTag.Input)
		{
			this.Attributes.Add("type", "text");
		}

		/// <summary>
		/// �Ƿ�ֻ��ʾ����
		/// </summary>
		///<remarks>�Ƿ�ֻ��ʾ����</remarks>
		[Category("Default")]
		[ScriptControlProperty]
		[ClientPropertyName("isOnlyCurrentMonth")]
		[Description("�Ƿ�ֻ��ʾ����")]
		public bool IsOnlyCurrentMonth
		{
			get { return GetPropertyValue("IsOnlyCurrentMonth", true); }
			set { SetPropertyValue("IsOnlyCurrentMonth", value); }
		}

		/// <summary>
		/// ��������ʽ������ΪĬ����ʽ
		/// </summary>
		///<remarks>��������ʽ������ΪĬ����ʽ</remarks>
		[Category("Appearance")]
		[ScriptControlPropertyAttribute]
		[ClientPropertyName("cssClass")]
		[Description("��������ʽ������ΪĬ����ʽ")]
		public override string CssClass
		{
			get { return GetPropertyValue("CssClass", "ajax__calendar"); }
			set { SetPropertyValue("CssClass", value); }
		}

		private Style style = new Style();
		/// <summary>
		/// �������ʽ
		/// </summary>
		/// <remarks>�������ʽ</remarks>
		[Category("Appearance")]
		[Description("�������ʽ")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public Style TextStyle
		{
			get { return style; }
			set { style = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]
		[ClientPropertyName("readOnly")]
		public new bool ReadOnly
		{
			get { return GetPropertyValue("InnerReadOnly", false); }
			set { SetPropertyValue("InnerReadOnly", value); }
		}

		/// <summary>
		/// �����ı����Css
		/// </summary>
		/// <remarks>�����ı����Css</remarks>
		[Category("Appearance")]
		[Description("�����ı����Css")]
		public string TextCssClass
		{
			get { return GetPropertyValue("TextCssClass", "ajax_calendartextbox"); }
			set { SetPropertyValue("TextCssClass", value); }
		}

		/// <summary>
		/// ͼƬ��Style
		/// </summary>
		/// <remarks>ͼƬ��Style</remarks>
		//[ClientPropertyName]
		[Category("Appearance")]
		[Description("ͼƬ��Style")]
		public string ImageStyle
		{
			get { return GetPropertyValue("ImageStyle", string.Empty); }
			set { SetPropertyValue("ImageStyle", value); }
		}

		/// <summary>
		/// ͼƬ��CssClass
		/// </summary>
		/// <remarks>ͼƬ��CssClass</remarks>
		[Category("Appearance")]
		[Description("ͼƬ��CssClass")]
		public string ImageCssClass
		{
			get { return GetPropertyValue("ButtonCssClass", "ajax_calendarimagebutton"); }
			set { SetPropertyValue("ButtonCssClass", value); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks></remarks>
		[Browsable(false)]
		[ScriptControlProperty]
		[Description("��ť��ID")]
		public string MaskedEditButtonID
		{
			get { return GetPropertyValue("MaskedEditButtonID", this.UniqueID + "_image"); }
			set { SetPropertyValue("MaskedEditButtonID", value); }
		}

		/// <summary>
		/// ��ťĬ��ͼƬ��Src
		/// </summary>
		/// <remarks>��ťĬ��ͼƬ��Src</remarks>
		[Browsable(false)]
		public string DefaultImageUrl
		{
			get { return Page.ClientScript.GetWebResourceUrl(typeof(DeluxeCalendar), "MCS.Web.WebControls.DeluxeCalendar.Images.datePicker.gif"); }
		}

		/// <summary>
		/// ��ťͼƬ��Src
		/// </summary>
		/// <remarks>��ťͼƬ��Src</remarks>
		[Category("Appearance")]
		[Description("��ťͼƬ��Src")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public string ImageUrl
		{
			get { return GetPropertyValue("ImageUrl", string.Empty); }
			set { SetPropertyValue("ImageUrl", value); }
		}

		#region     �������벿��

		//[DefaultValue("9999-99-99")]
		//[ScriptControlProperty]
		//[DescriptionAttribute("���ڸ�ʽ��")]
		//public string Mask
		//{
		//    get { return GetPropertyValue("Mask", "9999-99-99"); }
		//    set { SetPropertyValue("Mask", value); }
		//}

		/// <summary>
		/// �����ַ�
		/// </summary>
		/// <remarks>�����ַ�</remarks>
		[Category("Data")]
		[ScriptControlProperty]
		[Description("�����ַ�")]
		public string PromptCharacter
		{
			get { return GetPropertyValue("PromptCharacter", "_"); }
			set { SetPropertyValue("PromptCharacter", value); }
		}

		/// <summary>
		/// �Ƿ��Զ���������
		/// </summary>
		/// <remarks>�Ƿ��Զ���������</remarks>
		[Category("Default")]
		[ScriptControlProperty]
		[Description("�Ƿ��Զ���������")]
		public bool AutoComplete
		{
			get { return GetPropertyValue("AutoComplete", true); }
			set { SetPropertyValue("AutoComplete", value); }
		}

		/// <summary>
		/// �ṩ�Զ������ʱ�䴮����������ȡϵͳ����
		/// </summary>
		/// <remarks>�ṩ�Զ������ʱ�䴮����������ȡϵͳ����</remarks>
		[Category("Data")]
		[ScriptControlProperty]
		[Description("�ṩ�Զ������ʱ�䴮����������ȡϵͳ����")]
		public string AutoCompleteValue
		{
			get { return GetPropertyValue("AutoCompleteValue", string.Empty); }
			set { SetPropertyValue("AutoCompleteValue", value); }
		}

		/// <summary>
		/// �Ƿ�������֤
		/// </summary>
		/// <remarks>�Ƿ�������֤</remarks>
		[Category("Default")]
		[ScriptControlProperty]
		[Description("�Ƿ�������֤")]
		public bool IsValidValue
		{
			get { return GetPropertyValue("IsValidValue", true); }
			set { SetPropertyValue("IsValidValue", value); }
		}

		/// <summary>
		/// ��֤���ڵ���ʾ��Ϣ
		/// </summary>
		/// <remarks>��֤���ڵ���ʾ��Ϣ</remarks>
		[Category("Data")]
		[ScriptControlProperty]
		[Description("��֤���ڵ���ʾ��Ϣ")]
		public string CurrentMessageError
		{
			get { return GetPropertyValue("CurrentMessageError", Translator.Translate(Define.DefaultCategory, "��������ȷ�����ڣ�")); }
			set { SetPropertyValue("CurrentMessageError", value); }
		}

		/// <summary>
		/// �õ�����ʱ�ı������ʽ
		/// </summary>
		/// <remarks>�õ�����ʱ�ı������ʽ</remarks>
		[Category("Appearance")]
		[ScriptControlProperty]
		[Description("�õ�����ʱ�ı������ʽ")]
		public string OnFocusCssClass
		{

			get { return GetPropertyValue("OnFocusCssClass", "MaskedEditFocus"); }
			set
			{
				SetPropertyValue("OnFocusCssClass", value);
			}
		}

		/// <summary>
		/// ��֤��ͨ��ʱ�ı������ʽ
		/// </summary>
		/// <remarks>��֤��ͨ��ʱ�ı������ʽ</remarks>
		[Category("Appearance")]
		[ScriptControlProperty]
		[Description("��֤��ͨ��ʱ�ı������ʽ")]
		public string OnInvalidCssClass
		{

			get { return GetPropertyValue("OnInvalidCssClass", "MaskedEditError"); ; }
			set
			{

				SetPropertyValue("OnInvalidCssClass", value);
			}

		}

		[ScriptControlProperty]
		[ClientPropertyName("imageButtonPath")]
		private string ImageButtonPath
		{
			get
			{
				return (ImageUrl == string.Empty) ? DefaultImageUrl : ResolveUrl(ImageUrl);
			}
		}


		#endregion
		/// <summary>
		/// ��ҳ������ı���ǰ����ؼ�id�Ͱ�ֵ
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(System.EventArgs e)
		{
			base.Attributes.Add("name", this.UniqueID);
			base.Attributes.Add("value", CValue);

			//����readonly
			if (this.ReadOnly)
			{
				Attributes.Add("readonly", "readonly");
			}
			base.OnPreRender(e);
		}

		/// <summary>
		/// ��ҳ������ı���
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			this.ApplyStyle(TextStyle);

			if (this.ReadOnly)
			{
				Label lb = new Label();
				lb.AccessKey = this.AccessKey;
				lb.AppRelativeTemplateSourceDirectory = this.AppRelativeTemplateSourceDirectory;

				foreach (string s in this.Attributes.Keys)
					lb.Attributes.Add(s, this.Attributes[s]);

				foreach (string s in this.Style.Keys)
					lb.Style.Add(s, this.Style[s]);

				lb.ForeColor = this.ForeColor;
				lb.Font.CopyFrom(this.Font);

				lb.ControlStyle.CopyFrom(this.ControlStyle);
				lb.Style["min-height"] = this.Height.ToString();
				lb.Style["height"] = string.Empty;
				//lb.ID = this.ID;
				lb.TabIndex = this.TabIndex;
				lb.TemplateControl = this.TemplateControl;

				string txt = this.Value != DateTime.MinValue ?
					HttpUtility.HtmlEncode(string.Format(this.DisplayFormat, this.Value)) :
					string.Empty;

				txt = txt.Replace("\r\n", "<br>");
				lb.Text = txt;
				lb.ToolTip = this.ToolTip;
				lb.Visible = this.Visible;
				lb.Width = this.Width;

				lb.Style.Add("word-wrap", "break-word");

				lb.RenderControl(writer);

				base.Style["display"] = "none";
				base.Render(writer);
			}
			else
			{
				base.Attributes.Add("class", TextCssClass);
				base.Render(writer);

				writer.Write(this.PlaceholderHtmlNode);

				//writer.Write("<img id='{0}' src='{1}' class='{2}' style='{3}' tabindex='-1' align='absmiddle' />",
				//    this.UniqueID + "_image", (ImageUrl == string.Empty) ? DefaultImageUrl : ResolveUrl(ImageUrl), ImageCssClass, ImageStyle,
				//    this.ReadOnly ? "onclick='return false;'" : string.Empty);
			}
		}

		/// <summary>
		/// ���û��ǻ�ȡ������ֵ
		/// </summary>
		/// <remarks>���û��ǻ�ȡ������ֵ</remarks>
		[Category("Default")]
		[ClientPropertyName("cValue")]
		[WebDisplayName("���û��ǻ�ȡ������ֵ")]
		[Description("���û��ǻ�ȡ������ֵ")]
		public string CValue
		{
			get
			{
				string result = null;

				if (this.setTextBoxValue != null)
				{
					result = this.setTextBoxValue;
				}
				else
				{
					if (Page.Request.Form[this.UniqueID] != null)
						result = Page.Request.Form[this.UniqueID];
					else
						result = null;
				}

				if (string.IsNullOrEmpty(result) == false)
					result = result.Trim('_', '-');

				return result;
			}
			set
			{
				try
				{
					if (string.IsNullOrEmpty(value) == false)
						this.setTextBoxValue = Convert.ToDateTime(value).ToString("yyyy-MM-dd");
					else
						this.setTextBoxValue = string.Empty;
				}
				catch
				{
					throw new InvalidCastException(Translator.Translate(Define.DefaultCategory, "������ڵĸ�ʽת��ʱ����Ч��"));
				}
			}
		}

		/// <summary>
		/// ֻ��״̬�����ڸ�ʽ
		/// </summary>
		[DefaultValue("{0:yyyy-MM-dd}")]
		[Description("ֻ��״̬�����ڸ�ʽ")]
		public string DisplayFormat
		{
			get { return GetPropertyValue("DisplayFormat", "{0:yyyy-MM-dd}"); }
			set { SetPropertyValue("DisplayFormat", value); }
		}

		/// <summary>
		/// �Ƿ�������������
		/// </summary>
		/// <remarks>�Ƿ�������������</remarks>
		[Category("Default")]
		[ScriptControlPropertyAttribute]
		[ClientPropertyName("enabled")]
		[Description("�Ƿ�������������")]
		public bool EnabledOnClient
		{
			get { return GetPropertyValue("EnabledOnClient", true); }
			set { SetPropertyValue("EnabledOnClient", value); }
		}

		/// <summary>
		/// �Ƿ�������������Ч��
		/// </summary>
		/// <remarks>�Ƿ�������������Ч��</remarks>
		[DefaultValue(true)]
		[ScriptControlPropertyAttribute]
		[ClientPropertyName("animated")]
		[Description("���������·�ת���Ķ���Ч��")]
		public virtual bool Animated
		{
			get { return GetPropertyValue("Animated", true); }
			set { SetPropertyValue("Animated", value); }
		}

		/// <summary>
		/// �Ƿ��ṩ��������ѡ��
		/// </summary>
		/// <remarks>�Ƿ��ṩ��������ѡ��</remarks>
		[Category("Default")]
		[ScriptControlPropertyAttribute]
		[ClientPropertyName("isComplexHeader")]
		[Description("�Ƿ��ṩ��������ѡ��")]
		public bool IsComplexHeader
		{
			get { return GetPropertyValue("IsComplexHeader", true); }
			set { SetPropertyValue("IsComplexHeader", value); }
		}

		/// <summary>
		/// �Զ����һ���Ǵ��ܼ���ʼ
		/// </summary>
		/// <remarks>�Զ����һ���Ǵ��ܼ���ʼ</remarks>
		[Category("Data")]
		[ScriptControlPropertyAttribute]
		[ClientPropertyName("firstDayOfWeek")]
		[Description("�Զ����һ���Ǵ��ܼ���ʼ")]
		public FirstDayOfWeek FirstDayOfWeek
		{
			get { return GetPropertyValue("FirstDayOfWeek", FirstDayOfWeek.Default); }
			set { SetPropertyValue("FirstDayOfWeek", value); }
		}

		/// <summary>
		/// ��������ʱ�����Ŀͻ����¼�
		/// </summary>
		/// <remarks>��������ʱ�����Ŀͻ����¼�</remarks>
		[Category("Action")]
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("onClientShowing")]
		[Description("��������ʱ�����Ŀͻ����¼�")]
		public string OnClientShowing
		{
			get { return GetPropertyValue("OnClientShowing", string.Empty); }
			set { SetPropertyValue("OnClientShowing", value); }
		}

		/// <summary>
		/// ��������ʱ�󴥷��Ŀͻ����¼�
		/// </summary>
		/// <remarks>��������ʱ�󴥷��Ŀͻ����¼�</remarks>
		[Category("Action")]
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("onClientShown")]
		[Description("��������ʱ�󴥷��Ŀͻ����¼�")]
		public string OnClientShown
		{
			get { return GetPropertyValue("OnClientShown", string.Empty); }
			set { SetPropertyValue("OnClientShown", value); }
		}

		/// <summary>
		/// ��������ʱ�����Ŀͻ����¼�
		/// </summary>
		/// <remarks>��������ʱ�����Ŀͻ����¼�</remarks>
		[Category("Action")]
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("onClientHiding")]
		[Description("��������ʱ�����Ŀͻ����¼�")]
		public string OnClientHiding
		{
			get { return GetPropertyValue("OnClientHiding", string.Empty); }
			set { SetPropertyValue("OnClientHiding", value); }
		}

		/// <summary>
		/// ���������󴥷��Ŀͻ����¼�
		/// </summary>
		/// <remarks>���������󴥷��Ŀͻ����¼�</remarks>
		[Category("Action")]
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("onClientHidden")]
		[Description("���������󴥷��Ŀͻ����¼�")]
		public string OnClientHidden
		{
			get { return GetPropertyValue("OnClientHidden", string.Empty); }
			set { SetPropertyValue("OnClientHidden", value); }
		}

		/// <summary>
		/// ����ѡ��仯�󴥷��Ŀͻ����¼�
		/// </summary>
		/// <remarks>����ѡ��仯�󴥷��Ŀͻ����¼�</remarks>
		[Category("Action")]
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("onClientDateSelectionChanged")]
		[Description("����ѡ��仯�󴥷��Ŀͻ����¼�")]
		public string OnClientDateSelectionChanged
		{
			get { return GetPropertyValue("OnClientDateSelectionChanged", string.Empty); }
			set { SetPropertyValue("OnClientDateSelectionChanged", value); }
		}

		/// <summary>
		/// ���ڻ�ʱ������仯�󴥷��Ŀͻ����¼�
		/// </summary>
		[DefaultValue("")]
		[Category("Action")]
		[ScriptControlEvent()]
		[ClientPropertyName("clientValueChanged")]
		[Description("���ڻ�ʱ������仯�󴥷��Ŀͻ����¼�")]
		public string OnClientValueChanged
		{
			get { return GetPropertyValue("OnClientValueChanged", string.Empty); }
			set { SetPropertyValue("OnClientValueChanged", value); }
		}

		/// <summary>
		/// ��������
		/// </summary>
		/// <remarks>��������</remarks>
		public DateTime Value
		{
			get
			{
				string cv = this.CValue;

				if (string.IsNullOrEmpty(cv) == false)
				{
					try
					{
						return Convert.ToDateTime(cv);
					}
					catch
					{
						throw new InvalidCastException(Translator.Translate(Define.DefaultCategory, "������ڵĸ�ʽת��ʱ����Ч��"));
					}
				}
				else
				{
					return DateTime.MinValue;/*����Ĭ��ֵ*/
				}
			}
			set
			{
				if (value == DateTime.MinValue)
					CValue = null;
				else
					CValue = value.Date.ToString("yyyy-MM-dd");
			}
		}

		/// <summary>
		/// ռλ��ID
		/// </summary>
		[DefaultValue("div_placeholder")]
		[ScriptControlProperty()]
		[ClientPropertyName("placeholderID")]
		[Description("ռλ��ID")]
		public string PlaceholderID
		{
			get { return GetPropertyValue("PlaceholderID", this.ClientID + "_div_placeholder"); }
			set { SetPropertyValue("PlaceholderID", value); }
		}

		[Description("ռλ��Html")]
		private string PlaceholderHtmlNode
		{
			get
			{
				return "<div id='" + HttpUtility.HtmlAttributeEncode(PlaceholderID) + "' style='display:none;cursor: pointer'></div>";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(1900)]
		[ScriptControlProperty()]
		[ClientPropertyName("startYear")]
		[Description("��ʼ��")]
		public int StartYear
		{
			get { return GetPropertyValue("StartYear", 1900); }
			set { SetPropertyValue("StartYear", value); }
		}

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(2500)]
		[ScriptControlProperty()]
		[ClientPropertyName("endYear")]
		[Description("������")]
		public int EndYear
		{
			get { return GetPropertyValue("EndYear", 2500); }
			set { SetPropertyValue("EndYear", value); }
		}
	}
}

