#region using

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Core;
using MCS.Library.Accredit.OguAdmin;
using MCS.Applications.AccreditAdmin.classLib;
#endregion

namespace MCS.Applications.AccreditAdmin.dialogs
{
	/// <summary>
	/// changeUserPwd ��ժҪ˵����
	/// </summary>
	public partial class changeUserPwd : WebUserBaseClass
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();

			string strUserGuid = GetRequestData("userGuid", string.Empty).ToString();
			if (strUserGuid.Trim().Length == 0)
				strUserGuid = LogOnUserInfo.UserGuid;
			ExceptionHelper.FalseThrow(strUserGuid == LogOnUserInfo.UserGuid, "�Բ����û�ֻ���Լ����޸Ŀ��");

			UserGuid.Value = strUserGuid;
			userName.InnerText = LogOnUserInfo.OuUsers[0].UserDisplayName;

			if (false == IsPostBack)
			{
				string strSql = "SELECT GUID, NAME + '(' + VERSION + ')' AS DISPLAYNAME FROM PWD_ARITHMETIC WHERE VISIBLE = 1 ORDER BY SORT_ID";
				DataSet ds = InnerCommon.ExecuteDataset(strSql);

				DataView dv = new DataView(ds.Tables[0]);

				newPwdType.DataSource = dv;
				newPwdType.DataTextField = "DISPLAYNAME";
				newPwdType.DataValueField = "GUID";
				newPwdType.DataBind();

				oldPwdType.DataSource = dv;
				oldPwdType.DataTextField = "DISPLAYNAME";
				oldPwdType.DataValueField = "GUID";
				oldPwdType.DataBind();
			}
		}

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
	}
}
