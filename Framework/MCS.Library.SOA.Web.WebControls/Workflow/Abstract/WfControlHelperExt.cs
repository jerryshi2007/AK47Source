using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
	internal static class WfControlHelperExt
	{
		/// <summary>
		/// 初始化表单页面Error事件
		/// </summary>
		/// <param name="page"></param>
		public static void InitWfControlPostBackErrorHandler(Page page)
		{
			page.Error += new EventHandler(page_Error);
		}

		/// <summary>
		/// 注册当前用户权限信息的脚本
		/// </summary>
		/// <param name="page"></param>
		public static void RegisterCurrentUserAppAuthInfoScript(Page page)
		{
			page.ClientScript.RegisterStartupScript(typeof(WfControlHelperExt),
				"CurrentUserAppAuthInfoScript",
				GetCurrentUserAppAuthInfoScript(),
				true);
		}

		/// <summary>
		/// 通过Message显示错误信息
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void page_Error(object sender, EventArgs e)
		{
			Page page = (Page)sender;

			ScriptManager sm = ScriptManager.GetCurrent(page);

			if (page.IsPostBack && page.IsCallback == false && (sm == null || sm.IsInAsyncPostBack == false))
			{
				WfControlBase.ResponsePostBackPlaceHolder();

				System.Exception ex = HttpContext.Current.Error;

				if (ex != null)
				{
					HttpContext.Current.ClearError();

					System.Exception innerEx = ExceptionHelper.GetRealException(ex);


					if (innerEx is ApplicationInformationException)
						WebUtility.ResponseShowClientMessageScriptBlock(
						innerEx.Message,
						innerEx.StackTrace,
						Translator.Translate(Define.DefaultCulture, "信息"));
					else
						WebUtility.ResponseShowClientErrorScriptBlock(
							innerEx.Message,
							innerEx.StackTrace,
							Translator.Translate(Define.DefaultCulture, "错误"));

					page.Response.Write(
						"<script type='text/javascript'>if (parent.document.all('wfOperationNotifier')) parent.document.all('wfOperationNotifier').value = '';</script>");

					page.Response.End();
				}
			}
		}

		private static string GetCurrentUserAppAuthInfoScript()
		{
			string isAdmin = "false";
			string appAuthInfo = "[]";

			if (DeluxePrincipal.IsAuthenticated)
			{
				if (RolesDefineConfig.GetConfig().IsCurrentUserInRoles(DeluxeIdentity.CurrentUser, "ProcessAdmin"))
					isAdmin = "true";

				WfApplicationAuthCollection authInfo = WfApplicationAuthAdapter.Instance.GetUserApplicationAuthInfo(DeluxeIdentity.CurrentUser);

				appAuthInfo = JSONSerializerExecute.Serialize(authInfo);
			}

			string script = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Web.WebControls.Workflow.Abstract.currentUserPermissions.js");

			script = script.Replace("$_currentUserIsAdmin$", isAdmin);
			script = script.Replace("$_currentUserAppAuthInfoString$", appAuthInfo);

			return script;
		}
	}
}
