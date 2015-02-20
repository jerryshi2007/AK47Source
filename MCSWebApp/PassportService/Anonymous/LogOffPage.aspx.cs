using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.Passport;
using MCS.Web.Library;

namespace MCS.Web.Passport
{
	public partial class LogOffPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			string sessionID = Request.QueryString.GetValue("asid", true, string.Empty);

			string appID = Request.QueryString.GetValue("appID", true, string.Empty);
			string returnUrl = Request.QueryString.GetValue("ru", true, string.Empty);
			bool logOffAll = Request.QueryString.GetValue("loa", true, true);

			bool autoRedirectParams = Request.QueryString.GetValue("lar", true, false);
			string callbackUrl = Request.QueryString.GetValue("lou", true, string.Empty);
			bool windowsIntegrated = Request.QueryString.GetValue("wi", true, false);
			string lastUserID = Request.QueryString.GetValue("lu", true, string.Empty);

			if (sessionID.IsNotEmpty())
			{
				if (logOffAll)
				{
					LogOffAllAPP(sessionID, appID, callbackUrl);

					if (returnUrl.IsNotEmpty())
					{
						returnUrl = ModifyReturnUrl(returnUrl,
							lastUserID,
							windowsIntegrated);

						returnHref.HRef = returnUrl;

						autoRedirect.Value = autoRedirectParams.ToString();
					}
				}
				else
				{
					if (returnUrl.IsNotEmpty())
						Response.Redirect(returnUrl);
				}
			}

			Response.Expires = -1;
		}

		protected override void OnPreRender(EventArgs e)
		{
			returnHref.InnerText = Translate("注销完成，点击这里返回应用");
			base.OnPreRender(e);
		}

		private string ModifyReturnUrl(string returnUrl, string lastUserID, bool windowsIntegrated)
		{
			string result = returnUrl;

			if (windowsIntegrated)
			{
				result = string.Format("../Integration/ChangeUser.aspx?lastUserID={0}&ru={1}",
					HttpUtility.UrlEncode(lastUserID ?? string.Empty),
					HttpUtility.UrlEncode(returnUrl));
			}

			return result;
		}

		private void LogOffAllAPP(string sessionID, string appID, string callbackUrl)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(sessionID, "sessionID");
			ExceptionHelper.CheckStringIsNullOrEmpty(appID, "appID");
			ExceptionHelper.CheckStringIsNullOrEmpty(callbackUrl, "callbackUrl");

			List<AppLogOffCallBackUrl> urls =
				PassportSignInSettings.GetConfig().PersistSignInInfo.GetAllRelativeAppsLogOffCallBackUrl(sessionID);

			if (AppLogOffCallBackUrlExist(urls, appID, callbackUrl) == false)
			{
				AppLogOffCallBackUrl au = new AppLogOffCallBackUrl();

				au.AppID = appID;
				au.LogOffCallBackUrl = callbackUrl;

				urls.Add(au);
			}

			this.RenderCallBackUrls(urls);

			PassportSignInSettings.GetConfig().PersistSignInInfo.DeleteRelativeSignInInfo(sessionID);
			PassportManager.ClearSignInCookie();
		}

		private void RenderCallBackUrls(List<AppLogOffCallBackUrl> urls)
		{
			HtmlTable table = new HtmlTable();
			this.tableContainer.InnerHtml = string.Empty;
			this.tableContainer.Controls.Add(table);

			foreach (AppLogOffCallBackUrl au in urls)
			{
				HtmlTableRow tRow = new HtmlTableRow();
				table.Controls.Add(tRow);

				HtmlTableCell cell = new HtmlTableCell();
				cell.InnerText = " ";
				tRow.Controls.Add(cell);

				cell = new HtmlTableCell();
				tRow.Controls.Add(cell);

				HtmlImage img = new HtmlImage();
				img.Align = "absmiddle";
				img.Src = au.LogOffCallBackUrl;
				img.Alt = img.Src;
				cell.Controls.Add(img);

				cell = new HtmlTableCell();
				cell.InnerText = string.Format(Translate("退出应用程序{0}"), au.AppID);
				tRow.Controls.Add(cell);
			}
		}

		private static string Translate(string sourceText)
		{
			CultureInfo culture = new CultureInfo(GlobalizationWebHelper.GetUserDefaultLanguage());

			return Translator.Translate(Define.DefaultCategory, sourceText, culture);
		}

		private bool AppLogOffCallBackUrlExist(List<AppLogOffCallBackUrl> list, string appID, string callbackUrl)
		{
			bool result = false;

			for (int i = 0; i < list.Count; i++)
			{
				AppLogOffCallBackUrl au = list[i];

				if (string.Compare(au.AppID, appID, true) == 0 &&
					string.Compare(au.LogOffCallBackUrl, callbackUrl, true) == 0)
				{
					result = true;
					break;
				}
			}

			return result;
		}
	}
}
