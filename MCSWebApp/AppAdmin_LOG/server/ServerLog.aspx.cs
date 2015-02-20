using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.LogAdmin;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using MCS.Library.Core;

using MCS.Applications.AppAdmin_LOG.server;
using MCS.Applications.AppAdmin_LOG.Properties;

namespace MCS.Applications.AppAdmin_LOG.server
{
	/// <summary>
	/// ServerLog 的摘要说明。
	/// </summary>
	public partial class ServerLog : XmlRequestWebClass
	{
		private XmlElement _RootElement;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			_RootElement = this._XmlRequest.DocumentElement;

			switch (_RootElement.Name)
			{
				case "Insert":
					InsertAppType();
					break;
				case "Update":
					UpdateAppType();
					break;
				case "Delete":
					DeleteAppType();
					break;
				case "ClientCommand":
					GetTreeDataList();
					break;
				case "SysLogList":
					QuerySysList();
					break;
				case "UserLogList":
					QueryUserList();
					break;
				case "GetSelectList":
					GetOpSelectList();
					break;
				default:
					break;
			}
		}

		private void InsertAppType()
		{
			XmlNode xNode = _XmlRequest.DocumentElement.SelectSingleNode(".//SET");
			XmlNode root = null;

			string insertSql = string.Empty;
			string strSql = string.Empty;
			if (_XmlRequest.DocumentElement.SelectSingleNode("APP_OPERATION_TYPE") != null)
			{
				root = _XmlRequest.DocumentElement.SelectSingleNode("APP_OPERATION_TYPE");
				insertSql = "INSERT APP_OPERATION_TYPE ";

				string appGuid = ((XmlElement)root).GetAttribute("appGuid");
				string strOrgSort = ((XmlElement)root).GetAttribute("strSort");
				string typeGuid = Guid.NewGuid().ToString();

				InsertSqlClauseBuilder ic = new InsertSqlClauseBuilder();
				ic.AppendItem<string>("GUID", typeGuid);
				ic.AppendItem<string>("APP_GUID", appGuid);
				ic.AppendItem<string>("DISPLAYNAME", xNode.SelectSingleNode("DISPLAYNAME").InnerText);
				ic.AppendItem<string>("CODE_NAME", xNode.SelectSingleNode("CODE_NAME").InnerText);
				ic.AppendItem<string>("DISCRIPTION", xNode.SelectSingleNode("DESCRIPTION").InnerText);
				ic.AppendItem<string>("VISIBLE", "y");
				ic.AppendItem<string>("CLASS", LogReader.GetNewClassValue(appGuid, strOrgSort));

				strSql = insertSql + ic.ToSqlString(TSqlBuilder.Instance);
				UserDataWrite.InsertUserLog("APP_LOG", "", "", "", "ADD_OP_TYPE", "向日志审计系统添加新的操作类型:"
					+ xNode.SelectSingleNode("DISPLAYNAME").InnerText, _XmlRequest.DocumentElement.InnerXml, true);
			}
			else
			{
				root = _XmlRequest.DocumentElement.SelectSingleNode("APP_LOG_TYPE");
				insertSql = "INSERT APP_LOG_TYPE ";

				string appGuid = Guid.NewGuid().ToString();

				InsertSqlClauseBuilder ic = new InsertSqlClauseBuilder();
				ic.AppendItem<string>("GUID", appGuid);
				ic.AppendItem<string>("CODE_NAME", xNode.SelectSingleNode("CODE_NAME").InnerText);
				ic.AppendItem<string>("DISPLAYNAME", xNode.SelectSingleNode("DISPLAYNAME").InnerText);
				ic.AppendItem<string>("VISIBLE", "y");
				ic.AppendItem<string>("DISCRIPTION", xNode.SelectSingleNode("DESCRIPTION").InnerText);
				ic.AppendItem<string>("CLASS", LogReader.GetClassForApp());

				strSql = insertSql + ic.ToSqlString(TSqlBuilder.Instance);
				UserDataWrite.InsertUserLog("APP_LOG", "ADD_APP_TYPE", "向日志审计系统添加新的应用:"
					+ xNode.SelectSingleNode("DISPLAYNAME").InnerText, _XmlRequest.DocumentElement.InnerXml, true);
			}

			InnerCommon.ExecuteNonQuery(strSql);
		}

		public void UpdateAppType()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strTableName = root.FirstChild.Name;

			string strGuid = root.SelectSingleNode(".//GUID").InnerText;
			string strCodeName = root.SelectSingleNode(".//CODE_NAME").InnerText;
			string strDisplayname = root.SelectSingleNode(".//DISPLAYNAME").InnerText;
			string strDescription = root.SelectSingleNode(".//DESCRIPTION").InnerText;

			string strSql = "UPDATE " + strTableName
				+ " SET CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true)
				+ ", DISPLAYNAME = " + TSqlBuilder.Instance.CheckQuotationMark(strDisplayname, true)
				+ ", DISCRIPTION = " + TSqlBuilder.Instance.CheckQuotationMark(strDescription, true)
				+ " WHERE GUID = '" + strGuid + "'";

			InnerCommon.ExecuteNonQuery(strSql);
			UserDataWrite.InsertUserLog("APP_LOG", strTableName == "APP_LOG_TYPE" ? "MODIFY_APP_TYPE" : "MODIFY_OP_TYPE",
				"从日志审计系统中修改" + (strTableName == "APP_LOG_TYPE" ? "应用" : "操作类型") + "[" + strDisplayname + "]", _XmlRequest.DocumentElement.InnerXml, true);
		}

		public void DeleteAppType()
		{
			XmlElement root = _XmlRequest.DocumentElement;

			string strSort = root.GetAttribute("orgSort");
			string strSql = string.Empty;
			string strTableName = string.Empty;
			string strObjName = string.Empty;
			string strLogType = string.Empty;

			if (strSort.Length == 8)
			{
				strTableName = "APP_LOG_TYPE";
				strObjName = "应用类型";
				strLogType = "DEL_APP_TYPE";
			}
			else if (strSort.Length == 12)
			{
				strTableName = "APP_OPERATION_TYPE";
				strObjName = "应用操作类型";
				strLogType = "DEL_OP_TYPE";
			}
			strSql = "UPDATE " + strTableName + " SET VISIBLE = 'n' WHERE GUID = "
				+ TSqlBuilder.Instance.CheckQuotationMark(root.GetAttribute("Guid"), true);
			InnerCommon.ExecuteNonQuery(strSql);

			UserDataWrite.InsertUserLog("APP_LOG", strLogType, "从日志审计系统中删除" + strObjName + ":" + root.GetAttribute("DisplayName"),
				_XmlRequest.DocumentElement.InnerXml, true);
		}

		public void GetTreeDataList()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strLot = root.GetAttribute("listObjectType");//要求查询的对象类型
			string strGuid = root.GetAttribute("appTypeGuid");
			string strSql = string.Empty;

			if (strGuid != string.Empty)
				strSql = "SELECT GUID, 'OPTYPE' AS NODE_NAME, CODE_NAME, DISPLAYNAME, APP_GUID, VISIBLE, DISCRIPTION, CLASS FROM APP_OPERATION_TYPE WHERE APP_GUID = '" + strGuid + "' AND VISIBLE = 'y' ORDER BY CLASS";
			else
				strSql = "SELECT GUID, 'APPNAME' AS NODE_NAME, CODE_NAME, DISPLAYNAME, VISIBLE, DISCRIPTION, CLASS FROM APP_LOG_TYPE WHERE VISIBLE = 'y' ORDER BY CLASS, CODE_NAME";

			DataSet ds = InnerCommon.ExecuteDataset(strSql);
			_XmlResult = LogReader.GetLevelSortXmlDocAttr(ds.Tables[0], "CLASS", "NODE_NAME", LogReader.LOG_ORIGINAL_SORT.Length);
		}

		private void QuerySysList()
		{
			string strWhereCondition = GetWhereSqlString();

			string strSql = @" SELECT USER_LOGONNAME, HOST_IP, LOG_DATE, ID, IE_VERSION, WINDOWS_VERSION, KILL_VIRUS, STATUS, HOST_NAME, USER_DISTINCTNAME 
				FROM SYS_USER_LOGON 
				WHERE 1=1 " + strWhereCondition + @" 
				ORDER BY ID DESC ";

			int iStart = Convert.ToInt32(_RootElement.GetAttribute("lastKey"));
			int iLength = Convert.ToInt32(_RootElement.GetAttribute("rows"));
			using (DbContext context = DbContext.GetContext(LogResource.ConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				_XmlResult = InnerCommon.GetXmlDoc(database.ExecuteDataSet(CommandType.Text, strSql, iStart, iLength, "SYS_USER_LOGON"));

				string strCount = @" SELECT COUNT(ID) 
				FROM SYS_USER_LOGON 
				WHERE 1=1 " + strWhereCondition;

				XmlHelper.AppendNode<string>(_XmlResult.DocumentElement, "NUMBER", database.ExecuteScalar(CommandType.Text, strCount).ToString());
			}
		}

		private void QueryUserList()
		{
			string strWhereCondition = GetWhereSqlString();

			string strSql = @" SELECT UOL.*, ALT.DISPLAYNAME AS APP_DISPLAYNAME, AOT.DISPLAYNAME AS OP_DISPLAYNAME 
				FROM USER_OPEATION_LOG UOL, APP_LOG_TYPE ALT, APP_OPERATION_TYPE AOT 
				WHERE AOT.GUID = UOL.OP_GUID 
					AND ALT.GUID = UOL.APP_GUID 
					AND AOT.APP_GUID = ALT.GUID " + strWhereCondition + @" 
				ORDER BY ID DESC ";

			int iStart = Convert.ToInt32(_RootElement.GetAttribute("lastKey"));
			int iLength = Convert.ToInt32(_RootElement.GetAttribute("rows"));
			using (DbContext context = DbContext.GetContext(LogResource.ConnAlias))
			{
				Database database = DatabaseFactory.Create(context);

				_XmlResult = InnerCommon.GetXmlDoc(database.ExecuteDataSet(CommandType.Text, strSql, iStart, iLength, "USER_OPEATION_LOG"));

				string strCount = @" SELECT COUNT(UOL.ID) 
				FROM USER_OPEATION_LOG UOL 
				WHERE 1=1 " + strWhereCondition;

				XmlHelper.AppendNode<string>(_XmlResult.DocumentElement, "NUMBER", database.ExecuteScalar(CommandType.Text, strCount).ToString());
			}
		}


		private string GetWhereSqlString()
		{
			string strWhereSql = string.Empty;
			string start_time = _RootElement.GetAttribute("start_time");
			string end_time = _RootElement.GetAttribute("end_time");

			string sUserName = _RootElement.GetAttribute("userName");
			string sAppGuid = _RootElement.GetAttribute("appGuid");
			string sFileID = _RootElement.GetAttribute("fileID");
			string sOPGuid = _RootElement.GetAttribute("Guid");
			string sListCtr = _RootElement.GetAttribute("listCtr");
			string sOrgSort = _RootElement.GetAttribute("orgSort");

			if (sOrgSort.Length == 4 && sListCtr == "search")
			{
				strWhereSql += "AND UOL.LOG_DATE >= " + TSqlBuilder.Instance.CheckQuotationMark(start_time, true)
					+ " AND UOL.LOG_DATE < " + TSqlBuilder.Instance.CheckQuotationMark(end_time, true);

				sAppGuid = LogReader.GetGuidByDisplayName(_RootElement.GetAttribute("appName"), "APP_LOG_TYPE");
				if (sAppGuid != "")
					strWhereSql += " AND UOL.APP_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(sAppGuid, true);

				if (_RootElement.GetAttribute("opTypeName").Trim() != string.Empty)
					sOPGuid = LogReader.GetGuidByDisplayName(_RootElement.GetAttribute("opTypeName"), "APP_OPERATION_TYPE");

				if (sOPGuid != "")
					strWhereSql += " AND UOL.OP_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(sOPGuid, true);

				if (sUserName != "")
					strWhereSql += " AND UOL.OP_USER_DISPLAYNAME = " + TSqlBuilder.Instance.CheckQuotationMark(sUserName, true);
				if (sFileID != "")
					strWhereSql += " AND UOL.ID = " + TSqlBuilder.Instance.CheckQuotationMark(sFileID, true);
			}
			else if (sOrgSort.Length == 8)
			{
				if (sAppGuid != "")
					strWhereSql += " AND UOL.APP_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(sOPGuid, true);
			}
			else if (sOrgSort.Length == 12)
			{
				if (sOPGuid != "")
					strWhereSql += " AND UOL.OP_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(sOPGuid, true);
				if (sAppGuid != "")
					strWhereSql += " AND UOL.APP_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(sAppGuid, true);
			}
			return strWhereSql;
		}

		private void GetOpSelectList()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strDisplayname = root.GetAttribute("appDisplayname");
			string strSql = @"SELECT AOT.DISPLAYNAME 
FROM APP_LOG_TYPE ALT, APP_OPERATION_TYPE AOT
WHERE AOT.APP_GUID = ALT.GUID 
	AND AOT.VISIBLE = 'y' 
	AND ALT.DISPLAYNAME = " + TSqlBuilder.Instance.CheckQuotationMark(strDisplayname, true);

			_XmlResult = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSql));
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
