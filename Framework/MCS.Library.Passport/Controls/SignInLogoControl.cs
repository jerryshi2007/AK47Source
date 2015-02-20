#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	SignInLogoControl.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.Passport;
using MCS.Library.Passport.Properties;


namespace MCS.Library.Web.Controls
{
	/// <summary>
	/// 注销控件
	/// </summary>
	[ToolboxData("<{0}:SignInLogoControl runat=server></{0}:SignInLogoControl>")]
	public class SignInLogoControl : WebControl
	{
		/// <summary>
		/// 渲染控件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (AuthenticationModuleBase.IsDeluxeWorksAuthenticate)
			{
				//没有认证过或者已经Ticket合法
				if (HttpContext.Current.User == null || HttpContext.Current.User.Identity.IsAuthenticated == false || IsTicketValid || AuthenticationModuleBase.CanLogOff == true)
					Controls.Add(CreateLink());
			}
		}

		/// <summary>
		/// 登录链接的图片
		/// </summary>
		[UrlProperty, Editor(typeof(UrlEditor), typeof(UITypeEditor))]
		public string SignInImage
		{
			get
			{
				string result = (string)ViewState["SignInImage"];

				if (result == null)
					result = Page.ClientScript.GetWebResourceUrl(this.GetType(),
								"MCS.Library.Passport.Resources.signin.gif");

				return result;
			}
			set
			{
				ViewState["SignInImage"] = value;
			}
		}

		/// <summary>
		/// 注销图片的链接
		/// </summary>
		[UrlProperty, Editor(typeof(UrlEditor), typeof(UITypeEditor))]
		public string SignOutImage
		{
			get
			{
				string result = (string)ViewState["SignOutImage"];

				if (result == null)
					result = Page.ClientScript.GetWebResourceUrl(this.GetType(),
								Translator.Translate(Common.CultureCategory, "MCS.Library.Passport.Resources.signout.gif"));

				return result;
			}
			set
			{
				ViewState["SignOutImage"] = value;
			}
		}

		/// <summary>
		/// 注销后返回的地址
		/// </summary>
		public string ReturnUrl
		{
			get
			{
				string result = string.Empty;

				if (ViewState["ReturnUrl"] != null)
					result = (string)ViewState["ReturnUrl"];

				return result;
			}
			set
			{
				ViewState["ReturnUrl"] = value;
			}
		}

		/// <summary>
		/// 注销后，是否直接导航到返回地址。如果为false，则停留在注销页面
		/// </summary>
		public bool AutoRedirect
		{
			get
			{
				bool result = true;

				if (ViewState["AutoRedirect"] != null)
					result = (bool)ViewState["AutoRedirect"];

				return result;
			}
			set
			{
				ViewState["AutoRedirect"] = value;
			}
		}

		/// <summary>
		/// 是否注销所有的应用（此参数已经没有意义）
		/// </summary>
		public bool LogOffAll
		{
			get
			{
				bool result = true;

				if (ViewState["LogOffAll"] != null)
					result = (bool)ViewState["LogOffAll"];

				return result;
			}
			set
			{
				ViewState["LogOffAll"] = value;
			}
		}

		/// <summary>
		/// 导航的目标窗口
		/// </summary>
		public string Target
		{
			get
			{
				string result = string.Empty;

				if (ViewState["Target"] != null)
					result = (string)ViewState["Target"];

				return result;
			}
			set
			{
				ViewState["Target"] = value;
			}
		}
		/// <summary>
		/// 是否是设计模式
		/// </summary>
		protected static bool IsDesignMode
		{
			get
			{
				AppDomain domain = AppDomain.CurrentDomain;
				return string.Compare(domain.FriendlyName, "DefaultDomain", true) == 0;
			}
		}

		/// <summary>
		/// 渲染控件
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			if (IsDesignMode)
				writer.Write(string.Format("<img src=\"{0}\"/>",
					Page.ClientScript.GetWebResourceUrl(this.GetType(),
					"MCS.Library.Passport.Resources.signin.gif")));
			else
				base.Render(writer);
		}

		private bool IsTicketValid
		{
			get
			{
				bool result = false;
				bool fromCookie = false;

				ITicket ticket = PassportManager.GetTicket(out fromCookie);

				result = ticket != null && ticket.IsValid();

				return result;
			}
		}

		private HtmlAnchor CreateLink()
		{
			HtmlAnchor anchor = new HtmlAnchor();
			anchor.Style["border"] = "none";

			HtmlImage img = new HtmlImage();
			img.Style["border"] = "none";

			string ru = this.ResolveUrl(ReturnUrl);

			if (ru.IsNullOrEmpty())
				ru = UriHelper.RemoveUriParams(Page.Request.Url.ToString(), "t");

			ru = ChangeToAbsoluteUrl(ru);
			anchor.HRef = PassportManager.GetLogOnOrLogOffUrl(ru, this.AutoRedirect, this.LogOffAll);
			anchor.Target = this.Target;

			if (IsTicketValid)
				img.Src = SignOutImage;
			else
				img.Src = SignInImage;

			anchor.Controls.Add(img);

			return anchor;
		}

		/// <summary>
		/// 将需要返回的url变成绝对路径的url（如果本来是绝对地址，则忽略）
		/// </summary>
		/// <param name="ru"></param>
		/// <returns></returns>
		private static string ChangeToAbsoluteUrl(string ru)
		{
			string result = ru;

			Uri rUri = new Uri(ru, UriKind.RelativeOrAbsolute);

			if (rUri.IsAbsoluteUri == false)
				result = HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped)
					+ Path.Combine("/", ru.ToString());

			return result;
		}
	}
}
