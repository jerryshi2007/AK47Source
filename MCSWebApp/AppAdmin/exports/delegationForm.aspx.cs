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

namespace MCS.Applications.AppAdmin.Dialogs
{
	/// <summary>
	/// delegationForm ��ժҪ˵����
	/// </summary>
	public partial class delegationForm : WebUserBaseClass
	{
		protected System.Web.UI.HtmlControls.HtmlInputHidden TARGET_ID;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			SOURCE_ID.Value = this.LogOnUserInfo.UserGuid;
			SOURCE_LOGON_NAME.Value = this.LogOnUserInfo.UserLogOnName;
			
			appCodeName.Value= (string)GetRequestData("appCodeName", string.Empty);
//			sourceRootOrg.Value = (string)GetRequestData("sourceRootOrg", string.Empty);
			targetRootOrg.Value = (string)GetRequestData("targetRootOrg", string.Empty);

			if (SOURCE_ID.Value != string.Empty)
				sourceDnRow.Visible = false;

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
