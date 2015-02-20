using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;
using System.Reflection;
using MCS.Library.Core;
using MCS.Web.WebControls;
using MCS.Web.Library;

[assembly: WebResource("MCS.Web.WebControls.WebBrowserWrapper.WebBrowserWrapper.js", "application/x-javascript")]


namespace MCS.Web.WebControls
{
	/// <summary>
	/// WebBrowser包装
	/// </summary>
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:WebBrowserWrapper runat=server></{0}:WebBrowserWrapper>")]
	public class WebBrowserWrapper : WebControl
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public WebBrowserWrapper()
			: base(HtmlTextWriterTag.Div)
		{ }

		/// <summary>
		/// 当前上下文中查找控件实例
		/// </summary>
		public static WebBrowserWrapper Current
		{
			get
			{
				return (WebBrowserWrapper)HttpContext.Current.Items["WebBrowserWrapper"];
			}
		}

		/// <summary>
		/// 注册一个控件
		/// </summary>
		/// <param name="container"></param>
		public static void Register(Control container)
		{
			container.NullCheck("container");

			if (WebBrowserWrapper.Current == null)
			{
				WebBrowserWrapper wb = new WebBrowserWrapper();

				container.Controls.Add(wb);
			}
		}

		/// <summary>
		/// 重载OnInit
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ExceptionHelper.TrueThrow(WebBrowserWrapper.Current != null && WebBrowserWrapper.Current != this,
				"不能在页面上创建两个WebBrowserWrapper控件");

			HttpContext.Current.Items["WebBrowserWrapper"] = this;
		}

		/// <summary>
		/// 重写OnPreRender
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			RegisterWebBrowserObject();

			WebUtility.RequiredScript(typeof(WebBrowserWrapperScript));
		}

		private void RegisterWebBrowserObject()
		{
			string webBrowserTxt = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
				"MCS.Web.WebControls.WebBrowserWrapper.WebBrowserWrapper.txt");

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "StringWebBrowser", webBrowserTxt, false);
		}
	}
}
