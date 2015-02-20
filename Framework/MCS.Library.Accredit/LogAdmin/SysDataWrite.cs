using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Transactions;

using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using MCS.Library.Accredit.WebBase;

namespace MCS.Library.Accredit.LogAdmin
{
	/// <summary>
	/// 系统日志管理类
	/// </summary>
	public class SysDataWrite
	{
		#region 构造函数
		/// <summary>
		/// 构造函数
		/// </summary>
		public SysDataWrite()
		{
		}

		#endregion

		#region Public Function Define
		/// <summary>
		/// 写入系统日志
		/// </summary>
		/// <param name="killInfo">杀毒软件</param>
		/// <param name="status">是否登陆成功</param>
		/// <param name="description">系统说明</param>
		public static void InsertSysLog(string killInfo, string status, string description)
		{
			string strSql = GetInsertSysSql(killInfo, status, description);
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				CommonDefine.ExecuteNonQuery(strSql);
				scope.Complete();
			}
		}

		/// <summary>
		/// 写入系统日志
		/// </summary>
		/// <param name="userLogonName">用户登录名</param>
		/// <param name="winVer">windows版本</param>
		/// <param name="ieVer">ie版本</param>
		/// <param name="hostIP">客户端IP</param>
		/// <param name="hostName">客户端机器名</param>
		/// <param name="killInfo">杀毒软件</param>
		/// <param name="status">是否登陆成功</param>
		/// <param name="description">系统说明</param>
		public static void InsertSysLog(string userLogonName, 
			string winVer, 
			string ieVer, 
			string hostIP, 
			string hostName, 
			string killInfo, 
			string status, 
			string description)
		{
			string strSql = GetInsertSysSql2(userLogonName, winVer, ieVer, hostIP, hostName, killInfo, status, description);
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				CommonDefine.ExecuteNonQuery(strSql);
				scope.Complete();
			}
		}

		#endregion

		#region Private Function Define
		/// <summary>
		/// 取得填写日志的SQL语句
		/// </summary>
		/// <param name="killInfo">杀毒软件</param>
		/// <param name="status">是否登陆成功</param>
		/// <param name="description">系统说明</param>
		/// <returns></returns>
		private static string GetInsertSysSql(string killInfo, string status, string description)
		{
			string systemInfo = string.Empty;
			string winVer = string.Empty;
			string ieVer = string.Empty;

			if (GlobalInfo.HttpEnvironment != null)
			{
				systemInfo = GlobalInfo.HttpEnvironment.UserAgent;
			}
			char[] split = new char[1];
			split[0] = ';';
			if ((systemInfo != null) && (systemInfo.Length != 0))
			{
				string[] inforArr = systemInfo.Split(split, 3);
				winVer = inforArr[2];
				ieVer = inforArr[1];
			}

			string hostIP = GlobalInfo.HttpEnvironment.UserHostAddress;
			string hostName = GlobalInfo.HttpEnvironment.UserHostName;

			string userDisplayname = GlobalInfo.UserLogOnInfo.OuUsers[0].UserDisplayName;
			string userGuid = GlobalInfo.UserLogOnInfo.UserGuid;
			string userDistinctName = GlobalInfo.UserLogOnInfo.OuUsers[0].AllPathName;
			string userLogonName = GlobalInfo.UserLogOnInfo.UserLogOnName;

			InsertSqlClauseBuilder ic = new InsertSqlClauseBuilder();
			ic.AppendItem("USER_GUID", userGuid);
			ic.AppendItem("USER_DISPLAYNAME", userDisplayname);
			ic.AppendItem("USER_DISTINCTNAME", userDistinctName);
			ic.AppendItem("USER_LOGONNAME", userLogonName);
			ic.AppendItem("HOST_IP", hostIP);
			ic.AppendItem("HOST_NAME", hostName);
			ic.AppendItem("WINDOWS_VERSION", winVer);
			ic.AppendItem("IE_VERSION", ieVer);
			ic.AppendItem("KILL_VIRUS", killInfo);
			ic.AppendItem("STATUS", status);
			ic.AppendItem("DESCRIPTION", description);

			return " INSERT INTO SYS_USER_LOGON " + ic.ToSqlString(TSqlBuilder.Instance);
		}

		/// <summary>
		/// 取得填写日志的SQL语句
		/// </summary>
		/// <param name="userLogonName">用户登录名</param>
		/// <param name="winVer">windows版本</param>
		/// <param name="ieVer">IE版本</param>
		/// <param name="hostIP">客户端IP地址</param>
		/// <param name="hostName">客户端机器名</param>
		/// <param name="killInfo">杀毒软件</param>
		/// <param name="status">是否登陆成功</param>
		/// <param name="description">系统说明</param>
		/// <returns>INSERT INTO SYS_USER_LOGON + WHERE ......</returns>
		private static string GetInsertSysSql2(string userLogonName, 
			string winVer, 
			string ieVer, 
			string hostIP,
			string hostName, 
			string killInfo, 
			string status, 
			string description)
		{
			DataRow row = OGUReader.GetObjectsDetail("USERS", 
				userLogonName, 
				SearchObjectColumn.SEARCH_LOGON_NAME).Tables[0].Rows[0];

			string strUserDisplayname = (string)row["DISPLAY_NAME"];
			string strUserGuid = (string)row["USER_GUID"];
			string strUserAllPathName = (string)row["ALL_PATH_NAME"];

			InsertSqlClauseBuilder ic = new InsertSqlClauseBuilder();
			ic.AppendItem("USER_GUID", strUserGuid);
			ic.AppendItem("USER_DISPLAYNAME", strUserDisplayname);
			ic.AppendItem("USER_DISTINCTNAME", strUserAllPathName);
			ic.AppendItem("USER_LOGONNAME", userLogonName);
			ic.AppendItem("HOST_IP", hostIP);
			ic.AppendItem("HOST_NAME", hostName);
			ic.AppendItem("WINDOWS_VERSION", winVer);
			ic.AppendItem("IE_VERSION", ieVer);
			ic.AppendItem("KILL_VIRUS", killInfo);
			ic.AppendItem("STATUS", status);
			ic.AppendItem("DESCRIPTION", description);

			return " INSERT INTO SYS_USER_LOGON " + ic.ToSqlString(TSqlBuilder.Instance);
		}
		#endregion
	}
}
