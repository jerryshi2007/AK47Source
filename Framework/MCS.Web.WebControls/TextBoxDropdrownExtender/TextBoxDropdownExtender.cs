using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.Core;
using System.Web.UI.HtmlControls;
using MCS.Web.Library.Script;
using System.Reflection;

[assembly: WebResource("MCS.Web.WebControls.TextBoxDropdrownExtender.dropdown.gif", "image/gif")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 为TextBox控件扩展的下拉框，便于文字输入
	/// </summary>
	[ToolboxData("<{0}:TextBoxDropdownExtender runat=server></{0}:TextBoxDropdownExtender>")]
	public class TextBoxDropdownExtender : ListBox
	{
        private const string INNERFRAMEID = "__tbDdlExtInnerFrame";
		private TextBox targetControl = null;

		/// <summary>
		/// 
		/// </summary>
		public TextBoxDropdownExtender()
		{
			this.AutoPostBack = false;
		}

		/// <summary>
		/// 目标控件（TextBox）的ID
		/// </summary>
		[DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(TextBoxControlConverter))]
		[Category("Appearance")]
		public string TargetControlID
		{
			get
			{
				return ViewState.GetViewStateValue("TargetControlID", string.Empty);
			}
			set
			{
				ViewState.SetViewStateValue("TargetControlID", value);
				this.targetControl = null;
			}
		}

		/// <summary>
		/// 目标控件实例。通常由目标控件的ID来计算出实例
		/// </summary>
		[Browsable(false)]
		public TextBox TargetControl
		{
			get
			{
				if (this.targetControl == null)
				{
					if (this.TargetControlID.IsNotEmpty())
						this.targetControl = (TextBox)WebControlUtility.FindControlByID(WebUtility.GetCurrentPage(), this.TargetControlID, true);
				}

				return this.targetControl;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			WebUtility.RequiredScript(typeof(ControlBaseScript));

			string script = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Web.WebControls.TextBoxDropdrownExtender.TextBoxDropdownExtender.js");

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "TextBoxDropdrownExtender", script, true);
			RegisterStartupScript();

			base.OnPreRender(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			RenderDropdownImage(writer);
            //RenderIframe(writer);
			RenderListBox(writer);
		}

		private void RegisterStartupScript()
		{
			StringBuilder strB = new StringBuilder();

			if (this.TargetControl != null)
			{
				strB.AppendFormat("document.getElementById(\"{0}\").attachEvent(\"onkeydown\", onTextBoxDropdownExtenderTextBoxKeyDown);",
					this.TargetControl.ClientID);

				this.TargetControl.Attributes["listBoxID"] = this.ClientID;
                this.TargetControl.Attributes["iframeID"] = INNERFRAMEID;
			}

			Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID, strB.ToString(), true);

            Page.ClientScript.RegisterStartupScript(this.GetType(), "InnerFrame",
            string.Format("<iframe id=\"{0}\" name=\"{1}\" style=\"display:none;position:absolute;z-index:0\"></iframe>",
            INNERFRAMEID, INNERFRAMEID));

		}

		private void RenderDropdownImage(HtmlTextWriter writer)
		{
			HtmlImage image = new HtmlImage();

			image.Src = Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.WebControls.TextBoxDropdrownExtender.dropdown.gif");
			image.Style["cursor"] = "pointer";

			image.Attributes["listBoxID"] = this.ClientID;
            image.Attributes["iframeID"] = INNERFRAMEID;

			image.Attributes["onclick"] = "onTextBoxDropdownExtenderDropdown();";

			if (this.TargetControl != null)
			{
				image.Attributes["textBoxID"] = this.TargetControl.ClientID;

				if (this.TargetControl.ReadOnly || this.TargetControl.Enabled == false)
					image.Style["display"] = "none";
			}

			writer.Write(WebControlUtility.GetControlHtml(image));
		}

		private void RenderListBox(HtmlTextWriter writer)
		{
			StringBuilder strB = new StringBuilder(1024);

			this.Style["display"] = "none";
			this.Style["position"] = "absolute";
			this.Style["z-index"] = "1000";

			if (this.TargetControl != null)
				this.Attributes["textBoxID"] = this.TargetControl.ClientID;

			this.Attributes["onblur"] = "onTextBoxDropdownExtenderDropdownBlur();";
			this.Attributes["onchange"] = "onTextBoxDropdownExtenderDropdownChange();";
			this.Attributes["onclick"] = "onTextBoxDropdownExtenderDropdownClick();";
			this.Attributes["onkeydown"] = "onTextBoxDropdownExtenderDropdownKeyDown();";

			using (StringWriter sw = new StringWriter(strB))
			{
				using (HtmlTextWriter w = new HtmlTextWriter(sw))
				{
					base.Render(w);
				}
			}

			writer.Write(strB.ToString());
		}

        private void RenderIframe(HtmlTextWriter writer)
        {
            StringBuilder sbIframeHtml = new StringBuilder();
            sbIframeHtml.Append("<iframe style=\"display:block;position:absolute;z-index:1000\"></iframe>");
            
            LiteralControl iframe = new LiteralControl(sbIframeHtml.ToString());
            //iframe.Style["display"] = "none";
            //iframe.Style["position"] = "absolute";
            //iframe.Style["z-index"] = "1000";
            
            writer.Write(WebControlUtility.GetControlHtml(iframe));
        }
	}
}
