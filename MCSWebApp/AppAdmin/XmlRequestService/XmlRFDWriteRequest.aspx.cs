using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text;
using System.Transactions;
using System.Diagnostics;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.LogAdmin;
using MCS.Library.Accredit.AppAdmin;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Core;
using MCS.Applications.AppAdmin.Common;
using MCS.Applications.AppAdmin.Properties;
using MCS.Library.Data.Builder;
using MCS.Library.Data;

namespace MCS.Applications.AppAdmin
{
	/// <summary>
	/// XmlWriteRequest ��ժҪ˵����
	/// </summary>
	public partial class XmlRFDWriteRequest : XmlRequestUserWebClass
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				XmlElement root = _XmlRequest.DocumentElement;
				string tableName = root.FirstChild == null ? string.Empty : root.FirstChild.Name;

				switch (RootName)
				{
					case "Insert":
						switch (tableName)
						{
							case "ROLES": InsertRole(_XmlRequest);
								SecurityCheck.RemoveAllCache();
								break;
							case "FUNCTIONS": InsertFunction(_XmlRequest);
								SecurityCheck.RemoveAllCache();
								break;
							case "FUNCTION_SETS": InsertFunctionSet(_XmlRequest);
								SecurityCheck.RemoveAllCache();
								break;
						}
						break;
					case "Update":
						switch (tableName)
						{
							case "ROLES": UpdateRole(_XmlRequest);
								SecurityCheck.RemoveAllCache();
								break;
							case "FUNCTIONS": UpdateFunction(_XmlRequest);
								SecurityCheck.RemoveAllCache();
								break;
							case "FUNCTION_SETS": UpdateFunctionSet(_XmlRequest);
								SecurityCheck.RemoveAllCache();
								break;
						}
						break;
					case "Delete":
						switch (tableName)
						{
							case "ROLES": DeleteRole(_XmlRequest);
								SecurityCheck.RemoveAllCache();
								break;
							case "FUNCTIONS": DeleteFunction(_XmlRequest);
								SecurityCheck.RemoveAllCache();
								break;
							case "FUNCTION_SETS": DeleteFunctionSet(_XmlRequest);
								SecurityCheck.RemoveAllCache();
								break;

						}
						break;
					case "RTF": DoRTF(_XmlRequest);
						SecurityCheck.RemoveAllCache();
						break;
					case "RTFS": DoRTFS(_XmlRequest);
						SecurityCheck.RemoveAllCache();
						break;
					case "FSTF": DoFSTF(_XmlRequest);
						SecurityCheck.RemoveAllCache();
						break;
					case "changeDelegateID": DoChangeDelegateID(_XmlRequest);
						SecurityCheck.RemoveAllCache();
						break;
					default: break;
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				string strMessage = ex.Message;

				switch (ex.Number)
				{
					case 2627:
						if (ex.Message.IndexOf("CODE_NAME") >= 0)
							strMessage = "Ӣ�ı�ʶ�ظ����ڽ�ɫ�͹����У�ÿһ��Ӧ�ó����ڵ�Ӣ�ı�ʶ����Ψһ������Ӧ�ó����У�Ӣ�ı�ʶ����ȫ��Ψһ";
						if (ex.Message.IndexOf("USERS_TO_ROLES") >= 0)
							strMessage = "��Ȩ�ظ�����ͬ��ɫ�в���������ͬ����Ȩ����";
						break;
					case 2601:
						strMessage = "�ؼ��ֳ�ͻ����Ҫ��ӵ����������ݿ����Ѿ�����";
						break;
				}

				//ExceptionManager.Publish(ex);
				throw new Exception(strMessage, ex);
			}
			catch (System.Exception ex)
			{
				//ExceptionManager.Publish(ex);
				throw ex;
			}
		}

		#region ��ӽ�ɫ�����ܣ����ܼ���
		/// <summary>
		/// �����ɫ,һ�����һ����ɫ
		/// �������ݵĸ�ʽ:
		/// <Insert>
		///		<ROLES>
		///			<SET>
		///				<APP_ID>..</APP_ID>
		///				<NAME></NAME>
		///				<CODE_NAME></CODE_NAME>
		///				<DESCRIPTION></DESCRIPTION>
		///				<CLASSIFY>..</CLASSIFY>
		///				<ALLOW_DELEGATE></ALLOW_DELEGATE>
		///				<INHERITED>�ڵ㲻����ʱĬ�ϲ��̳�</INHERITED>
		///			</SET>
		///		</ROLES>
		///	</Insert>
		/// </summary>
		protected void InsertRole(XmlDocument xmlDoc)
		{
			XmlElement root = xmlDoc.DocumentElement;

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				int nInheritedState = (int)InheritedState.ROLES; //2;
				XmlNode nodeSet = root.SelectSingleNode(".//SET");

				string strAppID = nodeSet.SelectSingleNode("APP_ID").InnerText;

				//���Ȩ��
				CheckPermission(strAppID, FunctionNames.ADD_ROLE_FUNC, "��ӽ�ɫ");

				AppendInsertNodes(xmlDoc, nodeSet);

				string strSortID = string.Empty;
				string strAppResLevel = string.Empty;
				this.GetSortIDAndResLevel("ROLES", strAppID, out strSortID, out strAppResLevel);
				XmlHelper.AppendNode(nodeSet, "SORT_ID", strSortID);

				InnerCommon.ExecuteNonQuery(InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("ROLES")));
				//��ѯ�м̳е���Ӧ�õ�guid
				DataSet dsSubApp = this.GetSubAppID(strAppResLevel, nInheritedState);
				InsertSubAppRoleOrFunc("ROLES", xmlDoc, strAppResLevel, nInheritedState);

				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.INSERT_ROLE, AppResource.InsertRoleExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// ��Ӧ���¼ӻ������ܣ����漰���ܼ���,һ�����һ������
		/// <Insert>
		///		<FUNCTIONS>
		///			<SET>
		///				<APP_ID></APP_ID>
		///				<NAME></NAME>
		///				<CODE_NAME></CODE_NAME>
		///				<DESCRIPTION></DESCRIPTION>
		///				<INHERITED>�ڵ㲻����ʱĬ�ϲ��̳�</INHERITED>
		///				<CLASSIFY></CLASSIFY>
		///			</SET>
		///		</FUNCTIONS>
		/// </Insert>
		/// </summary>
		protected void InsertFunction(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				int nInheritedState = (int)InheritedState.FUNCTIONS;///4;
				XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");

				string strAppID = nodeSet.SelectSingleNode("APP_ID").InnerText;

				//���Ȩ��
				CheckPermission(strAppID, FunctionNames.ADD_FUNCTION_FUNC, "��ӹ���");
				AppendInsertNodes(xmlDoc, nodeSet);

				string strSortID = string.Empty;
				string strAppResLevel = string.Empty;
				this.GetSortIDAndResLevel("FUNCTIONS", strAppID, out strSortID, out strAppResLevel);
				XmlHelper.AppendNode(nodeSet, "SORT_ID", strSortID);

				InnerCommon.ExecuteNonQuery(InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("FUNCTIONS")));

				//��ѯ�м̳е���Ӧ�õ�guid
				InsertSubAppRoleOrFunc("FUNCTIONS", xmlDoc, strAppResLevel, nInheritedState);
				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.INSERT_FUNCTION, AppResource.InsertFuncExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// ���빦�ܼ��ϣ�һ��ֻ�����һ�����ܼ���
		/// �����ĵ��ĸ�ʽ��
		/// <Insert>
		///		<FUNCTION_SETS>
		///			<SET>
		///				<APP_ID></APP_ID>
		///				<NAME></NAME>
		///				<CODE_NAME></CODE_NAME>
		///				<DESCRIPTION></DESCRIPTION>
		///				<RESOURCE_LEVEL>�����ϵ�reslevel,��һ��Ϊ��</RESOURCE_LEVEL>
		///				<LOWEST_SET></LOWEST_SET>
		///				<INHERITED>û�иýڵ�ʱĬ�ϲ��̳�</INHERITED>
		///				<CLASSIFY></CLASSIFY>
		///			</SET>
		///		</FUNCTION_SETS>
		/// </Insert>
		/// </summary>
		protected void InsertFunctionSet(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				int nInheritedState = (int)InheritedState.FUNCTIONS;//4;
				XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");

				string strAppID = nodeSet.SelectSingleNode("APP_ID").InnerText;

				//���Ȩ��
				CheckPermission(strAppID, FunctionNames.ADD_FUNCTION_FUNC, "��ӹ��ܼ���");

				string strParentResLevel = XmlHelper.GetSingleNodeValue<string>(nodeSet, "RESOURCE_LEVEL", string.Empty);//�����ϵ�resource_level			
				AppendInsertNodes(xmlDoc, nodeSet);
				//������ڸ����ϣ��жϸ����ϵ�lowest_set�ֶ��Ƿ���"n"�������׳��쳣
				ExceptionHelper.TrueThrow<ApplicationException>(CanParentFuncSetHasFuncSets(strAppID, strParentResLevel),
					"�������빦��ֱ�ӹ���������������Ӽ���");

				string strSQL = "SELECT ISNULL(MAX(SORT_ID), 0)+1 AS SORT_ID FROM FUNCTION_SETS "
					+ " WHERE RESOURCE_LEVEL LIKE " + TSqlBuilder.Instance.CheckQuotationMark(strParentResLevel, true) + " +'%' "
					+ " AND LEN(RESOURCE_LEVEL) = " + TSqlBuilder.Instance.CheckQuotationMark((strParentResLevel.Length + 3).ToString(), true)
					+ " AND APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
					+ " SELECT RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true);
				DataSet ds = InnerCommon.ExecuteDataset(strSQL);

				string strSortID = ds.Tables[0].Rows[0]["SORT_ID"].ToString();
				string strAppResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();

				XmlHelper.AppendNode(nodeSet, "SORT_ID", strSortID);

				string strResLevel = this.GetFuncSetResLevel(strParentResLevel, strSortID);
				XmlHelper.ReplaceNode<string>(nodeSet, "RESOURCE_LEVEL", strResLevel);

				//����FunctionSet					
				InnerCommon.ExecuteNonQuery(InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("FUNCTION_SETS")));

				//��ѯ�м̳е���Ӧ�õ�guid
				InsertSubAppFuncSet(xmlDoc, strParentResLevel, nInheritedState, strAppResLevel);

				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.INSERT_FUNCTION, AppResource.InsertFuncExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// ����ί��,��Ҫ����source_id,target_id, role_id, start_time, end_time 
		/// ��������ݸ�ʽ
		/// <Insert>
		///		<DELEGATIONS>
		///			<SET>
		///				<SOURCE_ID></SOURCE_ID>
		///				<TARGET_ID></TARGET_ID>
		///				<ROLE_ID></ROLE_ID>
		///				<START_TIME></START_TIME>
		///				<END_TIME></END_TIME>
		///			</SET>
		///		</DELEGATIONS>
		/// </Insert>
		/// </summary>
		/// <param name="xmlDoc"></param>
		private void InsertDelegation(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				string strRoleID = xmlDoc.DocumentElement.SelectSingleNode(".//ROLE_ID").InnerText;

				string strSQL = "SELECT ALLOW_DELEGATE FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true);
				string strAllowDelegate = InnerCommon.ExecuteScalar(strSQL).ToString();

				//�����ɫ����ί��
				ExceptionHelper.FalseThrow<ApplicationException>(strAllowDelegate == "y", "�ý�ɫ������ί�ɣ�");

				InnerCommon.ExecuteNonQuery(InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("DELEGATES")));
				//ADD LOG
				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.INSERT_DELEGATION, AppResource.InsertDelegationExplain, xmlDoc.OuterXml);
		}
		#endregion

		#region ��Ӳ������Ӻ���
		/// <summary>
		/// ��xmlDoc�ĵ�������ID��MODIFY_TIME�ڵ��Լ��ڵ��ֵ
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="nodeSet"></param>
		private void AppendInsertNodes(XmlDocument xmlDoc, XmlNode nodeSet)
		{
			//XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
			XmlHelper.AppendNode(nodeSet, "ID", Guid.NewGuid().ToString());
			XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
		}
		/// <summary>
		/// �滻xmlDoc�ĵ�SET�ڵ��е�ID,APP_ID,SORT_ID,INHERITED,RESOURCE_LEVEL�Ľڵ�ֵ
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="strAppID"></param>
		/// <param name="strSortID"></param>
		/// <param name="tableName"></param>
		/// <param name="strResLevel"></param>
		private void ReplaceInsertNodes(XmlDocument xmlDoc, string strAppID, string strSortID, string tableName, string strResLevel)
		{
			XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
			XmlHelper.ReplaceNode<string>(nodeSet, "ID", Guid.NewGuid().ToString());
			XmlHelper.ReplaceNode<string>(nodeSet, "APP_ID", strAppID);
			XmlHelper.ReplaceNode<string>(nodeSet, "SORT_ID", strSortID);
			XmlHelper.ReplaceNode<string>(nodeSet, "INHERITED", "y");

			if (tableName == "FUNCTION_SETS")
				XmlHelper.ReplaceNode<string>(nodeSet, "RESOURCE_LEVEL", strResLevel);
		}
		/// <summary>
		/// �滻xmlDoc�ĵ�SET�ڵ��е�ID,APP_ID,SORT_ID,INHERITED�Ľڵ�ֵ
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="strAppID"></param>
		/// <param name="strSortID"></param>
		private void ReplaceInsertNodes(XmlDocument xmlDoc, string strAppID, string strSortID)
		{
			ReplaceInsertNodes(xmlDoc, strAppID, strSortID, string.Empty, string.Empty);
		}
		/// <summary>
		/// ���м̳е���Ӧ������Ӽ̳еĽ�ɫ����
		/// </summary>
		/// <param name="strTableName">"ROLES"</param>
		/// <param name="xmlDoc"></param>
		/// <param name="strAppResLevel">Ӧ�õ�resource_level</param>
		/// <param name="nInheritedState">�����ֵ</param>
		private void InsertSubAppRoleOrFunc(string strTableName, XmlDocument xmlDoc, string strAppResLevel, int nInheritedState)
		{
			DataSet dsSubApp = GetSubAppID(strAppResLevel, nInheritedState);
			if (dsSubApp.Tables[0].Rows.Count != 0)
			{
				foreach (DataRow dr in dsSubApp.Tables[0].Rows)
				{
					string strAppID = dr["APP_ID"].ToString();
					string strSQL = @"SELECT ISNULL(MAX(SORT_ID),0)+1 AS SORT_ID FROM "
						+ TSqlBuilder.Instance.CheckQuotationMark(strTableName, false)
						+ " WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true);
					string strSortID = InnerCommon.ExecuteScalar(strSQL).ToString();

					ReplaceInsertNodes(xmlDoc, strAppID, strSortID);

					try
					{
						InnerCommon.ExecuteNonQuery(InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument(strTableName)));
					}
					catch
					{
						//record the rename code_name's app_id �̳�ʱ��������еĽ�ɫ����codeName��ͻʱ���̳иý�ɫ
					}
				}
			}
		}
		/// <summary>
		/// �ж�������ڸ����ϣ��Ƿ��������Ӽ��ϣ�lowest_set�ֶ��Ƿ���"n"�������׳��쳣
		/// </summary>
		/// <param name="strAppID"></param>
		/// <param name="strParentResLevel"></param>
		/// <returns></returns>
		private bool CanParentFuncSetHasFuncSets(string strAppID, string strParentResLevel)
		{
			bool flag = false;
			if (strParentResLevel != string.Empty)
			{
				string strSQL = "SELECT LOWEST_SET FROM FUNCTION_SETS WHERE APP_ID = "
					+ TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
					+ " AND RESOURCE_LEVEL = " + TSqlBuilder.Instance.CheckQuotationMark(strParentResLevel, true);

				flag = InnerCommon.ExecuteScalar(strSQL).ToString().ToLower().Trim() == "y" ? true : false;
			}
			return flag;
		}

		/// <summary>
		/// ��Ӧ�ü̳й��ܼ���
		/// </summary>
		/// <param name="strParentResLevel"></param>
		private void InsertSubAppFuncSet(XmlDocument xmlDoc, string strParentResLevel, int nInheritedState, string strAppResLevel)
		{
			DataSet dsSubApp = GetSubAppID(strAppResLevel, nInheritedState);

			if (dsSubApp.Tables[0].Rows.Count != 0)
			{
				if (strParentResLevel != string.Empty)//���ڸ�����ʱ
				{
					#region ���ڸ�����ʱ
					string strSQL = @"SELECT CODE_NAME FROM FUNCTION_SETS WHERE RESOURCE_LEVEL = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strParentResLevel, true);
					string strParentCodeName = InnerCommon.ExecuteScalar(strSQL).ToString();

					foreach (DataRow dr in dsSubApp.Tables[0].Rows)
					{
						string strAppID = dr["APP_ID"].ToString();
						strSQL = @" DECLARE @sort_id INT, @resLevel NVARCHAR(32) "
							+ " SET @sort_id = 0 "
							+ " SELECT @resLevel = RESOURCE_LEVEL FROM FUNCTION_SETS WHERE APP_ID = "
							+ TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
							+ " AND CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strParentCodeName, true)
							+ " IF (@resLevel <> '') "
							+ " BEGIN"
							+ "		SELECT @sort_id = ISNULL(MAX(SORT_ID),0)+1  "
							+ "		FROM FUNCTION_SETS "
							+ "		WHERE RESOURCE_LEVEL LIKE @resLevel+'%' "
							+ "		AND LEN(RESOURCE_LEVEL) = LEN(@resLevel) + 3 "
							+ "		AND APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
							+ "		SET @resLevel = @resLevel + REPLACE(STR(@sort_id, 3), ' ', '0') "
							+ " END "
							+ " SELECT @sort_id AS SORT_ID "
							+ " SELECT @resLevel AS RESOURCE_LEVEL ";

						DataSet ds = InnerCommon.ExecuteDataset(strSQL);

						string strSortID = ds.Tables[0].Rows[0]["SORT_ID"].ToString();
						string strResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();
						if ((strSortID != "0") && (strResLevel != string.Empty))
						{
							ReplaceInsertNodes(xmlDoc, strAppID, strSortID, "FUNCTION_SETS", strResLevel);

							strSQL = InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("FUNCTION_SETS"));
							try
							{
								InnerCommon.ExecuteNonQuery(strSQL);
							}
							catch
							{//�������ʱ�����еļ��ϲ�����ͻ������ԭ�еļ���
							}
						}
					}
					#endregion
				}
				else//û�и�����,���ǵ�һ������ʱ
				{
					#region û�и�����,���ǵ�һ������ʱ
					foreach (DataRow dr in dsSubApp.Tables[0].Rows)
					{
						string strAppID = dr["APP_ID"].ToString();
						string strSQL = "DECLARE @sort_id INT, @resLevel NVARCHAR(32) "
							+ " SET @sort_id = 0 "
							+ " SELECT @sort_id = ISNULL(MAX(SORT_ID),0)+1 FROM FUNCTION_SETS "
							+ " WHERE LEN(RESOURCE_LEVEL) = 3 "
							+ " AND APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
							+ " SET @resLevel = REPLACE(STR(@sort_id, 3), ' ', '0') "
							+ " SELECT @sort_id AS SORT_ID "
							+ " SELECT @resLevel AS RESOURCE_LEVEL ";

						DataSet ds = InnerCommon.ExecuteDataset(strSQL);

						string strSortID = ds.Tables[0].Rows[0]["SORT_ID"].ToString();
						string strResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();

						ReplaceInsertNodes(xmlDoc, strAppID, strSortID, "FUNCTION_SETS", strResLevel);
						strSQL = InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("FUNCTION_SETS"));
						try
						{
							InnerCommon.ExecuteNonQuery(strSQL);
						}
						catch
						{//�������ʱ�����еļ��ϲ�����ͻ������ԭ�еļ���
						}
					}
					#endregion
				}
			}
		}

		#endregion

		#region ���½�ɫ�����ܣ����ܼ��ϵ�����
		/// <summary>
		/// ���½�ɫ������,һ��ֻ�ܸ���һ����ɫ
		/// <Update>
		///		<ROLES>
		///			<SET>
		///				<NAME>..</NAME>
		///				<DESCRIPTION></DESCRIPTION>
		///			</SET>
		///			<WHERE>
		///				<ID operator=\"=\">..</ID>
		///			</WHERE>
		///		</ROLES>
		///	</Update>
		/// </summary>
		protected void UpdateRole(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				int nInheritedState = 2;
				XmlNode nodeWhere = xmlDoc.DocumentElement.SelectSingleNode(".//WHERE");
				string strRoleID = nodeWhere.SelectSingleNode("ID").InnerText;

				//���Ȩ��
				CheckPermission(string.Empty, FunctionNames.MODIFY_ROLE_FUNC, "�޸Ľ�ɫ", "ROLES", strRoleID);

				XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
				XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

				ExceptionHelper.TrueThrow<ApplicationException>(IsInheritedObject("ROLES", strRoleID), "��ɫ�Ǽ̳еģ������޸�������");

				string strSQL = "SELECT CODE_NAME FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true)
					+ " SELECT ID,RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
					+ " (SELECT APP_ID AS ID FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true) + ")";
				DataSet ds = InnerCommon.ExecuteDataset(strSQL);

				string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();
				string strAppID = ds.Tables[1].Rows[0]["ID"].ToString();
				string strAppResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();

				//�жϼ̳��ֶ��Ƿ��޸�Ϊ�̳�
				CheckInheritedAttr("ROLES", nodeSet, strAppID, strCodeName, nInheritedState);

				//�ж�����ί�ɵ��ֶ��Ƿ��޸�
				CheckAllowDelegateAttr(nodeSet, strRoleID);

				//���½�ɫ
				strSQL = InnerCommon.GetUpdateSqlStr(xmlDoc, this.GetXSDDocument("ROLES"));
				InnerCommon.ExecuteNonQuery(strSQL);

				UpdateSubAppRoleOrFunc("ROLES", xmlDoc, nodeWhere, strAppResLevel, nInheritedState, strCodeName);
				scope.Complete();
			}
			//��¼��־
			InsertLog(AppResource.AppCodeName, LogOpType.UPDATE_ROLE, AppResource.UpdateRoleExplain, xmlDoc.OuterXml);
		}

		/// <summary>
		///  ���¹�������,һ��ֻ�ܸ���һ������
		/// <Update>
		///		<FUNCTIONS>
		///			<SET>
		///				<NAME></NAME>
		///				<DESCRIPTION></DESCRIPTION>
		///			</SET>
		///			<WHERE>
		///				<ID operator=\"=\"></ID>
		///			</WHERE>
		///		</FUNCTIONS>
		/// </Update>
		/// </summary>
		/// <param name="xmlDoc"></param>
		protected void UpdateFunction(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				int nInheritedState = (int)InheritedState.FUNCTIONS;//4;
				XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
				XmlNode nodeWhere = xmlDoc.DocumentElement.SelectSingleNode(".//WHERE");
				string strFuncID = nodeWhere.SelectSingleNode("ID").InnerText;

				//���Ȩ��
				CheckPermission(string.Empty, FunctionNames.MODIFY_FUNCTION_FUNC, "�޸Ĺ���", "FUNCTIONS", strFuncID);

				XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

				ExceptionHelper.TrueThrow<ApplicationException>(IsInheritedObject("FUNCTIONS", strFuncID), "�����Ǽ̳еģ������޸�������");

				string strSQL = "SELECT CODE_NAME FROM FUNCTIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true)
					+ " ; SELECT ID,RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
					+ " (SELECT APP_ID FROM FUNCTIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true) + ")";

				DataSet ds = InnerCommon.ExecuteDataset(strSQL);

				string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();
				string strAppID = ds.Tables[1].Rows[0]["ID"].ToString();
				string strAppResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();

				//�жϼ̳��ֶ��Ƿ��޸�
				CheckInheritedAttr("FUNCTIONS", nodeSet, strAppID, strCodeName, nInheritedState);

				strSQL = InnerCommon.GetUpdateSqlStr(xmlDoc, this.GetXSDDocument("FUNCTIONS"));
				int nAffectRow = InnerCommon.ExecuteNonQuery(strSQL);


				UpdateSubAppRoleOrFunc("FUNCTIONS", xmlDoc, nodeWhere, strAppResLevel, nInheritedState, strCodeName);
				scope.Complete();
			}
			//��¼��־
			InsertLog(AppResource.AppCodeName, LogOpType.UPDATE_FUNCTION, AppResource.UpdateFuncExplain, xmlDoc.OuterXml);
		}

		/// <summary>
		/// ���¹��ܼ��ϵ����ԣ�һ��ֻ�ܸ���һ������
		/// �������ݵĸ�ʽ
		/// <Update>
		///		<FUNCTION_SET>
		///			<SET>
		///				<NAME></NAME>
		///				<CODE_NAME></CODE_NAME>
		///			</SET>
		///			<WHERE>
		///				<ID operator=\"=\"></ID>
		///			</WHERE>
		///		</FUNCTION_SET>
		/// </Update>
		/// </summary>
		protected void UpdateFunctionSet(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				int nInheritedState = (int)InheritedState.FUNCTIONS;//4;
				XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
				XmlNode nodeWhere = xmlDoc.DocumentElement.SelectSingleNode(".//WHERE");
				string strFuncSetID = nodeWhere.SelectSingleNode("ID").InnerText;

				//���Ȩ��
				CheckPermission(string.Empty, FunctionNames.MODIFY_FUNCTION_FUNC, "�޸Ĺ��ܼ���", "FUNCTION_SETS", strFuncSetID);

				XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

				ExceptionHelper.TrueThrow<ApplicationException>(IsInheritedObject("FUNCTION_SETS", strFuncSetID), "���ܼ����Ǽ̳еģ������޸�������");

				string strSQL = "SELECT CODE_NAME FROM FUNCTION_SETS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true);
				strSQL += "; SELECT ID,RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
					+ " (SELECT APP_ID FROM FUNCTION_SETS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + ")";

				DataSet ds = InnerCommon.ExecuteDataset(strSQL);

				string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();
				string strAppID = ds.Tables[1].Rows[0]["ID"].ToString();
				string strAppResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();

				//�жϼ̳��ֶ��Ƿ��޸�
				CheckInheritedAttr("FUNCTION_SETS", nodeSet, strAppID, strCodeName, nInheritedState);

				//�ж�LOWEST_SET�ֶ��Ƿ��޸ģ�������޸�Ϊ��y��Ҫ�ж�ԭ���Ƿ����Ӽ��ϣ���n��Ҫ�ж�ԭ���Ƿ��������
				CheckLowestSetAttr(nodeSet, strAppID, strFuncSetID);

				strSQL = InnerCommon.GetUpdateSqlStr(xmlDoc, this.GetXSDDocument("FUNCTION_SETS"));
				int nAffectRow = InnerCommon.ExecuteNonQuery(strSQL);


				UpdateSubAppRoleOrFunc("FUNCTION_SETS", xmlDoc, nodeWhere, strAppResLevel, nInheritedState, strCodeName);
				scope.Complete();
			}

			//��¼��־
			InsertLog(AppResource.AppCodeName, LogOpType.UPDATE_FUNCTION, AppResource.UpdateFuncExplain, xmlDoc.OuterXml);
		}

		#endregion

		#region ���²������Ӻ���

		/// <summary>
		/// ����ɫ��ALLOW_DELEGATE�ֶ��Ƿ��޸�
		/// </summary>
		/// <param name="nodeSet"></param>
		/// <param name="strRoleID"></param>
		private void CheckAllowDelegateAttr(XmlNode nodeSet, string strRoleID)
		{
			if (nodeSet.SelectSingleNode(".//ALLOW_DELEGATE") != null)
			{
				string strAllowDelegate = nodeSet.SelectSingleNode("ALLOW_DELEGATE").InnerText;
				if (strAllowDelegate == "n")
				{
					string strSQL = "DELETE DELEGATIONS WHERE ROLE_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true);

					InnerCommon.ExecuteNonQuery(strSQL);
				}
			}
		}

		/// <summary>
		/// ���Inherited�ֶ��Ƿ��޸�
		/// </summary>
		/// <param name="nodeSet">���޸��ֶε�SET�ڵ�</param>
		/// <param name="strAppID">Ӧ�õ�ID</param>
		/// <param name="strCodeName">����CODE_NAMEֵ</param>
		/// <param name="nInheritedState">���������ֵ</param>
		private void CheckInheritedAttr(string strTableName, XmlNode nodeSet, string strAppID, string strCodeName, int nInheritedState)
		{
			if (nodeSet.SelectSingleNode("INHERITED") != null)
			{
				string strInherited = nodeSet.SelectSingleNode("INHERITED").InnerText;
				if (strInherited == "y")
				{
					string strSQL = "SELECT ID FROM " + TSqlBuilder.Instance.CheckQuotationMark(strTableName, false) + " WHERE APP_ID = "
						+ " (SELECT A1.ID AS APP_ID FROM APPLICATIONS AS A1, APPLICATIONS AS A2 "
						+ " WHERE A1.RESOURCE_LEVEL = SUBSTRING(A2.RESOURCE_LEVEL,1,LEN(A2.RESOURCE_LEVEL)-3) "
						+ " AND A2.ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
						+ ")"
						+ " AND CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true)
						+ " SELECT INHERITED_STATE FROM APPLICATIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true);

					DataSet ds = InnerCommon.ExecuteDataset(strSQL);
					string strObjName = string.Empty;
					switch (strTableName)
					{
						case "ROLES":
							strObjName = "��ɫ";
							break;
						case "FUNCTIONS":
							strObjName = "����";
							break;
						case "FUNCTION_SETS":
							strObjName = "���ܼ���";
							break;
					}

					ExceptionHelper.TrueThrow<ApplicationException>(ds.Tables[0].Rows.Count == 0, "û�п��Լ̳е�" + strObjName);
					ExceptionHelper.TrueThrow<ApplicationException>((int.Parse(ds.Tables[1].Rows[0]["INHERITED_STATE"].ToString()) & nInheritedState) == 0,
						"��Ӧ�ò��̳��ϼ�Ӧ�õ�" + strObjName);
				}
			}
		}

		/// <summary>
		/// �жϹ��ܼ��ϵ�LOWEST_SET�ֶ��Ƿ��޸Ĳ�����Ӧ����
		/// </summary>
		/// <param name="nodeSet">���޸��ֶε�SET�ڵ�</param>
		/// <param name="strAppID">Ӧ�õ�ID</param>
		/// <param name="strFuncSetID">���ܼ��ϵ�ID</param>
		private void CheckLowestSetAttr(XmlNode nodeSet, string strAppID, string strFuncSetID)
		{
			if (nodeSet.SelectSingleNode("LOWEST_SET") != null)
			{
				string strLowestSet = nodeSet.SelectSingleNode("LOWEST_SET").InnerText;
				if (strLowestSet == "y") //�ж��Ƿ����Ӽ���
				{
					string strSQL = "SELECT ID FROM FUNCTION_SETS WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
						+ " AND RESOURCE_LEVEL LIKE (SELECT RESOURCE_LEVEL FROM FUNCTION_SETS WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + ") + '%'"
						+ " AND ID <> " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true);

					DataSet ds = InnerCommon.ExecuteDataset(strSQL);

					ExceptionHelper.TrueThrow<ApplicationException>((ds.Tables[0].Rows.Count != 0), "�ü������Ӽ��ϣ������޸��빦��ֱ�ӹ����ĸ�ѡ��");
				}
				else //�ж��Ƿ�����������й���
				{
					string strSQL = "SELECT COUNT(*) AS COUNT FROM FUNC_SET_TO_FUNCS WHERE FUNC_SET_ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true);
					DataSet ds = InnerCommon.ExecuteDataset(strSQL);

					ExceptionHelper.TrueThrow<ApplicationException>((ds.Tables[0].Rows[0]["COUNT"].ToString() != "0"),
						"�ü��ϰ����й���,�����޸��빦��ֱ�ӹ����ĸ�ѡ��");
				}
			}
		}

		/// <summary>
		/// �����м̳е���Ӧ�õ�����(��ɫ�����ܣ����ϣ�
		/// </summary>
		/// <param name="strTableName">���ݿ�ı���</param>
		/// <param name="xmlDoc"></param>
		/// <param name="nodeWhere"></param>
		/// <param name="strAppResLevel"></param>
		/// <param name="nInheritedState"></param>
		private void UpdateSubAppRoleOrFunc(string strTableName,
			XmlDocument xmlDoc,
			XmlNode nodeWhere,
			string strAppResLevel,
			int nInheritedState,
			string strCodeName)
		{
			DataSet dsSubApp = GetSubAppID(strAppResLevel, nInheritedState);

			if (dsSubApp.Tables[0].Rows.Count != 0)
			{
				foreach (DataRow dr in dsSubApp.Tables[0].Rows)
				{
					string strAppID = dr["APP_ID"].ToString();

					string strSQL = "SELECT ID FROM " + TSqlBuilder.Instance.CheckQuotationMark(strTableName, false)
						+ " WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
						+ " AND CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true)
						+ " AND INHERITED = 'y'";
					DataSet ds = InnerCommon.ExecuteDataset(strSQL);

					if (ds.Tables[0].Rows.Count != 0)
					{
						XmlHelper.ReplaceNode<string>(nodeWhere, "ID", ds.Tables[0].Rows[0]["ID"].ToString());

						strSQL = InnerCommon.GetUpdateSqlStr(xmlDoc, this.GetXSDDocument(strTableName));

						try
						{
							InnerCommon.ExecuteNonQuery(strSQL);
						}
						catch
						{
							strSQL = "UPDATE " + strTableName + " SET INHERITED = 'n' WHERE ID = "
								+ TSqlBuilder.Instance.CheckQuotationMark(ds.Tables[0].Rows[0]["ID"].ToString(), true);

							InnerCommon.ExecuteDataset(strSQL);
						}
					}
				}
			}
		}


		#endregion

		#region ɾ����ɫ�����ܣ����ܼ���
		/// <summary>
		/// ɾ����ɫ��һ�ο���ɾ�����
		/// <Delete deleteSubApp="y/n" sureDelete="y/n">
		///		<ROLES>
		///			<WHERE>
		///				<ID operator=\"=\"></ID>
		///			</WHERE>
		///		</ROLES>
		///		<ROLES>
		///			<WHERE>
		///				<ID operator=\"=\"></ID>
		///			</WHERE>
		///		</ROLES>
		/// </Delete>
		/// </summary>
		protected void DeleteRole(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				int nInheritedState = (int)InheritedState.ROLES;//2			

				//���Ȩ��
				CheckPermission(string.Empty, FunctionNames.DELETE_ROLE_FUNC, "ɾ����ɫ", "ROLES",
					xmlDoc.DocumentElement.SelectSingleNode(".//ID").InnerText);

				//Ĭ�ϲ�ɾ����Ӧ�ü̳еĽ�ɫ
				string strDelSubApp = this.GetAttr(xmlDoc, "deleteSubApp", AppResource.DefaultDelSubApp);

				//�����ɫ�Ǽ̳У��Ƿ�ȷ��ɾ���ý�ɫ
				string strDelRole = AppResource.DefaultDelObj; //"n";			

				XmlNode nodeRole = xmlDoc.DocumentElement.SelectSingleNode("ROLES");

				while (nodeRole != null)
				{
					string strRoleID = nodeRole.SelectSingleNode(".//ID").InnerText;

					//�ж�ɾ�����Ƿ�������Ȩ����Ա������CODE_NAME
					ExceptionHelper.TrueThrow<ApplicationException>(IsSelfAccreditRole(strRoleID),
						"Ԥ���������Ȩ����Ա��ɫ����ɾ��");

					string strSQL = "SELECT INHERITED,CODE_NAME FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true)
						+ " SELECT RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
						+ " (SELECT APP_ID AS ID FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true) + ")";

					DataSet ds = InnerCommon.ExecuteDataset(strSQL);
					string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();

					//�жϽ�ɫ�Ƿ��Ǽ̳е�,���Ǽ̳е�,�Ƿ�ȷ��ɾ��(n,����)				
					if (ds.Tables[0].Rows[0]["INHERITED"].ToString() == "y")
					{
						ExceptionHelper.TrueThrow<ApplicationException>((strDelRole == "n" || strDelRole == string.Empty),
							"�ý�ɫ�Ǽ̳еģ�����ɾ����");
					}

					#region �������Ӧ�ü̳�,ɾ����Ӧ�Ľ�ɫ����һ���ļ̳й�ϵ��Ϊ�Ǽ̳�
					string strAppResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();
					StringBuilder sqlBuilder = new StringBuilder(512);
					if (strDelSubApp == "y")
					{
						DataSet dsSubApp = this.GetSubAppID(strAppResLevel, nInheritedState);
						if (dsSubApp.Tables[0].Rows.Count != 0)
						{
							foreach (DataRow dr in dsSubApp.Tables[0].Rows)
							{
								sqlBuilder.Append("DELETE FROM ROLES WHERE APP_ID = "
									+ TSqlBuilder.Instance.CheckQuotationMark(dr["APP_ID"].ToString(), true)
									+ " AND CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true)
									+ " AND INHERITED = 'y';" + Environment.NewLine);
							}
						}
					}
					else
					{
						strSQL = "SELECT A1.ID, A1.RESOURCE_LEVEL "
							+ " FROM APPLICATIONS AS A1,APPLICATIONS AS A2 "
							+ " WHERE A1.RESOURCE_LEVEL LIKE A2.RESOURCE_LEVEL +'%' "
							+ " AND LEN(A1.RESOURCE_LEVEL) = LEN(A2.RESOURCE_LEVEL) + 3 "
							+ " AND A2.ID = (SELECT APP_ID AS ID FROM ROLES WHERE ID = "
							+ TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true)
							+ ")";
						DataSet dsNextSubApp = InnerCommon.ExecuteDataset(strSQL);

						if (dsNextSubApp.Tables[0].Rows.Count != 0)
						{
							foreach (DataRow dr in dsNextSubApp.Tables[0].Rows)
							{
								sqlBuilder.Append("UPDATE ROLES SET INHERITED = 'n' WHERE APP_ID = "
									+ TSqlBuilder.Instance.CheckQuotationMark(dr["ID"].ToString(), true)
									+ " AND CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true)
									+ " AND INHERITED = 'y';" + Environment.NewLine);
							}
						}
					}

					sqlBuilder.Append("DELETE FROM ROLES WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true) + ";" + Environment.NewLine);

					InnerCommon.ExecuteNonQuery(sqlBuilder.ToString());
					#endregion

					nodeRole = nodeRole.NextSibling;
				}
				scope.Complete();
			}

			InsertLog(AppResource.AppCodeName, LogOpType.DELETE_ROLE, AppResource.DeleteRoleExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// ɾ�����ܣ�һ�ο�ɾ�����
		/// <Delete deleteSubApp="y/n" sureDelete = "y/n">
		///		<FUNCTIONS>
		///			<WHERE>
		///				<ID operator=\"=\">..</ID>
		///			</WHERE>
		///		</FUNCTIONS>
		///		<FUNCTIONS>
		///			<WHERE>
		///				<ID operator=\"=\">..</ID>
		///			</WHERE>
		///		</FUNCTIONS>
		///	</Delete>
		/// </summary>
		protected void DeleteFunction(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				int nInheritedState = (int)InheritedState.FUNCTIONS;//4;

				//���Ȩ��
				CheckPermission(string.Empty, FunctionNames.DELETE_FUNCTION_FUNC, "ɾ������", "FUNCTIONS",
					xmlDoc.DocumentElement.SelectSingleNode(".//ID").InnerText);

				//Ĭ�ϲ�ɾ����Ӧ�ü̳еĹ���
				string strDelSubApp = this.GetAttr(xmlDoc, "deleteSubApp", AppResource.DefaultDelSubApp);//"n"

				//����ù����Ǽ̳�,�Ƿ�ȷ��ɾ���ù���
				string strDelFunc = AppResource.DefaultDelObj; //"y"; 


				XmlNode nodeFunction = xmlDoc.DocumentElement.SelectSingleNode(".//FUNCTIONS");

				while (nodeFunction != null)
				{
					string strFuncID = nodeFunction.SelectSingleNode(".//ID").InnerText;

					//�ж�ɾ�����Ƿ�������Ȩ�Ĺ��ܣ�����CODE_NAME
					ExceptionHelper.TrueThrow<ApplicationException>(IsSelfAccreditFunction(strFuncID), "Ԥ���������Ȩ���ܲ���ɾ��");

					string strSQL = "SELECT INHERITED,CODE_NAME FROM FUNCTIONS WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true)
						+ " SELECT RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
						+ " (SELECT APP_ID AS ID FROM FUNCTIONS WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true) + ")";

					DataSet ds = InnerCommon.ExecuteDataset(strSQL);
					string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();

					// �жϹ����Ƿ��Ǽ̳е�; ���Ǽ̳е�,�Ƿ�ȷ��ɾ��(n,����), 						
					if (ds.Tables[0].Rows[0]["INHERITED"].ToString() == "y")
					{
						ExceptionHelper.TrueThrow<ApplicationException>((strDelFunc == "n" || strDelFunc == string.Empty),
							"�ù����Ǽ̳еģ�����ɾ����");
					}

					#region �������Ӧ�ü̳У���ɾ����Ӧ�Ĺ���,������һ���ļ̳й�ϵ��Ϊ�Ǽ̳�
					string strAppResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();
					StringBuilder sqlBuilder = new StringBuilder(512);
					if (strDelSubApp == "y")
					{
						DataSet dsSubApp = this.GetSubAppID(strAppResLevel, nInheritedState);
						if (dsSubApp.Tables[0].Rows.Count != 0)
						{
							foreach (DataRow dr in dsSubApp.Tables[0].Rows)
							{
								sqlBuilder.Append("DELETE FROM FUNCTIONS WHERE APP_ID = ");
								sqlBuilder.Append(TSqlBuilder.Instance.CheckQuotationMark(dr["APP_ID"].ToString(), true));
								sqlBuilder.Append(" AND CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true));
								sqlBuilder.Append(" AND INHERITED = 'y';" + Environment.NewLine);
							}
						}
					}
					else
					{
						strSQL = "SELECT A1.ID, A1.RESOURCE_LEVEL "
							+ " FROM APPLICATIONS AS A1,APPLICATIONS AS A2 "
							+ " WHERE A1.RESOURCE_LEVEL LIKE A2.RESOURCE_LEVEL +'%' "
							+ " AND LEN(A1.RESOURCE_LEVEL) = LEN(A2.RESOURCE_LEVEL) + 3 "
							+ " AND A2.ID = (SELECT APP_ID AS ID FROM FUNCTIONS WHERE ID = "
							+ TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true)
							+ ")";
						DataSet dsNextSubApp = InnerCommon.ExecuteDataset(strSQL);
						if (dsNextSubApp.Tables[0].Rows.Count != 0)
						{
							foreach (DataRow dr in dsNextSubApp.Tables[0].Rows)
							{
								sqlBuilder.Append("UPDATE FUNCTIONS SET INHERITED = 'n' WHERE APP_ID = ");
								sqlBuilder.Append(TSqlBuilder.Instance.CheckQuotationMark(dr["ID"].ToString(), true));
								sqlBuilder.Append(" AND CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true));
								sqlBuilder.Append(" AND INHERITED = 'y';" + Environment.NewLine);
							}
						}
					}

					sqlBuilder.Append("DELETE FROM FUNCTIONS WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true) + ";" + Environment.NewLine);
					InnerCommon.ExecuteDataset(sqlBuilder.ToString());
					#endregion
					nodeFunction = nodeFunction.NextSibling;
				}
				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.DELETE_FUNCTION, AppResource.DeleteFuncExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// ɾ�����ܼ��ϣ�һ�ο�ɾ�����
		/// <Delete deleteSubApp="y/n" sureDelete="y/n">
		///		<FUNCTION_SETS>
		///			<WHERE>
		///				<ID operator=\"=\">..</ID>
		///			</WHERE>
		///		</FUNCTION_SETS>
		///		<FUNCTION_SETS>
		///			<WHERE>
		///				<ID operator=\"=\"></ID>
		///			</WHERE>
		///		</FUNCTION_SETS>
		///	</Delete>
		/// </summary>
		protected void DeleteFunctionSet(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				int nInheritedState = (int)InheritedState.FUNCTIONS; //4;

				//���Ȩ��
				CheckPermission(string.Empty, FunctionNames.DELETE_FUNCTION_FUNC, "ɾ�����ܼ���", "FUNCTION_SETS",
					xmlDoc.DocumentElement.SelectSingleNode(".//ID").InnerText);

				//Ĭ�ϲ�ɾ����Ӧ�ü̳еĹ��ܼ���
				string strDelSubApp = this.GetAttr(xmlDoc, "deleteSubApp", AppResource.DefaultDelSubApp);//"n"

				//Ĭ����ȷ��ɾ���ù��ܼ���
				string strSureDel = AppResource.DefaultDelObj;//"y"; 

				XmlNode nodeFuncSet = xmlDoc.DocumentElement.SelectSingleNode(".//FUNCTION_SETS");

				while (nodeFuncSet != null)
				{
					string strFuncSetID = nodeFuncSet.SelectSingleNode(".//ID").InnerText;

					//�ж��Ƿ�����Ȩ
					ExceptionHelper.TrueThrow<ApplicationException>(IsSelfAccreditFunctionSet(strFuncSetID), "Ԥ���������Ȩ���ܼ��ϲ���ɾ��");

					string strSQL = "SELECT INHERITED,CODE_NAME,RESOURCE_LEVEL FROM FUNCTION_SETS WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true)
						+ " SELECT RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
						+ " (SELECT APP_ID AS ID FROM FUNCTION_SETS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + ")";

					DataSet ds = InnerCommon.ExecuteDataset(strSQL);

					string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();
					string strResLevel = ds.Tables[0].Rows[0]["RESOURCE_LEVEL"].ToString();

					//�Ƿ��Ǽ̳�,���Ǽ̳е�,�Ƿ�ȷ��ɾ��(n,����),
					if (ds.Tables[0].Rows[0]["INHERITED"].ToString() == "y")
					{
						ExceptionHelper.TrueThrow<ApplicationException>((strSureDel == "n" || strSureDel == string.Empty), "�ù��ܼ����Ǽ̳еģ�����ɾ����");
					}

					#region ɾ�������Ӽ��ϣ�ɾ���빦�ܵĶ�Ӧ,��һ���̳и�Ϊ�Ǽ̳�
					string strAppResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();
					StringBuilder sqlBuilder = new StringBuilder(512);

					if (strDelSubApp == "y")
					{
						DataSet dsSubApp = this.GetSubAppID(strAppResLevel, nInheritedState);
						if (dsSubApp.Tables[0].Rows.Count != 0)
						{
							foreach (DataRow dr in dsSubApp.Tables[0].Rows)
							{
								sqlBuilder.Append(" DELETE FROM FUNCTION_SETS WHERE RESOURCE_LEVEL LIKE ");
								sqlBuilder.Append(" (SELECT RESOURCE_LEVEL FROM FUNCTION_SETS WHERE APP_ID = ");
								sqlBuilder.Append(TSqlBuilder.Instance.CheckQuotationMark(dr["APP_ID"].ToString(), true));
								sqlBuilder.Append(" AND CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true));
								sqlBuilder.Append(" AND INHERITED = 'y') + '%';" + Environment.NewLine);
							}
						}
					}
					else
					{
						strSQL = " SELECT A1.ID, A1.RESOURCE_LEVEL "
							+ " FROM APPLICATIONS AS A1,APPLICATIONS AS A2 "
							+ " WHERE A1.RESOURCE_LEVEL LIKE A2.RESOURCE_LEVEL +'%' "
							+ " AND LEN(A1.RESOURCE_LEVEL) = LEN(A2.RESOURCE_LEVEL) + 3 "
							+ " AND A2.ID = (SELECT APP_ID FROM FUNCTION_SETS WHERE ID = "
							+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true)
							+ ")";

						DataSet dsNextSubApp = InnerCommon.ExecuteDataset(strSQL);

						if (dsNextSubApp.Tables[0].Rows.Count != 0)
						{
							foreach (DataRow dr in dsNextSubApp.Tables[0].Rows)
							{
								sqlBuilder.Append(" UPDATE FUNCTION_SETS SET INHERITED = 'n' WHERE APP_ID = ");
								sqlBuilder.Append(TSqlBuilder.Instance.CheckQuotationMark(dr["ID"].ToString(), true));
								sqlBuilder.Append(" AND INHERITED = 'y' ");
								sqlBuilder.Append(" AND RESOURCE_LEVEL LIKE ( SELECT RESOURCE_LEVEL FROM FUNCTION_SETS WHERE CODE_NAME = ");
								sqlBuilder.Append(TSqlBuilder.Instance.CheckQuotationMark(strCodeName, true));
								sqlBuilder.Append(" AND APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(dr["ID"].ToString(), true));
								sqlBuilder.Append(" )+'%' ;" + Environment.NewLine);
							}
						}
					}

					sqlBuilder.Append(" DELETE FROM FUNCTION_SETS WHERE RESOURCE_LEVEL LIKE (SELECT RESOURCE_LEVEL FROM FUNCTION_SETS WHERE ID = ");
					sqlBuilder.Append(TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + " ) + '%' ");
					sqlBuilder.Append(" AND APP_ID IN (SELECT APP_ID FROM FUNCTION_SETS WHERE ID = ");
					sqlBuilder.Append(TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + ");" + Environment.NewLine);
					InnerCommon.ExecuteNonQuery(sqlBuilder.ToString());
					#endregion

					nodeFuncSet = nodeFuncSet.NextSibling;
				}
				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.DELETE_FUNCTION, AppResource.DeleteFuncExplain, xmlDoc.OuterXml);
		}
		///// <summary>
		///// ɾ��ί��
		///// <Delete>
		/////		<DELEGATIONS>
		/////			<WHERE>
		/////				<SOURCE_ID operator=\"=\">..</SOURCE_ID>
		/////				<TARGET_ID operator=\"=\">..</TARGET_ID>
		/////				<ROLE_ID operator=\"=\">..</ROLE_ID>
		/////			</WHERE>
		/////		</DELEGATIONS>
		/////	</Delete>
		///// </summary>
		//private void DeleteDelegation(XmlDocument xmlDoc)
		//{
		//    using (TransactionScope scope = TransactionScopeFactory.Create())
		//    {
		//        InnerCommon.ExecuteNonQuery(InnerCommon.GetDeleteSqlStr(xmlDoc, this.GetXSDDocument("DELEGATIONS")));
		//        scope.Complete();
		//    }
		//    InsertLog(AppResource.AppCodeName, Define.LogOpType.DELETE_DELEGATION, Define.C_Insert_Delegation_Explain, xmlDoc.OuterXml);
		//}
		#endregion

		#region ά����ɫ���ܵĹ�ϵ��ά�����ܺ͹��ܼ��ϵĹ�ϵ��RTF,RTFS,FSTF
		/// <summary>
		/// �޸Ľ�ɫ���ܹ�ϵ
		/// </summary>
		/// <param name="xmlDoc"></param>
		protected void DoRTF(XmlDocument xmlDoc)
		{
			DoRTF(xmlDoc, true);
		}
		private void DoRTF(XmlDocument xmlDoc, bool IsNeedLog)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				#region DataPrepare
				int nInheritedState = (int)InheritedState.ROLE_TO_FUNCTIONS;//8;

				//���Ȩ��
				CheckPermission(string.Empty, FunctionNames.RTF_MAINTAIN_FUNC, "�޸Ľ�ɫ���ܹ�ϵ", "FUNCTIONS",
					xmlDoc.DocumentElement.SelectSingleNode(".//FUNC_ID").InnerText);

				XmlNode nodeInsert = xmlDoc.DocumentElement.SelectSingleNode(".//Insert");
				XmlNode nodeDelete = xmlDoc.DocumentElement.SelectSingleNode(".//Delete");

				string strDeleteSubApp = this.GetAttr(xmlDoc, "deleteSubApp", AppResource.DefaultDelSubApp);//"n"

				//get current app id and resource_level
				string strRoleID = string.Empty;
				string strFuncID = xmlDoc.DocumentElement.SelectSingleNode(".//FUNC_ID").InnerText;

				string strSQL = @"SELECT ID,RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
					+ "(SELECT APP_ID FROM FUNCTIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true) + ")";
				DataSet ds = InnerCommon.ExecuteDataset(strSQL);

				string strAppResLevel = ds.Tables[0].Rows[0]["RESOURCE_LEVEL"].ToString();
				string strAppID = ds.Tables[0].Rows[0]["ID"].ToString();

				DataSet dsSubApp = this.GetSubAppID(strAppResLevel, nInheritedState);
				DataTable dt = dsSubApp.Tables[0];
				#endregion

				bool HasInheritedRTF = false;

				#region insert role_to_functions
				if (nodeInsert != null)
				{
					XmlNode nodeRTF = nodeInsert.SelectSingleNode(".//ROLE_TO_FUNCTIONS");

					while (nodeRTF != null)
					{
						strRoleID = nodeRTF.SelectSingleNode(".//ROLE_ID").InnerText;
						strFuncID = nodeRTF.SelectSingleNode(".//FUNC_ID").InnerText;

						if (IsInheritedRTF(strRoleID, strFuncID))
						{
							//throw new Exception("��ɫ���ܹ�ϵ�Ǽ̳еģ������޸ģ�");
							HasInheritedRTF = true;
						}
						else
						{

							try
							{
								//strSQL = GetSqlStr.GetInsertSqlStr(xmlDocInsert, this.GetXSDDocument("ROLE_TO_FUNCTIONS"));
								strSQL = "INSERT INTO ROLE_TO_FUNCTIONS (ROLE_ID,FUNC_ID) VALUES (" + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true)
									+ " ," + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true) + ")";
								InnerCommon.ExecuteNonQuery(strSQL);
							}
							catch
							{//��ֹ�ڲ��빦�ܼ���ʱ�Ѿ��л����������ɫ�Ĺ�ϵ�Ѳ���
							}

							#region �����м̳е���Ӧ�õĽ�ɫ���ܹ�ϵ
							if (dt.Rows.Count != 0)
							{
								strSQL = "SELECT CODE_NAME FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true)
									+ " SELECT CODE_NAME FROM FUNCTIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true);

								ds = InnerCommon.ExecuteDataset(strSQL);

								string strRoleCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();
								string strFuncCodeName = ds.Tables[1].Rows[0]["CODE_NAME"].ToString();

								foreach (DataRow dr in dt.Rows)
								{
									strSQL = "SELECT ROLES.ID AS ROLE_ID, FUNCTIONS.ID AS FUNC_ID FROM ROLES, FUNCTIONS "
										+ " WHERE ROLES.APP_ID = FUNCTIONS.APP_ID AND ROLES.APP_ID = "
										+ TSqlBuilder.Instance.CheckQuotationMark(dr["APP_ID"].ToString(), true)
										+ " AND ROLES.CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleCodeName, true)
										+ " AND FUNCTIONS.CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncCodeName, true);

									ds = InnerCommon.ExecuteDataset(strSQL);

									//�����Ӧ���Ѿ��̳��˽�ɫ�͹������Զ��̳н�ɫ���ܹ�ϵ���������
									if (ds.Tables[0].Rows.Count != 0)
									{
										try
										{
											strSQL = "INSERT INTO ROLE_TO_FUNCTIONS VALUES ("
												+ TSqlBuilder.Instance.CheckQuotationMark(ds.Tables[0].Rows[0]["ROLE_ID"].ToString(), true)
												+ ","
												+ TSqlBuilder.Instance.CheckQuotationMark(ds.Tables[0].Rows[0]["FUNC_ID"].ToString(), true)
												+ ",'y')";
											InnerCommon.ExecuteNonQuery(strSQL);
										}
										catch
										{
										}
									}
								}//foreach
							}
							#endregion
						}

						nodeRTF = nodeRTF.NextSibling;
					}
				}
				#endregion

				#region delete role_to_functions
				if (nodeDelete != null)
				{
					XmlNode nodeRTF = nodeDelete.SelectSingleNode(".//ROLE_TO_FUNCTIONS");
					while (nodeRTF != null)
					{
						strRoleID = nodeRTF.SelectSingleNode(".//ROLE_ID").InnerText;
						strFuncID = nodeRTF.SelectSingleNode(".//FUNC_ID").InnerText;

						if (this.IsInheritedRTF(strRoleID, strFuncID))
						{
							HasInheritedRTF = true;
						}
						else
						{
							StringBuilder sqlBuilder = new StringBuilder(512);
							sqlBuilder.Append(" DELETE FROM ROLE_TO_FUNCTIONS WHERE ROLE_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true));
							sqlBuilder.Append(" AND FUNC_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true) + ";" + Environment.NewLine);

							if (dt.Rows.Count != 0)
							{
								if (strDeleteSubApp == "y")//ɾ���̳���Ӧ�õĽ�ɫ���ܹ�ϵ
								{
									#region //ɾ���̳���Ӧ�õĽ�ɫ���ܹ�ϵ
									foreach (DataRow dr in dt.Rows)
									{
										sqlBuilder.Append(" DELETE FROM ROLE_TO_FUNCTIONS ");
										sqlBuilder.Append(" WHERE ROLE_ID = (SELECT ID FROM ROLES WHERE CODE_NAME = ");
										sqlBuilder.Append(" (SELECT CODE_NAME FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true));
										sqlBuilder.Append(" ) AND APP_ID =" + TSqlBuilder.Instance.CheckQuotationMark(dr["APP_ID"].ToString(), true) + ")");
										sqlBuilder.Append(" AND FUNC_ID = (SELECT ID FROM FUNCTIONS WHERE CODE_NAME = (");
										sqlBuilder.Append("SELECT CODE_NAME FROM FUNCTIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true));
										sqlBuilder.Append(" )");
										sqlBuilder.Append(" AND APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(dr["APP_ID"].ToString(), true) + ")");
										sqlBuilder.Append(" AND INHERITED = 'y';" + Environment.NewLine);
									}
									#endregion
								}
								else //��ɾ���̳���Ӧ�õĽ�ɫ���ܹ�ϵ,����һ���ļ̳и�Ϊ���̳�
								{
									#region ��ɾ���̳���Ӧ�õĽ�ɫ���ܹ�ϵ,����һ���ļ̳и�Ϊ���̳�
									int nResLevelLen = strAppResLevel.Length + 3;

									strSQL = "SELECT ID,RESOURCE_LEVEL FROM APPLICATIONS "
										+ " WHERE RESOURCE_LEVEL LIKE " + TSqlBuilder.Instance.CheckQuotationMark(strAppResLevel + "%", true)
										+ " AND LEN(RESOURCE_LEVEL) = " + TSqlBuilder.Instance.CheckQuotationMark(nResLevelLen.ToString(), true);

									DataSet dsOneLevelSubApp = InnerCommon.ExecuteDataset(strSQL);
									if (dsOneLevelSubApp.Tables[0].Rows.Count != 0)
									{
										foreach (DataRow dr in dsOneLevelSubApp.Tables[0].Rows)
										{
											sqlBuilder.Append("UPDATE ROLE_TO_FUNCTIONS SET INHERITED = 'n' ");
											sqlBuilder.Append(" WHERE ROLE_ID = (SELECT ID FROM ROLES WHERE CODE_NAME = ");
											sqlBuilder.Append("(SELECT CODE_NAME FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true));
											sqlBuilder.Append(" ) AND APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(dr["ID"].ToString(), true));
											sqlBuilder.Append(" ) ");
											sqlBuilder.Append(" AND FUNC_ID = (SELECT ID FROM FUNCTIONS WHERE CODE_NAME = ");
											sqlBuilder.Append(" (SELECT CODE_NAME FROM FUNCTIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true));
											sqlBuilder.Append(" ) AND APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(dr["ID"].ToString(), true));
											sqlBuilder.Append(" )");
											sqlBuilder.Append(" AND INHERITED = 'y';" + Environment.NewLine);
										}
									}
									#endregion
								}
							}
							if (sqlBuilder.Length > 0)
								InnerCommon.ExecuteNonQuery(sqlBuilder.ToString());
						}
						nodeRTF = nodeRTF.NextSibling;
					} //while
				}
				#endregion
				//HasInheritedRTF = true;
				ExceptionHelper.TrueThrow<ApplicationException>(HasInheritedRTF, "��ɫ���ܹ�ϵ�Ǽ̳еģ������޸ģ�");
				scope.Complete();
			}
			if (IsNeedLog)
				InsertLog(AppResource.AppCodeName, LogOpType.MODIFY_ROLE_TO_FUNC, AppResource.ModifyRoleToFuncExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// �޸Ľ�ɫ�͹��ܼ��ϵĹ�ϵ
		/// �����xml�ļ���ʽ:
		/// <RTFS  deleteSubApp="y/n">
		///		<Insert>
		///			<ROLE_TO_FUNCTIONS>
		///				<SET>
		///					<ROLE_ID>��ɫ��ID</ROLE_ID>
		///					<FUNC_SET_ID>���ܼ��ϵ�ID</FUNC_SET_ID>
		///				</SET>
		///			</ROLE_TO_FUNCTIONS>
		///			...
		///		</Insert>
		///		<Delete>
		///			<ROLE_TO_FUNCTIONS>
		///				<WHERE>
		///					<ROLE_ID>��ɫ��ID</ROLE_ID>
		///					<FUNC_SET_ID>���ܼ��ϵ�ID</FUNC_SET_ID>
		///				</WHERE>
		///			</ROLE_TO_FUNCTIONS>
		///			...
		///		</Delete>
		///	</RTFS>
		/// </summary>
		/// <param name="xmlDoc"></param>
		protected void DoRTFS(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				string strFuncSetID = XmlHelper.GetSingleNodeValue<string>(xmlDoc.DocumentElement, ".//FUNC_SET_ID", string.Empty);

				//���Ȩ��
				CheckPermission(string.Empty, FunctionNames.RTF_MAINTAIN_FUNC, "�޸Ľ�ɫ���ܹ�ϵ", "FUNCTION_SETS", strFuncSetID);

				if (strFuncSetID != string.Empty)
				{
					string strSQL = "SELECT FUNC_ID FROM FUNC_SET_TO_FUNCS WHERE FUNC_SET_ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true);
					DataSet dsFuncID = InnerCommon.ExecuteDataset(strSQL);

					ExceptionHelper.TrueThrow(dsFuncID.Tables[0].Rows.Count == 0, "�Բ��𣬵�ǰ���ܰ���û�о��幦������ܹ�����ɫ");

					XmlDocument xmlDocRTF = new XmlDocument();
					xmlDocRTF.LoadXml("<RTF />");

					string strDeleteSubApp = GetAttr(xmlDoc, "deleteSubApp", AppResource.DefaultDelSubApp); //GetAttr(xmlDoc, "deleteSubApp", "n");
					XmlHelper.AppendAttr(xmlDocRTF.DocumentElement, "deleteSubApp", strDeleteSubApp);

					if (dsFuncID.Tables[0].Rows.Count != 0)
					{
						XmlNode nodeInsert = xmlDoc.DocumentElement.SelectSingleNode(".//Insert");
						XmlNode nodeDelete = xmlDoc.DocumentElement.SelectSingleNode(".//Delete");

						//�����ɫ�ͻ������ܵĹ����������ܼ���ת��Ϊ����
						this.ExchangeRTFSToRTF(xmlDocRTF, nodeInsert, dsFuncID, "Insert");

						//������Ҫɾ���Ľ�ɫ�͹��ܹ�ϵ�������ܼ���ת��Ϊ����
						this.ExchangeRTFSToRTF(xmlDocRTF, nodeDelete, dsFuncID, "Delete");

					}

					DoRTF(xmlDocRTF, false);
				}
				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.MODIFY_ROLE_TO_FUNC, AppResource.ModifyRoleToFuncExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// ��ӻ�ɾ�����ܼ��Ϻ͹���֮��Ĺ�����һ������ֻ������һ�����ϣ���ǰ�˿���
		/// ���ܺͼ��ϵĹ�ϵ���̳�
		/// ������ļ���ʽ:
		/// <FSTF  deleteSubApp="y/n">
		///		<Insert>
		///			<FUNC_SET_TO_FUNCS>
		///				<SET>
		///					<FUNC_SET_ID>���ܼ��ϵ�ID</FUNC_SET_ID>
		///					<FUNC_ID>���ϵ�ID</FUNC_ID>
		///				</SET>
		///			</FUNC_SET_TO_FUNCS>
		///			....
		///		</Insert>
		///		<Delete>
		///			<FUNC_SET_TO_FUNCS>
		///				<WHERE>
		///					<FUNC_SET_ID>���ܼ��ϵ�ID</FUNC_SET_ID>
		///					<FUNC_ID>���ϵ�ID</FUNC_ID>
		///				</WHERE>
		///			</FUNC_SET_TO_FUNCS>
		///			...
		///		</Delete>
		///	</FSTF>
		/// </summary>
		/// <param name="xmlDoc"></param>
		protected void DoFSTF(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				//���Ȩ��
				CheckPermission(string.Empty, FunctionNames.FSTF_MAINTAIN_FUNC, "�޸Ĺ���-���ܼ��Ϲ�ϵ", "FUNCTION_SETS",
					xmlDoc.DocumentElement.SelectSingleNode(".//FUNC_SET_ID").InnerText);

				XmlNode nodeInsert = xmlDoc.DocumentElement.SelectSingleNode(".//Insert");
				XmlNode nodeDelete = xmlDoc.DocumentElement.SelectSingleNode(".//Delete");

				//�жϹ��ܼ��ϵ�LOWEST_SET�Ƿ���"y",�����׳��쳣
				string strFuncSetID = XmlHelper.GetSingleNodeValue<string>(xmlDoc, ".//FUNC_SET_ID", string.Empty);

				if (strFuncSetID != string.Empty)
				{
					string strSQL1 = @"SELECT LOWEST_SET FROM FUNCTION_SETS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true);

					Object oLowestSet = InnerCommon.ExecuteScalar(strSQL1);

					ExceptionHelper.TrueThrow<ApplicationException>(oLowestSet != null && oLowestSet.ToString() != "y", "�ù��ܼ��ϲ��ܺ͹���ֱ�ӹ���");
				}

				string strSQL = DoInnerFSTF(nodeInsert, "Insert") + " " + DoInnerFSTF(nodeDelete, "Delete");

				if (strSQL != " ")
					InnerCommon.ExecuteNonQuery(strSQL);
				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.UPDATE_FUNCTION, AppResource.UpdateFuncExplain, xmlDoc.OuterXml);
		}
		#endregion

		#region ά����ɫ���ܹ�ϵ�����ܺͼ��Ϲ�ϵ���Ӻ���
		/// <summary>
		/// ���ڵ�nodeת����XmlDocument�ĵ�
		/// </summary>
		/// <param name="node">��Ҫת���Ľڵ�</param>
		/// <returns>ת�����xml�ĵ�</returns>
		private XmlDocument GetXmlDocument(XmlNode node)
		{
			XmlDocument xmlDoc = new XmlDocument();
			if (node != null)
				xmlDoc.LoadXml(node.OuterXml);

			return xmlDoc;
		}
		/// <summary>
		/// �ɽڵ�node����SQL��䣬strOperator�ǲ�������
		/// </summary>
		/// <param name="node">��Ҫת���Ľڵ�</param>
		/// <param name="strOperator">ִ�еĲ�����Insert��Delete</param>
		/// <returns>���ɵ�sql���</returns>
		private string DoInnerFSTF(XmlNode node, string strOperator)
		{
			string strSQL = string.Empty;

			if (node != null)
			{
				XmlDocument xmlDoc = GetXmlDocument(node);

				if (strOperator == "Insert")
					strSQL = InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("FUNC_SET_TO_FUNCS"));
				else
					strSQL = InnerCommon.GetDeleteSqlStr(xmlDoc, this.GetXSDDocument("FUNC_SET_TO_FUNCS"));
			}

			return strSQL;
		}
		/// <summary>
		/// ����ɫ�빦�ܼ��ϵĶ�Ӧ��ϵ�ı�ɽ�ɫ�빦�ܵĹ�ϵ
		/// </summary>
		/// <param name="xmlDocRTF">����ת����Ľڵ���ĵ�</param>
		/// <param name="nodeExchange">��Ҫ��RTFSת����RTF�Ľڵ�</param>
		/// <param name="dsFuncID">���ܼ��������������й���ID��DataSet</param>
		/// <param name="strOperator">��ִ�к��ֲ�����Insert����Delete��</param>
		private void ExchangeRTFSToRTF(XmlDocument xmlDocRTF, XmlNode nodeExchange, DataSet dsFuncID, string strOperator)
		{
			if (nodeExchange != null)
			{
				XmlNode nodeOperatorRTF = XmlHelper.AppendNode(xmlDocRTF.DocumentElement, strOperator, string.Empty);
				XmlNode node = nodeExchange.SelectSingleNode(".//ROLE_TO_FUNCTIONS");
				while (node != null)
				{
					string strRoleID = XmlHelper.GetSingleNodeValue<string>(node, ".//ROLE_ID", string.Empty);
					if (strRoleID != string.Empty)
					{
						foreach (DataRow dr in dsFuncID.Tables[0].Rows)
						{
							XmlNode nodeRTF = XmlHelper.AppendNode(nodeOperatorRTF, "ROLE_TO_FUNCTIONS");

							string strNodeName = string.Empty;
							if (strOperator == "Insert")
								strNodeName = "SET";
							else
								strNodeName = "WHERE";

							XmlNode nodeInnerRTF = XmlHelper.AppendNode(nodeRTF, strNodeName);

							XmlHelper.AppendNode(nodeInnerRTF, "ROLE_ID", strRoleID);
							XmlHelper.AppendNode(nodeInnerRTF, "FUNC_ID", dr["FUNC_ID"].ToString());
						}
					}
					node = node.NextSibling;
				}
			}
		}
		#endregion

		#region  ά��ί�ɲ���
		/// <summary>
		/// ά��ί�ɲ���
		/// </summary>
		/// <param name="dba"></param>
		/// <param name="xmlDoc"></param>
		/// <remarks>
		/// <code>
		/// <changeDelegateID sourceID="..." roleID=".." oldID=".." newID=".." START_TIME=".." END_TIME=".."></changeDelegateID>
		/// </code>
		/// </remarks>
		protected void DoChangeDelegateID(XmlDocument xmlDoc)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				XmlElement root = xmlDoc.DocumentElement;

				string strSourceID = root.GetAttribute("sourceID");
				string strRoleID = root.GetAttribute("roleID");

				string strOldDelegateID = root.GetAttribute("oldID");
				string strNewDelegateID = root.GetAttribute("newID");

				string strStartTime = root.GetAttribute("START_TIME");
				string strEndTime = root.GetAttribute("END_TIME");

				string strSQL = string.Empty;

				if (strOldDelegateID == string.Empty)
				{
					string strValue = strSourceID + "," + strNewDelegateID + "," + strRoleID + "," + strStartTime + "," + strEndTime;
					strSQL = "INSERT INTO DELEGATIONS VALUES( " + InnerCommon.AddMulitStrWithQuotationMark(strValue) + " )";
				}
				else
				{
					string strSqlTargetID = string.Empty;
					if (strOldDelegateID != strNewDelegateID)
						strSqlTargetID = " ,TARGET_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strNewDelegateID, true);

					strSQL = "UPDATE DELEGATIONS SET START_TIME = " + TSqlBuilder.Instance.CheckQuotationMark(strStartTime, true)
						+ ", END_TIME = " + TSqlBuilder.Instance.CheckQuotationMark(strEndTime, true)
						+ strSqlTargetID
						+ " WHERE SOURCE_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strSourceID, true)
						+ " AND ROLE_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true);
				}

				int nRow = InnerCommon.ExecuteNonQuery(strSQL);
				ExceptionHelper.TrueThrow<ApplicationException>(nRow == 0, "ί�ɲ���ʧ��");
				scope.Complete();
			}
		}
		#endregion

		#region �ڲ�˽���Ӻ���
		/// <summary>
		/// �����־��¼
		/// </summary>
		/// <param name="strAppName"></param>
		/// <param name="strOpType">��������</param>
		/// <param name="strExplain"></param>
		/// <param name="strOriginalData"></param>
		private void InsertLog(string strAppName, string strOpType, string strExplain, string strOriginalData)
		{
			UserDataWrite.InsertUserLog(strAppName, strOpType, strExplain, strOriginalData);
		}
		/// <summary>
		/// �����־��¼
		/// </summary>
		/// <param name="strAppName"></param>
		/// <param name="lot">��־ϵͳ�еĹ�������</param>
		/// <param name="strExplain"></param>
		/// <param name="strOriginalData"></param>
		private void InsertLog(string strAppName, LogOpType lot, string strExplain, string strOriginalData)
		{
			InsertLog(strAppName, lot.ToString(), strExplain, strOriginalData);
		}
		/// <summary>
		/// Ȩ�޼��
		/// </summary>
		/// <param name="strUserID">�û���GUID</param>
		/// <param name="strAppID">Ӧ�õ�ID</param>
		/// <param name="strFuncName">���ܵ�CODE_NAME</param>
		/// <param name="strFuncMsg">��������</param>
		private void CheckPermission(string strAppID, string strFuncName, string strFuncMsg)
		{
			ExceptionHelper.FalseThrow<ApplicationException>(
				SecurityCheck.DoesUserHasPermissions(this.LogOnUserInfo.UserGuid, new Guid(strAppID), strFuncName, UserValueType.Guid),
				"û��" + strFuncMsg + "��Ȩ��");
		}
		private void CheckPermission(string strAppID, FunctionNames funcName, string strFuncMsg)
		{
			ExceptionHelper.FalseThrow<ApplicationException>(
				SecurityCheck.DoesUserHasPermissions(this.LogOnUserInfo.UserGuid, new Guid(strAppID), funcName.ToString(), UserValueType.Guid),
				"û��" + strFuncMsg + "��Ȩ��");
		}
		/// <summary>
		/// Ȩ�޼�飬���strAppIDΪ�գ������strTableName��ID��ѯstrAppID���ж�Ȩ��
		/// </summary>
		/// <param name="strUserID">�û���GUID</param>
		/// <param name="strAppID">Ӧ�õ�ID</param>
		/// <param name="strFuncName">���ܵ�CODE_NAME</param>
		/// <param name="strFuncMsg">��������</param>
		/// <param name="strTableName">������</param>
		/// <param name="strID">ID</param>
		private void CheckPermission(string strAppID, string strFuncName, string strFuncMsg, string strTableName, string strID)
		{
			if (strAppID == string.Empty)
			{
				string strSQL = "SELECT APP_ID FROM " + strTableName
					+ " WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strID, true);
				DataSet ds = InnerCommon.ExecuteDataset(strSQL);
				if (ds.Tables[0].Rows.Count != 0)
					strAppID = ds.Tables[0].Rows[0]["APP_ID"].ToString();
			}

			CheckPermission(strAppID, strFuncName, strFuncMsg);
		}
		/// <summary>
		/// Ȩ�޼�飬���strAppIDΪ�գ������strTableName��ID��ѯstrAppID���ж�Ȩ��
		/// </summary>
		/// <param name="strUserID">�û���GUID</param>
		/// <param name="strAppID">Ӧ�õ�ID</param>
		/// <param name="funcName">���ܱ�־</param>
		/// <param name="strFuncMsg">��������</param>
		/// <param name="strTableName">������</param>
		/// <param name="strID"></param>
		private void CheckPermission(string strAppID, FunctionNames funcName, string strFuncMsg, string strTableName, string strID)
		{
			if (strAppID == string.Empty)
			{
				string strSQL = "SELECT APP_ID FROM " + TSqlBuilder.Instance.CheckQuotationMark(strTableName, false)
					+ " WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strID, true);

				DataSet ds = InnerCommon.ExecuteDataset(strSQL);
				if (ds.Tables[0].Rows.Count != 0)
					strAppID = ds.Tables[0].Rows[0]["APP_ID"].ToString();
			}

			CheckPermission(strAppID, funcName, strFuncMsg);
		}
		/// <summary>
		/// �жϽ�ɫ���ܹ�ϵ�Ƿ��Ǽ̳еģ�ֻҪ��ɫ�͹��ܶ��Ǽ̳еı�ʾRTF�Ǽ̳еģ������޸�
		/// </summary>
		/// <param name="strRoleID"></param>
		/// <param name="strFuncID"></param>
		/// <returns></returns>
		private bool IsInheritedRTF(string strRoleID, string strFuncID)
		{
			string strSQL = "SELECT INHERITED FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true)
				+ " SELECT INHERITED FROM FUNCTIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true);
			DataSet ds = InnerCommon.ExecuteDataset(strSQL);

			return ((ds.Tables[0].Rows[0]["INHERITED"].ToString() == "y") && (ds.Tables[1].Rows[0]["INHERITED"].ToString() == "y"));
		}
		/// <summary>
		/// ���ĵ�xmlDoc�в������Ե�ֵ
		/// </summary>
		/// <param name="xmlDoc">��Ҫ������ĵ�</param>
		/// <param name="attrName">��Ҫ���ҵ����Ե�����</param>
		/// <param name="strDefault">����������ʱ��Ĭ��ֵ</param>
		/// <returns></returns>
		private string GetAttr(XmlDocument xmlDoc, string attrName, string strDefault)
		{
			XmlElement root = xmlDoc.DocumentElement;

			string strDelSubApp = strDefault; //"n";Ĭ�ϲ�ɾ����Ӧ�ü̳еĹ��ܼ���			
			if (root.HasAttribute(attrName))
				strDelSubApp = root.GetAttribute(attrName);

			return strDelSubApp;
		}
		/// <summary>
		/// �ж϶��󣨽�ɫ�����ܣ����ϣ��Ƿ��Ǽ̳е�
		/// </summary>
		/// <param name="dba">���Ӷ���</param>
		/// <param name="strTableName">��������ݿ����</param>
		/// <param name="strObjID">�����ID</param>
		/// <returns></returns>
		private bool IsInheritedObject(string strTableName, string strObjID)
		{
			bool flag = false;

			string strSQL = "SELECT INHERITED FROM " + TSqlBuilder.Instance.CheckQuotationMark(strTableName, false)
				+ " WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strObjID, true);

			string strInherited = InnerCommon.ExecuteScalar(strSQL).ToString();

			//�жϽ�ɫ�Ƿ��Ǽ̳е�,(n,����)				
			//			if (ds.Tables[0].Rows[0]["INHERITED"].ToString() == "y")
			if (strInherited == "y")
				flag = true;

			return flag;
		}
		/// <summary>
		/// ��ù��ܼ��ϵ�RESOURCE_LEVEL�ֶε�ֵ
		/// </summary>
		/// <param name="strParentResLevel">�����ϵ�resource_level��ֵ</param>
		/// <param name="strSortID">��ǰ�����ڸ������е��������</param>
		/// <returns></returns>
		private string GetFuncSetResLevel(string strParentResLevel, string strSortID)
		{
			string strResLevel = strParentResLevel;
			if (strSortID.Length == 1)
				strResLevel += "00" + strSortID;
			else if (strSortID.Length == 2)
				strResLevel += "0" + strSortID;
			else if (strSortID.Length == 3)
				strResLevel += strSortID;

			return strResLevel;
		}
		/// <summary>
		/// ��ѯ��Ҫ��sort_id ��Ӧ�õ�resource_level�������ڽ�ɫ�͹���
		/// </summary>
		/// <param name="dba"></param>
		/// <param name="tableName">����ı���</param>
		/// <param name="appGuid">Ӧ�õ�ID</param>
		/// <param name="strSortID">��Ҫ���صĶ�������</param>
		/// <param name="strResourceLevel">��Ҫ���ص�Ӧ�õ�resouce_level��ֵ</param>
		private void GetSortIDAndResLevel(string tableName, string appGuid, out string strSortID, out string strResourceLevel)
		{
			string sql = "SELECT ISNULL(MAX(SORT_ID), 0)+1 AS SORT_ID FROM " + TSqlBuilder.Instance.CheckQuotationMark(tableName, false)
				+ " WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(appGuid, true) + ";" + Environment.NewLine
				+ "SELECT RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(appGuid, true);
			DataSet result = InnerCommon.ExecuteDataset(sql);
			strSortID = result.Tables[0].Rows[0][0].ToString();
			strResourceLevel = result.Tables[1].Rows[0][0].ToString();
		}
		/*
		/// <summary>
		/// �������в�ѯ�м̳е���Ӧ�ã�����id,code_name,resource_level
		/// </summary>
		/// <param name="dba"></param>
		/// <param name="strAppResLevel">��ǰӦ�õ�resource_level</param>
		/// <param name="nInherited">���������</param>
		/// <returns>�м̳е���Ӧ�õ�id��code_name,resource_level</returns>
		private DataSet GetSubAppID(string strAppResLevel, int nInherited)
		{				
			string strSQL = GetSubAppIDSql();

			SqlParameter sParam = new SqlParameter("@RESOURCE_LEVEL", strAppResLevel);
			SqlParameter sParam2 = new SqlParameter("@INHERITED", nInherited);
			DataSet ds = InnerCommon.ExecuteDataset(CommandType.Text, strSQL, sParam, sParam2);
		
			return ds;
		}
		*/

		/// <summary>
		/// ��ͨ�Ĳ�ѯ�м̳е���Ӧ��
		/// </summary>
		/// <param name="dba"></param>
		/// <param name="strAppResLevel">��ǰӦ�õ�resource_level</param>
		/// <param name="nInherited">���������</param>
		/// <returns>�м̳е���Ӧ�õ�id��code_name,resource_level</returns>
		private DataSet GetSubAppID(string strAppResLevel, int nInherited)
		{
			using (DbContext context = DbContext.GetContext(AppResource.ConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				string strSQL = GetSubAppIDSql();
				DbCommand command = database.GetSqlStringCommand(strSQL);
				database.AddInParameter(command, "RESOURCE_LEVEL", DbType.String, strAppResLevel);
				database.AddInParameter(command, "INHERITED", DbType.Int32, nInherited);
				return database.ExecuteDataSet(command);
			}
		}
		private string GetSubAppIDSql()
		{
			string strSQL = @"
SELECT ID, CODE_NAME, RESOURCE_LEVEL INTO #TEMPAPP 
FROM APPLICATIONS 
WHERE RESOURCE_LEVEL LIKE @RESOURCE_LEVEL + '%' 
	AND ((INHERITED_STATE & @INHERITED ) = @INHERITED ) 
	AND RESOURCE_LEVEL != @RESOURCE_LEVEL 

DECLARE SUBAPP_CURSOR CURSOR FOR
	SELECT ID,RESOURCE_LEVEL 
	FROM APPLICATIONS 
	WHERE RESOURCE_LEVEL LIKE @RESOURCE_LEVEL + '%' 
		AND ((INHERITED_STATE & @INHERITED ) = 0) 
		AND RESOURCE_LEVEL != @RESOURCE_LEVEL 

DECLARE @ID NVARCHAR(36),@RESOURCE_LEVEL_NEW NVARCHAR(32)

OPEN SUBAPP_CURSOR

FETCH NEXT FROM SUBAPP_CURSOR INTO @ID,@RESOURCE_LEVEL_NEW

WHILE @@FETCH_STATUS = 0
BEGIN
	DELETE FROM #TEMPAPP WHERE RESOURCE_LEVEL LIKE @RESOURCE_LEVEL_NEW + '%'
	
	FETCH NEXT FROM SUBAPP_CURSOR INTO @ID,@RESOURCE_LEVEL_NEW 
END

CLOSE SUBAPP_CURSOR
DEALLOCATE SUBAPP_CURSOR

SELECT ID AS APP_ID,CODE_NAME,RESOURCE_LEVEL FROM #TEMPAPP";

			return strSQL;
		}
		#region ��ʱδ��
		/// <summary>
		/// ��ѯ�����ܼ��ϵ�resource_level
		/// </summary>
		/// <param name="dba"></param>
		/// <param name="strParentGuid">��Ҫ��ѯ�Ĺ��ܼ��ϵ�ID</param>
		/// <returns></returns>
		//private string GetParentLevel(string strParentGuid)
		//{
		//    string strSQL = "SELECT RESOURCE_LEVEL FROM FUNCTION_SET WHERE ID = "
		//        + TSqlBuilder.Instance.CheckQuotationMark(strParentGuid, true);
		//    return InnerCommon.ExecuteScalar(strSQL).ToString();
		//}
		#endregion

		private bool IsSelfAccreditRole(string strRoleID)
		{
			return IsSelfAccreditObject(strRoleID, "ROLES");
		}

		private bool IsSelfAccreditFunction(string strFuncID)
		{
			return IsSelfAccreditObject(strFuncID, "FUNCTIONS");

		}

		private bool IsSelfAccreditFunctionSet(string strFuncSetID)
		{
			return IsSelfAccreditObject(strFuncSetID, "FUNCTION_SETS");
		}

		private bool IsSelfAccreditObject(string strID, string tableName)
		{
			string strSQL = "SELECT CODE_NAME FROM " + TSqlBuilder.Instance.CheckQuotationMark(tableName, false)
				+ " WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strID, true);

			string strCodeName = InnerCommon.ExecuteScalar(strSQL).ToString();

			XmlDocument xmlDoc = this.GetXMLDocument(@"../XML/preDefineObj");

			XmlNodeList nodeList = xmlDoc.SelectNodes(".//" + tableName);

			bool flag = false;
			int index = 0;

			while ((index < nodeList.Count) && (flag == false))
			{
				if (nodeList[index].SelectSingleNode(".//CODE_NAME").InnerText == strCodeName)
					flag = true;

				index++;
			}

			return flag;
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
