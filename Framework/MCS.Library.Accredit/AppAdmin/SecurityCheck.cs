#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Accredit
// FileName	：	FunctionNames.cs
// Remark	：		授权系统接口定义实现，主要对外提供数据对象以及数据服务内容
// -------------------------------------------------
// VERSION  	AUTHOR				DATE					CONTENT
// 1.0				ccic\yuanyong		20081216			新创建
//	1.1			ccic\yuanyong		20081216			修改SecurityCheck为StaticClass以优化系统处理
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
	/// 授权管理的关键接口实现
	/// </summary>
	/// <remarks>授权管理的关键接口实现</remarks>
	public static class SecurityCheck
	{
		//private const string ConnStr = "AccreditAdmin";
		private static Hashtable RankDefineSortHT = new Hashtable();
		private static Hashtable RankDefineNameHT = new Hashtable();

		///// <summary>
		///// 构造函数
		///// </summary>
		///// <remarks>空构造函数，不做任何处理实现</remarks>
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
		/// 判断人员是否是总管理员
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns>判断人员是否是总管理员</returns>
		/// <remarks>总管理员是通用授权平台的总管理者，如果总管理员中没有用户，系统的授权失效；必须有一个总管理员才能激活系统的授权管理控制；
		/// 这里判断用户userValue是否是通用授权管理系统中的总管理员角色</remarks>
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
					if (obj != null && (int)obj == 0)////总管理员角色中没有对象,所有人员均是总管理员
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
		/// 判断人员是否是总管理员
		/// </summary>
		/// <param name="userValue">用户登录名，这里特别指明数据为登录名</param>
		/// <returns>判断人员是否是总管理员</returns>
		/// <remarks>总管理员是通用授权平台的总管理者，如果总管理员中没有用户，系统的授权失效；必须有一个总管理员才能激活系统的授权管理控制；
		/// 这里判断用户userValue是否是通用授权管理系统中的总管理员角色</remarks>
		public static bool IsAdminUser(string userValue)
		{
			return IsAdminUser(userValue, UserValueType.LogonName);
		}
		#endregion IsAdminUser

		#region GetFunctionsRoles
		/// <summary>
		/// 查找指定应用中，具有指定功能的角色。
		/// </summary>
		/// <param name="appCodeName">应用的英文标识，只能单个</param>
		/// <param name="funcCodeNames">功能的英文标识，允许多个，多个时用逗号分隔</param>
		/// <returns>查询结果以DataSet方式返回</returns>
		/// <remarks>查找指定应用appCodeName中，具有指定功能funcCodeNames的所有角色信息，结果以DataSet方式返回。
		/// 返回结果中只有一个DataTable，所有角色都在其中，不再区分每一个Function对应的Role数据。
		/// 系统无论查询结果是否存在都会将具体结果返回，不会返回一个Null值</remarks>
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
		/// 获得当前通用授权平台下注册的所有应用的信息
		/// </summary>
		/// <returns>以一个DataSet返回所有的查询结果。</returns>
		/// <remarks>获得当前通用授权平台下注册的所有应用的信息。其中的所有数据都存储在一个DataTable数据表中</remarks>
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
		/// 查询指定应用中，指定类别的所有角色
		/// </summary>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="rightMask">权限类型</param>
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
		/// 查询指定应用中，指定类别的所有角色
		/// </summary>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <returns></returns>
		public static DataSet GetRoles(string appCodeName)
		{
			return GetRoles(appCodeName, RightMaskType.All);
		}
		#endregion GetRoles

		#region GetFunctions
		/// <summary>
		/// 查询指定应用中，指定类别的所有功能
		/// </summary>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="rightMask">权限类型</param>
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
		/// 查询指定应用中，指定类别的所有功能
		/// </summary>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <returns></returns>
		public static DataSet GetFunctions(string appCodeName)
		{
			return GetFunctions(appCodeName, RightMaskType.All);
		}
		#endregion GetFunctions

		#region GetRolesUsers
		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有人员
		/// 不建议使用参数extAttr，使用参数extAttr会调有机构人员接口，影响性能
		/// </summary>
		/// <param name="orgRoot">根部门的全路径</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="delegationMask">委派类型</param>
		/// <param name="sidelineMask">职位类型</param>
		/// <param name="extAttr">要求获取的扩展属性(已无效，保留此参数是为了兼容原来版本的接口函数)</param>
		/// <returns></returns>
		//[Obsolete("不建议使用参数extAttr，使用参数extAttr会调有机构人员接口，影响性能", false)]
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
				//不建议使用参数extAttr，使用参数extAttr会调有机构人员接口，影响性能
				result = InnerGetRolesUsers(orgRoot, appCodeName, roleCodeNames, delegationMask, sidelineMask, extAttr);
				GetRolesUsersQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有人员
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="delegationMask">委派类型</param>
		/// <param name="sidelineMask">职位类型</param>
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
		/// 查询指定部门下，指定应用中，指定角色的所有人员
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="delegationMask">委派类型</param>
		/// <returns></returns>
		public static DataSet GetRolesUsers(string orgRoot, string appCodeName, string roleCodeNames, DelegationMaskType delegationMask)
		{
			return GetRolesUsers(orgRoot, appCodeName, roleCodeNames, delegationMask, SidelineMaskType.All);
		}
		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有人员
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static DataSet GetRolesUsers(string orgRoot, string appCodeName, string roleCodeNames)
		{
			return GetRolesUsers(orgRoot, appCodeName, roleCodeNames, DelegationMaskType.All, SidelineMaskType.All);
		}
		#endregion GetRolesUsers

		#region GetChildrenInRoles
		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门，多个时用逗号分隔</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，再组，后人员</param>
		/// <param name="doesSortRank">是否排序级别，true:先级别，后层次 false:只排层次</param>
		/// <param name="includeDelegate">是否包括委派权限的被授权对象，true:包括 false:不包括</param>
		/// <param name="bExpandGroup">是否展开Group中的User</param>
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
					//简单查询被授权对象的过滤条件
					string strWhereSql = @"
							WHERE ROLE_ID IN 
								(SELECT ID FROM ROLES WHERE CODE_NAME IN 
									({0})
									AND APP_ID IN (SELECT ID FROM APPLICATIONS WHERE CODE_NAME = {1}) )";
					strWhereSql = string.Format(strWhereSql, OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames),
						TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));


					//得到根部门的排序串
					string[] arrRootSorts = new string[0];
					if (orgRoot != string.Empty)
					{
						arrRootSorts = GetOrgSorts(orgRoot);
					}

					//解析表达式，并将解析结果写入表变量中，作为其后表连接的依据
					string strSql = ParseExpsToTable_SqlStr2(strWhereSql);

					#region Modify By Yuanyong
					#region 原有代码，Del By Yuanyong 20070911
					/*if (bExpandGroup)//如果展开组中人员，不能包括表达式ID，因为一个人在不同组中时，会出现重记录
				strSql += @"SELECT '' AS OBJECTCLASS,  '' AS PERSON_ID, '' CUSTOMS_CODE,  NULL AS SIDELINE,   '' AS RANK_NAME, '' ALL_PATH_NAME, '' GLOBAL_SORT, '' ORIGINAL_SORT, '' DISPLAY_NAME, '' OBJ_NAME,  '' AS LOGON_NAME, '' PARENT_GUID, '' GUID, '' CODE_NAME, NULL SORT_ID, '' NAME, '' CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, NULL IsDelegated WHERE 1>1";
			else //不展开组时，需要表达式的ID，因为需要根据表达式ID，得到表达式的范围，进而确定角色的范围
				strSql += @"SELECT '' AS OBJECTCLASS,  '' AS PERSON_ID, '' CUSTOMS_CODE,  NULL AS SIDELINE,   '' AS RANK_NAME, '' ALL_PATH_NAME, '' GLOBAL_SORT, '' ORIGINAL_SORT, '' DISPLAY_NAME, '' OBJ_NAME,  '' AS LOGON_NAME, '' PARENT_GUID, '' GUID, '' CODE_NAME, NULL SORT_ID, '' NAME, '' CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, NULL ID, NULL IsDelegated WHERE 1>1";

			//人员
			strSql	+= string.Format("\n UNION {0}", GetExpressionsDedail_SqlStr("USERS", arrRootSorts, bExpandGroup ));

			//机构
			strSql	+= string.Format("\n UNION {0}", GetExpressionsDedail_SqlStr("ORGANIZATIONS", arrRootSorts, bExpandGroup ));
			
			//组
			strSql	+= string.Format("\n UNION {0}", GetExpressionsDedail_SqlStr("GROUPS", arrRootSorts, bExpandGroup ));


			if (doesMixSort)//混排
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

					if (bExpandGroup)//如果展开组中人员，不能包括表达式ID，因为一个人在不同组中时，会出现重记录
						sqlUnion.Append(@"
	SELECT '' AS OBJECTCLASS,  '' AS PERSON_ID, '' CUSTOMS_CODE,  NULL AS SIDELINE,   '' AS RANK_NAME, '' ALL_PATH_NAME, '' GLOBAL_SORT, 
		'' ORIGINAL_SORT, '' DISPLAY_NAME, '' OBJ_NAME,  '' AS LOGON_NAME, '' PARENT_GUID, '' GUID, '' CODE_NAME, NULL SORT_ID, '' NAME, 
		'' CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, NULL IsDelegated 
	WHERE 1>1");
					else //不展开组时，需要表达式的ID，因为需要根据表达式ID，得到表达式的范围，进而确定角色的范围
						sqlUnion.Append(@"
	SELECT '' AS OBJECTCLASS,  '' AS PERSON_ID, '' CUSTOMS_CODE,  NULL AS SIDELINE,   '' AS RANK_NAME, '' ALL_PATH_NAME, '' GLOBAL_SORT, 
		'' ORIGINAL_SORT, '' DISPLAY_NAME, '' OBJ_NAME,  '' AS LOGON_NAME, '' PARENT_GUID, '' GUID, '' CODE_NAME, NULL SORT_ID, '' NAME, 
		'' CLASSIFY, '' ACCESS_LEVEL, '' ACCESS_LEVEL_NAME, NULL ID, NULL IsDelegated 
	WHERE 1>1");

					//人员
					sqlUnion.AppendFormat(Environment.NewLine + "	UNION" + Environment.NewLine + "	{0}", GetExpressionsDedail_SqlStr("USERS", arrRootSorts, bExpandGroup));
					//机构
					sqlUnion.AppendFormat(Environment.NewLine + "	UNION" + Environment.NewLine + "	{0}", GetExpressionsDedail_SqlStr("ORGANIZATIONS", arrRootSorts, bExpandGroup));
					//组
					sqlUnion.AppendFormat(Environment.NewLine + "	UNION" + Environment.NewLine + "	{0}", GetExpressionsDedail_SqlStr("GROUPS", arrRootSorts, bExpandGroup));

					//strSql += ";" + Environment.NewLine; // Del By CCIC\yuanyong 20081008 SQL 2000中不能支持连续分号的写法
					strSql += Environment.NewLine;
					if (doesMixSort)//混排
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
					#region 构建数据
					//读委派权限类人员??
					if ((result.Tables[0].Rows.Count > 0) && includeDelegate)//&& ( bIncludeUser || bIncludeGroup ) )
					{
						string strSql2 = string.Empty;

						//得到当前角色下，有效的委派
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

						#region 数据内部处理
						if (ds.Tables[0].Rows.Count > 0)
						{
							string strTemp = string.Empty;
							string strTemp2 = string.Empty;
							foreach (DataRow row in ds.Tables[0].Rows)
							{
								//委派者属于角色
								strTemp = string.Format("[GUID] = '{0}'", row["SOURCE_ID"]);
								//被委派者不属于角色
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
							//读被委派者
							if (strSql2 != string.Empty)
							{
								//***********
								//委派的人不过滤机构：原有权限在范围中，认定被委派人在范围中。
								//***********
								result.Merge(OGUCommonDefine.ExecuteDataset("--委派人在根机构的范围中，则认定被委派人在范围中。  \n" + strSql2));
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
		/// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门，多个时用逗号分隔</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，再组，后人员</param>
		/// <param name="doesSortRank">是否排序级别，true:先级别，后层次 false:只排层次</param>
		/// <param name="includeDelegate">是否包括委派权限的被授权对象，true:包括 false:不包括</param>
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
		/// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门，多个时用逗号分隔</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，再组，后人员</param>
		/// <param name="doesSortRank">是否排序级别，true:先级别，后层次 false:只排层次</param>
		/// <returns></returns>
		public static DataSet GetChildrenInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort, bool doesSortRank)
		{
			return GetChildrenInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, doesSortRank, true);
		}

		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门，多个时用逗号分隔</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，再组，后人员</param>
		/// <returns></returns>
		public static DataSet GetChildrenInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort)
		{
			return GetChildrenInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, false);
		}

		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门，多个时用逗号分隔</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static DataSet GetChildrenInRoles(string orgRoot, string appCodeName, string roleCodeNames)
		{
			return GetChildrenInRoles(orgRoot, appCodeName, roleCodeNames, false);
		}

		#endregion GetChildrenInRoles

		#region GetDepartmentAndUserInRoles
		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门，多个时用逗号分隔</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，再组，后人员</param>
		/// <param name="doesSortRank">是否排序级别，true:先级别，后层次 false:只排层次</param>
		/// <param name="includeDelegate">是否包括委派权限的被授权对象，true:包括 false:不包括</param>
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
				//去掉节尾的'\'
				if (orgRoot != string.Empty && orgRoot.LastIndexOf("\\") == orgRoot.Length - 1)
					orgRoot = orgRoot.Substring(0, orgRoot.Length - 1);

				result = GetChildrenInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, doesSortRank, includeDelegate, true);

				//处理兼职造成的重复记录
				DataRow[] rows;

				rows = result.Tables[0].Select("[OBJECTCLASS] = 'USERS' AND [SIDELINE] = 1");
				if (rows.Length != 0)
				{
					DataRow[] tempRows;
					foreach (DataRow row in rows)
					{
						tempRows = result.Tables[0].Select(string.Format("[GUID] = '{0}' AND [PARENT_GUID] <> '{1}'", row["GUID"], row["PARENT_GUID"]));

						//如果还有其它职位，则删此职位
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
		///// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		///// </summary>
		///// <param name="orgRoot">根部门的全路径，空串时不限制部门，多个时用逗号分隔</param>
		///// <param name="appCodeName">应用的英文标识</param>
		///// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		///// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，再组，后人员</param>
		///// <param name="doesSortRank">是否排序级别，true:先级别，后层次 false:只排层次</param>
		///// <param name="includeDelegate">是否包括委派权限的被授权对象，true:包括 false:不包括</param>
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
		/// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门，多个时用逗号分隔</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，再组，后人员</param>
		/// <param name="doesSortRank">是否排序级别，true:先级别，后层次 false:只排层次</param>
		/// <returns></returns>
		public static DataSet GetDepartmentAndUserInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort, bool doesSortRank)
		{
			return GetDepartmentAndUserInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, doesSortRank, true);
		}
		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门，多个时用逗号分隔</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="doesMixSort">是否采用混合排序，true:机构、组、人员混排，false:先机构，再组，后人员</param>
		/// <returns></returns>
		public static DataSet GetDepartmentAndUserInRoles(string orgRoot, string appCodeName, string roleCodeNames, bool doesMixSort)
		{
			return GetDepartmentAndUserInRoles(orgRoot, appCodeName, roleCodeNames, doesMixSort, false);
		}
		/// <summary>
		/// 查询指定部门下，指定应用中，指定角色的所有被授权对象
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门，多个时用逗号分隔</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static DataSet GetDepartmentAndUserInRoles(string orgRoot, string appCodeName, string roleCodeNames)
		{
			return GetDepartmentAndUserInRoles(orgRoot, appCodeName, roleCodeNames, false);
		}
		#endregion GetDepartmentAndUserInRoles

		#region GetOriginalUser
		/// <summary>
		/// 查询指定人员在指定应用中，指定角色的原有委派者
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="includeDisabled">是否包括无效的委派</param>
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
		/// 查询指定人员在指定应用中，指定角色的原有委派者
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetOriginalUser(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType)
		{
			return GetOriginalUser(userValue, appCodeName, roleCodeNames, userValueType, false);
		}
		/// <summary>
		/// 查询指定人员在指定应用中，指定角色的原有委派者
		/// </summary>
		/// <param name="userValue">用户的登录名</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static DataSet GetOriginalUser(string userValue, string appCodeName, string roleCodeNames)
		{
			return GetOriginalUser(userValue, appCodeName, roleCodeNames, UserValueType.LogonName);
		}
		#endregion GetOriginalUser

		#region GetAllOriginalUser
		/// <summary>
		/// 查询指定人员在指定所有应用中所有角色的原有委派者
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="includeDisabled">是否包括无效的委派</param>
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
		/// 查询指定人员在指定所有应用中的原有委派者
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetAllOriginalUser(string userValue, UserValueType userValueType)
		{
			return GetAllOriginalUser(userValue, userValueType, false);
		}
		/// <summary>
		/// 查询指定人员在指定所有应用中的原有委派者
		/// </summary>
		/// <param name="userValue">用户的登录名</param>
		/// <returns></returns>
		public static DataSet GetAllOriginalUser(string userValue)
		{
			return GetAllOriginalUser(userValue, UserValueType.LogonName);
		}
		#endregion GetAllOriginalUser

		#region GetDelegatedUser
		/// <summary>
		/// 查询指定人员在指定应用中，指定角色的被委派者
		/// </summary>
		/// <param name="userValues">用户身份标识（由userValueType参数指定类型，多个时用逗号分隔）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="includeDisabled">是否包括无效的委派</param>
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
		/// 查询指定人员在指定应用中，指定角色的被委派者
		/// </summary>
		/// <param name="userValues">用户身份标识（由userValueType参数指定类型，多个时用逗号分隔）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetDelegatedUser(string userValues, string appCodeName, string roleCodeNames, UserValueType userValueType)
		{
			return GetDelegatedUser(userValues, appCodeName, roleCodeNames, userValueType, false);
		}
		/// <summary>
		/// 查询指定人员在指定应用中，指定角色的被委派者
		/// </summary>
		/// <param name="userValues">用户身份标识（由userValueType参数指定类型，多个时用逗号分隔）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static DataSet GetDelegatedUser(string userValues, string appCodeName, string roleCodeNames)
		{
			return GetDelegatedUser(userValues, appCodeName, roleCodeNames, UserValueType.LogonName);
		}
		#endregion GetDelegatedUser

		#region GetAllDelegatedUser
		/// <summary>
		/// 查询指定人员在所有应用中、所有角色的被委派者
		/// </summary>
		/// <param name="userValues">用户登录名</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="includeDisabled">是否包括无效的委派</param>
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
		/// 查询指定人员在所有应用中、所有角色的被委派者
		/// </summary>
		/// <param name="userValues">用户身份标识（由userValueType参数指定类型，多个时用逗号分隔）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetAllDelegatedUser(string userValues, UserValueType userValueType)
		{
			return GetAllDelegatedUser(userValues, userValueType, false);
		}
		/// <summary>
		/// 查询指定人员在所有应用中、所有角色的被委派者
		/// </summary>
		/// <param name="userValues">用户登录名</param>
		/// <returns></returns>
		public static DataSet GetAllDelegatedUser(string userValues)
		{
			return GetAllDelegatedUser(userValues, UserValueType.LogonName);
		}
		#endregion GetAllDelegatedUser

		#region GetFunctionsUsers
		/// <summary>
		/// 查询指定部门下，指定应用中，拥有指定功能的所有人员
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔[只要有其中一项就可以]</param>
		/// <param name="delegationMask">委派类型</param>
		/// <param name="sidelineMask">职位类型</param>
		/// <param name="extAttr">要求获取的扩展属性</param>
		/// <returns>查询指定部门下，指定应用中，拥有指定功能的所有人员[多个功能号的时候，只要有其中一项就可以]</returns>
		//[Obsolete("不建议使用参数extAttr，使用参数extAttr会调有机构人员接口，影响性能", false)]
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
		/// 查询指定部门下，指定应用中，拥有指定功能的所有人员
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="delegationMask">委派类型</param>
		/// <param name="sidelineMask">职位类型</param>
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
		/// 查询指定部门下，指定应用中，拥有指定功能的所有人员
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="delegationMask">委派类型</param>
		/// <returns></returns>
		public static DataSet GetFunctionsUsers(string orgRoot, string appCodeName, string funcCodeNames, DelegationMaskType delegationMask)
		{
			return GetFunctionsUsers(orgRoot, appCodeName, funcCodeNames, delegationMask, SidelineMaskType.All);
		}
		/// <summary>
		/// 查询指定部门下，指定应用中，拥有指定功能的所有人员
		/// </summary>
		/// <param name="orgRoot">根部门的全路径，空串时不限制部门</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static DataSet GetFunctionsUsers(string orgRoot, string appCodeName, string funcCodeNames)
		{
			return GetFunctionsUsers(orgRoot, appCodeName, funcCodeNames, DelegationMaskType.All);
		}
		#endregion GetFunctionsUsers

		#region IsUserInRoles
		/// <summary>
		/// 判断人员是否在指定应用，指定角色中
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 判断人员是否在指定应用，指定角色中
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static bool IsUserInRoles(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType)
		{
			return IsUserInRoles(userValue, appCodeName, roleCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// 判断人员是否在指定应用，指定角色中
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static bool IsUserInRoles(string userValue, string appCodeName, string roleCodeNames)
		{
			return IsUserInRoles(userValue, appCodeName, roleCodeNames, UserValueType.LogonName);
		}
		/// <summary>
		/// 判断人员是否在指定应用，指定角色中
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 判断人员是否在指定应用，指定角色中
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static bool IsUserInRoles(string userValue, Guid appID, string roleCodeNames, UserValueType userValueType)
		{
			return IsUserInRoles(userValue, appID, roleCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// 判断人员是否在指定应用，指定角色中
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static bool IsUserInRoles(string userValue, Guid appID, string roleCodeNames)
		{
			return IsUserInRoles(userValue, appID, roleCodeNames, UserValueType.LogonName);
		}
		#endregion IsUserInRoles

		#region IsUserInAllRoles
		/// <summary>
		/// 判断人员是否在指定应用，指定的所有角色中
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 判断人员是否在指定应用，指定的所有角色中
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static bool IsUserInAllRoles(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType)
		{
			return IsUserInAllRoles(userValue, appCodeName, roleCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// 判断人员是否在指定应用，指定的所有角色中
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static bool IsUserInAllRoles(string userValue, string appCodeName, string roleCodeNames)
		{
			return IsUserInAllRoles(userValue, appCodeName, roleCodeNames, UserValueType.LogonName);
		}
		/// <summary>
		/// 判断人员是否在指定应用，指定的所有角色中
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 判断人员是否在指定应用，指定的所有角色中
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static bool IsUserInAllRoles(string userValue, Guid appID, string roleCodeNames, UserValueType userValueType)
		{
			return IsUserInAllRoles(userValue, appID, roleCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// 判断人员是否在指定应用，指定的所有角色中
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static bool IsUserInAllRoles(string userValue, Guid appID, string roleCodeNames)
		{
			return IsUserInAllRoles(userValue, appID, roleCodeNames, UserValueType.LogonName);
		}
		#endregion IsUserInAllRoles

		#region DoesUserHasPermissions
		/// <summary>
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(有一个即可)
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(有一个即可)
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static bool DoesUserHasPermissions(string userValue, string appCodeName, string funcCodeNames, UserValueType userValueType)
		{
			return DoesUserHasPermissions(userValue, appCodeName, funcCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(有一个即可)
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static bool DoesUserHasPermissions(string userValue, string appCodeName, string funcCodeNames)
		{
			return DoesUserHasPermissions(userValue, appCodeName, funcCodeNames, UserValueType.LogonName);
		}
		/// <summary>
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(有一个即可)
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(有一个即可)
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static bool DoesUserHasPermissions(string userValue, Guid appID, string funcCodeNames, UserValueType userValueType)
		{
			return DoesUserHasPermissions(userValue, appID, funcCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(有一个即可)
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static bool DoesUserHasPermissions(string userValue, Guid appID, string funcCodeNames)
		{
			return DoesUserHasPermissions(userValue, appID, funcCodeNames, UserValueType.LogonName);
		}
		#endregion DoesUserHasPermissions

		#region DoesUserHasAllPermissions
		/// <summary>
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(拥有全部功能)
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(拥有全部功能)
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static bool DoesUserHasAllPermissions(string userValue, string appCodeName, string funcCodeNames, UserValueType userValueType)
		{
			return DoesUserHasAllPermissions(userValue, appCodeName, funcCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(拥有全部功能)
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static bool DoesUserHasAllPermissions(string userValue, string appCodeName, string funcCodeNames)
		{
			return DoesUserHasAllPermissions(userValue, appCodeName, funcCodeNames, UserValueType.LogonName);
		}
		/// <summary>
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(拥有全部功能)
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(拥有全部功能)
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static bool DoesUserHasAllPermissions(string userValue, Guid appID, string funcCodeNames, UserValueType userValueType)
		{
			return DoesUserHasAllPermissions(userValue, appID, funcCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// 查询指定人员，在指定应用中，是否拥有指定的功能权限(拥有全部功能)
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appID">应用的GUID</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static bool DoesUserHasAllPermissions(string userValue, Guid appID, string funcCodeNames)
		{
			return DoesUserHasAllPermissions(userValue, appID, funcCodeNames, UserValueType.LogonName);
		}
		#endregion DoesUserHasAllPermissions

		#region GetUserRoles
		/// <summary>
		/// 查询指定用户，在指定应用中所拥有的角色
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限类型</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 查询指定用户，在指定应用中所拥有的角色
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限类型</param>
		/// <returns></returns>
		public static DataSet GetUserRoles(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask)
		{
			return GetUserRoles(userValue, appCodeName, userValueType, rightMask, DelegationMaskType.All);
		}
		/// <summary>
		/// 查询指定用户，在指定应用中所拥有的角色
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetUserRoles(string userValue, string appCodeName, UserValueType userValueType)
		{
			return GetUserRoles(userValue, appCodeName, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// 查询指定用户，在指定应用中所拥有的角色
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <returns></returns>
		public static DataSet GetUserRoles(string userValue, string appCodeName)
		{
			return GetUserRoles(userValue, appCodeName, UserValueType.LogonName);
		}
		#endregion GetUserRoles

		#region GetUserAllowDelegteRoles
		/// <summary>
		/// 查询指定用户，在指定应用中所拥有的，可进行委派的角色
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限类型</param>
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
		/// 查询指定用户，在指定应用中所拥有的，可进行委派的角色
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetUserAllowDelegteRoles(string userValue, string appCodeName, UserValueType userValueType)
		{
			return GetUserAllowDelegteRoles(userValue, appCodeName, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// 查询指定用户，在指定应用中所拥有的，可进行委派的角色
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <returns></returns>
		public static DataSet GetUserAllowDelegteRoles(string userValue, string appCodeName)
		{
			return GetUserAllowDelegteRoles(userValue, appCodeName, UserValueType.LogonName);
		}
		#endregion GetUserAllowDelegteRoles

		#region GetUserPermissions
		/// <summary>
		/// 查询指定人员，在指定应用具有的权限（功能）
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限类型</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 查询指定人员，在指定应用具有的权限（功能）
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限类型</param>
		/// <returns></returns>
		public static DataSet GetUserPermissions(string userValue, string appCodeName, UserValueType userValueType, RightMaskType rightMask)
		{
			return GetUserPermissions(userValue, appCodeName, userValueType, rightMask, DelegationMaskType.All);
		}
		/// <summary>
		/// 查询指定人员，在指定应用具有的权限（功能）
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetUserPermissions(string userValue, string appCodeName, UserValueType userValueType)
		{
			return GetUserPermissions(userValue, appCodeName, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// 查询指定人员，在指定应用具有的权限（功能）
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <returns></returns>
		public static DataSet GetUserPermissions(string userValue, string appCodeName)
		{
			return GetUserPermissions(userValue, appCodeName, UserValueType.LogonName);
		}
		#endregion GetUserPermissions

		#region GetUserApplicationsRoles
		/// <summary>
		/// 查询指定人员的应用角色信息
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限类型</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 查询指定人员的应用角色信息
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限类型</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsRoles(string userValue, UserValueType userValueType, RightMaskType rightMask)
		{
			return GetUserApplicationsRoles(userValue, userValueType, rightMask, DelegationMaskType.All);
		}
		/// <summary>
		/// 查询指定人员的应用角色信息
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsRoles(string userValue, UserValueType userValueType)
		{
			return GetUserApplicationsRoles(userValue, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// 查询指定人员的应用角色信息
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsRoles(string userValue)
		{
			return GetUserApplicationsRoles(userValue, UserValueType.LogonName);
		}
		#endregion GetUserApplicationsRoles

		#region GetUserApplications
		/// <summary>
		/// 查询指定人员所拥有的所有应用信息
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限类型</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 查询指定人员所拥有的所有应用信息
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限类型</param>
		/// <returns></returns>
		public static DataSet GetUserApplications(string userValue, UserValueType userValueType, RightMaskType rightMask)
		{
			return GetUserApplications(userValue, userValueType, rightMask, DelegationMaskType.All);
		}
		/// <summary>
		/// 查询指定人员所拥有的所有应用信息
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetUserApplications(string userValue, UserValueType userValueType)
		{
			return GetUserApplications(userValue, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// 查询指定人员所拥有的所有应用信息
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <returns></returns>
		public static DataSet GetUserApplications(string userValue)
		{
			return GetUserApplications(userValue, UserValueType.LogonName);
		}
		#endregion GetUserApplications

		#region GetUserApplicationsForDelegation
		/// <summary>
		/// 查询指定人员所拥有的所有可进行委派操作的应用信息
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="rightMask">权限类型</param>
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
		/// 查询指定人员所拥有的所有可进行委派操作的应用信息
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsForDelegation(string userValue, UserValueType userValueType)
		{
			return GetUserApplicationsForDelegation(userValue, userValueType, RightMaskType.All);
		}
		/// <summary>
		/// 查询指定人员所拥有的所有可进行委派操作的应用信息
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <returns></returns>
		public static DataSet GetUserApplicationsForDelegation(string userValue)
		{
			return GetUserApplicationsForDelegation(userValue, UserValueType.LogonName);
		}
		#endregion GetUserApplicationsForDelegation

		#region GetUserRolesScopes
		/// <summary>
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
		/// <param name="scopeMask">服务范围类型</param>
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
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetUserRolesScopes(string userValue, string appCodeName, string roleCodeNames, UserValueType userValueType)
		{
			return GetUserRolesScopes(userValue, appCodeName, roleCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="roleCodeNames">角色的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static DataSet GetUserRolesScopes(string userValue, string appCodeName, string roleCodeNames)
		{
			return GetUserRolesScopes(userValue, appCodeName, roleCodeNames, UserValueType.LogonName);
		}
		#endregion GetUserRolesScopes

		#region GetUserFunctionsScopes
		/// <summary>
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
		/// <param name="scopeMask">服务范围类型</param>
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
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <param name="delegationMask">委派类型</param>
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
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
		/// </summary>
		/// <param name="userValue">用户身份标识（由userValueType参数指定类型）</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <param name="userValueType">用户身份标识类型（UserValueType.LogonName:登录名, UserValueType.AllPath:全路径）</param>
		/// <returns></returns>
		public static DataSet GetUserFunctionsScopes(string userValue, string appCodeName, string funcCodeNames, UserValueType userValueType)
		{
			return GetUserFunctionsScopes(userValue, appCodeName, funcCodeNames, userValueType, DelegationMaskType.All);
		}
		/// <summary>
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
		/// </summary>
		/// <param name="userValue">用户登录名</param>
		/// <param name="appCodeName">应用的英文标识</param>
		/// <param name="funcCodeNames">功能的英文标识，多个时用逗号分隔</param>
		/// <returns></returns>
		public static DataSet GetUserFunctionsScopes(string userValue, string appCodeName, string funcCodeNames)
		{
			return GetUserFunctionsScopes(userValue, appCodeName, funcCodeNames, UserValueType.LogonName);
		}
		#endregion GetUserFunctionsScopes

		#region RemoveAllCache
		/// <summary>
		/// 清理数据缓存
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
		/// 生成语句:查询指定应用中，指定类别的所有角色
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
				"请正确设定要查询的角色类型！");
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
		/// 生成语句:查询指定应用中，指定类别的所有功能
		/// </summary>
		/// <param name="appCodeName"></param>
		/// <param name="rightMask">rightMask(掩码): 1:自授权; 2:应用授权; 3:全部	</param>
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

			//ExceptionHelper.TrueThrow<ApplicationException>((rightMask & RightMaskType.All) == RightMaskType.None, "请正确设定要查询的功能类型！");
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
		//查询指定部门下，指定应用中，指定角色，指定类别的所有人员
		private static DataSet InnerGetRolesUsers(string orgRoot,
			string appCodeName,
			string roleCodeNames,
			DelegationMaskType delegationMask,
			SidelineMaskType sidelineMask,
			string extAttr)
		{
			//取原有权限人员
			//参数extAttr已无效
			string strSql = GetRolesUsers_SqlStr(orgRoot, appCodeName, roleCodeNames, sidelineMask, extAttr);

			if (strSql == string.Empty)
			{
				//如果没有表达式
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

				//过滤非选定的职位
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


			//读委派权限类人员??
			DataSet delegateDS = null;
			if ((returnDS.Tables[0].Rows.Count) > 0 && (delegationMask & DelegationMaskType.Delegated) != DelegationMaskType.None)
			{
				//得到当前角色下，有效的委派
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
						//委派者属于角色
						strTemp = string.Format("[USER_GUID] = '{0}'", row["SOURCE_ID"]);
						//被委派者不属于角色
						strTemp2 = string.Format("[USER_GUID] = '{0}'", row["TARGET_ID"]);
						if (returnDS.Tables[0].Select(strTemp).Length > 0 && returnDS.Tables[0].Select(strTemp2).Length == 0)
						{
							dUserIDs += string.Format(",'{0}'", row["TARGET_ID"]);
						}

					}
					//读被委派者
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
						//委派的人不过滤机构：原有权限在范围中，认定被委派人在范围中。
						//***********

						//处理兼职
						if ((sidelineMask & SidelineMaskType.All) != SidelineMaskType.All)
						{
							if (sidelineMask == SidelineMaskType.NotSideline) //主职
							{
								strSql2 += " AND SIDELINE = 0";
							}

							if (sidelineMask == SidelineMaskType.Sideline) //兼职
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
		//查询指定部门下，指定应用中，指定角色，指定类别的所有人员(不分职位)
		private static DataSet InnerGetRolesUsers(string orgRoot, string appCodeName, string roleCodeNames, DelegationMaskType delegationMask, string extAttr)
		{
			return InnerGetRolesUsers(orgRoot, appCodeName, roleCodeNames, delegationMask, SidelineMaskType.All, extAttr);
		}
		private static string GetRolesUsers_SqlStr(string orgRoot, string appCodeName, string roleCodeNames, SidelineMaskType sidelineMask, string extAttr)
		{
			//解析表达式，并将解析结果写入表变量中，作为其后表连接的依据
			//strSql = ParseExpsToTable_SqlStr(expDT, out bIncludeUser, out bIncludeOrg, out bIncludeGroup ) ;

			//得到所有的表达式
			string strWhereSql = @"WHERE ROLE_ID IN 
								(SELECT ID FROM ROLES WHERE CODE_NAME IN 
									({0})
									AND APP_ID IN (SELECT ID FROM APPLICATIONS WHERE CODE_NAME = {1}) )";
			strWhereSql = string.Format(strWhereSql, OGUCommonDefine.AddMulitStrWithQuotationMark(roleCodeNames),
				TSqlBuilder.Instance.CheckQuotationMark(appCodeName, true));

			//得到根部门的排序串
			string[] arrRootSorts = new string[0];
			if (orgRoot != string.Empty)
			{
				arrRootSorts = GetOrgSorts(orgRoot);
			}

			string strSql = ParseExpsToTable_SqlStr2(strWhereSql);


			//则找出机构下的所有机构的Guid
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


			//人员
			strSql += string.Format("\n UNION \n {0}", GetExpressionsUsers_SqlStr("USERS", arrRootSorts, sidelineMask, extAttr));
			//机构
			strSql += string.Format("\n UNION \n {0}", GetExpressionsUsers_SqlStr("ORGANIZATIONS", arrRootSorts, sidelineMask, extAttr));
			//组
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
				//去掉节尾的'\'
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
				builder.Append("------");//20070807按照杨睿要求增加

			return builder.ToString().Split(',');
		}
		/// <summary>
		/// 从已解析的表变量的表中，得到指定类的表达式的涉及的所有人员
		/// </summary>
		/// <param name="strObjType">表达式类型</param>
		/// <param name="arrRootSorts">根机构排序串</param>
		/// <param name="sidelineMask">职位类型</param>
		/// <param name="extAttr">获取更多的扩展属性</param>
		/// <returns>SQL语句</returns>
		private static string GetExpressionsUsers_SqlStr(string strObjType, string[] arrRootSorts, SidelineMaskType sidelineMask, string extAttr)
		{
			string strSql = string.Empty;

			//人员
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

			//机构
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
	AND RD.SORT_ID <= T.SORT_ID --级别限定
	AND OU.STATUS = 1 
	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME 
	AND @DPT_COUNT > 0 ";
			}
			//组
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
	AND RD.SORT_ID <= T.SORT_ID --级别限定
	AND OU.STATUS = 1 
	AND GETDATE() BETWEEN OU.START_TIME AND OU.END_TIME 
	AND @GRP_COUNT > 0 ";
			}

			//处理根机构限定
			if (arrRootSorts.Length > 0)
			{
				strSql += " AND ( 1 > 1 ";
				foreach (string strTemp in arrRootSorts)
				{
					strSql += string.Format(" OR OU.GLOBAL_SORT LIKE '{0}%'", strTemp);
				}
				strSql += " ) ";
			}

			//处理兼职
			if ((sidelineMask & SidelineMaskType.All) != SidelineMaskType.All)
			{
				if (sidelineMask == SidelineMaskType.NotSideline) //主职
					strSql += " AND OU.SIDELINE = 0";

				if (sidelineMask == SidelineMaskType.Sideline) //兼职
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

			//处理表达式
			StringBuilder strB = new StringBuilder(4096);


			//如果展开组中人员，不能包括表达式ID，因为一个人在不同组中时，会出现重记录
			//不展开组时，需要表达式的ID，因为需要根据表达式ID，得到表达式的范围
			if (bExpandGroup)
			{
				#region 组内成员展开
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
				#region 组内成员不展开
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

		#region delete by yangrui 2006-6-17 不引用表达解析组件进行表达式解析，在数据库中直接翻译
		//		/// <summary>
		//		/// 解析表达式，并将解析结果写入表变量中，作为其后表连接的依据
		//		/// </summary>
		//		/// <param name="expDT">被解析的表达式结果集</param>
		//		/// <param name="bIncludeUser">涉及用户</param>
		//		/// <param name="bIncludeOrg">涉及机构</param>
		//		/// <param name="bIncludeGroup">涉及组</param>
		//		/// <param name="bOrgRank">机构有级别限定</param>
		//		/// <param name="bGroupRank">组中有级别限定</param>
		//		/// <returns>SQL语句:解析后的结果，并存入临时表</returns>
		//		private static string ParseExpsToTable_SqlStr( DataTable expDT, out bool bIncludeUser, out bool bIncludeOrg, out bool bIncludeGroup	)
		//		{
		//			bIncludeUser	= false;
		//			bIncludeOrg		= false;
		//			bIncludeGroup	= false;
		//
		//			//设定表达式变量，记录角色对应的表达式
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
		//			string objType;		//对象类型
		//			string objID;		//对象Guid
		//			string parentID;	//对象父对象Guid
		//			string rankCode;	//对象的级别限定(人员级别代码)
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
		//					//为了排序
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
		//				//向表变量中插入记录
		//				strSql += string.Format("\nINSERT INTO @EXP_TABLE VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', 0);\n", 
		//						row["ROLE_ID"], objType, row["ID"], objID, parentID, rankCode, rankName, rankSort == null ? 65535 : rankSort );
		//
		//			}
		//			return strSql;
		//		}
		//

		//		/// <summary>
		//		/// 解析被授权对象的表达式列表
		//		/// </summary>
		//		/// <param name="objList">表达式列表</param>
		//		/// <param name="objIDs">输出参数，被授权对象ID串，多个以逗号分隔</param>
		//		/// <param name="expIDTable">被授权对象ID-表达式ID，对应关系</param>
		//		/// <param name="userParentTable">被授权对象ID-父对象ID，对应关系</param>
		//		/// <param name="rankTable">被授权对象ID-人员限定级别，对应关系</param>
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
		//				string strObjType;//对象类型
		//				string strObjID;//对象ID
		//				string strParentID;//对象父ID
		//				string strRankCode;//级别限定
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
		/// 解析表达式，并将解析结果写入表变量中，作为其后表连接的依据
		/// </summary>
		/// <param name="strWhereCond">选择表达式的过滤条件</param>
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

		//		//查询指定应用，指定角色下的所有表达式
		//		private static DataSet GetRolesExpressions(DataAccess dba, string appCodeName, string roleCodeNames)
		//		{
		//			string strSql = GetRolesExpressions_SqlStr(dba, appCodeName, roleCodeNames);
		//			strSql += "\n ORDER BY CLASSIFY, SORT_ID";
		//			return ExecuteSqlDataset(strSql, dba);
		//		}
		//		//生成语句:查询指定应用，指定角色下的所有表达式
		//		private static string GetRolesExpressions_SqlStr(DataAccess dba, string appCodeName, string roleCodeNames)
		//		{
		//			//找出所有的表达式
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
		//查询指定人员在指定应用中，指定角色的原有委派者
		private static DataSet GetOriginalUser(string userID, string appCodeName, string roleCodeNames, bool includeDisabled)
		{
			//委派者ID
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

			//读信息
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
		//查询指定人员在所有应用中，原有委派者
		private static DataSet GetAllOriginalUser(string userID, bool includeDisabled)
		{
			//委派者ID
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
		//查询指定人员，在指定应用、指定角色中的被委派者
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
		//查询指定人员，在所有应用、所有角色中的被委派者
		private static DataSet GetAllDelegatedUser(string userIDs, bool includeDisabled)
		{
			//委派者ID
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
		//查询拥有指定功能的所有人员
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
			//得到拥有功能的所有角色
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
		//判断人员是否在指定应用，指定角色中
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
		//判断人员是否在指定应用，指定角色中
		private static bool IsUserInAllRoles(string userID,
			string userAllPath,
			string appCodeName,
			string roleCodeNames,
			DelegationMaskType delegationMask)
		{
			//modified by yangrui 2005.1
			//新版程序
			//不调机构人员接口，直接读库进行表连接

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
		//查询指定人员，在指定应用中，是否拥有指定的功能权限(有一个即可)
		private static bool DoesUserHasPermissions(string userID,
			string userAllPath,
			string appCodeName,
			string funcCodeNames,
			DelegationMaskType delegationMask)
		{
			//查找函数对应的角色,把funcCodeNames转成roleCodeNames
			DataTable roleCodeNameDT = GetFunctionsRoles(appCodeName, funcCodeNames).Tables[0];

			//ExceptionHelper.FalseThrow(roleCodeNameDT.Rows.Count > 0, 
			//	string.Format("应用名称【{0}】或者功能名称【{1}】是非法数据值", appCodeName, funcCodeNames));
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
		//查询指定人员，在指定应用中，是否拥有指定的功能权限(全部才可)
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
		/// 得到用户所在的被授权对象的表达式的sql语句
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
			AND U.SORT_ID <= O_S.SORT_ID --限定人员级别
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
			AND U.SORT_ID <= T.SORT_ID --限定人员级别
WHERE T.CLASSIFY = '2'
";
			}
			return strSql;
		}
		//2009-6-11修改人员角色信息入口，返回dataset结果，子方法为拼sql语句
		/// <summary>
		/// 查询指定用户，在指定应用中所拥有的角色
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="appCodeName"></param>
		/// <param name="userAllPath"></param>
		/// <param name="rightMask"></param>
		/// <param name="delegationMask"></param>
		/// <param name="roleCodeNames">角色范围限定，由角色的code_name串表示</param>
		/// <returns></returns>
		private static DataSet GetUserRoles(string userID,
			string userAllPath,
			string appCodeName,
			RightMaskType rightMask,
			DelegationMaskType delegationMask,
			string roleCodeNames)
		{
			//沈峥优化，根据OA常用情况，去掉了Deligate的判断，如果appCodeName为空，走优化分支
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
		/// <param name="roleCodeNames">角色范围限定，由角色的code_name串表示</param>
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
		/// 得到用户所在的表达式IDs(包括委派)，本函数进行原始权限和委派权限的相关处理，细节处理由其重载函数完成
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="appCodeName"></param>
		/// <param name="userAllPath"></param>
		/// <param name="rightMask"></param>
		/// <param name="delegationMask"></param>
		/// <param name="roleCodeNames">角色范围限定，由角色的code_name串表示</param>
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

			//原始的权限对应的表达式IDs
			string strExpIDs_O = string.Empty;
			//委派的权限对应的表达式IDs
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

			//读委派权限类人员??
			if ((delegationMask & DelegationMaskType.Delegated) != DelegationMaskType.None)
			{

				strSql2 = GetValidDelegationsForTarget_SqlStr(userID, appCodeName, roleCodeNames, rightMask);

				DataSet ds = OGUCommonDefine.ExecuteDataset(strSql2);

				if (ds.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in ds.Tables[0].Rows)
					{

						//委派人 在 相应的角色 中， 才能证明被委派在此角色中。 所以设置了row["ROLE_ID"].ToString()参数。
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

			//合并结果
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
		/// 找到对于被委派者，有效的委派
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
		//找到对于委派者，有效的委派
		private static string GetValidDelegationsForSource_SqlStr(string userID, string appCodeName, string roleCodeNames, RightMaskType rightMask)
		{
			return GetValidDelegations_SqlStr(appCodeName, roleCodeNames, rightMask, userID, "SOURCE_ID");
		}
		//找出对于指定应用的，有效的委派
		private static string GetValidDelegationsForApp_SqlStr(string appCodeName, RightMaskType rightMask)
		{
			return GetValidDelegations_SqlStr(appCodeName, string.Empty, rightMask, string.Empty, string.Empty);
		}
		//找出对于指定应用中指定角色，有效的委派
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
		/// 得到一个用户所在的表达式（不包括委派的权限）
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="appCodeName"></param>
		/// <param name="userAllPath"></param>
		/// <param name="rightMask"></param>
		/// <param name="strRoleID">角色Guid，为空时，指所有角色</param>
		/// <returns>没有符合情况时，返回空串</returns>
		private static string GetUserExpressionIDs_SqlStr2(string userID,
			string userAllPath,
			string appCodeName,
			RightMaskType rightMask,
			string strRoleID)
		{
			if (userID == string.Empty)
				return string.Empty;

			string strSql = string.Empty;
			//得到用户Guid, 所在部门的Guid等
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


			//把用户的基本信息写入表变量
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

			//找到符合条件的表达式，并存入表变量

			//过滤表达式对应的角色类型
			string strTemp = string.Empty;
			if (rightMask == RightMaskType.App)
				strTemp += " AND ROLE_ID IN ( SELECT ID FROM ROLES WHERE CLASSIFY = 'n')";
			else if (rightMask == RightMaskType.Self)
				strTemp += " AND ROLE_ID IN ( SELECT ID FROM ROLES WHERE CLASSIFY = 'y')";

			//简单查询被授权对象过滤条件
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

			//解析表达式，并将解析结果写入表变量中，作为其后表连接的依据
			string strReturnSql = string.Empty;
			strReturnSql = string.Format(" {0} \n {1} \n",
				strSql, ParseExpsToTable_SqlStr2(strExp));


			//则找出机构的排序串
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
		//查询指定用户，在指定应用中所拥有的可委派角色
		private static DataSet GetUserAllowDelegteRoles(string userID, string userAllPath, string appCodeName, RightMaskType rightMask)
		{
			string strSql = @" 
SELECT R.ID, R.APP_ID, R.NAME, R.CODE_NAME, R.DESCRIPTION, R.CLASSIFY, R.SORT_ID, R.ALLOW_DELEGATE, R.INHERITED 
FROM ROLES R 
WHERE 1>1 ";

			if (userID != string.Empty)
			{
				//得到所有相应角色
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
		//查询指定人员，在指定应用具有的权限（功能）
		private static DataSet GetUserPermissions(string userID, string userAllPath, string appCodeName, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			string strSql = GetUserPermissions_SqlStr(userID, userAllPath, appCodeName, rightMask, delegationMask);
			strSql += "\n ORDER BY APP_ID, CLASSIFY DESC, SORT_ID";
			return OGUCommonDefine.ExecuteDataset(strSql);
		}

		//生成语句:查询指定人员，在指定应用具有的权限（功能）
		private static string GetUserPermissions_SqlStr(string userID, string userAllPath, string appCodeName, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			//得到人员所拥有的角色
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
		//查询指定人员的应用角色信息
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
		//查询指定人员的应用角色信息
		private static DataSet GetUserApplications(string userID, string userAllPath, RightMaskType rightMask, DelegationMaskType delegationMask)
		{
			string strSql = GetUserApplications_SqlStr(userID, userAllPath, rightMask, delegationMask);
			strSql += "\n ORDER BY RESOURCE_LEVEL, SORT_ID";
			return OGUCommonDefine.ExecuteDataset(strSql);
		}

		//生成语句:查询指定人员的应用角色信息
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
		//查询指定人员的应用角色信息
		private static DataSet GetUserApplicationsForDelegation(string userID, string userAllPath, RightMaskType rightMask)
		{
			string strSql = GetUserApplicationsForDelegation_SqlStr(userID, userAllPath, rightMask);
			strSql += "\n ORDER BY RESOURCE_LEVEL, SORT_ID";
			return OGUCommonDefine.ExecuteDataset(strSql);
		}

		//生成语句:查询指定人员的应用角色信息
		private static string GetUserApplicationsForDelegation_SqlStr(string userID, string userAllPath, RightMaskType rightMask)
		{
			//得到用户所有应用所在的表达式
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
		//查询指定人员，在指定应用，指定角色中所拥有的服务范围
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
		/// 生成语句:查询指定人员，在指定应用，指定角色中所拥有的服务范围
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
		/// 转换内定机构服务范围，得到实际的机构范围的全路径
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

			DataRow bbbRow = null;//本部门
			DataRow bgRow = null;//本关

			for (int i = 0; i < scopeDs.Tables[0].Rows.Count; i++)
			{
				if (scopeDs.Tables[0].Rows[i]["CLASSIFY"].ToString() != "y")
					continue;

				//本部门
				if (scopeDs.Tables[0].Rows[i]["EXPRESSION"].ToString() == "curDepartScope(curUserId)")
					bbbRow = scopeDs.Tables[0].Rows[i];
				//本关区
				if (scopeDs.Tables[0].Rows[i]["EXPRESSION"].ToString() == "curCustomsScope(curUserId)")
					bgRow = scopeDs.Tables[0].Rows[i];
			}

			//本部门
			if (bbbRow != null)
			{
				//查看本部门的级别配置情况
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
					catch { } //防止int解析错误
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
						//认定的层级无效,得到默认“本部门”
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
			//本关区
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
		/// 查询指定人员，在指定应用，指定角色中所拥有的服务范围
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
		//生成语句:查询指定人员，在指定应用，指定角色中所拥有的服务范围
		private static string GetUserFunctionsScopes_SqlStr(string userID,
			string userAllPath,
			string appCodeName,
			string funcCodeNames,
			DelegationMaskType delegationMask,
			ScopeMaskType scopeMask)
		{
			//从功能到角色
			DataTable roleDT = GetFunctionsRoles(appCodeName, funcCodeNames).Tables[0];

			string roleCodeNames = GetColumnValue(roleDT, "CODE_NAME");

			return GetUserRolesScopes_SqlStr(userID, userAllPath, appCodeName, roleCodeNames, delegationMask, scopeMask);
		}
		#endregion GetUserFunctionsScopes
		/// <summary>
		/// 得到用户的GUID，用户非法或无效时，返回空
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
		///  得到用户的排序串，用户非法或无效时，返回空
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
				//为配合南京海关统一平台切换，新增加字段ID[自增唯一字段]
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
		/// 复制记录
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
