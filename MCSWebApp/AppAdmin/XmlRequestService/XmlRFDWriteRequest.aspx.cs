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
	/// XmlWriteRequest 的摘要说明。
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
							strMessage = "英文标识重复。在角色和功能中，每一个应用程序内的英文标识必须唯一；而在应用程序中，英文标识必须全局唯一";
						if (ex.Message.IndexOf("USERS_TO_ROLES") >= 0)
							strMessage = "授权重复，相同角色中不允许有相同被授权对象！";
						break;
					case 2601:
						strMessage = "关键字冲突，您要添加的内容在数据库中已经存在";
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

		#region 添加角色，功能，功能集合
		/// <summary>
		/// 插入角色,一次添加一个角色
		/// 传入数据的格式:
		/// <Insert>
		///		<ROLES>
		///			<SET>
		///				<APP_ID>..</APP_ID>
		///				<NAME></NAME>
		///				<CODE_NAME></CODE_NAME>
		///				<DESCRIPTION></DESCRIPTION>
		///				<CLASSIFY>..</CLASSIFY>
		///				<ALLOW_DELEGATE></ALLOW_DELEGATE>
		///				<INHERITED>节点不存在时默认不继承</INHERITED>
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

				//检查权限
				CheckPermission(strAppID, FunctionNames.ADD_ROLE_FUNC, "添加角色");

				AppendInsertNodes(xmlDoc, nodeSet);

				string strSortID = string.Empty;
				string strAppResLevel = string.Empty;
				this.GetSortIDAndResLevel("ROLES", strAppID, out strSortID, out strAppResLevel);
				XmlHelper.AppendNode(nodeSet, "SORT_ID", strSortID);

				InnerCommon.ExecuteNonQuery(InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("ROLES")));
				//查询有继承的子应用的guid
				DataSet dsSubApp = this.GetSubAppID(strAppResLevel, nInheritedState);
				InsertSubAppRoleOrFunc("ROLES", xmlDoc, strAppResLevel, nInheritedState);

				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.INSERT_ROLE, AppResource.InsertRoleExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// 在应用下加基本功能，不涉及功能集合,一次添加一个功能
		/// <Insert>
		///		<FUNCTIONS>
		///			<SET>
		///				<APP_ID></APP_ID>
		///				<NAME></NAME>
		///				<CODE_NAME></CODE_NAME>
		///				<DESCRIPTION></DESCRIPTION>
		///				<INHERITED>节点不存在时默认不继承</INHERITED>
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

				//检查权限
				CheckPermission(strAppID, FunctionNames.ADD_FUNCTION_FUNC, "添加功能");
				AppendInsertNodes(xmlDoc, nodeSet);

				string strSortID = string.Empty;
				string strAppResLevel = string.Empty;
				this.GetSortIDAndResLevel("FUNCTIONS", strAppID, out strSortID, out strAppResLevel);
				XmlHelper.AppendNode(nodeSet, "SORT_ID", strSortID);

				InnerCommon.ExecuteNonQuery(InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("FUNCTIONS")));

				//查询有继承的子应用的guid
				InsertSubAppRoleOrFunc("FUNCTIONS", xmlDoc, strAppResLevel, nInheritedState);
				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.INSERT_FUNCTION, AppResource.InsertFuncExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// 插入功能集合，一次只能添加一个功能集合
		/// 传入文档的格式：
		/// <Insert>
		///		<FUNCTION_SETS>
		///			<SET>
		///				<APP_ID></APP_ID>
		///				<NAME></NAME>
		///				<CODE_NAME></CODE_NAME>
		///				<DESCRIPTION></DESCRIPTION>
		///				<RESOURCE_LEVEL>父集合的reslevel,第一级为空</RESOURCE_LEVEL>
		///				<LOWEST_SET></LOWEST_SET>
		///				<INHERITED>没有该节点时默认不继承</INHERITED>
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

				//检查权限
				CheckPermission(strAppID, FunctionNames.ADD_FUNCTION_FUNC, "添加功能集合");

				string strParentResLevel = XmlHelper.GetSingleNodeValue<string>(nodeSet, "RESOURCE_LEVEL", string.Empty);//父集合的resource_level			
				AppendInsertNodes(xmlDoc, nodeSet);
				//如果存在父集合，判断父集合的lowest_set字段是否是"n"，否则抛出异常
				ExceptionHelper.TrueThrow<ApplicationException>(CanParentFuncSetHasFuncSets(strAppID, strParentResLevel),
					"父集合与功能直接关联，不能再添加子集合");

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

				//插入FunctionSet					
				InnerCommon.ExecuteNonQuery(InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("FUNCTION_SETS")));

				//查询有继承的子应用的guid
				InsertSubAppFuncSet(xmlDoc, strParentResLevel, nInheritedState, strAppResLevel);

				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.INSERT_FUNCTION, AppResource.InsertFuncExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// 插入委派,需要参数source_id,target_id, role_id, start_time, end_time 
		/// 传入的数据格式
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

				//如果角色允许委派
				ExceptionHelper.FalseThrow<ApplicationException>(strAllowDelegate == "y", "该角色不允许委派！");

				InnerCommon.ExecuteNonQuery(InnerCommon.GetInsertSqlStr(xmlDoc, this.GetXSDDocument("DELEGATES")));
				//ADD LOG
				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.INSERT_DELEGATION, AppResource.InsertDelegationExplain, xmlDoc.OuterXml);
		}
		#endregion

		#region 添加操作的子函数
		/// <summary>
		/// 向xmlDoc文档中增加ID、MODIFY_TIME节点以及节点的值
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
		/// 替换xmlDoc文档SET节点中的ID,APP_ID,SORT_ID,INHERITED,RESOURCE_LEVEL的节点值
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
		/// 替换xmlDoc文档SET节点中的ID,APP_ID,SORT_ID,INHERITED的节点值
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="strAppID"></param>
		/// <param name="strSortID"></param>
		private void ReplaceInsertNodes(XmlDocument xmlDoc, string strAppID, string strSortID)
		{
			ReplaceInsertNodes(xmlDoc, strAppID, strSortID, string.Empty, string.Empty);
		}
		/// <summary>
		/// 向有继承的子应用中添加继承的角色或功能
		/// </summary>
		/// <param name="strTableName">"ROLES"</param>
		/// <param name="xmlDoc"></param>
		/// <param name="strAppResLevel">应用的resource_level</param>
		/// <param name="nInheritedState">掩码的值</param>
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
						//record the rename code_name's app_id 继承时如果和以有的角色发生codeName冲突时不继承该角色
					}
				}
			}
		}
		/// <summary>
		/// 判断如果存在父集合，是否可以添加子集合，lowest_set字段是否是"n"，否则抛出异常
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
		/// 子应用继承功能集合
		/// </summary>
		/// <param name="strParentResLevel"></param>
		private void InsertSubAppFuncSet(XmlDocument xmlDoc, string strParentResLevel, int nInheritedState, string strAppResLevel)
		{
			DataSet dsSubApp = GetSubAppID(strAppResLevel, nInheritedState);

			if (dsSubApp.Tables[0].Rows.Count != 0)
			{
				if (strParentResLevel != string.Empty)//存在父集合时
				{
					#region 存在父集合时
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
							{//如果插入时与已有的集合产生冲突，则保留原有的集合
							}
						}
					}
					#endregion
				}
				else//没有父集合,即是第一级集合时
				{
					#region 没有父集合,即是第一级集合时
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
						{//如果插入时与已有的集合产生冲突，则保留原有的集合
						}
					}
					#endregion
				}
			}
		}

		#endregion

		#region 更新角色，功能，功能集合的属性
		/// <summary>
		/// 更新角色的属性,一次只能更新一个角色
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

				//检查权限
				CheckPermission(string.Empty, FunctionNames.MODIFY_ROLE_FUNC, "修改角色", "ROLES", strRoleID);

				XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
				XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

				ExceptionHelper.TrueThrow<ApplicationException>(IsInheritedObject("ROLES", strRoleID), "角色是继承的，不能修改其属性");

				string strSQL = "SELECT CODE_NAME FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true)
					+ " SELECT ID,RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
					+ " (SELECT APP_ID AS ID FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true) + ")";
				DataSet ds = InnerCommon.ExecuteDataset(strSQL);

				string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();
				string strAppID = ds.Tables[1].Rows[0]["ID"].ToString();
				string strAppResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();

				//判断继承字段是否被修改为继承
				CheckInheritedAttr("ROLES", nodeSet, strAppID, strCodeName, nInheritedState);

				//判断允许委派的字段是否被修改
				CheckAllowDelegateAttr(nodeSet, strRoleID);

				//更新角色
				strSQL = InnerCommon.GetUpdateSqlStr(xmlDoc, this.GetXSDDocument("ROLES"));
				InnerCommon.ExecuteNonQuery(strSQL);

				UpdateSubAppRoleOrFunc("ROLES", xmlDoc, nodeWhere, strAppResLevel, nInheritedState, strCodeName);
				scope.Complete();
			}
			//记录日志
			InsertLog(AppResource.AppCodeName, LogOpType.UPDATE_ROLE, AppResource.UpdateRoleExplain, xmlDoc.OuterXml);
		}

		/// <summary>
		///  更新功能属性,一次只能更新一个功能
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

				//检查权限
				CheckPermission(string.Empty, FunctionNames.MODIFY_FUNCTION_FUNC, "修改功能", "FUNCTIONS", strFuncID);

				XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

				ExceptionHelper.TrueThrow<ApplicationException>(IsInheritedObject("FUNCTIONS", strFuncID), "功能是继承的，不能修改其属性");

				string strSQL = "SELECT CODE_NAME FROM FUNCTIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true)
					+ " ; SELECT ID,RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
					+ " (SELECT APP_ID FROM FUNCTIONS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true) + ")";

				DataSet ds = InnerCommon.ExecuteDataset(strSQL);

				string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();
				string strAppID = ds.Tables[1].Rows[0]["ID"].ToString();
				string strAppResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();

				//判断继承字段是否被修改
				CheckInheritedAttr("FUNCTIONS", nodeSet, strAppID, strCodeName, nInheritedState);

				strSQL = InnerCommon.GetUpdateSqlStr(xmlDoc, this.GetXSDDocument("FUNCTIONS"));
				int nAffectRow = InnerCommon.ExecuteNonQuery(strSQL);


				UpdateSubAppRoleOrFunc("FUNCTIONS", xmlDoc, nodeWhere, strAppResLevel, nInheritedState, strCodeName);
				scope.Complete();
			}
			//记录日志
			InsertLog(AppResource.AppCodeName, LogOpType.UPDATE_FUNCTION, AppResource.UpdateFuncExplain, xmlDoc.OuterXml);
		}

		/// <summary>
		/// 更新功能集合的属性，一次只能更新一个集合
		/// 传入数据的格式
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

				//检查权限
				CheckPermission(string.Empty, FunctionNames.MODIFY_FUNCTION_FUNC, "修改功能集合", "FUNCTION_SETS", strFuncSetID);

				XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

				ExceptionHelper.TrueThrow<ApplicationException>(IsInheritedObject("FUNCTION_SETS", strFuncSetID), "功能集合是继承的，不能修改其属性");

				string strSQL = "SELECT CODE_NAME FROM FUNCTION_SETS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true);
				strSQL += "; SELECT ID,RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
					+ " (SELECT APP_ID FROM FUNCTION_SETS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + ")";

				DataSet ds = InnerCommon.ExecuteDataset(strSQL);

				string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();
				string strAppID = ds.Tables[1].Rows[0]["ID"].ToString();
				string strAppResLevel = ds.Tables[1].Rows[0]["RESOURCE_LEVEL"].ToString();

				//判断继承字段是否被修改
				CheckInheritedAttr("FUNCTION_SETS", nodeSet, strAppID, strCodeName, nInheritedState);

				//判断LOWEST_SET字段是否被修改，如果被修改为“y”要判断原来是否有子集合，“n”要判断原来是否包含功能
				CheckLowestSetAttr(nodeSet, strAppID, strFuncSetID);

				strSQL = InnerCommon.GetUpdateSqlStr(xmlDoc, this.GetXSDDocument("FUNCTION_SETS"));
				int nAffectRow = InnerCommon.ExecuteNonQuery(strSQL);


				UpdateSubAppRoleOrFunc("FUNCTION_SETS", xmlDoc, nodeWhere, strAppResLevel, nInheritedState, strCodeName);
				scope.Complete();
			}

			//记录日志
			InsertLog(AppResource.AppCodeName, LogOpType.UPDATE_FUNCTION, AppResource.UpdateFuncExplain, xmlDoc.OuterXml);
		}

		#endregion

		#region 更新操作的子函数

		/// <summary>
		/// 检查角色的ALLOW_DELEGATE字段是否被修改
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
		/// 检查Inherited字段是否被修改
		/// </summary>
		/// <param name="nodeSet">被修改字段的SET节点</param>
		/// <param name="strAppID">应用的ID</param>
		/// <param name="strCodeName">对象CODE_NAME值</param>
		/// <param name="nInheritedState">对象的掩码值</param>
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
							strObjName = "角色";
							break;
						case "FUNCTIONS":
							strObjName = "功能";
							break;
						case "FUNCTION_SETS":
							strObjName = "功能集合";
							break;
					}

					ExceptionHelper.TrueThrow<ApplicationException>(ds.Tables[0].Rows.Count == 0, "没有可以继承的" + strObjName);
					ExceptionHelper.TrueThrow<ApplicationException>((int.Parse(ds.Tables[1].Rows[0]["INHERITED_STATE"].ToString()) & nInheritedState) == 0,
						"该应用不继承上级应用的" + strObjName);
				}
			}
		}

		/// <summary>
		/// 判断功能集合的LOWEST_SET字段是否被修改并作相应处理
		/// </summary>
		/// <param name="nodeSet">被修改字段的SET节点</param>
		/// <param name="strAppID">应用的ID</param>
		/// <param name="strFuncSetID">功能集合的ID</param>
		private void CheckLowestSetAttr(XmlNode nodeSet, string strAppID, string strFuncSetID)
		{
			if (nodeSet.SelectSingleNode("LOWEST_SET") != null)
			{
				string strLowestSet = nodeSet.SelectSingleNode("LOWEST_SET").InnerText;
				if (strLowestSet == "y") //判断是否有子集合
				{
					string strSQL = "SELECT ID FROM FUNCTION_SETS WHERE APP_ID = " + TSqlBuilder.Instance.CheckQuotationMark(strAppID, true)
						+ " AND RESOURCE_LEVEL LIKE (SELECT RESOURCE_LEVEL FROM FUNCTION_SETS WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + ") + '%'"
						+ " AND ID <> " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true);

					DataSet ds = InnerCommon.ExecuteDataset(strSQL);

					ExceptionHelper.TrueThrow<ApplicationException>((ds.Tables[0].Rows.Count != 0), "该集合有子集合，不能修改与功能直接关联的复选框");
				}
				else //判断是否与基本功能有关联
				{
					string strSQL = "SELECT COUNT(*) AS COUNT FROM FUNC_SET_TO_FUNCS WHERE FUNC_SET_ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true);
					DataSet ds = InnerCommon.ExecuteDataset(strSQL);

					ExceptionHelper.TrueThrow<ApplicationException>((ds.Tables[0].Rows[0]["COUNT"].ToString() != "0"),
						"该集合包含有功能,不能修改与功能直接关联的复选框");
				}
			}
		}

		/// <summary>
		/// 更新有继承的子应用的数据(角色，功能，集合）
		/// </summary>
		/// <param name="strTableName">数据库的表名</param>
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

		#region 删除角色，功能，功能集合
		/// <summary>
		/// 删除角色，一次可以删除多个
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

				//检查权限
				CheckPermission(string.Empty, FunctionNames.DELETE_ROLE_FUNC, "删除角色", "ROLES",
					xmlDoc.DocumentElement.SelectSingleNode(".//ID").InnerText);

				//默认不删除子应用继承的角色
				string strDelSubApp = this.GetAttr(xmlDoc, "deleteSubApp", AppResource.DefaultDelSubApp);

				//如果角色是继承，是否确认删除该角色
				string strDelRole = AppResource.DefaultDelObj; //"n";			

				XmlNode nodeRole = xmlDoc.DocumentElement.SelectSingleNode("ROLES");

				while (nodeRole != null)
				{
					string strRoleID = nodeRole.SelectSingleNode(".//ID").InnerText;

					//判断删除的是否是自授权管理员，根据CODE_NAME
					ExceptionHelper.TrueThrow<ApplicationException>(IsSelfAccreditRole(strRoleID),
						"预定义的自授权管理员角色不能删除");

					string strSQL = "SELECT INHERITED,CODE_NAME FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true)
						+ " SELECT RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
						+ " (SELECT APP_ID AS ID FROM ROLES WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true) + ")";

					DataSet ds = InnerCommon.ExecuteDataset(strSQL);
					string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();

					//判断角色是否是继承的,若是继承的,是否确认删除(n,返回)				
					if (ds.Tables[0].Rows[0]["INHERITED"].ToString() == "y")
					{
						ExceptionHelper.TrueThrow<ApplicationException>((strDelRole == "n" || strDelRole == string.Empty),
							"该角色是继承的，不能删除！");
					}

					#region 如果有子应用继承,删除相应的角色或下一级的继承关系改为非继承
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
		/// 删除功能，一次可删除多个
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

				//检查权限
				CheckPermission(string.Empty, FunctionNames.DELETE_FUNCTION_FUNC, "删除功能", "FUNCTIONS",
					xmlDoc.DocumentElement.SelectSingleNode(".//ID").InnerText);

				//默认不删除子应用继承的功能
				string strDelSubApp = this.GetAttr(xmlDoc, "deleteSubApp", AppResource.DefaultDelSubApp);//"n"

				//如果该功能是继承,是否确认删除该功能
				string strDelFunc = AppResource.DefaultDelObj; //"y"; 


				XmlNode nodeFunction = xmlDoc.DocumentElement.SelectSingleNode(".//FUNCTIONS");

				while (nodeFunction != null)
				{
					string strFuncID = nodeFunction.SelectSingleNode(".//ID").InnerText;

					//判断删除的是否是自授权的功能，根据CODE_NAME
					ExceptionHelper.TrueThrow<ApplicationException>(IsSelfAccreditFunction(strFuncID), "预定义的自授权功能不能删除");

					string strSQL = "SELECT INHERITED,CODE_NAME FROM FUNCTIONS WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true)
						+ " SELECT RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
						+ " (SELECT APP_ID AS ID FROM FUNCTIONS WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncID, true) + ")";

					DataSet ds = InnerCommon.ExecuteDataset(strSQL);
					string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();

					// 判断功能是否是继承的; 若是继承的,是否确认删除(n,返回), 						
					if (ds.Tables[0].Rows[0]["INHERITED"].ToString() == "y")
					{
						ExceptionHelper.TrueThrow<ApplicationException>((strDelFunc == "n" || strDelFunc == string.Empty),
							"该功能是继承的，不能删除！");
					}

					#region 如果有子应用继承，则删除相应的功能,否则将下一级的继承关系改为非继承
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
		/// 删除功能集合，一次可删除多个
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

				//检查权限
				CheckPermission(string.Empty, FunctionNames.DELETE_FUNCTION_FUNC, "删除功能集合", "FUNCTION_SETS",
					xmlDoc.DocumentElement.SelectSingleNode(".//ID").InnerText);

				//默认不删除子应用继承的功能集合
				string strDelSubApp = this.GetAttr(xmlDoc, "deleteSubApp", AppResource.DefaultDelSubApp);//"n"

				//默认是确认删除该功能集合
				string strSureDel = AppResource.DefaultDelObj;//"y"; 

				XmlNode nodeFuncSet = xmlDoc.DocumentElement.SelectSingleNode(".//FUNCTION_SETS");

				while (nodeFuncSet != null)
				{
					string strFuncSetID = nodeFuncSet.SelectSingleNode(".//ID").InnerText;

					//判断是否自授权
					ExceptionHelper.TrueThrow<ApplicationException>(IsSelfAccreditFunctionSet(strFuncSetID), "预定义的自授权功能集合不能删除");

					string strSQL = "SELECT INHERITED,CODE_NAME,RESOURCE_LEVEL FROM FUNCTION_SETS WHERE ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true)
						+ " SELECT RESOURCE_LEVEL FROM APPLICATIONS WHERE ID = "
						+ " (SELECT APP_ID AS ID FROM FUNCTION_SETS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true) + ")";

					DataSet ds = InnerCommon.ExecuteDataset(strSQL);

					string strCodeName = ds.Tables[0].Rows[0]["CODE_NAME"].ToString();
					string strResLevel = ds.Tables[0].Rows[0]["RESOURCE_LEVEL"].ToString();

					//是否是继承,若是继承的,是否确认删除(n,返回),
					if (ds.Tables[0].Rows[0]["INHERITED"].ToString() == "y")
					{
						ExceptionHelper.TrueThrow<ApplicationException>((strSureDel == "n" || strSureDel == string.Empty), "该功能集合是继承的，不能删除！");
					}

					#region 删除所有子集合，删除与功能的对应,下一级继承改为非继承
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
		///// 删除委派
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

		#region 维护角色功能的关系，维护功能和功能集合的关系，RTF,RTFS,FSTF
		/// <summary>
		/// 修改角色功能关系
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

				//检查权限
				CheckPermission(string.Empty, FunctionNames.RTF_MAINTAIN_FUNC, "修改角色功能关系", "FUNCTIONS",
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
							//throw new Exception("角色功能关系是继承的，不能修改！");
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
							{//防止在插入功能集合时已经有基本功能与角色的关系已插入
							}

							#region 插入有继承的子应用的角色功能关系
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

									//如果子应用已经继承了角色和功能则自动继承角色功能关系，否则结束
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
								if (strDeleteSubApp == "y")//删除继承子应用的角色功能关系
								{
									#region //删除继承子应用的角色功能关系
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
								else //不删除继承子应用的角色功能关系,将下一级的继承改为不继承
								{
									#region 不删除继承子应用的角色功能关系,将下一级的继承改为不继承
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
				ExceptionHelper.TrueThrow<ApplicationException>(HasInheritedRTF, "角色功能关系是继承的，不能修改！");
				scope.Complete();
			}
			if (IsNeedLog)
				InsertLog(AppResource.AppCodeName, LogOpType.MODIFY_ROLE_TO_FUNC, AppResource.ModifyRoleToFuncExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// 修改角色和功能集合的关系
		/// 传入的xml文件格式:
		/// <RTFS  deleteSubApp="y/n">
		///		<Insert>
		///			<ROLE_TO_FUNCTIONS>
		///				<SET>
		///					<ROLE_ID>角色的ID</ROLE_ID>
		///					<FUNC_SET_ID>功能集合的ID</FUNC_SET_ID>
		///				</SET>
		///			</ROLE_TO_FUNCTIONS>
		///			...
		///		</Insert>
		///		<Delete>
		///			<ROLE_TO_FUNCTIONS>
		///				<WHERE>
		///					<ROLE_ID>角色的ID</ROLE_ID>
		///					<FUNC_SET_ID>功能集合的ID</FUNC_SET_ID>
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

				//检查权限
				CheckPermission(string.Empty, FunctionNames.RTF_MAINTAIN_FUNC, "修改角色功能关系", "FUNCTION_SETS", strFuncSetID);

				if (strFuncSetID != string.Empty)
				{
					string strSQL = "SELECT FUNC_ID FROM FUNC_SET_TO_FUNCS WHERE FUNC_SET_ID = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true);
					DataSet dsFuncID = InnerCommon.ExecuteDataset(strSQL);

					ExceptionHelper.TrueThrow(dsFuncID.Tables[0].Rows.Count == 0, "对不起，当前功能包中没有具体功能项，不能关联角色");

					XmlDocument xmlDocRTF = new XmlDocument();
					xmlDocRTF.LoadXml("<RTF />");

					string strDeleteSubApp = GetAttr(xmlDoc, "deleteSubApp", AppResource.DefaultDelSubApp); //GetAttr(xmlDoc, "deleteSubApp", "n");
					XmlHelper.AppendAttr(xmlDocRTF.DocumentElement, "deleteSubApp", strDeleteSubApp);

					if (dsFuncID.Tables[0].Rows.Count != 0)
					{
						XmlNode nodeInsert = xmlDoc.DocumentElement.SelectSingleNode(".//Insert");
						XmlNode nodeDelete = xmlDoc.DocumentElement.SelectSingleNode(".//Delete");

						//插入角色和基本功能的关联，将功能集合转换为功能
						this.ExchangeRTFSToRTF(xmlDocRTF, nodeInsert, dsFuncID, "Insert");

						//插入需要删除的角色和功能关系，将功能集合转化为功能
						this.ExchangeRTFSToRTF(xmlDocRTF, nodeDelete, dsFuncID, "Delete");

					}

					DoRTF(xmlDocRTF, false);
				}
				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.MODIFY_ROLE_TO_FUNC, AppResource.ModifyRoleToFuncExplain, xmlDoc.OuterXml);
		}
		/// <summary>
		/// 添加或删除功能集合和功能之间的关联，一个功能只包含于一个集合，由前端控制
		/// 功能和集合的关系不继承
		/// 传入的文件格式:
		/// <FSTF  deleteSubApp="y/n">
		///		<Insert>
		///			<FUNC_SET_TO_FUNCS>
		///				<SET>
		///					<FUNC_SET_ID>功能集合的ID</FUNC_SET_ID>
		///					<FUNC_ID>集合的ID</FUNC_ID>
		///				</SET>
		///			</FUNC_SET_TO_FUNCS>
		///			....
		///		</Insert>
		///		<Delete>
		///			<FUNC_SET_TO_FUNCS>
		///				<WHERE>
		///					<FUNC_SET_ID>功能集合的ID</FUNC_SET_ID>
		///					<FUNC_ID>集合的ID</FUNC_ID>
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
				//检查权限
				CheckPermission(string.Empty, FunctionNames.FSTF_MAINTAIN_FUNC, "修改功能-功能集合关系", "FUNCTION_SETS",
					xmlDoc.DocumentElement.SelectSingleNode(".//FUNC_SET_ID").InnerText);

				XmlNode nodeInsert = xmlDoc.DocumentElement.SelectSingleNode(".//Insert");
				XmlNode nodeDelete = xmlDoc.DocumentElement.SelectSingleNode(".//Delete");

				//判断功能集合的LOWEST_SET是否是"y",否则抛出异常
				string strFuncSetID = XmlHelper.GetSingleNodeValue<string>(xmlDoc, ".//FUNC_SET_ID", string.Empty);

				if (strFuncSetID != string.Empty)
				{
					string strSQL1 = @"SELECT LOWEST_SET FROM FUNCTION_SETS WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strFuncSetID, true);

					Object oLowestSet = InnerCommon.ExecuteScalar(strSQL1);

					ExceptionHelper.TrueThrow<ApplicationException>(oLowestSet != null && oLowestSet.ToString() != "y", "该功能集合不能和功能直接关联");
				}

				string strSQL = DoInnerFSTF(nodeInsert, "Insert") + " " + DoInnerFSTF(nodeDelete, "Delete");

				if (strSQL != " ")
					InnerCommon.ExecuteNonQuery(strSQL);
				scope.Complete();
			}
			InsertLog(AppResource.AppCodeName, LogOpType.UPDATE_FUNCTION, AppResource.UpdateFuncExplain, xmlDoc.OuterXml);
		}
		#endregion

		#region 维护角色功能关系，功能和集合关系的子函数
		/// <summary>
		/// 将节点node转换成XmlDocument文档
		/// </summary>
		/// <param name="node">需要转换的节点</param>
		/// <returns>转换后的xml文档</returns>
		private XmlDocument GetXmlDocument(XmlNode node)
		{
			XmlDocument xmlDoc = new XmlDocument();
			if (node != null)
				xmlDoc.LoadXml(node.OuterXml);

			return xmlDoc;
		}
		/// <summary>
		/// 由节点node生成SQL语句，strOperator是操作类型
		/// </summary>
		/// <param name="node">需要转换的节点</param>
		/// <param name="strOperator">执行的操作，Insert或Delete</param>
		/// <returns>生成的sql语句</returns>
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
		/// 将角色与功能集合的对应关系改变成角色与功能的关系
		/// </summary>
		/// <param name="xmlDocRTF">包含转换后的节点的文档</param>
		/// <param name="nodeExchange">需要将RTFS转换成RTF的节点</param>
		/// <param name="dsFuncID">功能集合所包含的所有功能ID的DataSet</param>
		/// <param name="strOperator">是执行何种操作（Insert还是Delete）</param>
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

		#region  维护委派操作
		/// <summary>
		/// 维护委派操作
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
				ExceptionHelper.TrueThrow<ApplicationException>(nRow == 0, "委派操作失败");
				scope.Complete();
			}
		}
		#endregion

		#region 内部私有子函数
		/// <summary>
		/// 添加日志记录
		/// </summary>
		/// <param name="strAppName"></param>
		/// <param name="strOpType">功能类型</param>
		/// <param name="strExplain"></param>
		/// <param name="strOriginalData"></param>
		private void InsertLog(string strAppName, string strOpType, string strExplain, string strOriginalData)
		{
			UserDataWrite.InsertUserLog(strAppName, strOpType, strExplain, strOriginalData);
		}
		/// <summary>
		/// 添加日志记录
		/// </summary>
		/// <param name="strAppName"></param>
		/// <param name="lot">日志系统中的功能类型</param>
		/// <param name="strExplain"></param>
		/// <param name="strOriginalData"></param>
		private void InsertLog(string strAppName, LogOpType lot, string strExplain, string strOriginalData)
		{
			InsertLog(strAppName, lot.ToString(), strExplain, strOriginalData);
		}
		/// <summary>
		/// 权限检查
		/// </summary>
		/// <param name="strUserID">用户的GUID</param>
		/// <param name="strAppID">应用的ID</param>
		/// <param name="strFuncName">功能的CODE_NAME</param>
		/// <param name="strFuncMsg">功能名称</param>
		private void CheckPermission(string strAppID, string strFuncName, string strFuncMsg)
		{
			ExceptionHelper.FalseThrow<ApplicationException>(
				SecurityCheck.DoesUserHasPermissions(this.LogOnUserInfo.UserGuid, new Guid(strAppID), strFuncName, UserValueType.Guid),
				"没有" + strFuncMsg + "的权限");
		}
		private void CheckPermission(string strAppID, FunctionNames funcName, string strFuncMsg)
		{
			ExceptionHelper.FalseThrow<ApplicationException>(
				SecurityCheck.DoesUserHasPermissions(this.LogOnUserInfo.UserGuid, new Guid(strAppID), funcName.ToString(), UserValueType.Guid),
				"没有" + strFuncMsg + "的权限");
		}
		/// <summary>
		/// 权限检查，如果strAppID为空，则根据strTableName的ID查询strAppID后判断权限
		/// </summary>
		/// <param name="strUserID">用户的GUID</param>
		/// <param name="strAppID">应用的ID</param>
		/// <param name="strFuncName">功能的CODE_NAME</param>
		/// <param name="strFuncMsg">功能名称</param>
		/// <param name="strTableName">表名称</param>
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
		/// 权限检查，如果strAppID为空，则根据strTableName的ID查询strAppID后判断权限
		/// </summary>
		/// <param name="strUserID">用户的GUID</param>
		/// <param name="strAppID">应用的ID</param>
		/// <param name="funcName">功能标志</param>
		/// <param name="strFuncMsg">功能名称</param>
		/// <param name="strTableName">表名称</param>
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
		/// 判断角色功能关系是否是继承的，只要角色和功能都是继承的表示RTF是继承的，不可修改
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
		/// 从文档xmlDoc中查找属性的值
		/// </summary>
		/// <param name="xmlDoc">需要处理的文档</param>
		/// <param name="attrName">需要查找的属性的名字</param>
		/// <param name="strDefault">不存在属性时的默认值</param>
		/// <returns></returns>
		private string GetAttr(XmlDocument xmlDoc, string attrName, string strDefault)
		{
			XmlElement root = xmlDoc.DocumentElement;

			string strDelSubApp = strDefault; //"n";默认不删除子应用继承的功能集合			
			if (root.HasAttribute(attrName))
				strDelSubApp = root.GetAttribute(attrName);

			return strDelSubApp;
		}
		/// <summary>
		/// 判断对象（角色，功能，集合）是否是继承的
		/// </summary>
		/// <param name="dba">连接对象</param>
		/// <param name="strTableName">对象的数据库表明</param>
		/// <param name="strObjID">对象的ID</param>
		/// <returns></returns>
		private bool IsInheritedObject(string strTableName, string strObjID)
		{
			bool flag = false;

			string strSQL = "SELECT INHERITED FROM " + TSqlBuilder.Instance.CheckQuotationMark(strTableName, false)
				+ " WHERE ID = " + TSqlBuilder.Instance.CheckQuotationMark(strObjID, true);

			string strInherited = InnerCommon.ExecuteScalar(strSQL).ToString();

			//判断角色是否是继承的,(n,返回)				
			//			if (ds.Tables[0].Rows[0]["INHERITED"].ToString() == "y")
			if (strInherited == "y")
				flag = true;

			return flag;
		}
		/// <summary>
		/// 获得功能集合的RESOURCE_LEVEL字段的值
		/// </summary>
		/// <param name="strParentResLevel">父集合的resource_level的值</param>
		/// <param name="strSortID">当前集合在父集合中的排序序号</param>
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
		/// 查询需要的sort_id 和应用的resource_level，适用于角色和功能
		/// </summary>
		/// <param name="dba"></param>
		/// <param name="tableName">对象的表名</param>
		/// <param name="appGuid">应用的ID</param>
		/// <param name="strSortID">需要返回的对象的序号</param>
		/// <param name="strResourceLevel">需要返回的应用的resouce_level的值</param>
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
		/// 在事务中查询有继承的子应用，返回id,code_name,resource_level
		/// </summary>
		/// <param name="dba"></param>
		/// <param name="strAppResLevel">当前应用的resource_level</param>
		/// <param name="nInherited">对象的掩码</param>
		/// <returns>有继承的子应用的id，code_name,resource_level</returns>
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
		/// 普通的查询有继承的子应用
		/// </summary>
		/// <param name="dba"></param>
		/// <param name="strAppResLevel">当前应用的resource_level</param>
		/// <param name="nInherited">对象的掩码</param>
		/// <returns>有继承的子应用的id，code_name,resource_level</returns>
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
		#region 暂时未用
		/// <summary>
		/// 查询父功能集合的resource_level
		/// </summary>
		/// <param name="dba"></param>
		/// <param name="strParentGuid">需要查询的功能集合的ID</param>
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
