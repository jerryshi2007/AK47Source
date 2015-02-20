#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Accredit
// FileName	��	FunctionNames.cs
// Remark	��		��Ȩϵͳ�ӿڶ���ʵ�֣���Ҫ�����ṩ���ݶ����Լ����ݷ�������
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		20081216			�´���
//	1.1			ccic\yuanyong		20081216			�޸�SecurityCheckΪStaticClass���Ż�ϵͳ����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Diagnostics;

using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.Properties;
using MCS.Library.Accredit.Configuration;
using MCS.Library.Accredit.Common;
using MCS.Library.Accredit.AppAdmin.Caching;

namespace MCS.Library.Accredit.AppAdmin
{
	/// <summary>
	/// ��Ȩ����Ĺؼ��ӿ�ʵ��
	/// </summary>
	/// <remarks>��Ȩ����Ĺؼ��ӿ�ʵ��</remarks>
	public static class SecurityCheck
	{
		//private const string ConnStr = "AccreditAdmin";
		private static Hashtable RankDefineSortHT = new Hashtable();
		private static Hashtable RankDefineNameHT = new Hashtable();

		///// <summary>
		///// ���캯��
		///// </summary>
		///// <remarks>�չ��캯���������κδ���ʵ��</remarks>
		//public SecurityCheck()	// Del By Yuanyong 20081216
		//{
		//}
		static SecurityCheck()
		{
			string sql = "SELECT CODE_NAME, SORT_ID, NAME FROM RANK_DEFINE;";
			DataTable RankDefineDT = OGUCommonDefine.ExecuteDataset(sql).Tables[0];
			foreach (DataRow row in RankDefineDT.Rows)
			{
				RankDefineSortHT.Add(row["CODE_NAME"], row["SORT_ID"]);
				RankDefineNameHT.Add(row["CODE_NAME"], row["NAME"]);
			}
		}

		#region public functions
		#region IsAdminUser
		/// <summary>
		/// �ж���Ա�Ƿ����ܹ���Ա
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns>�ж���Ա�Ƿ����ܹ���Ա</returns>
		/// <remarks>�ܹ���Ա��ͨ����Ȩƽ̨���ܹ����ߣ�����ܹ���Ա��û���û���ϵͳ����ȨʧЧ��������һ���ܹ���Ա���ܼ���ϵͳ����Ȩ������ƣ�
		/// �����ж��û�userValue�Ƿ���ͨ����Ȩ����ϵͳ�е��ܹ���Ա��ɫ</remarks>
		public static bool IsAdminUser(string userValue, UserValueType userValueType)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, userValueType);
			bool result;
			//if (false == IsAdminUserQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(IsAdminUserQueue))//.CacheQueueSync)
			//    {
			if (false == IsAdminUserQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = GetUserGuids(userValue, userValueType);

					string strSql = @"SELECT COUNT(*) FROM EXPRESSIONS WHERE ROLE_ID IN 
								(SELECT ID FROM ROLES WHERE CODE_NAME =	'ADMINISTRATOR_ROLE'
									AND APP_ID = (SELECT ID FROM APPLICATIONS WHERE CODE_NAME = 'APP_ADMIN') )";
					object obj = OGUCommonDefine.ExecuteScalar(strSql);
					if (obj != null && (int)obj == 0)////�ܹ���Ա��ɫ��û�ж���,������Ա�����ܹ���Ա
					{
						result = true;
					}
					else
					{
						string userAllPath = userValueType == UserValueType.AllPath ? userValue : string.Empty;
						result = SecurityCheck.IsUserInRoles(strUserIDs,
							userAllPath,
							"APP_ADMIN",
							"ADMINISTRATOR_ROLE",
							DelegationMaskType.All);
					}
				}
				IsAdminUserQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// �ж���Ա�Ƿ����ܹ���Ա
		/// </summary>
		/// <param name="userValue">�û���¼���������ر�ָ������Ϊ��¼��</param>
		/// <returns>�ж���Ա�Ƿ����ܹ���Ա</returns>
		/// <remarks>�ܹ���Ա��ͨ����Ȩƽ̨���ܹ����ߣ�����ܹ���Ա��û���û���ϵͳ����ȨʧЧ��������һ���ܹ���Ա���ܼ���ϵͳ����Ȩ������ƣ�
		/// �����ж��û�userValue�Ƿ���ͨ����Ȩ����ϵͳ�е��ܹ���Ա��ɫ</remarks>
		public static bool IsAdminUser(string userValue)
		{
			return IsAdminUser(userValue, UserValueType.LogonName);
		}
		#endregion IsAdminUser

		#region GetFunctionsRoles
		/// <summary>
		/// ����ָ��Ӧ���У�����ָ�����ܵĽ�ɫ��
		/// </summary>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ��ֻ�ܵ���</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�������������ʱ�ö��ŷָ�</param>
		/// <returns>��ѯ�����DataSet��ʽ����</returns>
		/// <remarks>����ָ��Ӧ��appCodeName�У�����ָ������funcCodeNames�����н�ɫ��Ϣ�������DataSet��ʽ���ء�
		/// ���ؽ����ֻ��һ��DataTable�����н�ɫ�������У���������ÿһ��Function��Ӧ��Role���ݡ�
		/// ϵͳ���۲�ѯ����Ƿ���ڶ��Ὣ���������أ����᷵��һ��Nullֵ</remarks>
		public static DataSet GetFunctionsRoles(string appCodeName, string funcCodeNames)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(appCodeName, funcCodeNames);
			DataSet result;
			//if (false == GetFunctionsRolesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetFunctionsRolesQueue))//.CacheQueueSync)
			//    {
			if (false == GetFunctionsRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string sql = @"
SELECT DISTINCT R.ID, R.APP_ID, R.NAME, R.CODE_NAME, R.DESCRIPTION, R.CLASSIFY, R.SORT_ID, R.ALLOW_DELEGATE, R.INHERITED 
FROM APPLICATIONS A, FUNCTIONS F, ROLES R, ROLE_TO_FUNCTIONS RTF
WHERE A.ID = F.APP_ID
	AND A.ID = R.APP_ID
	AND F.ID = RTF.FUNC_ID
	AND R.ID = RTF.ROLE_ID
	AND F.CODE_NAME IN ({0})
	AND A.CODE_NAME = {1}
ORDER BY R.ID;";
				sql = string.Format(sql, OGUCommonDefine.AddMulitStrWithQuotationMark(funcCodeNames),
					TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));

				result = OGUCommonDefine.ExecuteDataset(sql);
				GetFunctionsRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		#endregion GetFunctionsRoles

		#region GetApplications
		/// <summary>
		/// ��õ�ǰͨ����Ȩƽ̨��ע�������Ӧ�õ���Ϣ
		/// </summary>
		/// <returns>��һ��DataSet�������еĲ�ѯ�����</returns>
		/// <remarks>��õ�ǰͨ����Ȩƽ̨��ע�������Ӧ�õ���Ϣ�����е��������ݶ��洢��һ��DataTable���ݱ���</remarks>
		public static DataSet GetApplications()
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey("GetApplications");
			DataSet result;
			//if (false == GetApplicationsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetApplicationsQueue))//.CacheQueueSync)
			//    {
			if (false == GetApplicationsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strSql = @"
SELECT A.ID, A.NAME, A.CODE_NAME, A.DESCRIPTION, A.SORT_ID, 
	A.RESOURCE_LEVEL, A.CHILDREN_COUNT, A.ADD_SUBAPP, A.USE_SCOPE, A.INHERITED_STATE  
FROM APPLICATIONS A
ORDER BY RESOURCE_LEVEL ASC, SORT_ID";

				result = OGUCommonDefine.ExecuteDataset(strSql);
				GetApplicationsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		#endregion GetApplications

		#region GetRoles
		/// <summary>
		/// ��ѯָ��Ӧ���У�ָ���������н�ɫ
		/// </summary>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <returns></returns>
		public static DataSet GetRoles(string appCodeName, RightMaskType rightMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(appCodeName, rightMask);
			DataSet result;
			//if (false == GetRolesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetRolesQueue))//.CacheQueueSync)
			//    {
			if (false == GetRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strSql = GetRoles_SqlStr(appCodeName, rightMask);
					strSql += "\n ORDER BY R.CLASSIFY DESC, R.SORT_ID";

					result = OGUCommonDefine.ExecuteDataset(strSql);
				}
				GetRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ��Ӧ���У�ָ���������н�ɫ
		/// </summary>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <returns></returns>
		public static DataSet GetRoles(string appCodeName)
		{
			return GetRoles(appCodeName, RightMaskType.All);
		}
		#endregion GetRoles

		#region GetFunctions
		/// <summary>
		/// ��ѯָ��Ӧ���У�ָ���������й���
		/// </summary>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <returns></returns>
		public static DataSet GetFunctions(string appCodeName, RightMaskType rightMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(appCodeName, rightMask);
			DataSet result;
			//if (false == GetFunctionsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetFunctionsQueue))//.CacheQueueSync)
			//    {
			if (false == GetFunctionsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strSql = GetFunctions_SqlStr(appCodeName, rightMask);
				strSql += "\n ORDER BY F.CLASSIFY DESC, F.SORT_ID";
				result = OGUCommonDefine.ExecuteDataset(strSql);
				GetFunctionsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ��Ӧ���У�ָ���������й���
		/// </summary>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <returns></returns>
		public static DataSet GetFunctions(string appCodeName)
		{
			return GetFunctions(appCodeName, RightMaskType.All);
		}
		#endregion GetFunctions

		#region GetRolesUsers
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ��������Ա
		/// ������ʹ�ò���extAttr��ʹ�ò���extAttr����л�����Ա�ӿڣ�Ӱ������
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="delegationMask">ί������</param>
		/// <param name="sidelineMask">ְλ����</param>
		/// <param name="extAttr">Ҫ���ȡ����չ����(����Ч�������˲�����Ϊ�˼���ԭ���汾�Ľӿں���)</param>
		/// <returns></returns>
		//[Obsolete("������ʹ�ò���extAttr��ʹ�ò���extAttr����л�����Ա�ӿڣ�Ӱ������", false)]
		public static DataSet GetRolesUsers(string orgRoot,
			string appCodeName,
			string roleCodeNames,
			DelegationMaskType delegationMask,
			SidelineMaskType sidelineMask,
			string extAttr)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(orgRoot, appCodeName, roleCodeNames, delegationMask, sidelineMask, extAttr);
			DataSet result;
			//if (false == GetRolesUsersQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetRolesUsersQueue))//.CacheQueueSync)
			//    {
			if (false == GetRolesUsersQueue.Instance.TryGetValue(cacheKey, out result))
			{
				//������ʹ�ò���extAttr��ʹ�ò���extAttr����л�����Ա�ӿڣ�Ӱ������
				result = InnerGetRolesUsers(orgRoot, appCodeName, roleCodeNames, delegationMask, sidelineMask, extAttr);
				GetRolesUsersQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ��������Ա
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="delegationMask">ί������</param>
		/// <param name="sidelineMask">ְλ����</param>
		/// <returns></returns>
		public static DataSet GetRolesUsers(string orgRoot,
			string appCodeName,
			string roleCodeNames,
			DelegationMaskType delegationMask,
			SidelineMaskType sidelineMask)
		{
			return GetRolesUsers(orgRoot, appCodeName, roleCodeNames, delegationMask, sidelineMask, string.Empty);
		}
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ��������Ա
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static DataSet GetRolesUsers(string orgRoot, string appCodeName, string roleCodeNames, DelegationMaskType delegationMask)
		{
			return GetRolesUsers(orgRoot, appCodeName, roleCodeNames, delegationMask, SidelineMaskType.All);
		}
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ��������Ա
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static DataSet GetRolesUsers(string orgRoot, string appCodeName, string roleCodeNames)
		{
			return GetRolesUsers(orgRoot, appCodeName, roleCodeNames, DelegationMaskType.All, SidelineMaskType.All);
		}
		#endregion GetRolesUsers

		#region GetChildrenInRoles
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ��ţ����ʱ�ö��ŷָ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ��������飬����Ա</param>
		/// <param name="doesSortRank">�Ƿ����򼶱�true:�ȼ��𣬺��� false:ֻ�Ų��</param>
		/// <param name="includeDelegate">�Ƿ����ί��Ȩ�޵ı���Ȩ����true:���� false:������</param>
		/// <param name="bExpandGroup">�Ƿ�չ��Group�е�User</param>
		/// <returns></returns>
		public static DataSet GetChildrenInRoles(string orgRoot,
			string appCodeName,
			string roleCodeNames,
			bool doesMixSort,
			bool doesSortRank,
			bool includeDelegate,
			bool bExpandGroup)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(orgRoot, appCodeName, roleCodeNames, doesMixSort, doesSortRank, includeDelegate, bExpandGroup);
			DataSet result;
			//if (false == GetChildrenInRolesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetChildrenInRolesQueue))//.CacheQueueSync)
			//    {
			if (false == GetChildrenInRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					//�򵥲�ѯ����Ȩ����Ĺ�������
					string strWhereSql = @"
							WHERE ROLE_ID IN 
								(SELECT ID FROM ROLES WHERE CODE_NAME IN 
									({0})
									AND APP_ID IN (SELECT ID FROM APPLICATIONS WHERE CODE_NAME = {1}) )";
					strWhereSql = string.Format(strWhereSql, OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames),
						TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));


					//�õ������ŵ�����
					string[] arrRootSorts = new string[0];
					if (orgRoot != string.Empty)
					{
						arrRootSorts = GetOrgSorts(orgRoot);
					}

					//�������ʽ�������������д�������У���Ϊ�������ӵ�����
					string strSql = ParseExpsToTable_SqlStr2(strWhereSql);

					#region Modify By Yuanyong
					#region ԭ�д��룬Del By Yuanyong 20070911
					/*if (bExpandGroup)//���չ��������Ա�����ܰ������ʽID����Ϊһ�����ڲ�ͬ����ʱ��������ؼ�¼
				strSql += @"SELECT '' AS OBJECTCLASS,  '' AS PERSON_ID, '' CUSTOMS_CODE,  NULL AS SIDELINE,   '' AS RANK_NAME, '' ALL_PATH_NAME, '' GLOBAL_SORT, '' ORIGINAL_SORT, '' DISPLAY_NAME, '' OBJ_NAME,  '' AS LOGON_NAME, '' PARENT_GUID, '' GUID, '' CODE_NAME, NULL SORT_ID, '' NAME, '' CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, NULL IsDelegated WHERE 1>1";
			else //��չ����ʱ����Ҫ���ʽ��ID����Ϊ��Ҫ���ݱ��ʽID���õ����ʽ�ķ�Χ������ȷ����ɫ�ķ�Χ
				strSql += @"SELECT '' AS OBJECTCLASS,  '' AS PERSON_ID, '' CUSTOMS_CODE,  NULL AS SIDELINE,   '' AS RANK_NAME, '' ALL_PATH_NAME, '' GLOBAL_SORT, '' ORIGINAL_SORT, '' DISPLAY_NAME, '' OBJ_NAME,  '' AS LOGON_NAME, '' PARENT_GUID, '' GUID, '' CODE_NAME, NULL SORT_ID, '' NAME, '' CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, NULL ID, NULL IsDelegated WHERE 1>1";

			//��Ա
			strSql	+= string.Format("\n UNION {0}", GetExpressionsDedail_SqlStr("USERS", arrRootSorts, bExpandGroup ));

			//����
			strSql	+= string.Format("\n UNION {0}", GetExpressionsDedail_SqlStr("ORGANIZATIONS", arrRootSorts, bExpandGroup ));
			
			//��
			strSql	+= string.Format("\n UNION {0}", GetExpressionsDedail_SqlStr("GROUPS", arrRootSorts, bExpandGroup ));


			if (doesMixSort)//����
			{
				if ( doesSortRank )
					strSql += " ORDER BY SORT_ID, GLOBAL_SORT";
				else
					strSql += " ORDER BY GLOBAL_SORT";
				

			}
			else
			{
				if ( doesSortRank )
					strSql += " ORDER BY CLASSIFY, SORT_ID, GLOBAL_SORT";
				else
					strSql += " ORDER BY CLASSIFY, GLOBAL_SORT";
			}*/
					#endregion
					#region Result Add By Yuanyong 20070911

					StringBuilder sqlUnion = new StringBuilder(1024);

					if (bExpandGroup)//���չ��������Ա�����ܰ������ʽID����Ϊһ�����ڲ�ͬ����ʱ��������ؼ�¼
						sqlUnion.Append(@"
	SELECT '' AS OBJECTCLASS,  '' AS PERSON_ID, '' CUSTOMS_CODE,  NULL AS SIDELINE,   '' AS RANK_NAME, '' ALL_PATH_NAME, '' GLOBAL_SORT, 
		'' ORIGINAL_SORT, '' DISPLAY_NAME, '' OBJ_NAME,  '' AS LOGON_NAME, '' PARENT_GUID, '' GUID, '' CODE_NAME, NULL SORT_ID, '' NAME, 
		'' CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, NULL IsDelegated 
	WHERE 1>1");
					else //��չ����ʱ����Ҫ���ʽ��ID����Ϊ��Ҫ���ݱ��ʽID���õ����ʽ�ķ�Χ������ȷ����ɫ�ķ�Χ
						sqlUnion.Append(@"
	SELECT '' AS OBJECTCLASS,  '' AS PERSON_ID, '' CUSTOMS_CODE,  NULL AS SIDELINE,   '' AS RANK_NAME, '' ALL_PATH_NAME, '' GLOBAL_SORT, 
		'' ORIGINAL_SORT, '' DISPLAY_NAME, '' OBJ_NAME,  '' AS LOGON_NAME, '' PARENT_GUID, '' GUID, '' CODE_NAME, NULL SORT_ID, '' NAME, 
		'' CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, NULL ID, NULL IsDelegated 
	WHERE 1>1");

					//��Ա
					sqlUnion.AppendFormat(Environment.NewLine + "	UNION" + Environment.NewLine + "	{0}", GetExpressionsDedail_SqlStr("USERS", arrRootSorts, bExpandGroup));
					//����
					sqlUnion.AppendFormat(Environment.NewLine + "	UNION" + Environment.NewLine + "	{0}", GetExpressionsDedail_SqlStr("ORGANIZATIONS", arrRootSorts, bExpandGroup));
					//��
					sqlUnion.AppendFormat(Environment.NewLine + "	UNION" + Environment.NewLine + "	{0}", GetExpressionsDedail_SqlStr("GROUPS", arrRootSorts, bExpandGroup));

					//strSql += ";" + Environment.NewLine; // Del By CCIC\yuanyong 20081008 SQL 2000�в���֧�������ֺŵ�д��
					strSql += Environment.NewLine;
					if (doesMixSort)//����
					{
						if (doesSortRank)
							strSql += string.Format("SELECT * FROM ({0}) SearchResult ORDER BY SORT_ID, GLOBAL_SORT;", sqlUnion.ToString());
						else
							strSql += string.Format("SELECT * FROM ({0}) SearchResult ORDER BY GLOBAL_SORT;", sqlUnion.ToString());


					}
					else
					{
						if (doesSortRank)
							strSql += string.Format("SELECT * FROM ({0}) SearchResult ORDER BY CLASSIFY, SORT_ID, GLOBAL_SORT;", sqlUnion.ToString());
						else
							strSql += string.Format("SELECT * FROM ({0}) SearchResult ORDER BY CLASSIFY, GLOBAL_SORT;", sqlUnion.ToString());
					}
					#endregion
					#endregion

					result = OGUCommonDefine.ExecuteDataset(strSql);
					#region ��������
					//��ί��Ȩ������Ա??
					if ((result.Tables[0].Rows.Count > 0) && includeDelegate)//&& ( bIncludeUser || bIncludeGroup ) )
					{
						string strSql2 = string.Empty;

						//�õ���ǰ��ɫ�£���Ч��ί��
						strSql = @"SELECT * FROM DELEGATIONS 
								WHERE ROLE_ID IN 
									(SELECT ID FROM ROLES 
										WHERE APP_ID IN (SELECT ID FROM APPLICATIONS WHERE CODE_NAME = {0})
										AND CODE_NAME IN ({1}) AND ALLOW_DELEGATE = 'y')
								AND GETDATE() BETWEEN START_TIME AND END_TIME";
						strSql = string.Format(strSql,
							TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true),
							OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames));

						DataSet ds = OGUCommonDefine.ExecuteDataset(strSql);

						#region �����ڲ�����
						if (ds.Tables[0].Rows.Count > 0)
						{
							string strTemp = string.Empty;
							string strTemp2 = string.Empty;
							foreach (DataRow row in ds.Tables[0].Rows)
							{
								//ί�������ڽ�ɫ
								strTemp = string.Format("[GUID] = '{0}'", row["SOURCE_ID"]);
								//��ί���߲����ڽ�ɫ
								strTemp2 = string.Format("[GUID] = '{0}'", row["TARGET_ID"]);
								if (result.Tables[0].Select(strTemp).Length > 0 && result.Tables[0].Select(strTemp2).Length == 0)
								{
									if (strSql2 != string.Empty)
										strSql2 += "\n UNION \n";

									if (bExpandGroup)
										strSql2 += string.Format(@"	
SELECT 'USERS' AS OBJECTCLASS, U.PERSON_ID, CAST(NULL AS NVARCHAR(4))  AS CUSTOMS_CODE, OU.SIDELINE, OU.RANK_NAME, OU.ALL_PATH_NAME, 
	OU.GLOBAL_SORT, OU.ORIGINAL_SORT, OU.DISPLAY_NAME, OU.OBJ_NAME, U.LOGON_NAME, OU.PARENT_GUID, U.GUID, RD.CODE_NAME, RD.SORT_ID, RD.NAME, 
	3 CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, 1 IsDelegated
FROM OU_USERS OU INNER JOIN  USERS U ON OU.USER_GUID = U.GUID LEFT JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME
WHERE OU.USER_GUID = '{0}'", row["TARGET_ID"]);
									else
										strSql2 += string.Format(@"	
SELECT 'USERS' AS OBJECTCLASS, U.PERSON_ID,  CAST(NULL AS NVARCHAR(4)) AS CUSTOMS_CODE, OU.SIDELINE, OU.RANK_NAME, OU.ALL_PATH_NAME, 
	OU.GLOBAL_SORT, OU.ORIGINAL_SORT, OU.DISPLAY_NAME, OU.OBJ_NAME, U.LOGON_NAME, OU.PARENT_GUID, U.GUID, RD.CODE_NAME, RD.SORT_ID, RD.NAME, 
	3 CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, '{0}' ID, 1 IsDelegated
FROM OU_USERS OU INNER JOIN  USERS U ON OU.USER_GUID = U.GUID LEFT JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME
WHERE OU.USER_GUID = '{1}'", row["SOURCE_ID"], row["TARGET_ID"]);
								}
							}
							//����ί����
							if (strSql2 != string.Empty)
							{
								//***********
								//ί�ɵ��˲����˻�����ԭ��Ȩ���ڷ�Χ�У��϶���ί�����ڷ�Χ�С�
								//***********
								result.Merge(OGUCommonDefine.ExecuteDataset("--ί�����ڸ������ķ�Χ�У����϶���ί�����ڷ�Χ�С�  \n" + strSql2));
							}
						}
						#endregion
					}
					#endregion
				}
				GetChildrenInRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ��ţ����ʱ�ö��ŷָ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ��������飬����Ա</param>
		/// <param name="doesSortRank">�Ƿ����򼶱�true:�ȼ��𣬺��� false:ֻ�Ų��</param>
		/// <param name="includeDelegate">�Ƿ����ί��Ȩ�޵ı���Ȩ����true:���� false:������</param>
		/// <returns></returns>
		public static DataSet GetChildrenInRoles(string orgRoot,
			string appCodeName,
			string roleCodeNames,
			bool doesMixSort,
			bool doesSortRank,
			bool includeDelegate)
		{
			return GetChildrenInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, doesSortRank, includeDelegate, false);
		}
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ��ţ����ʱ�ö��ŷָ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ��������飬����Ա</param>
		/// <param name="doesSortRank">�Ƿ����򼶱�true:�ȼ��𣬺��� false:ֻ�Ų��</param>
		/// <returns></returns>
		public static DataSet GetChildrenInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort, bool doesSortRank)
		{
			return GetChildrenInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, doesSortRank, true);
		}

		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ��ţ����ʱ�ö��ŷָ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ��������飬����Ա</param>
		/// <returns></returns>
		public static DataSet GetChildrenInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort)
		{
			return GetChildrenInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, false);
		}

		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ��ţ����ʱ�ö��ŷָ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static DataSet GetChildrenInRoles(string orgRoot, string appCodeName, string roleCodeNames)
		{
			return GetChildrenInRoles(orgRoot, appCodeName, roleCodeNames, false);
		}

		#endregion GetChildrenInRoles

		#region GetDepartmentAndUserInRoles
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ��ţ����ʱ�ö��ŷָ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ��������飬����Ա</param>
		/// <param name="doesSortRank">�Ƿ����򼶱�true:�ȼ��𣬺��� false:ֻ�Ų��</param>
		/// <param name="includeDelegate">�Ƿ����ί��Ȩ�޵ı���Ȩ����true:���� false:������</param>
		/// <returns></returns>
		public static DataSet GetDepartmentAndUserInRoles(string orgRoot,
			string appCodeName,
			string roleCodeNames,
			bool doesMixSort,
			bool doesSortRank,
			bool includeDelegate)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(orgRoot, appCodeName, roleCodeNames, doesMixSort, doesSortRank, includeDelegate);
			DataSet result;
			//if (false == GetDepartmentAndUserInRolesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetDepartmentAndUserInRolesQueue))//.CacheQueueSync)
			//    {
			if (false == GetDepartmentAndUserInRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				//ȥ����β��'\'
				if (orgRoot != string.Empty && orgRoot.LastIndexOf("\\") == orgRoot.Length - 1)
					orgRoot = orgRoot.Substring(0, orgRoot.Length - 1);

				result = GetChildrenInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, doesSortRank, includeDelegate, true);

				//�����ְ��ɵ��ظ���¼
				DataRow[] rows;

				rows = result.Tables[0].Select("[OBJECTCLASS] = 'USERS' AND [SIDELINE] = 1");
				if (rows.Length != 0)
				{
					DataRow[] tempRows;
					foreach (DataRow row in rows)
					{
						tempRows = result.Tables[0].Select(string.Format("[GUID] = '{0}' AND [PARENT_GUID] <> '{1}'", row["GUID"], row["PARENT_GUID"]));

						//�����������ְλ����ɾ��ְλ
						if (tempRows.Length > 0)
						{
							result.Tables[0].Rows.Remove(row);
						}
					}
					result.AcceptChanges();
				}
				GetDepartmentAndUserInRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		///// </summary>
		///// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ��ţ����ʱ�ö��ŷָ�</param>
		///// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		///// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		///// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ��������飬����Ա</param>
		///// <param name="doesSortRank">�Ƿ����򼶱�true:�ȼ��𣬺��� false:ֻ�Ų��</param>
		///// <param name="includeDelegate">�Ƿ����ί��Ȩ�޵ı���Ȩ����true:���� false:������</param>
		///// <returns></returns>
		//public static DataSet GetDepartmentAndUserInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort, bool doesSortRank, bool includeDelegate)
		//{
		//    DataAccess dba = new DataAccess(ConnStr);
		//    using (dba.dBContextInfo)
		//    {
		//        dba.dBContextInfo.OpenConnection();
		//        try
		//        {
		//            return GetDepartmentAndUserInRoles(dba, orgRoot, appCodeName, roleCodeNames, doesMixSort, doesSortRank, includeDelegate);
		//        }
		//        catch (Exception ex)
		//        {
		//            ExceptionManager.Publish(ex);
		//            throw new Exception(ex.Message, ex);
		//        }
		//    }
		//}

		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ��ţ����ʱ�ö��ŷָ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ��������飬����Ա</param>
		/// <param name="doesSortRank">�Ƿ����򼶱�true:�ȼ��𣬺��� false:ֻ�Ų��</param>
		/// <returns></returns>
		public static DataSet GetDepartmentAndUserInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort, bool doesSortRank)
		{
			return GetDepartmentAndUserInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, doesSortRank, true);
		}
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ��ţ����ʱ�ö��ŷָ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="doesMixSort">�Ƿ���û������true:�������顢��Ա���ţ�false:�Ȼ��������飬����Ա</param>
		/// <returns></returns>
		public static DataSet GetDepartmentAndUserInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort)
		{
			return GetDepartmentAndUserInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, false);
		}
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ�����б���Ȩ����
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ��ţ����ʱ�ö��ŷָ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static DataSet GetDepartmentAndUserInRoles(string orgRoot, string appCodeName, string roleCodeNames)
		{
			return GetDepartmentAndUserInRoles(orgRoot, appCodeName, roleCodeNames, false);
		}
		#endregion GetDepartmentAndUserInRoles

		#region GetOriginalUser
		/// <summary>
		/// ��ѯָ����Ա��ָ��Ӧ���У�ָ����ɫ��ԭ��ί����
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
		/// <returns></returns>
		public static DataSet GetOriginalUser(string userValue,
			string appCodeName,
			string roleCodeNames,
			UserValueType userValueType,
			bool includeDisabled)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appCodeName, roleCodeNames, userValueType, includeDisabled);
			DataSet result;
			//if (false == GetOriginalUserQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetOriginalUserQueue))//.CacheQueueSync)
			//    {
			if (false == GetOriginalUserQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					result = GetOriginalUser(strUserIDs, appCodeName, roleCodeNames, includeDisabled);
				}
				GetOriginalUserQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա��ָ��Ӧ���У�ָ����ɫ��ԭ��ί����
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetOriginalUser(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType)
		{
			return GetOriginalUser(userValue, appCodeName, roleCodeNames, userValueType, false);
		}
		/// <summary>
		/// ��ѯָ����Ա��ָ��Ӧ���У�ָ����ɫ��ԭ��ί����
		/// </summary>
		/// <param name="userValue">�û��ĵ�¼��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static DataSet GetOriginalUser(string userValue, string appCodeName, string roleCodeNames)
		{
			return GetOriginalUser(userValue, appCodeName, roleCodeNames, UserValueType.LogonName);
		}
		#endregion GetOriginalUser

		#region GetAllOriginalUser
		/// <summary>
		/// ��ѯָ����Ա��ָ������Ӧ�������н�ɫ��ԭ��ί����
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
		/// <returns></returns>
		public static DataSet GetAllOriginalUser(string userValue, UserValueType userValueType, bool includeDisabled)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, userValueType, includeDisabled);
			DataSet result;
			//if (false == GetAllOriginalUserQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetAllOriginalUserQueue))//.CacheQueueSync)
			//    {
			if (false == GetAllOriginalUserQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					result = GetAllOriginalUser(strUserIDs, includeDisabled);
				}
				GetAllOriginalUserQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա��ָ������Ӧ���е�ԭ��ί����
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetAllOriginalUser(string userValue, UserValueType userValueType)
		{
			return GetAllOriginalUser(userValue, userValueType, false);
		}
		/// <summary>
		/// ��ѯָ����Ա��ָ������Ӧ���е�ԭ��ί����
		/// </summary>
		/// <param name="userValue">�û��ĵ�¼��</param>
		/// <returns></returns>
		public static DataSet GetAllOriginalUser(string userValue)
		{
			return GetAllOriginalUser(userValue, UserValueType.LogonName);
		}
		#endregion GetAllOriginalUser

		#region GetDelegatedUser
		/// <summary>
		/// ��ѯָ����Ա��ָ��Ӧ���У�ָ����ɫ�ı�ί����
		/// </summary>
		/// <param name="userValues">�û���ݱ�ʶ����userValueType����ָ�����ͣ����ʱ�ö��ŷָ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
		/// <returns></returns>
		public static DataSet GetDelegatedUser(string userValues, string appCodeName, string roleCodeNames, UserValueType userValueType, bool includeDisabled)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValues, appCodeName, roleCodeNames, userValueType, includeDisabled);
			DataSet result;
			//if (false == GetDelegatedUserQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetDelegatedUserQueue))//.CacheQueueSync)
			//    {
			if (false == GetDelegatedUserQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValues, userValueType);
					result = GetDelegatedUser(strUserIDs, appCodeName, roleCodeNames, includeDisabled);
				}
				GetDelegatedUserQueue.Instance.Add(cacheKey, result);
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա��ָ��Ӧ���У�ָ����ɫ�ı�ί����
		/// </summary>
		/// <param name="userValues">�û���ݱ�ʶ����userValueType����ָ�����ͣ����ʱ�ö��ŷָ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetDelegatedUser(string userValues, string appCodeName, string roleCodeNames, UserValueType userValueType)
		{
			return GetDelegatedUser(userValues, appCodeName, roleCodeNames, userValueType, false);
		}
		/// <summary>
		/// ��ѯָ����Ա��ָ��Ӧ���У�ָ����ɫ�ı�ί����
		/// </summary>
		/// <param name="userValues">�û���ݱ�ʶ����userValueType����ָ�����ͣ����ʱ�ö��ŷָ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static DataSet GetDelegatedUser(string userValues, string appCodeName, string roleCodeNames)
		{
			return GetDelegatedUser(userValues, appCodeName, roleCodeNames, UserValueType.LogonName);
		}
		#endregion GetDelegatedUser

		#region GetAllDelegatedUser
		/// <summary>
		/// ��ѯָ����Ա������Ӧ���С����н�ɫ�ı�ί����
		/// </summary>
		/// <param name="userValues">�û���¼��</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="includeDisabled">�Ƿ������Ч��ί��</param>
		/// <returns></returns>
		public static DataSet GetAllDelegatedUser(string userValues, UserValueType userValueType, bool includeDisabled)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValues, userValueType, includeDisabled);
			DataSet result;
			//if (false == GetAllDelegatedUserQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetAllDelegatedUserQueue))//.CacheQueueSync)
			//    {
			if (false == GetAllDelegatedUserQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValues, userValueType);
					result = GetAllDelegatedUser(strUserIDs, includeDisabled);
				}
				GetAllDelegatedUserQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա������Ӧ���С����н�ɫ�ı�ί����
		/// </summary>
		/// <param name="userValues">�û���ݱ�ʶ����userValueType����ָ�����ͣ����ʱ�ö��ŷָ���</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetAllDelegatedUser(string userValues, UserValueType userValueType)
		{
			return GetAllDelegatedUser(userValues, userValueType, false);
		}
		/// <summary>
		/// ��ѯָ����Ա������Ӧ���С����н�ɫ�ı�ί����
		/// </summary>
		/// <param name="userValues">�û���¼��</param>
		/// <returns></returns>
		public static DataSet GetAllDelegatedUser(string userValues)
		{
			return GetAllDelegatedUser(userValues, UserValueType.LogonName);
		}
		#endregion GetAllDelegatedUser

		#region GetFunctionsUsers
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ӵ��ָ�����ܵ�������Ա
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�[ֻҪ������һ��Ϳ���]</param>
		/// <param name="delegationMask">ί������</param>
		/// <param name="sidelineMask">ְλ����</param>
		/// <param name="extAttr">Ҫ���ȡ����չ����</param>
		/// <returns>��ѯָ�������£�ָ��Ӧ���У�ӵ��ָ�����ܵ�������Ա[������ܺŵ�ʱ��ֻҪ������һ��Ϳ���]</returns>
		//[Obsolete("������ʹ�ò���extAttr��ʹ�ò���extAttr����л�����Ա�ӿڣ�Ӱ������", false)]
		public static DataSet GetFunctionsUsers(string orgRoot,
			string appCodeName,
			string funcCodeNames,
			DelegationMaskType delegationMask,
			SidelineMaskType sidelineMask,
			string extAttr)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(orgRoot, appCodeName, funcCodeNames, delegationMask, sidelineMask, extAttr);
			DataSet result;
			//if (false == GetFunctionsUsersQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetFunctionsUsersQueue))//.CacheQueueSync)
			//    {
			if (false == GetFunctionsUsersQueue.Instance.TryGetValue(cacheKey, out result))
			{
				result = InnerGetFunctionsUsers(orgRoot, appCodeName, funcCodeNames, delegationMask, sidelineMask, extAttr);
				GetFunctionsUsersQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ӵ��ָ�����ܵ�������Ա
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="delegationMask">ί������</param>
		/// <param name="sidelineMask">ְλ����</param>
		/// <returns></returns>
		public static DataSet GetFunctionsUsers(string orgRoot,
			string appCodeName,
			string funcCodeNames,
			DelegationMaskType delegationMask,
			SidelineMaskType sidelineMask)
		{
			return GetFunctionsUsers(orgRoot, appCodeName, funcCodeNames, delegationMask, sidelineMask, string.Empty);
		}
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ӵ��ָ�����ܵ�������Ա
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static DataSet GetFunctionsUsers(string orgRoot, string appCodeName, string funcCodeNames, DelegationMaskType delegationMask)
		{
			return GetFunctionsUsers(orgRoot, appCodeName, funcCodeNames, delegationMask, SidelineMaskType.All);
		}
		/// <summary>
		/// ��ѯָ�������£�ָ��Ӧ���У�ӵ��ָ�����ܵ�������Ա
		/// </summary>
		/// <param name="orgRoot">�����ŵ�ȫ·�����մ�ʱ�����Ʋ���</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static DataSet GetFunctionsUsers(string orgRoot, string appCodeName, string funcCodeNames)
		{
			return GetFunctionsUsers(orgRoot, appCodeName, funcCodeNames, DelegationMaskType.All);
		}
		#endregion GetFunctionsUsers

		#region IsUserInRoles
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ����ɫ��
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static bool IsUserInRoles(string userValue,
			string appCodeName,
			string roleCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appCodeName, roleCodeNames, userValueType, delegationMask);
			bool result;
			//if (false == IsUserInRolesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(IsUserInRolesQueue))//.CacheQueueSync)
			//    {
			if (false == IsUserInRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = IsUserInRoles(strUserIDs, userValue, appCodeName, roleCodeNames, delegationMask);
					}
					else
					{
						result = IsUserInRoles(strUserIDs, string.Empty, appCodeName, roleCodeNames, delegationMask);
					}
				}
				IsUserInRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ����ɫ��
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static bool IsUserInRoles(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType)
		{
			return IsUserInRoles(userValue, appCodeName, roleCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ����ɫ��
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static bool IsUserInRoles(string userValue, string appCodeName, string roleCodeNames)
		{
			return IsUserInRoles(userValue, appCodeName, roleCodeNames, UserValueType.LogonName);
		}
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ����ɫ��
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static bool IsUserInRoles(string userValue,
			Guid appID,
			string roleCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appID, roleCodeNames, userValueType, delegationMask);
			bool result;
			//if (false == IsUserInRolesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(IsUserInRolesQueue))//.CacheQueueSync)
			//    {
			if (false == IsUserInRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string appCodeName = GetApplicatonCodeName(appID);
					if (appCodeName == string.Empty)
						return false;

					result = IsUserInRoles(userValue, appCodeName, roleCodeNames, userValueType, delegationMask);
				}
				IsUserInRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ����ɫ��
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static bool IsUserInRoles(string userValue, Guid appID, string roleCodeNames, UserValueType userValueType)
		{
			return IsUserInRoles(userValue, appID, roleCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ����ɫ��
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static bool IsUserInRoles(string userValue, Guid appID, string roleCodeNames)
		{
			return IsUserInRoles(userValue, appID, roleCodeNames, UserValueType.LogonName);
		}
		#endregion IsUserInRoles

		#region IsUserInAllRoles
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ�������н�ɫ��
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static bool IsUserInAllRoles(string userValue,
			string appCodeName,
			string roleCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appCodeName, roleCodeNames, userValueType, delegationMask);
			bool result;
			//if (false == IsUserInAllRolesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(IsUserInAllRolesQueue))//.CacheQueueSync)
			//    {
			if (false == IsUserInAllRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = IsUserInAllRoles(strUserIDs, userValue, appCodeName, roleCodeNames, delegationMask);
					}
					else
					{
						result = IsUserInAllRoles(strUserIDs, string.Empty, appCodeName, roleCodeNames, delegationMask);
					}
				}
				IsUserInAllRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ�������н�ɫ��
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static bool IsUserInAllRoles(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType)
		{
			return IsUserInAllRoles(userValue, appCodeName, roleCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ�������н�ɫ��
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static bool IsUserInAllRoles(string userValue, string appCodeName, string roleCodeNames)
		{
			return IsUserInAllRoles(userValue, appCodeName, roleCodeNames, UserValueType.LogonName);
		}
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ�������н�ɫ��
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static bool IsUserInAllRoles(string userValue,
			Guid appID,
			string roleCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appID, roleCodeNames, userValueType, delegationMask);
			bool result;
			//if (false == IsUserInAllRolesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(IsUserInAllRolesQueue))//.CacheQueueSync)
			//    {
			if (false == IsUserInAllRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string appCodeName = GetApplicatonCodeName(appID);
					if (appCodeName == string.Empty)
						return false;

					result = IsUserInAllRoles(userValue, appCodeName, roleCodeNames, userValueType, delegationMask);
				}
				IsUserInAllRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ�������н�ɫ��
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static bool IsUserInAllRoles(string userValue, Guid appID, string roleCodeNames, UserValueType userValueType)
		{
			return IsUserInAllRoles(userValue, appID, roleCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// �ж���Ա�Ƿ���ָ��Ӧ�ã�ָ�������н�ɫ��
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static bool IsUserInAllRoles(string userValue, Guid appID, string roleCodeNames)
		{
			return IsUserInAllRoles(userValue, appID, roleCodeNames, UserValueType.LogonName);
		}
		#endregion IsUserInAllRoles

		#region DoesUserHasPermissions
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(��һ������)
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static bool DoesUserHasPermissions(string userValue,
			string appCodeName,
			string funcCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appCodeName, funcCodeNames, userValueType, delegationMask);
			bool result;
			//if (false == DoesUserHasPermissionsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(DoesUserHasPermissionsQueue))//.CacheQueueSync)
			//    {
			if (false == DoesUserHasPermissionsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = DoesUserHasPermissions(strUserIDs, userValue, appCodeName, funcCodeNames, delegationMask);
					}
					else
					{
						result = DoesUserHasPermissions(strUserIDs, string.Empty, appCodeName, funcCodeNames, delegationMask);
					}
				}
				DoesUserHasPermissionsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(��һ������)
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static bool DoesUserHasPermissions(string userValue, string appCodeName, string funcCodeNames, UserValueType userValueType)
		{
			return DoesUserHasPermissions(userValue, appCodeName, funcCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(��һ������)
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static bool DoesUserHasPermissions(string userValue, string appCodeName, string funcCodeNames)
		{
			return DoesUserHasPermissions(userValue, appCodeName, funcCodeNames, UserValueType.LogonName);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(��һ������)
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static bool DoesUserHasPermissions(string userValue,
			Guid appID,
			string funcCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appID, funcCodeNames, userValueType, delegationMask);
			bool result;
			//if (false == DoesUserHasPermissionsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(DoesUserHasPermissionsQueue))//.CacheQueueSync)
			//    {
			if (false == DoesUserHasPermissionsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string appCodeName = GetApplicatonCodeName(appID);
					if (appCodeName == string.Empty)
						return false;

					result = DoesUserHasPermissions(userValue, appCodeName, funcCodeNames, userValueType, delegationMask);
				}
				DoesUserHasPermissionsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(��һ������)
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static bool DoesUserHasPermissions(string userValue, Guid appID, string funcCodeNames, UserValueType userValueType)
		{
			return DoesUserHasPermissions(userValue, appID, funcCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(��һ������)
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static bool DoesUserHasPermissions(string userValue, Guid appID, string funcCodeNames)
		{
			return DoesUserHasPermissions(userValue, appID, funcCodeNames, UserValueType.LogonName);
		}
		#endregion DoesUserHasPermissions

		#region DoesUserHasAllPermissions
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(ӵ��ȫ������)
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static bool DoesUserHasAllPermissions(string userValue, string appCodeName, string funcCodeNames, UserValueType userValueType, DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appCodeName, funcCodeNames, userValueType, delegationMask);
			bool result;
			//if (false == DoesUserHasAllPermissionsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(DoesUserHasAllPermissionsQueue))//.CacheQueueSync)
			//    {
			if (false == DoesUserHasAllPermissionsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = DoesUserHasAllPermissions(strUserIDs, userValue, appCodeName, funcCodeNames, delegationMask);
					}
					else
					{
						result = DoesUserHasAllPermissions(strUserIDs, string.Empty, appCodeName, funcCodeNames, delegationMask);
					}
				}
				DoesUserHasAllPermissionsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(ӵ��ȫ������)
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static bool DoesUserHasAllPermissions(string userValue, string appCodeName, string funcCodeNames, UserValueType userValueType)
		{
			return DoesUserHasAllPermissions(userValue, appCodeName, funcCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(ӵ��ȫ������)
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static bool DoesUserHasAllPermissions(string userValue, string appCodeName, string funcCodeNames)
		{
			return DoesUserHasAllPermissions(userValue, appCodeName, funcCodeNames, UserValueType.LogonName);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(ӵ��ȫ������)
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static bool DoesUserHasAllPermissions(string userValue,
			Guid appID,
			string funcCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appID, funcCodeNames, userValueType, delegationMask);
			bool result;
			//if (false == DoesUserHasAllPermissionsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(DoesUserHasAllPermissionsQueue))//.CacheQueueSync)
			//    {
			if (false == DoesUserHasAllPermissionsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string appCodeName = GetApplicatonCodeName(appID);
					if (appCodeName == string.Empty)
						return false;

					result = DoesUserHasAllPermissions(userValue, appCodeName, funcCodeNames, userValueType, delegationMask);
				}
				DoesUserHasAllPermissionsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(ӵ��ȫ������)
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static bool DoesUserHasAllPermissions(string userValue, Guid appID, string funcCodeNames, UserValueType userValueType)
		{
			return DoesUserHasAllPermissions(userValue, appID, funcCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(ӵ��ȫ������)
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appID">Ӧ�õ�GUID</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static bool DoesUserHasAllPermissions(string userValue, Guid appID, string funcCodeNames)
		{
			return DoesUserHasAllPermissions(userValue, appID, funcCodeNames, UserValueType.LogonName);
		}
		#endregion DoesUserHasAllPermissions

		#region GetUserRoles
		/// <summary>
		/// ��ѯָ���û�����ָ��Ӧ������ӵ�еĽ�ɫ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static DataSet GetUserRoles(string userValue,
			string appCodeName,
			UserValueType userValueType,
			RightMaskType rightMask,
			DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appCodeName, userValueType, rightMask, delegationMask);
			DataSet result;

			if (false == GetUserRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = GetUserRoles(strUserIDs, userValue, appCodeName, rightMask, delegationMask);
					}
					else
					{
						result = GetUserRoles(strUserIDs, string.Empty, appCodeName, rightMask, delegationMask);
					}
				}

				GetUserRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ���û�����ָ��Ӧ������ӵ�еĽ�ɫ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <returns></returns>
		public static DataSet GetUserRoles(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask)
		{
			return GetUserRoles(userValue, appCodeName, userValueType, rightMask, DelegationMaskType.All);
		}
		/// <summary>
		/// ��ѯָ���û�����ָ��Ӧ������ӵ�еĽ�ɫ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetUserRoles(string userValue, string appCodeName, UserValueType userValueType)
		{
			return GetUserRoles(userValue, appCodeName, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// ��ѯָ���û�����ָ��Ӧ������ӵ�еĽ�ɫ
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <returns></returns>
		public static DataSet GetUserRoles(string userValue, string appCodeName)
		{
			return GetUserRoles(userValue, appCodeName, UserValueType.LogonName);
		}
		#endregion GetUserRoles

		#region GetUserAllowDelegteRoles
		/// <summary>
		/// ��ѯָ���û�����ָ��Ӧ������ӵ�еģ��ɽ���ί�ɵĽ�ɫ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <returns></returns>
		public static DataSet GetUserAllowDelegteRoles(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appCodeName, userValueType, rightMask);
			DataSet result;
			//if (false == GetUserAllowDelegteRolesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUserAllowDelegteRolesQueue))//.CacheQueueSync)
			//    {
			if (false == GetUserAllowDelegteRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = GetUserAllowDelegteRoles(strUserIDs, userValue, appCodeName, rightMask);
					}
					else
					{
						result = GetUserAllowDelegteRoles(strUserIDs, string.Empty, appCodeName, rightMask);
					}
				}
				GetUserAllowDelegteRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ���û�����ָ��Ӧ������ӵ�еģ��ɽ���ί�ɵĽ�ɫ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetUserAllowDelegteRoles(string userValue, string appCodeName, UserValueType userValueType)
		{
			return GetUserAllowDelegteRoles(userValue, appCodeName, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// ��ѯָ���û�����ָ��Ӧ������ӵ�еģ��ɽ���ί�ɵĽ�ɫ
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <returns></returns>
		public static DataSet GetUserAllowDelegteRoles(string userValue, string appCodeName)
		{
			return GetUserAllowDelegteRoles(userValue, appCodeName, UserValueType.LogonName);
		}
		#endregion GetUserAllowDelegteRoles

		#region GetUserPermissions
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�þ��е�Ȩ�ޣ����ܣ�
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static DataSet GetUserPermissions(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appCodeName, userValueType, rightMask, delegationMask);
			DataSet result;
			//if (false == GetUserPermissionsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUserPermissionsQueue))//.CacheQueueSync)
			//    {
			if (false == GetUserPermissionsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = GetUserPermissions(strUserIDs, userValue, appCodeName, rightMask, delegationMask);
					}
					else
					{
						result = GetUserPermissions(strUserIDs, string.Empty, appCodeName, rightMask, delegationMask);
					}
				}
				GetUserPermissionsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�þ��е�Ȩ�ޣ����ܣ�
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <returns></returns>
		public static DataSet GetUserPermissions(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask)
		{
			return GetUserPermissions(userValue, appCodeName, userValueType, rightMask, DelegationMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�þ��е�Ȩ�ޣ����ܣ�
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetUserPermissions(string userValue, string appCodeName, UserValueType userValueType)
		{
			return GetUserPermissions(userValue, appCodeName, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�þ��е�Ȩ�ޣ����ܣ�
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <returns></returns>
		public static DataSet GetUserPermissions(string userValue, string appCodeName)
		{
			return GetUserPermissions(userValue, appCodeName, UserValueType.LogonName);
		}
		#endregion GetUserPermissions

		#region GetUserApplicationsRoles
		/// <summary>
		/// ��ѯָ����Ա��Ӧ�ý�ɫ��Ϣ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsRoles(string userValue, UserValueType userValueType, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, userValueType, rightMask, delegationMask);
			DataSet result;
			//if (false == GetUserApplicationsRolesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUserApplicationsRolesQueue))//.CacheQueueSync)
			//    {
			if (false == GetUserApplicationsRolesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = GetUserApplicationsRoles(strUserIDs, userValue, rightMask, delegationMask);
					}
					else
					{
						result = GetUserApplicationsRoles(strUserIDs, string.Empty, rightMask, delegationMask);
					}
				}
				GetUserApplicationsRolesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա��Ӧ�ý�ɫ��Ϣ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsRoles(string userValue, UserValueType userValueType, RightMaskType rightMask)
		{
			return GetUserApplicationsRoles(userValue, userValueType, rightMask, DelegationMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա��Ӧ�ý�ɫ��Ϣ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsRoles(string userValue, UserValueType userValueType)
		{
			return GetUserApplicationsRoles(userValue, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա��Ӧ�ý�ɫ��Ϣ
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsRoles(string userValue)
		{
			return GetUserApplicationsRoles(userValue, UserValueType.LogonName);
		}
		#endregion GetUserApplicationsRoles

		#region GetUserApplications
		/// <summary>
		/// ��ѯָ����Ա��ӵ�е�����Ӧ����Ϣ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static DataSet GetUserApplications(string userValue, UserValueType userValueType, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, userValueType, rightMask, delegationMask);
			DataSet result;
			//if (false == GetUserApplicationsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUserApplicationsQueue))//.CacheQueueSync)
			//    {
			if (false == GetUserApplicationsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = GetUserApplications(strUserIDs, userValue, rightMask, delegationMask);
					}
					else
					{
						result = GetUserApplications(strUserIDs, string.Empty, rightMask, delegationMask);
					}
				}
				GetUserApplicationsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա��ӵ�е�����Ӧ����Ϣ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <returns></returns>
		public static DataSet GetUserApplications(string userValue, UserValueType userValueType, RightMaskType rightMask)
		{
			return GetUserApplications(userValue, userValueType, rightMask, DelegationMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա��ӵ�е�����Ӧ����Ϣ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetUserApplications(string userValue, UserValueType userValueType)
		{
			return GetUserApplications(userValue, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա��ӵ�е�����Ӧ����Ϣ
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <returns></returns>
		public static DataSet GetUserApplications(string userValue)
		{
			return GetUserApplications(userValue, UserValueType.LogonName);
		}
		#endregion GetUserApplications

		#region GetUserApplicationsForDelegation
		/// <summary>
		/// ��ѯָ����Ա��ӵ�е����пɽ���ί�ɲ�����Ӧ����Ϣ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="rightMask">Ȩ������</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsForDelegation(string userValue, UserValueType userValueType, RightMaskType rightMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, userValueType, rightMask);
			DataSet result;
			//if (false == GetUserApplicationsForDelegationQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUserApplicationsForDelegationQueue))//.CacheQueueSync)
			//    {
			if (false == GetUserApplicationsForDelegationQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = GetUserApplicationsForDelegation(strUserIDs, userValue, rightMask);
					}
					else
					{
						result = GetUserApplicationsForDelegation(strUserIDs, string.Empty, rightMask);
					}
				}
				GetUserApplicationsForDelegationQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա��ӵ�е����пɽ���ί�ɲ�����Ӧ����Ϣ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsForDelegation(string userValue, UserValueType userValueType)
		{
			return GetUserApplicationsForDelegation(userValue, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա��ӵ�е����пɽ���ί�ɲ�����Ӧ����Ϣ
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsForDelegation(string userValue)
		{
			return GetUserApplicationsForDelegation(userValue, UserValueType.LogonName);
		}
		#endregion GetUserApplicationsForDelegation

		#region GetUserRolesScopes
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <param name="scopeMask">����Χ����</param>
		/// <returns></returns>
		public static DataSet GetUserRolesScopes(string userValue,
			string appCodeName,
			string roleCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask,
			ScopeMaskType scopeMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appCodeName, roleCodeNames, userValueType, delegationMask, scopeMask);
			DataSet result;
			//if (false == GetUserRolesScopesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUserRolesScopesQueue))//.CacheQueueSync)
			//    {
			if (false == GetUserRolesScopesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = GetUserRolesScopes(strUserIDs, userValue, appCodeName, roleCodeNames, delegationMask, scopeMask);
					}
					else
					{
						result = GetUserRolesScopes(strUserIDs, string.Empty, appCodeName, roleCodeNames, delegationMask, scopeMask);
					}
				}
				GetUserRolesScopesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static DataSet GetUserRolesScopes(string userValue,
			string appCodeName,
			string roleCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask)
		{
			return GetUserRolesScopes(userValue, appCodeName, roleCodeNames, userValueType, delegationMask, ScopeMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetUserRolesScopes(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType)
		{
			return GetUserRolesScopes(userValue, appCodeName, roleCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="roleCodeNames">��ɫ��Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static DataSet GetUserRolesScopes(string userValue, string appCodeName, string roleCodeNames)
		{
			return GetUserRolesScopes(userValue, appCodeName, roleCodeNames, UserValueType.LogonName);
		}
		#endregion GetUserRolesScopes

		#region GetUserFunctionsScopes
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <param name="scopeMask">����Χ����</param>
		/// <returns></returns>
		public static DataSet GetUserFunctionsScopes(string userValue,
			string appCodeName,
			string funcCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask,
			ScopeMaskType scopeMask)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(userValue, appCodeName, funcCodeNames, userValueType, delegationMask, scopeMask);
			DataSet result;
			//if (false == GetUserFunctionsScopesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUserFunctionsScopesQueue))//.CacheQueueSync)
			//    {
			if (false == GetUserFunctionsScopesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AppConnAlias))
				{
					string strUserIDs = string.Empty;
					strUserIDs = GetUserGuids(userValue, userValueType);
					if (userValueType == UserValueType.AllPath)
					{
						result = GetUserFunctionsScopes(strUserIDs, userValue, appCodeName, funcCodeNames, delegationMask, scopeMask);
					}
					else
					{
						result = GetUserFunctionsScopes(strUserIDs, string.Empty, appCodeName, funcCodeNames, delegationMask, scopeMask);
					}
				}
				GetUserFunctionsScopesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <param name="delegationMask">ί������</param>
		/// <returns></returns>
		public static DataSet GetUserFunctionsScopes(string userValue,
			string appCodeName,
			string funcCodeNames,
			UserValueType userValueType,
			DelegationMaskType delegationMask)
		{
			return GetUserFunctionsScopes(userValue, appCodeName, funcCodeNames, userValueType, delegationMask, ScopeMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userValue">�û���ݱ�ʶ����userValueType����ָ�����ͣ�</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <param name="userValueType">�û���ݱ�ʶ���ͣ�UserValueType.LogonName:��¼��, UserValueType.AllPath:ȫ·����</param>
		/// <returns></returns>
		public static DataSet GetUserFunctionsScopes(string userValue, string appCodeName, string funcCodeNames, UserValueType userValueType)
		{
			return GetUserFunctionsScopes(userValue, appCodeName, funcCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userValue">�û���¼��</param>
		/// <param name="appCodeName">Ӧ�õ�Ӣ�ı�ʶ</param>
		/// <param name="funcCodeNames">���ܵ�Ӣ�ı�ʶ�����ʱ�ö��ŷָ�</param>
		/// <returns></returns>
		public static DataSet GetUserFunctionsScopes(string userValue, string appCodeName, string funcCodeNames)
		{
			return GetUserFunctionsScopes(userValue, appCodeName, funcCodeNames, UserValueType.LogonName);
		}
		#endregion GetUserFunctionsScopes

		#region RemoveAllCache
		/// <summary>
		/// �������ݻ���
		/// </summary>
		public static void RemoveAllCache()
		{
			InnerCacheHelper.RemoveAllCache();
		}
		#endregion
		#endregion public functions

		#region private functions
		#region GetRoles
		/// <summary>
		/// �������:��ѯָ��Ӧ���У�ָ���������н�ɫ
		/// </summary>
		/// <param name="appCodeName"></param>
		/// <param name="rightMask"></param>
		/// <returns></returns>
		private static string GetRoles_SqlStr(string appCodeName, RightMaskType rightMask)
		{
			string strSql = @"
SELECT R.ID, R.APP_ID, R.NAME, R.CODE_NAME, R.DESCRIPTION, R.CLASSIFY, R.SORT_ID, R.ALLOW_DELEGATE, R.INHERITED 
FROM ROLES R, APPLICATIONS A
WHERE R.APP_ID = A.ID
	AND A.CODE_NAME = {0}
";
			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));

			ExceptionHelper.TrueThrow<ApplicationException>((rightMask & RightMaskType.All) == RightMaskType.None,
				"����ȷ�趨Ҫ��ѯ�Ľ�ɫ���ͣ�");
			string strTypeCondition = string.Empty;
			rightMask = rightMask & RightMaskType.All;
			switch (rightMask)
			{
				case RightMaskType.Self:
					strTypeCondition = " AND R.CLASSIFY = 'y'";
					break;
				case RightMaskType.App:
					strTypeCondition = " AND R.CLASSIFY = 'n'";
					break;
				case RightMaskType.All:
					strTypeCondition = "";
					break;
			}
			return strSql + strTypeCondition;
		}

		#endregion GetRoles

		#region GetFunctions
		/// <summary>
		/// �������:��ѯָ��Ӧ���У�ָ���������й���
		/// </summary>
		/// <param name="appCodeName"></param>
		/// <param name="rightMask">rightMask(����): 1:����Ȩ; 2:Ӧ����Ȩ; 3:ȫ��	</param>
		/// <returns></returns>
		private static string GetFunctions_SqlStr(string appCodeName, RightMaskType rightMask)
		{
			string strSql = @"
SELECT F.ID, F.APP_ID, F.NAME, F.CODE_NAME, F.DESCRIPTION, F.SORT_ID, F.CLASSIFY, F.INHERITED  
FROM FUNCTIONS F, APPLICATIONS A
WHERE F.APP_ID = A.ID
	AND A.CODE_NAME = {0}
";
			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));

			//ExceptionHelper.TrueThrow<ApplicationException>((rightMask & RightMaskType.All) == RightMaskType.None, "����ȷ�趨Ҫ��ѯ�Ĺ������ͣ�");
			string strTypeCondition = string.Empty;
			rightMask = rightMask & RightMaskType.All;
			switch (rightMask)
			{
				case RightMaskType.Self:
					strTypeCondition = " AND F.CLASSIFY = 'y'";
					break;
				case RightMaskType.App:
					strTypeCondition = " AND F.CLASSIFY = 'n'";
					break;
				case RightMaskType.All:
					strTypeCondition = "";
					break;
			}

			return strSql + strTypeCondition;
		}
		#endregion GetFunctions

		#region InnerGetRolesUsers
		//��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ��ָ������������Ա
		private static DataSet InnerGetRolesUsers(string orgRoot,
			string appCodeName,
			string roleCodeNames,
			DelegationMaskType delegationMask,
			SidelineMaskType sidelineMask,
			string extAttr)
		{
			//ȡԭ��Ȩ����Ա
			//����extAttr����Ч
			string strSql = GetRolesUsers_SqlStr(orgRoot, appCodeName, roleCodeNames, sidelineMask, extAttr);

			if (strSql == string.Empty)
			{
				//���û�б��ʽ
				if (extAttr == string.Empty)
				{
					strSql = @" 
SELECT 'USERS' OBJECTCLASS, OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, 
	OU.GLOBAL_SORT, OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, OU.START_TIME, 
	OU.END_TIME, OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, OU.OUSYSCONTENT3, U.GUID, 
	U.FIRST_NAME, U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, U.POSTURAL, U.PERSON_ID, U.SYSDISTINCT1, 
	U.SYSDISTINCT2, U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3, RD.CODE_NAME, RD.SORT_ID, RD.NAME 
FROM OU_USERS OU INNER JOIN USERS U 
		ON OU.USER_GUID = U.GUID
	INNER JOIN RANK_DEFINE RD 
		ON U.RANK_CODE = RD.CODE_NAME
WHERE 1>1 ";
					return OGUCommonDefine.ExecuteDataset(strSql);
				}
				else
				{
					return OGUReader.GetObjectsDetail("USERS",
						string.Empty,
						SearchObjectColumn.SEARCH_LOGON_NAME,
						string.Empty,
						SearchObjectColumn.SEARCH_NULL,
						extAttr);
				}
			}


			DataSet returnDS = null;
			if (extAttr == string.Empty)
			{
				strSql += "\n ORDER BY OU.GLOBAL_SORT, RD.SORT_ID, OU.SIDELINE";
				returnDS = OGUCommonDefine.ExecuteDataset(strSql);
			}
			else
			{
				DataTable dt = OGUCommonDefine.ExecuteDataset(strSql).Tables[0];
				string IDs = GetColumnValue(dt, "USER_GUID");
				returnDS = OGUReader.GetObjectsDetail("USERS",
					IDs,
					SearchObjectColumn.SEARCH_GUID,
					string.Empty,
					SearchObjectColumn.SEARCH_GUID,
					extAttr);

				//���˷�ѡ����ְλ
				string strIDpIDs = string.Empty;
				foreach (DataRow row in dt.Rows)
				{
					if (strIDpIDs == string.Empty)
						strIDpIDs += row["USER_GUID"].ToString() + row["PARENT_GUID"].ToString();
					else
						strIDpIDs += "," + row["USER_GUID"].ToString() + row["PARENT_GUID"].ToString();

				}
				for (int i = returnDS.Tables[0].Rows.Count - 1; i >= 0; i--)
				{
					DataRow row2 = returnDS.Tables[0].Rows[i];
					//str = row2["USER_GUID"].ToString() + row2["PARENT_GUID"].ToString();//Delete By Yuanyong 20080724
					string str = row2["GUID"].ToString() + row2["PARENT_GUID"].ToString();// Add By yuanyong 20080724
					if (strIDpIDs.IndexOf(str) == -1)
						returnDS.Tables[0].Rows.Remove(row2);
				}
				returnDS.AcceptChanges();

			}


			//��ί��Ȩ������Ա??
			DataSet delegateDS = null;
			if ((returnDS.Tables[0].Rows.Count) > 0 && (delegationMask & DelegationMaskType.Delegated) != DelegationMaskType.None)
			{
				//�õ���ǰ��ɫ�£���Ч��ί��
				strSql = @"
SELECT * 
FROM DELEGATIONS D, ROLES R, APPLICATIONS A
WHERE D.ROLE_ID = R.ID
	AND R.APP_ID = A.ID
	AND A.CODE_NAME = {0}
	AND R.CODE_NAME IN ({1})
	AND R.ALLOW_DELEGATE = 'y'
	AND D.START_TIME < GETDATE()
	AND D.END_TIME > GETDATE()
ORDER BY D.ROLE_ID, D.SOURCE_ID;";
				strSql = string.Format(strSql,
					TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true),
					OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames));

				DataSet ds = OGUCommonDefine.ExecuteDataset(strSql);


				if (ds.Tables[0].Rows.Count > 0)
				{
					string dUserIDs = "''";
					string strTemp = string.Empty;
					string strTemp2 = string.Empty;
					foreach (DataRow row in ds.Tables[0].Rows)
					{
						//ί�������ڽ�ɫ
						strTemp = string.Format("[USER_GUID] = '{0}'", row["SOURCE_ID"]);
						//��ί���߲����ڽ�ɫ
						strTemp2 = string.Format("[USER_GUID] = '{0}'", row["TARGET_ID"]);
						if (returnDS.Tables[0].Select(strTemp).Length > 0 && returnDS.Tables[0].Select(strTemp2).Length == 0)
						{
							dUserIDs += string.Format(",'{0}'", row["TARGET_ID"]);
						}

					}
					//����ί����
					if (dUserIDs.Length > 2)
					{
						string strSql2 = string.Empty;
						if (extAttr == string.Empty)
						{
							strSql2 = @"
SELECT 'USERS' OBJECTCLASS, OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, 
	OU.GLOBAL_SORT, OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, OU.START_TIME, 
	OU.END_TIME, OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, OU.OUSYSCONTENT3 ,U.GUID, 
	U.FIRST_NAME, U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, U.POSTURAL, U.PERSON_ID, U.SYSDISTINCT1, 
	U.SYSDISTINCT2, U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3, RD.CODE_NAME, RD.SORT_ID, RD.NAME ";
						}
						else
						{
							strSql2 = @"OU.PARENT_GUID, OU.USER_GUID";
						}

						strSql2 += string.Format(@" 
FROM OU_USERS OU
	INNER JOIN USERS U 
		ON OU.USER_GUID = U.GUID
	INNER JOIN RANK_DEFINE RD 
		ON U.RANK_CODE = RD.CODE_NAME
WHERE OU.USER_GUID IN ({0})
	AND OU.STATUS = 1 
	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME
ORDER BY OU.GLOBAL_SORT, RD.SORT_ID, OU.SIDELINE", dUserIDs);
						//***********
						//ί�ɵ��˲����˻�����ԭ��Ȩ���ڷ�Χ�У��϶���ί�����ڷ�Χ�С�
						//***********

						//�����ְ
						if ((sidelineMask & SidelineMaskType.All) != SidelineMaskType.All)
						{
							if (sidelineMask == SidelineMaskType.NotSideline) //��ְ
							{
								strSql2 += " AND SIDELINE = 0";
							}

							if (sidelineMask == SidelineMaskType.Sideline) //��ְ
							{
								strSql2 += " AND SIDELINE = 1";
							}
						}
						delegateDS = OGUCommonDefine.ExecuteDataset(strSql2);

						if (extAttr != string.Empty)
						{
							string IDs = GetColumnValue(delegateDS.Tables[0], "USER_GUID");
							string pIDs = GetColumnValue(delegateDS.Tables[0], "PARENT_GUID");
							delegateDS = OGUReader.GetObjectsDetail("USERS",
								IDs,
								SearchObjectColumn.SEARCH_GUID,
								string.Empty,
								SearchObjectColumn.SEARCH_GUID,
								extAttr);
						}
					}
				}

			}

			if (delegationMask == DelegationMaskType.Delegated)
			{
				if (delegateDS == null)
				{
					if (extAttr == string.Empty)
					{
						string strSql3 = @" 
SELECT 'USERS' OBJECTCLASS, OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, 
	OU.GLOBAL_SORT, OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, OU.START_TIME, 
	OU.END_TIME, OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, OU.OUSYSCONTENT3 ,U.GUID, 
	U.FIRST_NAME, U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, U.POSTURAL, U.PERSON_ID, U.SYSDISTINCT1, 
	U.SYSDISTINCT2, U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3, RD.CODE_NAME, RD.SORT_ID, RD.NAME 
FROM OU_USERS OU
	INNER JOIN USERS U 
		ON OU.USER_GUID = U.GUID
	INNER JOIN RANK_DEFINE RD 
		ON U.RANK_CODE = RD.CODE_NAME
WHERE 1>1 ";
						delegateDS = OGUCommonDefine.ExecuteDataset(strSql3);
					}
					else
					{
						delegateDS = OGUReader.GetObjectsDetail("",
							string.Empty,
							SearchObjectColumn.SEARCH_GUID,
							string.Empty,
							SearchObjectColumn.SEARCH_NULL,
							extAttr);
					}
				}
				returnDS = delegateDS;
			}
			else if (delegationMask == DelegationMaskType.All)
			{
				if (delegateDS != null)
				{
					returnDS.Merge(delegateDS);
				}
			}

			return returnDS;

		}
		//��ѯָ�������£�ָ��Ӧ���У�ָ����ɫ��ָ������������Ա(����ְλ)
		private static DataSet InnerGetRolesUsers(string orgRoot, string appCodeName, string roleCodeNames, DelegationMaskType delegationMask, string extAttr)
		{
			return InnerGetRolesUsers(orgRoot, appCodeName, roleCodeNames, delegationMask, SidelineMaskType.All, extAttr);
		}
		private static string GetRolesUsers_SqlStr(string orgRoot, string appCodeName, string roleCodeNames, SidelineMaskType sidelineMask, string extAttr)
		{
			//�������ʽ�������������д�������У���Ϊ�������ӵ�����
			//strSql = ParseExpsToTable_SqlStr(expDT, out bIncludeUser, out bIncludeOrg, out bIncludeGroup ) ;

			//�õ����еı��ʽ
			string strWhereSql = @"WHERE ROLE_ID IN 
								(SELECT ID FROM ROLES WHERE CODE_NAME IN 
									({0})
									AND APP_ID IN (SELECT ID FROM APPLICATIONS WHERE CODE_NAME = {1}) )";
			strWhereSql = string.Format(strWhereSql, OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames),
				TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));

			//�õ������ŵ�����
			string[] arrRootSorts = new string[0];
			if (orgRoot != string.Empty)
			{
				arrRootSorts = GetOrgSorts(orgRoot);
			}

			string strSql = ParseExpsToTable_SqlStr2(strWhereSql);


			//���ҳ������µ����л�����Guid
			strSql += @"
DECLARE @ORG_GUID TABLE
(
	[GUID] [nvarchar] (36) NOT NULL,
	[EXP_GUID] [nvarchar] (36) NOT NULL,
	[SORT_ID] [int] NOT NULL 
);

INSERT INTO @ORG_GUID 
SELECT  O1.GUID, T.ID, T.SORT_ID 
FROM ORGANIZATIONS O2
	INNER JOIN @EXP_TABLE T ON O2.GUID = T.OBJ_GUID  
	INNER JOIN ORGANIZATIONS O1 ON O1.GLOBAL_SORT LIKE O2.GLOBAL_SORT + '%'	
WHERE T.CLASSIFY = '1'
						";
			if (arrRootSorts.Length > 0)
			{
				strSql += " AND ( 1>1\n";
				foreach (string strTemp in arrRootSorts)
				{
					//strSql += "";
					strSql += string.Format(" OR O2.GLOBAL_SORT LIKE '{0}%'", strTemp);
				}
				strSql += " ) \n";
			}

			if (extAttr == string.Empty)
			{
				strSql += @" 
SELECT 'USERS' OBJECTCLASS, OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, OU.GLOBAL_SORT,
	OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, OU.START_TIME, OU.END_TIME, 
	OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, OU.OUSYSCONTENT3 , U.GUID, U.FIRST_NAME, 
	U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, U.POSTURAL, U.PERSON_ID, U.SYSDISTINCT1, U.SYSDISTINCT2, 
	U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3 ,RD.CODE_NAME, RD.SORT_ID, RD.NAME 
FROM OU_USERS OU
	INNER JOIN USERS U ON OU.USER_GUID = U.GUID
	INNER JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME
WHERE 1 > 1 ";
			}
			else
			{
				strSql += "SELECT '' USER_GUID, '' PARENT_GUID";
			}


			//��Ա
			strSql += string.Format("\n UNION \n {0}", GetExpressionsUsers_SqlStr("USERS", arrRootSorts, sidelineMask, extAttr));
			//����
			strSql += string.Format("\n UNION \n {0}", GetExpressionsUsers_SqlStr("ORGANIZATIONS", arrRootSorts, sidelineMask, extAttr));
			//��
			strSql += string.Format("\n UNION \n {0}", GetExpressionsUsers_SqlStr("GROUPS", arrRootSorts, sidelineMask, extAttr));

			return strSql;
		}
		private static string[] GetOrgSorts(string strOrgRoots)
		{
			if (strOrgRoots == string.Empty)
				return new string[0];

			string[] arrRoots = strOrgRoots.Split(',');
			for (int i = 0; i < arrRoots.Length; i++)
			{
				if (arrRoots[i] == string.Empty)
					continue;
				//ȥ����β��'\'
				if (arrRoots[i].LastIndexOf("\\") == arrRoots[i].Length - 1)
					arrRoots[i] = arrRoots[i].Substring(0, arrRoots[i].Length - 1);
			}

			strOrgRoots = string.Join(",", arrRoots);

			string sql = string.Format("SELECT GLOBAL_SORT FROM ORGANIZATIONS WHERE ALL_PATH_NAME IN ({0})",
				OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgRoots));

			StringBuilder builder = new StringBuilder(128);
			DataSet reader = OGUCommonDefine.ExecuteDataset(sql);
			foreach (DataRow row in reader.Tables[0].Rows)
			{
				if (builder.Length > 0)
					builder.Append(",");
				builder.Append((string)row["GLOBAL_SORT"]);
			}
			if (builder.Length == 0)
				builder.Append("------");//20070807�������Ҫ������

			return builder.ToString().Split(',');
		}
		/// <summary>
		/// ���ѽ����ı�����ı��У��õ�ָ����ı��ʽ���漰��������Ա
		/// </summary>
		/// <param name="strObjType">���ʽ����</param>
		/// <param name="arrRootSorts">����������</param>
		/// <param name="sidelineMask">ְλ����</param>
		/// <param name="extAttr">��ȡ�������չ����</param>
		/// <returns>SQL���</returns>
		private static string GetExpressionsUsers_SqlStr(string strObjType, string[] arrRootSorts, SidelineMaskType sidelineMask, string extAttr)
		{
			string strSql = string.Empty;

			//��Ա
			if (strObjType.ToUpper() == "USERS")
			{
				if (extAttr == string.Empty)
				{
					strSql = @" 
SELECT 'USERS' OBJECTCLASS, OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, OU.GLOBAL_SORT, 
	OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, OU.START_TIME, OU.END_TIME, 
	OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, OU.OUSYSCONTENT3, U.GUID, U.FIRST_NAME, 
	U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, U.POSTURAL, U.PERSON_ID, U.SYSDISTINCT1, U.SYSDISTINCT2, 
	U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3, RD.CODE_NAME, RD.SORT_ID, RD.NAME ";
				}
				else
				{
					strSql = @" 
SELECT OU.USER_GUID, OU.PARENT_GUID ";
				}
				strSql += @" 
FROM OU_USERS OU 
	INNER JOIN @EXP_TABLE T ON OU.USER_GUID = T.OBJ_GUID AND OU.PARENT_GUID = T.PARENT_GUID
	INNER JOIN USERS U ON OU.USER_GUID = U.GUID
	INNER JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME
WHERE T.CLASSIFY = '3'
	AND OU.STATUS = 1 
	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME AND @USR_COONT > 0 ";
			}

			//����
			if (strObjType.ToUpper() == "ORGANIZATIONS")
			{
				if (extAttr == string.Empty)
				{
					strSql = @"
SELECT 'USERS' OBJECTCLASS, OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, 
	OU.GLOBAL_SORT, OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, OU.START_TIME, 
	OU.END_TIME, OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, OU.OUSYSCONTENT3, U.GUID, 
	U.FIRST_NAME, U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, U.POSTURAL, U.PERSON_ID, U.SYSDISTINCT1, 
	U.SYSDISTINCT2, U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3, RD.CODE_NAME, RD.SORT_ID, RD.NAME ";
				}
				else
				{
					strSql = @" 
SELECT OU.USER_GUID, OU.PARENT_GUID ";
				}
				strSql += @" 
FROM OU_USERS OU 
	INNER JOIN ORGANIZATIONS O ON OU.PARENT_GUID = O.GUID
	INNER JOIN @ORG_GUID O_ID ON O.GUID = O_ID.GUID
	INNER JOIN @EXP_TABLE T ON O_ID.EXP_GUID = T.ID
	INNER JOIN USERS U ON OU.USER_GUID = U.GUID
	INNER JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME
WHERE T.CLASSIFY = '1'
	AND OU.ORIGINAL_SORT LIKE O.ORIGINAL_SORT + '%'
	AND RD.SORT_ID <= T.SORT_ID --�����޶�
	AND OU.STATUS = 1 
	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME 
	AND @DPT_COUNT > 0 ";
			}
			//��
			if (strObjType.ToUpper() == "GROUPS")
			{
				if (extAttr == string.Empty)
				{
					strSql = @" 
SELECT 'USERS' OBJECTCLASS, OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, 
	OU.GLOBAL_SORT, OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, OU.START_TIME, 
	OU.END_TIME, OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, OU.OUSYSCONTENT3, U.GUID, 
	U.FIRST_NAME, U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, U.POSTURAL, U.PERSON_ID, U.SYSDISTINCT1, 
	U.SYSDISTINCT2, U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3, RD.CODE_NAME, RD.SORT_ID, RD.NAME  ";
				}
				else
				{
					strSql = @" 
SELECT OU.USER_GUID, OU.PARENT_GUID ";
				}
				strSql += @" 
FROM @EXP_TABLE T
	INNER JOIN GROUPS G ON G.GUID = T.OBJ_GUID AND G.PARENT_GUID = T.PARENT_GUID
	INNER JOIN GROUP_USERS GU ON GU.GROUP_GUID = G.GUID
	INNER JOIN OU_USERS OU ON OU.USER_GUID = GU.USER_GUID AND OU.PARENT_GUID = GU.USER_PARENT_GUID
	INNER JOIN USERS U ON OU.USER_GUID = U.GUID
	INNER JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME
WHERE T.CLASSIFY = '2'
	AND RD.SORT_ID <= T.SORT_ID --�����޶�
	AND OU.STATUS = 1 
	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME 
	AND @GRP_COUNT > 0 ";
			}

			//����������޶�
			if (arrRootSorts.Length > 0)
			{
				strSql += " AND ( 1 > 1 ";
				foreach (string strTemp in arrRootSorts)
				{
					strSql += string.Format(" OR OU.GLOBAL_SORT LIKE '{0}%'", strTemp);
				}
				strSql += " ) ";
			}

			//�����ְ
			if ((sidelineMask & SidelineMaskType.All) != SidelineMaskType.All)
			{
				if (sidelineMask == SidelineMaskType.NotSideline) //��ְ
					strSql += " AND OU.SIDELINE = 0";

				if (sidelineMask == SidelineMaskType.Sideline) //��ְ
					strSql += " AND OU.SIDELINE = 1";
			}

			return strSql;

		}

		#endregion GetRolesUsers

		#region GetChildrenInRoles
		private static string GetExpressionsDedail_SqlStr(string strObjType, string[] arrRootSorts, bool bExpandGroup)
		{
			string userParentLimit = AccreditSection.GetConfig().AccreditSettings.RoleRelatedUserParentDept ?
				"AND T.PARENT_GUID = OU.PARENT_GUID" : string.Empty;

			string orgParentLimit = AccreditSection.GetConfig().AccreditSettings.RoleRelatedUserParentDept ?
				"AND T.PARENT_GUID = O.PARENT_GUID" : string.Empty;

			string groupParentLimit = AccreditSection.GetConfig().AccreditSettings.RoleRelatedUserParentDept ?
				"AND T.PARENT_GUID = G.PARENT_GUID" : string.Empty;

			string groupOUParentLimit = AccreditSection.GetConfig().AccreditSettings.RoleRelatedUserParentDept ?
				"AND OU.PARENT_GUID = GU.USER_PARENT_GUID" : string.Empty;

			//������ʽ
			StringBuilder strB = new StringBuilder(4096);


			//���չ��������Ա�����ܰ������ʽID����Ϊһ�����ڲ�ͬ����ʱ��������ؼ�¼
			//��չ����ʱ����Ҫ���ʽ��ID����Ϊ��Ҫ���ݱ��ʽID���õ����ʽ�ķ�Χ
			if (bExpandGroup)
			{
				#region ���ڳ�Աչ��
				switch (strObjType.ToUpper())
				{
					case "USERS":
						strB.Append(@"	
SELECT 'USERS' AS OBJECTCLASS, U.PERSON_ID,  NULL AS CUSTOMS_CODE, OU.SIDELINE, OU.RANK_NAME, OU.ALL_PATH_NAME, 
	OU.GLOBAL_SORT, OU.ORIGINAL_SORT, OU.DISPLAY_NAME, OU.OBJ_NAME, U.LOGON_NAME, OU.PARENT_GUID, U.GUID, RD.CODE_NAME, 
	RD.SORT_ID, RD.NAME, T.CLASSIFY, T.ACCESS_LEVEL, T.ACCESS_LEVEL_NAME, T.IsDelegated
FROM @EXP_TABLE T INNER JOIN OU_USERS OU ON T.OBJ_GUID = OU.USER_GUID " + userParentLimit + @" AND OU.STATUS = 1
	INNER JOIN  USERS U ON OU.USER_GUID = U.GUID LEFT JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME
WHERE T.CLASSIFY = '3' ");
						break;
					case "ORGANIZATIONS":
						strB.Append(@"	
SELECT 'ORGANIZATIONS' AS OBJECTCLASS,  NULL AS PERSON_ID, O.CUSTOMS_CODE,  NULL AS SIDELINE,  NULL AS RANK_NAME, O.ALL_PATH_NAME, 
	O.GLOBAL_SORT, O.ORIGINAL_SORT, O.DISPLAY_NAME, O.OBJ_NAME,  NULL AS LOGON_NAME, O.PARENT_GUID, O.GUID, RD.CODE_NAME, 
	RD.SORT_ID, RD.NAME, T.CLASSIFY, T.ACCESS_LEVEL, T.ACCESS_LEVEL_NAME, T.IsDelegated
FROM @EXP_TABLE T INNER JOIN ORGANIZATIONS O ON T.OBJ_GUID = O.GUID " + orgParentLimit + @" AND O.STATUS = 1
	LEFT JOIN RANK_DEFINE RD ON O.RANK_CODE = RD.CODE_NAME
WHERE T.CLASSIFY = '1' ");
						break;
					case "GROUPS":
						strB.Append(@"	
SELECT 'USERS' AS OBJECTCLASS, U.PERSON_ID,  NULL AS CUSTOMS_CODE, OU.SIDELINE, OU.RANK_NAME, OU.ALL_PATH_NAME, 
	OU.GLOBAL_SORT, OU.ORIGINAL_SORT, OU.DISPLAY_NAME, OU.OBJ_NAME, U.LOGON_NAME, OU.PARENT_GUID, U.GUID, RD.CODE_NAME, 
	RD.SORT_ID, RD.NAME, 3 CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, T.IsDelegated
FROM GROUP_USERS GU 
	INNER JOIN @EXP_TABLE T ON GU.GROUP_GUID = T.OBJ_GUID
	INNER JOIN GROUPS G ON G.GUID = GU.GROUP_GUID " + groupParentLimit + @" AND G.STATUS = 1
	INNER JOIN OU_USERS OU ON GU.USER_GUID = OU.USER_GUID " + groupOUParentLimit + @" AND (OU.STATUS = 1)
	INNER JOIN USERS U ON GU.USER_GUID = U.GUID 
	INNER JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME AND RD.SORT_ID <= T.SORT_ID
WHERE T.CLASSIFY = '2' ");

						break;
				}
				#endregion
			}
			else
			{
				#region ���ڳ�Ա��չ��
				switch (strObjType.ToUpper())
				{
					case "USERS":
						strB.Append(@"	
SELECT 'USERS' AS OBJECTCLASS, U.PERSON_ID,  NULL AS CUSTOMS_CODE, OU.SIDELINE, OU.RANK_NAME, OU.ALL_PATH_NAME, 
	OU.GLOBAL_SORT, OU.ORIGINAL_SORT, OU.DISPLAY_NAME, OU.OBJ_NAME, U.LOGON_NAME, OU.PARENT_GUID, U.GUID, RD.CODE_NAME, 
	RD.SORT_ID, RD.NAME, T.CLASSIFY, T.ACCESS_LEVEL, T.ACCESS_LEVEL_NAME, T.ID, T.IsDelegated
FROM @EXP_TABLE T 
	INNER JOIN OU_USERS OU 
		ON T.OBJ_GUID = OU.USER_GUID " + userParentLimit + @" AND OU.STATUS = 1
	INNER JOIN  USERS U 
		ON OU.USER_GUID = U.GUID LEFT JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME
WHERE T.CLASSIFY = '3' ");
						break;
					case "ORGANIZATIONS":
						strB.Append(@"	
SELECT 'ORGANIZATIONS' AS OBJECTCLASS,  NULL AS PERSON_ID, O.CUSTOMS_CODE,  NULL AS SIDELINE,   NULL AS RANK_NAME, O.ALL_PATH_NAME, 
	O.GLOBAL_SORT, O.ORIGINAL_SORT, O.DISPLAY_NAME, O.OBJ_NAME,  NULL AS LOGON_NAME, O.PARENT_GUID, O.GUID, RD.CODE_NAME, 
	RD.SORT_ID, RD.NAME, T.CLASSIFY, T.ACCESS_LEVEL, T.ACCESS_LEVEL_NAME, T.ID, T.IsDelegated
FROM @EXP_TABLE T 
	INNER JOIN ORGANIZATIONS O 
		ON T.OBJ_GUID = O.GUID " + orgParentLimit + @" AND O.STATUS = 1
	LEFT JOIN RANK_DEFINE RD 
		ON O.RANK_CODE = RD.CODE_NAME
WHERE T.CLASSIFY = '1' ");
						break;
					case "GROUPS":
						strB.Append(@"	
SELECT 'GROUPS' AS OBJECTCLASS,  NULL AS PERSON_ID,  NULL AS CUSTOMS_CODE,  NULL AS SIDELINE,  NULL AS RANK_NAME, G.ALL_PATH_NAME, 
	G.GLOBAL_SORT, G.ORIGINAL_SORT, G.DISPLAY_NAME, G.OBJ_NAME,  NULL AS LOGON_NAME, G.PARENT_GUID, G.GUID,  NULL AS CODE_NAME, 
	NULL SORT_ID, NULL NAME, 2 CLASSIFY, T.ACCESS_LEVEL, T.ACCESS_LEVEL_NAME, T.ID, T.IsDelegated
FROM @EXP_TABLE T 
	INNER JOIN GROUPS G 
		ON T.OBJ_GUID = G.GUID " + groupParentLimit + @" AND G.STATUS = 1
WHERE T.CLASSIFY = '2' ");
						break;
				}
				#endregion
			}

			if (arrRootSorts.Length > 0)
			{
				strB.Append(" AND ( 1 > 1 ");
				//strB.AppendFormat( " OR GLOBAL_SORT LIKE '{0}%'",  strTemp) ;
				foreach (string strTemp in arrRootSorts)
				{

					string temp = string.Empty;
					switch (strObjType.ToUpper())
					{
						case "GROUPS": temp = bExpandGroup ? "OU" : "G";
							break;
						case "ORGANIZATIONS": temp = "O";
							break;
						default: temp = "OU";
							break;
					}
					strB.AppendFormat(" OR {0}.GLOBAL_SORT LIKE '{1}%'", temp, strTemp);//Modify By yuanyong 20070911
				}
				strB.Append(" ) ");
			}


			return strB.ToString();
		}

		#region delete by yangrui 2006-6-17 �����ñ�����������б��ʽ�����������ݿ���ֱ�ӷ���
		//		/// <summary>
		//		/// �������ʽ�������������д�������У���Ϊ�������ӵ�����
		//		/// </summary>
		//		/// <param name="expDT">�������ı��ʽ�����</param>
		//		/// <param name="bIncludeUser">�漰�û�</param>
		//		/// <param name="bIncludeOrg">�漰����</param>
		//		/// <param name="bIncludeGroup">�漰��</param>
		//		/// <param name="bOrgRank">�����м����޶�</param>
		//		/// <param name="bGroupRank">�����м����޶�</param>
		//		/// <returns>SQL���:������Ľ������������ʱ��</returns>
		//		private static string ParseExpsToTable_SqlStr( DataTable expDT, out bool bIncludeUser, out bool bIncludeOrg, out bool bIncludeGroup	)
		//		{
		//			bIncludeUser	= false;
		//			bIncludeOrg		= false;
		//			bIncludeGroup	= false;
		//
		//			//�趨���ʽ��������¼��ɫ��Ӧ�ı��ʽ
		//			string strSql;
		//			strSql = @"DECLARE @EXP_TABLE 
		//						TABLE(
		//							[ROLE_ID] [nvarchar] (36) NOT NULL ,
		//							[CLASSIFY] [int] NULL ,
		//							[ID] [nvarchar] (36) NOT NULL ,
		//							[OBJ_GUID] [nvarchar] (36) NOT NULL ,
		//							[PARENT_GUID] [nvarchar] (36) NOT NULL ,
		//							[ACCESS_LEVEL] [nvarchar] (32) NULL ,
		//							[ACCESS_LEVEL_NAME] [nvarchar] (32) NULL ,
		//							[SORT_ID] [int] NOT NULL ,
		//							[IsDelegated] [int] NOT NULL DEFAULT (0)
		//						); ";
		//
		//			string objType;		//��������
		//			string objID;		//����Guid
		//			string parentID;	//���󸸶���Guid
		//			string rankCode;	//����ļ����޶�(��Ա�������)
		//			//string rankName = string.Empty;
		//			object rankSort = null;
		//			object rankName = null;
		//
		//			foreach(DataRow row in expDT.Rows)
		//			{
		//				DoExpParsing.getObjInfo(row["EXPRESSION"].ToString(), out objType, out objID, out parentID, out rankCode);
		//				rankName = null;
		//				rankSort = null;
		//
		//				if (objType == string.Empty) 
		//				{
		//					continue; 
		//				}
		//				else  
		//				{
		//					//Ϊ������
		//					switch(objType.ToUpper())
		//					{
		//						case "ORGANIZATIONS":	objType = "1" ; bIncludeOrg = true ; break ;
		//						case "GROUPS":			objType = "2" ; bIncludeGroup = true ; break ;
		//						case "USERS":			objType = "3" ; bIncludeUser = true ; break ;
		//					}
		//				}
		//
		//				if ( rankCode != string.Empty )
		//				{
		//					rankName = RankDefineNameHT[rankCode] ;
		//					rankSort = RankDefineSortHT[rankCode] ;
		//					if ( rankSort == null ) 
		//						rankSort = 65535;
		//
		//				}
		//				//�������в����¼
		//				strSql += string.Format("\nINSERT INTO @EXP_TABLE VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', 0);\n", 
		//						row["ROLE_ID"], objType, row["ID"], objID, parentID, rankCode, rankName, rankSort == null ? 65535 : rankSort );
		//
		//			}
		//			return strSql;
		//		}
		//

		//		/// <summary>
		//		/// ��������Ȩ����ı��ʽ�б�
		//		/// </summary>
		//		/// <param name="objList">���ʽ�б�</param>
		//		/// <param name="objIDs">�������������Ȩ����ID��������Զ��ŷָ�</param>
		//		/// <param name="expIDTable">����Ȩ����ID-���ʽID����Ӧ��ϵ</param>
		//		/// <param name="userParentTable">����Ȩ����ID-������ID����Ӧ��ϵ</param>
		//		/// <param name="rankTable">����Ȩ����ID-��Ա�޶����𣬶�Ӧ��ϵ</param>
		//		private static void ParseObject(DataRow[] objList, out string objIDs, Hashtable expIDTable, Hashtable userParentTable, Hashtable rankTable)
		//		{
		//			objIDs = string.Empty;
		//			
		//			ParseExpression pe = new ParseExpression();
		//			pe.UserFunctions = (IExpParsing)new DoExpParsing();
		//			string strExpression = string.Empty;
		//			
		//			for (int i = 0; i < objList.Length; i++)
		//			{
		//				strExpression = objList[i]["EXPRESSION"].ToString();
		//				
		//				string strObjType;//��������
		//				string strObjID;//����ID
		//				string strParentID;//����ID
		//				string strRankCode;//�����޶�
		//
		//				DoExpParsing.getObjInfo(strExpression, out strObjType, out strObjID, out strParentID, out strRankCode, pe);
		//
		//				if (strObjID != string.Empty)
		//				{
		//					if (objIDs == string.Empty)
		//						objIDs += strObjID;
		//					else 
		//						objIDs += ',' + strObjID;
		//
		//					if (expIDTable != null)
		//						expIDTable.Add(strObjID, objList[i]["ID"].ToString());
		//
		//				}
		//
		//				if (strParentID != string.Empty && userParentTable != null && strObjType.ToUpper() == "USERS")
		//				{
		//					userParentTable.Add(strObjID, strParentID);
		//				}
		//
		//				if (rankTable != null)
		//				{
		//					if (strRankCode != string.Empty)
		//					{
		//						rankTable.Add(strObjID, strRankCode);
		//					}
		//				}
		//			}
		//		}
		//
		#endregion

		/// <summary>
		/// �������ʽ�������������д�������У���Ϊ�������ӵ�����
		/// </summary>
		/// <param name="strWhereCond">ѡ����ʽ�Ĺ�������</param>
		/// <returns></returns>
		private static string ParseExpsToTable_SqlStr2(string strWhereCond)
		{
			string strSql;
			strSql = string.Format(
					@"
DECLARE @EXP_TABLE TABLE
(
	[ROLE_ID] [nvarchar] (36) NOT NULL ,
	[CLASSIFY] [int] NULL ,
	[ID] [nvarchar] (36) NOT NULL ,
	[OBJ_GUID] [nvarchar] (36) NULL ,
	[PARENT_GUID] [nvarchar] (36) NULL ,
	[ACCESS_LEVEL] [nvarchar] (32) NULL ,
	[ACCESS_LEVEL_NAME] [nvarchar] (32) NULL ,
	[SORT_ID] [int] NULL ,
	[IsDelegated] [int] NOT NULL DEFAULT (0),
	[EXPs] nvarchar (512),
	BELONGTO INT,
	USERRANK INT,
	GE INT
); 

INSERT INTO @EXP_TABLE (ROLE_ID, ID, IsDelegated, EXPs, CLASSIFY)
SELECT ROLE_ID, ID, 0, REPLACE( UPPER(EXPRESSION), ' ', ''),
	CASE  CLASSIFY WHEN 0 THEN 3 WHEN 1 THEN 1 WHEN 2 THEN 2 END
FROM dbo.EXPRESSIONS
{0};

UPDATE @EXP_TABLE SET EXPs = REPLACE( EXPs, 'ORGANIZATIONS', '1' );
UPDATE @EXP_TABLE SET EXPs = REPLACE( EXPs, 'GROUPS', '2' );
UPDATE @EXP_TABLE SET EXPs = REPLACE( EXPs, 'USERS', '3' );

UPDATE @EXP_TABLE
SET BELONGTO = CHARINDEX('BELONGTO', EXPs),
	USERRANK = CHARINDEX('USERRANK', EXPs), 
	GE = CHARINDEX('>=', EXPs);

UPDATE @EXP_TABLE
SET OBJ_GUID = dbo.GetParamValue(EXPs, BELONGTO + 8, 2),
	PARENT_GUID = dbo.GetParamValue(EXPs, BELONGTO + 8, 3),
	ACCESS_LEVEL = CASE WHEN USERRANK > 0 THEN SUBSTRING( EXPs, USERRANK+10, GE-USERRANK-13) ELSE NULL END;

UPDATE @EXP_TABLE
SET ACCESS_LEVEL_NAME = B.NAME,
	SORT_ID	= CASE WHEN B.SORT_ID IS NULL THEN 65535 ELSE B.SORT_ID END
FROM @EXP_TABLE A
LEFT JOIN RANK_DEFINE B
ON A.ACCESS_LEVEL = CODE_NAME;

DECLARE @DPT_COUNT INT,
	@GRP_COUNT INT,
	@USR_COONT INT

SELECT @DPT_COUNT = COUNT(*) FROM @EXP_TABLE WHERE CLASSIFY = 1;
SELECT @GRP_COUNT = COUNT(*) FROM @EXP_TABLE WHERE CLASSIFY = 2;
SELECT @USR_COONT = COUNT(*) FROM @EXP_TABLE WHERE CLASSIFY = 3; ", strWhereCond);

			return strSql;
		}

		#endregion GetChildrenInRoles

		#region GetRolesExpressions

		//		//��ѯָ��Ӧ�ã�ָ����ɫ�µ����б��ʽ
		//		private static DataSet GetRolesExpressions(DataAccess dba, string appCodeName, string roleCodeNames)
		//		{
		//			string strSql = GetRolesExpressions_SqlStr(dba, appCodeName, roleCodeNames);
		//			strSql += "\n ORDER BY CLASSIFY, SORT_ID";
		//			return ExecuteSqlDataset(strSql, dba);
		//		}
		//		//�������:��ѯָ��Ӧ�ã�ָ����ɫ�µ����б��ʽ
		//		private static string GetRolesExpressions_SqlStr(DataAccess dba, string appCodeName, string roleCodeNames)
		//		{
		//			//�ҳ����еı��ʽ
		//			string strSql = @"SELECT ID, ROLE_ID, NAME, EXPRESSION, DESCRIPTION, SORT_ID, INHERITED, CLASSIFY FROM EXPRESSIONS WHERE ROLE_ID IN 
		//								(SELECT ID FROM ROLES WHERE CODE_NAME IN 
		//									({0})
		//									AND APP_ID IN (SELECT ID FROM APPLICATIONS WHERE CODE_NAME = {1}) )";
		//			strSql = string.Format(strSql, OGUCommonDefine.AddMulitStrWithQuotationMark( roleCodeNames ), 
		//					TSqlBuilder.Instance.CheckQuotationMark( appCodeName ));
		//			return strSql;
		//		}

		#endregion GetRolesExpressions

		#region GetOriginalUser
		//��ѯָ����Ա��ָ��Ӧ���У�ָ����ɫ��ԭ��ί����
		private static DataSet GetOriginalUser(string userID, string appCodeName, string roleCodeNames, bool includeDisabled)
		{
			//ί����ID
			string strSql = @"
SELECT D.SOURCE_ID 
FROM DELEGATIONS D, ROLES R, APPLICATIONS A
WHERE D.ROLE_ID = R.ID
	AND R.APP_ID = A.ID
	AND A.CODE_NAME = {1}
	AND D.TARGET_ID IN ({0})
	AND R.CODE_NAME IN ({2})
	AND R.ALLOW_DELEGATE = 'y' ";
			strSql = string.Format(strSql,
				OGUCommonDefine.AddMulitStrWithQuotationMark(userID),
				TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true),
				OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames));

			if (false == includeDisabled)
				strSql += "\n AND GETDATE() BETWEEN START_TIME AND END_TIME";

			//����Ϣ
			return OGUCommonDefine.ExecuteDataset(
				string.Format(@"
SELECT OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, 
	OU.GLOBAL_SORT, OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, 
	OU.START_TIME, OU.END_TIME, OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, 
	OU.OUSYSCONTENT3 ,U.GUID, U.FIRST_NAME, U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, 
	U.POSTURAL, U.PERSON_ID, U.SYSDISTINCT1, U.SYSDISTINCT2, U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3 ,
	RD.CODE_NAME, RD.SORT_ID, RD.NAME
FROM OU_USERS OU INNER JOIN USERS U ON OU.USER_GUID = U.GUID 
	LEFT JOIN RANK_DEFINE RD 
		ON U.RANK_CODE = RD.CODE_NAME
WHERE OU.USER_GUID IN ({0})
ORDER BY OU.GLOBAL_SORT, RD.SORT_ID, OU.SIDELINE ", strSql));
		}
		#endregion GetOriginalUser

		#region GetAllOriginalUser
		//��ѯָ����Ա������Ӧ���У�ԭ��ί����
		private static DataSet GetAllOriginalUser(string userID, bool includeDisabled)
		{
			//ί����ID
			string strSql = @"
SELECT D.SOURCE_ID 
FROM DELEGATIONS D, ROLES R
WHERE D.TARGET_ID IN ({0}) 
	AND D.ROLE_ID = R.ID
	AND R.ALLOW_DELEGATE = 'y' ";
			strSql = string.Format(strSql, OGUCommonDefine.AddMulitStrWithQuotationMark(userID));

			if (false == includeDisabled)
				strSql += "\n AND GETDATE() BETWEEN START_TIME AND END_TIME";

			return OGUCommonDefine.ExecuteDataset(string.Format(@"
SELECT OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, OU.GLOBAL_SORT, 
	OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, OU.START_TIME, OU.END_TIME, 
	OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, OU.OUSYSCONTENT3 ,U.GUID, U.FIRST_NAME, 
	U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, U.POSTURAL, U.PERSON_ID, U.SYSDISTINCT1, U.SYSDISTINCT2, 
	U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3, RD.CODE_NAME, RD.SORT_ID, RD.NAME
FROM OU_USERS OU 
	INNER JOIN USERS U 
		ON OU.USER_GUID = U.GUID 
	LEFT JOIN RANK_DEFINE RD 
		ON U.RANK_CODE = RD.CODE_NAME
WHERE OU.USER_GUID IN ({0})
ORDER BY OU.GLOBAL_SORT, RD.SORT_ID, OU.SIDELINE", strSql));
		}
		#endregion GetAllOriginalUser

		#region GetDelegatedUser
		//��ѯָ����Ա����ָ��Ӧ�á�ָ����ɫ�еı�ί����
		private static DataSet GetDelegatedUser(string userIDs, string appCodeName, string roleCodeNames, bool includeDisabled)
		{
			string strSql = @"
SELECT D.TARGET_ID 
FROM DELEGATIONS D, ROLES R, APPLICATIONS A
WHERE D.ROLE_ID = R.ID
	AND R.APP_ID = A.ID
	AND A.CODE_NAME = {0}
	AND R.CODE_NAME IN ({1})
	AND R.ALLOW_DELEGATE = 'y'
	AND D.SOURCE_ID IN ({2}) ";
			strSql = string.Format(strSql,
				TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true),
				OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames),
				OGUCommonDefine.AddMulitStrWithQuotationMark(userIDs));

			if (false == includeDisabled)
				strSql += "\n AND GETDATE() BETWEEN START_TIME AND END_TIME ";

			return OGUCommonDefine.ExecuteDataset(string.Format(@"
SELECT OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, OU.GLOBAL_SORT, 
	OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, OU.START_TIME, 
	OU.END_TIME, OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, OU.OUSYSCONTENT3 ,
	U.GUID, U.FIRST_NAME, U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, U.POSTURAL, U.PERSON_ID, 
	U.SYSDISTINCT1, U.SYSDISTINCT2, U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3 ,RD.CODE_NAME, RD.SORT_ID, RD.NAME
FROM OU_USERS OU 
	INNER JOIN USERS U ON OU.USER_GUID = U.GUID 
	LEFT JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME
WHERE OU.USER_GUID IN ({0})
ORDER BY OU.GLOBAL_SORT, RD.SORT_ID, OU.SIDELINE ", strSql));
		}
		#endregion GetDelegatedUser

		#region GetAllDelegatedUser
		//��ѯָ����Ա��������Ӧ�á����н�ɫ�еı�ί����
		private static DataSet GetAllDelegatedUser(string userIDs, bool includeDisabled)
		{
			//ί����ID
			string strSql = @"
SELECT D.TARGET_ID 
FROM DELEGATIONS D, ROLES R
WHERE D.ROLE_ID = R.ID
	AND D.SOURCE_ID IN ({0}) 
	AND R.ALLOW_DELEGATE = 'y' ";
			strSql = string.Format(strSql, OGUCommonDefine.AddMulitStrWithQuotationMark(userIDs));

			if (false == includeDisabled)
				strSql += "\n AND GETDATE() BETWEEN START_TIME AND END_TIME";

			return OGUCommonDefine.ExecuteDataset(string.Format(@"
SELECT OU.PARENT_GUID, OU.USER_GUID, OU.DISPLAY_NAME, OU.OBJ_NAME, OU.INNER_SORT, OU.ORIGINAL_SORT, OU.GLOBAL_SORT, 
	OU.ALL_PATH_NAME, OU.STATUS, OU.SIDELINE, OU.RANK_NAME, OU.ATTRIBUTES, OU.DESCRIPTION, OU.START_TIME, OU.END_TIME, 
	OU.OUSYSDISTINCT1, OU.OUSYSDISTINCT2, OU.OUSYSCONTENT1, OU.OUSYSCONTENT2, OU.OUSYSCONTENT3, U.GUID, U.FIRST_NAME, 
	U.LAST_NAME, U.LOGON_NAME, U.IC_CARD, U.RANK_CODE, U.E_MAIL, U.POSTURAL, U.PERSON_ID, U.SYSDISTINCT1, U.SYSDISTINCT2, 
	U.SYSCONTENT1, U.SYSCONTENT2, U.SYSCONTENT3, RD.CODE_NAME, RD.SORT_ID, RD.NAME
FROM OU_USERS OU 
	INNER JOIN USERS U ON OU.USER_GUID = U.GUID 
	LEFT JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME
WHERE OU.USER_GUID IN ({0})
ORDER BY OU.GLOBAL_SORT, RD.SORT_ID, OU.SIDELINE  ", strSql));
		}
		#endregion GetAllDelegatedUser

		#region GetFunctionsUsers
		//��ѯӵ��ָ�����ܵ�������Ա
		private static DataSet InnerGetFunctionsUsers(string orgRoot,
			string appCodeName,
			string funcCodeNames,
			DelegationMaskType delegationMask,
			SidelineMaskType sidelineMask,
			string extAttr)
		{
			//ExceptionHelper.CheckStringIsNullOrEmpty(orgRoot, "orgRoot");
			//ExceptionHelper.CheckStringIsNullOrEmpty(appCodeName, "appCodeName");
			//ExceptionHelper.CheckStringIsNullOrEmpty(funcCodeNames, "funcCodeNames");
			//�õ�ӵ�й��ܵ����н�ɫ
			DataTable roleDT = GetFunctionsRoles(appCodeName, funcCodeNames).Tables[0];
			DataSet result = null;
			//if (roleDT.Rows.Count > 0)
			{
				string roleCodeNames = GetColumnValue(roleDT, "CODE_NAME");

				result = InnerGetRolesUsers(orgRoot, appCodeName, roleCodeNames, delegationMask, sidelineMask, extAttr);
			}
			return result;
		}
		#endregion GetFunctionsUsers

		#region IsUserInRoles
		//�ж���Ա�Ƿ���ָ��Ӧ�ã�ָ����ɫ��
		private static bool IsUserInRoles(string userID,
			string userAllPath,
			string appCodeName,
			string roleCodeNames,
			DelegationMaskType delegationMask)
		{
			//ExceptionHelper.CheckStringIsNullOrEmpty(userID, "userID");
			//ExceptionHelper.CheckStringIsNullOrEmpty(appCodeName, "appCodeName");
			//ExceptionHelper.CheckStringIsNullOrEmpty(roleCodeNames, "roleCodeNames");

			string strSql = GetUserRoles_SqlStr(userID, userAllPath, appCodeName, RightMaskType.All, delegationMask, roleCodeNames);

			if (strSql == string.Empty)
				return false;

			strSql = string.Format(@"
SELECT COUNT(*) FROM ({0} AND R.CODE_NAME IN ({1}) AND R.APP_ID = (SELECT ID FROM APPLICATIONS WHERE CODE_NAME = {2})) AS T1 ",
				strSql, OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames), TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));
			object obj = OGUCommonDefine.ExecuteScalar(strSql);

			if (obj != null && (int)obj > 0)
				return true;

			return false;
		}
		#endregion IsUserInRoles

		#region IsUserInAllRoles
		//�ж���Ա�Ƿ���ָ��Ӧ�ã�ָ����ɫ��
		private static bool IsUserInAllRoles(string userID,
			string userAllPath,
			string appCodeName,
			string roleCodeNames,
			DelegationMaskType delegationMask)
		{
			//modified by yangrui 2005.1
			//�°����
			//����������Ա�ӿڣ�ֱ�Ӷ�����б�����

			string strSql = GetUserRoles_SqlStr(userID, userAllPath, appCodeName, RightMaskType.All, delegationMask, roleCodeNames);

			if (strSql == string.Empty)
				return false;

			strSql = string.Format(@"
SELECT COUNT(*) FROM ({0} AND R.CODE_NAME IN ({1}) AND R.APP_ID = (SELECT ID FROM APPLICATIONS WHERE CODE_NAME = {2}) ) AS T1 ",
				strSql,
				OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames),
				TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));

			object obj = OGUCommonDefine.ExecuteScalar(strSql);

			if (obj != null && (int)obj == roleCodeNames.Split(',').Length)
				return true;

			return false;
		}
		#endregion IsUserInAllRoles

		#region DoesUserHasPermissions
		//��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(��һ������)
		private static bool DoesUserHasPermissions(string userID,
			string userAllPath,
			string appCodeName,
			string funcCodeNames,
			DelegationMaskType delegationMask)
		{
			//���Һ�����Ӧ�Ľ�ɫ,��funcCodeNamesת��roleCodeNames
			DataTable roleCodeNameDT = GetFunctionsRoles(appCodeName, funcCodeNames).Tables[0];

			//ExceptionHelper.FalseThrow(roleCodeNameDT.Rows.Count > 0, 
			//	string.Format("Ӧ�����ơ�{0}�����߹������ơ�{1}���ǷǷ�����ֵ", appCodeName, funcCodeNames));
			bool result = false;
			//if (roleCodeNameDT.Rows.Count > 0)
			{
				string roleCodeNames = GetColumnValue(roleCodeNameDT, "CODE_NAME");

				result = IsUserInRoles(userID, userAllPath, appCodeName, roleCodeNames, delegationMask);
			}
			return result;
		}
		#endregion DoesUserHasPermissions

		#region DoesUserHasAllPermissions
		//��ѯָ����Ա����ָ��Ӧ���У��Ƿ�ӵ��ָ���Ĺ���Ȩ��(ȫ���ſ�)
		private static bool DoesUserHasAllPermissions(string userID,
			string userAllPath,
			string appCodeName,
			string funcCodeNames,
			DelegationMaskType delegationMask)
		{
			string[] arrFuncCodeName = funcCodeNames.Split(new char[] { ',', ';' });

			for (int i = 0; i < arrFuncCodeName.Length; i++)
			{
				if (false == DoesUserHasPermissions(userID, userAllPath, appCodeName, arrFuncCodeName[i], delegationMask))
					return false;
			}
			return true;
		}
		#endregion DoesUserHasAllPermissions

		#region GetUserRoles
		/// <summary>
		/// �õ��û����ڵı���Ȩ����ı��ʽ��sql���
		/// </summary>
		/// <param name="strObjType"></param>
		/// <returns></returns>
		private static string GetExpressionsForUser_SqlStr(string strObjType)
		{
			string userParentLimit = AccreditSection.GetConfig().AccreditSettings.RoleRelatedUserParentDept ?
				"AND T.PARENT_GUID = U.PARENT_GUID" : string.Empty;

			string groupParentLimit = AccreditSection.GetConfig().AccreditSettings.RoleRelatedUserParentDept ?
				"AND U.PARENT_GUID = GU.USER_PARENT_GUID" : string.Empty;

			string strSql = string.Empty;

			if (strObjType.ToUpper() == "USERS")
			{
				strSql = @"
SELECT DISTINCT T.ID
FROM @EXP_TABLE T
	INNER JOIN @USER_TABLE U 
		ON T.OBJ_GUID = U.USER_GUID " + userParentLimit + @"
WHERE T.CLASSIFY = '3'
							";
			}
			if (strObjType.ToUpper() == "ORGANIZATIONS")
			{
				strSql = @"
SELECT DISTINCT O_S.EXP_GUID
FROM @ORG_SORT O_S
	INNER JOIN @USER_TABLE U 
		ON U.GLOBAL_SORT LIKE O_S.GLOBAL_SORT + '%' 
			AND U.SORT_ID <= O_S.SORT_ID --�޶���Ա����
							";
			}
			if (strObjType.ToUpper() == "GROUPS")
			{
				strSql = @"
SELECT DISTINCT T.ID
FROM @EXP_TABLE T
	INNER JOIN GROUPS G 
		ON T.OBJ_GUID = G.GUID
	INNER JOIN GROUP_USERS GU 
		ON GU.GROUP_GUID = G.GUID
	INNER JOIN @USER_TABLE U 
		ON U.USER_GUID = GU.USER_GUID " + groupParentLimit + @"
			AND U.SORT_ID <= T.SORT_ID --�޶���Ա����
WHERE T.CLASSIFY = '2'
";
			}
			return strSql;
		}
		//2009-6-11�޸���Ա��ɫ��Ϣ��ڣ�����dataset������ӷ���Ϊƴsql���
		/// <summary>
		/// ��ѯָ���û�����ָ��Ӧ������ӵ�еĽ�ɫ
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="appCodeName"></param>
		/// <param name="userAllPath"></param>
		/// <param name="rightMask"></param>
		/// <param name="delegationMask"></param>
		/// <param name="roleCodeNames">��ɫ��Χ�޶����ɽ�ɫ��code_name����ʾ</param>
		/// <returns></returns>
		private static DataSet GetUserRoles(string userID,
			string userAllPath,
			string appCodeName,
			RightMaskType rightMask,
			DelegationMaskType delegationMask,
			string roleCodeNames)
		{
			//����Ż�������OA���������ȥ����Deligate���жϣ����appCodeNameΪ�գ����Ż���֧
			DataSet result = null;

			if (string.IsNullOrEmpty(userAllPath))
				result = ExtSecurityCheck.GetUserAppRoles(userID, appCodeName, RightMaskType.All);
			else
			{
				string strSql = string.Empty;
				if (appCodeName != string.Empty)
					strSql = GetUserRoles_SqlStr(userID, userAllPath, appCodeName, rightMask, delegationMask, roleCodeNames);

				if (strSql != string.Empty)
					strSql += "\n ORDER BY R.CLASSIFY DESC, R.SORT_ID";
				else
					strSql = @"
SELECT R.ID, R.APP_ID, R.NAME, R.CODE_NAME, R.DESCRIPTION, R.CLASSIFY, R.SORT_ID, R.ALLOW_DELEGATE, R.INHERITED 
FROM ROLES R 
WHERE 1>1";

				result = OGUCommonDefine.ExecuteDataset(strSql);
			}

			return result;
		}

		private static DataSet GetUserRoles(string userID,
			string userAllPath,
			string appCodeName,
			RightMaskType rightMask,
			DelegationMaskType delegationMask)
		{
			return GetUserRoles(userID, userAllPath, appCodeName, rightMask, delegationMask, string.Empty);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="appCodeName"></param>
		/// <param name="userAllPath"></param>
		/// <param name="rightMask"></param>
		/// <param name="delegationMask"></param>
		/// <param name="roleCodeNames">��ɫ��Χ�޶����ɽ�ɫ��code_name����ʾ</param>
		/// <returns></returns>
		private static string GetUserRoles_SqlStr(string userID,
			string userAllPath,
			string appCodeName,
			RightMaskType rightMask,
			DelegationMaskType delegationMask,
			string roleCodeNames)
		{
			string strSql = GetUserExpressionIDs_SqlStr(userID, userAllPath, appCodeName, rightMask, delegationMask, roleCodeNames);

			if (strSql != string.Empty)
				strSql = string.Format(@"
SELECT DISTINCT R.ID, R.APP_ID, R.NAME, R.CODE_NAME, R.DESCRIPTION, R.CLASSIFY, R.SORT_ID, R.ALLOW_DELEGATE, R.INHERITED 
FROM ROLES R, EXPRESSIONS E
WHERE R.ID = E.ROLE_ID
	AND E.ID IN ({0})", strSql);

			return strSql;
		}

		/// <summary>
		/// �õ��û����ڵı��ʽIDs(����ί��)������������ԭʼȨ�޺�ί��Ȩ�޵���ش���ϸ�ڴ����������غ������
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="appCodeName"></param>
		/// <param name="userAllPath"></param>
		/// <param name="rightMask"></param>
		/// <param name="delegationMask"></param>
		/// <param name="roleCodeNames">��ɫ��Χ�޶����ɽ�ɫ��code_name����ʾ</param>
		/// <returns></returns>
		private static string GetUserExpressionIDs_SqlStr(string userID,
			string userAllPath,
			string appCodeName,
			RightMaskType rightMask,
			DelegationMaskType delegationMask,
			string roleCodeNames)
		{
			if (userID == string.Empty)
				return string.Empty;

			//ԭʼ��Ȩ�޶�Ӧ�ı��ʽIDs
			string strExpIDs_O = string.Empty;
			//ί�ɵ�Ȩ�޶�Ӧ�ı��ʽIDs
			string strExpIDs_D = string.Empty;

			string strSql = string.Empty;
			DataTable expDT = null;

			if ((delegationMask & DelegationMaskType.Original) != DelegationMaskType.None)
			{
				strSql = GetUserExpressionIDs_SqlStr2(userID, userAllPath, appCodeName, rightMask, string.Empty);

				if (strSql != string.Empty)
				{
					expDT = OGUCommonDefine.ExecuteDataset(strSql).Tables[0];
					foreach (DataRow row in expDT.Rows)
					{
						if (row[0].ToString() == string.Empty)
							continue;
						if (strExpIDs_O == string.Empty)
							strExpIDs_O = "'" + row[0].ToString() + "'";
						else
							strExpIDs_O += ",'" + row[0].ToString() + "'";
					}
				}
				else
					return string.Empty;
			}

			string strSql2 = string.Empty;

			//��ί��Ȩ������Ա??
			if ((delegationMask & DelegationMaskType.Delegated) != DelegationMaskType.None)
			{

				strSql2 = GetValidDelegationsForTarget_SqlStr(userID, appCodeName, roleCodeNames, rightMask);

				DataSet ds = OGUCommonDefine.ExecuteDataset(strSql2);

				if (ds.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in ds.Tables[0].Rows)
					{

						//ί���� �� ��Ӧ�Ľ�ɫ �У� ����֤����ί���ڴ˽�ɫ�С� ����������row["ROLE_ID"].ToString()������
						strSql2 = GetUserExpressionIDs_SqlStr2(row["SOURCE_ID"].ToString(),
							string.Empty,
							appCodeName,
							rightMask,
							row["ROLE_ID"].ToString());

						if (strSql2 != string.Empty)
						{
							expDT = OGUCommonDefine.ExecuteDataset(strSql2).Tables[0];
							foreach (DataRow row2 in expDT.Rows)
							{
								if (row2[0].ToString() == string.Empty)
									continue;
								if (strExpIDs_D == string.Empty)
									strExpIDs_D = "'" + row2[0].ToString() + "'";
								else
									strExpIDs_D += ",'" + row2[0].ToString() + "'";
							}
						}
					}
				}

			}

			//�ϲ����
			//			strRoleIDs_O += "," + strRoleIDs_D ;
			if (strExpIDs_O != string.Empty && strExpIDs_D != string.Empty)
				strSql = string.Format("{0},{1}", strExpIDs_O, strExpIDs_D);
			else if (strExpIDs_O == string.Empty && strExpIDs_D == string.Empty)
				strSql = string.Empty;
			else
				strSql = strExpIDs_O + strExpIDs_D;

			return strSql;

		}
		/// <summary>
		/// �ҵ����ڱ�ί���ߣ���Ч��ί��
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="appCodeName"></param>
		/// <param name="rightMask"></param>
		/// <param name="roleCodeNames"></param>
		/// <returns></returns>
		private static string GetValidDelegationsForTarget_SqlStr(string userID, string appCodeName, string roleCodeNames, RightMaskType rightMask)
		{
			return GetValidDelegations_SqlStr(appCodeName, roleCodeNames, rightMask, userID, "TARGET_ID");
		}
		//�ҵ�����ί���ߣ���Ч��ί��
		private static string GetValidDelegationsForSource_SqlStr(string userID, string appCodeName, string roleCodeNames, RightMaskType rightMask)
		{
			return GetValidDelegations_SqlStr(appCodeName, roleCodeNames, rightMask, userID, "SOURCE_ID");
		}
		//�ҳ�����ָ��Ӧ�õģ���Ч��ί��
		private static string GetValidDelegationsForApp_SqlStr(string appCodeName, RightMaskType rightMask)
		{
			return GetValidDelegations_SqlStr(appCodeName, string.Empty, rightMask, string.Empty, string.Empty);
		}
		//�ҳ�����ָ��Ӧ����ָ����ɫ����Ч��ί��
		private static string GetValidDelegationsForAppRole_SqlStr(string appCodeName, string roleCodeNames, RightMaskType rightMask)
		{
			return GetValidDelegations_SqlStr(appCodeName, roleCodeNames, rightMask, string.Empty, string.Empty);
		}

		private static string GetValidDelegations_SqlStr(string appCodeName,
			string roleCodeNames,
			RightMaskType rightMask,
			string userID,
			string strFieldName)
		{
			string strAppFilter = string.Empty;
			if (appCodeName != string.Empty)
				strAppFilter = string.Format("AND APP_ID IN ( SELECT ID FROM APPLICATIONS WHERE CODE_NAME = {0} )",
					TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));

			string strRoleFilter = string.Empty;
			if (roleCodeNames != string.Empty)
				strRoleFilter = string.Format(" AND CODE_NAME IN ({0}) ", OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames));

			string strRightFilter = string.Empty;
			if (rightMask == RightMaskType.App)
			{
				strRightFilter = "AND CLASSIFY = 'n' ";
			}
			else if (rightMask == RightMaskType.Self)
			{
				strRightFilter = "AND CLASSIFY = 'y' ";
			}

			string strUserFilter = string.Empty;
			if (strFieldName != string.Empty)
				strUserFilter = string.Format(" AND {0} = {1} ", strFieldName, OGUCommonDefine.AddMulitStrWithQuotationMark(userID));

			string strReturn = string.Format(@"
SELECT DISTINCT SOURCE_ID, TARGET_ID, ROLE_ID FROM DELEGATIONS 
WHERE ROLE_ID IN 
	(SELECT ID FROM ROLES 
	WHERE ALLOW_DELEGATE = 'y' 
	{0}
	{1}
	{2})
AND GETDATE() BETWEEN START_TIME AND END_TIME 
{3} ", strAppFilter, strRoleFilter, strRightFilter, strUserFilter);
#if DEBUG
			Debug.WriteLine(strReturn);
#endif
			return strReturn;

		}
		//added by yangrui 2005.1
		/// <summary>
		/// �õ�һ���û����ڵı��ʽ��������ί�ɵ�Ȩ�ޣ�
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="appCodeName"></param>
		/// <param name="userAllPath"></param>
		/// <param name="rightMask"></param>
		/// <param name="strRoleID">��ɫGuid��Ϊ��ʱ��ָ���н�ɫ</param>
		/// <returns>û�з������ʱ�����ؿմ�</returns>
		private static string GetUserExpressionIDs_SqlStr2(string userID,
			string userAllPath,
			string appCodeName,
			RightMaskType rightMask,
			string strRoleID)
		{
			if (userID == string.Empty)
				return string.Empty;

			string strSql = string.Empty;
			//�õ��û�Guid, ���ڲ��ŵ�Guid��
			if (userAllPath == string.Empty)
			{
				strSql = string.Format(@"
SELECT DISTINCT OU.USER_GUID, OU.PARENT_GUID PARENT_GUID, OU.GLOBAL_SORT, RD.SORT_ID 
FROM USERS U 
	INNER JOIN OU_USERS OU ON U.GUID = OU.USER_GUID 
	LEFT JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME 
WHERE U.GUID = {0}", TSqlBuilder.Instance.CheckQuotationMark(userID, true));
			}
			else
			{
				strSql = string.Format(@"
SELECT DISTINCT OU.USER_GUID, OU.PARENT_GUID PARENT_GUID, OU.GLOBAL_SORT, RD.SORT_ID 
FROM USERS U 
	INNER JOIN OU_USERS OU ON U.GUID = OU.USER_GUID 
	LEFT JOIN RANK_DEFINE RD ON U.RANK_CODE = RD.CODE_NAME 
WHERE U.GUID IN ({0}) AND OU.ALL_PATH_NAME IN ({1})",
					OGUCommonDefine.AddMulitStrWithQuotationMark(userID),
					OGUCommonDefine.AddMulitStrWithQuotationMark(userAllPath));
			}
			//            switch (userValueType)
			//            {
			//                case UserValueType.AllPath:
			//                    strSql = string.Format(@"
			//SELECT OU.USER_GUID, OU.PARENT_GUID, OU.GLOBAL_SORT, RD.SORT_ID 
			//FROM USERS U 
			//	INNER JOIN OU_USERS OU 
			//		ON U.GUID = OU.USER_GUID 
			//	LEFT JOIN RANK_DEFINE RD 
			//		ON U.RANK_CODE = RD.CODE_NAME 
			//WHERE ALL_PATH_NAME = {0}", TSqlBuilder.Instance.CheckQuotationMark(userValue));
			//                    break;
			//                case UserValueType.Guid:
			//                    strSql = string.Format(@"
			//SELECT OU.USER_GUID, OU.PARENT_GUID PARENT_GUID, OU.GLOBAL_SORT, RD.SORT_ID 
			//FROM USERS U 
			//	INNER JOIN OU_USERS OU 
			//		ON U.GUID = OU.USER_GUID 
			//	LEFT JOIN RANK_DEFINE RD 
			//		ON U.RANK_CODE = RD.CODE_NAME 
			//WHERE U.GUID = {0}", TSqlBuilder.Instance.CheckQuotationMark(userValue));
			//                    break;
			//                case UserValueType.ICCode:
			//                    strSql = string.Format(@"
			//SELECT OU.USER_GUID, OU.PARENT_GUID PARENT_GUID, OU.GLOBAL_SORT, RD.SORT_ID 
			//FROM USERS U 
			//	INNER JOIN OU_USERS OU 
			//		ON U.GUID = OU.USER_GUID 
			//	LEFT JOIN RANK_DEFINE RD 
			//		ON U.RANK_CODE = RD.CODE_NAME 
			//WHERE U.IC_CARD = {0}", TSqlBuilder.Instance.CheckQuotationMark(userValue));
			//                    break;
			//                case UserValueType.LogonName:
			//                    strSql = string.Format(@"
			//SELECT OU.USER_GUID, OU.PARENT_GUID PARENT_GUID, OU.GLOBAL_SORT, RD.SORT_ID 
			//FROM USERS U 
			//	INNER JOIN OU_USERS OU 
			//		ON U.GUID = OU.USER_GUID 
			//	LEFT JOIN RANK_DEFINE RD 
			//		ON U.RANK_CODE = RD.CODE_NAME 
			//WHERE U.LOGON_NAME = {0}", TSqlBuilder.Instance.CheckQuotationMark(userValue));
			//                    break;
			//                case UserValueType.PersonID:
			//                    strSql = string.Format(@"
			//SELECT OU.USER_GUID, OU.PARENT_GUID PARENT_GUID, OU.GLOBAL_SORT, RD.SORT_ID 
			//FROM USERS U 
			//	INNER JOIN OU_USERS OU 
			//		ON U.GUID = OU.USER_GUID 
			//	LEFT JOIN RANK_DEFINE RD 
			//		ON U.RANK_CODE = RD.CODE_NAME 
			//WHERE U.PERSON_ID = {0}", TSqlBuilder.Instance.CheckQuotationMark(userValue));
			//                    break;
			//            }


			//���û��Ļ�����Ϣд������
			strSql = string.Format(@"
DECLARE @USER_TABLE TABLE
(
	[USER_GUID] [nvarchar] (36) NOT NULL ,
	[PARENT_GUID] [nvarchar] (36) NOT NULL ,
	[GLOBAL_SORT] [nvarchar] (255) NOT NULL ,
	[SORT_ID] [int] NOT NULL 
); 
INSERT INTO @USER_TABLE {0}
	", strSql);

			//�ҵ����������ı��ʽ������������

			//���˱��ʽ��Ӧ�Ľ�ɫ����
			string strTemp = string.Empty;
			if (rightMask == RightMaskType.App)
				strTemp += " AND ROLE_ID IN ( SELECT ID FROM ROLES WHERE CLASSIFY = 'n')";
			else if (rightMask == RightMaskType.Self)
				strTemp += " AND ROLE_ID IN ( SELECT ID FROM ROLES WHERE CLASSIFY = 'y')";

			//�򵥲�ѯ����Ȩ�����������
			string strExp = string.Empty;
			if (strTemp == string.Empty && appCodeName == string.Empty)
			{
				strExp += string.Format(@" WHERE ( CLASSIFY = 1  OR CLASSIFY = 2 OR (CLASSIFY = 0 AND EXPRESSION LIKE '%{0}%')) ", userID);
			}
			else
			{
				strExp += @"
SELECT DISTINCT E.EXPRESSION, E.CLASSIFY, E.ID, E.ROLE_ID
FROM EXPRESSIONS E
	INNER JOIN ROLES R ON E.ROLE_ID = R.ID";
				strExp = " WHERE 1=1";

				if (appCodeName != string.Empty)
					strExp += string.Format(@" 
AND ROLE_ID IN ( SELECT ID FROM ROLES WHERE ROLES.APP_ID IN (SELECT ID FROM APPLICATIONS WHERE CODE_NAME IN ({0})))",
								OGUCommonDefine.AddMulitStrWithQuotationMark(appCodeName));

				strExp += string.Format("\n AND ( CLASSIFY = 1  OR CLASSIFY = 2 OR (CLASSIFY = 0 AND EXPRESSION LIKE '%{0}%'))", userID);


				if (strTemp != string.Empty)
					strExp += strTemp;
			}
			if (strRoleID != string.Empty)
				strExp += string.Format(" AND ROLE_ID  = {0}", TSqlBuilder.Instance.CheckQuotationMark(strRoleID, true));

			//�������ʽ�������������д�������У���Ϊ�������ӵ�����
			string strReturnSql = string.Empty;
			strReturnSql = string.Format(" {0} \n {1} \n",
				strSql, ParseExpsToTable_SqlStr2(strExp));


			//���ҳ�����������
			strReturnSql += @" 
DECLARE @ORG_SORT TABLE
(
	[GLOBAL_SORT] [nvarchar] (255) NOT NULL,
	[EXP_GUID] [nvarchar] (36) NOT NULL,
	[SORT_ID] [int] NOT NULL 
);
INSERT INTO @ORG_SORT 
SELECT DISTINCT O.GLOBAL_SORT, T.ID, T.SORT_ID 
FROM ORGANIZATIONS O
	INNER JOIN @EXP_TABLE T ON O.GUID = T.OBJ_GUID  
WHERE T.CLASSIFY = '1' AND @DPT_COUNT > 0 ";

			strReturnSql += "\n SELECT '' ID \n";
			strReturnSql += string.Format("\n UNION {0}", GetExpressionsForUser_SqlStr("USERS"));
			strReturnSql += string.Format("\n UNION {0}", GetExpressionsForUser_SqlStr("ORGANIZATIONS"));
			strReturnSql += string.Format("\n UNION {0}", GetExpressionsForUser_SqlStr("GROUPS"));

			return strReturnSql;
		}

		#endregion GetUserRoles

		#region GetUserAllowDelegteRoles
		//��ѯָ���û�����ָ��Ӧ������ӵ�еĿ�ί�ɽ�ɫ
		private static DataSet GetUserAllowDelegteRoles(string userID, string userAllPath, string appCodeName, RightMaskType rightMask)
		{
			string strSql = @" 
SELECT R.ID, R.APP_ID, R.NAME, R.CODE_NAME, R.DESCRIPTION, R.CLASSIFY, R.SORT_ID, R.ALLOW_DELEGATE, R.INHERITED 
FROM ROLES R 
WHERE 1>1 ";

			if (userID != string.Empty)
			{
				//�õ�������Ӧ��ɫ
				string strSql2 = GetUserRoles_SqlStr(userID, userAllPath, appCodeName, rightMask, DelegationMaskType.Original, string.Empty);

				if (strSql2 != string.Empty)
				{
					strSql = @"
SELECT R.ID, R.APP_ID, R.NAME, R.CODE_NAME, R.DESCRIPTION, R.CLASSIFY, R.SORT_ID, R.ALLOW_DELEGATE, R.INHERITED, 
	D.ROLE_ID, D.SOURCE_ID, D.TARGET_ID, D.START_TIME, D.END_TIME 
FROM ({0} AND ALLOW_DELEGATE = 'y') AS R 
	LEFT OUTER JOIN DELEGATIONS D 
		ON R.ID = D.ROLE_ID AND D.SOURCE_ID IN ( {1} )";
					strSql = string.Format(strSql, strSql2, OGUCommonDefine.AddMulitStrWithQuotationMark(userID));
					strSql += "\n ORDER BY CLASSIFY DESC, SORT_ID";
				}
			}
			return OGUCommonDefine.ExecuteDataset(strSql);
		}
		#endregion GetUserAllowDelegteRoles

		#region GetUserPermissions
		//��ѯָ����Ա����ָ��Ӧ�þ��е�Ȩ�ޣ����ܣ�
		private static DataSet GetUserPermissions(string userID, string userAllPath, string appCodeName, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			string strSql = GetUserPermissions_SqlStr(userID, userAllPath, appCodeName, rightMask, delegationMask);
			strSql += "\n ORDER BY APP_ID, CLASSIFY DESC, SORT_ID";
			return OGUCommonDefine.ExecuteDataset(strSql);
		}

		//�������:��ѯָ����Ա����ָ��Ӧ�þ��е�Ȩ�ޣ����ܣ�
		private static string GetUserPermissions_SqlStr(string userID, string userAllPath, string appCodeName, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			//�õ���Ա��ӵ�еĽ�ɫ
			string strSql = GetUserRoles_SqlStr(userID, userAllPath, appCodeName, rightMask, delegationMask, string.Empty);

			if (strSql != string.Empty)
			{
				strSql = string.Format(@"
SELECT DISTINCT F.ID, F.APP_ID, F.NAME, F.CODE_NAME, F.DESCRIPTION, F.SORT_ID, F.CLASSIFY, F.INHERITED 
FROM FUNCTIONS F, ROLE_TO_FUNCTIONS RTF
WHERE F.ID = RTF.FUNC_ID
	AND RTF.ROLE_ID IN (SELECT ID FROM ({0}) AS T1) ", strSql);
			}
			else
				strSql = "SELECT F.ID, F.APP_ID, F.NAME, F.CODE_NAME, F.DESCRIPTION, F.SORT_ID, F.CLASSIFY, F.INHERITED FROM FUNCTIONS F WHERE 1>1";

			return strSql;
		}

		#endregion GetUserPermissions

		#region GetUserApplicationsRoles
		//��ѯָ����Ա��Ӧ�ý�ɫ��Ϣ
		private static DataSet GetUserApplicationsRoles(string userID, string userAllPath, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			//modified by yangrui 2005.1
			string strSql = GetUserRoles_SqlStr(userID, userAllPath, string.Empty, rightMask, delegationMask, string.Empty);
			if (strSql == string.Empty)
			{
				strSql = @"
SELECT DISTINCT A.NAME APP_NAME, A.CODE_NAME APP_CODE_NAME, A.RESOURCE_LEVEL APP_RESOURCE_LEVEL, R.ID, R.APP_ID, R.NAME, R.CODE_NAME, 
	R.DESCRIPTION, R.CLASSIFY, R.SORT_ID, R.ALLOW_DELEGATE, R.INHERITED
FROM APPLICATIONS A 
	INNER JOIN ROLES R ON R.APP_ID = A.ID
WHERE 1>1 ";
			}
			else
			{
				strSql = string.Format(@"
SELECT DISTINCT A.NAME APP_NAME, A.CODE_NAME APP_CODE_NAME, A.RESOURCE_LEVEL APP_RESOURCE_LEVEL, R.ID, R.APP_ID, R.NAME, R.CODE_NAME, 
	R.DESCRIPTION, R.CLASSIFY, R.SORT_ID, R.ALLOW_DELEGATE, R.INHERITED 
FROM ({0}) AS R 
	INNER JOIN APPLICATIONS A ON R.APP_ID = A.ID ", strSql);
				strSql += "\n ORDER BY RESOURCE_LEVEL, CLASSIFY DESC, R.SORT_ID";
			}

			return OGUCommonDefine.ExecuteDataset(strSql);

		}
		#endregion GetUserApplicationsRoles

		#region GetUserApplications
		//��ѯָ����Ա��Ӧ�ý�ɫ��Ϣ
		private static DataSet GetUserApplications(string userID, string userAllPath, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			string strSql = GetUserApplications_SqlStr(userID, userAllPath, rightMask, delegationMask);
			strSql += "\n ORDER BY RESOURCE_LEVEL, SORT_ID";
			return OGUCommonDefine.ExecuteDataset(strSql);
		}

		//�������:��ѯָ����Ա��Ӧ�ý�ɫ��Ϣ
		private static string GetUserApplications_SqlStr(string userID, string userAllPath, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			//modified by yangrui 2005.1
			string strSql = GetUserRoles_SqlStr(userID, userAllPath, String.Empty, rightMask, delegationMask, string.Empty);

			if (strSql != string.Empty)
			{
				strSql = string.Format(@"
SELECT DISTINCT A.ID, A.NAME, A.CODE_NAME, A.DESCRIPTION, A.SORT_ID, A.RESOURCE_LEVEL, A.CHILDREN_COUNT, A.ADD_SUBAPP, A.USE_SCOPE, A.INHERITED_STATE
FROM APPLICATIONS A 
WHERE ID IN (SELECT APP_ID FROM ({0}) AS T1)", strSql);
			}
			else
			{
				strSql = @"
SELECT A.ID, A.NAME, A.CODE_NAME, A.DESCRIPTION, A.SORT_ID, A.RESOURCE_LEVEL, A.CHILDREN_COUNT, A.ADD_SUBAPP, A.USE_SCOPE, A.INHERITED_STATE 
FROM APPLICATIONS A 
WHERE 1>1";
			}
			return strSql;
		}

		#endregion GetUserApplications

		#region GetUserApplicationsForDelegation
		//��ѯָ����Ա��Ӧ�ý�ɫ��Ϣ
		private static DataSet GetUserApplicationsForDelegation(string userID, string userAllPath, RightMaskType rightMask)
		{
			string strSql = GetUserApplicationsForDelegation_SqlStr(userID, userAllPath, rightMask);
			strSql += "\n ORDER BY RESOURCE_LEVEL, SORT_ID";
			return OGUCommonDefine.ExecuteDataset(strSql);
		}

		//�������:��ѯָ����Ա��Ӧ�ý�ɫ��Ϣ
		private static string GetUserApplicationsForDelegation_SqlStr(string userID, string userAllPath, RightMaskType rightMask)
		{
			//�õ��û�����Ӧ�����ڵı��ʽ
			string strSql = GetUserExpressionIDs_SqlStr2(userID, userAllPath, string.Empty, rightMask, string.Empty);

			string expIDs = string.Empty;
			if (strSql != string.Empty)
			{
				DataTable expDT = OGUCommonDefine.ExecuteDataset(strSql).Tables[0];
				expIDs = GetColumnValue(expDT, "ID");
			}
			strSql = string.Format(@"
SELECT DISTINCT A.ID, A.NAME, A.CODE_NAME, A.DESCRIPTION, A.SORT_ID, A.RESOURCE_LEVEL, A.CHILDREN_COUNT, A.ADD_SUBAPP, A.USE_SCOPE, A.INHERITED_STATE  
FROM APPLICATIONS A
WHERE ID IN (SELECT APP_ID FROM ROLES WHERE ALLOW_DELEGATE = 'y' AND ID IN (SELECT ROLE_ID FROM EXPRESSIONS WHERE ID IN ({0})) )",
						OGUCommonDefine.AddMulitStrWithQuotationMark(expIDs));
			return strSql;
		}

		#endregion GetUserApplicationsForDelegation

		#region GetUserRolesScopes
		//��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		private static DataSet GetUserRolesScopes(string userID,
			string userAllPath,
			string appCodeName,
			string roleCodeNames,
			DelegationMaskType delegationMask,
			ScopeMaskType scopeMask)
		{
			string strSql = GetUserRolesScopes_SqlStr(userID, userAllPath, appCodeName, roleCodeNames, delegationMask, scopeMask);
			if (strSql == string.Empty)
			{
				strSql = string.Format(@"
SELECT ID, APP_ID, NAME, CODE_NAME, EXPRESSION, DESCRIPTION, CLASSIFY, SORT_ID, INHERITED  
FROM SCOPES 
WHERE 1>1");
				return OGUCommonDefine.ExecuteDataset(strSql);
			}
			else
			{
				strSql += "\n ORDER BY CLASSIFY, SORT_ID";
				return GetScopeAllPath(OGUCommonDefine.ExecuteDataset(strSql), userID, userAllPath);
			}
		}

		/// <summary>
		/// �������:��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="userAllPath"></param>
		/// <param name="appCodeName"></param>
		/// <param name="roleCodeNames"></param>
		/// <param name="delegationMask"></param>
		/// <param name="scopeMask"></param>
		/// <returns></returns>
		private static string GetUserRolesScopes_SqlStr(string userID,
			string userAllPath,
			string appCodeName,
			string roleCodeNames,
			DelegationMaskType delegationMask,
			ScopeMaskType scopeMask)
		{
			//modified by yangrui 2005.1
			string strSql = string.Format("SELECT ISNULL(USE_SCOPE, 'n') FROM APPLICATIONS WHERE CODE_NAME = {0}",
				TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));

			object obj = OGUCommonDefine.ExecuteScalar(strSql);

			if (obj == null || obj.ToString().ToLower() == "n")
			{
				return string.Empty;
			}

			string expIDsSql = GetUserExpressionIDs_SqlStr(userID, userAllPath, appCodeName, RightMaskType.All, delegationMask, roleCodeNames);
			strSql = string.Empty;
			if (expIDsSql != string.Empty)
			{
				strSql = string.Format(@"
SELECT ID, APP_ID, NAME, CODE_NAME, EXPRESSION, DESCRIPTION, CLASSIFY, SORT_ID, INHERITED  
FROM SCOPES 
WHERE ID IN 
	(
		SELECT ES.SCOPE_ID
		FROM EXP_TO_SCOPES ES INNER JOIN EXPRESSIONS E ON ES.EXP_ID = E.ID
			INNER JOIN ROLES R ON E.ROLE_ID = R.ID 
			INNER JOIN APPLICATIONS A ON R.APP_ID = A.ID
			WHERE ES.EXP_ID IN ({0}) AND A.CODE_NAME = {1} AND R.CODE_NAME IN ({2}) 
	)",
					expIDsSql, TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true),
					OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames));
				string classCondition = string.Empty;
				if (scopeMask == ScopeMaskType.OrgScope)
					classCondition = "\n AND (CLASSIFY = 'y') ";
				if (scopeMask == ScopeMaskType.DataScope)
					classCondition = "\n AND (CLASSIFY = 'n') ";
				if (classCondition != string.Empty)
					strSql += classCondition;

				//strSql = string.Format(strSql, OGUCommonDefine.AddMulitStrWithQuotationMark(expIDs));
			}

			return strSql;

		}

		#endregion GetUserRolesScopes

		#region GetScopeAllPath
		/// <summary>
		/// ת���ڶ���������Χ���õ�ʵ�ʵĻ�����Χ��ȫ·��
		/// </summary>
		/// <param name="scopeDs"></param>
		/// <param name="userID"></param>
		/// <param name="userAllPath"></param>
		/// <returns></returns>
		private static DataSet GetScopeAllPath(DataSet scopeDs, string userID, string userAllPath)
		{
			//modified by yangrui 2005.1
			string strSql = string.Empty;
			string[] strUserSorts;

			strUserSorts = GetUserSorts(userID, userAllPath).Split(',');

			DataRow bbbRow = null;//������
			DataRow bgRow = null;//����

			for (int i = 0; i < scopeDs.Tables[0].Rows.Count; i++)
			{
				if (scopeDs.Tables[0].Rows[i]["CLASSIFY"].ToString() != "y")
					continue;

				//������
				if (scopeDs.Tables[0].Rows[i]["EXPRESSION"].ToString() == "curDepartScope(curUserId)")
					bbbRow = scopeDs.Tables[0].Rows[i];
				//������
				if (scopeDs.Tables[0].Rows[i]["EXPRESSION"].ToString() == "curCustomsScope(curUserId)")
					bgRow = scopeDs.Tables[0].Rows[i];
			}

			//������
			if (bbbRow != null)
			{
				//�鿴�����ŵļ����������
				//#if Framework2_0
				//                string strCurDepLevel = System.Configuration.ConfigurationManager.AppSettings["CurDepartLevel"];
				//#else
				//                string strCurDepLevel = System.Configuration.ConfigurationSettings.AppSettings["CurDepartLevel"];
				//#endif
				string strCurDepLevel = AccreditSection.GetConfig().AccreditSettings.CurDepartLevel;
				int iCurDepLevel = 0;
				if (strCurDepLevel != null)
				{
					try
					{
						iCurDepLevel = int.Parse(strCurDepLevel);
					}
					catch { } //��ֹint��������
				}

				ArrayList orgPaths = new ArrayList();
				foreach (string strUserSort in strUserSorts)
				{
					object orgPath = string.Empty;

					if (iCurDepLevel >= 3)
					{
						strSql = string.Format(@"
SELECT DISTINCT ALL_PATH_NAME 
FROM ORGANIZATIONS 
WHERE GLOBAL_SORT =(
		SELECT MAX(O.GLOBAL_SORT) 
		FROM ORGANIZATIONS O
		WHERE ( SELECT GLOBAL_SORT FROM OU_USERS OU WHERE OU.GLOBAL_SORT = '{0}' )  LIKE O.GLOBAL_SORT+'%'
			AND LEN(O.GLOBAL_SORT) <= {1}  AND (ORG_TYPE & 1) = 0)",
							strUserSort, iCurDepLevel * 6);

						orgPath = OGUCommonDefine.ExecuteScalar(strSql);
					}
					if (orgPath == null || orgPath.ToString() == string.Empty)
					{
						//�϶��Ĳ㼶��Ч,�õ�Ĭ�ϡ������š�
						strSql = string.Format(@"
SELECT DISTINCT ALL_PATH_NAME 
FROM ORGANIZATIONS 
WHERE GLOBAL_SORT =(
		SELECT MAX(O.GLOBAL_SORT) 
		FROM ORGANIZATIONS O
		WHERE ( SELECT GLOBAL_SORT FROM OU_USERS OU WHERE OU.GLOBAL_SORT = '{0}' )  LIKE O.GLOBAL_SORT+'%'
			AND (ORG_TYPE & 1) = 0)", strUserSort);
						orgPath = OGUCommonDefine.ExecuteScalar(strSql);
					}

					if (orgPath != null && orgPaths.IndexOf(orgPath) == -1)
						orgPaths.Add(orgPath);

				}

				foreach (object obj in orgPaths)
				{
					DataRow row = scopeDs.Tables[0].NewRow();
					CopyRow(bbbRow, row);
					row["DESCRIPTION"] = obj;
					scopeDs.Tables[0].Rows.Add(row);
				}

				scopeDs.Tables[0].Rows.Remove(bbbRow);
				scopeDs.Tables[0].AcceptChanges();
			}
			//������
			if (bgRow != null)
			{
				ArrayList orgPaths = new ArrayList();
				foreach (string strUserSort in strUserSorts)
				{
					object orgPath = string.Empty;

					strSql = string.Format(@"
SELECT DISTINCT ALL_PATH_NAME FROM ORGANIZATIONS O
WHERE ( SELECT GLOBAL_SORT FROM OU_USERS OU WHERE OU.GLOBAL_SORT = '{0}' )  LIKE O.GLOBAL_SORT+'%'
	AND LEN(O.GLOBAL_SORT) = 12 ", strUserSort);

					orgPath = OGUCommonDefine.ExecuteScalar(strSql);

					if (orgPath != null
						&& orgPaths.IndexOf(orgPath) == -1
						&& scopeDs.Tables[0].Select(string.Format("[DESCRIPTION] = '{0}'", orgPath)).Length == 0)
						orgPaths.Add(orgPath);
				}

				foreach (object obj in orgPaths)
				{
					DataRow row = scopeDs.Tables[0].NewRow();
					CopyRow(bgRow, row);
					row["DESCRIPTION"] = obj;
					scopeDs.Tables[0].Rows.Add(row);
				}

				scopeDs.Tables[0].Rows.Remove(bgRow);

				scopeDs.Tables[0].AcceptChanges();
			}

			scopeDs.Tables[0].AcceptChanges();
			return scopeDs;

		}
		#endregion GetScopeAllPath

		#region GetUserFunctionsScopes
		/// <summary>
		/// ��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="userAllPath"></param>
		/// <param name="appCodeName"></param>
		/// <param name="funcCodeNames"></param>
		/// <param name="delegationMask"></param>
		/// <param name="scopeMask"></param>
		/// <returns></returns>
		private static DataSet GetUserFunctionsScopes(string userID,
			string userAllPath,
			string appCodeName,
			string funcCodeNames,
			DelegationMaskType delegationMask,
			ScopeMaskType scopeMask)
		{
			string strSql = GetUserFunctionsScopes_SqlStr(userID, userAllPath, appCodeName, funcCodeNames, delegationMask, scopeMask);
			if (strSql == string.Empty)
			{
				strSql = @"
SELECT ID, APP_ID, NAME, CODE_NAME, EXPRESSION, DESCRIPTION, CLASSIFY, SORT_ID, INHERITED  
FROM SCOPES 
WHERE 1>1";
				return OGUCommonDefine.ExecuteDataset(strSql);
			}
			else
			{
				strSql += "\n ORDER BY CLASSIFY, SORT_ID";
				return GetScopeAllPath(OGUCommonDefine.ExecuteDataset(strSql), userID, userAllPath);
			}
		}
		//�������:��ѯָ����Ա����ָ��Ӧ�ã�ָ����ɫ����ӵ�еķ���Χ
		private static string GetUserFunctionsScopes_SqlStr(string userID,
			string userAllPath,
			string appCodeName,
			string funcCodeNames,
			DelegationMaskType delegationMask,
			ScopeMaskType scopeMask)
		{
			//�ӹ��ܵ���ɫ
			DataTable roleDT = GetFunctionsRoles(appCodeName, funcCodeNames).Tables[0];

			string roleCodeNames = GetColumnValue(roleDT, "CODE_NAME");

			return GetUserRolesScopes_SqlStr(userID, userAllPath, appCodeName, roleCodeNames, delegationMask, scopeMask);
		}
		#endregion GetUserFunctionsScopes
		/// <summary>
		/// �õ��û���GUID���û��Ƿ�����Чʱ�����ؿ�
		/// </summary>
		/// <param name="userValues"></param>
		/// <param name="userValueType"></param>
		/// <returns></returns>
		private static string GetUserGuids(string userValues, UserValueType userValueType)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey("GetUserGuids", userValues, userValueType);
			string result;
			//if (false == CommonCoreQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(CommonCoreQueue))//.CacheQueueSync)
			//    {
			if (false == CommonCoreQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strUserColName = OGUCommonDefine.GetSearchObjectColumn(GetUserValueClass(userValueType));
				string temp = DatabaseSchema.Instence.GetTableColumns(strUserColName, "OU_USERS", "USERS");
				string strSql = @"
SELECT DISTINCT USER_GUID 
FROM USERS INNER JOIN OU_USERS
	ON USERS.GUID = OU_USERS.USER_GUID 
WHERE " + temp + " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(userValues) + @") 
	AND OU_USERS.STATUS = 1 
	AND GETDATE() BETWEEN OU_USERS.START_TIME AND OU_USERS.END_TIME";

				//                        if (userValueType == UserValueType.LogonName)
				//                            strSql = string.Format(@"
				//SELECT DISTINCT OU.USER_GUID 
				//FROM USERS U INNER JOIN OU_USERS OU 
				//	ON U.GUID = OU.USER_GUID 
				//WHERE U.LOGON_NAME IN ({0}) 
				//	AND OU.STATUS = 1 
				//	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME", OGUCommonDefine.AddMulitStrWithQuotationMark(userValues));
				//                        if (userValueType == UserValueType.AllPath)
				//                            strSql = string.Format(@"
				//SELECT DISTINCT OU.USER_GUID 
				//FROM OU_USERS OU 
				//WHERE OU.ALL_PATH_NAME IN ({0}) 
				//	AND OU.STATUS = 1 
				//	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME", OGUCommonDefine.AddMulitStrWithQuotationMark(userValues));
				//                        if (userValueType == UserValueType.PersonID)
				//                            strSql = string.Format(@"
				//SELECT DISTINCT OU.USER_GUID 
				//FROM USERS U INNER JOIN OU_USERS OU 
				//	ON U.GUID = OU.USER_GUID 
				//WHERE U.PERSON_ID IN ({0}) 
				//	AND OU.STATUS = 1 
				//	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME", OGUCommonDefine.AddMulitStrWithQuotationMark(userValues));
				//                        if (userValueType == UserValueType.ICCode)
				//                            strSql = string.Format(@"
				//SELECT DISTINCT OU.USER_GUID 
				//FROM USERS U INNER JOIN OU_USERS OU 
				//	ON U.GUID = OU.USER_GUID 
				//WHERE U.IC_CARD IN ({0}) 
				//	AND OU.STATUS = 1 
				//	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME", OGUCommonDefine.AddMulitStrWithQuotationMark(userValues));
				//                        if (userValueType == UserValueType.Guid)
				//                            strSql = string.Format(@"
				//SELECT DISTINCT OU.USER_GUID 
				//FROM USERS U INNER JOIN OU_USERS OU 
				//	ON U.GUID = OU.USER_GUID 
				//WHERE GUID IN ({0}) 
				//	AND OU.STATUS = 1 
				//	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME", OGUCommonDefine.AddMulitStrWithQuotationMark(userValues));

				DataTable DT = OGUCommonDefine.ExecuteDataset(strSql).Tables[0];
				result = GetColumnValue(DT, "USER_GUID");
				CommonCoreQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		///  �õ��û������򴮣��û��Ƿ�����Чʱ�����ؿ�
		/// </summary>
		/// <param name="userIDs"></param>
		/// <param name="userAllPaths"></param>
		/// <returns></returns>
		private static string GetUserSorts(string userIDs, string userAllPaths)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey("GetUserSorts", userIDs, userAllPaths);
			string result;
			//if (false == CommonCoreQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(CommonCoreQueue))//.CacheQueueSync)
			//    {
			if (false == CommonCoreQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strSql = string.Empty;
				if (userAllPaths == string.Empty)
				{
					strSql = string.Format(@"
SELECT DISTINCT GLOBAL_SORT 
FROM OU_USERS 
WHERE USER_GUID IN ({0}) 
	AND STATUS = 1 
	AND GETDATE() BETWEEN START_TIME AND END_TIME", OGUCommonDefine.AddMulitStrWithQuotationMark(userIDs));
				}
				else
				{
					strSql = string.Format(@"
SELECT DISTINCT GLOBAL_SORT
FROM OU_USERS 
WHERE USER_GUID IN ({0}) 
	AND All_PATH_NAME IN ({1}) 
	AND STATUS = 1 
	AND GETDATE() BETWEEN START_TIME AND END_TIME",
								OGUCommonDefine.AddMulitStrWithQuotationMark(userIDs),
								OGUCommonDefine.AddMulitStrWithQuotationMark(userAllPaths));
				}

				DataTable DT = OGUCommonDefine.ExecuteDataset(strSql).Tables[0];

				result = GetColumnValue(DT, "GLOBAL_SORT");
				CommonCoreQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userValueType"></param>
		/// <returns></returns>
		private static OguAdmin.SearchObjectColumn GetUserValueClass(UserValueType userValueType)
		{
			switch (userValueType)
			{
				case UserValueType.Guid:
					return SearchObjectColumn.SEARCH_GUID;

				case UserValueType.LogonName:
					return SearchObjectColumn.SEARCH_LOGON_NAME;

				case UserValueType.PersonID:
					return SearchObjectColumn.SEARCH_PERSON_ID;

				case UserValueType.AllPath:
					return SearchObjectColumn.SEARCH_ALL_PATH_NAME;

				case UserValueType.ICCode:
					return SearchObjectColumn.SEARCH_IC_CARD;
				//Ϊ����Ͼ�����ͳһƽ̨�л����������ֶ�ID[����Ψһ�ֶ�]
				case UserValueType.Identity:
					return SearchObjectColumn.SEARCH_IDENTITY;
			}
			return SearchObjectColumn.SEARCH_NULL;

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="appID"></param>
		/// <returns></returns>
		private static string GetApplicatonCodeName(Guid appID)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey("GetApplicatonCodeName", appID);
			string result;
			//if (false == CommonCoreQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(CommonCoreQueue))//.CacheQueueSync)
			//    {
			if (false == CommonCoreQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string sql = string.Format("SELECT CODE_NAME FROM APPLICATIONS WHERE ID = '{0}' ", appID.ToString());
				object obj = OGUCommonDefine.ExecuteScalar(sql);
				result = obj is DBNull ? string.Empty : obj.ToString();
				CommonCoreQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		private static string GetColumnValue(DataTable table, string columnName)
		{
			StringBuilder builder = new StringBuilder(128);

			string str = string.Empty;
			foreach (DataRow row in table.Rows)
			{
				if (builder.Length > 0)
					builder.Append(",");

				builder.Append(row[columnName].ToString());
			}
			return builder.ToString();
		}
		private static string GetColumnValue(DataRow[] rowCollection, string columnName)
		{
			StringBuilder builder = new StringBuilder(128);

			string str = string.Empty;
			foreach (DataRow row in rowCollection)
			{
				if (builder.Length > 0)
					builder.Append(",");

				builder.Append(row[columnName].ToString());
			}
			return builder.ToString();
		}
		/// <summary>
		/// ���Ƽ�¼
		/// </summary>
		/// <param name="rowSource"></param>
		/// <param name="rowTarget"></param>
		private static void CopyRow(DataRow rowSource, DataRow rowTarget)
		{
			for (int i = 0; i < rowSource.ItemArray.Length; i++)
			{
				rowTarget[i] = rowSource[i];
			}
		}
		#endregion private functions
	}
}
