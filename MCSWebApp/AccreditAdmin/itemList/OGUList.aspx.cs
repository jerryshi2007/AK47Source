#region

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
using MCS.Library.Accredit.OguAdmin;
#endregion

namespace MCS.Applications.AccreditAdmin.itemList
{
	/// <summary>
	/// OGUList ��ժҪ˵����
	/// </summary>
	public partial class OGUList : WebUserBaseClass
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			userPermission.Value = classLib.OGUUserPermission.GetOGUPemission();
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
