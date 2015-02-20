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
#endregion

namespace MCS.Applications.AccreditAdmin.dialogs
{
	/// <summary>
	/// PrepareForExport ��ժҪ˵����
	/// </summary>
	public partial class PrepareForExport : WebUserBaseClass 
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			sysPositionShow.Text = ((string)this.GetRequestData("allPathName", string.Empty)).Replace("\\", "��");
			rootOrganizationGuid.Value = (string)this.GetRequestData("GUID", string.Empty);
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
