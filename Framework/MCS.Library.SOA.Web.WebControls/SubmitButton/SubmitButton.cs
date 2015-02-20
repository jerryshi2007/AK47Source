using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Web.Library;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 进度条的模式
	/// </summary>
	public enum SubmitButtonProgressMode
	{
		/// <summary>
		/// 按照时间间隔
		/// </summary>
		ByTimeInterval = 0,

		/// <summary>
		/// 按照步骤（真实步骤）
		/// </summary>
		BySteps = 1
	}

	/// <summary>
	/// 带量程计的提交按钮。在提交时，可以disable掉其它同类型的按钮
	/// </summary>
	[DefaultEvent("Click")]
	[DefaultProperty("Text"),
		ToolboxData("<{0}:SubmitButton runat=server></{0}:SubmitButton>")]
	public class SubmitButton : System.Web.UI.WebControls.Button
	{
		private IAttributeAccessor relativeControl = null;

		/// <summary>
		/// 过程信息的隐藏域的ID
		/// </summary>
		public const string ProgressInfoHiddenID = "submitButtonProgressInfoHiddenID";

		/// <summary>
		/// 通知过程信息变化的ButtonID
		/// </summary>
		public const string ProgressInfoChangedButtonID = "submitButtonProgressInfoChangedButtonID";

		/// <summary>
		/// 提交时显示的提示信息
		/// </summary>
		#region Properties
		[Category("Appearance"), DefaultValue("True")]
		public string PopupCaption
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "PopupCaption", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "PopupCaption", value);
			}
		}

		/// <summary>
		/// 是否自动在提交时Disabled
		/// </summary>
		[Category("Appearance"), DefaultValue("True")]
		public bool AutoDisabled
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AutoDisabled", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AutoDisabled", value);
			}
		}

		/// <summary>
		/// 进度条的模式
		/// </summary>
		[Category("Appearance"), DefaultValue(SubmitButtonProgressMode.ByTimeInterval)]
		public SubmitButtonProgressMode ProgressMode
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ProgressMode", SubmitButtonProgressMode.ByTimeInterval);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ProgressMode", value);
			}
		}

		/// <summary>
		/// 进度条的行进间隔。默认是200毫秒
		/// </summary>
		public TimeSpan ProgressInterval
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ProgressInterval", TimeSpan.FromMilliseconds(200));
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ProgressInterval", value);
			}
		}

		/// <summary>
		/// 相关控件的ClientID，和SubmitButton一起Enable或Disable
		/// </summary>
		[DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
		[Category("Appearance")]
		public string RelativeControlID
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "RelativeControlID", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "RelativeControlID", value);
			}
		}

		/// <summary>
		/// 最小步骤
		/// </summary>
		[DefaultValue(0)]
		[Category("Appearance")]
		public int MinStep
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "MinStep", 0);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "MinStep", value);
			}
		}

		/// <summary>
		/// 最小步骤
		/// </summary>
		[DefaultValue(100)]
		[Category("Appearance")]
		public int MaxStep
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "MaxStep", 100);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "MaxStep", value);
			}
		}

		public string AsyncInvoke
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AsyncInvoke", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AsyncInvoke", value);
			}
		}

		/// <summary>
		/// 相关控件。提交时，disable此控件
		/// </summary>
		public IAttributeAccessor RelativeControl
		{
			get
			{
				if (this.relativeControl == null)
				{
					if (string.IsNullOrEmpty(RelativeControlID) == false)
						this.relativeControl = (IAttributeAccessor)WebControlUtility.FindControlByID(WebUtility.GetCurrentPage(), RelativeControlID, true);
				}

				return this.relativeControl;
			}
			set
			{
				this.relativeControl = value;
			}
		}
		#endregion

		/// <summary>
		/// 得到修改父窗口中状态变化的脚本
		/// </summary>
		/// <param name="progress"></param>
		/// <param name="addScriptTag"></param>
		/// <returns></returns>
		public static string GetChangeParentProcessInfoScript(ProcessProgress progress, bool addScriptTag)
		{
			string script = string.Format("parent.document.getElementById(\"{0}\").value=\"{1}\";\nparent.document.getElementById(\"{2}\").click();",
				SubmitButton.ProgressInfoHiddenID,
				WebUtility.CheckScriptString(JSONSerializerExecute.Serialize(progress), false),
				SubmitButton.ProgressInfoChangedButtonID);

			if (addScriptTag)
				script = string.Format("<script type=\"text/javascript\">\n{0}\n</script>", script);

			return script;
		}

		/// <summary>
		/// 得到重置所有父窗口按钮状态的脚本
		/// </summary>
		/// <param name="addScriptTag"></param>
		/// <returns></returns>
		public static string GetResetAllParentButtonsScript(bool addScriptTag)
		{
			string script = string.Format("parent.document.getElementById(\"{0}\").value=\"reset\";\nparent.document.getElementById(\"{1}\").click();",
				SubmitButton.ProgressInfoHiddenID,
				SubmitButton.ProgressInfoChangedButtonID);

			if (addScriptTag)
				script = string.Format("<script type=\"text/javascript\">\n{0}\n</script>", script);

			return script;
		}

		#region Protected
		protected override void OnPreRender(EventArgs e)
		{
			WebUtility.RegisterClientMessageScript();
			WebUtility.RequiredScript(typeof(SubmitButtonScript));

			if (InUpdatePanel() == false)
			{
				string attachEvent = string.Format("window.attachEvent(\"onload\", new Function(\"{0}\"));", GetRegisterButtonScript());
				string addEventListener = string.Format("window.addEventListener(\"onload\", new Function(\"{0}\"), false);", GetRegisterButtonScript());

				Page.ClientScript.RegisterStartupScript(this.GetType(), this.UniqueID,
					string.Format("if (window.attachEvent) {0} else {1}", attachEvent, addEventListener,
					GetRegisterButtonScript()), true);
			}
			else
			{
				ScriptManager.RegisterStartupScript(this, this.GetType(), "resetScript" + this.ClientID,
					GetRegisterButtonScript(),
					true);

				ScriptManager.RegisterStartupScript(this, this.GetType(), "resetScript",
					"SubmitButton.resetAllStates();" + GetRegisterButtonScript(),
					true);
			}

			Page.ClientScript.RegisterHiddenField(SubmitButton.ProgressInfoHiddenID, string.Empty);
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), SubmitButton.ProgressInfoChangedButtonID,
				string.Format("<input type=\"button\" id=\"{0}\" style=\"display:none\" onclick=\"ProgressBarInstance.onProcessInfoChanged();\"/>", SubmitButton.ProgressInfoChangedButtonID));

			base.OnPreRender(e);
		}

		private string GetRegisterButtonScript()
		{
			return string.Format("SubmitButtonIntance._registerButton($get('{0}'));", this.ClientID);
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (this.Page != null)
				this.Page.VerifyRenderingInServerForm(this);

			writer.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
			writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
			writer.AddAttribute(HtmlTextWriterAttribute.Value, this.Text);

			AddBasicAttributes(writer);

			if (this.PopupCaption.IsNotEmpty())
				writer.AddAttribute("popupCaption", Translator.Translate(Define.DefaultCulture, this.PopupCaption));

			if (this.RelativeControl != null)
				writer.AddAttribute("relControlID", ((Control)RelativeControl).ClientID);

			if (this.AsyncInvoke.IsNotEmpty())
				writer.AddAttribute("AsyncInvokeFunction", AsyncInvoke);

			writer.AddAttribute("interval", ((int)ProgressInterval.TotalMilliseconds).ToString());

			writer.AddAttribute("minStep", this.MinStep.ToString());
			writer.AddAttribute("maxStep", this.MaxStep.ToString());

			writer.AddAttribute("progressMode", ((int)this.ProgressMode).ToString());
		}

		private void AddBasicAttributes(HtmlTextWriter writer)
		{
			string strAttrValue = string.Empty;
			if (this.ID != null)
				writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);

			strAttrValue = this.AccessKey;
			if (strAttrValue.Length > 0)
				writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, strAttrValue);

			if (!this.Enabled)
				writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

			int num = this.TabIndex;
			if (num != 0)
				writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, num.ToString(NumberFormatInfo.InvariantInfo));

			strAttrValue = this.ToolTip;
			if (strAttrValue.Length > 0)
				writer.AddAttribute(HtmlTextWriterAttribute.Title, strAttrValue);

			if (this.ControlStyleCreated)
			{
				this.ControlStyle.AddAttributesToRender(writer, this);
			}

			System.Web.UI.AttributeCollection attrs = this.Attributes;
			IEnumerator enumerator = attrs.Keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Current;

				if (string.Compare(text, "onclick", true) != 0)
					writer.AddAttribute(text, attrs[text]);
			}

			if (AutoDisabled)
				AppendOnClickEvent(writer);
		}
		#endregion

		#region Private
		private void AppendOnClickEvent(HtmlTextWriter writer)
		{
			string strClick = Attributes["onclick"];

			if (strClick == null)
				strClick = string.Empty;

			StringBuilder strB = new StringBuilder(256);

			strB.Append(strClick);

			if (strB.Length > 0)
				strB.Append(";");

			strB.Append("if (event.srcElement.alreadyClicked) {event.srcElement.disabled = true;return;}");

			if (((this.Page != null) && this.CausesValidation) && (this.Page.Validators.Count > 0))
				strB.Append("if (typeof(Page_ClientValidate) == 'function') Page_ClientValidate();");

			strB.Append("SubmitButtonIntance._onSubmitButtonClick();");

			writer.AddAttribute(HtmlTextWriterAttribute.Onclick, strB.ToString());
		}

		private bool InUpdatePanel()
		{
			bool result = false;

			Control parent = this.Parent;

			while (parent != null)
			{
				if (parent is UpdatePanel)
				{
					result = true;
					break;
				}

				parent = parent.Parent;
			}

			return result;
		}
		#endregion
	}
}
