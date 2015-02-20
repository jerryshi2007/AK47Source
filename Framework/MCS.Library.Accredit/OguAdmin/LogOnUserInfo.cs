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
	/// �û���Ϣ������ ��ժҪ˵����
	/// </summary>
	public class LogOnUserInfo : Interfaces.ILogOnUserInfo
	{
		/// <summary>
		/// ���캯��(����ļ����㷨����)
		/// </summary>
		/// <param name="strUserLogOnName">�û��ĵ�¼����</param>
		/// <param name="strPwdTypeGuid">�û���ʹ�õ���������</param>
		/// <param name="strUserPwd">�û���ʹ�õĵ�¼������룬��ת����</param>
		public LogOnUserInfo(string strUserLogOnName, string strPwdTypeGuid, string strUserPwd)
		{
			InitLogOnUserInfo(strUserLogOnName, LogonType.LOGON_NAME, strPwdTypeGuid, strUserPwd);
		}

		/// <summary>
		/// ���캯��(����ļ����㷨����)
		/// </summary>
		/// <param name="strUserLogOnName">�û��ĵ�¼����</param>
		/// <param name="strUserPwd">�û���ʹ�õĵ�¼������룬��ת����</param>
		public LogOnUserInfo(string strUserLogOnName, string strUserPwd)
		{
			InitLogOnUserInfo(strUserLogOnName, LogonType.LOGON_NAME, string.Empty, strUserPwd);
		}

		/// <summary>
		/// �ж��û�ϵͳ��¼
		/// </summary>
		/// <param name="strUserValue">�û�����ֵ</param>
		/// <param name="eunmValueType">�û�����ֵ����</param>
		/// <param name="strPwdTypeGuid">�û�ʹ�ÿ���ļ�������</param>
		/// <param name="strUserPwd">�û���¼ʹ�õĿ���</param>
		public LogOnUserInfo(string strUserValue, LogonType eunmValueType, string strPwdTypeGuid, string strUserPwd)
		{
			InitLogOnUserInfo(strUserValue, eunmValueType, string.Empty, strUserPwd);
		}

		/// <summary>
		/// ���캯��������http���ĵ������ĳ�ʼ����ǰ��¼�û�����Ϣ���ݣ�
		/// </summary>
		/// <param name="content">http���ĵ�������</param>
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
		/// �û������֤
		/// </summary>
		/// <param name="strUserValue">�û�����ֵ</param>
		/// <param name="eunmValueType">�û�����ֵ����</param>
		/// <param name="strPwdTypeGuid">�û���ʹ�õ���������</param>
		/// <param name="strUserPwd">�û���ʹ�õĵ�¼������룬��ת����</param>
		private void InitLogOnUserInfo(string strUserValue, LogonType eunmValueType, string strPwdTypeGuid, string strUserPwd)
		{
			ExceptionHelper.TrueThrow(string.IsNullOrEmpty(strUserValue.Trim()), "�Բ���û��ȷ�����û���¼��Ϣ��");
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
							"�Բ��������ʺ�[" + strUserValue + "]Ŀǰ�������ˣ�\n\n����ϵ����Ա��");
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
		/// ���ݲ�ѯ���ds������ֵ����ʼ����ǰ�����Ӧ�ĸ�����������
		/// </summary>
		/// <param name="ds">����û���Ϣ�Ĳ�ѯ���</param>
		private void InitData(DataSet ds)
		{
			DataTable table = ds.Tables[0];
			ExceptionHelper.TrueThrow<ApplicationException>(table.Rows.Count == 0,
				"�Բ���, ϵͳ��û���ҵ���\"" + _StrUserLogOnName + "\"��Ӧ����Ч�ʺţ�\n��������ȷ���û����Ϳ��");

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
		/// ���������ļ����õ�ǰ��¼�û���ģ���¼���
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

		#region ILogOnUserInfo ��Ա

		private string _StrUserGuid = string.Empty;
		/// <summary>
		/// ��ǰ�û��ı�ʶ
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
		/// ��ǰ�û��ĵ�¼��
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
		/// ��ǰ�û�����������
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
		/// ��ǰ�û������ڵ����п��õĻ�����Ա��ϵ
		/// </summary>
		public Interfaces.IOuUsers[] OuUsers
		{
			get
			{
				return _OuUsers;
			}
		}

		#endregion

		#region IPrincipal ��Ա
		/// <summary>
		/// ��¼��ϵͳ���
		/// </summary>
		private IIdentity _IIdentity;
		/// <summary>
		/// ��¼��ϵͳ���
		/// </summary>
		public IIdentity Identity
		{
			get
			{
				return _IIdentity;
			}
		}

		/// <summary>
		/// �ж��û��Ƿ������ָ����ɫ��(��û��ʵ�֣��벻Ҫʹ�ã�������)
		/// </summary>
		/// <param name="role">ָ���Ľ�ɫ����</param>
		/// <returns></returns>
		public bool IsInRole(string role)
		{
			ExceptionHelper.TrueThrow(true, "ϵͳ�в��ṩ�ú������ܣ�");
			return false;
		}

		#endregion
	}
}
