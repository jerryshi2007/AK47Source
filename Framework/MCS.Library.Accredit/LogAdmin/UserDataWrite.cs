using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Transactions;

using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.WebBase;

namespace MCS.Library.Accredit.LogAdmin
{
	/// <summary>
	/// 用户日志记录器
	/// </summary>
	public class UserDataWrite
	{
		#region 构造函数
		/// <summary>
		/// 构造函数
		/// </summary>
		public UserDataWrite()
		{
		}

		#endregion

		#region Public Function Define
		/// <summary>
		/// 获取对应数据的标识
		/// </summary>
		/// <param name="strAppName">应用程序英文标识</param>
		/// <param name="strOpType">操作类型英文标识</param>
		/// <param name="strColName">设置的字段名称</param>
		/// <returns>对应数据的标识</returns>
		public static string GetGuidByType(string strAppName, 
			string strOpType, 
			string strColName)
		{
			string strSql = @"
					SELECT GUID 
					FROM APP_LOG_TYPE 
					WHERE " + TSqlBuilder.Instance.CheckQuotationMark(strColName, false)
							+ " = " + TSqlBuilder.Instance.CheckQuotationMark(strAppName, true);

			if (strOpType != "")
				strSql = @"
					SELECT APP_OPERATION_TYPE.GUID 
					FROM APP_OPERATION_TYPE, APP_LOG_TYPE 
					WHERE APP_OPERATION_TYPE.CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strOpType, true) + @" 
						AND APP_LOG_TYPE." + TSqlBuilder.Instance.CheckQuotationMark(strColName, false)
										   + " = " + TSqlBuilder.Instance.CheckQuotationMark(strAppName, true) + @" 
						AND APP_OPERATION_TYPE.APP_GUID = APP_LOG_TYPE.GUID";

			return CommonDefine.ExecuteScalar(strSql).ToString();
		}

		/// <summary>
		/// 向日志表中插入数据
		/// </summary>
		/// <param name="userLogonName">用户登录名</param>
		/// <param name="strAppname">应用程序英文标识</param>
		/// <param name="strHostIP">客户端IP</param>
		/// <param name="strHostName">客户端机器名</param>
		/// <param name="strUrl">用户访问URL</param>
		/// <param name="strGoalID">跟踪操作的英文标识</param>
		/// <param name="strGoalName">跟踪操作的中文标识</param>
		/// <param name="strGoalDisplayName">跟踪操作的中文解释</param>
		/// <param name="strOpType">操作类型英文标识</param>
		/// <param name="strExplain">操作的详细说明</param>
		/// <param name="strOriginalData">操作的原始数据</param>
		/// <param name="bOpSucceed">该操作是否成功</param>
		public static void InsertUserLog(string userLogonName, 
			string strAppname, 
			string strHostIP, 
			string strHostName, 
			string strUrl, 
			string strGoalID, 
			string strGoalName, 
			string strGoalDisplayName, 
			string strOpType, 
			string strExplain, 
			string strOriginalData, 
			bool bOpSucceed)
		{
			string strSql = GetInsertUserSql2(userLogonName, strAppname, strHostIP, strHostName, strUrl, strGoalID, strGoalName, strGoalDisplayName, strOpType, strExplain, strOriginalData, bOpSucceed);
			if (false==string.IsNullOrEmpty(strSql))
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					CommonDefine.ExecuteNonQuery(strSql);
					scope.Complete();
				}
			}
		}

		/// <summary>
		/// 向日志表中插入数据
		/// </summary>
		/// <param name="strAppName">应用程序英文标识</param>
		/// <param name="strOpType">操作类型英文标识</param>
		/// <param name="strExplain">操作的详细说明</param>
		/// <param name="strOriginalData">操作的原始数据</param>
		public static void InsertUserLog(string strAppName, 
			string strOpType, 
			string strExplain,
			string strOriginalData)
		{
			InsertUserLog(strAppName, strOpType, strExplain, strOriginalData, true);
		}

		/// <summary>
		/// 向日志表中插入数据
		/// </summary>
		/// <param name="strAppName">应用程序英文标识</param>
		/// <param name="strOpType">操作类型英文标识</param>
		/// <param name="strExplain">操作的详细说明</param>
		/// <param name="strOriginalData">操作的原始数据</param>
		/// <param name="bOpSucced">该操作是否成功</param>
		public static void InsertUserLog(string strAppName, 
			string strOpType, 
			string strExplain, 
			string strOriginalData, 
			bool bOpSucced)
		{
			InsertUserLog(strAppName, "", "", "", strOpType, strExplain, strOriginalData, bOpSucced);
		}

		/// <summary>
		/// 向日志表中插入数据
		/// </summary>
		/// <param name="strAppName">应用程序英文标识</param>
		/// <param name="strGoalID">跟踪操作的英文标识</param>
		/// <param name="strGoalName">跟踪操作的中文标识</param>
		/// <param name="strGoalDisplayName">跟踪操作的中文解释</param>
		/// <param name="strOpType">操作类型英文标志</param>
		/// <param name="strExplain">操作的详细说明</param>
		/// <param name="strOriginalData">操作的原始数据</param>
		/// <param name="bOpSucceed">该操作是否成功</param>
		public static void InsertUserLog(string strAppName, 
			string strGoalID,
			string strGoalName, 
			string strGoalDisplayName, 
			string strOpType,
			string strExplain,
			string strOriginalData,
			bool bOpSucceed)
		{
			string strSql = GetInsertUserSql(strAppName, strGoalID, strGoalName, strGoalDisplayName, strOpType, strExplain, strOriginalData, bOpSucceed);
			if (false==string.IsNullOrEmpty(strSql))
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					CommonDefine.ExecuteNonQuery(strSql);
					scope.Complete();
				}
			}
		}

		#endregion

		#region Private Function Define
		//取得填写日志的SQL语句
		private static string GetInsertUserSql(string strAppName,
			string strGoalID, 
			string strGoalName,
			string strGoalDisplayName, 
			string strOpType,
			string strExplain,
			string strOriginalData, 
			bool bOpSucceed)
		{
			string strUserDisplayname = GlobalInfo.UserLogOnInfo.OuUsers[0].UserDisplayName;
			string strUserGuid = GlobalInfo.UserLogOnInfo.UserGuid;
			string strUserAllPathName = GlobalInfo.UserLogOnInfo.OuUsers[0].AllPathName;
			string strUserLogonName = GlobalInfo.UserLogOnInfo.UserLogOnName;

			string strHostIP = GlobalInfo.HttpEnvironment.UserHostAddress;
			string strHostName = GlobalInfo.HttpEnvironment.UserHostName;
			string strUrl = GlobalInfo.HttpEnvironment.Url.ToString();

			InsertSqlClauseBuilder ic = new InsertSqlClauseBuilder();
			ic.AppendItem("OP_USER_DISPLAYNAME", strUserDisplayname);
			ic.AppendItem("OP_USER_GUID", strUserGuid);
			ic.AppendItem("OP_USER_DISTINCTNAME", strUserAllPathName);
			ic.AppendItem("OP_USER_LOGONNAME", strUserLogonName);
			ic.AppendItem("HOST_IP", strHostIP);
			ic.AppendItem("HOST_NAME", strHostName);
			ic.AppendItem("APP_GUID", GetGuidByType(strAppName, "", "CODE_NAME"));
			ic.AppendItem("OP_URL", strUrl);
			ic.AppendItem("GOAL_ID", strGoalID);
			ic.AppendItem("GOAL_NAME", strGoalName);
			ic.AppendItem("GOAL_DISNAME", strGoalDisplayName);
			ic.AppendItem("OP_GUID", GetGuidByType(strAppName, strOpType, "CODE_NAME"));
			ic.AppendItem("GOAL_EXPLAIN", strExplain);
			ic.AppendItem("ORIGINAL_DATA", strOriginalData);
			ic.AppendItem("LOG_SUCCED", bOpSucceed ? "y" : "n");

			return " INSERT INTO USER_OPEATION_LOG " + ic.ToSqlString(TSqlBuilder.Instance);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userLogonName"></param>
		/// <param name="strAppName"></param>
		/// <param name="strHostIP"></param>
		/// <param name="strHostName"></param>
		/// <param name="strUrl"></param>
		/// <param name="strGoalID"></param>
		/// <param name="strGoalName"></param>
		/// <param name="strGoalDisplayName"></param>
		/// <param name="strOpType"></param>
		/// <param name="strExplain"></param>
		/// <param name="strOriginalData"></param>
		/// <param name="bOpSucceed"></param>
		/// <returns></returns>
		private static string GetInsertUserSql2(string userLogonName,
			string strAppName, 
			string strHostIP, 
			string strHostName, 
			string strUrl, 
			string strGoalID, 
			string strGoalName, 
			string strGoalDisplayName, 
			string strOpType, 
			string strExplain, 
			string strOriginalData, 
			bool bOpSucceed)
		{
			DataRow row = OGUReader.GetObjectsDetail("USERS", 
				userLogonName, 
				SearchObjectColumn.SEARCH_LOGON_NAME).Tables[0].Rows[0];

			string strUserDisplayname = (string)row["DISPLAY_NAME"];
			string strUserGuid = (string)row["USER_GUID"];
			string strUserAllPathName = (string)row["ALL_PATH_NAME"];

			InsertSqlClauseBuilder ic = new InsertSqlClauseBuilder();
			ic.AppendItem("OP_USER_DISPLAYNAME", strUserDisplayname);
			ic.AppendItem("OP_USER_GUID", strUserGuid);
			ic.AppendItem("OP_USER_DISTINCTNAME", strUserAllPathName);
			ic.AppendItem("OP_USER_LOGONNAME", userLogonName);
			ic.AppendItem("HOST_IP", strHostIP);
			ic.AppendItem("HOST_NAME", strHostName);
			ic.AppendItem("APP_GUID", GetGuidByType(strAppName, "", "CODE_NAME"));
			//ic.AppendItem("APP_GUID", strAppName);
			ic.AppendItem("OP_URL", strUrl);
			ic.AppendItem("GOAL_ID", strGoalID);
			ic.AppendItem("GOAL_NAME", strGoalName);
			ic.AppendItem("GOAL_DISNAME", strGoalDisplayName);
			ic.AppendItem("OP_GUID", GetGuidByType(strAppName, strOpType, "CODE_NAME"));
			//ic.AppendItem("OP_GUID", strAppName);
			ic.AppendItem("GOAL_EXPLAIN", strExplain);
			ic.AppendItem("ORIGINAL_DATA", strOriginalData);
			ic.AppendItem("LOG_SUCCED", bOpSucceed ? "y" : "n");

			return " INSERT INTO USER_OPEATION_LOG " + ic.ToSqlString(TSqlBuilder.Instance);
		}
		#endregion
	}
}
