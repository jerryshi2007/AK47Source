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
	/// delegationForm 的摘要说明。
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
