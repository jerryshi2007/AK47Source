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
using MCS.Library.Accredit.Configuration;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Applications.AccreditAdmin.Properties;
#endregion

namespace MCS.Applications.AccreditAdmin.dialogs
{
	/// <summary>
	/// UsersInGroup ��ժҪ˵����
	/// </summary>
	public partial class UsersInGroup : WebUserBaseClass
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();

			string strGroupGuid = (string)GetRequestData("Guid", string.Empty);
			ExceptionHelper.TrueThrow(strGroupGuid == string.Empty, "�Բ���û��ȷ������Ա���ʶ��");

			groupGuid.Value = strGroupGuid;

			using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
			{
				DataSet ds = OGUReader.GetObjectsDetail("GROUPS",
					strGroupGuid, 
					SearchObjectColumn.SEARCH_GUID, 
					string.Empty, 
					SearchObjectColumn.SEARCH_NULL);
				GroupDisplayName.Text = OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["DISPLAY_NAME"]);

				userPermission.Value = classLib.OGUUserPermission.GetOGUPemission();
			}
			txtPageSize.Value = AccreditSection.GetConfig().AccreditSettings.GroupUsersPageSize.ToString();
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
