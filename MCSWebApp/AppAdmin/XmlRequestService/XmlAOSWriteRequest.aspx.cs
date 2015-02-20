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
	/// XmlAOSWriteRequest ��ժҪ˵����
	/// </summary>
	public partial class XmlAOSWriteRequest : XmlRequestUserWebClass
	{		//���ܻ��ɫ��ʶ
		private enum FunctionOrRole
		{
			//����
			Fuunction,
			//��ɫ
			Role
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// �ڴ˴������û������Գ�ʼ��ҳ��
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

		#region Ӧ��ά��

		#region insertӦ��
		/// <summary>
		/// ����Ӧ��
		/// </summary>
		/// <Insert>
		///		<APPLICATIONS parentLevel="" parentID="">
		///			<SET>
		///				<NAME>newͨ����Ȩ</NAME>
		///				<CODE_NAME>new_APP_ADMIN_APPLICATION</CODE_NAME>
		///				<DESCRIPTION>newͨ����Ȩ����ƽ̨</DESCRIPTION>
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

					#region ���Ȩ��
					//****************
					//���Ȩ��
					//**************
					//Ӧ�����ӵ�Ȩ���ڸ�Ӧ����
					bool hasPermi = false;
					if (strParentID != string.Empty)
						hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, new Guid(strParentID), "ADD_APPLICATION_FUNC");

					#region �ܹ���Ա���Լӵ�һ��Ӧ��
					if (false == hasPermi)
					{
						if (SecurityCheck.IsAdminUser(LogOnUserInfo.UserLogOnName) && strParentID == string.Empty)
							hasPermi = true;
					}

					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, "û����ӦȨ��!");
					#endregion
					#endregion
					//����Ӧ��
					XmlNode nodeSet = xmlDocInsert.DocumentElement.SelectSingleNode(".//SET");
					string strAppID = Guid.NewGuid().ToString();
					XmlHelper.AppendNode<string>(nodeSet, "ID", strAppID);

					#region ������Ӧ���Լ��������Ȩ��ɫ�����Լ���ϵ
					XmlDocument xmlDoc;
					xmlDoc = ApplicationDataToDB(xmlDocInsert, strParentLevel, strAppID);

					//��������Ȩ��ɫ���ܡ����ܼ��ϡ���Ӧ��ϵ
					string strExpID;//�¼ӵĵ�ǰ��Ա����Ȩ�����ID
					strExpID = AutoInsertApplicationRTF(strAppID);

					Database database = DatabaseFactory.Create(context);
					DbCommand command = database.CreateStoredProcedureCommand("COPY_APPLICATION_DATA");
					database.AddInParameter(command, "PARENT_APP_ID", DbType.String, strParentID);
					database.AddInParameter(command, "CURRENT_APP_ID", DbType.String, strAppID);
					database.ExecuteNonQuery(command);
					//����Ӧ�õļ̳�����

					//����ǰ��Ա�������ķ���Χ
					AddAdminUserScope(strAppID, strExpID);
					#endregion
					_XmlResult = xmlDoc;
				}
				scope.Complete();
			}
			//д��־
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.ADD_APPLICATION_FUNC.ToString(),
				"����Ӧ��ϵͳ", xmlDocInsert.OuterXml);
		}

		/// <summary>
		/// �µ�Ӧ�����
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
		/// �Զ���Ϊ�����Ӧ�ý�������Ȩ�еĽ�ɫ�������Լ���ɫ�͹���֮��Ķ�Ӧ��ϵ
		/// </summary>
		/// <param name="dbc"></param>
		/// <param name="strUserDN"></param>
		/// <param name="strUserLogOn"></param>
		/// <param name="strAppID"></param>
		/// <returns></returns>
		/// <remarks>��������Ȩ��Ϣ</remarks>
		private string AutoInsertApplicationRTF(string strAppID)
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				//�������Ȩ��Ϣ��
				XmlDocument xmlTemplate = this.GetXMLDocument("../XML", "preDefineObj");
				//this.GetXMLDocument����Cache�м����ļ�������¼�ı䣬����Ҫʹ����ʱ�����ļ�
				XmlElement root = (XmlElement)xmlTemplate.DocumentElement.CloneNode(true);
				string strAdminRoleID = string.Empty;
				StringBuilder sqlBuilder = new StringBuilder(1024);

				#region Roles�������
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

				#region functions���
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

				#region RTF�������
				XmlNode rtfData = root.SelectSingleNode("RTF_DATA");
				xsdDoc = this.GetXSDDocument("ROLE_TO_FUNCTIONS");
				foreach (XmlNode rtfNode in rtfData.ChildNodes)
				{
					XmlNode nodeSet = rtfNode.FirstChild;
					XmlHelper.AppendNode(nodeSet, "INHERITED", "n");
					sqlBuilder.Append(InnerCommon.GetOneInsertSqlStr(rtfNode, xsdDoc) + ";" + Environment.NewLine);
				}
				#endregion

				#region function_set���
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

				#region FSTF�������
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

				#region �ѵ�ǰ��Ա��������Ȩ����Ա��ɫ��
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
				//get ���Ļ�����Χȫ·��
				DataTable orgRootDT = OGUReader.GetObjectDepOrgs("USERS", LogOnUserInfo.UserLogOnName,
					SearchObjectColumn.SEARCH_LOGON_NAME, 1, string.Empty).Tables[0];
				string strAllPath = orgRootDT.Rows[0]["ALL_PATH_NAME"].ToString();

				//get scope's id
				string strSql = "SELECT * FROM SCOPES WHERE APP_ID = {0} AND DESCRIPTION = {1}";
				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(appID, true), TSqlBuilder.Instance.CheckQuotationMark(strAllPath, true));

				DataTable scopeDT = InnerCommon.ExecuteDataset(strSql).Tables[0];

				string strScopeID = string.Empty;//��Χid


				if (scopeDT.Rows.Count == 0)
				{
					string strID = Guid.NewGuid().ToString();
					string strCodeName = Guid.NewGuid().ToString().Replace("-", "");
					string strExp = string.Format("userDefineScope(\"{0}\")", orgRootDT.Rows[0]["GUID"]);
					string strName = orgRootDT.Rows[0]["DISPLAY_NAME"].ToString();

					//insert��������Χ
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

		#region deleteӦ��
		/// <summary>
		/// ɾ��Ӧ��
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
					//���Ȩ��
					//**************
					//Ӧ�õ�ɾ��Ȩ���ڱ���Ӧ����
					bool hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, new Guid(appID), "DELETE_APPLICATION_FUNC");
					string appName = string.Empty;
					if (false == hasPermi)
					{
						appName = InnerCommon.ExecuteScalar(string.Format("SELECT NAME FROM APPLICATIONS WHERE ID = {0}",
							TSqlBuilder.Instance.CheckQuotationMark(appID, true))).ToString();
					}

					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, string.Format("û��ɾ��Ӧ��({0})����ӦȨ��!", appName));
					string strTemp = "DELETE APPLICATIONS WHERE ID = {0} AND CHILDREN_COUNT = 0;";
					sqlBuilder.Append(string.Format(strTemp, TSqlBuilder.Instance.CheckQuotationMark(appID, true)) + Environment.NewLine);
				}

				if (sqlBuilder.Length > 0)
					InnerCommon.ExecuteNonQuery(sqlBuilder.ToString());

				scope.Complete();
			}
			//д��־
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.DELETE_APPLICATION_FUNC.ToString(),
				"ɾ��Ӧ��ϵͳ", xmlDocDelete.OuterXml);
			return xmlDocDelete;
		}
		#endregion deleteӦ��

		#region updateӦ��

		/// <summary>
		/// updateӦ��
		/// </summary>
		/// <param name="xmlDocUpdate"></param>
		/// <returns></returns>
		/// 
		///<Update>
		///	<APPLICATIONS>
		///		<SET>
		///			<INHERITED_STATE>15</INHERITED_STATE>
		///			<NAME>�޸ĺ������</NAME>
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
					//���Ȩ��
					//**************
					string appID = xmlDocUpdate.SelectSingleNode(".//ID").InnerText;
					bool hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, new Guid(appID), "MODIFY_APPLICATION_FUNC");
					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, "û����ӦȨ��!");
					string strSQL = string.Empty;
					string strAppID = xmlDocUpdate.SelectSingleNode(".//ID").InnerText;

					#region ʹ�÷���Χ�ж�
					XmlNode scopeNode = xmlDocUpdate.SelectSingleNode(".//USE_SCOPE");
					if (scopeNode != null && scopeNode.InnerText == "n")
					{
						strSQL = string.Format("SELECT COUNT(*) FROM EXP_TO_SCOPES WHERE SCOPE_ID IN (SELECT ID FROM SCOPES WHERE APP_ID = {0})",
							TSqlBuilder.Instance.CheckQuotationMark(strAppID, true));
						int count = (int)InnerCommon.ExecuteScalar(strSQL);
						ExceptionHelper.TrueThrow<ApplicationException>(count > 0, "��Ӧ���Ѿ�ʹ���˷���Χ�� ���ܸ���ʹ�÷���Χ��־��");
					}
					#endregion

					strSQL = string.Format("SELECT INHERITED_STATE FROM APPLICATIONS WHERE ID = {0}",
						TSqlBuilder.Instance.CheckQuotationMark(strAppID, true));
					//����Ӧ��,���õ�inherited_state
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
						//�޸�Ӧ�����������ݵļ̳�״̬
						database.ExecuteNonQuery(command);
					}
				}
				scope.Complete();
			}
			//д��־
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.MODIFY_APPLICATION_FUNC.ToString(),
				"�޸�Ӧ��ϵͳ������", xmlDocUpdate.OuterXml);
			return xmlDocUpdate;
		}
		#endregion updateӦ��

		#endregion Ӧ��ά��

		#region ����Χά��

		#region �������Χ
		/***************************************************************
		//�ĵ���ʽ
			<Insert>
				<SCOPES>
					<SET>
						<EXPRESSION>curDepartScope(curUserId)</EXPRESSION>
						<APP_ID>ff6332da-4654-45cf-9247-97b5b60998ab</APP_ID>
						<NAME>������</NAME>
						<DESCRIPTION>������</DESCRIPTION>
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
					//���Ȩ��
					//**************
					string appID = xmlDocInsert.SelectSingleNode(".//APP_ID").InnerText;
					bool hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, new Guid(appID), "ADD_SCOPE_FUNC");
					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, "û����ӦȨ��!");

					XmlNode nodeSet = xmlDocInsert.DocumentElement.SelectSingleNode(".//SET");
					//����modify_time���
					string strTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", strTime);

					//ȥ��app_id���
					XmlNode appIDNode = xmlDocInsert.DocumentElement.SelectSingleNode(".//APP_ID");
					string strAppID = appIDNode.InnerText;
					xmlDocInsert.FirstChild.FirstChild.FirstChild.RemoveChild(appIDNode);

					//ȥ��inherited���
					XmlNode inheritedNode = xmlDocInsert.DocumentElement.SelectSingleNode(".//INHERITED");
					string strInherited = inheritedNode.InnerText;
					xmlDocInsert.FirstChild.FirstChild.FirstChild.RemoveChild(inheritedNode);

					//����code_name���
					string strCodeName = Guid.NewGuid().ToString().Replace("-", "");
					XmlHelper.AppendNode(xmlDocInsert.SelectSingleNode(".//SET"), "CODE_NAME", strCodeName);

					//��Ӧ������
					InsertScopeOfApplication(xmlDocInsert, strAppID, strInherited);

					//��Ӧ����
					//����inherited_stateȷ���̳���
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

			//д��־
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.ADD_SCOPE_FUNC.ToString(), "���ӷ���Χ", xmlDocInsert.OuterXml);
		}

		private void InsertScopeOfApplication(XmlDocument xmlDoc, string strAppID, string strInherited)
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
				string strExp = xmlDoc.DocumentElement.SelectSingleNode(".//EXPRESSION").InnerText;
				string strClass = xmlDoc.DocumentElement.SelectSingleNode(".//CLASSIFY").InnerText;

				//�ж��Ƿ��Ѵ��ڷ���Χ
				string strSql = @"SELECT COUNT(*) FROM SCOPES WHERE APP_ID = {0} AND CLASSIFY = {1} AND EXPRESSION = {2}";
				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strAppID, true),
					TSqlBuilder.Instance.CheckQuotationMark(strClass, true), TSqlBuilder.Instance.CheckQuotationMark(strExp, true));

				int rowCount = int.Parse(InnerCommon.ExecuteScalar(strSql).ToString());

				//�������˳�
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
		#endregion �������Χ

		#region ɾ������Χ
		/// <summary>
		/// ɾ������Χ
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
					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, "û����ӦȨ��!");

					string strSureDelete = xmlDocDelete.DocumentElement.GetAttribute("sureDelete");
					if (strSureDelete == string.Empty)
						strSureDelete = AppResource.DefaultDelObj;

					string strDeleteSubApp = xmlDocDelete.DocumentElement.GetAttribute("deleteSubApp");
					if (strDeleteSubApp == string.Empty)
						strDeleteSubApp = AppResource.DefaultDelSubApp;

					//��ͬʱɾ���������Χ
					foreach (XmlNode socpeNode in xmlDocDelete.FirstChild.ChildNodes)
					{
						string strScopeID = socpeNode.SelectSingleNode(".//ID").InnerText;

						//ɾ��������Χ
						string strSql = string.Format(@"SELECT * FROM SCOPES WHERE ID = {0};
											SELECT COUNT(*) FROM EXP_TO_SCOPES WHERE SCOPE_ID = {0};
													DELETE FROM SCOPES WHERE ID = {0}",
							TSqlBuilder.Instance.CheckQuotationMark(strScopeID, true));
						DataSet scopeDS = InnerCommon.ExecuteDataset(strSql);
						DataRow scopeDR = scopeDS.Tables[0].Rows[0];
						int iCount = int.Parse(scopeDS.Tables[1].Rows[0][0].ToString());

						ExceptionHelper.TrueThrow<ApplicationException>(iCount > 0,
							string.Format("����Χ({0})�ѱ�ʹ�ã����ܱ�ɾ��", scopeDR["NAME"].ToString()));

						string strInherite = scopeDR["INHERITED"].ToString();
						ExceptionHelper.TrueThrow<ApplicationException>((strSureDelete == "n") && (strInherite == "y"),
							string.Format("�̳еķ���Χ({0})�� ���ܱ�ɾ����", scopeDR["NAME"].ToString()));

						string strAppID = scopeDR["APP_ID"].ToString();
						string strCodeName = scopeDR["CODE_NAME"].ToString();

						DataTable appTempDT;
						if (strDeleteSubApp == "y")
						{
							#region  ����̳���
							//����inherited_stateȷ���̳���
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
							#region ����̳���
							//����inherited_stateȷ���̳����еĵ�һ��Ӧ��
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
			//д��־
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.DELETE_SCOPE_FUNC.ToString(),
				"ɾ������Χ", xmlDocDelete.OuterXml);
			return xmlDocDelete;
		}

		#endregion ɾ������Χ

		#region ά�����ʽ-����Χ��Ӧ��ϵ
		/// <summary>
		/// ά�����ʽ-����Χ��Ӧ��ϵ
		/// </summary>
		/// <param name="xmlDocDelete"></param>
		/// <returns></returns>
		/// <remarks>
		/// 
		/// </remarks>
		/// <example>
		///	xmlDocModify�ĵ���ʽ
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
					"û����ӦȨ��!");

				ParseExpression pe = new ParseExpression();
				pe.UserFunctions = (IExpParsing)new DoExpParsing();

				//�õ���ǰ�û��ķ���Χ
				ArrayList userScopeList = GetUserScopes(appCodeName, "MODIFY_SCOPE_FUNC", FunctionOrRole.Fuunction);

				XmlDocument curNodeDoc = new XmlDocument();
				StringBuilder sqlBuilder = new StringBuilder(128);
				foreach (XmlNode curNode in xmlDocModify.FirstChild.ChildNodes)
				{
					string scopeName;
					bool bValid = isValidScope(curNode, userScopeList, pe, out scopeName);

					ExceptionHelper.TrueThrow<ApplicationException>(false == bValid,
						string.Format("��Χ��{0}�����ڵ�ǰ��Ա�Ļ�������Χ�ڣ� �����޸Ķ�Ӧ��ϵ��", scopeName));

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
			//д��־
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.MODIFY_SCOPE_FUNC.ToString(),
				"�޸ķ���Χ-����Ȩ����֮��Ĺ�ϵ", xmlDocModify.OuterXml);

			return xmlDocModify;
		}
		/// <summary>
		/// �ж��ǿ��޸ĵĺϷ���������Χ
		/// </summary>
		/// <param name="trans"></param>
		/// <param name="scopeNode">Ҫ�޸ĵķ���Χ-����Ȩ�����ϵ��XML���</param>
		/// <param name="userScopes">��ǰ��Ա�Ļ�����Χ</param>
		/// <param name="pe">���ʽ��������</param>
		/// <param name="scopeName">��ǰ����Χ����(�������)</param>
		/// <returns></returns>
		private bool isValidScope(XmlNode scopeNode, ArrayList userScopes, ParseExpression pe, out string scopeName)
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				//�鵱ǰ��������Χ����Ϣ
				scopeName = string.Empty;
				string scopeID = scopeNode.SelectSingleNode(".//SCOPE_ID").InnerText;
				string expID = scopeNode.SelectSingleNode(".//EXP_ID").InnerText;

				//��ѯ����Ȩ����������Ϣ
				string strExp = string.Empty;//����Ȩ������ʽ
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

				//��ѯ��������Χ��ȫ·��
				strSql = string.Format("SELECT * FROM SCOPES WHERE ID = {0}",
					TSqlBuilder.Instance.CheckQuotationMark(scopeID, true));
				DataTable scopeDT = InnerCommon.ExecuteDataset(strSql).Tables[0];

				string scopeExp = string.Empty;
				ArrayList scopePaths = new ArrayList();
				if (scopeDT.Rows.Count > 0)
				{
					//��������ݷ���Χ�������ж�
					if (scopeDT.Rows[0]["CLASSIFY"].ToString() == "n")
						return true;

					scopePaths.Add(scopeDT.Rows[0]["DESCRIPTION"].ToString());
					scopeExp = scopeDT.Rows[0]["EXPRESSION"].ToString();
					scopeName = scopeDT.Rows[0]["NAME"].ToString();

					#region ���ݵ�ǰ�ı���Ȩ���󣬵õ��������򱾲��ŵ�ȫ·��
					if (scopeExp == "curDepartScope(curUserId)" || scopeExp == "curCustomsScope(curUserId)")
					{
						DataTable dt = null;
						//������
						if (scopeExp == "curDepartScope(curUserId)")
						{
							dt = OGUReader.GetObjectParentOrgs(objType, objID, SearchObjectColumn.SEARCH_GUID, true, false, string.Empty, string.Empty).Tables[0];
						}
						//������
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

				#region �жϺϷ���
				for (int i = 0; i < scopePaths.Count; i++)
				{
					for (int j = 0; j < userScopes.Count; j++)
					{
						if (scopePaths[i].ToString().IndexOf(userScopes[j].ToString()) >= 0)
							return true;
					}
				}
				#endregion

				#region ������ܹ���Ա��ͨ��
				DataTable detailDT = OGUReader.GetObjectsDetail(objID).Tables[0];
				if (detailDT.Rows.Count > 0 && detailDT.Rows[0]["OBJECTCLASS"].ToString() == "USERS")
				{
					return SecurityCheck.IsAdminUser(detailDT.Rows[0]["LOGON_NAME"].ToString(), UserValueType.LogonName);
				}
				#endregion
				return false;
			}
		}
		#endregion ά�����ʽ-����Χ��Ӧ��ϵ

		#endregion ����Χά��

		#region ����Ȩ����ά��

		#region ���뱻��Ȩ����
		/// <summary>
		/// ���뱻��Ȩ����
		/// </summary>
		///	<Insert app_code_name="ASDF" use_scope="y" role_classify="y">
		///		<EXPRESSIONS ALL_PATH_NAME=\"�й�����\01��������\00���쵼\���ѫ" OBJ_ID="5e3aa542-29c3-40b5-b4cc-617045223c22">
		///			<SET>
		///				<EXPRESSION>BelongTo(USERS, "5e3aa542-29c3-40b5-b4cc-617045223c22", "65eb8160-f0fa-4f1c-8984-2649788fe1d0")</EXPRESSION>
		///				<ROLE_ID>ec16e6b8-5a94-4b9c-963c-08ace45dffd7</ROLE_ID>
		///				<NAME>���ѫ</NAME>
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

					#region �ж�Ȩ��
					bool hasPermi = false;
					if (roleClassify == "y")//����Ȩ��ɫ
						hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, appCodeName, "SELF_MAINTAIN_FUNC");
					else if (roleClassify == "n")//Ӧ����Ȩ��ɫ
						hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, appCodeName, "ADD_OBJECT_FUNC");

					bool defaultPermi = false;//Ĭ��Ȩ�� 
					if (false == hasPermi)
					{
						bool adminUser = SecurityCheck.IsAdminUser(LogOnUserInfo.UserLogOnName);
						if (adminUser)
						{
							//�ܹ���Ա����ͨ����Ȩ����ϵͳ�У����ӱ���Ȩ��������Ȩ��
							if (appCodeName == AppResource.AppCodeName)
								hasPermi = true;
							else//������ϵͳ�У�ֻ�ܼ��Լ�
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
					ExceptionHelper.TrueThrow<ApplicationException>(false == hasPermi && !defaultPermi, "û����Ӧ��Ȩ��!");
					#endregion

					ParseExpression pe = new ParseExpression();
					pe.UserFunctions = (IExpParsing)(new DoExpParsing());
					//��ͬʱ����������Ȩ����
					DataTable appTempDT = null;
					string strAppID = string.Empty;
					string strRoleCodeName = string.Empty;
					for (int i = 0; i < xmlDocInsert.FirstChild.ChildNodes.Count; i++)
					{
						XmlNode nodeSet = xmlDocInsert.FirstChild.ChildNodes[i].SelectSingleNode(".//SET");
						string strExp = nodeSet.SelectSingleNode(".//EXPRESSION").InnerText;
						string strClassify = nodeSet.SelectSingleNode(".//CLASSIFY").InnerText;

						//��Ĭ��Ȩ���£�ֻ�ܼ��Լ�
						if (false == hasPermi && defaultPermi)
							if (strExp.IndexOf(LogOnUserInfo.UserGuid) < 0)
								continue;
						#region ����Sql
						//get description
						string strDescription = xmlDocInsert.FirstChild.ChildNodes[i].Attributes["ALL_PATH_NAME"].Value;
						//����description���
						XmlHelper.AppendNode(nodeSet, "DESCRIPTION", strDescription);
						//����modify_time���
						string strModifyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", strModifyTime);
						//ȥ��role_id���
						XmlNode roleIDNode = nodeSet.SelectSingleNode(".//ROLE_ID");
						string strRoleID = roleIDNode.InnerText;
						nodeSet.RemoveChild(roleIDNode);
						//ȥ��inherited���
						XmlNode inheritedNode = nodeSet.SelectSingleNode(".//INHERITED");
						string strInherited = inheritedNode.InnerText;
						nodeSet.RemoveChild(inheritedNode);
						//�õ����е��Ѵ��ڵı���Ȩ�����GUID
						string strSql = string.Empty;
						//0.������ʽ�Ѵ��ڣ�������
						strSql = string.Format("SELECT COUNT(*) FROM EXPRESSIONS WHERE ROLE_ID = {0} AND CLASSIFY = {1} AND EXPRESSION = {2}",
							TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true),
							TSqlBuilder.Instance.CheckQuotationMark(strClassify, true),
							TSqlBuilder.Instance.CheckQuotationMark(strExp, true));
						#endregion
						int iCount = (int)InnerCommon.ExecuteScalar(strSql);
						if (iCount != 0)
							continue;

						//1.��Ӧ������
						InsertExpOfApplication(nodeSet, strRoleID, strInherited);

						//2.��Ӧ����
						//����Ȩ�ı���Ȩ���󲻼̳�
						if (roleClassify == "n")
						{
							#region ����Ȩ�ı���Ȩ���󲻼̳�
							if (strRoleID != strPreRoleID || appTempDT == null)
							{
								//get app_id, code_name of role
								strSql = "SELECT APP_ID, CODE_NAME FROM ROLES WHERE ID = {0}";
								strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true));
								DataTable DT = InnerCommon.ExecuteDataset(strSql).Tables[0];
								strAppID = DT.Rows[0]["APP_ID"].ToString();
								strRoleCodeName = DT.Rows[0]["CODE_NAME"].ToString();

								//����inherited_stateȷ���̳���
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
			//д��־
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.ADD_OBJECT_FUNC.ToString(),
				"����ɫ���ӱ���Ȩ����", xmlDocInsert.OuterXml);
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

			//���ӱ���Ȩ����ķ���Χ

			//Debug.WriteLine(strSql);
			InnerCommon.ExecuteNonQuery(strSql);
		}

		#endregion ���뱻��Ȩ����


		#region delete����Ȩ����
		/// <summary>
		/// ɾ������Ȩ����
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
					#region ���Ȩ��
					bool hasPermi = false;
					if (roleClassify == "y")//����Ȩ��ɫ
						hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, appCodeName, "SELF_MAINTAIN_FUNC");
					else if (roleClassify == "n")//Ӧ����Ȩ��ɫ
						hasPermi = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName, appCodeName, "DELETE_OBJECT_FUNC");

					if (false == hasPermi)
					{
						bool adminUser = SecurityCheck.IsAdminUser(LogOnUserInfo.UserLogOnName);
						if (adminUser)
						{
							//�ܹ���Ա����ͨ����Ȩ����ϵͳ�У�ɾ������Ȩ��������Ȩ��
							if (appCodeName == AppResource.AppCodeName)
								hasPermi = true;
						}
					}

					ExceptionHelper.FalseThrow<ApplicationException>(hasPermi, "û����ӦȨ��!");
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

					//�õ���ǰ��Ա�Ļ�������Χ
					string useScope = xmlDocDelete.DocumentElement.GetAttribute("use_scope");

					ArrayList userScopeList = null;
					if (useScope == "y")
					{
						userScopeList = GetUserScopes(appCodeName, "DELETE_OBJECT_FUNC", FunctionOrRole.Fuunction);
					}
					#endregion

					#region ��ͬʱɾ���������Ȩ����
					foreach (XmlNode expNode in xmlDocDelete.FirstChild.ChildNodes)
					{
						string strExpID = expNode.SelectSingleNode(".//ID").InnerText;

						//ɾ��������Ȩ����
						string strSql = string.Format(@"SELECT * FROM EXPRESSIONS WHERE ID = {0};
													DELETE FROM EXPRESSIONS WHERE ID = {0}",
							TSqlBuilder.Instance.CheckQuotationMark(strExpID, true));

						DataRow expDR = InnerCommon.ExecuteDataset(strSql).Tables[0].Rows[0];

						//�жϱ���Ȩ�����Ƿ��ڵ�ǰ��Ա�Ļ�������Χ��
						bool bTemp = true;
						if (useScope == "y")
						{
							bTemp = IsInUserScopes(userScopeList, expDR["DESCRIPTION"].ToString(), appCodeName);
						}
						ExceptionHelper.TrueThrow<ApplicationException>(false == bTemp,
							string.Format("����Ȩ����{0}�����ڵ�ǰ�û�����Ļ�����Χ�У�����ɾ��!", expDR["NAME"].ToString()));

						//get inherite
						string strInherite = expDR["INHERITED"].ToString();

						ExceptionHelper.TrueThrow<ApplicationException>((strSureDelete == "n") && (strInherite == "y"),
							string.Format("�̳еı���Ȩ����({0})�� ���ܱ�ɾ����", expDR["NAME"].ToString()));

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
							#region ����inherited_stateȷ���̳���
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
							#region ����inherited_stateȷ���̳����еĵ�һ��Ӧ��
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
			//д��־
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.DELETE_OBJECT_FUNC.ToString(),
				"ɾ����ɫ�еı���Ȩ����", xmlDocDelete.OuterXml);
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
		#endregion delete����Ȩ����

		#endregion ����Ȩ����ά��

		#region �õ���ǰ��Ա�Ļ�������Χ

		/// <summary>
		/// �õ���ǰ��Ա�Ļ�������Χ
		/// </summary>
		/// <param name="appCodeName">Ӧ�ñ�ʶ</param>
		/// <param name="CodeNames">���ܻ��ɫӢ�ı�ʶ</param>
		/// <param name="funcOrRole">���ܻ��ɫ���</param>
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

		#endregion �õ���ǰ��Ա�Ļ�������Χ

		#region ɾ����Ч�ı���Ȩ����
		/// <summary>
		/// �������ʽ�������������д�������У���Ϊ�������ӵ�����
		/// </summary>
		/// <param name="expDT">�������ı��ʽ�����</param>
		/// <returns>SQL���:������Ľ������������ʱ��</returns>
		private static string ParseExpsToTable_SqlStr(DataTable expDT)
		{
			//�趨���ʽ��������¼��ɫ��Ӧ�ı��ʽ
			string strSql;
			strSql = @"DECLARE @EXP_TABLE 
						TABLE(
							[ROLE_ID] [nvarchar] (36) NOT NULL ,
							[CLASSIFY] [int] NULL ,
							[ID] [nvarchar] (36) NOT NULL ,
							[OBJ_GUID] [nvarchar] (36) NOT NULL ,
							[PARENT_GUID] [nvarchar] (36) NOT NULL
						); ";

			string objType;		//��������
			string objID;		//����Guid
			string parentID;	//���󸸶���Guid
			string rankCode;

			foreach (DataRow row in expDT.Rows)
			{
				DoExpParsing.getObjInfo(row["EXPRESSION"].ToString(), out objType, out objID, out parentID, out rankCode);

				if (objType == string.Empty)
				{
					continue;
				}

				//�������в����¼
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

				//���ҷ��������ı���Ȩ����
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

				//ɾ��������е���Ч�ı���Ȩ�������µľ�����Ч�ı���Ȩ����,Ȼ���ڱ��ʽ����ɾ��
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
			//д��־
			UserDataWrite.InsertUserLog(AppResource.AppCodeName, LogOpType.DELETE_OBJECT_FUNC.ToString(),
				"ɾ����ɫ����Ч�ı���Ȩ����", xmlDoc.OuterXml);
		}
		#endregion

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
