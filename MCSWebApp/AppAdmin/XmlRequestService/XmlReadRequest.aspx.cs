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
using System.Text;
using System.Diagnostics;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.AppAdmin;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Applications.AppAdmin.Common;
using MCS.Applications.AppAdmin.Properties;

namespace MCS.Applications.AppAdmin
{
	/// <summary>
	/// XmlReadRequest 的摘要说明。
	/// </summary>
	public partial class XmlReadRequest : XmlRequestUserWebClass
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				switch (RootName)
				{
					case "queryList": DoQueryList();
						break;
					case "queryObj": DoQueryObj();
						break;
					case "queryUserFuncScopes": DoQueryUserFuncScopes();
						break;
					case "getObjInfo": GetObjInfo(_XmlRequest);
						break;
					case "getAppDelegationRoles": GetAppDelegationRoles(_XmlRequest);
						break;
					case "getDelegationApps": GetDelegationApplications(_XmlRequest);
						break;
					//case "getRoleDelegationUser": GetRoleDelegationUser(_XmlRequest);
					//    break;
					case "getUserAllPathName": GetUserAllPathName(_XmlRequest);
						break;
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				string strMessage = ex.Message;

				switch (ex.Number)
				{
					case 2627:
						if (ex.Message.IndexOf("CODE_NAME") >= 0)
							strMessage = "";
						break;
					case 2601:
						strMessage = "";
						break;
				}

				Debug.WriteLine(ex.Message);
				//ExceptionManager.Publish(ex);
				throw new Exception(ex.Message);
			}
			catch (System.Exception ex)
			{
				//Debug.WriteLine(ex.Message);
				//ExceptionManager.Publish(ex);
				throw ex;
			}
		}

		protected void DoQueryList()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strType = root.GetAttribute("type");
			switch (strType)
			{
				case "APPLICATION": DoQueryApplication();
					break;
				case "FUNCTION": DoQueryFunction();
					break;
				case "ROLE": DoQueryRole();
					break;
				case "APP_SCOPE": DoQueryAppScope();
					break;
				case "FUNCTION_TO_ROLE": DoQueryFuncToRole();
					break;
				case "ROLE_TO_FUNCTION": DoQueryRoleToFunc();
					break;
				case "FUNCTION_SET_TO_ROLE": DoQueryFuncSetToRole();
					break;
				case "FUNCTION_SET_TO_FUNCTION": DoQueryFuncSetToFunc();
					break;
				case "ROLE_TO_EXPRESSION": DoQueryRoleToExp();
					break;
				case "EXPRESSION_SCOPE": DoQueryExpScope();
					break;
			}
		}

		/// <summary>
		/// 查询角色、功能或功能集合的信息
		/// </summary>
		/// <param name="xmlDoc"></param>
		protected void GetObjInfo(XmlDocument xmlDoc)
		{
			XmlElement root = xmlDoc.DocumentElement;
			string strTableName = root.GetAttribute("type");
			string strID = root.GetAttribute("id");

			string strSQL = "SELECT * FROM " + TSqlBuilder.Instance.CheckQuotationMark(strTableName, false)
				+ " WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strID, true);
			DataSet ds = InnerCommon.ExecuteDataset(strSQL);
			_XmlResult = InnerCommon.GetXmlDoc(ds);

		}

		/// <summary>
		/// 查询userID在应用的角色中存在的所有应用
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <remarks>
		///	<code>
		///	<getDelegationApps logonName="userlogonname" idType="logonName" appID="application_id"></getDelegationApps>
		/// </code>
		/// </remarks>
		protected void GetDelegationApplications(XmlDocument xmlDoc)
		{
			string strLogonName = xmlDoc.DocumentElement.GetAttribute("logonName");

			DataSet ds = SecurityCheck.GetUserApplicationsForDelegation(strLogonName, UserValueType.LogonName, RightMaskType.All);
			_XmlResult = InnerCommon.GetXmlDoc(ds);

			ds = OGUReader.GetObjectsDetail("USERS", strLogonName, SearchObjectColumn.SEARCH_LOGON_NAME,
				string.Empty, SearchObjectColumn.SEARCH_NULL);
			string strDisplayName = ds.Tables[0].Rows[0]["DISPLAY_NAME"].ToString();

			_XmlResult.DocumentElement.SetAttribute("displayName", strDisplayName);
		}

		/// <summary>
		/// 查询应用(app_id)中包含用户(UserID)的所有角色
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <remarks>
		/// <code>
		/// <getAppDelegationRoles logonName="userLogonName" appID="app_id" appCodeName="app_code_name"></getAppDelegationRoles>
		/// </code>
		/// </remarks>
		protected void GetAppDelegationRoles(XmlDocument xmlDoc)
		{
			string strLogonName = xmlDoc.DocumentElement.GetAttribute("logonName");

			string strAppCodeName = xmlDoc.DocumentElement.GetAttribute("appCodeName");

			DataSet ds = SecurityCheck.GetUserAllowDelegteRoles(strLogonName, strAppCodeName,
				UserValueType.LogonName, RightMaskType.All);

			_XmlResult = InnerCommon.GetXmlDoc(ds);
		}

		/// <summary>
		/// 获得用户的全名称
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <remarks>
		/// <code>
		///	<getUserAllPathName guid="dddd"></getUserdisplayName>
		/// </code>
		/// </remarks>
		protected void GetUserAllPathName(XmlDocument xmlDoc)
		{
			string strUserGuid = xmlDoc.DocumentElement.GetAttribute("guid");

			DataSet ds = OGUReader.GetObjectsDetail("USERS", strUserGuid);

			string strAllPathName = ds.Tables[0].Rows[0]["ALL_PATH_NAME"].ToString();

			_XmlResult.LoadXml("<DataSet />");
			_XmlResult.DocumentElement.SetAttribute("allPathName", strAllPathName);
		}

		/// <summary>
		/// 获得用户sourceID在角色roleID委派中的被委派对象的显示名称
		/// </summary>
		/// <param name="xmlDoc"></param>
		protected void GetRoleDelegationUser(XmlDocument xmlDoc)
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				string strSourceID = xmlDoc.DocumentElement.GetAttribute("sourceID");
				string strRoleID = xmlDoc.DocumentElement.GetAttribute("roleID");

				string strSQL = "SELECT TARGET_ID, START_TIME, END_TIME FROM DELEGATIONS WHERE SOURCE_ID = "
					+ TSqlBuilder.Instance.CheckQuotationMark(strSourceID, true)
					+ " AND ROLE_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true);

				DataSet ds = InnerCommon.ExecuteDataset(strSQL);
				_XmlResult = InnerCommon.GetXmlDoc(ds);

				string strTargetID = string.Empty;

				if (ds.Tables[0].Rows.Count != 0)
					strTargetID = ds.Tables[0].Rows[0]["TARGET_ID"].ToString();

				if (strTargetID != string.Empty)
				{
					ds = OGUReader.GetObjectsDetail("USERS", strTargetID,
						SearchObjectColumn.SEARCH_GUID, string.Empty, SearchObjectColumn.SEARCH_NULL);
					string strDisplayName = ds.Tables[0].Rows[0]["DISPLAY_NAME"].ToString();

					XmlHelper.AppendNode<string>(_XmlResult.DocumentElement.SelectSingleNode("DELEGATIONS"),
						"TARGET_DISPLAYNAME", strDisplayName);
				}
			}
		}

		//查询当前用户有权限的应用
		private void DoQueryApplication()
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				ExceptionHelper.TrueThrow(this.LogOnUserInfo == null, "没有登录者信息");
				//********************
				//查询权限确定应用范围
				//********************
				XmlElement root = _XmlRequest.DocumentElement;
				string strParentID = root.GetAttribute("parent_id");

				string strSQL = string.Empty;
				bool bAdminUser = SecurityCheck.IsAdminUser(LogOnUserInfo.UserLogOnName);
				string strAppLevels = string.Empty;
				if (false == bAdminUser)
				{
					#region 得到父应用的resource_level
					strSQL = string.Format("SELECT RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = {0}",
						TSqlBuilder.Instance.CheckQuotationMark(strParentID, true));
					string strParentLevel = string.Empty;
					object obj = InnerCommon.ExecuteScalar(strSQL);
					if (obj != null)
						strParentLevel = obj.ToString();
					string strTemp = string.Empty;
					DataTable userApps = SecurityCheck.GetUserApplications(LogOnUserInfo.UserLogOnName,
						UserValueType.LogonName, RightMaskType.Self).Tables[0];
					for (int i = 0; i < userApps.Rows.Count; i++)
					{
						strTemp = userApps.Rows[i]["RESOURCE_LEVEL"].ToString();
						if (strTemp.Length >= strParentLevel.Length + 3 &&
							strTemp.Substring(0, strParentLevel.Length) == strParentLevel)
						{
							if (strAppLevels == string.Empty)
								strAppLevels += strTemp.Substring(0, strParentLevel.Length + 3);
							else
								strAppLevels += "," + strTemp.Substring(0, strParentLevel.Length + 3);
						}
					}
					#endregion
				}

				#region Prepare SQL
				strSQL = "SELECT ID,NAME, CODE_NAME, DESCRIPTION, SORT_ID, RESOURCE_LEVEL, CHILDREN_COUNT, ADD_SUBAPP, USE_SCOPE, INHERITED_STATE "
					+ "FROM APPLICATIONS ";
				if (strParentID != "")
					strSQL += "WHERE LEFT(RESOURCE_LEVEL, LEN(RESOURCE_LEVEL)-3) = (SELECT RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strParentID, true) + ") ";
				else
					strSQL += "WHERE LEN(RESOURCE_LEVEL) = 3 ";
				if (strAppLevels != string.Empty)
					strSQL += string.Format("\n AND RESOURCE_LEVEL IN ({0})", InnerCommon.AddMulitStrWithQuotationMark(strAppLevels));
				else
					if (false == bAdminUser) strSQL += "\n AND (1=0)";

				strSQL += " ORDER BY SORT_ID";
				#endregion

				DataSet ds = InnerCommon.ExecuteDataset(strSQL);
				_XmlResult = InnerCommon.GetXmlDoc(ds);
				DataTable userRoleDT = null;
				DataTable appFuncDT = null;

				foreach (XmlNode xNode in _XmlResult.FirstChild.ChildNodes)
				{
					string appCodeName = xNode.SelectSingleNode(".//CODE_NAME").InnerText;
					if (false == bAdminUser)
					{
						if (userRoleDT == null)
							userRoleDT = SecurityCheck.GetUserApplicationsRoles(LogOnUserInfo.UserLogOnName, UserValueType.LogonName,
								RightMaskType.Self, DelegationMaskType.Original).Tables[0];
						if (appFuncDT == null)
						{
							string roleIDs = "''";
							foreach (DataRow row in userRoleDT.Rows)
							{
								roleIDs += string.Format(",'{0}'", row["ID"]);
							}
							strSQL = string.Format(@"SELECT A.CODE_NAME APP_CODE_NAME, F.CODE_NAME FUN_CODE_NAME
														FROM ROLES R 
														INNER JOIN APPLICATIONS A ON R.APP_ID = A.ID
														INNER JOIN ROLE_TO_FUNCTIONS RF ON R.ID = RF.ROLE_ID
														INNER JOIN FUNCTIONS F ON RF.FUNC_ID = F.ID
														WHERE R.ID IN ({0})", roleIDs);
							appFuncDT = InnerCommon.ExecuteDataset(strSQL).Tables[0];
						}
						//显示服务范围结点
						if (appFuncDT.Select(string.Format("[APP_CODE_NAME]= '{0}' AND [FUN_CODE_NAME] IN ( 'ADD_SCOPE_FUNC','DELETE_SCOPE_FUNC' )", appCodeName)).Length > 0)
							XmlHelper.AppendNode(xNode, "APP_SCOPES", "True");
						else
							XmlHelper.AppendNode(xNode, "APP_SCOPES", "False");

						//显示自授权结点
						if (appFuncDT.Select(string.Format("[APP_CODE_NAME]= '{0}' AND [FUN_CODE_NAME] IN ( 'SELF_MAINTAIN_FUNC' )", appCodeName)).Length > 0)
							XmlHelper.AppendNode(xNode, "SELF_MAINTAIN_FUNC", "True");
						else
							XmlHelper.AppendNode(xNode, "SELF_MAINTAIN_FUNC", "False");

						//显示应用角色结点
						if (appFuncDT.Select(string.Format("[APP_CODE_NAME]= '{0}' AND [FUN_CODE_NAME] IN ( 'MODIFY_SCOPE_FUNC','ADD_ROLE_FUNC','DELETE_ROLE_FUNC','MODIFY_ROLE_FUNC','ADD_OBJECT_FUNC','DELETE_OBJECT_FUNC','MODIFY_OBJECT_FUNC' )", appCodeName)).Length > 0)
							XmlHelper.AppendNode(xNode, "APP_ROLES", "True");
						else
							XmlHelper.AppendNode(xNode, "APP_ROLES", "False");

						//显示应用功能结点
						if (appFuncDT.Select(string.Format("[APP_CODE_NAME]= '{0}' AND [FUN_CODE_NAME] IN ( 'ADD_FUNCTION_FUNC','DELETE_FUNCTION_FUNC','MODIFY_FUNCTION_FUNC','RTF_MAINTAIN_FUNC' )", appCodeName)).Length > 0)
							XmlHelper.AppendNode(xNode, "APP_FUNCTIONS", "True");
						else
							XmlHelper.AppendNode(xNode, "APP_FUNCTIONS", "False");
					}
					else
					{
						XmlHelper.AppendNode(xNode, "APP_SCOPES", "True");
						XmlHelper.AppendNode(xNode, "SELF_MAINTAIN_FUNC", "True");
						XmlHelper.AppendNode(xNode, "APP_ROLES", "True");
						XmlHelper.AppendNode(xNode, "APP_FUNCTIONS", "True");
					}
				}
			}

		}

		/// <summary>
		/// 查询某应用下某类型的所有角色
		/// </summary>
		private void DoQueryRole()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strAppID = root.GetAttribute("app_id");
			string strClassify = root.GetAttribute("classify");

			string strSQL = "SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,CLASSIFY,SORT_ID,INHERITED,ALLOW_DELEGATE "
				+ " FROM ROLES WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
				+ " AND CLASSIFY = " + TSqlBuilder.Instance.CheckQuotationMark(strClassify, true)
				+ " ORDER BY SORT_ID";

			_XmlResult = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSQL));
		}

		/// <summary>
		/// 查询某应用下某种类型的所有功能
		/// </summary>
		private void DoQueryFunction()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strAppID = root.GetAttribute("app_id");
			string strClassify = root.GetAttribute("classify");
			string strFuncSetID = root.GetAttribute("parent_id");

			string strSQL = string.Empty;

			if (strFuncSetID == string.Empty)
			{
				strSQL = "SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,SORT_ID,CHILDREN_COUNT,RESOURCE_LEVEL,LOWEST_SET,INHERITED,CLASSIFY,1 AS TYPE "
					+ " FROM FUNCTION_SETS "
					+ " WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
					+ " AND CLASSIFY = " + TSqlBuilder.Instance.CheckQuotationMark(strClassify, true)
					+ " AND LEN(RESOURCE_LEVEL) = 3";

				strSQL += " union all SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,SORT_ID,0 AS CHILDREN_COUNT,'' AS RESOURCE_LEVEL,'' "
					+ "AS LOWEST_SET,INHERITED,CLASSIFY,0 AS TYPE"
					+ " FROM FUNCTIONS "
					+ " WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
					+ " AND CLASSIFY = " + TSqlBuilder.Instance.CheckQuotationMark(strClassify, true)
					+ " AND ID NOT IN (SELECT FUNC_ID AS ID FROM FUNC_SET_TO_FUNCS)"
					+ " ORDER BY TYPE DESC, SORT_ID";
			}
			else
			{
				strSQL = "SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,SORT_ID,CHILDREN_COUNT,RESOURCE_LEVEL,LOWEST_SET,INHERITED,CLASSIFY,1 AS TYPE "
					+ " FROM FUNCTION_SETS "
					+ " WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
					+ " AND CLASSIFY = " + TSqlBuilder.Instance.CheckQuotationMark(strClassify, true)
					+ " AND LEN(RESOURCE_LEVEL) = LEN((SELECT RESOURCE_LEVEL FROM FUNCTION_SETS WHERE ID = "
					+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + ")) + 3"
					+ " AND RESOURCE_LEVEL LIKE (SELECT RESOURCE_LEVEL FROM FUNCTION_SETS WHERE ID = "
					+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + ") + '%'";

				strSQL += " union all SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,SORT_ID,0 AS CHILDREN_COUNT,'' AS RESOURCE_LEVEL,'' AS LOWEST_SET,INHERITED,CLASSIFY,0 AS TYPE "
					+ " FROM FUNCTIONS "
					+ " WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
					+ " AND CLASSIFY = " + TSqlBuilder.Instance.CheckQuotationMark(strClassify, true)
					+ " AND ID IN (SELECT FUNC_ID AS ID FROM FUNC_SET_TO_FUNCS WHERE FUNC_SET_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + ")"
					+ " ORDER BY SORT_ID;";
			}
			_XmlResult = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSQL));
		}

		/// <summary>
		/// 查询某应用下的所有服务范围
		/// </summary>
		private void DoQueryAppScope()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strAppID = root.GetAttribute("app_id");

			string strSQL = "SELECT ID,APP_ID,NAME,CODE_NAME, EXPRESSION,DESCRIPTION,CLASSIFY,SORT_ID,INHERITED "
				+ " FROM SCOPES "
				+ " WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
				+ " ORDER BY DESCRIPTION";

			_XmlResult = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSQL));
		}

		/// <summary>
		/// 查询某功能与所有角色的对应关系
		/// </summary>
		private void DoQueryFuncToRole()
		{

			XmlElement root = _XmlRequest.DocumentElement;
			string strAppID = root.GetAttribute("app_id");
			string strFuncID = root.GetAttribute("func_id");

			string strSQL = @"SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,CLASSIFY,SORT_ID,INHERITED,ALLOW_DELEGATE,{1} AS FUNC_ID 
							FROM ROLES  
							WHERE APP_ID = {0} 
							AND ID IN (SELECT ROLE_ID AS ID FROM ROLE_TO_FUNCTIONS WHERE FUNC_ID = {1})
							AND CLASSIFY = (SELECT CLASSIFY FROM FUNCTIONS WHERE ID = {1}) 

							union all 

							SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,CLASSIFY,SORT_ID,INHERITED,ALLOW_DELEGATE,'' AS FUNC_ID 
							FROM ROLES  
							WHERE APP_ID = {0} 
							AND ID NOT IN 
							(SELECT ROLE_ID AS ID FROM ROLE_TO_FUNCTIONS WHERE FUNC_ID = {1})
							AND CLASSIFY = (SELECT CLASSIFY FROM FUNCTIONS WHERE ID = {1})
							ORDER BY SORT_ID";
			strSQL = string.Format(strSQL,
				TSqlBuilder.Instance.CheckQuotationMark(strAppID, true),
				TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true));
#if DEBUG
			Debug.WriteLine(strSQL.ToString());
#endif
			_XmlResult = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSQL));
		}

		/// <summary>
		/// 查询某角色与所有功能的对应关系
		/// </summary>
		private void DoQueryRoleToFunc()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strAppID = root.GetAttribute("app_id");
			string strRoleID = root.GetAttribute("role_id");

			string strSQL = "SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,CLASSIFY,SORT_ID,INHERITED,"
				+ TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true) + " AS ROLE_ID "
				+ " FROM FUNCTIONS "
				+ " WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
				+ " AND ID IN (SELECT FUNC_ID AS ID FROM ROLE_TO_FUNCTIONS WHERE ROLE_ID = "
				+ TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true)
				+ ")";

			strSQL += " union all SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,CLASSIFY,SORT_ID,INHERITED,'' AS ROLE_ID "
				+ " FROM FUNCTIONS "
				+ " WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
				+ " AND ID NOT IN (SELECT FUNC_ID AS ID FROM ROLE_TO_FUNCTIONS WHERE ROLE_ID = "
				+ TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true)
				+ ")"
				+ " ORDER BY SORT_ID";
			_XmlResult = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSQL));
		}

		private void DoQueryFuncSetToRole()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strAppID = root.GetAttribute("app_id");
			string strFuncSetID = root.GetAttribute("func_set_id");

			string strSQL = @"SELECT FUNC_ID INTO #FUNC_IDS
							FROM FUNC_SET_TO_FUNCS
							WHERE FUNC_SET_ID IN(
								SELECT ID FROM FUNCTION_SETS
								WHERE APP_ID = {0} 
								AND RESOURCE_LEVEL LIKE (SELECT RESOURCE_LEVEL FROM FUNCTION_SETS WHERE ID = {1} ) + '%'
								AND CLASSIFY = (SELECT CLASSIFY FROM FUNCTION_SETS WHERE ID = {1}));

							SELECT ID INTO #ROLE_IDS
							FROM ROLES
							WHERE APP_ID = {0} 
							AND (SELECT COUNT(DISTINCT FUNC_ID) FROM ROLE_TO_FUNCTIONS WHERE ROLE_TO_FUNCTIONS.ROLE_ID = ROLES.ID AND FUNC_ID IN (SELECT * FROM #FUNC_IDS)) = (SELECT COUNT(*) FROM #FUNC_IDS)
							AND (SELECT COUNT(*) FROM #FUNC_IDS) > 0;

							SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,CLASSIFY,SORT_ID,INHERITED,ALLOW_DELEGATE, {1} AS FUNC_ID FROM ROLES
							WHERE ID IN (SELECT * FROM #ROLE_IDS)
							AND APP_ID = {0}
							AND CLASSIFY = ( SELECT CLASSIFY FROM FUNCTION_SETS WHERE ID = {1} )
							UNION ALL
							SELECT ID,APP_ID,NAME,CODE_NAME,DESCRIPTION,CLASSIFY,SORT_ID,INHERITED,ALLOW_DELEGATE,'' AS FUNC_ID FROM ROLES
							WHERE ID NOT IN (SELECT * FROM #ROLE_IDS)
							AND APP_ID = {0}
							AND CLASSIFY = ( SELECT CLASSIFY FROM FUNCTION_SETS WHERE ID = {1} )
							ORDER BY SORT_ID;
							
							SELECT * FROM FUNCTIONS WHERE ID IN (SELECT * FROM #FUNC_IDS)";
			strSQL = string.Format(strSQL, TSqlBuilder.Instance.CheckQuotationMark(strAppID, true), TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true));
#if DEBUG
			Debug.Write(strSQL.ToString());
#endif
			_XmlResult = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSQL));
		}

		private void DoQueryFuncSetToFunc()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strAppID = TSqlBuilder.Instance.CheckQuotationMark(root.GetAttribute("app_id"), true);
			string strFuncSetID = TSqlBuilder.Instance.CheckQuotationMark(root.GetAttribute("func_set_id"), true);

			string strSQL = @"SELECT FUNC_ID INTO #FUNC_IDS
								FROM FUNC_SET_TO_FUNCS
								WHERE FUNC_SET_ID = {0};
								SELECT FUNC_ID INTO #FUNC_IDS2 FROM FUNC_SET_TO_FUNCS
								WHERE FUNC_ID IN (SELECT ID FROM FUNCTIONS WHERE APP_ID = {1});

								SELECT *, 0 AS TYPE, {0} AS FUNC_SET_ID  FROM FUNCTIONS
								WHERE ID IN (SELECT * FROM #FUNC_IDS)
								AND APP_ID = {1}
								AND CLASSIFY = (SELECT CLASSIFY FROM FUNCTION_SETS WHERE ID = {0})
								UNION ALL
								SELECT *, 0 AS TYPE, '' AS FUNC_SET_ID FROM FUNCTIONS
								WHERE ID NOT IN (SELECT * FROM #FUNC_IDS2)
								AND APP_ID = {1}
								AND CLASSIFY = (SELECT CLASSIFY FROM FUNCTION_SETS WHERE ID = {0})
								ORDER BY FUNC_SET_ID DESC, SORT_ID";
			strSQL = string.Format(strSQL, strFuncSetID, strAppID);
			_XmlResult = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSQL));
		}

		private void DoQueryRoleToExp()
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				XmlElement root = _XmlRequest.DocumentElement;
				string strAppID = root.GetAttribute("app_id");
				string strRoleID = root.GetAttribute("role_id");

				//string strSQL = "SELECT ID, ROLE_ID, NAME, EXPRESSION, DESCRIPTION, SORT_ID, INHERITED, CLASSIFY "
				//    + " FROM EXPRESSIONS "
				//    + " WHERE ROLE_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID)
				//    + " ORDER BY CLASSIFY DESC, DESCRIPTION, SORT_ID";

				string strSQL = string.Format("SELECT CODE_NAME FROM APPLICATIONS WHERE ID = {0}; SELECT CODE_NAME FROM ROLES WHERE ID={1}",
					TSqlBuilder.Instance.CheckQuotationMark(strAppID, true), TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true));
				DataSet ds = InnerCommon.ExecuteDataset(strSQL);
				string strAppCodeName = string.Empty;
				string strRoleCodeName = string.Empty;
				if (ds.Tables[0].Rows.Count > 0)
				{
					strAppCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();
				}
				if (ds.Tables[1].Rows.Count > 0)
				{
					strRoleCodeName = ds.Tables[1].Rows[0]["CODE_NAME"].ToString();
				}

				//如果不是总管理员，则得到机构管理范围
				string strOrgRoot = string.Empty;
				if (false == SecurityCheck.IsAdminUser(LogOnUserInfo.UserLogOnName))
				{
					ds = SecurityCheck.GetUserFunctionsScopes(LogOnUserInfo.UserLogOnName, strAppCodeName, "ADD_OBJECT_FUNC,DELETE_OBJECT_FUNC,MODIFY_OBJECT_FUNC");
					for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
					{
						if (strOrgRoot == string.Empty)
							strOrgRoot += ds.Tables[0].Rows[i]["DESCRIPTION"].ToString();
						else
							strOrgRoot += "," + ds.Tables[0].Rows[i]["DESCRIPTION"].ToString();

					}
					if (strOrgRoot == string.Empty)
						strOrgRoot = "NoOrgRoot";
				}
				ds = SecurityCheck.GetChildrenInRoles(strOrgRoot, strAppCodeName, strRoleCodeName, false, false, false);
				_XmlResult = InnerCommon.GetXmlDoc(ds);
			}
		}

		private void DoQueryExpScope()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strAppID = TSqlBuilder.Instance.CheckQuotationMark(root.GetAttribute("app_id"), true);
			string strExpID = TSqlBuilder.Instance.CheckQuotationMark(root.GetAttribute("exp_id"), true);

			string strSQL = @"SELECT SCOPE_ID INTO #SCOPE_IDS
							FROM EXP_TO_SCOPES 
							WHERE EXP_ID = {1}


							SELECT * , {1} AS EXP_ID
							FROM SCOPES 
							WHERE APP_ID = {0}
							AND ID IN (SELECT * FROM #SCOPE_IDS)
							UNION ALL
							SELECT * , '' AS EXP_ID
							FROM SCOPES 
							WHERE APP_ID = {0}
							AND ID NOT IN (SELECT * FROM #SCOPE_IDS)
							ORDER BY EXP_ID DESC, DESCRIPTION ";
			strSQL = string.Format(strSQL, strAppID, strExpID);
			_XmlResult = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSQL));
		}

		protected void DoQueryObj()
		{
			XmlElement root = _XmlRequest.DocumentElement;
			string strTable = root.GetAttribute("type");
			string strAppID = root.GetAttribute("app_id");
			string strObjID = root.GetAttribute("id");

			string strAnd;
			if (strTable == "APPLICATIONS")
				strAnd = string.Empty;
			else
				strAnd = string.Format(" AND APP_ID = {0}", TSqlBuilder.Instance.CheckQuotationMark(strAppID, true));

			string strSQL = @"SELECT * 
							FROM {0} 
							WHERE ID = {1} 
							{2}";
			strSQL = string.Format(strSQL, strTable, TSqlBuilder.Instance.CheckQuotationMark(strObjID, true), strAnd);

			_XmlResult = InnerCommon.GetXmlDoc(InnerCommon.ExecuteDataset(strSQL));
		}

		/// <summary>
		/// 得到当前人员指定功能的，相应服务范围
		/// </summary>
		/// <example>
		/// <code>
		///		<queryUserFuncScopes app_code_name="asdf" func_code_names="ADD_OBJECT_FUNC" delegation_mask="3" scope_mask="1"/>
		/// </code>
		/// </example>
		protected void DoQueryUserFuncScopes()
		{
			XmlElement root = _XmlRequest.DocumentElement;

			string appCodeName = root.GetAttribute("app_code_name");
			string funcCodeNames = root.GetAttribute("func_code_names");
			string delegationMask = root.GetAttribute("delegation_mask");
			string scopeMask = root.GetAttribute("scope_mask");

			DelegationMaskType dm = DelegationMaskType.All;
			ScopeMaskType sm = ScopeMaskType.All;

			if (delegationMask != string.Empty)
				dm = (DelegationMaskType)int.Parse(delegationMask);
			if (scopeMask != string.Empty)
				sm = (ScopeMaskType)int.Parse(scopeMask);

			string userID = LogOnUserInfo.UserLogOnName;

			//得到服务范围
			DataTable dt = SecurityCheck.GetUserFunctionsScopes(userID, appCodeName, funcCodeNames, UserValueType.LogonName, dm, sm).Tables[0];

			_XmlResult = new XmlDocument();
			_XmlResult.LoadXml("<DataSet/>");

			XmlHelper.AppendNode(_XmlResult.FirstChild, "Table");

			string strRootOrg;
			string[] arrRootOrg;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				strRootOrg = dt.Rows[i]["DESCRIPTION"].ToString();
				arrRootOrg = strRootOrg.Split(new char[] { ',', ';' });
				for (int j = 0; j < arrRootOrg.Length; j++)
				{
					if (arrRootOrg[j] != string.Empty)
						if (_XmlResult.SelectSingleNode(string.Format(".//ORGANIZATIONS[.='{0}']", arrRootOrg[j])) == null)
							XmlHelper.AppendNode(_XmlResult.FirstChild.FirstChild, "ORGANIZATIONS", arrRootOrg[j]);
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
