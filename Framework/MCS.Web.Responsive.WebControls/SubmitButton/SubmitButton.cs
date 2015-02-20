using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using MCS.Web.Responsive.Library;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using MCS.Library.Globalization;
using System.Collections;
using System.Globalization;

[assembly: WebResource("MCS.Web.Responsive.WebControls.SubmitButton.SubmitButton.css", "text/css")]

namespace MCS.Web.Responsive.WebControls
{
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

		#region 属性

		[Category("Appearance"), DefaultValue("")]
		public string PopupCaption
		{
			get
			{
				return ViewState.GetViewStateValue<string>("PopupCaption", string.Empty);
			}

			set
			{
				ViewState.SetViewStateValue("PopupCaption", value);
			}
		}

		/// <summary>
		/// 是否自动在提交时Disabled
		/// </summary>
		[Category("Appearance"), DefaultValue(true)]
		public bool AutoDisabled
		{
			get
			{
				return ViewState.GetViewStateValue("AutoDisabled", true);
			}
			set
			{
				ViewState.SetViewStateValue("AutoDisabled", value);
			}
		}

		/// <summary>
		/// 进度条的模式
		/// </summary>
		[Category("Appearance"), Description("Style of the progress bar"), DefaultValue(SubmitButtonProgressMode.ByTimeInterval)]
		public SubmitButtonProgressMode ProgressMode
		{
			get
			{
				return ViewState.GetViewStateValue("ProgressMode", SubmitButtonProgressMode.ByTimeInterval);
			}
			set
			{
				ViewState.SetViewStateValue("ProgressMode", value);
			}
		}

		/// <summary>
		/// 进度条的行进间隔。默认是200毫秒
		/// </summary>
		[DefaultValue(typeof(TimeSpan), "00:00:00.0000200")]
		public TimeSpan ProgressInterval
		{
			get
			{
				return ViewState.GetViewStateValue("ProgressInterval", TimeSpan.FromMilliseconds(200));
			}
			set
			{
				ViewState.SetViewStateValue("ProgressInterval", value);
			}
		}

		/// <summary>
		/// 相关控件的ClientID，和SubmitButton一起Enable或Disable
		/// </summary>
		[DefaultValue(""), IDReferenceProperty(), Themeable(false), TypeConverter(typeof(AttributeAccessorControlIDConverter))]
		[Category("Appearance")]
		public string RelativeControlID
		{
			get
			{
				return ViewState.GetViewStateValue("RelativeControlID", string.Empty);
			}
			set
			{
				ViewState.SetViewStateValue("RelativeControlID", value);
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
				return ViewState.GetViewStateValue("MinStep", 0);
			}
			set
			{
				ViewState.SetViewStateValue("MinStep", value);
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
				return ViewState.GetViewStateValue("MaxStep", 100);
			}
			set
			{
				ViewState.SetViewStateValue("MaxStep", value);
			}
		}

		[DefaultValue("")]
		public string AsyncInvoke
		{
			get
			{
				return ViewState.GetViewStateValue("AsyncInvoke", string.Empty);
			}
			set
			{
				ViewState.SetViewStateValue("AsyncInvoke", value);
			}
		}

		/// <summary>
		/// 相关控件。提交时，disable此控件
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IAttributeAccessor RelativeControl
		{
			get
			{
				if (this.relativeControl == null)
				{
					if (string.IsNullOrEmpty(RelativeControlID) == false)
					{
						this.relativeControl = (IAttributeAccessor)WebUtility.GetCurrentPage().FindTargetControl(RelativeControlID, true);
					}
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
			string script = "parent.document.getElementById('" +
				SubmitButton.ProgressInfoHiddenID + @"').value='" + JSONSerializerExecute.Serialize(progress) +
				"';\r\n parent.document.getElementById('" + SubmitButton.ProgressInfoChangedButtonID + "').click();";

			if (addScriptTag)
				script = "<script type=\"text/javascript\">" + script + "</script>";

			return script;
		}

		/// <summary>
		/// 得到重置所有父窗口按钮状态的脚本
		/// </summary>
		/// <param name="addScriptTag"></param>
		/// <returns></returns>
		public static string GetResetAllParentButtonsScript(bool addScriptTag)
		{
			string script = string.Format("parent.document.getElementById(\"{0}\").value=\"reset\"; parent.document.getElementById(\"{1}\").click();",
				SubmitButton.ProgressInfoHiddenID,
				SubmitButton.ProgressInfoChangedButtonID);

			if (addScriptTag)
				script = "<script type=\"text/javascript\">" + script + "</script>";

			return script;
		}

		#region Protected
		protected override void OnPreRender(EventArgs e)
		{
			//WebUtility.RegisterClientMessageScript();
			//TODO: aaaa
			WebUtility.RequiredScript(typeof(SubmitButtonScript));

			if (InUpdatePanel() == false)
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), this.UniqueID, "SubmitButton.attachButton('" + this.UniqueID + "');", true);
			}
			else
			{
				ScriptManager.RegisterStartupScript(this, this.GetType(), "resetScriptFor" + this.ClientID, "SubmitButton.attachButton('" + this.UniqueID + "');", true);

				ScriptManager.RegisterStartupScript(this, this.GetType(), "resetScript", "SubmitButton.resetAllStates(); SubmitButton.attachButton('" + this.UniqueID + "');", true);
			}

			Page.ClientScript.RegisterHiddenField(SubmitButton.ProgressInfoHiddenID, string.Empty);
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), SubmitButton.ProgressInfoChangedButtonID,
				string.Format("<input type=\"button\" id=\"{0}\" style=\"display:none\" onclick=\"ProgressBarInstance.onProcessInfoChanged();\"/>", SubmitButton.ProgressInfoChangedButtonID));

			base.OnPreRender(e);
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
				writer.AddAttribute("data-popup-caption", Translator.Translate(Define.DefaultCulture, this.PopupCaption));

			if (this.RelativeControl != null)
				writer.AddAttribute("data-rel-control", ((Control)RelativeControl).ClientID);

			if (this.AsyncInvoke.IsNotEmpty())
				writer.AddAttribute("data-async-function", AsyncInvoke);

			writer.AddAttribute("data-interval", ((int)ProgressInterval.TotalMilliseconds).ToString());

			writer.AddAttribute("data-min-step", this.MinStep.ToString());
			writer.AddAttribute("data-max-step", this.MaxStep.ToString());

			writer.AddAttribute("data-progress-mode", ((int)this.ProgressMode).ToString());
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
			bool causeValidation = (this.Page != null && this.CausesValidation && this.Page.Validators.Count > 0);

			if (string.IsNullOrWhiteSpace(strClick))
				strClick = causeValidation ? "return SubmitButton.handleClick(this,event, true);" : "return SubmitButton.handleClick(this,event,false);";
			else
				strClick += (causeValidation ? "; return SubmitButton.handleClick(this,event,true);" : "; return SubmitButton.handleClick(this,event,false);");

			writer.AddAttribute(HtmlTextWriterAttribute.Onclick, strClick);

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

	public enum SubmitButtonProgressMode
	{
		/// <summary>
		/// 按照时间间隔
		/// </summary>
		ByTimeInterval = 0,

		/// <summary>
		/// 按照步骤（真实步骤）
		/// </summary>
		BySteps = 1,

		/// <summary>
		/// 连续的
		/// </summary>
		Continues = 2
	}
}
