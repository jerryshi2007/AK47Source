#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Web.WebControls
// FileName	��	DeluxeTime.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ��ΰ	    20070720		����
// -------------------------------------------------
#endregion


using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MCS.Web.Library.Script;
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeTime.DeluxeTime.css", "text/css")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeTime.DeluxeTime.js", "text/javascript")]
namespace MCS.Web.WebControls
{
	/// <summary>
	/// ʱ��ؼ���
	/// </summary>
	[DefaultProperty("MValue")]
	[RequiredScript(typeof(ControlBaseScript), 0)]
	[ClientCssResource("MCS.Web.WebControls.DeluxeTime.DeluxeTime.css")]
	[ClientScriptResource("MCS.Web.WebControls.DeluxeTime", "MCS.Web.WebControls.DeluxeTime.DeluxeTime.js")]
	public class DeluxeTime : ScriptControlBase
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <remarks></remarks>
		public DeluxeTime()
			: base(true, HtmlTextWriterTag.Input)
		{
			this.Attributes.Add("type", "text");
		}

		private string setTextBoxValue;
		private GenericInputExtender extender = new GenericInputExtender();

		/// <summary>
		/// ��дOnPreRender��������ʽ�ɿ���Button�����
		/// </summary>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected override void OnPreRender(EventArgs e)
		{
			this.Attributes.Add("name", this.UniqueID);
			this.Attributes.Add("class", TextCssClass);
			this.Attributes.Add("value", MValue);

			if (ShowButton)
			{
				this.extender.TargetControlID = this.UniqueID;
				this.extender.Text = MValue;
				//this.extender.Items.Add(DataSource);
				this.Controls.Add(this.extender);
			}

			base.OnPreRender(e);
		}

		/// <summary>
		/// ��дRender��������ʽ
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

				lb.Text = MValue;
				lb.ToolTip = this.ToolTip;
				lb.Visible = this.Visible;
				lb.Width = this.Width;

				lb.Style.Add("word-wrap", "break-word");

				lb.RenderControl(writer);

				base.Style["display"] = "none";
			}

			base.Render(writer);
		}

		/// <summary>
		/// �����ı������ʽ
		/// </summary>
		/// <remarks>�����ı������ʽ</remarks>
		[Category("Appearance")]
		[Description("�����ı������ʽ")]
		public string TextCssClass
		{
			get { return GetPropertyValue("TextCssClass", string.Empty); }
			set { SetPropertyValue("TextCssClass", value); }
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
			get { return this.style; }
			set { this.style = value; }
		}

		/// <summary>
		/// ʱ���ʽ��
		/// </summary>
		/// <remarks>ʱ���ʽ��</remarks>
		[Category("Data")]
		[ScriptControlProperty]
		[Description("ʱ���ʽ��")]
		public string Mask
		{
			get { return GetPropertyValue("Mask", "99:99:99"); }
			set { SetPropertyValue("Mask", value); }
		}

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
		/// �Ƿ��Զ�����ʱ��
		/// </summary>
		/// <remarks>�Ƿ��Զ�����ʱ��</remarks>
		[Category("Default")]
		[ScriptControlProperty]
		[Description("�Ƿ��Զ�����ʱ��")]
		public bool AutoComplete
		{
			get { return GetPropertyValue("AutoComplete", true); }
			set { SetPropertyValue("AutoComplete", value); }
		}

		/// <summary>
		/// �ṩ�Զ������ʱ�䴮����������ȡϵͳʱ��
		/// </summary>
		/// <remarks>�ṩ�Զ������ʱ�䴮����������ȡϵͳʱ��</remarks>
		[Category("Data")]
		[ScriptControlProperty]
		[Description("�ṩ�Զ������ʱ�䴮����������ȡϵͳʱ��")]
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
		/// ��֤ʱ�����ʾ��Ϣ
		/// </summary>
		/// <remarks>��֤ʱ�����ʾ��Ϣ</remarks>
		[Category("Appearance")]
		[ScriptControlProperty]
		[Description("��֤ʱ�����ʾ��Ϣ")]
		public string CurrentMessageError
		{
			get { return GetPropertyValue("CurrentMessageError", string.Empty); }
			set { SetPropertyValue("CurrentMessageError", value); }
		}

		/// <summary>
		/// �Ƿ��ṩ��ť��ѡ���Զ���ʱ���б�,����������������Դ
		/// </summary>
		/// <remarks>�Ƿ��ṩ��ť��ѡ���Զ���ʱ���б�,����������������Դ</remarks>
		[Category("Appearance")]
		[Description("�Ƿ��ṩ��ť��ѡ���Զ���ʱ���б�,����������������Դ")]
		public bool ShowButton
		{
			get { return GetPropertyValue("ShowButton", false); }
			set { SetPropertyValue("ShowButton", value); }
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
			set { SetPropertyValue("OnFocusCssClass", value); }
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
			get { return GetPropertyValue("OnInvalidCssClass", "MaskedEditError"); }
			set { SetPropertyValue("OnInvalidCssClass", value); }
		}

		/// <summary>
		/// ���û��ǻ�ȡʱ���ֵ
		/// </summary>
		/// <remarks>���û��ǻ�ȡʱ���ֵ</remarks>
		[Category("Default")]
		[Description("���û��ǻ�ȡʱ���ֵ")]
		public string MValue
		{
			get
			{
				if (this.setTextBoxValue != null)
				{
					return this.setTextBoxValue;
				}
				else
				{
					if (Page.Request.Form[this.UniqueID] != null)
						return Page.Request.Form[this.UniqueID];
					else
						return null;
				}
			}
			set
			{
				try
				{
					if (string.IsNullOrEmpty(value) == false)
						this.setTextBoxValue = Convert.ToDateTime(value).ToString("HH:mm:ss");
					else
						this.setTextBoxValue = string.Empty;
				}
				catch
				{
					throw new InvalidCastException("������ڵĸ�ʽת��ʱ����Ч��");
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Value
		{
			get
			{
				if (Page.Request.Form[this.UniqueID] != null)
					return Page.Request.Form[this.UniqueID];
				else
					return null;
			}
		}

		/// <summary>
		/// ʱ������仯�󴥷��Ŀͻ����¼�
		/// </summary>
		/// <remarks>ʱ������仯�󴥷��Ŀͻ����¼�</remarks>
		[DefaultValue("")]
		[Category("Action")]
		[ScriptControlEvent]
		[Description("ʱ������仯�󴥷��Ŀͻ����¼�")]
		public string OnClientValueChanged
		{
			get { return GetPropertyValue("OnClientValueChanged", string.Empty); }
			set { SetPropertyValue("OnClientValueChanged", value); }
		}

		#region    �б���
		private ListItem listItem = new ListItem();

		/// <summary>
		/// ����Դ
		/// </summary>
		/// <remarks>ListItem���͵�����Դ</remarks>
		[Category("Data")]
		[Description("����Դ")]
		public ListItemCollection DataSource
		{
			get { return this.extender.Items; }

		}
		#endregion
	}
}
