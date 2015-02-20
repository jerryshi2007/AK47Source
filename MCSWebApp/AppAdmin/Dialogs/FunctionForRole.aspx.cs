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
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Applications.AppAdmin.Common;
using MCS.Applications.AppAdmin.Properties;

namespace MCS.Applications.AppAdmin.Dialogs
{
	/// <summary>
	/// FunctionForRole 的摘要说明。
	/// </summary>
	public partial class FunctionForRole : WebBaseClass
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//清缓存
			DenyHistory();

			// 在此处放置用户代码以初始化页面
			string strSQL;
			DataSet ds			= null;
			string strRoleID	= string.Empty;
			string strAppID		= string.Empty;
			string strRoleName	= string.Empty;
			bool   bReadOnly	= false;

			if ( Request.QueryString["rID"] != null )
				strRoleID = Request.QueryString["rID"];
			
			if ( Request.QueryString["aID"] != null )
				strAppID = Request.QueryString["aID"];
			
			if ( Request.QueryString["read"] != null && Request.QueryString["read"].ToLower() == "true")
				bReadOnly = true;


			//strRoleID		= "365f8197-4f97-4ea9-ae26-2de8c89a1fdf";//
			//strAppCodeName	= "asdf";


			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				strSQL = string.Format("SELECT NAME FROM ROLES WHERE ID = {0}", TSqlBuilder.Instance.CheckQuotationMark( strRoleID , true ));
				strRoleName = InnerCommon.ExecuteScalar(strSQL).ToString();
				strSQL = string.Format("SELECT CHILDREN_COUNT FROM APPLICATIONS WHERE ID = {0}", TSqlBuilder.Instance.CheckQuotationMark( strAppID , true));
				hdSupAppCount.Value = InnerCommon.ExecuteScalar(strSQL).ToString();
				
				strSQL = string.Format(@"SELECT ID, NAME, CODE_NAME, DESCRIPTION, RESOURCE_LEVEL,  'S' AS TYPE , APP_ID, '' AS ROLE_ID
										FROM FUNCTION_SETS 
										WHERE APP_ID = {0}
										AND CLASSIFY = (SELECT CLASSIFY FROM ROLES WHERE ID = {1})

										UNION

										SELECT  F.ID, F.NAME, F.CODE_NAME, F.DESCRIPTION, ISNULL(FS.RESOURCE_LEVEL, '999') + '001' AS RESOUREC_LEVEL, 'F' AS TYPE, F.APP_ID, (SELECT ISNULL(ROLE_ID, '') FROM ROLE_TO_FUNCTIONS WHERE ROLE_ID = {1} AND FUNC_ID = F.ID) ROLE_ID
										FROM FUNCTIONS F
										LEFT JOIN FUNC_SET_TO_FUNCS FSTF ON F.ID = FSTF.FUNC_ID
										LEFT JOIN FUNCTION_SETS FS ON FSTF.FUNC_SET_ID = FS.ID
										WHERE F.APP_ID = {0}
										AND F.CLASSIFY = (SELECT CLASSIFY FROM ROLES WHERE ID = {1})

										ORDER BY RESOURCE_LEVEL", TSqlBuilder.Instance.CheckQuotationMark( strAppID , true), TSqlBuilder.Instance.CheckQuotationMark( strRoleID, true ));

				ds = InnerCommon.ExecuteDataset(strSQL);
			}

			tdRoleName.InnerText = "角色名："+strRoleName;
			hdRoleID.Value = strRoleID;
			dgFuncList.DataSource = ds;
			dgFuncList.DataBind();
			string strTemp = string.Empty;
			foreach (DataGridItem item in dgFuncList.Controls[0].Controls)
			{
				if (item.ItemType == ListItemType.Header)
				{
					((CheckBox) item.FindControl("chkAll")).Enabled = !bReadOnly;
					break;
				}
				if (item.ItemType == ListItemType.Item)
				{
				}
			}

			int i = 0;
			foreach( DataGridItem item in dgFuncList.Items )
			{
				strTemp = ds.Tables[0].Rows[i]["ROLE_ID"].ToString();

				item.Attributes["ROLE_ID"] = strTemp;
				((CheckBox) item.Cells[0].FindControl("chkItem")).Checked = strTemp == string.Empty ? false : true;
				((CheckBox) item.Cells[0].FindControl("chkItem")).Enabled = !bReadOnly;
				item.Attributes["FUNC_ID"] = ds.Tables[0].Rows[i]["ID"].ToString();
				strTemp = ds.Tables[0].Rows[i]["RESOURCE_LEVEL"].ToString();
				if ( strTemp.Substring(0, 3) == "999" )
					strTemp = strTemp.Substring(4);
				item.Attributes["RESOURCE_LEVEL"] = strTemp;
				item.Attributes["TYPE"] = ds.Tables[0].Rows[i]["TYPE"].ToString();
				i++;
			}
		
		}

		/// <summary>
		/// 清除页面的缓存
		/// </summary>
		private void DenyHistory()
		{
			Page.Response.Cache.SetNoStore();
//			Response.Buffer = false;
//			Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
//			Response.Expires = 0;
//			Response.CacheControl = "no-cache";		
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
