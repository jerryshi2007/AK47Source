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
using System.Xml;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.AppAdmin;
using MCS.Library.Accredit.Configuration;
using MCS.Library.Core;

namespace MCS.Applications.AppAdmin.Dialogs
{
	/// <summary>
	/// WebForm1 的摘要说明。
	/// </summary>
	public partial class QueryUserAppRoles : WebBaseClass
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			HttpContext context = HttpContext.Current;
			HttpRequest request = HttpContext.Current.Request;
			string appCodeName = request.QueryString["aName"];
			string appScope = request.QueryString["aScope"];
			string appName = string.Empty;
			if (appCodeName == null || appCodeName == string.Empty)
			{
				lbAppSocpe.InnerText = "(所有应用)";
			}
			else
			{
				DataTable dt = SecurityCheck.GetApplications().Tables[0];
				DataRow[] drs = dt.Select("[CODE_NAME] = '"+appCodeName+"'");
				if ( drs.Length > 0)
					appName = drs[0]["NAME"].ToString();
				lbAppSocpe.InnerText = string.Format("({0}-{1})", appName, appCodeName);
			}

			XmlDocument xDoc = XmlHelper.CreateDomDocument("<Config><multiSelect>false</multiSelect><selectObjType>1</selectObjType><RootOrg/><BottomRow>true</BottomRow></Config>");
			XmlNode xNode = xDoc.SelectSingleNode(".//RootOrg");


			string strUser = HttpContext.Current.User.Identity.Name;
			if ( !IsPostBack )
			{
				if ( appScope != null && appScope.ToLower() == "y" )
				{
					DataTable dt = SecurityCheck.GetUserRolesScopes( strUser, appCodeName, "SELF_ADMIN_ROLE,COMMON_ADMIN_ROLE").Tables[0]; 
					string strScope = string.Empty;
					if ( dt.Rows.Count == 0 )
					{
						throw new Exception(string.Format("用户[{0}]没有设定相应的服务范围,不能完成此项操作", strUser));
					}
					foreach( DataRow row in dt.Rows )
					{
						strScope = row["DESCRIPTION"].ToString();
						foreach ( string scope in strScope.Split(new char[] {',',';'}) )
						{
							XmlHelper.AppendNode( xNode, "ORGANIZATIONS", scope);
						}
					}
				}
				else
				{
					string strRoot = AccreditSection.GetConfig().AccreditSettings.OguRootName;
					XmlHelper.AppendNode( xNode, "ORGANIZATIONS", strRoot);
				}
				hdConfig.Value = xDoc.OuterXml;
				return;
			}

			if ( hdUserGuid.Value != string.Empty )
			{
				if ( appCodeName == null || appCodeName == string.Empty )
				{//查所有权限
					lbTitle.InnerText = string.Format("[{0}]所具有的角色：", hdAllPathName.Value);
					DataSet ds = SecurityCheck.GetUserApplicationsRoles(
						hdUserGuid.Value,
						UserValueType.Guid,
						RightMaskType.All,
						DelegationMaskType.All);

					DataView dv = new DataView(ds.Tables[0]);

					dv.Sort = "APP_RESOURCE_LEVEL ASC, SORT_ID ASC";
					for (int i = 0; i < dv.Count; i++)
					{
						HtmlTableRow row = new HtmlTableRow();
						HtmlTableCell cell = new HtmlTableCell();

						cell.InnerText = dv[i]["APP_CODE_NAME"].ToString();
						row.Controls.Add(cell);

						cell = new HtmlTableCell();

						cell.InnerText = dv[i]["APP_NAME"].ToString();//ars[i].AppName;
						row.Controls.Add(cell);

						cell = new HtmlTableCell();

						cell.InnerText = dv[i]["CODE_NAME"].ToString();
						row.Controls.Add(cell);

						cell = new HtmlTableCell();

						cell.InnerText = dv[i]["NAME"].ToString();
						row.Controls.Add(cell);

						appRolesTable.Controls.Add(row);
					}
				}
				else
				{//查一个应用的权限
					lbTitle.InnerText = string.Format("[{0}]在\n[{1}-{2}]中所具有的角色：", hdAllPathName.Value, appName, appCodeName);
					bool bIsSelfAdmin = SecurityCheck.IsUserInRoles( strUser, appCodeName, "SELF_ADMIN_ROLE" );
					DataSet ds = SecurityCheck.GetUserRoles(
						hdUserGuid.Value,
						appCodeName,
						UserValueType.Guid,
						bIsSelfAdmin ? RightMaskType.All : RightMaskType.App,
						DelegationMaskType.All);
					

					DataView dv = new DataView(ds.Tables[0]);

					dv.Sort = "CLASSIFY ASC, SORT_ID ASC";
					for (int i = 0; i < dv.Count; i++)
					{
						HtmlTableRow row = new HtmlTableRow();
						HtmlTableCell cell = new HtmlTableCell();

						cell.InnerText = appCodeName;
						row.Controls.Add(cell);

						cell = new HtmlTableCell();

						cell.InnerText = appName;
						row.Controls.Add(cell);

						cell = new HtmlTableCell();

						cell.InnerText = dv[i]["CODE_NAME"].ToString();
						row.Controls.Add(cell);

						cell = new HtmlTableCell();

						cell.InnerText = dv[i]["NAME"].ToString();
						row.Controls.Add(cell);

						appRolesTable.Controls.Add(row);
					}
				}
			}
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
