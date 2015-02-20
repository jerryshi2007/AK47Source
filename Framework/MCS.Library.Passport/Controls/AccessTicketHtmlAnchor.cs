using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 带AccessTicket的HtmlAnchor
	/// </summary>
	[DefaultEvent("ServerClick")]
	[SupportsEventValidation]
	[ToolboxData("<{0}:AccessTicketHtmlAnchor runat=server></{0}:AccessTicketHtmlAnchor>")]
	public class AccessTicketHtmlAnchor : HtmlAnchor
	{
		/// <summary>
		/// 生成票据时，是否自动将相对地址转换为绝对地址
		/// </summary>
		[DefaultValue(true)]
		[Description("生成票据时，是否自动将相对地址转换为绝对地址")]
		public bool AutoMakeAbsolute
		{
			get
			{
				bool result = true;

				if (ViewState["AutoMakeAbsolute"] != null)
					result = (bool)ViewState["AutoMakeAbsolute"];

				return result;
			}
			set
			{
				ViewState["AutoMakeAbsolute"] = value;
			}
		}

		/// <summary>
		/// 当客户端得到AccessTicket后的
		/// </summary>
		public string OnClientAccquiredAccessTicket
		{
			get
			{
				string result = (string)ViewState["OnClientAccquiredAccessTicket"];

				if (result == null)
					result = string.Empty;

				return result;
			}
			set
			{
				ViewState["OnClientAccquiredAccessTicket"] = value;
			}
		}

		/// <summary>
		/// 注册脚本
		/// </summary>
		/// <param name="page"></param>
		public static void RegisterScript(Page page)
		{
			page.NullCheck("page");

			string script = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
				"MCS.Library.Passport.Controls.AccessTicketScript.htm");

			page.ClientScript.RegisterClientScriptBlock(typeof(AccessTicketHtmlAnchor), "AccessTicketScript", script);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			RegisterScript(this.Page);

			base.OnPreRender(e);
		}

		/// <summary>
		/// 渲染
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
			{
				writer.Write("AccessTicketHtmlAnchor");
			}
			else
			{
				this.Attributes["onclick"] = "onAccquireAccessTicketLinkClick()";
				this.Attributes["AutoMakeAbsolute"] = this.AutoMakeAbsolute.ToString().ToLower();

				base.Render(writer);
			}
		}
	}
}
