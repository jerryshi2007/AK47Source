#region using

using System;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Transactions;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Core;
using MCS.Applications.AccreditAdmin.classLib;
using MCS.Applications.AccreditAdmin.Properties;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
#endregion

namespace MCS.Applications.AccreditAdmin.sysAdmin
{
	/// <summary>
	/// OGUAdmin 的摘要说明。
	/// </summary>
	public partial class OGUAdmin : XmlRequestUserWebClass
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
			{
				switch (this.RootName)
				{
					case "Insert": OGUWriter.InsertObjects(_XmlRequest);
						_XmlResult = InnerCommon.XmlNodeSetToAttribute(_XmlRequest);
						break;
					case "Update": OGUWriter.UpdateObjects(_XmlRequest);
						_XmlResult = InnerCommon.XmlNodeSetToAttribute(_XmlRequest);
						break;
					case "logicDelete": OGUWriter.LogicDeleteObjects(_XmlRequest);
						break;
					case "furbishDelete": OGUWriter.FurbishDeleteObjects(_XmlRequest);
						break;
					case "realDelete": OGUWriter.RealDeleteObjects(_XmlRequest);
						break;
					case "Move": OGUWriter.MoveObjects(_XmlRequest);
						_XmlResult = _XmlRequest;
						break;
					case "Sort": OGUWriter.SortObjects(_XmlRequest);
						break;
					case "GroupSort": _XmlResult = OGUWriter.GroupSortObjects(_XmlRequest);
						GetUsersInGroupsInPage();
						break;
					case "addObjectsToGroups": _XmlResult = OGUWriter.AddObjectsToGroups(_XmlRequest);
						GetUsersInGroupsInPage();
						break;
					case "delUsersFromGroups": OGUWriter.DelUsersFromGroups(_XmlRequest);
						GetUsersInGroupsInPage();
						break;
					case "setMainDuty": OGUWriter.SetUserMainDuty(_XmlRequest);
						break;
					case "ResetPassword": OGUWriter.ResetPassword(_XmlRequest);
						break;
					case "InitPassword": OGUWriter.InitPassword(_XmlRequest);
						break;
					case "addSecsToLeader": _XmlResult = OGUWriter.SetSecsToLeader(_XmlRequest);
						break;
					case "delSecsOfLeader": OGUWriter.DelSecsOfLeader(_XmlRequest);
						break;
					default: SetErrorResult(_XmlResult, "系统中没有相关数据处理\"" + this.RootName + "\"的程序", string.Empty);
						break;
				}
			}
		}

		private void GetUsersInGroupsInPage()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strGroupGuid = root.GetAttribute("GUID");
			string strAttrs = OGUCommonDefine.CombinateAttr(root.GetAttribute("extAttr"));
			int iPageNo = Convert.ToInt32(root.GetAttribute("PageNo"));
			int iPageSize = Convert.ToInt32(root.GetAttribute("PageSize"));
			string strSortColumn = root.GetAttribute("PageSort");
			string strSearchName = root.GetAttribute("SearchName");

			DataSet ds = OGUReader.GetUsersInGroups(strGroupGuid,
				SearchObjectColumn.SEARCH_GUID,
				strSearchName,
				strSortColumn,
				strAttrs,
				iPageNo,
				iPageSize);
			_XmlResult = InnerCommon.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");

			_XmlResult.DocumentElement.SetAttribute("GetCount",
				OGUReader.GetUsersInGroups(strGroupGuid,
					SearchObjectColumn.SEARCH_GUID,
					strSearchName,
					strSortColumn,
					0,
					0).Tables[0].Rows.Count.ToString());
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
