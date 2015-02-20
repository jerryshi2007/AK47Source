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
	/// SecretariesOfLeader 的摘要说明。
	/// </summary>
	public partial class SecretariesOfLeader : WebUserBaseClass
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();

			string strGroupGuid = (string)GetRequestData("Guid", string.Empty);
			ExceptionHelper.TrueThrow(strGroupGuid == string.Empty, "对不起，没有确定的人员组标识！");
			LeaderGuid.Value = strGroupGuid;

			DataSet ds = OGUReader.GetObjectsDetail("USERS", 
				strGroupGuid, 
				SearchObjectColumn.SEARCH_USER_GUID, 
				string.Empty, 
				SearchObjectColumn.SEARCH_NULL);
			LeaderDisplayName.Text = OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["DISPLAY_NAME"]);

			userPermission.Value = classLib.OGUUserPermission.GetOGUPemission();
		}

		#region Web 窗体设计器生成的代码
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
