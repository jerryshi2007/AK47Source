using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Web.Library;
//using MCS.Web.WebControls;
using MCS.Library.Globalization;
using PermissionCenter.Common;

namespace PermissionCenter.Anonymous
{
	public partial class LogOffPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			string sessionID = Request.QueryString["asid"];
			string appID = Request.QueryString["appID"];
			string returnUrl = Request.QueryString["ru"];
			string logOffUrl = Request.QueryString["loa"];
			string autoRedirectParams = Request.QueryString["lar"];
			string callbackUrl = Request.QueryString["lou"];
			string windowsIntegrated = Request.QueryString["wi"];
			string lastUserID = Request.QueryString["lu"];

			if (sessionID != null && sessionID != string.Empty)
			{
				if (logOffUrl != null && logOffUrl.ToLower() == "y")
				{
					LogOffAllAPP(sessionID, appID, callbackUrl);

					if (returnUrl != null && returnUrl != string.Empty)
					{
						returnUrl = ModifyReturnUrl(returnUrl,
							lastUserID,
							windowsIntegrated != null && windowsIntegrated.ToLower() == "true");

						returnHref.HRef = returnUrl;

						if (autoRedirectParams != null)
							autoRedirect.Value = autoRedirectParams;
					}
				}
				else
				{
					if (returnUrl != null && returnUrl != string.Empty)
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

			List<AppLogOffCallBackUrl> list =
				PassportSignInSettings.GetConfig().PersistSignInInfo.GetAllRelativeAppsLogOffCallBackUrl(sessionID);

			if (AppLogOffCallBackUrlExist(list, appID, callbackUrl) == false)
			{
				AppLogOffCallBackUrl au = new AppLogOffCallBackUrl();

				au.AppID = appID;
				au.LogOffCallBackUrl = callbackUrl;

				list.Add(au);
			}

			HtmlTable table = new HtmlTable();
			tableContainer.InnerHtml = string.Empty;
			tableContainer.Controls.Add(table);

			foreach (AppLogOffCallBackUrl au in list)
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

			PassportSignInSettings.GetConfig().PersistSignInInfo.DeleteRelativeSignInInfo(sessionID);
			PassportManager.ClearSignInCookie();
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