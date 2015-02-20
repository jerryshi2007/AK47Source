#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	SignInLogoControl.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
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
	/// ע���ؼ�
	/// </summary>
	[ToolboxData("<{0}:SignInLogoControl runat=server></{0}:SignInLogoControl>")]
	public class SignInLogoControl : WebControl
	{
		/// <summary>
		/// ��Ⱦ�ؼ�
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (AuthenticationModuleBase.IsDeluxeWorksAuthenticate)
			{
				//û����֤�������Ѿ�Ticket�Ϸ�
				if (HttpContext.Current.User == null || HttpContext.Current.User.Identity.IsAuthenticated == false || IsTicketValid || AuthenticationModuleBase.CanLogOff == true)
					Controls.Add(CreateLink());
			}
		}

		/// <summary>
		/// ��¼���ӵ�ͼƬ
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
		/// ע��ͼƬ������
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
		/// ע���󷵻صĵ�ַ
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
		/// ע�����Ƿ�ֱ�ӵ��������ص�ַ�����Ϊfalse����ͣ����ע��ҳ��
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
		/// �Ƿ�ע�����е�Ӧ�ã��˲����Ѿ�û�����壩
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
		/// ������Ŀ�괰��
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
		/// �Ƿ������ģʽ
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
		/// ��Ⱦ�ؼ�
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
		/// ����Ҫ���ص�url��ɾ���·����url����������Ǿ��Ե�ַ������ԣ�
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
