using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Principal;
using MCS.Web.Responsive.Library;
using MCS.Library.Passport;
using MCS.Library.Passport.Social.Configuration;

namespace ResponsivePassportService.TestPages
{
	public partial class PrincipalTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			signInLogo.ReturnUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped);
		}

		protected override void OnPreRender(EventArgs e)
		{
			string openID = this.Request.Form.GetValue("OpenID", string.Empty);

			if (openID.IsNotEmpty())
				RemoveBinding(openID);

			if (Request.IsAuthenticated)
			{
				ShowPrincipalInfo();
				principalOP.Visible = true;
			}

			this.RenderBindings(this.bindingsContainer);

			base.OnPreRender(e);
		}

		private void RemoveBinding(string openID)
		{
			PassportSignInSettings.GetConfig().PersistOpenIDBinding.RemoveOpenIDBinding(openID);
		}

		private void ShowPrincipalInfo()
		{
			ShowSinglePrincipalInfo(principalInfo, "User Logon Name", DeluxeIdentity.CurrentUser.LogOnName);
			ShowSinglePrincipalInfo(principalInfo, "User Display Name", DeluxeIdentity.CurrentUser.DisplayName);

			if (DeluxeIdentity.CurrentUser.TopOU != null)
				ShowSinglePrincipalInfo(principalInfo, "Top OU",
					string.Format("{0}({1})",
					DeluxeIdentity.CurrentUser.TopOU.DisplayName,
					DeluxeIdentity.CurrentUser.TopOU.FullPath));

			ShowSinglePrincipalInfo(principalInfo, "Simulated Time", DateTime.Now.SimulateTime().ToString("yyyy-MM-dd HH:mm:ss"));
			ShowSinglePrincipalInfo(principalInfo, "Use Current Time", TimePointContext.Current.UseCurrentTime.ToString());
			ShowSinglePrincipalInfo(principalInfo, "Simulated TimePoint", TimePointContext.Current.SimulatedTime.ToString("yyyy-MM-dd HH:mm:ss"));
            ShowSinglePrincipalInfo(principalInfo, "Tenant Code", TenantContext.Current.TenantCode);

			ShowBindingLinks();
		}

		private void ShowBindingLinks()
		{
			OpenIDBindingCollection bindings =
				PassportSignInSettings.GetConfig().PersistOpenIDBinding.GetBindingsByUserID(DeluxeIdentity.CurrentUser.ID);

			foreach(OpenIDBinding binding in bindings)
			{
				string openIDUrl = string.Format("../AnonymousTestPage/OpenIDIdentityTest.aspx?openID={0}", binding.OpenID);

				string anchor = string.Format("<a href=\"{0}\">{0}</a>", openIDUrl);

				string name = string.Format("OpenID({0})", binding.OpenIDType);

				ShowSinglePrincipalHtmlInfo(principalInfo, name, anchor);
			}	
		}

		private void ShowSinglePrincipalHtmlInfo(Control parent, string name, string html)
		{
			parent.CreateSubItems("dt", dt => dt.InnerText = name);
			parent.CreateSubItems("dd", dd => dd.InnerHtml = html);
		}

		private void ShowSinglePrincipalInfo(Control parent, string name, string data)
		{
			parent.CreateSubItems("dt", dt => dt.InnerText = name);
			parent.CreateSubItems("dd", dd => dd.InnerText = data);
		}

		private void RenderBindings(Control parent)
		{
			OpenIDBindingCollection bindings =
				PassportSignInSettings.GetConfig().PersistOpenIDBinding.GetBindingsByUserID(DeluxeIdentity.CurrentUser.ID);

			SocialPassportSettings settings = SocialPassportSettings.GetConfig();

			foreach (OpenIDBinding binding in bindings)
			{
				SocialConnectionConfigurationElement element = settings.Connections[binding.OpenIDType];

				if (element != null)
					RenderBindingInfo(parent, binding, element);
			}
		}

		private void RenderBindingInfo(Control parent, OpenIDBinding binding, SocialConnectionConfigurationElement element)
		{
			parent.CreateSubItems("div", row =>
				{
					row.Attributes["class"] = "col-md-12";

					row.CreateSubItems("button", btn =>
						{
							btn.Attributes["class"] = "btn btn-warning";
							btn.Attributes["name"] = "OpenID";
							btn.Attributes["value"] = binding.OpenID;

							btn.CreateSubImage(this.ResolveUrl(element.LogoPath.ToString()));

							btn.CreateSubItems("span", span => span.InnerText = string.Format("解除{0}绑定", element.Description));
						});
				}
			);
		}

		protected void clearPrincipal_Click(object sender, EventArgs e)
		{
			DeluxePrincipal.Current.ClearCacheInWebApp();

			Response.Redirect(this.Request.Url.ToString());
		}
	}
}