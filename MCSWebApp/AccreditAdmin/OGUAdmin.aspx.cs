#region using 

using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using MCS.Library.Accredit.WebBase;
#endregion

namespace MCS.Applications.AccreditAdmin
{
	/// <summary>
	/// OGUAdmin ��ժҪ˵����
	/// </summary>
	public partial class OGUAdmin : WebUserBaseClass
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			currentUserName.Value = this.LogOnUserInfo.OuUsers[0].UserDisplayName;
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
