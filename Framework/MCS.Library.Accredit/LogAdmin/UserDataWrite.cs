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
	/// �û���־��¼��
	/// </summary>
	public class UserDataWrite
	{
		#region ���캯��
		/// <summary>
		/// ���캯��
		/// </summary>
		public UserDataWrite()
		{
		}

		#endregion

		#region Public Function Define
		/// <summary>
		/// ��ȡ��Ӧ���ݵı�ʶ
		/// </summary>
		/// <param name="strAppName">Ӧ�ó���Ӣ�ı�ʶ</param>
		/// <param name="strOpType">��������Ӣ�ı�ʶ</param>
		/// <param name="strColName">���õ��ֶ�����</param>
		/// <returns>��Ӧ���ݵı�ʶ</returns>
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
		/// ����־���в�������
		/// </summary>
		/// <param name="userLogonName">�û���¼��</param>
		/// <param name="strAppname">Ӧ�ó���Ӣ�ı�ʶ</param>
		/// <param name="strHostIP">�ͻ���IP</param>
		/// <param name="strHostName">�ͻ��˻�����</param>
		/// <param name="strUrl">�û�����URL</param>
		/// <param name="strGoalID">���ٲ�����Ӣ�ı�ʶ</param>
		/// <param name="strGoalName">���ٲ��������ı�ʶ</param>
		/// <param name="strGoalDisplayName">���ٲ��������Ľ���</param>
		/// <param name="strOpType">��������Ӣ�ı�ʶ</param>
		/// <param name="strExplain">��������ϸ˵��</param>
		/// <param name="strOriginalData">������ԭʼ����</param>
		/// <param name="bOpSucceed">�ò����Ƿ�ɹ�</param>
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
		/// ����־���в�������
		/// </summary>
		/// <param name="strAppName">Ӧ�ó���Ӣ�ı�ʶ</param>
		/// <param name="strOpType">��������Ӣ�ı�ʶ</param>
		/// <param name="strExplain">��������ϸ˵��</param>
		/// <param name="strOriginalData">������ԭʼ����</param>
		public static void InsertUserLog(string strAppName, 
			string strOpType, 
			string strExplain,
			string strOriginalData)
		{
			InsertUserLog(strAppName, strOpType, strExplain, strOriginalData, true);
		}

		/// <summary>
		/// ����־���в�������
		/// </summary>
		/// <param name="strAppName">Ӧ�ó���Ӣ�ı�ʶ</param>
		/// <param name="strOpType">��������Ӣ�ı�ʶ</param>
		/// <param name="strExplain">��������ϸ˵��</param>
		/// <param name="strOriginalData">������ԭʼ����</param>
		/// <param name="bOpSucced">�ò����Ƿ�ɹ�</param>
		public static void InsertUserLog(string strAppName, 
			string strOpType, 
			string strExplain, 
			string strOriginalData, 
			bool bOpSucced)
		{
			InsertUserLog(strAppName, "", "", "", strOpType, strExplain, strOriginalData, bOpSucced);
		}

		/// <summary>
		/// ����־���в�������
		/// </summary>
		/// <param name="strAppName">Ӧ�ó���Ӣ�ı�ʶ</param>
		/// <param name="strGoalID">���ٲ�����Ӣ�ı�ʶ</param>
		/// <param name="strGoalName">���ٲ��������ı�ʶ</param>
		/// <param name="strGoalDisplayName">���ٲ��������Ľ���</param>
		/// <param name="strOpType">��������Ӣ�ı�־</param>
		/// <param name="strExplain">��������ϸ˵��</param>
		/// <param name="strOriginalData">������ԭʼ����</param>
		/// <param name="bOpSucceed">�ò����Ƿ�ɹ�</param>
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
		//ȡ����д��־��SQL���
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
