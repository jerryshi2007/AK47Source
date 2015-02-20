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
using MCS.Library.Accredit.AppAdmin;

namespace MCS.Applications.AppAdmin_LOG
{
	/// <summary>
	/// LogViewer ��ժҪ˵����
	/// </summary>
	public partial class LogViewer : WebUserBaseClass
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// �ڴ˴������û������Գ�ʼ��ҳ��
			currentUserName.Value = this.LogOnUserInfo.OuUsers[0].UserDisplayName;
			bool bRoles = SecurityCheck.DoesUserHasPermissions(this.LogOnUserInfo.UserLogOnName, "APP_LOG", "APP_LOG_MAINTAIN");
			if (bRoles == true)
				rolesValue.Value = "true";
			else rolesValue.Value = "false";
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
