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
using System.Reflection;
using System.Transactions;
using System.Data.Common;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.AppAdmin;
using MCS.Library.Accredit.LogAdmin;
using MCS.Library.Accredit.ExpParsing;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Applications.AppAdmin.Properties;
using MCS.Applications.AppAdmin.Common;

namespace MCS.Applications.AppAdmin.XmlRequestService
{
	/// <summary>
	/// XmlAOSWriteRequest 的摘要说明。
	/// </summary>
	public partial class XmlAOSWriteRequest : XmlRequestUserWebClass
	{		//功能或角色标识
		private enum FunctionOrRole
		{
			//功能
			Fuunction,
			//角色
			Role
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// 在此处放置用户代码以初始化页面
			switch (RootName)
			{
				case "checkAndDeleteUserInRole": CheckObjInRole(_XmlRequest);
					break;
				case "Insert":
					switch (_XmlRequest.DocumentElement.FirstChild.Name)
					{
						case "APPLICATIONS": InsertApplication(_XmlRequest);
							SecurityCheck.RemoveAllCache();
							break;
						case "SCOPES": InsertScope(_XmlRequest);
							SecurityCheck.RemoveAllCache();
							break;
						case "EXPRESSIONS": InsertExp(_XmlRequest);
							SecurityCheck.RemoveAllCache();
							break;
					}
					break;
				case "Update":
					switch (_XmlRequest.DocumentElement.FirstChild.Name)
					{
						case "APPLICATIONS": _XmlResult = UpdateApplication(_XmlRequest);
							SecurityCheck.RemoveAllCache();
							break;
					}
					break;
				case "Delete":
					switch (_XmlRequest.DocumentElement.FirstChild.Name)
					{
						case "APPLICATIONS": _XmlResult = DeleteApplication(_XmlRequest);
							SecurityCheck.RemoveAllCache();
							break;
						case "SCOPES": _XmlResult = DeleteScope(_XmlRequest);
							SecurityCheck.RemoveAllCache();
							break;
						case "EXPRESSIONS": _XmlResult = DeleteExp(_XmlRequest);
							SecurityCheck.RemoveAllCache();
							break;
					}
					break;
				case "ETS": _XmlResult = ModifyExpToScope(_XmlRequest);
					SecurityCheck.RemoveAllCache();
					break;
				default: break;
			}
		}

		#region 应用维护

		#region insert应用
		/// <summary>
		/// 插入应用
		/// </summary>
		/// <Insert>
		///		<APPLICATIONS parentLevel="" parentID="">
		///			<SET>
		///				<NAME>new通用授权</NAME>
		///				<CODE_NAME>new_APP_ADMIN_APPLICATION</CODE_NAME>
		///				<DESCRIPTION>new通用授权管理平台</DESCRIPTION>
		///				<ADD_SUBAPP>n</ADD_SUBAPP>
		///				<USE_SCOPE>n</USE_SCOPE>
		///				<INHERITED_STATE>0</INHERITED_STATE>
		///			</SET>
		///		</APPLICATIONS>
		///</Insert>
		protected void InsertApplication(XmlDocument xmlDocInsert)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
				{
					XmlElement xmlEle = (XmlElement)xmlDocInsert.DocumentElement.FirstChild;
					string strParentLevel = xmlEle.GetAttribute("parentLevel");
					string strParentID = xmlEle.GetAttribute("parentID");
					string strUseScope = xmlDocInsert.SelectSingleNode(".//USE_SCOPE").InnerText;

					#region 检查权限
					//****************
					//检查权限
					//**************
					//应用增加的权限在父应用中
					bool hasPermi = false;
					if (strParentID != string.Empty)
						hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, new Guid(strParentID), "ADD_APPLICATION_FUNC");

					#region 总管理员可以加第一级应用
					if (false == hasPermi)
					{
						if (SecurityCheck.IsAdminUser(LogOnUserInfo.UserLogOnName) && strParentID == string.Empty)
							hasPermi = true;
					}

					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, "没有相应权限!");
					#endregion
					#endregion
					//插入应用
					XmlNode nodeSet = xmlDocInsert.DocumentElement.SelectSingleNode(".//SET");
					string strAppID = Guid.NewGuid().ToString();
					XmlHelper.AppendNode<string>(nodeSet, "ID", strAppID);

					#region 插入新应用以及相关自授权角色功能以及关系
					XmlDocument xmlDoc;
					xmlDoc = ApplicationDataToDB(xmlDocInsert, strParentLevel, strAppID);

					//加入自授权角色功能、功能集合、对应关系
					string strExpID;//新加的当前人员的授权对象的ID
					strExpID = AutoInsertApplicationRTF(strAppID);

					Database database = DatabaseFactory.Create(context);
					DbCommand command = database.CreateStoredProcedureCommand("COPY_APPLICATION_DATA");
					database.AddInParameter(command, "PARENT_APP_ID", DbType.String, strParentID);
					database.AddInParameter(command, "CURRENT_APP_ID", DbType.String, strAppID);
					database.ExecuteNonQuery(command);
					//复制应用的继承数据

					//给当前人员加上最大的服务范围
					AddAdminUserScope(strAppID, strExpID);
					#endregion
					_XmlResult = xmlDoc;
				}
				scope.Complete();
			}
			//写日志
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.ADD_APPLICATION_FUNC.ToString(),
				"增加应用系统", xmlDocInsert.OuterXml);
		}

		/// <summary>
		/// 新的应用入库
		/// </summary>
		/// <param name="insertXmlDom"></param>
		/// <param name="strParentResLevel"></param>
		/// <param name="strNewAppID"></param>
		/// <returns></returns>
		private XmlDocument ApplicationDataToDB(XmlDocument insertXmlDom, string strParentResLevel, string strNewAppID)
		{
			strParentResLevel = TSqlBuilder.Instance.CheckQuotationMark(strParentResLevel, true);
			strNewAppID = TSqlBuilder.Instance.CheckQuotationMark(strNewAppID, true);
			StringBuilder colsBuilder = new StringBuilder(256);
			StringBuilder valuesBuilder = new StringBuilder(256);
			colsBuilder.Append("SORT_ID, RESOURCE_LEVEL, MODIFY_TIME");
			valuesBuilder.Append("@MAX_SORT, @MAX_LEVEL, GETDATE()");

			XmlNode setNode = insertXmlDom.DocumentElement.FirstChild.FirstChild;
			foreach (XmlNode node in setNode.ChildNodes)
			{
				colsBuilder.Append(", " + TSqlBuilder.Instance.CheckQuotationMark(node.Name, false));
				valuesBuilder.Append(", " + TSqlBuilder.Instance.CheckQuotationMark(node.InnerText, true));
			}

			string strSql = @"DECLARE 
								@MAX_SORT INT,
								@TAIL NVARCHAR(3),
								@MAX_LEVEL NVARCHAR(32)

								SELECT @MAX_SORT = ISNULL(MAX(SORT_ID),0), @MAX_LEVEL = ISNULL(MAX(RESOURCE_LEVEL), {0}+'000')
								FROM APPLICATIONS 
								WHERE LEFT(RESOURCE_LEVEL, LEN(RESOURCE_LEVEL)-3) = {0}

								SET @TAIL = RIGHT(@MAX_LEVEL, 3)+1
								SET @TAIL = REPLACE(STR(@TAIL, 3), ' ', '0')

								SET @MAX_SORT = @MAX_SORT + 1
								SET @MAX_LEVEL = LEFT(@MAX_LEVEL, LEN(@MAX_LEVEL) - 3) + @TAIL

							INSERT INTO 
							APPLICATIONS ({1}) 
							SELECT {2};
							SELECT * 
							FROM APPLICATIONS 
							WHERE ID = {3} ";
			strSql = string.Format(strSql, strParentResLevel, colsBuilder.ToString(), valuesBuilder.ToString(), strNewAppID);
#if DEBUG
			Debug.WriteLine(strSql);
#endif
			DataSet ds = InnerCommon.ExecuteDataset(strSql);
			return InnerCommon.GetXmlDoc(ds);
		}

		/// <summary>
		/// 自动的为新添加应用建立自授权中的角色、功能以及角色和功能之间的对应关系
		/// </summary>
		/// <param name="dbc"></param>
		/// <param name="strUserDN"></param>
		/// <param name="strUserLogOn"></param>
		/// <param name="strAppID"></param>
		/// <returns></returns>
		/// <remarks>加入自授权信息</remarks>
		private string AutoInsertApplicationRTF(string strAppID)
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				//获得自授权信息板
				XmlDocument xmlTemplate = this.GetXMLDocument("../XML", "preDefineObj");
				//this.GetXMLDocument会在Cache中加载文件，并记录改变，所以要使用临时副本文件
				XmlElement root = (XmlElement)xmlTemplate.DocumentElement.CloneNode(true);
				string strAdminRoleID = string.Empty;
				StringBuilder sqlBuilder = new StringBuilder(1024);

				#region Roles数据入库
				XmlNode roleData = root.SelectSingleNode("ROLE_DATA");
				XmlDocument xsdDoc = this.GetXSDDocument("ROLES");
				int sort = 0;
				foreach (XmlNode roleNode in roleData.ChildNodes)
				{
					string strRoleID = Guid.NewGuid().ToString();
					XmlNode nodeSet = roleNode.FirstChild;
					XmlHelper.AppendNode(nodeSet, "ID", strRoleID);
					XmlHelper.AppendNode(nodeSet, "APP_ID", strAppID);
					XmlHelper.AppendNode(nodeSet, "SORT_ID", (++sort).ToString());
					XmlHelper.AppendNode(nodeSet, "CLASSIFY", "y");
					XmlHelper.AppendNode(nodeSet, "INHERITED", "n");
					XmlHelper.AppendNode(nodeSet, "ALLOW_DELEGATE", "n");
					sqlBuilder.Append(InnerCommon.GetOneInsertSqlStr(roleNode, xsdDoc) + ";" + Environment.NewLine);
					string strCodeName = nodeSet.SelectSingleNode("CODE_NAME").InnerText;
					if (strCodeName == "SELF_ADMIN_ROLE")
						strAdminRoleID = TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true);
					foreach (XmlNode rtfNode in root.SelectSingleNode("RTF_DATA").SelectNodes("ROLE_TO_FUNCTIONS/SET[ROLE_ID=\"" + strCodeName + "\"]"))
						rtfNode.SelectSingleNode("ROLE_ID").InnerText = strRoleID;
				}
				#endregion

				#region functions入库
				XmlNode funcData = root.SelectSingleNode("FUNCTION_DATA");
				xsdDoc = this.GetXSDDocument("FUNCTIONS");
				sort = 0;
				foreach (XmlNode funcNode in funcData.ChildNodes)
				{
					string strFuncID = Guid.NewGuid().ToString();
					XmlNode nodeSet = funcNode.FirstChild;
					XmlHelper.AppendNode(nodeSet, "ID", strFuncID);
					XmlHelper.AppendNode(nodeSet, "APP_ID", strAppID);
					XmlHelper.AppendNode(nodeSet, "SORT_ID", (++sort).ToString());
					XmlHelper.AppendNode(nodeSet, "CLASSIFY", "y");
					XmlHelper.AppendNode(nodeSet, "INHERITED", "n");
					sqlBuilder.Append(InnerCommon.GetOneInsertSqlStr(funcNode, xsdDoc) + ";" + Environment.NewLine);
					string strCodeName = nodeSet.SelectSingleNode("CODE_NAME").InnerText;
					foreach (XmlNode rtfNode in root.SelectSingleNode("RTF_DATA").SelectNodes("ROLE_TO_FUNCTIONS/SET[FUNC_ID=\"" + strCodeName + "\"]"))
						rtfNode.SelectSingleNode("FUNC_ID").InnerText = strFuncID;
					foreach (XmlNode fstfNode in root.SelectSingleNode("FSTF_DATA").SelectNodes("FUNC_SET_TO_FUNCS/SET[FUNC_ID=\"" + strCodeName + "\"]"))
						fstfNode.SelectSingleNode("FUNC_ID").InnerText = strFuncID;
				}
				#endregion

				#region RTF数据入库
				XmlNode rtfData = root.SelectSingleNode("RTF_DATA");
				xsdDoc = this.GetXSDDocument("ROLE_TO_FUNCTIONS");
				foreach (XmlNode rtfNode in rtfData.ChildNodes)
				{
					XmlNode nodeSet = rtfNode.FirstChild;
					XmlHelper.AppendNode(nodeSet, "INHERITED", "n");
					sqlBuilder.Append(InnerCommon.GetOneInsertSqlStr(rtfNode, xsdDoc) + ";" + Environment.NewLine);
				}
				#endregion

				#region function_set入库
				XmlNode funcSetData = root.SelectSingleNode("FUNCTION_SET_DATA");
				xsdDoc = this.GetXSDDocument("FUNCTION_SETS");
				sort = 0;
				foreach (XmlNode funcSetNode in funcSetData.ChildNodes)
				{
					string strFuncSetID = Guid.NewGuid().ToString();
					XmlNode nodeSet = funcSetNode.FirstChild;
					XmlHelper.AppendNode(nodeSet, "ID", strFuncSetID);
					XmlHelper.AppendNode(nodeSet, "APP_ID", strAppID);
					XmlHelper.AppendNode(nodeSet, "SORT_ID", (++sort).ToString());
					XmlHelper.AppendNode(nodeSet, "CLASSIFY", "y");
					XmlHelper.AppendNode(nodeSet, "INHERITED", "n");
					sqlBuilder.Append(InnerCommon.GetOneInsertSqlStr(funcSetNode, xsdDoc) + ";" + Environment.NewLine);
					string strCodeName = nodeSet.SelectSingleNode("CODE_NAME").InnerText;
					foreach (XmlNode fstfNode in root.SelectSingleNode("FSTF_DATA").SelectNodes("FUNC_SET_TO_FUNCS/SET[FUNC_SET_ID=\"" + strCodeName + "\"]"))
						fstfNode.SelectSingleNode("FUNC_SET_ID").InnerText = strFuncSetID;
				}
				#endregion

				#region FSTF数据入库
				XmlNode fstfData = root.SelectSingleNode("FSTF_DATA");
				xsdDoc = this.GetXSDDocument("FUNC_SET_TO_FUNCS");
				sort = 0;
				foreach (XmlNode fstfNode in fstfData.ChildNodes)
				{
					XmlNode nodeSet = fstfNode.FirstChild;
					XmlHelper.AppendNode(nodeSet, "SORT_ID", (++sort).ToString());
					sqlBuilder.Append(InnerCommon.GetOneInsertSqlStr(fstfNode, xsdDoc) + ";" + Environment.NewLine);
				}
				#endregion

				#region 把当前人员加入自授权管理员角色中
				string strExpID = Guid.NewGuid().ToString();
				string strUserID = this.LogOnUserInfo.UserGuid;
				string strUserName = string.Empty, strUserParentID = string.Empty, strAllPath = string.Empty;

				for (int j = 0; j < this.LogOnUserInfo.OuUsers.Length; j++)
				{
					if (false == this.LogOnUserInfo.OuUsers[j].Sideline)
					{
						strUserParentID = this.LogOnUserInfo.OuUsers[j].OUGuid;
						strUserName = this.LogOnUserInfo.OuUsers[j].UserDisplayName;
						strAllPath = this.LogOnUserInfo.OuUsers[j].AllPathName;
						break;
					}
				}
				string strExp = string.Format("BelongTo(Users, \"{0}\", \"{1}\")", strUserID, strUserParentID);
				string strReturn = strExpID;
				strExpID = TSqlBuilder.Instance.CheckQuotationMark(strExpID, true);
				strUserID = TSqlBuilder.Instance.CheckQuotationMark(strUserID, true);
				strExp = TSqlBuilder.Instance.CheckQuotationMark(strExp, true);
				strUserName = TSqlBuilder.Instance.CheckQuotationMark(strUserName, true);
				strAllPath = TSqlBuilder.Instance.CheckQuotationMark(strAllPath, true);
				string strSql2 = @"INSERT INTO EXPRESSIONS(ID, ROLE_ID, NAME, EXPRESSION, DESCRIPTION, SORT_ID, INHERITED, CLASSIFY)
								VALUES({0}, {1}, {2}, {3}, {4}, 1, 'n', '0')";
				strSql2 = string.Format(strSql2, strExpID, strAdminRoleID, strUserName, strExp, strAllPath);
				sqlBuilder.Append(strSql2 + ";" + Environment.NewLine);
				#endregion

				DataSet ds = InnerCommon.ExecuteDataset(sqlBuilder.ToString());
				return strReturn;
			}
		}

		private void AddAdminUserScope(string appID, string strExpID)
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				//get 最大的机构范围全路径
				DataTable orgRootDT = OGUReader.GetObjectDepOrgs("USERS", LogOnUserInfo.UserLogOnName,
					SearchObjectColumn.SEARCH_LOGON_NAME, 1, string.Empty).Tables[0];
				string strAllPath = orgRootDT.Rows[0]["ALL_PATH_NAME"].ToString();

				//get scope's id
				string strSql = "SELECT * FROM SCOPES WHERE APP_ID = {0} AND DESCRIPTION = {1}";
				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(appID, true), TSqlBuilder.Instance.CheckQuotationMark(strAllPath, true));

				DataTable scopeDT = InnerCommon.ExecuteDataset(strSql).Tables[0];

				string strScopeID = string.Empty;//范围id


				if (scopeDT.Rows.Count == 0)
				{
					string strID = Guid.NewGuid().ToString();
					string strCodeName = Guid.NewGuid().ToString().Replace("-", "");
					string strExp = string.Format("userDefineScope(\"{0}\")", orgRootDT.Rows[0]["GUID"]);
					string strName = orgRootDT.Rows[0]["DISPLAY_NAME"].ToString();

					//insert机构服务范围
					strSql = @"INSERT INTO SCOPES(ID, APP_ID, NAME, CODE_NAME, EXPRESSION, DESCRIPTION, CLASSIFY, SORT_ID, INHERITED, MODIFY_TIME)
					SELECT {0}, {1}, {2}, {3}, {4}, {5}, 'y', ISNULL(MAX(SORT_ID), 1), 'n', GETDATE()
					FROM SCOPES WHERE APP_ID = {1}";
					strSql = string.Format(strSql,
						TSqlBuilder.Instance.CheckQuotationMark(strID, true),
						TSqlBuilder.Instance.CheckQuotationMark(appID, true),
						TSqlBuilder.Instance.CheckQuotationMark(strName, true),
						TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true),
						TSqlBuilder.Instance.CheckQuotationMark(strExp, true),
						TSqlBuilder.Instance.CheckQuotationMark(strAllPath, true));
					InnerCommon.ExecuteNonQuery(strSql);
					strScopeID = strID;
				}
				else
				{
					strScopeID = scopeDT.Rows[0]["ID"].ToString();
				}

				strSql = @"INSERT INTO EXP_TO_SCOPES(EXP_ID, SCOPE_ID) 
							SELECT TOP 1 {0}, {1} FROM EXP_TO_SCOPES
							WHERE NOT EXISTS (SELECT * FROM EXP_TO_SCOPES 
												WHERE EXP_ID = {0}
												AND SCOPE_ID = {1})";
				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strExpID, true), TSqlBuilder.Instance.CheckQuotationMark(strScopeID, true));


				InnerCommon.ExecuteNonQuery(strSql);
			}
		}

		#endregion

		#region delete应用
		/// <summary>
		/// 删除应用
		/// </summary>
		///	<Delete>
		///		<APPLICATIONS>
		///			<WHERE>
		///				<ID operator="=">ff6332da-4654-45cf-9247-97b5b60998ab</ID>
		///				<CHILDREN_COUNT operator="=">0</CHILDREN_COUNT>
		///			</WHERE>
		///		</APPLICATIONS>
		///		<APPLICATIONS>
		///			...
		///		</APPLICATIONS>
		///	</Delete>
		protected XmlDocument DeleteApplication(XmlDocument xmlDocDelete)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				StringBuilder sqlBuilder = new StringBuilder(256);

				foreach (XmlNode xNode in xmlDocDelete.FirstChild.ChildNodes)
				{
					string appID = xNode.SelectSingleNode(".//ID").InnerText;

					//****************
					//检查权限
					//**************
					//应用的删除权限在本身应用中
					bool hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, new Guid(appID), "DELETE_APPLICATION_FUNC");
					string appName = string.Empty;
					if (false == hasPermi)
					{
						appName = InnerCommon.ExecuteScalar(string.Format("SELECT NAME FROM APPLICATIONS WHERE ID = {0}",
							TSqlBuilder.Instance.CheckQuotationMark(appID, true))).ToString();
					}

					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, string.Format("没有删除应用({0})的相应权限!", appName));
					string strTemp = "DELETE APPLICATIONS WHERE ID = {0} AND CHILDREN_COUNT = 0;";
					sqlBuilder.Append(string.Format(strTemp, TSqlBuilder.Instance.CheckQuotationMark(appID, true)) + Environment.NewLine);
				}

				if (sqlBuilder.Length > 0)
					InnerCommon.ExecuteNonQuery(sqlBuilder.ToString());

				scope.Complete();
			}
			//写日志
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.DELETE_APPLICATION_FUNC.ToString(),
				"删除应用系统", xmlDocDelete.OuterXml);
			return xmlDocDelete;
		}
		#endregion delete应用

		#region update应用

		/// <summary>
		/// update应用
		/// </summary>
		/// <param name="xmlDocUpdate"></param>
		/// <returns></returns>
		/// 
		///<Update>
		///	<APPLICATIONS>
		///		<SET>
		///			<INHERITED_STATE>15</INHERITED_STATE>
		///			<NAME>修改后的名称</NAME>
		///			<CODE_NAME>MODIFY_CODE_NAME</CODE_NAME>
		///			<ADD_SUBAPP>y</ADD_SUBAPP>
		///			<USE_SCOPE>n</USE_SCOPE>
		///		</SET>
		///		<WHERE>
		///			<ID operator="=">17ecf269-1424-4435-9b69-4ec3b78236d2</ID>
		///		</WHERE>
		///	</APPLICATIONS>
		/// </Update>
		protected XmlDocument UpdateApplication(XmlDocument xmlDocUpdate)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
				{
					//****************
					//检查权限
					//**************
					string appID = xmlDocUpdate.SelectSingleNode(".//ID").InnerText;
					bool hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, new Guid(appID), "MODIFY_APPLICATION_FUNC");
					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, "没有相应权限!");
					string strSQL = string.Empty;
					string strAppID = xmlDocUpdate.SelectSingleNode(".//ID").InnerText;

					#region 使用服务范围判断
					XmlNode scopeNode = xmlDocUpdate.SelectSingleNode(".//USE_SCOPE");
					if (scopeNode != null && scopeNode.InnerText == "n")
					{
						strSQL = string.Format("SELECT COUNT(*) FROM EXP_TO_SCOPES WHERE SCOPE_ID IN (SELECT ID FROM SCOPES WHERE APP_ID = {0})",
							TSqlBuilder.Instance.CheckQuotationMark(strAppID, true));
						int count = (int)InnerCommon.ExecuteScalar(strSQL);
						ExceptionHelper.TrueThrow<ApplicationException>(count > 0, "此应用已经使用了服务范围， 不能更改使用服务范围标志！");
					}
					#endregion

					strSQL = string.Format("SELECT INHERITED_STATE FROM APPLICATIONS WHERE ID = {0}",
						TSqlBuilder.Instance.CheckQuotationMark(strAppID, true));
					//更新应用,并得到inherited_state
					strSQL += InnerCommon.GetUpdateSqlStr(xmlDocUpdate, this.GetXSDDocument("APPLICATIONS"));
					string strOldState = InnerCommon.ExecuteScalar(strSQL).ToString();

					XmlNode stateNode = xmlDocUpdate.SelectSingleNode(".//INHERITED_STATE");
					if (stateNode != null)
					{
						Database database = DatabaseFactory.Create(context);
						DbCommand command = database.CreateStoredProcedureCommand("UPDATE_APPLICATION_INHERITED_STATE");
						database.AddInParameter(command, "APP_ID", DbType.String, strAppID);
						database.AddInParameter(command, "OLD_INHERITED_STATE", DbType.String, strOldState);
						database.AddInParameter(command, "NEW_INHERITED_STATE", DbType.String, stateNode.InnerText);
						//修改应用中其它数据的继承状态
						database.ExecuteNonQuery(command);
					}
				}
				scope.Complete();
			}
			//写日志
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.MODIFY_APPLICATION_FUNC.ToString(),
				"修改应用系统的属性", xmlDocUpdate.OuterXml);
			return xmlDocUpdate;
		}
		#endregion update应用

		#endregion 应用维护

		#region 服务范围维护

		#region 插入服务范围
		/***************************************************************
		//文档格式
			<Insert>
				<SCOPES>
					<SET>
						<EXPRESSION>curDepartScope(curUserId)</EXPRESSION>
						<APP_ID>ff6332da-4654-45cf-9247-97b5b60998ab</APP_ID>
						<NAME>本部门</NAME>
						<DESCRIPTION>本部门</DESCRIPTION>
						<INHERITED>n</INHERITED>
						<CLASSIFY>Y</CLASSIFY>
					</SET>
				</SCOPES>
			</Insert>
		***************************************************************/
		protected void InsertScope(XmlDocument xmlDocInsert)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
				{
					//****************
					//检查权限
					//**************
					string appID = xmlDocInsert.SelectSingleNode(".//APP_ID").InnerText;
					bool hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, new Guid(appID), "ADD_SCOPE_FUNC");
					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, "没有相应权限!");

					XmlNode nodeSet = xmlDocInsert.DocumentElement.SelectSingleNode(".//SET");
					//增加modify_time结点
					string strTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", strTime);

					//去掉app_id结点
					XmlNode appIDNode = xmlDocInsert.DocumentElement.SelectSingleNode(".//APP_ID");
					string strAppID = appIDNode.InnerText;
					xmlDocInsert.FirstChild.FirstChild.FirstChild.RemoveChild(appIDNode);

					//去掉inherited结点
					XmlNode inheritedNode = xmlDocInsert.DocumentElement.SelectSingleNode(".//INHERITED");
					string strInherited = inheritedNode.InnerText;
					xmlDocInsert.FirstChild.FirstChild.FirstChild.RemoveChild(inheritedNode);

					//增加code_name结点
					string strCodeName = Guid.NewGuid().ToString().Replace("-", "");
					XmlHelper.AppendNode(xmlDocInsert.SelectSingleNode(".//SET"), "CODE_NAME", strCodeName);

					//本应用增加
					InsertScopeOfApplication(xmlDocInsert, strAppID, strInherited);

					//子应增加
					//根据inherited_state确定继承链
					Database database = DatabaseFactory.Create(context);
					DbCommand command = database.CreateStoredProcedureCommand("GET_INHERITED_APPLICATIONS");
					database.AddInParameter(command, "APP_ID", DbType.String, strAppID);
					database.AddInParameter(command, "INHERITED_STATE", DbType.Int32, (int)InheritedState.SCOPES);
					DataTable appTempDT = database.ExecuteDataSet(command).Tables[0];

					for (int i = 0; i < appTempDT.Rows.Count; i++)
					{
						InsertScopeOfApplication(xmlDocInsert, appTempDT.Rows[i]["ID"].ToString(), "y");
					}
				}
				scope.Complete();
			}

			//写日志
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.ADD_SCOPE_FUNC.ToString(), "增加服务范围", xmlDocInsert.OuterXml);
		}

		private void InsertScopeOfApplication(XmlDocument xmlDoc, string strAppID, string strInherited)
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
				string strExp = xmlDoc.DocumentElement.SelectSingleNode(".//EXPRESSION").InnerText;
				string strClass = xmlDoc.DocumentElement.SelectSingleNode(".//CLASSIFY").InnerText;

				//判断是否已存在服务范围
				string strSql = @"SELECT COUNT(*) FROM SCOPES WHERE APP_ID = {0} AND CLASSIFY = {1} AND EXPRESSION = {2}";
				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strAppID, true),
					TSqlBuilder.Instance.CheckQuotationMark(strClass, true), TSqlBuilder.Instance.CheckQuotationMark(strExp, true));

				int rowCount = int.Parse(InnerCommon.ExecuteScalar(strSql).ToString());

				//存在则退出
				if (rowCount != 0)
					return;

				string strID = Guid.NewGuid().ToString();

				string strCols = "SORT_ID, ID, APP_ID, INHERITED";
				string strValues = string.Format("ISNULL(MAX(SORT_ID)+1,1), {0}, {1}, {2}",
					TSqlBuilder.Instance.CheckQuotationMark(strID, true),
					TSqlBuilder.Instance.CheckQuotationMark(strAppID, true),
					TSqlBuilder.Instance.CheckQuotationMark(strInherited, true));

				XmlNode setNode = xmlDoc.DocumentElement.FirstChild.FirstChild;
				foreach (XmlNode node in setNode.ChildNodes)
				{
					strCols += ", " + TSqlBuilder.Instance.CheckQuotationMark(node.Name, false);
					strValues += ", " + TSqlBuilder.Instance.CheckQuotationMark(node.InnerText, true);
				}

				strSql = @"INSERT INTO 
							SCOPES ({0}) 
							(SELECT {1} 
							FROM SCOPES 
							WHERE APP_ID = {2})";
				strSql = string.Format(strSql, strCols, strValues, TSqlBuilder.Instance.CheckQuotationMark(strAppID, true));

				//Debug.WriteLine(strSql);
				InnerCommon.ExecuteNonQuery(strSql);
			}

		}
		#endregion 插入服务范围

		#region 删除服务范围
		/// <summary>
		/// 删除服务范围
		/// </summary>
		///	<Delete deleteSubApp="y" app_code_name="NEW" use_scope="y">
		///		<SCOPES>
		///			<WHERE>
		///				<ID operator="=">ff6332da-4654-45cf-9247-97b5b60998ab</ID>
		///			</WHERE>
		///		</SCOPES>
		///		<SCOPES>
		///			...
		///		</SCOPES>
		///	</Delete>
		protected XmlDocument DeleteScope(XmlDocument xmlDocDelete)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
				{
					string appCodeName = xmlDocDelete.DocumentElement.GetAttribute("app_code_name");
					bool hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, appCodeName, "DELETE_SCOPE_FUNC");
					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, "没有相应权限!");

					string strSureDelete = xmlDocDelete.DocumentElement.GetAttribute("sureDelete");
					if (strSureDelete == string.Empty)
						strSureDelete = AppResource.DefaultDelObj;

					string strDeleteSubApp = xmlDocDelete.DocumentElement.GetAttribute("deleteSubApp");
					if (strDeleteSubApp == string.Empty)
						strDeleteSubApp = AppResource.DefaultDelSubApp;

					//可同时删除多个服务范围
					foreach (XmlNode socpeNode in xmlDocDelete.FirstChild.ChildNodes)
					{
						string strScopeID = socpeNode.SelectSingleNode(".//ID").InnerText;

						//删除本服务范围
						string strSql = string.Format(@"SELECT * FROM SCOPES WHERE ID = {0};
											SELECT COUNT(*) FROM EXP_TO_SCOPES WHERE SCOPE_ID = {0};
													DELETE FROM SCOPES WHERE ID = {0}",
							TSqlBuilder.Instance.CheckQuotationMark(strScopeID, true));
						DataSet scopeDS = InnerCommon.ExecuteDataset(strSql);
						DataRow scopeDR = scopeDS.Tables[0].Rows[0];
						int iCount = int.Parse(scopeDS.Tables[1].Rows[0][0].ToString());

						ExceptionHelper.TrueThrow<ApplicationException>(iCount > 0,
							string.Format("服务范围({0})已被使用，不能被删除", scopeDR["NAME"].ToString()));

						string strInherite = scopeDR["INHERITED"].ToString();
						ExceptionHelper.TrueThrow<ApplicationException>((strSureDelete == "n") && (strInherite == "y"),
							string.Format("继承的服务范围({0})， 不能被删除！", scopeDR["NAME"].ToString()));

						string strAppID = scopeDR["APP_ID"].ToString();
						string strCodeName = scopeDR["CODE_NAME"].ToString();

						DataTable appTempDT;
						if (strDeleteSubApp == "y")
						{
							#region  处理继承链
							//根据inherited_state确定继承链
							Database database = DatabaseFactory.Create(context);
							DbCommand command = database.CreateStoredProcedureCommand("GET_INHERITED_APPLICATIONS");
							database.AddInParameter(command, "APP_ID", DbType.String, strAppID);
							database.AddInParameter(command, "INHERITED_STATE", DbType.Int32, (int)InheritedState.SCOPES);
							appTempDT = database.ExecuteDataSet(command).Tables[0];

							StringBuilder sqlBuilder = new StringBuilder(256);
							for (int i = 0; i < appTempDT.Rows.Count; i++)
							{
								sqlBuilder.Append(string.Format(@"DELETE FROM SCOPES WHERE ID IN
													(SELECT ID FROM SCOPES WHERE APP_ID = {0} AND CODE_NAME = {1} AND INHERITED = 'y')",
									TSqlBuilder.Instance.CheckQuotationMark(appTempDT.Rows[i]["ID"].ToString(), true),
									TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true)) + ";" + Environment.NewLine);
							}
							if (sqlBuilder.Length > 0)
								InnerCommon.ExecuteNonQuery(sqlBuilder.ToString());
							#endregion
						}
						else
						{
							#region 处理继承链
							//根据inherited_state确定继承链中的第一级应用
							Database database = DatabaseFactory.Create(context);
							DbCommand command = database.CreateStoredProcedureCommand("GET_INHERITED_SON_APPLICATIONS");
							database.AddInParameter(command, "APP_ID", DbType.String, strAppID);
							database.AddInParameter(command, "INHERITED_STATE", DbType.Int32, (int)InheritedState.SCOPES);
							appTempDT = database.ExecuteDataSet(command).Tables[0];

							StringBuilder sqlBuilder = new StringBuilder(256);
							for (int i = 0; i < appTempDT.Rows.Count; i++)
							{
								sqlBuilder.Append(string.Format(@"UPDATE SCOPES SET INHERITED = 'n' WHERE ID IN
													(SELECT ID FROM SCOPES WHERE APP_ID = {0} AND CODE_NAME = {1} AND INHERITED = 'y')",
									TSqlBuilder.Instance.CheckQuotationMark(appTempDT.Rows[i]["ID"].ToString(), true),
									TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true)));
							}
							if (sqlBuilder.Length > 0)
								InnerCommon.ExecuteNonQuery(sqlBuilder.ToString());
							#endregion
						}
					}
				}
				scope.Complete();
			}
			//写日志
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.DELETE_SCOPE_FUNC.ToString(),
				"删除服务范围", xmlDocDelete.OuterXml);
			return xmlDocDelete;
		}

		#endregion 删除服务范围

		#region 维护表达式-服务范围对应关系
		/// <summary>
		/// 维护表达式-服务范围对应关系
		/// </summary>
		/// <param name="xmlDocDelete"></param>
		/// <returns></returns>
		/// <remarks>
		/// 
		/// </remarks>
		/// <example>
		///	xmlDocModify文档格式
		/// <code>
		/// <ETS app_code_name="NEW" use_scope="y">
		///		<Delete>
		///			<EXP_TO_SCOPES>
		///				<WHERE>
		///					<EXP_ID operator="=">acdba130-bcbf-45c3-993a-a7bf8761abb8</EXP_ID>
		///					<SCOPE_ID operator="=">14ecc3c0-c30d-4329-96db-47edf47bfabc</SCOPE_ID>
		///				</WHERE>
		///			</EXP_TO_SCOPES>
		///		</Delete>
		///		<Insert>
		///			<EXP_TO_SCOPES>
		///				<SET>
		///					<EXP_ID>acdba130-bcbf-45c3-993a-a7bf8761abb8</EXP_ID>
		///					<SCOPE_ID>aecaa7d7-a1a6-4dfe-8626-05db003c714f</SCOPE_ID>
		///				</SET>
		///			</EXP_TO_SCOPES>
		///		 </Insert>
		/// </ETS>
		///	
		/// </code>
		/// </example>
		protected XmlDocument ModifyExpToScope(XmlDocument xmlDocModify)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				string appCodeName = xmlDocModify.DocumentElement.GetAttribute("app_code_name");
				ExceptionHelper.FalseThrow<ApplicationException>(
					SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, appCodeName, "MODIFY_SCOPE_FUNC"),
					"没有相应权限!");

				ParseExpression pe = new ParseExpression();
				pe.UserFunctions = (IExpParsing)new DoExpParsing();

				//得到当前用户的服务范围
				ArrayList userScopeList = GetUserScopes(appCodeName, "MODIFY_SCOPE_FUNC", FunctionOrRole.Fuunction);

				XmlDocument curNodeDoc = new XmlDocument();
				StringBuilder sqlBuilder = new StringBuilder(128);
				foreach (XmlNode curNode in xmlDocModify.FirstChild.ChildNodes)
				{
					string scopeName;
					bool bValid = isValidScope(curNode, userScopeList, pe, out scopeName);

					ExceptionHelper.TrueThrow<ApplicationException>(false == bValid,
						string.Format("范围（{0}）不在当前人员的机构服务范围内， 不可修改对应关系！", scopeName));

					curNodeDoc.LoadXml(curNode.OuterXml);
					if (curNode.Name == "Delete")
					{
						sqlBuilder.Append(InnerCommon.GetDeleteSqlStr(curNodeDoc, this.GetXSDDocument("EXP_TO_SCOPES")) + ";" + Environment.NewLine);
					}
					else if (curNode.Name == "Insert")
					{
						sqlBuilder.Append(InnerCommon.GetInsertSqlStr(curNodeDoc, this.GetXSDDocument("EXP_TO_SCOPES")) + ";" + Environment.NewLine);
					}
				}

				if (sqlBuilder.Length > 0)
					InnerCommon.ExecuteNonQuery(sqlBuilder.ToString());
				scope.Complete();
			}
			//写日志
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.MODIFY_SCOPE_FUNC.ToString(),
				"修改服务范围-被授权对象之间的关系", xmlDocModify.OuterXml);

			return xmlDocModify;
		}
		/// <summary>
		/// 判断是可修改的合法机构服务范围
		/// </summary>
		/// <param name="trans"></param>
		/// <param name="scopeNode">要修改的服务范围-被授权对象关系的XML结点</param>
		/// <param name="userScopes">当前人员的机构范围</param>
		/// <param name="pe">表达式解析对象</param>
		/// <param name="scopeName">当前服务范围名称(输出参数)</param>
		/// <returns></returns>
		private bool isValidScope(XmlNode scopeNode, ArrayList userScopes, ParseExpression pe, out string scopeName)
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				//查当前机构服务范围的信息
				scopeName = string.Empty;
				string scopeID = scopeNode.SelectSingleNode(".//SCOPE_ID").InnerText;
				string expID = scopeNode.SelectSingleNode(".//EXP_ID").InnerText;

				//查询被授权对象的相关信息
				string strExp = string.Empty;//被授权对象表达式
				string strSql = string.Format("SELECT * FROM EXPRESSIONS WHERE ID = {0}",
					TSqlBuilder.Instance.CheckQuotationMark(expID, true));

				DataTable expDT = InnerCommon.ExecuteDataset(strSql).Tables[0];
				if (expDT.Rows.Count > 0)
				{
					strExp = expDT.Rows[0]["EXPRESSION"].ToString();
				}

				string objType;
				string objID;
				string objParentID;
				string userAccessLevel;

				DoExpParsing.getObjInfo(strExp, out objType, out objID, out objParentID, out userAccessLevel, pe);

				//查询机构服务范围的全路径
				strSql = string.Format("SELECT * FROM SCOPES WHERE ID = {0}",
					TSqlBuilder.Instance.CheckQuotationMark(scopeID, true));
				DataTable scopeDT = InnerCommon.ExecuteDataset(strSql).Tables[0];

				string scopeExp = string.Empty;
				ArrayList scopePaths = new ArrayList();
				if (scopeDT.Rows.Count > 0)
				{
					//如果是数据服务范围，不做判断
					if (scopeDT.Rows[0]["CLASSIFY"].ToString() == "n")
						return true;

					scopePaths.Add(scopeDT.Rows[0]["DESCRIPTION"].ToString());
					scopeExp = scopeDT.Rows[0]["EXPRESSION"].ToString();
					scopeName = scopeDT.Rows[0]["NAME"].ToString();

					#region 根据当前的被授权对象，得到本关区或本部门的全路径
					if (scopeExp == "curDepartScope(curUserId)" || scopeExp == "curCustomsScope(curUserId)")
					{
						DataTable dt = null;
						//本部门
						if (scopeExp == "curDepartScope(curUserId)")
						{
							dt = OGUReader.GetObjectParentOrgs(objType, objID, SearchObjectColumn.SEARCH_GUID, true, false, string.Empty, string.Empty).Tables[0];
						}
						//本关区
						else if (scopeExp == "curCustomsScope(curUserId)")
						{
							dt = OGUReader.GetObjectDepOrgs(objType, objID, SearchObjectColumn.SEARCH_GUID, 2, string.Empty).Tables[0];
						}

						scopePaths.Clear();

						for (int j = 0; j < dt.Rows.Count; j++)
						{
							string strs = dt.Rows[j]["ALL_PATH_NAME"].ToString();
							if (strs.Length > 0)
								scopePaths.Add(strs);
						}
					}
					#endregion
				}

				#region 判断合法性
				for (int i = 0; i < scopePaths.Count; i++)
				{
					for (int j = 0; j < userScopes.Count; j++)
					{
						if (scopePaths[i].ToString().IndexOf(userScopes[j].ToString()) >= 0)
							return true;
					}
				}
				#endregion

				#region 如果是总管理员则通过
				DataTable detailDT = OGUReader.GetObjectsDetail(objID).Tables[0];
				if (detailDT.Rows.Count > 0 && detailDT.Rows[0]["OBJECTCLASS"].ToString() == "USERS")
				{
					return SecurityCheck.IsAdminUser(detailDT.Rows[0]["LOGON_NAME"].ToString(), UserValueType.LogonName);
				}
				#endregion
				return false;
			}
		}
		#endregion 维护表达式-服务范围对应关系

		#endregion 服务范围维护

		#region 被授权对象维护

		#region 插入被授权对象
		/// <summary>
		/// 插入被授权对象
		/// </summary>
		///	<Insert app_code_name="ASDF" use_scope="y" role_classify="y">
		///		<EXPRESSIONS ALL_PATH_NAME=\"中国海关\01海关总署\00署领导\杨国勋" OBJ_ID="5e3aa542-29c3-40b5-b4cc-617045223c22">
		///			<SET>
		///				<EXPRESSION>BelongTo(USERS, "5e3aa542-29c3-40b5-b4cc-617045223c22", "65eb8160-f0fa-4f1c-8984-2649788fe1d0")</EXPRESSION>
		///				<ROLE_ID>ec16e6b8-5a94-4b9c-963c-08ace45dffd7</ROLE_ID>
		///				<NAME>杨国勋</NAME>
		///				<INHERITED>n</INHERITED>
		///				<CLASSIFY>0</CLASSIFY>
		///			</SET>
		///		</EXPRESSIONS>
		///		<EXPRESSIONS>
		///			<SET>
		///			...
		///			</SET>
		///		</EXPRESSIONS>
		///	</Insert>
		protected void InsertExp(XmlDocument xmlDocInsert)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
				{
					string appCodeName = xmlDocInsert.DocumentElement.GetAttribute("app_code_name");
					string roleClassify = xmlDocInsert.DocumentElement.GetAttribute("role_classify");
					string strPreRoleID = string.Empty;

					#region 判断权限
					bool hasPermi = false;
					if (roleClassify == "y")//自授权角色
						hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, appCodeName, "SELF_MAINTAIN_FUNC");
					else if (roleClassify == "n")//应用授权角色
						hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, appCodeName, "ADD_OBJECT_FUNC");

					bool defaultPermi = false;//默认权限 
					if (false == hasPermi)
					{
						bool adminUser = SecurityCheck.IsAdminUser(LogOnUserInfo.UserLogOnName);
						if (adminUser)
						{
							//总管理员，在通用授权管理系统中，增加被授权对象，总有权限
							if (appCodeName == AppResource.AppCodeName)
								hasPermi = true;
							else//在其它系统中，只能加自己
							{
								XmlNodeList expNodes = xmlDocInsert.SelectNodes(".//EXPRESSION");
								for (int i = 0; i < expNodes.Count; i++)
									if (expNodes.Item(i).InnerText.IndexOf(LogOnUserInfo.UserGuid) >= 0)
									{
										defaultPermi = true;
										break;
									}
							}
						}
					}
					ExceptionHelper.TrueThrow<ApplicationException>(false == hasPermi && !defaultPermi, "没有相应的权限!");
					#endregion

					ParseExpression pe = new ParseExpression();
					pe.UserFunctions = (IExpParsing)(new DoExpParsing());
					//可同时插入多个被授权对象
					DataTable appTempDT = null;
					string strAppID = string.Empty;
					string strRoleCodeName = string.Empty;
					for (int i = 0; i < xmlDocInsert.FirstChild.ChildNodes.Count; i++)
					{
						XmlNode nodeSet = xmlDocInsert.FirstChild.ChildNodes[i].SelectSingleNode(".//SET");
						string strExp = nodeSet.SelectSingleNode(".//EXPRESSION").InnerText;
						string strClassify = nodeSet.SelectSingleNode(".//CLASSIFY").InnerText;

						//在默认权限下，只能加自己
						if (false == hasPermi && defaultPermi)
							if (strExp.IndexOf(LogOnUserInfo.UserGuid) < 0)
								continue;
						#region 构建Sql
						//get description
						string strDescription = xmlDocInsert.FirstChild.ChildNodes[i].Attributes["ALL_PATH_NAME"].Value;
						//增加description结点
						XmlHelper.AppendNode(nodeSet, "DESCRIPTION", strDescription);
						//增加modify_time结点
						string strModifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", strModifyTime);
						//去掉role_id结点
						XmlNode roleIDNode = nodeSet.SelectSingleNode(".//ROLE_ID");
						string strRoleID = roleIDNode.InnerText;
						nodeSet.RemoveChild(roleIDNode);
						//去掉inherited结点
						XmlNode inheritedNode = nodeSet.SelectSingleNode(".//INHERITED");
						string strInherited = inheritedNode.InnerText;
						nodeSet.RemoveChild(inheritedNode);
						//得到所有的已存在的被授权对象的GUID
						string strSql = string.Empty;
						//0.如果表达式已存在，则跳过
						strSql = string.Format("SELECT COUNT(*) FROM EXPRESSIONS WHERE ROLE_ID = {0} AND CLASSIFY = {1} AND EXPRESSION = {2}",
							TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true),
							TSqlBuilder.Instance.CheckQuotationMark(strClassify, true),
							TSqlBuilder.Instance.CheckQuotationMark(strExp, true));
						#endregion
						int iCount = (int)InnerCommon.ExecuteScalar(strSql);
						if (iCount != 0)
							continue;

						//1.本应用增加
						InsertExpOfApplication(nodeSet, strRoleID, strInherited);

						//2.子应增加
						//自授权的被授权对象不继承
						if (roleClassify == "n")
						{
							#region 自授权的被授权对象不继承
							if (strRoleID != strPreRoleID || appTempDT == null)
							{
								//get app_id, code_name of role
								strSql = "SELECT APP_ID, CODE_NAME FROM ROLES WHERE ID = {0}";
								strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true));
								DataTable DT = InnerCommon.ExecuteDataset(strSql).Tables[0];
								strAppID = DT.Rows[0]["APP_ID"].ToString();
								strRoleCodeName = DT.Rows[0]["CODE_NAME"].ToString();

								//根据inherited_state确定继承链
								Database database = DatabaseFactory.Create(context);
								DbCommand command = database.CreateStoredProcedureCommand("GET_INHERITED_APPLICATIONS");
								database.AddInParameter(command, "APP_ID", DbType.String, strAppID);
								database.AddInParameter(command, "INHERITED_STATE", DbType.Int32, (int)(InheritedState.ROLES | InheritedState.OBJECT));
								appTempDT = database.ExecuteDataSet(command).Tables[0];
							}
							strPreRoleID = strRoleID;

							for (int j = 0; j < appTempDT.Rows.Count; j++)
							{
								string subAppID = appTempDT.Rows[j]["ID"].ToString();
								//get subRoleID
								strSql = "SELECT ID FROM ROLES WHERE APP_ID = {0} AND CODE_NAME = {1} AND INHERITED = 'y'";
								strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(subAppID, true),
									TSqlBuilder.Instance.CheckQuotationMark(strRoleCodeName, true));

								object obj = InnerCommon.ExecuteScalar(strSql);
								if (obj != null)
									InsertExpOfApplication(nodeSet, obj.ToString(), "y");
							}
							#endregion
						}
					}
				}
				scope.Complete();
			}
			//写日志
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.ADD_OBJECT_FUNC.ToString(),
				"给角色增加被授权对象", xmlDocInsert.OuterXml);
		}

		private void InsertExpOfApplication(XmlNode xNode, string strRoleID, string strInherited)
		{
			string strObjID = ((XmlElement)xNode.ParentNode).GetAttribute("OBJ_ID");
			string strSql = string.Empty;

			//get id of expression
			string strID = Guid.NewGuid().ToString();

			string strCols = "SORT_ID, ID, ROLE_ID, INHERITED";
			string strValues = string.Format("ISNULL(MAX(SORT_ID)+1,1), {0}, {1}, {2}",
				TSqlBuilder.Instance.CheckQuotationMark(strID, true),
				TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true),
				TSqlBuilder.Instance.CheckQuotationMark(strInherited, true));

			foreach (XmlNode node in xNode.ChildNodes)
			{
				strCols += ", " + TSqlBuilder.Instance.CheckQuotationMark(node.Name, false);
				strValues += ", " + TSqlBuilder.Instance.CheckQuotationMark(node.InnerText, true);
			}

			strSql = @"INSERT INTO 
							EXPRESSIONS ({0}) 
							(SELECT {1} 
							FROM EXPRESSIONS 
							WHERE ROLE_ID = {2});";

			strSql = string.Format(strSql, strCols, strValues, TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true));

			//增加被授权对象的服务范围

			//Debug.WriteLine(strSql);
			InnerCommon.ExecuteNonQuery(strSql);
		}

		#endregion 插入被授权对象


		#region delete被授权对象
		/// <summary>
		/// 删除被授权对象
		/// </summary>
		///	<Delete deleteSubApp="y" app_code_name="NEW" use_scope="y" role_classify="n">
		///		<EXPRESSIONS>
		///			<WHERE>
		///				<ID operator="=">ff6332da-4654-45cf-9247-97b5b60998ab</ID>
		///			</WHERE>
		///		</EXPRESSIONS>
		///		<EXPRESSIONS>
		///			...
		///		</EXPRESSIONS>
		///	</Delete>
		protected XmlDocument DeleteExp(XmlDocument xmlDocDelete)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
				{
					string appCodeName = xmlDocDelete.DocumentElement.GetAttribute("app_code_name");
					string roleClassify = xmlDocDelete.DocumentElement.GetAttribute("role_classify");
					#region 检查权限
					bool hasPermi = false;
					if (roleClassify == "y")//自授权角色
						hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, appCodeName, "SELF_MAINTAIN_FUNC");
					else if (roleClassify == "n")//应用授权角色
						hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, appCodeName, "DELETE_OBJECT_FUNC");

					if (false == hasPermi)
					{
						bool adminUser = SecurityCheck.IsAdminUser(LogOnUserInfo.UserLogOnName);
						if (adminUser)
						{
							//总管理员，在通用授权管理系统中，删除被授权对象，总有权限
							if (appCodeName == AppResource.AppCodeName)
								hasPermi = true;
						}
					}

					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, "没有相应权限!");
					#endregion

					#region Prepare Sql
					//get sureDelete
					string strSureDelete = xmlDocDelete.DocumentElement.GetAttribute("sureDelete");
					if (strSureDelete == string.Empty)
						strSureDelete = AppResource.DefaultDelObj;

					//get deleteSubApp
					string strDeleteSubApp = xmlDocDelete.DocumentElement.GetAttribute("deleteSubApp");
					if (strDeleteSubApp == string.Empty)
						strDeleteSubApp = AppResource.DefaultDelSubApp;

					//得到当前人员的机构服务范围
					string useScope = xmlDocDelete.DocumentElement.GetAttribute("use_scope");

					ArrayList userScopeList = null;
					if (useScope == "y")
					{
						userScopeList = GetUserScopes(appCodeName, "DELETE_OBJECT_FUNC", FunctionOrRole.Fuunction);
					}
					#endregion

					#region 可同时删除多个被授权对象
					foreach (XmlNode expNode in xmlDocDelete.FirstChild.ChildNodes)
					{
						string strExpID = expNode.SelectSingleNode(".//ID").InnerText;

						//删除本被授权对象
						string strSql = string.Format(@"SELECT * FROM EXPRESSIONS WHERE ID = {0};
													DELETE FROM EXPRESSIONS WHERE ID = {0}",
							TSqlBuilder.Instance.CheckQuotationMark(strExpID, true));

						DataRow expDR = InnerCommon.ExecuteDataset(strSql).Tables[0].Rows[0];

						//判断被授权对象是否在当前人员的机构服务范围中
						bool bTemp = true;
						if (useScope == "y")
						{
							bTemp = IsInUserScopes(userScopeList, expDR["DESCRIPTION"].ToString(), appCodeName);
						}
						ExceptionHelper.TrueThrow<ApplicationException>(false == bTemp,
							string.Format("被授权对象（{0}）不在当前用户管理的机构范围中，不能删除!", expDR["NAME"].ToString()));

						//get inherite
						string strInherite = expDR["INHERITED"].ToString();

						ExceptionHelper.TrueThrow<ApplicationException>((strSureDelete == "n") && (strInherite == "y"),
							string.Format("继承的被授权对象({0})， 不能被删除！", expDR["NAME"].ToString()));

						//get role_id
						string strRoleID = expDR["ROLE_ID"].ToString();
						//get expression
						string strExp = expDR["EXPRESSION"].ToString();

						//get app_id, code_name of role
						strSql = "SELECT APP_ID, CODE_NAME FROM ROLES WHERE ID = {0}";
						strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true));
						DataTable DT = InnerCommon.ExecuteDataset(strSql).Tables[0];
						string strAppID = DT.Rows[0]["APP_ID"].ToString();
						string strRoleCodeName = DT.Rows[0]["CODE_NAME"].ToString();

						DataTable appTempDT;
						if (strDeleteSubApp == "y")
						{
							#region 根据inherited_state确定继承链
							Database database = DatabaseFactory.Create(context);
							DbCommand command = database.CreateStoredProcedureCommand("GET_INHERITED_APPLICATIONS");
							database.AddInParameter(command, "APP_ID", DbType.String, strAppID);
							database.AddInParameter(command, "INHERITED_STATE", DbType.Int32, (int)(InheritedState.ROLES | InheritedState.OBJECT));
							appTempDT = database.ExecuteDataSet(command).Tables[0];

							for (int i = 0; i < appTempDT.Rows.Count; i++)
							{
								string subAppID = appTempDT.Rows[i]["ID"].ToString();
								//get subRoleID
								string subRoleID;
								strSql = "SELECT ID FROM ROLES WHERE APP_ID = {0} AND CODE_NAME = {1} AND INHERITED = 'y'";
								strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(subAppID, true),
									TSqlBuilder.Instance.CheckQuotationMark(strRoleCodeName, true));

								object obj = InnerCommon.ExecuteScalar(strSql);
								if (obj != null)
								{
									subRoleID = obj.ToString();
									strSql = string.Format(@"DELETE FROM EXPRESSIONS WHERE ID IN
													(SELECT ID FROM EXPRESSIONS WHERE ROLE_ID = {0} AND EXPRESSION = {1} AND INHERITED = 'y')",
										TSqlBuilder.Instance.CheckQuotationMark(subRoleID, true),
										TSqlBuilder.Instance.CheckQuotationMark(strExp, true));
									InnerCommon.ExecuteNonQuery(strSql);
								}
							}
							#endregion
						}
						else
						{
							#region 根据inherited_state确定继承链中的第一级应用
							Database database = DatabaseFactory.Create(context);
							DbCommand command = database.CreateStoredProcedureCommand("GET_INHERITED_SON_APPLICATIONS");
							database.AddInParameter(command, "APP_ID", DbType.String, strAppID);
							database.AddInParameter(command, "INHERITED_STATE", DbType.Int32, (int)(InheritedState.ROLES | InheritedState.OBJECT));
							appTempDT = database.ExecuteDataSet(command).Tables[0];

							for (int i = 0; i < appTempDT.Rows.Count; i++)
							{
								string subAppID = appTempDT.Rows[i]["ID"].ToString();
								//get subRoleID
								string subRoleID = string.Empty;
								strSql = "SELECT ID FROM ROLES WHERE APP_ID = {0} AND CODE_NAME = {1} AND INHERITED = 'y'";
								strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(subAppID, true),
									TSqlBuilder.Instance.CheckQuotationMark(strRoleCodeName, true));

								object obj = InnerCommon.ExecuteScalar(strSql);
								if (obj != null)
								{
									subRoleID = obj.ToString();
									strSql = string.Format(@"UPDATE EXPRESSIONS SET INHERITED = 'n' WHERE ID IN
													(SELECT ID FROM EXPRESSIONS WHERE ROLE_ID = {0} AND EXPRESSION = {1} AND INHERITED = 'y')",
										TSqlBuilder.Instance.CheckQuotationMark(subRoleID, true), TSqlBuilder.Instance.CheckQuotationMark(strExp, true));
									InnerCommon.ExecuteNonQuery(strSql);
								}
							}
							#endregion
						}
					}
					#endregion
				}
				scope.Complete();
			}
			//写日志
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.DELETE_OBJECT_FUNC.ToString(),
				"删除角色中的被授权对象", xmlDocDelete.OuterXml);
			return xmlDocDelete;
		}

		private bool IsInUserScopes(ArrayList userScopeList, string expDsp, String appCodeName)
		{
			for (int i = 0; i < userScopeList.Count; i++)
			{
				if (expDsp.IndexOf(userScopeList[i].ToString()) >= 0)
					return true;
			}

			return false;
		}
		#endregion delete被授权对象

		#endregion 被授权对象维护

		#region 得到当前人员的机构服务范围

		/// <summary>
		/// 得到当前人员的机构服务范围
		/// </summary>
		/// <param name="appCodeName">应用标识</param>
		/// <param name="CodeNames">功能或角色英文标识</param>
		/// <param name="funcOrRole">功能或角色类别</param>
		/// <returns></returns>
		private ArrayList GetUserScopes(string appCodeName, string CodeNames, FunctionOrRole funcOrRole)
		{
			DataTable userScopeDT = null;
			if (funcOrRole == FunctionOrRole.Fuunction)
				userScopeDT = SecurityCheck.GetUserFunctionsScopes(LogOnUserInfo.UserLogOnName, appCodeName, CodeNames).Tables[0];
			else if (funcOrRole == FunctionOrRole.Role)
				userScopeDT = SecurityCheck.GetUserRolesScopes(LogOnUserInfo.UserLogOnName, appCodeName, CodeNames).Tables[0];

			ArrayList userScopeList = new ArrayList();
			for (int i = 0; i < userScopeDT.Rows.Count; i++)
			{
				string[] userScopes = userScopeDT.Rows[i]["DESCRIPTION"].ToString().Split(new char[] { ';', ',' });
				for (int j = 0; j < userScopes.Length; j++)
					if (userScopes[j].Length > 0) userScopeList.Add(userScopes[j]);
			}
			return userScopeList;
		}

		#endregion 得到当前人员的机构服务范围

		#region 删除无效的被授权对象
		/// <summary>
		/// 解析表达式，并将解析结果写入表变量中，作为其后表连接的依据
		/// </summary>
		/// <param name="expDT">被解析的表达式结果集</param>
		/// <returns>SQL语句:解析后的结果，并存入临时表</returns>
		private static string ParseExpsToTable_SqlStr(DataTable expDT)
		{
			//设定表达式变量，记录角色对应的表达式
			string strSql;
			strSql = @"DECLARE @EXP_TABLE 
						TABLE(
							[ROLE_ID] [nvarchar] (36) NOT NULL ,
							[CLASSIFY] [int] NULL ,
							[ID] [nvarchar] (36) NOT NULL ,
							[OBJ_GUID] [nvarchar] (36) NOT NULL ,
							[PARENT_GUID] [nvarchar] (36) NOT NULL
						); ";

			string objType;		//对象类型
			string objID;		//对象Guid
			string parentID;	//对象父对象Guid
			string rankCode;

			foreach (DataRow row in expDT.Rows)
			{
				DoExpParsing.getObjInfo(row["EXPRESSION"].ToString(), out objType, out objID, out parentID, out rankCode);

				if (objType == string.Empty)
				{
					continue;
				}

				//向表变量中插入记录
				strSql += string.Format("\nINSERT INTO @EXP_TABLE VALUES('{0}', '{1}', '{2}', '{3}', '{4}');\n",
					row["ROLE_ID"], row["CLASSIFY"], row["ID"], objID, parentID);

			}
			return strSql;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <example><checkAndDeleteUserInRole app_code_name="APP_ADMIN" app_id="11111111-1111-1111-1111-111111111111" classify="y"/></example>
		protected void CheckObjInRole(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				//string appCodeName	= xmlDoc.DocumentElement.GetAttribute("app_code_name");
				string appID = xmlDoc.DocumentElement.GetAttribute("app_id");
				string classify = xmlDoc.DocumentElement.GetAttribute("classify");

				if (appID == string.Empty)
					return;

				string strDisabledExpIDs = string.Empty;
				string strSql = string.Empty;

				//查找符合条件的被授权对象
				string strAnd = string.Empty;
				if (classify != string.Empty)
				{
					strAnd = string.Format(" AND CLASSIFY = {0}", TSqlBuilder.Instance.CheckQuotationMark(classify, true));
				}
				strSql = string.Format(@"SELECT ROLE_ID, ID, CLASSIFY, EXPRESSION FROM EXPRESSIONS
												WHERE ROLE_ID IN(
												SELECT ID FROM ROLES 
												WHERE APP_ID = {0}
												{1}
												)", TSqlBuilder.Instance.CheckQuotationMark(appID, true), strAnd);

				DataTable expDT = InnerCommon.ExecuteDataset(strSql).Tables[0];

				if (expDT.Rows.Count == 0)
					return;

				strSql = ParseExpsToTable_SqlStr(expDT);

				//删除表变量中的有效的被授权对象，留下的就是无效的被授权对象,然后在表达式表中删除
				strSql += @"
							DELETE @EXP_TABLE 
							FROM @EXP_TABLE E
							INNER JOIN OU_USERS OU ON E.OBJ_GUID = OU.USER_GUID AND E.PARENT_GUID = OU.PARENT_GUID
							WHERE E.CLASSIFY = 0
							AND OU.STATUS = 1;

							DELETE @EXP_TABLE
							FROM @EXP_TABLE E
							INNER JOIN ORGANIZATIONS O ON E.OBJ_GUID = O.GUID AND E.PARENT_GUID = O.PARENT_GUID
							WHERE E.CLASSIFY = 1
							AND O.STATUS =1;

							DELETE @EXP_TABLE
							FROM @EXP_TABLE E
							INNER JOIN GROUPS G ON E.OBJ_GUID = G.GUID AND E.PARENT_GUID = G.PARENT_GUID
							WHERE E.CLASSIFY = 2
							AND G.STATUS = 1

							SELECT COUNT(*) AS DEL_EXP_COUNT FROM @EXP_TABLE;

							DELETE EXPRESSIONS
							FROM EXPRESSIONS EP
							INNER JOIN @EXP_TABLE ET ON EP.ID = ET.ID
							";
				DataSet ds = InnerCommon.ExecuteDataset(strSql);

				_XmlResult = InnerCommon.GetXmlDoc(ds);
				scope.Complete();
			}
			//写日志
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.DELETE_OBJECT_FUNC.ToString(),
				"删除角色中无效的被授权对象", xmlDoc.OuterXml);
		}
		#endregion

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
