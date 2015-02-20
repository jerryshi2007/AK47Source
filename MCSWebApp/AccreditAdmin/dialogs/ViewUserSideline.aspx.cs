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
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Core;
#endregion

namespace MCS.Applications.AccreditAdmin.dialogs
{
	/// <summary>
	/// ViewUserSideline ��ժҪ˵����
	/// </summary>
	public partial class ViewUserSideline : WebUserBaseClass
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();

			string strUserGuid = (string)GetRequestData("Guid", string.Empty);
			ExceptionHelper.TrueThrow(strUserGuid == string.Empty, "�Բ���û��ָ��ȷ�����û���");
			DataSet ds = OGUReader.GetObjectsDetail("USERS", strUserGuid);

			rsUserMessDetail.Value = OGUReader.GetLevelSortXmlDocAttr(ds.Tables[0], 
				"GLOBAL_SORT", 
				"OBJECTCLASS", 
				Properties.AccreditResource.OriginalSortDefault.Length).OuterXml;
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
