using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;
using System.Web.UI.HtmlControls;

[assembly: WebResource("MCS.Web.Responsive.WebControls.CommandInput.CommandInput.js", "application/x-javascript")]

namespace MCS.Web.Responsive.WebControls
{
	[ToolboxData("<{0}:CommandInput runat=server></{0}:CommandInput>")]
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[ClientScriptResource("MCS.Web.WebControls.CommandInput", "MCS.Web.Responsive.WebControls.CommandInput.CommandInput.js")]
	public class CommandInput : ScriptControlBase
	{
		private static readonly object ControlKey = new object();

		public CommandInput()
			: base(HtmlTextWriterTag.Input)
		{
		}

		protected override void OnInit(EventArgs e)
		{
			ExceptionHelper.TrueThrow(Page.Items.Contains(ControlKey),
				"页面中只能有一个CommandInput控件！");
			Page.Items.Add(ControlKey, null);
			base.OnInit(e);
		}

		public override string ClientID
		{
			get
			{
				return DeluxeScript.C_CommandIputClientID;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("formID")]
		private string FormID
		{
			get
			{
				return Page.Form.ClientID;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("isPostBack")]
		private bool IsPostBack
		{
			get
			{
				return Page.IsPostBack;
			}
		}

		/// <summary>
		/// 客户端响应commandInput函数
		/// </summary>
		[ScriptControlEvent]
		[ClientPropertyName("commandInput")]
		[Description("客户端响应callbackComplete函数")]
		public string OnClientCommandInput
		{
			get
			{
				return this.GetPropertyValue<string>("OnClientCommandInput", string.Empty);
			}

			set
			{
				this.SetPropertyValue<string>("OnClientCommandInput", value);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
			{
				writer.Write("CommandInputControl");
			}
			else
			{
				this.Attributes["type"] = "button";
				this.Style["display"] = "none";

				base.Render(writer);
			}
		}
	}
}
