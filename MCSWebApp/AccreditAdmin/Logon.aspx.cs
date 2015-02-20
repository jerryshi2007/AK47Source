#region using

using System;
using System.Text;
using System.Web;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Web.Security;
using System.Web.SessionState;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.LogAdmin;
using MCS.Library.Accredit.OguAdmin.Interfaces;
using MCS.Library.Core;
using MCS.Library.Accredit.OguAdmin;
using MCS.Applications.AccreditAdmin.classLib;
#endregion

namespace MCS.Applications.AccreditAdmin
{
	/// <summary>
	/// LogOn ��ժҪ˵����
	/// </summary>
	public partial class LogOn : WebBaseClass
	{
		#region Controls On Page


		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (false == IsPostBack)
				{
					string strSql = "SELECT GUID, NAME + '(' + VERSION + ')' AS DISPLAYNAME FROM PWD_ARITHMETIC WHERE VISIBLE = 1 ORDER BY SORT_ID";

					DataSet ds = InnerCommon.ExecuteDataset(strSql);

					DataView dv = new DataView(ds.Tables[0]);
					userPwdType.DataSource = dv;
					userPwdType.DataTextField = "DISPLAYNAME";
					userPwdType.DataValueField = "GUID";
					userPwdType.DataBind();
					errorMsg.Visible = false;
				}
			}
			catch (System.Exception ex)
			{
				//ExceptionManager.Publish(ex);
				throw ex;
			}
		}

		#endregion

		#region Web ������������ɵĴ���
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: �õ����� ASP.NET Web ���������������ġ�
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		#region private Function
		protected void btnLogOn_Click(object sender, System.EventArgs e)
		{
			//			userName.Value = "luzhiqiang";
			//			userPassword.Value = "000000";
			try
			{
				ExceptionHelper.TrueThrow<ApplicationException>(string.IsNullOrEmpty(userName.Value), "�Բ��𣬵�¼���Ʋ���Ϊ��");

				ILogOnUserInfo logonUserInfo = new LogOnUserInfo(userName.Value, userPwdType.SelectedItem.Value, userPassword.Value);

				Session["logonUserInfo"] = logonUserInfo;

				//��¼ϵͳ��¼��־����¼�ɹ���
				SetUserPrincipal(logonUserInfo.UserLogOnName);

				FormsAuthentication.SetAuthCookie(logonUserInfo.UserLogOnName, false);

				GlobalInfo.InitHttpEnv(Request);
				GlobalInfo.InitLogOnUser(logonUserInfo);

				UserDataWrite.InsertUserLog(OGULogDefine.LOGON_APP,
					OGULogDefine.LOGON_TYPE_SUCCESS,
					"�û���" + logonUserInfo.OuUsers[0].UserDisplayName + "���ɹ���¼��",
					logonUserInfo.UserLogOnName);

				SysDataWrite.InsertSysLog("jinshan", "y", "adfadsfasdfasdfasdf");

				string strRequestUrl = Request.QueryString["ReturnUrl"];

				if (strRequestUrl != null)
					FormsAuthentication.RedirectFromLoginPage(logonUserInfo.UserLogOnName, false);
				else
					Response.Redirect("OGUAdmin.aspx", false);
			}
			catch (System.Exception ex)
			{
				errorMsg.Visible = true;
				errorMsg.Text = ex.Message.Replace("\n", "<br>");
			}
		}

		private void SetUserPrincipal(string strUserName)
		{
			HttpContext context = HttpContext.Current;

			IIdentity id = new GenericIdentity(strUserName);
			string[] strRoles = { "" };
			context.User = new GenericPrincipal(id, strRoles);
		}
		#endregion
	}
}
