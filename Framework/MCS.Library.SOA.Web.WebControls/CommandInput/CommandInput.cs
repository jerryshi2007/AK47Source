using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.CommandInput.CommandInput.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript))]
	[RequiredScript(typeof(HBCommonScript))]
	[ClientScriptResource("MCS.Web.WebControls.CommandInput",
	   "MCS.Web.WebControls.CommandInput.CommandInput.js")]
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
				this.Attributes.Add("type", "hidden");
				base.Render(writer);
			}
		}
	}
}
