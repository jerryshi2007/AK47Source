using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Web;
using System.Data;
using System.Xml;
using System.IO;

using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.Properties;
using MCS.Library.Accredit.OguAdmin.Interfaces;
using MCS.Library.Accredit.Configuration;
using MCS.Library.Accredit.OguAdmin.Caching;
using MCS.Library.Accredit.Common;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 用户信息数据类 的摘要说明。
	/// </summary>
	public class LogOnUserInfo : Interfaces.ILogOnUserInfo
	{
		/// <summary>
		/// 构造函数(密码的加密算法计算)
		/// </summary>
		/// <param name="strUserLogOnName">用户的登录名称</param>
		/// <param name="strPwdTypeGuid">用户所使用的密码类型</param>
		/// <param name="strUserPwd">用户所使用的登录口令（明码，待转换）</param>
		public LogOnUserInfo(string strUserLogOnName, string strPwdTypeGuid, string strUserPwd)
		{
			InitLogOnUserInfo(strUserLogOnName, LogonType.LOGON_NAME, strPwdTypeGuid, strUserPwd);
		}

		/// <summary>
		/// 构造函数(密码的加密算法计算)
		/// </summary>
		/// <param name="strUserLogOnName">用户的登录名称</param>
		/// <param name="strUserPwd">用户所使用的登录口令（明码，待转换）</param>
		public LogOnUserInfo(string strUserLogOnName, string strUserPwd)
		{
			InitLogOnUserInfo(strUserLogOnName, LogonType.LOGON_NAME, string.Empty, strUserPwd);
		}

		/// <summary>
		/// 判断用户系统登录
		/// </summary>
		/// <param name="strUserValue">用户数据值</param>
		/// <param name="eunmValueType">用户数据值类型</param>
		/// <param name="strPwdTypeGuid">用户使用口令的加密类型</param>
		/// <param name="strUserPwd">用户登录使用的口令</param>
		public LogOnUserInfo(string strUserValue, LogonType eunmValueType, string strPwdTypeGuid, string strUserPwd)
		{
			InitLogOnUserInfo(strUserValue, eunmValueType, string.Empty, strUserPwd);
		}

		/// <summary>
		/// 构造函数（根据http的文档上下文初始化当前登录用户的信息数据）
		/// </summary>
		/// <param name="content">http的文档上下文</param>
		public LogOnUserInfo(HttpContext content)
		{
			try
			{
				_StrUserLogOnName = content.User.Identity.Name;
				DataSet result;
				//if (false==LogOnUserInfoQueue.Instance.TryGetValue(_StrUserLogOnName, out result))
				//{
				//    lock (typeof(LogOnUserInfoQueue))
				//    {
				if (false == LogOnUserInfoQueue.Instance.TryGetValue(_StrUserLogOnName, out result))
				{
					string strSql = @"
					SELECT  OU_USERS.PARENT_GUID, OU_USERS.USER_GUID, OU_USERS.DISPLAY_NAME, OU_USERS.OBJ_NAME,
						OU_USERS.ALL_PATH_NAME, OU_USERS.INNER_SORT, OU_USERS.GLOBAL_SORT, OU_USERS.ORIGINAL_SORT, OU_USERS.SIDELINE,
						OU_USERS.START_TIME, OU_USERS.END_TIME,	USERS.LOGON_NAME, OU_USERS.DESCRIPTION,
						USERS.RANK_CODE, RANK_DEFINE.SORT_ID, RANK_DEFINE.NAME, RANK_DEFINE.VISIBLE 
					FROM OU_USERS, USERS LEFT JOIN RANK_DEFINE ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME 
					WHERE OU_USERS.USER_GUID = USERS.GUID 
						AND USERS.LOGON_NAME = {0} 
						AND USERS.POSTURAL <> 1 
						AND OU_USERS.STATUS = 1 
						AND DATEDIFF(DAY, OU_USERS.START_TIME, GETDATE()) >= 0
						AND DATEDIFF(DAY, GETDATE(), OU_USERS.END_TIME) >= 0 ";

					strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(_StrUserLogOnName, true));

					using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
					{
						Database database = DatabaseFactory.Create(context);
						result = database.ExecuteDataSet(CommandType.Text, strSql);
					}
					LogOnUserInfoQueue.Instance.Add(_StrUserLogOnName, result, InnerCacheHelper.PrepareDependency());
				}
				//    }
				//}

				InitData(result);

			}
			catch (System.Exception ex)
			{
				//ExceptionManager.Publish(ex);
				throw ex;
			}
		}


		/// <summary>
		/// 用户身份认证
		/// </summary>
		/// <param name="strUserValue">用户数据值</param>
		/// <param name="eunmValueType">用户数据值类型</param>
		/// <param name="strPwdTypeGuid">用户所使用的密码类型</param>
		/// <param name="strUserPwd">用户所使用的登录口令（明码，待转换）</param>
		private void InitLogOnUserInfo(string strUserValue, LogonType eunmValueType, string strPwdTypeGuid, string strUserPwd)
		{
			ExceptionHelper.TrueThrow(string.IsNullOrEmpty(strUserValue.Trim()), "对不起，没有确定的用户登录信息！");
			_StrUserLogOnName = strUserValue;
			try
			{
				string strPwd = SecurityCalculate.PwdCalculate(strPwdTypeGuid, strUserPwd);
				string strOriginal = @"
					SELECT  OU_USERS.PARENT_GUID, OU_USERS.USER_GUID, OU_USERS.DISPLAY_NAME, OU_USERS.OBJ_NAME, 
						OU_USERS.ALL_PATH_NAME, OU_USERS.INNER_SORT, OU_USERS.GLOBAL_SORT, OU_USERS.ORIGINAL_SORT, OU_USERS.SIDELINE, 
						OU_USERS.START_TIME, OU_USERS.END_TIME,	USERS.LOGON_NAME, OU_USERS.DESCRIPTION,
						USERS.RANK_CODE, RANK_DEFINE.SORT_ID, RANK_DEFINE.NAME, RANK_DEFINE.VISIBLE 
					FROM OU_USERS, USERS LEFT JOIN RANK_DEFINE ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
					WHERE OU_USERS.USER_GUID = USERS.GUID 
						AND USERS." + TSqlBuilder.Instance.CheckQuotationMark(eunmValueType.ToString(), false) + @" = {0} 
						{1} 
						{2}
						AND OU_USERS.STATUS = 1 
						AND DATEDIFF(DAY, OU_USERS.START_TIME, GETDATE()) >= 0
						AND DATEDIFF(DAY, GETDATE(), OU_USERS.END_TIME) >= 0 ";
				string strSql = string.Format(strOriginal,
					TSqlBuilder.Instance.CheckQuotationMark(strUserValue, true),
					" AND USERS.USER_PWD = " + TSqlBuilder.Instance.CheckQuotationMark(strPwd, true),
					strPwdTypeGuid == string.Empty ? string.Empty : " AND USERS.PWD_TYPE_GUID = "
					+ TSqlBuilder.Instance.CheckQuotationMark(strPwdTypeGuid, true));

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);

					DataSet ds = database.ExecuteDataSet(CommandType.Text, strSql + " AND USERS.POSTURAL <> 1 ");
					if (ds.Tables[0].Rows.Count > 0)
					{
						SetImpersonateUser();
						if (_StrUserLogOnName != strUserValue)
						{
							strSql = string.Format(strOriginal, TSqlBuilder.Instance.CheckQuotationMark(_StrUserLogOnName, true),
								string.Empty, string.Empty);

							ds = database.ExecuteDataSet(CommandType.Text, strSql);
						}
					}
					else
					{
						DataSet posDS = database.ExecuteDataSet(CommandType.Text, strSql);
						ExceptionHelper.TrueThrow(posDS.Tables[0].Rows.Count > 0,
							"对不起，您的帐号[" + strUserValue + "]目前被禁用了！\n\n请联系管理员！");
					}
					InitData(ds);
				}
			}
			catch (System.Exception ex)
			{
				//ExceptionManager.Publish(ex);
				throw ex;
			}
		}

		/// <summary>
		/// 根据查询结果ds中数据值来初始化当前对象对应的各个属性数据
		/// </summary>
		/// <param name="ds">针对用户信息的查询结果</param>
		private void InitData(DataSet ds)
		{
			DataTable table = ds.Tables[0];
			ExceptionHelper.TrueThrow<ApplicationException>(table.Rows.Count == 0,
				"对不起, 系统中没有找到与\"" + _StrUserLogOnName + "\"对应的有效帐号！\n请输入正确的用户名和口令！");

			DataRow row = table.Rows[0];
			_StrUserGuid = OGUCommonDefine.DBValueToString(row["USER_GUID"]);
			_StrUserLogOnName = OGUCommonDefine.DBValueToString(row["LOGON_NAME"]);
			_RankDefine = new RankDefine(OGUCommonDefine.DBValueToString(row["RANK_CODE"]),
				(int)row["SORT_ID"], OGUCommonDefine.DBValueToString(row["NAME"]), (int)row["VISIBLE"]);
			_OuUsers = new IOuUsers[table.Rows.Count];
			for (int i = 0; i < table.Rows.Count; i++)
				_OuUsers[i] = new OuUsers(table.Rows[i]);

			_IIdentity = new GenericIdentity(_StrUserLogOnName);
		}

		/// <summary>
		/// 根据配置文件设置当前登录用户的模拟登录身份
		/// </summary>
		private void SetImpersonateUser()
		{
			XmlDocument xmlDoc = GetImpersonateUserDocument();// (new SysConfig()).GetConfigXmlDocument("ImpersonateUser");

			if (xmlDoc != null)
			{
				XmlElement root = xmlDoc.DocumentElement;

				XmlNode uNode = root.SelectSingleNode("ImpersonateUser[@userName=\"" + _StrUserLogOnName + "\"]");

				if (uNode != null && !string.IsNullOrEmpty(uNode.InnerText))
				{
					_StrUserLogOnName = uNode.InnerText;
				}
			}
		}

		private static XmlDocument ImpersonateUserDocument = null;
		private XmlDocument GetImpersonateUserDocument()
		{
			if (LogOnUserInfo.ImpersonateUserDocument == null)
			{
				string filePath = AccreditSection.GetConfig().AccreditSettings.ImpersonateUser;
				if (false == string.IsNullOrEmpty(filePath))
				{
					bool IsFileExist = false;
					if (false == File.Exists(filePath))
					{
						if (HttpContext.Current != null)
							filePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath) + filePath;
						else
							filePath = AppDomain.CurrentDomain.BaseDirectory + filePath;

						IsFileExist = File.Exists(filePath);
					}

					if (IsFileExist)
					{
						LogOnUserInfo.ImpersonateUserDocument = XmlHelper.LoadDocument(filePath);
					}
				}
			}

			return LogOnUserInfo.ImpersonateUserDocument;
		}

		#region ILogOnUserInfo 成员

		private string _StrUserGuid = string.Empty;
		/// <summary>
		/// 当前用户的标识
		/// </summary>
		public string UserGuid
		{
			get
			{
				return _StrUserGuid;
			}
		}

		private string _StrUserLogOnName = string.Empty;
		/// <summary>
		/// 当前用户的登录名
		/// </summary>
		public string UserLogOnName
		{
			get
			{
				return _StrUserLogOnName;
			}
		}

		private Interfaces.IRankDefine _RankDefine = null;
		/// <summary>
		/// 当前用户的行政级别
		/// </summary>
		public Interfaces.IRankDefine RankDefine
		{
			get
			{
				return _RankDefine;
			}
		}

		private Interfaces.IOuUsers[] _OuUsers = null;
		/// <summary>
		/// 当前用户所处于的所有可用的机构人员关系
		/// </summary>
		public Interfaces.IOuUsers[] OuUsers
		{
			get
			{
				return _OuUsers;
			}
		}

		#endregion

		#region IPrincipal 成员
		/// <summary>
		/// 登录人系统身份
		/// </summary>
		private IIdentity _IIdentity;
		/// <summary>
		/// 登录人系统身份
		/// </summary>
		public IIdentity Identity
		{
			get
			{
				return _IIdentity;
			}
		}

		/// <summary>
		/// 判断用户是否存在于指定角色中(还没有实现，请不要使用！！！！)
		/// </summary>
		/// <param name="role">指定的角色名称</param>
		/// <returns></returns>
		public bool IsInRole(string role)
		{
			ExceptionHelper.TrueThrow(true, "系统中不提供该函数功能！");
			return false;
		}

		#endregion
	}
}
