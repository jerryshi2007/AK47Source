using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Web;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

using MCS.Library.Accredit.Properties;
using MCS.Library.Accredit.Configuration;
using MCS.Library.Accredit.OguAdmin.Interfaces;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.OguAdmin.Caching;
using MCS.Library.Accredit.Common;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 机构人员管理系统中的所有查询操作实现
	/// </summary>
	public sealed class OGUReader
	{
		#region Variable Define
		///// <summary>
		///// 考虑数据库结构的不变性，所以可以采用静态变量保存数据库结构的Schema
		///// </summary>
		//private static DataSet _DataSet_Schema = null;

		private static XmlDocument MaskObjectDocument = null;

		/// <summary>
		/// 
		/// </summary>
		public const string SearchAllTerm = "@SearchAll@";
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造函数
		/// </summary>
		public OGUReader()
		{
		}

		#endregion

		#region public function
		#region GetOrganizationChildren
		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="iLot">要求查询的数据对象类型（机构、组、人员、兼职对象）</param>
		/// <param name="iLod">是否包含被逻辑删除的成员</param>
		/// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
		/// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
		/// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
		/// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <param name="iOrgClass">要求展现机构的类型</param>
		/// <param name="iOrgType">要求展现机构的属性</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			int iLot,
			int iLod,
			int iDepth,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strHideType,
			string strAttrs,
			int iOrgClass,
			int iOrgType)
		{
			strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

			SearchOrgChildrenCondition scc = new SearchOrgChildrenCondition(strOrgValues, soc, strAttrs);
			scc.ListObjDelete = (ListObjectDelete)iLod;
			scc.ListObjType = (ListObjectType)iLot;
			scc.Depth = iDepth;
			scc.OrgRankCN = strOrgRankCodeName;
			scc.UserRankCN = strUserRankCodeName;
			scc.HideType = strHideType;
			scc.OrgClass = iOrgClass;
			scc.OrgType = iOrgType;

			return GetOrganizationChildren(scc);
		}

		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="iLot">要求查询的数据对象类型（机构、组、人员、兼职对象）</param>
		/// <param name="iLod">是否包含被逻辑删除的成员</param>
		/// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>      
		/// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <param name="iOrgClass">要求展现机构的类型</param>
		/// <param name="iOrgType">要求展现机构的属性</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		/// 
		//2009-05-07
		public static DataSet GetOrganizationChildren2(string strOrgValues,
			SearchObjectColumn soc,
			int iLot,
			int iLod,
			int iDepth,
			string strHideType,
			string strAttrs,
			int iOrgClass,
			int iOrgType)
		{
			strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

			SearchOrgChildrenCondition scc = new SearchOrgChildrenCondition(strOrgValues, soc, strAttrs);
			scc.ListObjDelete = (ListObjectDelete)iLod;
			scc.ListObjType = (ListObjectType)iLot;
			scc.Depth = iDepth;
			scc.HideType = strHideType;
			scc.OrgClass = iOrgClass;
			scc.OrgType = iOrgType;

			return GetOrganizationChildren2(scc);
		}

		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （0、空；1、GUID；2、USER_GUID；3、ORIGINAL_SORT；4、GLOBAL_SORT；5、ALL_PATH_NAME；6、LOGON_NAME；）</param>
		/// <param name="iLot">要求查询的数据对象类型（机构、组、人员、兼职对象）</param>
		/// <param name="iLod">是否包含被逻辑删除的成员</param>
		/// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
		/// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
		/// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
		/// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			int iLot,
			int iLod,
			int iDepth,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strHideType,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strHideType, strAttrs, 0, 0);
		}


		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="iLot">要求查询的数据对象类型（机构、组、人员、兼职对象）</param>
		/// <param name="iLod">是否包含被逻辑删除的成员</param>
		/// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
		/// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
		/// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			int iLot,
			int iLod,
			int iDepth,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, string.Empty, strAttrs);
		}

		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="iLod">是否包含被逻辑删除的成员</param>
		/// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
		/// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
		/// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			int iLod,
			int iDepth,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, (int)(ListObjectType.ORGANIZATIONS | ListObjectType.USERS), iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strAttrs);
		}

		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
		/// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
		/// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			int iDepth,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, (int)ListObjectDelete.COMMON, iDepth, strOrgRankCodeName, strUserRankCodeName, strAttrs);
		}

		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
		/// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, 0, strOrgRankCodeName, strUserRankCodeName, strAttrs);
		}

		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			string strUserRankCodeName,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, string.Empty, strUserRankCodeName, strAttrs);
		}

		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, string.Empty, strAttrs);
		}

		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgGuids">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		public static DataSet GetOrganizationChildren(string strOrgGuids, string strAttrs)
		{
			return GetOrganizationChildren(strOrgGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// 获取指定部门下的所有子对象
		/// </summary>
		/// <param name="strOrgGuids">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		/// <returns>获取指定部门下的所有子对象的查询结果</returns>
		public static DataSet GetOrganizationChildren(string strOrgGuids)
		{
			return GetOrganizationChildren(strOrgGuids, string.Empty);
		}

		/// <summary>
		/// 按照一定的查询条件查询系统中的数据对象
		/// </summary>
		/// <param name="scc">系统的查询条件对象</param>
		/// <returns>按照一定的查询条件查询系统中的数据对象</returns>
		/// 
		//2009-05-11
		public static DataSet GetOrganizationChildren(SearchOrgChildrenCondition scc)
		{
			string searchKey = scc.GetHashString();
#if DEBUG
			long cast = DateTime.Now.Ticks;
			Trace.WriteLine(searchKey);
#endif
			DataSet result;

			string strRootGuids = TransHashToSqlString(scc.RootGuids).Trim('\'');
			//得到All_Path_Name
			DataSet dsRootOrg = OGUReader.GetObjectsDetail("ORGANIZATIONS",
						strRootGuids,
						SearchObjectColumn.SEARCH_GUID,
						string.Empty,
						SearchObjectColumn.SEARCH_GUID);

			ExceptionHelper.FalseThrow(dsRootOrg.Tables[0].Rows.Count > 0, "不能找到ID为{0}的机构", strRootGuids);
			string rootPath = dsRootOrg.Tables[0].Rows[0]["ORIGINAL_SORT"].ToString();

			if (false == Caching.GetOrganizationChildrenQueue.Instance.TryGetValue(searchKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					StringBuilder strB = new StringBuilder(1024);
					if ((scc.ListObjType & ListObjectType.ORGANIZATIONS) != ListObjectType.None)
					{
						strB.Append(" ( " + GetOrganizationsSqlByScc(scc, rootPath) + " \n )");
					}

					if ((scc.ListObjType & ListObjectType.GROUPS) != ListObjectType.None)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");
						strB.Append(" ( " + GetGroupsSqlByScc(scc, rootPath) + " \n )");
					}

					if ((scc.ListObjType & ListObjectType.USERS) != ListObjectType.None)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");

						strB.Append(" ( " + GetUsersSqlByScc(scc, rootPath) + " \n )");
					}

					string strSql = "SELECT * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";

					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				Caching.GetOrganizationChildrenQueue.Instance.Add(searchKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
#if DEBUG
			cast = DateTime.Now.Ticks - cast;
			Trace.WriteLine(cast.ToString(), "Cast Time");
#endif
			return result;
		}

		/// <summary>
		/// 按照一定的查询条件查询系统中的数据对象
		/// </summary>
		/// <param name="scc">系统的查询条件对象</param>
		/// <returns>按照一定的查询条件查询系统中的数据对象</returns>
		/// 
		//2009-05-07
		public static DataSet GetOrganizationChildren2(SearchOrgChildrenCondition scc)
		{
			string searchKey = scc.GetHashString();
#if DEBUG
			long cast = DateTime.Now.Ticks;
			Trace.WriteLine(searchKey);
#endif
			DataSet result;

			string strRootGuids = TransHashToSqlString(scc.RootGuids).Trim('\'');
			//得到All_Path_Name
			DataSet dsRootOrg = OGUReader.GetObjectsDetail("ORGANIZATIONS",
						strRootGuids,
						SearchObjectColumn.SEARCH_GUID,
						string.Empty,
						SearchObjectColumn.SEARCH_GUID);

			ExceptionHelper.FalseThrow(dsRootOrg.Tables[0].Rows.Count > 0, "不能找到ID为{0}的机构", strRootGuids);
			string rootPath = dsRootOrg.Tables[0].Rows[0]["ORIGINAL_SORT"].ToString();


			if (false == Caching.GetOrganizationChildrenQueue.Instance.TryGetValue(searchKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					StringBuilder strB = new StringBuilder(1024);
					if ((scc.ListObjType & ListObjectType.ORGANIZATIONS) != ListObjectType.None)
					{
						strB.Append(" ( " + GetOrganizationsSqlByScc2(scc, rootPath) + " \n )");
					}

					if ((scc.ListObjType & ListObjectType.GROUPS) != ListObjectType.None)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");
						strB.Append(" ( " + GetGroupsSqlByScc2(scc, rootPath) + " \n )");
					}

					if ((scc.ListObjType & ListObjectType.USERS) != ListObjectType.None)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");

						strB.Append(" ( " + GetUsersSqlByScc2(scc, rootPath) + " \n )");
					}

					string strSql = "SELECT * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";

					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				Caching.GetOrganizationChildrenQueue.Instance.Add(searchKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
#if DEBUG
			cast = DateTime.Now.Ticks - cast;
			Trace.WriteLine(cast.ToString(), "Cast Time");
#endif
			return result;
		}

		#endregion

		#region IsUserInObjects
		///// <summary>
		///// 获取指定部门下的所有子对象
		///// </summary>
		///// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		///// <param name="soc">查询要求的查询列名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		///// <param name="iLot">要求查询的数据对象类型（机构、组、人员、兼职对象）</param>
		///// <param name="iLod">是否包含被逻辑删除的成员</param>
		///// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
		///// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
		///// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
		///// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		///// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		///// <param name="iOrgClass">要求展现机构的类型</param>
		///// <param name="iOrgType">要求展现机构的属性</param>
		///// <param name="da">数据库操作对象</param>
		///// <returns>获取指定部门下的所有子对象的查询结果</returns>
		//public static DataSet GetOrganizationChildren(string strOrgValues, SearchObjectColumn soc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs, int iOrgClass, int iOrgType, DataAccess da)
		//{
		//    strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

		//    SearchOrgChildrenCondition scc = new SearchOrgChildrenCondition(strOrgValues, soc, strAttrs, da);
		//    scc.ListObjDelete = (ListObjectDelete)iLod;
		//    scc.ListObjType = (ListObjectType)iLot;
		//    scc.Depth = iDepth;
		//    scc.OrgRankCN = strOrgRankCodeName;
		//    scc.UserRankCN = strUserRankCodeName;
		//    scc.HideType = strHideType;
		//    scc.OrgClass = iOrgClass;
		//    scc.OrgType = iOrgType;
		//    return GetOrganizationChildren(scc, da);
		//}

		///// <summary>
		///// 获取指定部门下的所有子对象
		///// </summary>
		///// <param name="strOrgValues">要求查询的部门对象(父部门标识,多个之间采用","分隔)</param>
		///// <param name="soc">查询要求的查询列名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		///// <param name="iLot">要求查询的数据对象类型（机构、组、人员、兼职对象）</param>
		///// <param name="iLod">是否包含被逻辑删除的成员</param>
		///// <param name="iDepth">要求查询的层次（最少一层）（0代表全部子对象）</param>
		///// <param name="strOrgRankCodeName">查询中要求的机构对象级别</param>
		///// <param name="strUserRankCodeName">查询中要求的人员对象级别</param>
		///// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		///// <param name="strAttrs">查询中要求获取数据对象的属性类型</param>
		///// <param name="da">数据库操作对象</param>
		///// <returns>获取指定部门下的所有子对象的查询结果</returns>
		//public static DataSet GetOrganizationChildren(string strOrgValues, SearchObjectColumn soc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs, DataAccess da)
		//{
		//    return GetOrganizationChildren(strOrgValues, soc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strHideType, strAttrs, 0, 0, da);
		//}

		///// <summary>
		///// 按照一定的查询条件查询系统中的数据对象
		///// </summary>
		///// <param name="scc">系统的查询条件对象</param>
		///// <param name="da">数据库操作对象</param>
		///// <returns>按照一定的查询条件查询系统中的数据对象</returns>
		//public static DataSet GetOrganizationChildren(SearchOrgChildrenCondition scc, DataAccess da)
		//{
		//    StringBuilder strB = new StringBuilder(1024);
		//    if ((scc.ListObjType & ListObjectType.ORGANIZATIONS) != 0)
		//    {
		//        strB.Append(" ( " + GetOrganizationsSqlByScc(scc, da) + " \n )");
		//    }

		//    if ((scc.ListObjType & ListObjectType.GROUPS) != 0)
		//    {
		//        if (strB.Length > 0)
		//            strB.Append(" \n UNION \n ");
		//        strB.Append(" ( " + GetGroupsSqlByScc(scc, da) + " \n )");
		//    }

		//    if ((scc.ListObjType & ListObjectType.USERS) != 0)
		//    {
		//        if (strB.Length > 0)
		//            strB.Append(" \n UNION \n ");

		//        strB.Append(" ( " + GetUsersSqlByScc(scc, da) + " \n )");
		//    }

		//    string strSql = "SELECT * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";

		//    return OGUCommonDefine.ExecuteDataset(strSql, da);
		//}

		/// <summary>
		/// 判断一个用户是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="strUserValue">用户的属性数据值</param>
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
		/// <param name="objectXmlDoc">判断对象的属性数据值</param>
		/// <param name="soc">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="lod">是否包含被逻辑删除的成员</param>
		/// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		/// <param name="bDirect">是否直接从属（无中间部门）</param>
		/// <param name="bFitAll">是否要求完全匹配（存在于指定的每一个部门中）</param>
		/// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
		/// <remarks>
		/// objectXmlDoc的结构如下：
		/// <code>
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static bool IsUserInObjects(string strUserValue,
			SearchObjectColumn socu,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soc,
			ListObjectDelete lod,
			string strHideType,
			bool bDirect,
			bool bFitAll)
		{
			string cacheKey = Common.InnerCacheHelper.BuildCacheKey(strUserValue,
				socu,
				strUserParentGuid,
				objectXmlDoc,
				soc,
				lod,
				strHideType,
				bDirect,
				bFitAll);

			bool result;
			//if (false == IsUserInObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(IsUserInObjectsQueue))
			//    {
			if (false == IsUserInObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strUserColName = OGUCommonDefine.GetSearchObjectColumn(socu);
				string strObjColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				strHideType = OGUCommonDefine.GetHideType(strHideType);
				string strHideList = GetHideTypeFromXmlForLike(strHideType, "OU_USERS");

				string strDirect = "%";
				if (bDirect)
					strDirect = "______";

				string strUserLimit = " AND " + DatabaseSchema.Instence.GetTableColumns(strUserColName, "OU_USERS", "USERS")
					+ " = " + TSqlBuilder.Instance.CheckQuotationMark(strUserValue, true);
				if (strUserParentGuid.Length > 0)
					strUserLimit += "	AND OU_USERS.PARENT_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(strUserParentGuid, true);

				string strDelUser = GetSqlSearchStatus("OU_USERS", lod);
				StringBuilder strB = new StringBuilder(1024);
				#region 内部实现
				XmlElement root = objectXmlDoc.DocumentElement;
				foreach (XmlElement elem in root.ChildNodes)
				{
					string strRankLimit = string.Empty;
					if (elem.GetAttribute("rankCode") != null && elem.GetAttribute("rankCode").Length > 0)
						strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
							+ TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("rankCode"), true) + ") ";

					switch (elem.LocalName)
					{
						case "ORGANIZATIONS":
							strB.Append(@"
							SELECT DISTINCT ORGANIZATIONS.GUID, OU_USERS.USER_GUID 
							FROM OU_USERS, ORGANIZATIONS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
								" + strRankLimit + @"
							WHERE OU_USERS.USER_GUID = USERS.GUID
								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + "
									  + TSqlBuilder.Instance.CheckQuotationMark(strDirect, true) + @" 
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS") + @" = "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND (" + strDelUser + @") 
								" + strUserLimit + strHideList + @";");
							break;
						case "GROUPS":
							strB.Append(@"
							SELECT DISTINCT GROUPS.GUID, OU_USERS.USER_GUID
							FROM GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
								" + strRankLimit + @"
							WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
								AND OU_USERS.USER_GUID = GROUP_USERS.USER_GUID
								AND OU_USERS.PARENT_GUID = GROUP_USERS.USER_PARENT_GUID
								AND OU_USERS.USER_GUID = USERS.GUID
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + @" = "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND (" + strDelUser + @") 
								" + strUserLimit + strHideList + @";");
							break;
						case "USERS":
							strB.Append(@"
							SELECT DISTINCT USERS.GUID, OU_USERS.USER_GUID
							FROM OU_USERS, USERS
							WHERE OU_USERS.USER_GUID = USERS.GUID
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "OU_USERS", "USERS") + " =  "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND (" + strDelUser + @")" + strUserLimit + strHideList + @";");
							break;
						default: ExceptionHelper.TrueThrow(true, "对不起,系统没有对应处理“" + elem.LocalName + "”的相应程序！");
							break;
					}
				}
				#endregion
				result = CheckIsUserInOrganizations(strB, bFitAll);

				IsUserInObjectsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}

			return result;
		}
		///// <summary>
		///// 判断一个用户是否存在于指定的多个部门之中
		///// </summary>
		///// <param name="strUserValue">用户的属性数据值</param>
		///// 
		///// <param name="socu">用户的属性名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		///// </param>
		///// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
		///// <param name="objectXmlDoc">判断对象的属性数据值</param>
		///// <param name="soco">机构的属性名称
		///// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		///// </param>
		///// <param name="lod">是否包含被逻辑删除的成员</param>
		///// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		///// <param name="bDirect">是否直接从属（无中间部门）</param>
		///// <param name="bFitAll">是否要求完全匹配（存在于指定的每一个部门中）</param>
		///// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
		//public static bool IsUserInObjects(string strUserValue, SearchObjectColumn socu, string strUserParentGuid, XmlDocument objectXmlDoc, SearchObjectColumn soco, ListObjectDelete lod, string strHideType, bool bDirect, bool bFitAll)
		//{
		//    using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
		//    {
		//        return IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, lod, strHideType, bDirect, bFitAll, da);
		//    }
		//}

		/// <summary>
		/// 判断一个用户是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="strUserValue">用户的属性数据值</param>
		/// 
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
		/// <param name="objectXmlDoc">判断对象的属性数据值</param>
		/// <param name="soco">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="lod">是否包含被逻辑删除的成员</param>
		/// <param name="bDirect">是否直接从属（无中间部门）</param>
		/// <param name="bFitAll">是否要求完全匹配（存在于指定的每一个部门中）</param>
		/// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
		public static bool IsUserInObjects(string strUserValue,
			SearchObjectColumn socu,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soco,
			ListObjectDelete lod,
			bool bDirect,
			bool bFitAll)
		{
			return IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, lod, string.Empty, bDirect, bFitAll);
		}

		/// <summary>
		/// 判断一个用户是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="strUserValue">用户的属性数据值</param>
		/// 
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
		/// <param name="objectXmlDoc">判断对象的属性数据值</param>
		/// <param name="soco">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="bDirect">是否直接从属（无中间部门）</param>
		/// <param name="bFitAll">是否要求完全匹配（存在于指定的每一个部门中）</param>
		/// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
		public static bool IsUserInObjects(string strUserValue,
			SearchObjectColumn socu,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soco,
			bool bDirect,
			bool bFitAll)
		{
			return IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, ListObjectDelete.COMMON, bDirect, bFitAll);
		}

		/// <summary>
		/// 判断一个用户是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="strUserValue">用户的属性数据值</param>
		/// 
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
		/// <param name="objectXmlDoc">判断对象的属性数据值</param>
		/// <param name="soco">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="bFitAll">是否要求完全匹配（存在于指定的每一个部门中）</param>
		/// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
		public static bool IsUserInObjects(string strUserValue,
			SearchObjectColumn socu,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soco,
			bool bFitAll)
		{
			return IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, false, bFitAll);
		}

		/// <summary>
		/// 判断一个用户是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="strUserValue">用户的属性数据值</param>
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
		/// <param name="objectXmlDoc">判断对象的属性数据值</param>
		/// <param name="soco">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
		public static bool IsUserInObjects(string strUserValue,
			SearchObjectColumn socu,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soco)
		{
			return IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, false);
		}

		/// <summary>
		/// 判断一个用户是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="strUserGuid">用户的属性数据值</param>
		/// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
		/// <param name="objectXmlDoc">判断对象的属性数据值</param>
		/// <param name="soco">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
		public static bool IsUserInObjects(string strUserGuid,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soco)
		{
			return IsUserInObjects(strUserGuid, SearchObjectColumn.SEARCH_GUID, strUserParentGuid, objectXmlDoc, soco);
		}

		/// <summary>
		/// 判断一个用户是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="strUserGuid">用户的属性数据值</param>
		/// <param name="strUserParentGuid">指定用户所在的父部门标识（可空）</param>
		/// <param name="objectXmlDoc">判断对象的属性数据值</param>
		/// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
		public static bool IsUserInObjects(string strUserGuid,
			string strUserParentGuid,
			XmlDocument objectXmlDoc)
		{
			return IsUserInObjects(strUserGuid, strUserParentGuid, objectXmlDoc, SearchObjectColumn.SEARCH_GUID);
		}

		/// <summary>
		/// 判断一个用户是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="strUserGuid">用户的属性数据值</param>
		/// <param name="objectXmlDoc">判断对象的属性数据值</param>
		/// <returns>判断一个用户是否存在于指定的多个部门之中</returns>
		public static bool IsUserInObjects(string strUserGuid,
			XmlDocument objectXmlDoc)
		{
			return IsUserInObjects(strUserGuid, string.Empty, objectXmlDoc);
		}

		#endregion

		#region CheckUserInObjects
		/// <summary>
		/// 判断用户群是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="xmlUserDoc">用户群标识（多个之间采用","分隔）</param>
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="xmlObjDoc">机构群（采用XML方式）</param>
		/// <param name="soc">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="lod">是否包含被逻辑删除的成员</param>
		/// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		/// <param name="bDirect">是否直接从属（无中间部门）</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc的返回结果（字节点方式嵌入返回）：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc,
			SearchObjectColumn socu,
			XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			ListObjectDelete lod,
			string strHideType,
			bool bDirect)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(xmlUserDoc.DocumentElement.OuterXml,
				socu,
				xmlObjDoc.DocumentElement.OuterXml,
				soc,
				lod,
				strHideType,
				bDirect);
			XmlDocument result;
#if DEBUG
			Debug.WriteLine(xmlObjDoc.OuterXml, "begin");
#endif
			//if (false == CheckUserInObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(CheckUserInObjectsQueue))
			//    {
			if (false == CheckUserInObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strUserColName = OGUCommonDefine.GetSearchObjectColumn(socu);
				string strObjColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				strHideType = OGUCommonDefine.GetHideType(strHideType);
				string strHideList = GetHideTypeFromXmlForLike(strHideType, "OU_USERS");

				string strDelUser = GetSqlSearchStatus("OU_USERS", lod);

				string strDirect = "%";
				if (bDirect)
					strDirect = "______";
				#region Db control
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					string strUserLimit = GetUserLimitInCheckUserInOrganizations(xmlUserDoc, strUserColName);
					/*****************************************************************************************************************/
					string strExtraWhere = @"
								AND (" + strDelUser + @") 
								AND " + strUserLimit + strHideList;
					StringBuilder strB = GetSqlSearchForCheckUserInObjects(xmlObjDoc, strUserColName, strObjColName, strDirect, strExtraWhere);

					if (strB.Length > 0)
					{
						Database database = DatabaseFactory.Create(context);
						DataSet ds = database.ExecuteDataSet(CommandType.Text, strB.ToString());
						foreach (DataTable oTable in ds.Tables)
						{
							foreach (DataRow row in oTable.Rows)
							{
								XmlNode uNode = xmlUserDoc.DocumentElement.SelectSingleNode("USERS[@oValue=\"" + OGUCommonDefine.DBValueToString(row["USER_VALUE"]) + "\"]");
								if (uNode != null)
								{
									XmlElement oRoot = xmlObjDoc.DocumentElement;
									string oClass = OGUCommonDefine.DBValueToString(row["OBJECTCLASS"]);
									string oValue = OGUCommonDefine.DBValueToString(row["OBJ_VALUE"]);

									foreach (XmlElement oElem in oRoot.SelectNodes(oClass + "[@oValue=\"" + oValue + "\"]"))
									{
										if (row["RANK_CODE"] is DBNull)
										{
											if (oElem.GetAttribute("rankCode") == string.Empty)
											{
												XmlElement uElem = (XmlElement)XmlHelper.AppendNode(oElem, uNode.LocalName);
												foreach (XmlAttribute xAttr in uNode.Attributes)
													uElem.SetAttribute(xAttr.LocalName, xAttr.InnerText);
											}
										}
										else
										{
											if (oElem.GetAttribute("rankCode") == OGUCommonDefine.DBValueToString(row["RANK_CODE"]))
											{
												XmlElement uElem = (XmlElement)XmlHelper.AppendNode(oElem, uNode.LocalName);
												foreach (XmlAttribute xAttr in uNode.Attributes)
													uElem.SetAttribute(xAttr.LocalName, xAttr.InnerText);
											}
										}
									}
								}
							}
						}
					}
				}
				#endregion

				#region Deleted
				/*****************************************************************************************************************/
				/*******下面一段程序并没有错误，但是性能上有差异。以下程序针对每个OBJ对象产生一条SQL查询语句，********************/
				/*******在对象多的情况下导致SQL查询语句的庞大，修改参阅GetSqlSearchForCheckUserInObjects**************************/
				/*****************************************************************************************************************/
				//			foreach (XmlElement elem in xmlObjDoc.DocumentElement.ChildNodes)
				//			{
				//				string strRankLimit = elem.GetAttribute("rankCode");
				//				if (strRankLimit != null && strRankLimit.Length > 0)
				//					strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("rankCode")) + ") ";
				//				else
				//					strRankLimit = string.Empty;
				//
				//				string strSql = string.Empty;
				//				switch (elem.LocalName)
				//				{
				//					case "ORGANIZATIONS":
				//						strSql = @"
				//							SELECT DISTINCT " + GetTableColumns(strUserColName, da, "USERS", "OU_USERS") + " AS USER_VALUE, " + GetTableColumns(strObjColName, da, "ORGANIZATIONS") + @" AS OBJ_VALUE
				//							FROM ORGANIZATIONS, OU_USERS, USERS JOIN RANK_DEFINE ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
				//							WHERE OU_USERS.USER_GUID = USERS.GUID
				//								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + " + TSqlBuilder.Instance.CheckQuotationMark(strDirect) + @"
				//								AND (" + strDelUser + @")
				//								AND " + GetTableColumns(strObjColName, da, "ORGANIZATIONS") + " = " + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue")) + @"
				//								AND " + strUserLimit + @"
				//								" + strRankLimit + strHideList + @"
				//							ORDER BY OBJ_VALUE";
				//						break;
				//					case "GROUPS":
				//						strSql = @"
				//							SELECT DISTINCT " + GetTableColumns(strUserColName, da, "USERS", "OU_USERS") + " AS USER_VALUE, " + GetTableColumns(strObjColName, da, "GROUPS") + @" AS OBJ_VALUE
				//							FROM GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
				//							WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
				//								AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
				//								AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
				//								AND OU_USERS.USER_GUID = USERS.GUID
				//								AND GROUPS." + strObjColName + " = " + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue")) + @"
				//								AND ( " + strDelUser + @" ) 
				//								AND " + strUserLimit + strHideList + @"
				//								" + strRankLimit + @"
				//							ORDER BY OBJ_VALUE";
				//						break;
				//					case "USERS":
				//						strSql = @"
				//							SELECT DISTINCT " + GetTableColumns(strUserColName, da, "USERS", "OU_USERS") + " AS USER_VALUE, " + GetTableColumns(strObjColName, da, "USERS", "OU_USERS") + @" AS OBJ_VALUE
				//							FROM OU_USERS, USERS
				//							WHERE OU_USERS.USER_GUID = USERS.GUID
				//								AND " + GetTableColumns(strObjColName, da, "USERS", "OU_USERS") + " = " + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue")) + @"
				//								AND ( " + strDelUser + @" ) 
				//								AND " + strUserLimit + strHideList + @"
				//							ORDER BY OBJ_VALUE ";
				//						break;
				//					default :	ExceptionHelper.TrueThrow(true, "对不起,系统没有对应处理“" + elem.LocalName + "”的相应程序！");
				//						break;
				//				}
				//
				//				strB.Append(strSql + ";\n");
				//			}
				//			if (strB.Length > 0)
				//			{
				//				DataSet ds = OGUCommonDefine.ExecuteDataset(strB.ToString(), da);
				//				for (int iTable = 0; iTable < ds.Tables.Count; iTable++)
				//				{
				//					foreach (DataRow row in ds.Tables[iTable].Rows)
				//					{
				//						XmlNode uNode = xmlUserDoc.DocumentElement.SelectSingleNode("USERS[@oValue=\"" + XmlHelper.DBValueToString(row["USER_VALUE"]) + "\"]");
				//						if (uNode != null)
				//						{						
				//							XmlElement oElem = (XmlElement)XmlHelper.AppendNode(xmlObjDoc, xmlObjDoc.DocumentElement.ChildNodes[iTable], uNode.LocalName);
				//							foreach (XmlAttribute xAttr in uNode.Attributes)
				//								oElem.SetAttribute(xAttr.LocalName, xAttr.InnerText);
				//						}
				//					}
				//				}
				//			}
				/*****************************************************************************************************************/
				#endregion

				result = xmlObjDoc;
				CheckUserInObjectsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			xmlObjDoc = result;
#if DEBUG
			Debug.WriteLine(xmlObjDoc.OuterXml, "result");
#endif
		}
		///// <summary>
		///// 判断用户群是否存在于指定的多个部门之中
		///// </summary>
		///// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
		///// <param name="socu">用户的属性名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		///// <param name="xmlObjDoc">机构群（采用XML方式）</param>
		///// <param name="soc">机构的属性名称
		///// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		///// <param name="lod">是否包含被逻辑删除的成员</param>
		///// <param name="strHideType">查询中要求屏蔽的数据(对应于配置文件HideTypes.xml中的配置)</param>
		///// <param name="bDirect">是否直接从属（无中间部门）</param>
		///// <remarks>
		///// <code>
		///// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
		/////		<USERS>
		/////			<USERS oValue="" parentGuid="" />
		/////			<USERS oValue="" parentGuid="" />
		/////		</USERS>
		///// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
		/////		<OBJECTS>
		/////			<ORGANIZATIONS oValue="" rankCode="" />
		/////			<GROUPS oValue="" rankCode="" />
		/////			<USERS oValue="" parentGuid="" />
		/////		</OBJECTS>
		///// </code>
		///// xmlObjDoc的返回结果（字节点方式嵌入返回）：
		///// <OBJECTS>
		/////			<ORGANIZATIONS oValue="" rankCode="" >
		/////				<USERS oValue="" parentGuid=""/>
		/////				<USERS oValue="" parentGuid=""/>
		/////			</ORGANIZATIONS>
		/////			<GROUPS oValue="" rankCode="" >
		/////				<USERS oValue="" parentGuid=""/>
		/////				<USERS oValue="" parentGuid=""/>
		/////			</GROUPS>
		/////			<USERS oValue="" parentGuid="" >
		/////				<USERS oValue="" parentGuid=""/>
		/////			</USERS>
		/////		</OBJECTS>
		///// </remarks>
		//public static void CheckUserInObjects(XmlDocument xmlUserDoc, SearchObjectColumn socu, XmlDocument xmlObjDoc, SearchObjectColumn soc, ListObjectDelete lod, string strHideType, bool bDirect)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, lod, strHideType, bDirect, da);
		//    }
		//}

		/// <summary>
		/// 判断用户群是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="xmlObjDoc">机构群（采用XML方式）</param>
		/// <param name="soc">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="lod">是否包含被逻辑删除的成员</param>
		/// <param name="bDirect">是否直接从属（无中间部门）</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc的返回结果（字节点方式嵌入返回）：
		/// <OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc,
			SearchObjectColumn socu,
			XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			ListObjectDelete lod,
			bool bDirect)
		{
			CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, lod, string.Empty, bDirect);
		}

		/// <summary>
		/// 判断用户群是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="xmlObjDoc">机构群（采用XML方式）</param>
		/// <param name="soc">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="bDirect">是否直接从属（无中间部门）</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc的返回结果（字节点方式嵌入返回）：
		/// <OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc,
			SearchObjectColumn socu,
			XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			bool bDirect)
		{
			CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, ListObjectDelete.COMMON, bDirect);
		}

		/// <summary>
		/// 判断用户群是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="xmlObjDoc">机构群（采用XML方式）</param>
		/// <param name="soc">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc的返回结果（字节点方式嵌入返回）：
		/// <OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc,
			SearchObjectColumn socu,
			XmlDocument xmlObjDoc,
			SearchObjectColumn soc)
		{
			CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, false);
		}

		/// <summary>
		/// 判断用户群是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="xmlUserDoc">用户群标识（采用XML方式）</param>
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="xmlObjDoc">机构群（采用XML方式）(默认记录GUID)</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc的返回结果（字节点方式嵌入返回）：
		/// <OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc, SearchObjectColumn socu, XmlDocument xmlObjDoc)
		{
			CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, SearchObjectColumn.SEARCH_GUID);
		}

		/// <summary>
		/// 判断用户群是否存在于指定的多个部门之中
		/// </summary>
		/// <param name="xmlUserDoc">用户群标识（采用XML方式）(默认记录GUID)</param>
		/// <param name="xmlObjDoc">机构群（采用XML方式）(默认记录GUID)</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc的结构如下（说明oValue必填，与socu配合使用；parentGuid可不填）：
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc的结构如下（说明oValue必填，与soc配合使用；parentGuid可不填；rankCode可不填）：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc的返回结果（字节点方式嵌入返回）：
		/// <OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc, XmlDocument xmlObjDoc)
		{
			CheckUserInObjects(xmlUserDoc, SearchObjectColumn.SEARCH_GUID, xmlObjDoc);
		}
		#endregion

		#region GetAllUsersInAllObjects
		/// <summary>
		/// 获取指定对象中的所有用户对象
		/// </summary>
		/// <param name="xmlObjDoc">要求被查询的数据对象</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
		/// <param name="soco">要求所在机构的范围的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="lot">要求被查询的数据对象类型（主要是用于辨别是否要求查询兼职人员）</param>
		/// <param name="lod">系统中被逻辑删除对象是否查询取出</param>
		/// <param name="strHideType">要求隐藏的设置类型</param>
		/// <param name="strAttrs">要求获得的数据属性</param>
		/// <returns>获取指定对象中的所有用户对象</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc的结构：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			string strOrgLimitValues,
			SearchObjectColumn soco,
			ListObjectType lot,
			ListObjectDelete lod,
			string strHideType,
			string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(xmlObjDoc.DocumentElement.OuterXml,
				soc,
				strOrgLimitValues,
				soco,
				lot,
				lod,
				strHideType,
				strAttrs);
			DataSet result;
			//if (false == GetAllUsersInAllObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetAllUsersInAllObjectsQueue))
			//    {
			if (false == GetAllUsersInAllObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				#region Prepare Db
				ExceptionHelper.TrueThrow(xmlObjDoc.DocumentElement.ChildNodes.Count <= 0, "对不起，系统没有给定要求查询的数据对象！");
				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				string strObjColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				string strDelUser = GetSqlSearchStatus("OU_USERS", lod);

				strHideType = OGUCommonDefine.GetHideType(strHideType);
				string strHideList = GetHideTypeFromXmlForLike(strHideType, "OU_USERS");

				string strListObjectType = string.Empty;
				if ((lot & ListObjectType.SIDELINE) == 0)
					strListObjectType = " AND OU_USERS.SIDELINE = 0 ";

				if (strOrgLimitValues.Length == 0)
				{
					strOrgLimitValues = AccreditSection.GetConfig().AccreditSettings.OguRootName;// (new SysConfig()).GetDataFromConfig("OGURootName", string.Empty);
					ExceptionHelper.TrueThrow<ApplicationException>(string.IsNullOrEmpty(strOrgLimitValues),
						"对不起，您没有配置好系统默认指定的初始机构！请检查web.config中的configuration\\appSettings\\<add key=\"OGURootName\" value=\"\" />");
					soco = SearchObjectColumn.SEARCH_ALL_PATH_NAME;
				}
				#endregion
				StringBuilder strB = new StringBuilder(1024);

				foreach (XmlElement elem in xmlObjDoc.DocumentElement.ChildNodes)
				{
					string strRankLimit = elem.GetAttribute("rankCode");
					if (strRankLimit != null && strRankLimit.Length > 0)
						strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
							+ TSqlBuilder.Instance.CheckQuotationMark(strRankLimit, true) + " ) ";
					else
						strRankLimit = string.Empty;

					string strSql = string.Empty;
					switch (elem.LocalName)
					{
						case "ORGANIZATIONS":
							#region ORGANIZATIONS
							strSql = @"
							SELECT 'USERS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
							FROM ORGANIZATIONS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
								" + strRankLimit + @",
								(
									SELECT ORIGINAL_SORT
									FROM ORGANIZATIONS
									WHERE " + OGUCommonDefine.GetSearchObjectColumn(soco) + " IN ("
												+ OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgLimitValues) + @" )
								) ORG_LIMIT
							WHERE USERS.GUID = OU_USERS.USER_GUID 
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS") + " = "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
								AND OU_USERS.ORIGINAL_SORT LIKE ORG_LIMIT.ORIGINAL_SORT + '%'
								AND (" + strDelUser + @") 
								" + strHideList + strListObjectType;
							break;
							#endregion
						case "GROUPS":
							#region GROUPS
							strSql = @"
							SELECT 'USERS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
							FROM GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
								" + strRankLimit + @",
								(
									SELECT ORIGINAL_SORT
									FROM ORGANIZATIONS
									WHERE " + OGUCommonDefine.GetSearchObjectColumn(soco) + " IN ("
												+ OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgLimitValues) + @" )
								) ORG_LIMIT
							WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
								AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
								AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
								AND USERS.GUID = OU_USERS.USER_GUID 
								AND OU_USERS.ORIGINAL_SORT LIKE ORG_LIMIT.ORIGINAL_SORT + '%'
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + " = "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND (" + strDelUser + @") 
								" + strHideList + strListObjectType;
							break;
							#endregion
						case "USERS":
							#region USERS
							strSql = @"
							SELECT 'USERS' AS OBJECTCLASS, "
								+ DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
							FROM OU_USERS, USERS JOIN RANK_DEFINE 
								ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
								" + strRankLimit + @",
								(
									SELECT ORIGINAL_SORT
									FROM ORGANIZATIONS
									WHERE " + OGUCommonDefine.GetSearchObjectColumn(soco) + " IN ("
												+ OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgLimitValues) + @" )
								) ORG_LIMIT
							WHERE OU_USERS.USER_GUID = USERS.GUID
								AND OU_USERS.ORIGINAL_SORT LIKE ORG_LIMIT.ORIGINAL_SORT + '%'
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "OU_USERS", "USERS") + " = "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND (" + strDelUser + @") 
								" + strHideList + strListObjectType;
							break;
							#endregion
						default: ExceptionHelper.TrueThrow(true, "对不起,系统没有对应处理“" + elem.LocalName + "”的相应程序！");
							break;
					}

					if (strB.Length > 0)
						strB.Append(Environment.NewLine + " UNION " + Environment.NewLine);

					strB.Append("(" + strSql + ")");
				}

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					string sql = "SELECT * FROM (" + strB.ToString() + ") A ORDER BY GLOBAL_SORT";
					result = database.ExecuteDataSet(CommandType.Text, sql);
				}

				GetAllUsersInAllObjectsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// 获取指定对象中的所有用户对象
		///// </summary>
		///// <param name="xmlObjDoc">要求被查询的数据对象</param>
		///// <param name="soc">查询要求的查询列名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		///// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
		///// <param name="soco">要求所在机构的范围的查询列名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		///// <param name="lot">要求被查询的数据对象类型（主要是用于辨别是否要求查询兼职人员）</param>
		///// <param name="lod">系统中被逻辑删除对象是否查询取出</param>
		///// <param name="strHideType">要求隐藏的设置类型</param>
		///// <param name="strAttrs">要求获得的数据属性</param>
		///// <returns>获取指定对象中的所有用户对象</returns>
		///// <remarks>
		///// <code>
		///// xmlObjDoc的结构：
		/////		<OBJECTS>
		/////			<ORGANIZATIONS oValue="" rankCode="" />
		/////			<GROUPS oValue="" rankCode="" />
		/////			<USERS oValue="" parentGuid="" />
		/////		</OBJECTS>
		///// </code>
		///// </remarks>
		//public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, SearchObjectColumn soc, string strOrgLimitValues, SearchObjectColumn soco, ListObjectType lot, ListObjectDelete lod, string strHideType, string strAttrs)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, lot, lod, strHideType, strAttrs, da);
		//    }
		//}

		/// <summary>
		/// 获取指定对象中的所有用户对象
		/// </summary>
		/// <param name="xmlObjDoc">要求被查询的数据对象</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
		/// <param name="soco">要求所在机构的范围的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="lot">要求被查询的数据对象类型（主要是用于辨别是否要求查询兼职人员）</param>
		/// <param name="lod">系统中被逻辑删除对象是否查询取出</param>
		/// <param name="strAttrs">要求获得的数据属性</param>
		/// <returns>获取指定对象中的所有用户对象</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc的结构：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			string strOrgLimitValues,
			SearchObjectColumn soco,
			ListObjectType lot,
			ListObjectDelete lod,
			string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, lot, lod, string.Empty, strAttrs);
		}

		/// <summary>
		/// 获取指定对象中的所有用户对象
		/// </summary>
		/// <param name="xmlObjDoc">要求被查询的数据对象</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
		/// <param name="soco">要求所在机构的范围的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="lod">系统中被逻辑删除对象是否查询取出</param>
		/// <param name="strAttrs">要求获得的数据属性</param>
		/// <returns>获取指定对象中的所有用户对象</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc的结构：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			string strOrgLimitValues,
			SearchObjectColumn soco,
			ListObjectDelete lod,
			string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, ListObjectType.ALL_TYPE, lod, strAttrs);
		}

		/// <summary>
		/// 获取指定对象中的所有用户对象
		/// </summary>
		/// <param name="xmlObjDoc">要求被查询的数据对象</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
		/// <param name="soco">要求所在机构的范围的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strAttrs">要求获得的数据属性</param>
		/// <returns>获取指定对象中的所有用户对象</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc的结构：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			string strOrgLimitValues,
			SearchObjectColumn soco,
			string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, ListObjectDelete.COMMON, strAttrs);
		}

		/// <summary>
		/// 获取指定对象中的所有用户对象
		/// </summary>
		/// <param name="xmlObjDoc">要求被查询的数据对象</param>
		/// <param name="strOrgLimitValues">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
		/// <param name="soco">要求所在机构的范围的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strAttrs">要求获得的数据属性</param>
		/// <returns>获取指定对象中的所有用户对象</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc的结构：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strOrgLimitValues, SearchObjectColumn soco, string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, SearchObjectColumn.SEARCH_GUID, strOrgLimitValues, soco, strAttrs);
		}

		/// <summary>
		/// 获取指定对象中的所有用户对象
		/// </summary>
		/// <param name="xmlObjDoc">要求被查询的数据对象</param>
		/// <param name="strOrgLimitGuids">要求对象所在机构的范围内（如果没有将采用系统配置数据，可空）</param>
		/// <param name="strAttrs">要求获得的数据属性</param>
		/// <returns>获取指定对象中的所有用户对象</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc的结构：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strOrgLimitGuids, string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, strOrgLimitGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// 获取指定对象中的所有用户对象
		/// </summary>
		/// <param name="xmlObjDoc">要求被查询的数据对象</param>
		/// <param name="strAttrs">要求获得的数据属性</param>
		/// <returns>获取指定对象中的所有用户对象</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc的结构：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, string.Empty, strAttrs);
		}

		/// <summary>
		/// 获取指定对象中的所有用户对象
		/// </summary>
		/// <param name="xmlObjDoc">要求被查询的数据对象</param>
		/// <returns>获取指定对象中的所有用户对象</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc的结构：
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, string.Empty);
		}

		#endregion

		#region GetObjectsDetail
		///// <summary>
		///// 获取指定对象的详细属性数据
		///// </summary>
		///// <param name="strObjType">要求查询对象的类型(可以为空，空则采用混合查询)</param>
		///// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
		///// <param name="socu">查询要求的查询列名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		///// <param name="strParentValues">对象所在的机构标识（在对象为人员对象的时候有效，一般都为空）</param>
		///// <param name="soco">查询要求的查询列名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		///// <param name="da">数据库操作对象</param>
		///// <returns>针对性的查询结果返回</returns>
		//public static DataSet GetObjectsDetail(string strObjType, 
		//    string strObjValues, 
		//    SearchObjectColumn socu, 
		//    string strParentValues, 
		//    SearchObjectColumn soco)
		//{
		//    return GetObjectsDetail(strObjType, strObjValues, socu, strParentValues, soco, string.Empty, da);
		//}
		/// <summary>
		/// 获取指定对象的详细属性数据
		/// </summary>
		/// <param name="strObjType">要求查询对象的类型(可以为空，空则采用混合查询)</param>
		/// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
		/// <param name="socu">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strParentValues">对象所在的机构标识（在对象为人员对象的时候有效，一般都为空）</param>
		/// <param name="soco">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strExtAttrs">所需要的扩展属性（仅仅用于strObjType为空时）</param>
		/// <returns>针对性的查询结果返回</returns>
		public static DataSet GetObjectsDetail(string strObjType,
			string strObjValues,
			SearchObjectColumn socu,
			string strParentValues,
			SearchObjectColumn soco,
			string strExtAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strObjType, strObjValues, socu, strParentValues, soco, strExtAttrs);
			DataSet result;
			//if (false == GetObjectsDetailQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetObjectsDetailQueue))
			//    {
			if (false == GetObjectsDetailQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strColName = OGUCommonDefine.GetSearchObjectColumn(socu);
				string strSql = string.Empty;

				#region Data Prepare
				string columns = string.Empty;
				string objWhereClause = string.Empty;

				string objValues = OGUCommonDefine.AddMulitStrWithQuotationMark(strObjValues);
				string parentWhereClause = string.Empty;
				if (strParentValues.Length > 0 && soco != SearchObjectColumn.SEARCH_NULL)
				{
					parentWhereClause = " AND PARENT_GUID IN (SELECT GUID FROM ORGANIZATIONS WHERE "
						+ DatabaseSchema.Instence.GetTableColumns(OGUCommonDefine.GetSearchObjectColumn(soco), "ORGANIZATIONS")
						+ " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strParentValues) + "))";
				}
				string strAttrs = OGUCommonDefine.CombinateAttr(strExtAttrs);
				#endregion

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					switch (strObjType)
					{
						case "ORGANIZATIONS":
							#region ORGANIZATIONS
							columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE");
							objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "ORGANIZATIONS") + " IN (" + objValues + ")";
							strSql = @"
SELECT 'ORGANIZATIONS' AS OBJECTCLASS, *
FROM ORGANIZATIONS JOIN RANK_DEFINE 
	ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME 
WHERE {1}
	{2}
ORDER BY RANK_DEFINE.SORT_ID, ORGANIZATIONS.GLOBAL_SORT;";
							strSql = string.Format(strSql, columns, objWhereClause, parentWhereClause);
							break;
							#endregion
						case "GROUPS":
							#region GROUPS
							columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "GROUPS");
							objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "GROUPS") + " IN (" + objValues + ")";
							strSql = @"
SELECT 'GROUPS' AS OBJECTCLASS, *
FROM GROUPS
WHERE {1}
	{2}
ORDER BY GROUPS.GLOBAL_SORT;";
							strSql = string.Format(strSql, columns, objWhereClause, parentWhereClause);
							break;
							#endregion
						case "USERS":
							#region USERS
							columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "OU_USERS", "USERS", "RANK_DEFINE");
							objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "USERS", "OU_USERS") + " IN (" + objValues + ")";
							strSql = @"
SELECT 'USERS' AS OBJECTCLASS, OU_USERS.*, USERS.*, RANK_DEFINE.*
FROM OU_USERS, USERS JOIN RANK_DEFINE 
	ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
WHERE USERS.GUID = OU_USERS.USER_GUID
	AND {1}
	{2}
ORDER BY USERS.GUID, OU_USERS.SIDELINE, RANK_DEFINE.SORT_ID, OU_USERS.GLOBAL_SORT ; ";
							strSql = string.Format(strSql, columns, objWhereClause, parentWhereClause);
							break;
							#endregion
						case "":
							#region UNKNOW
							StringBuilder strTempSql = new StringBuilder(512);
							#region ORGANIZATIONS
							if (DatabaseSchema.Instence.CheckTableColumns(strColName, "ORGANIZATIONS"))
							{
								columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE");
								objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "ORGANIZATIONS") + " IN (" + objValues + ") ";
								strTempSql.Append(string.Format(@" 
SELECT 'ORGANIZATIONS' AS OBJECTCLASS, {0}
FROM ORGANIZATIONS JOIN RANK_DEFINE 
	ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME 
WHERE {1}
	{2}", columns, objWhereClause, parentWhereClause));
							}
							#endregion
							#region GROUPS
							if (DatabaseSchema.Instence.CheckTableColumns(strColName, "GROUPS"))
							{
								if (strTempSql.Length > 0)
									strTempSql.Append("\nUNION\n");

								columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "GROUPS");
								objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "GROUPS") + " IN (" + objValues + ") ";

								strTempSql.Append(string.Format(@" 
SELECT 'GROUPS' AS OBJECTCLASS, {0}
FROM GROUPS
WHERE {1}
	{2}", columns, objWhereClause, parentWhereClause));
							}
							#endregion
							#region USERS
							if (DatabaseSchema.Instence.GetTableColumns(strColName, "OU_USERS", "USERS").IndexOf("NULL AS") < 0)
							{
								if (strTempSql.Length > 0)
									strTempSql.Append("\nUNION\n");

								columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "OU_USERS", "USERS", "RANK_DEFINE");
								objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "OU_USERS", "USERS") + " IN (" + objValues + ") ";

								strTempSql.Append(string.Format(@" 
SELECT 'USERS' AS OBJECTCLASS, {0}
FROM OU_USERS, USERS JOIN RANK_DEFINE 
	ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
WHERE USERS.GUID = OU_USERS.USER_GUID
	AND {1}
	{2}", columns, objWhereClause, parentWhereClause));
							}
							#endregion
							strSql = @"
						SELECT * 
						FROM	(" + strTempSql.ToString() + @") RESULT
						ORDER BY GLOBAL_SORT ";
							break;
							#endregion
						default: ExceptionHelper.TrueThrow(true, "对不起，系统中没有处理该类型(" + strObjType + ")的处理程序！");
							break;
					}

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetObjectsDetailQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// 获取指定对象的详细属性数据
		///// </summary>
		///// <param name="strObjType">要求查询对象的类型(可以为空，涉及多类查询)</param>
		///// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
		///// <param name="soc">查询要求的查询列名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		///// </param>
		///// <param name="strParentValues">对象所在的机构标识（在对象为人员对象的时候有效，一般都为空）</param>
		///// <param name="soco">查询要求的查询列名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		///// <param name="strExtAttrs">所需要的扩展属性（仅仅用于strObjType为空时）</param>
		///// <returns>针对性的查询结果返回</returns>
		//public static DataSet GetObjectsDetail(string strObjType, string strObjValues, SearchObjectColumn soc, string strParentValues, SearchObjectColumn soco, string strExtAttrs)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetObjectsDetail(strObjType, strObjValues, soc, strParentValues, soco, strExtAttrs, da);
		//    }
		//}

		/// <summary>
		/// 获取指定对象的详细属性数据
		/// </summary>
		/// <param name="strObjType">要求查询对象的类型(可以为空，涉及多类查询)</param>
		/// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strParentValues">对象所在的机构标识（在对象为人员对象的时候有效，一般都为空）</param>
		/// <param name="soco">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <returns>针对性的查询结果返回</returns>
		public static DataSet GetObjectsDetail(string strObjType,
			string strObjValues,
			SearchObjectColumn soc,
			string strParentValues,
			SearchObjectColumn soco)
		{
			return GetObjectsDetail(strObjType, strObjValues, soc, strParentValues, soco, string.Empty);
		}

		/// <summary>
		/// 获取指定对象的详细属性数据
		/// </summary>
		/// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strParentValues">对象所在的机构标识（在对象为人员对象的时候有效，一般都为空）</param>
		/// <param name="soco">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <returns>针对性的查询结果返回</returns>
		public static DataSet GetObjectsDetail(string strObjValues,
			SearchObjectColumn soc,
			string strParentValues,
			SearchObjectColumn soco)
		{
			return GetObjectsDetail(string.Empty, strObjValues, soc, strParentValues, soco);
		}

		/// <summary>
		/// 获取指定对象的详细属性数据
		/// </summary>
		/// <param name="strObjType">要求查询对象的类型(可以为空，涉及多类查询)</param>
		/// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <returns>针对性的查询结果返回</returns>
		public static DataSet GetObjectsDetail(string strObjType, string strObjValues, SearchObjectColumn soc)
		{
			return GetObjectsDetail(strObjType, strObjValues, soc, string.Empty, SearchObjectColumn.SEARCH_NULL);
		}

		/// <summary>
		/// 获取指定对象的详细属性数据
		/// </summary>
		/// <param name="strObjValues">要求查询对象数据的标识(多个之间用","分隔开)</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <returns>针对性的查询结果返回</returns>
		public static DataSet GetObjectsDetail(string strObjValues, SearchObjectColumn soc)
		{
			return GetObjectsDetail(string.Empty, strObjValues, soc);
		}

		/// <summary>
		/// 获取指定对象的详细属性数据
		/// </summary>
		/// <param name="strObjType">要求查询对象的类型(可以为空，涉及多类查询)</param>
		/// <param name="strObjGuids">要求查询对象数据的标识GUID(多个GUID之间用","分隔开)</param>
		/// <returns>针对性的查询结果返回</returns>
		public static DataSet GetObjectsDetail(string strObjType, string strObjGuids)
		{
			return GetObjectsDetail(strObjType, strObjGuids, SearchObjectColumn.SEARCH_GUID);
		}

		/// <summary>
		/// 获取指定对象的详细属性数据
		/// </summary>
		/// <param name="strObjGuids">要求查询对象数据的标识GUID(多个GUID之间用","分隔开)</param>
		/// <returns>针对性的查询结果返回</returns>
		public static DataSet GetObjectsDetail(string strObjGuids)
		{
			return GetObjectsDetail(string.Empty, strObjGuids);
		}

		#endregion

		#region GetRankDefine
		/// <summary>
		/// 获取行政级别定义的所有数据
		/// </summary>
		/// <param name="iObjType">查询行政级别信息上的类别(1、机构级别；2、人员级别)</param>
		/// <param name="iShowHidden">是否展现系统中的隐藏个人级别信息（有些级别信息是不能做展现的，默认情况下为0）</param>
		/// <returns>获取行政级别定义的所有数据</returns>
		public static DataSet GetRankDefine(int iObjType, int iShowHidden)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(iObjType, iShowHidden);
			DataSet result;
			//if (false == GetRankDefineQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetRankDefineQueue))
			//    {
			if (false == GetRankDefineQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strShowHidden = " AND VISIBLE > 0 ";
				if (iShowHidden > 0)
					strShowHidden = string.Empty;
				string strSql = "SELECT CODE_NAME, NAME, SORT_ID FROM RANK_DEFINE WHERE RANK_CLASS = {0} {1} ORDER BY SORT_ID";
				strSql = string.Format(strSql, iObjType.ToString(), strShowHidden);

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetRankDefineQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// 获取行政级别定义的所有数据
		///// </summary>
		///// <param name="iObjType">查询行政级别信息上的类别(1、机构级别；2、人员级别)</param>
		///// <param name="iShowHidden">是否展现系统中的隐藏个人级别信息（有些级别信息是不能做展现的，默认情况下为0）</param>
		///// <returns>获取行政级别定义的所有数据</returns>
		//public static DataSet GetRankDefine(int iObjType, int iShowHidden)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetRankDefine(iObjType, iShowHidden, da);
		//    }
		//}

		/// <summary>
		/// 获取行政级别定义的所有数据
		/// </summary>
		/// <param name="iObjType">查询行政级别信息上的类别(1、机构级别；2、人员级别)</param>
		/// <returns>获取行政级别定义的所有数据</returns>
		public static DataSet GetRankDefine(int iObjType)
		{
			return GetRankDefine(iObjType, 0);
		}

		#endregion

		#region QueryOGUByCondition
		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="bLike">是否采用模糊匹配</param>
		/// <param name="bFirstPerson">要求一把手与否</param>
		/// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
		/// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
		/// <param name="strAttr">要求获取的字段</param>
		/// <param name="iListObjType">要求查询的对象类型</param>
		/// <param name="iDep">查询深度</param>
		/// <param name="strHideType">要求屏蔽的类型设置</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		/// 
		//2009-05-11
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			bool bFirstPerson,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType,
			int iDep,
			string strHideType)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strOrgValues,
				soc,
				strLikeName,
				bLike,
				bFirstPerson,
				strOrgRankCodeName,
				strUserRankCodeName,
				strAttr,
				iListObjType,
				iDep,
				strHideType);
			DataSet result;

			//得到All_Path_Name
			DataSet dsRootOrg = OGUReader.GetObjectsDetail("ORGANIZATIONS",
						strOrgValues,
						SearchObjectColumn.SEARCH_GUID,
						string.Empty,
						SearchObjectColumn.SEARCH_GUID);

			ExceptionHelper.FalseThrow(dsRootOrg.Tables[0].Rows.Count > 0, "不能找到ID为{0}的机构", strOrgValues);
			string rootPath = dsRootOrg.Tables[0].Rows[0]["ORIGINAL_SORT"].ToString();

			if (false == QueryOGUByConditionQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					if (strOrgValues.Length == 0)
					{
						strOrgValues = OGUCommonDefine.DBValueToString(OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"]);
						soc = SearchObjectColumn.SEARCH_GUID;
					}

					strAttr = OGUCommonDefine.CombinateAttr(strAttr);

					strHideType = OGUCommonDefine.GetHideType(strHideType);

					strLikeName = strLikeName.Replace("*", "%");

					StringBuilder strB = new StringBuilder(1024);
					if ((iListObjType & (int)ListObjectType.ORGANIZATIONS) != 0)
						strB.Append(" ( " + QueryOrganizationsByCondition(strOrgValues, soc, strLikeName, bLike, strOrgRankCodeName, strAttr, iDep, strHideType, rootPath) + " \n )");

					if ((iListObjType & (int)ListObjectType.GROUPS) != 0)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");
						strB.Append(" ( " + QueryGroupsByCondition(strOrgValues, soc, strLikeName, bLike, strAttr, iDep, strHideType, rootPath) + " \n )");
					}

					if ((iListObjType & (int)ListObjectType.USERS) != 0)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");

						strB.Append(" ( " + QueryUsersByCondition(strOrgValues, soc, strLikeName, bLike, bFirstPerson, strUserRankCodeName, strAttr, iListObjType, iDep, strHideType, rootPath) + " \n )");
					}

					string strSql = "SELECT * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				QueryOGUByConditionQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}


		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="bLike">是否采用模糊匹配</param>      
		/// <param name="strAttr">要求获取的字段</param>
		/// <param name="iListObjType">要求查询的对象类型</param>
		/// <param name="iLod">查询删除的对象策略</param>
		/// <param name="iDep">查询深度</param>
		/// <param name="strHideType">要求屏蔽的类型设置</param>
		/// <param name="rtnRowLimit">返回行数限制</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		/// 
		//2009-05-06 删除 RANK_DEFINE 约束，调整ORIGINAL_SORT约束，增加返回行数限制:-1(全部)
		public static DataSet QueryOGUByCondition2(string strOrgValues,
								SearchObjectColumn soc,
								string strLikeName,
								bool bLike,
								string strAttr,
								int iListObjType,
								ListObjectDelete iLod,
								int iDep,
								string strHideType,
								int rtnRowLimit)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strOrgValues,
				soc,
				strLikeName,
				bLike,
				strAttr,
				iListObjType,
				iDep,
				strHideType,
				rtnRowLimit);

			DataSet result;

			//得到All_Path_Name
			DataSet dsRootOrg = OGUReader.GetObjectsDetail("ORGANIZATIONS",
						strOrgValues,
						SearchObjectColumn.SEARCH_GUID,
						string.Empty,
						SearchObjectColumn.SEARCH_GUID);

			ExceptionHelper.FalseThrow(dsRootOrg.Tables[0].Rows.Count > 0, "不能找到ID为{0}的机构", strOrgValues);
			string rootPath = dsRootOrg.Tables[0].Rows[0]["ORIGINAL_SORT"].ToString();

			if (false == QueryOGUByConditionQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					if (strOrgValues.Length == 0)
					{
						strOrgValues = OGUCommonDefine.DBValueToString(OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"]);
						soc = SearchObjectColumn.SEARCH_GUID;
					}

					strAttr = OGUCommonDefine.CombinateAttr(strAttr);

					strHideType = OGUCommonDefine.GetHideType(strHideType);

					strLikeName = strLikeName.Replace("*", "%");

					StringBuilder strB = new StringBuilder(1024);
					if ((iListObjType & (int)ListObjectType.ORGANIZATIONS) != 0)
						strB.Append(" ( " + QueryOrganizationsByCondition2(strOrgValues, soc, strLikeName, bLike, strAttr, iLod, iDep, strHideType, rootPath) + " \n )");

					if ((iListObjType & (int)ListObjectType.GROUPS) != 0)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");
						strB.Append(" ( " + QueryGroupsByCondition2(strOrgValues, soc, strLikeName, bLike, strAttr, iLod, iDep, strHideType, rootPath) + " \n )");
					}

					if ((iListObjType & (int)ListObjectType.USERS) != 0)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");

						strB.Append(" ( " + QueryUsersByCondition2(strOrgValues, soc, strLikeName, bLike, strAttr, iListObjType, iLod, iDep, strHideType, rootPath) + " \n )");
					}

					string strSql = string.Empty;
					if (rtnRowLimit >= 0)
					{
						strSql = "SELECT top " + rtnRowLimit + " * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";
					}
					else
					{
						strSql = "SELECT * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";
					}

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				QueryOGUByConditionQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}

			return result;
		}

		///// <summary>
		///// 按照不同的要求查询系统中的所有符合条件的数据
		///// </summary>
		///// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		///// <param name="soc">查询要求的查询列名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		///// </param>
		///// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		///// <param name="bLike">是否采用模糊匹配</param>
		///// <param name="bFirstPerson">要求一把手与否</param>
		///// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
		///// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
		///// <param name="strAttr">要求获取的字段</param>
		///// <param name="iListObjType">要求查询的对象类型</param>
		///// <param name="iDep">查询深度</param>
		///// <param name="strHideType">要求屏蔽的类型设置</param>
		///// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		//public static DataSet QueryOGUByCondition(string strOrgValues, SearchObjectColumn soc, string strLikeName, bool bLike, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType, int iDep, string strHideType)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return QueryOGUByCondition(strOrgValues, soc, strLikeName, bLike, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep, strHideType, da);
		//    }
		//}

		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="bFirstPerson">要求一把手与否</param>
		/// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
		/// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
		/// <param name="strAttr">要求获取的字段</param>
		/// <param name="iListObjType">要求查询的对象类型</param>
		/// <param name="iDep">查询深度</param>
		/// <param name="strHideType">要求屏蔽的类型设置</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bFirstPerson,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType,
			int iDep,
			string strHideType)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, true, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep, strHideType);
		}
		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="bFirstPerson">要求一把手与否</param>
		/// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
		/// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
		/// <param name="strAttr">要求获取的字段</param>
		/// <param name="iListObjType">要求查询的对象类型</param>
		/// <param name="iDep">查询深度</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bFirstPerson,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType, int iDep)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep, string.Empty);
		}

		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="bFirstPerson">要求一把手与否</param>
		/// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
		/// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
		/// <param name="strAttr">要求获取的字段</param>
		/// <param name="iListObjType">要求查询的对象类型</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bFirstPerson,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, 0);
		}

		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
		/// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
		/// <param name="strAttr">要求获取的字段</param>
		/// <param name="iListObjType">要求查询的对象类型</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, false, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType);
		}

		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="strOrgRankCodeName">要求查询的机构上的行政级别设置</param>
		/// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
		/// <param name="strAttr">要求获取的字段</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, strOrgRankCodeName, strUserRankCodeName, strAttr, (int)(ListObjectType.ORGANIZATIONS | ListObjectType.USERS));
		}

		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="strUserRankCodeName">要求查询上的用户的行政级别设置</param>
		/// <param name="strAttr">要求获取的字段</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			string strUserRankCodeName,
			string strAttr)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, string.Empty, strUserRankCodeName, strAttr);
		}

		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <param name="strAttr">要求获取的字段</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues, SearchObjectColumn soc, string strLikeName, string strAttr)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, string.Empty, strAttr);
		}

		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgValues">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="soc">查询要求的查询列名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues, SearchObjectColumn soc, string strLikeName)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, string.Empty);
		}

		/// <summary>
		/// 按照不同的要求查询系统中的所有符合条件的数据
		/// </summary>
		/// <param name="strOrgGuids">指定父机构（多个之间采用","分隔,空就采用默认）</param>
		/// <param name="strLikeName">名称中的（模糊匹配对象）</param>
		/// <returns>按照不同的要求查询系统中的所有符合条件的数据</returns>
		public static DataSet QueryOGUByCondition(string strOrgGuids, string strLikeName)
		{
			return QueryOGUByCondition(strOrgGuids, SearchObjectColumn.SEARCH_GUID, strLikeName);
		}

		#endregion

		#region GetUsersInGroups
		/// <summary>
		/// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
		/// </summary>
		/// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
		/// <param name="socg">被查询对象所要求对应的数据类型（数据表字段名称）</param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <param name="strOrgValues">要求查询数据的机构范围(多个之间采用","分隔， 空的时候表示无机构要求)</param>
		/// <param name="soco">被查询对象所要求对应的数据类型（数据表字段名称）</param>
		/// <param name="strUserRankCodeName">对人员要求的最低行政级别</param>
		/// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
		/// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco,
			string strUserRankCodeName,
			int iLod)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strGroupValues, socg, strAttrs, strOrgValues, soco, strUserRankCodeName, iLod);
			DataSet result;
			//if (false == GetUsersInGroupsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUsersInGroupsQueue))
			//    {
			if (false == GetUsersInGroupsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
					#region SQL Prepare
					string strSql = @"
				SELECT 'USERS' AS OBJECTCLASS, GROUP_USERS.GROUP_GUID, "
						+ DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
				FROM ORGANIZATIONS, GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE 
					ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
					{2}
				WHERE " + DatabaseSchema.Instence.GetTableColumns(OGUCommonDefine.GetSearchObjectColumn(socg), "GROUPS") + @" IN ({0})
					AND GROUP_USERS.GROUP_GUID = GROUPS.GUID
					AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
					AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
					AND ({1})
					AND USERS.GUID = GROUP_USERS.USER_GUID
					AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
					{3}
				ORDER BY GROUPS.GLOBAL_SORT, GROUP_USERS.INNER_SORT";

					string strOrgLimit = string.Empty;
					if (strOrgValues.Length > 0)
						strOrgLimit = " AND " + DatabaseSchema.Instence.GetTableColumns(OGUCommonDefine.GetSearchObjectColumn(soco), "ORGANIZATIONS")
							+ " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgValues) + ") ";
					else
						strOrgLimit = " AND ORGANIZATIONS.GUID = "
							+ TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"]), true);

					string strRankLimit = string.Empty;
					if (strUserRankCodeName.Length > 0)
						strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
							+ TSqlBuilder.Instance.CheckQuotationMark(strUserRankCodeName, true) + ") ";


					strSql = string.Format(strSql,
						OGUCommonDefine.AddMulitStrWithQuotationMark(strGroupValues),
						GetSqlSearchStatus("OU_USERS", (ListObjectDelete)iLod),
						strRankLimit,
						strOrgLimit);
					#endregion
					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetUsersInGroupsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
		///// </summary>
		///// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
		///// <param name="socg">被查询对象所要求对应的数据类型（数据表字段名称）</param>
		///// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		///// <param name="strOrgValues">要求查询数据的机构范围(多个之间采用","分隔， 空的时候表示无机构要求)</param>
		///// <param name="soco">被查询对象所要求对应的数据类型（数据表字段名称）</param>
		///// <param name="strUserRankCodeName">对人员要求的最低行政级别</param>
		///// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
		///// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
		//public static DataSet GetUsersInGroups(string strGroupValues, SearchObjectColumn socg, string strAttrs, string strOrgValues, SearchObjectColumn soco, string strUserRankCodeName, int iLod)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco, strUserRankCodeName, iLod, da);
		//    }
		//}

		/// <summary>
		/// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
		/// </summary>
		/// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
		/// <param name="socg">被查询对象所要求对应的数据类型（数据表字段名称）</param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <param name="strOrgValues">要求查询数据的机构范围(多个之间采用","分隔， 空的时候表示无机构要求)</param>
		/// <param name="soco">被查询对象所要求对应的数据类型（数据表字段名称）</param>
		/// <param name="strUserRankCodeName">对人员要求的最低行政级别</param>
		/// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco,
			string strUserRankCodeName)
		{
			return GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco, strUserRankCodeName, (int)ListObjectDelete.COMMON);
		}
		/// <summary>
		/// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
		/// </summary>
		/// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
		/// <param name="socg">被查询对象所要求对应的数据类型（数据表字段名称）</param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <param name="strOrgValues">要求查询数据的机构范围(多个之间采用","分隔， 空的时候表示无机构要求)</param>
		/// <param name="soco">被查询对象所要求对应的数据类型（数据表字段名称）</param>
		/// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco)
		{
			return GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco, string.Empty);
		}
		/// <summary>
		/// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
		/// </summary>
		/// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
		/// <param name="socg">被查询对象所要求对应的数据类型（数据表字段名称）</param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetUsersInGroups(string strGroupValues, SearchObjectColumn socg, string strAttrs)
		{
			return GetUsersInGroups(strGroupValues, socg, strAttrs, string.Empty, SearchObjectColumn.SEARCH_NULL);
		}
		/// <summary>
		/// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
		/// </summary>
		/// <param name="strGroupValues">要求查询的人员组对象标识（多个之间采用","分隔）</param>
		/// <param name="socg">被查询对象所要求对应的数据类型（数据表字段名称）</param>
		/// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetUsersInGroups(string strGroupValues, SearchObjectColumn socg)
		{
			return GetUsersInGroups(strGroupValues, socg, string.Empty);
		}
		/// <summary>
		/// 获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）
		/// </summary>
		/// <param name="strGroupGuids">要求查询的人员组对象标识GUID（多个GUID之间采用","分隔）</param>
		/// <returns>获取指定人员组中的所有成员（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetUsersInGroups(string strGroupGuids)
		{
			return GetUsersInGroups(strGroupGuids, SearchObjectColumn.SEARCH_GUID);
		}

		#endregion

		#region GetUsersInGroups [Add By Yuanyong @ 2005-04-20]
		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strNameLike"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="strAttrs"></param>
		/// <param name="strOrgValues"></param>
		/// <param name="soco"></param>
		/// <param name="strUserRankCodeName"></param>
		/// <param name="lod"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strNameLike,
			string strSortColumn,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco,
			string strUserRankCodeName,
			ListObjectDelete lod,
			int iPageNo,
			int iPageSize)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strGroupValues,
				socg,
				strNameLike,
				strSortColumn,
				strAttrs,
				strOrgValues,
				soco,
				strUserRankCodeName,
				lod,
				iPageNo,
				iPageSize);
			DataSet result;
			//if (false == GetUsersInGroupsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUsersInGroupsQueue))
			//    {
			if (false == GetUsersInGroupsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
					#region SQL Prepare
					string strSql = @"
				SELECT 'USERS' AS OBJECTCLASS, GROUP_USERS.GROUP_GUID, "
						+ DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
				FROM ORGANIZATIONS, GROUPS, GROUP_USERS, OU_USERS, USERS LEFT JOIN RANK_DEFINE 
					ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
					{2}
				WHERE " + DatabaseSchema.Instence.GetTableColumns(OGUCommonDefine.GetSearchObjectColumn(socg), "GROUPS") + @" IN ({0})
					AND GROUP_USERS.GROUP_GUID = GROUPS.GUID
					AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
					AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
					AND ({1})
					AND USERS.GUID = GROUP_USERS.USER_GUID
					AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
					{3}
					{4}
				ORDER BY GROUPS.GLOBAL_SORT, "
							+ TSqlBuilder.Instance.CheckQuotationMark(strSortColumn == string.Empty ? "GROUP_USERS.INNER_SORT" : strSortColumn, false);

					string strOrgLimit = string.Empty;
					if (strOrgValues.Length > 0)
						strOrgLimit = " AND " + DatabaseSchema.Instence.GetTableColumns(OGUCommonDefine.GetSearchObjectColumn(soco), "ORGANIZATIONS")
							+ " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgValues) + ") ";
					else
						strOrgLimit = " AND ORGANIZATIONS.GUID = "
							+ TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"]), true);

					string strRankLimit = string.Empty;
					if (strUserRankCodeName.Length > 0)
						strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
							+ TSqlBuilder.Instance.CheckQuotationMark(strUserRankCodeName, true) + ") ";

					string strNameLikeWhere = string.Empty;
					if (strNameLike != string.Empty)
					{
						strNameLike = strNameLike.Replace("*", "");
						strNameLikeWhere = " AND (USERS.LOGON_NAME LIKE '%' + " + TSqlBuilder.Instance.CheckQuotationMark(strNameLike, true) + @" + '%' 
						OR OU_USERS.OBJ_NAME LIKE '%' + " + TSqlBuilder.Instance.CheckQuotationMark(strNameLike, true) + @" + '%' 
						OR OU_USERS.DISPLAY_NAME LIKE '%' + " + TSqlBuilder.Instance.CheckQuotationMark(strNameLike, true) + @" + '%') ";
					}
					#endregion
					strSql = string.Format(strSql, OGUCommonDefine.AddMulitStrWithQuotationMark(strGroupValues), GetSqlSearchStatus("OU_USERS", lod), strRankLimit, strOrgLimit, strNameLikeWhere);
					//if (iPageNo >= 0 && iPageSize > 0)
					//    result = database.ExecuteDataSet(Com, strSql, "GROUPS", iPageNo, iPageSize);
					//else
					result = database.ExecuteDataSet(CommandType.Text, strSql, iPageNo, iPageSize, "USERS");
				}
				GetUsersInGroupsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="strGroupValues"></param>
		///// <param name="socg"></param>
		///// <param name="strNameLike"></param>
		///// <param name="strSortColumn"></param>
		///// <param name="strAttrs"></param>
		///// <param name="strOrgValues"></param>
		///// <param name="soco"></param>
		///// <param name="strUserRankCodeName"></param>
		///// <param name="lod"></param>
		///// <param name="iPageNo"></param>
		///// <param name="iPageSize"></param>
		///// <returns></returns>
		//public static DataSet GetUsersInGroups(string strGroupValues, SearchObjectColumn socg, string strNameLike, string strSortColumn, string strAttrs, string strOrgValues, SearchObjectColumn soco, string strUserRankCodeName, ListObjectDelete lod, int iPageNo, int iPageSize)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetUsersInGroups(strGroupValues, socg, strNameLike, strSortColumn, strAttrs, strOrgValues, soco, strUserRankCodeName, lod, iPageNo, iPageSize, da);
		//    }
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strNameLike"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="strAttrs"></param>
		/// <param name="strOrgValues"></param>
		/// <param name="soco"></param>
		/// <param name="strUserRankCodeName"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strNameLike,
			string strSortColumn,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco,
			string strUserRankCodeName,
			int iPageNo,
			int iPageSize)
		{
			return GetUsersInGroups(strGroupValues, socg, strNameLike, strSortColumn, strAttrs, strOrgValues, soco, strUserRankCodeName, ListObjectDelete.COMMON, iPageNo, iPageSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strNameLike"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="strAttrs"></param>
		/// <param name="strOrgValues"></param>
		/// <param name="soco"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strNameLike,
			string strSortColumn,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco,
			int iPageNo,
			int iPageSize)
		{
			return GetUsersInGroups(strGroupValues, socg, strNameLike, strSortColumn, strAttrs, strOrgValues, soco, string.Empty, iPageNo, iPageSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strNameLike"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="strAttrs"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strNameLike,
			string strSortColumn,
			string strAttrs,
			int iPageNo,
			int iPageSize)
		{
			return GetUsersInGroups(strGroupValues, socg, strNameLike, strSortColumn, strAttrs, string.Empty, SearchObjectColumn.SEARCH_NULL, iPageNo, iPageSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strNameLike"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strNameLike,
			string strSortColumn,
			int iPageNo,
			int iPageSize)
		{
			return GetUsersInGroups(strGroupValues, socg, strNameLike, strSortColumn, string.Empty, iPageNo, iPageSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strSortColumn,
			int iPageNo,
			int iPageSize)
		{
			return GetUsersInGroups(strGroupValues, socg, string.Empty, strSortColumn, string.Empty, iPageNo, iPageSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupGuids"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupGuids, int iPageNo, int iPageSize)
		{
			return GetUsersInGroups(strGroupGuids, SearchObjectColumn.SEARCH_GUID, string.Empty, iPageNo, iPageSize);
		}

		#endregion

		#region GetGroupsOfUsers
		/// <summary>
		/// 获取指定用户所从属的"人员组"集合
		/// </summary>
		/// <param name="strUserValues">指定的用户标识（多个之间采用“,”分隔）</param>
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strParentValue">指定的用户所在部门（用于区别兼职问题）</param>
		/// <param name="soco">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strAttrs">所要求获取的属性信息</param>
		/// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
		/// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetGroupsOfUsers(string strUserValues,
			SearchObjectColumn socu,
			string strParentValue,
			SearchObjectColumn soco,
			string strAttrs,
			int iLod)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strUserValues, socu, strParentValue, soco, strAttrs, iLod);
			DataSet result;
			//if (false == GetGroupsOfUsersQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetGroupsOfUsersQueue))
			//    {
			if (false == GetGroupsOfUsersQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strColName = OGUCommonDefine.GetSearchObjectColumn(socu);
				string strParentColName = OGUCommonDefine.GetSearchObjectColumn(soco);

				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					string strSql = @"
				SELECT 'GROUPS' AS OBJECTCLASS, GROUPS.GUID, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "GROUPS") + @"
				FROM GROUPS, GROUP_USERS, OU_USERS, USERS, ORGANIZATIONS
				WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
					AND ORGANIZATIONS.GUID = OU_USERS.PARENT_GUID
					AND OU_USERS.USER_GUID = USERS.GUID
					AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
					AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
					{0}
					AND {1} IN ({2})
					AND ({3})
				ORDER BY OU_USERS.GLOBAL_SORT, GROUPS.GLOBAL_SORT
				";

					string strParent = string.Empty;
					if (strParentValue.Length != 0)
						strParent = " AND " + DatabaseSchema.Instence.GetTableColumns(strParentColName, "ORGANIZATIONS")
							+ " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strParentValue) + ") ";

					strSql = string.Format(strSql, strParent,
						DatabaseSchema.Instence.GetTableColumns(strColName, "OU_USERS", "USERS"),
						OGUCommonDefine.AddMulitStrWithQuotationMark(strUserValues),
						GetSqlSearchStatus("GROUPS", (ListObjectDelete)iLod));

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetGroupsOfUsersQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// 获取指定用户所从属的"人员组"集合
		///// </summary>
		///// <param name="strUserValues">指定的用户标识（多个之间采用“,”分隔）</param>
		///// <param name="socu">用户的属性名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		///// </param>
		///// <param name="strParentValue">指定的用户所在部门（用于区别兼职问题）</param>
		///// <param name="soco">机构的属性名称
		///// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		///// </param>
		///// <param name="strAttrs">所要求获取的属性信息</param>
		///// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
		///// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
		//public static DataSet GetGroupsOfUsers(string strUserValues, SearchObjectColumn socu, string strParentValue, SearchObjectColumn soco, string strAttrs, int iLod)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetGroupsOfUsers(strUserValues, socu, strParentValue, soco, strAttrs, iLod, da);
		//    }
		//}

		/// <summary>
		/// 获取指定用户所从属的"人员组"集合
		/// </summary>
		/// <param name="strUserValues">指定的用户标识（多个之间采用“,”分隔）</param>
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strParentValue">指定的用户所在部门（用于区别兼职问题）</param>
		/// <param name="soco">机构的属性名称
		/// （ＧＵＩＤ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strAttrs">所要求获取的属性信息</param>
		/// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetGroupsOfUsers(string strUserValues,
			SearchObjectColumn socu,
			string strParentValue,
			SearchObjectColumn soco,
			string strAttrs)
		{
			return GetGroupsOfUsers(strUserValues, socu, strParentValue, soco, strAttrs, (int)ListObjectDelete.COMMON);
		}

		/// <summary>
		/// 获取指定用户所从属的"人员组"集合
		/// </summary>
		/// <param name="strUserValues">指定的用户标识（多个之间采用“,”分隔）</param>
		/// <param name="socu">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strAttrs">所要求获取的属性信息</param>
		/// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetGroupsOfUsers(string strUserValues, SearchObjectColumn socu, string strAttrs)
		{
			return GetGroupsOfUsers(strUserValues, socu, string.Empty, SearchObjectColumn.SEARCH_NULL, strAttrs);
		}

		/// <summary>
		/// 获取指定用户所从属的"人员组"集合
		/// </summary>
		/// <param name="strUserGuids">指定的用户标识GUID（多个GUID之间采用“,”分隔）</param>
		/// <param name="strAttrs">所要求获取的属性信息</param>
		/// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetGroupsOfUsers(string strUserGuids, string strAttrs)
		{
			return GetGroupsOfUsers(strUserGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// 获取指定用户所从属的"人员组"集合
		/// </summary>
		/// <param name="strUserGuids">指定的用户标识GUID（多个GUID之间采用“,”分隔）</param>
		/// <returns>获取指定人员的所有"人员组"集合（注意其中逻辑删除的数据对象）</returns>
		public static DataSet GetGroupsOfUsers(string strUserGuids)
		{
			return GetGroupsOfUsers(strUserGuids, string.Empty);
		}

		#endregion

		#region GetSecretariesOfLeaders
		/// <summary>
		/// 获取指定领导的所有秘书人成员
		/// </summary>
		/// <param name="strLeaderValues">指定领导的标识（多个之间采用","分隔）</param>
		/// <param name="soc">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
		/// <returns>获取指定领导的所有秘书人成员</returns>
		public static DataSet GetSecretariesOfLeaders(string strLeaderValues,
			SearchObjectColumn soc,
			string strAttrs,
			int iLod)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strLeaderValues, soc, strAttrs, iLod);
			DataSet result;
			//if (false == GetSecretariesOfLeadersQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetSecretariesOfLeadersQueue))
			//    {
			if (false == GetSecretariesOfLeadersQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					string strSql = @"
				SELECT 'USERS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE", "SECRETARIES") + @"
				FROM OU_USERS, SECRETARIES, USERS JOIN RANK_DEFINE 
					ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
				WHERE USERS.GUID = OU_USERS.USER_GUID
					AND OU_USERS.USER_GUID = SECRETARIES.SECRETARY_GUID
					AND OU_USERS.SIDELINE = 0
					AND SECRETARIES.LEADER_GUID IN	(
														SELECT USERS.GUID 
														FROM USERS, OU_USERS 
														WHERE USERS.GUID = OU_USERS.USER_GUID 
															AND {0} IN ({1})
													)
					AND ({2})
				ORDER BY SECRETARIES.LEADER_GUID, RANK_DEFINE.SORT_ID;
				";

					strSql = string.Format(strSql,
						DatabaseSchema.Instence.GetTableColumns(strColName, "OU_USERS", "USERS"),
						OGUCommonDefine.AddMulitStrWithQuotationMark(strLeaderValues),
						GetSqlSearchStatus("OU_USERS", (ListObjectDelete)iLod));

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetSecretariesOfLeadersQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// 获取指定领导的所有秘书人成员
		///// </summary>
		///// <param name="strLeaderValues">指定领导的标识（多个之间采用","分隔）</param>
		///// <param name="soc">用户的属性名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		///// </param>
		///// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		///// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
		///// <returns>获取指定领导的所有秘书人成员</returns>
		//public static DataSet GetSecretariesOfLeaders(string strLeaderValues, SearchObjectColumn soc, string strAttrs, int iLod)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetSecretariesOfLeaders(strLeaderValues, soc, strAttrs, iLod, da);
		//    }
		//}

		/// <summary>
		/// 获取指定领导的所有秘书人成员
		/// </summary>
		/// <param name="strLeaderValues">指定领导的标识（多个之间采用","分隔）</param>
		/// <param name="soc">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <returns>获取指定领导的所有秘书人成员</returns>
		public static DataSet GetSecretariesOfLeaders(string strLeaderValues, SearchObjectColumn soc, string strAttrs)
		{
			return GetSecretariesOfLeaders(strLeaderValues, soc, strAttrs, (int)ListObjectDelete.COMMON);
		}

		/// <summary>
		/// 获取指定领导的所有秘书人成员
		/// </summary>
		/// <param name="strLeaderGuids">指定领导的标识GUID（多个GUID之间采用","分隔）</param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <returns>获取指定领导的所有秘书人成员</returns>
		public static DataSet GetSecretariesOfLeaders(string strLeaderGuids, string strAttrs)
		{
			return GetSecretariesOfLeaders(strLeaderGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// 获取指定领导的所有秘书人成员
		/// </summary>
		/// <param name="strLeaderGuids">指定领导的标识GUID（多个GUID之间采用","分隔）</param>
		/// <returns>获取指定领导的所有秘书人成员</returns>
		public static DataSet GetSecretariesOfLeaders(string strLeaderGuids)
		{
			return GetSecretariesOfLeaders(strLeaderGuids, string.Empty);
		}

		#endregion

		#region GetLeadersOfSecretaries
		/// <summary>
		/// 获取指定秘书的所有领导人成员
		/// </summary>
		/// <param name="strSecValues">指定秘书的标识（多个之间采用","分隔）</param>
		/// <param name="soc">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
		/// <returns>获取指定秘书的所有领导人成员</returns>
		public static DataSet GetLeadersOfSecretaries(string strSecValues,
			SearchObjectColumn soc,
			string strAttrs,
			int iLod)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strSecValues, soc, strAttrs, iLod);
			DataSet result;
			//if (false == GetLeadersOfSecretariesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetLeadersOfSecretariesQueue))
			//    {
			if (false == GetLeadersOfSecretariesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);
				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					string strSql = @"
				SELECT 'USERS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE", "SECRETARIES") + @"
				FROM OU_USERS, SECRETARIES, USERS JOIN RANK_DEFINE 
					ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
				WHERE USERS.GUID = OU_USERS.USER_GUID
					AND OU_USERS.USER_GUID = SECRETARIES.LEADER_GUID
					AND OU_USERS.SIDELINE = 0
					AND SECRETARIES.SECRETARY_GUID IN	(
															SELECT USERS.GUID 
															FROM USERS, OU_USERS 
															WHERE USERS.GUID = OU_USERS.USER_GUID 
																AND {0} IN ({1})
														)
					AND ({2})
				ORDER BY SECRETARIES.SECRETARY_GUID, RANK_DEFINE.SORT_ID
				";

					strSql = string.Format(strSql,
						DatabaseSchema.Instence.GetTableColumns(strColName, "OU_USERS", "USERS"),
						OGUCommonDefine.AddMulitStrWithQuotationMark(strSecValues),
						GetSqlSearchStatus("OU_USERS", (ListObjectDelete)iLod));

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetLeadersOfSecretariesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// 获取指定秘书的所有领导人成员
		///// </summary>
		///// <param name="strSecValues">指定秘书的标识（多个之间采用","分隔）</param>
		///// <param name="soc">用户的属性名称
		///// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		///// </param>
		///// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		///// <param name="iLod">本次查询中要求查询对象的状态信息数据（是否包含逻辑删除对象）</param>
		///// <returns>获取指定秘书的所有领导人成员</returns>
		//public static DataSet GetLeadersOfSecretaries(string strSecValues, SearchObjectColumn soc, string strAttrs, int iLod)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetLeadersOfSecretaries(strSecValues, soc, strAttrs, iLod, da);
		//    }
		//}

		/// <summary>
		/// 获取指定秘书的所有领导人成员
		/// </summary>
		/// <param name="strSecValues">指定秘书的标识（多个之间采用","分隔）</param>
		/// <param name="soc">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <returns>获取指定秘书的所有领导人成员</returns>
		public static DataSet GetLeadersOfSecretaries(string strSecValues, SearchObjectColumn soc, string strAttrs)
		{
			return GetLeadersOfSecretaries(strSecValues, soc, strAttrs, (int)ListObjectDelete.COMMON);
		}

		/// <summary>
		/// 获取指定秘书的所有领导人成员
		/// </summary>
		/// <param name="strSecGuids">指定秘书的标识GUID（多个GUID之间采用","分隔）</param>
		/// <param name="strAttrs">要求在本次查询中获取对象的字段名称</param>
		/// <returns>获取指定秘书的所有领导人成员</returns>
		public static DataSet GetLeadersOfSecretaries(string strSecGuids, string strAttrs)
		{
			return GetLeadersOfSecretaries(strSecGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// 获取指定秘书的所有领导人成员
		/// </summary>
		/// <param name="strSecGuids">指定秘书的标识GUID（多个GUID之间采用","分隔）</param>
		/// <returns>获取指定秘书的所有领导人成员</returns>
		public static DataSet GetLeadersOfSecretaries(string strSecGuids)
		{
			return GetLeadersOfSecretaries(strSecGuids, string.Empty);
		}

		#endregion

		#region GetObjectParentOrgs
		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjValues">自身所具有的数据值</param>
		/// <param name="soc">自身所具有的数据类型（GUID、OBJ_NAME、ORIGINAL_SORT、GLOBAL_SORT等）</param>
		/// <param name="bOnlyDirectly">是否仅仅获取最接近的机构对象</param>
		/// <param name="bWithVisiual">是否要求忽略虚拟部门</param>
		/// <param name="strOrgRankCodeName">要求最低的机构行政级别</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>获取指定对象的父部门对象</returns>
		public static DataSet GetObjectParentOrgs(string strObjType,
			string strObjValues,
			SearchObjectColumn soc,
			bool bOnlyDirectly,
			bool bWithVisiual,
			string strOrgRankCodeName,
			string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strObjType, strObjValues, soc, bOnlyDirectly, bWithVisiual, strOrgRankCodeName, strAttrs);
			DataSet result;
			//if (false == GetObjectParentOrgsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetObjectParentOrgsQueue))
			//    {
			if (false == GetObjectParentOrgsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

				string strSql = string.Empty;

				string strWhere = string.Empty;
				string strWhereRankDefine = string.Empty;

				if (false == bWithVisiual)//计虚拟部门
					strWhere = " AND ORGANIZATIONS.ORG_TYPE <> 1 ";
				if (strOrgRankCodeName.Length > 0)
					strWhereRankDefine = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strOrgRankCodeName.Trim(), true) + ") ";

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					switch (strObjType)
					{
						case "ORGANIZATIONS":
						case "GROUPS":
							#region ORGANIZATIONS And GROUPS
							strSql = @"
						SELECT {0} 'ORGANIZATIONS' AS OBJECTCLASS, "
								+ DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE") + @"
						FROM ORGANIZATIONS JOIN RANK_DEFINE 
							ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME " + strWhereRankDefine + @"
						WHERE (SELECT ORIGINAL_SORT FROM {1} WHERE {2}) LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
							" + strWhere + @" 
						ORDER BY ORGANIZATIONS.ORIGINAL_SORT DESC";

							string strOnlyDirectly = string.Empty;
							if (bOnlyDirectly)
								strOnlyDirectly = " TOP 2 "; //strOnlyDirectly = " TOP 1 ";//Modify By Yuanyong 20080723如果为1的话就只能返回自身这个对象了

							string strOrgSelf = DatabaseSchema.Instence.GetTableColumns(strColName, strObjType)
								+ " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strObjValues) + ") ";

							strSql = string.Format(strSql, strOnlyDirectly, TSqlBuilder.Instance.CheckQuotationMark(strObjType, false), strOrgSelf);
							break;
							#endregion
						case "USERS":
							#region USERS
							strWhere += " AND " + DatabaseSchema.Instence.GetTableColumns(strColName, strObjType, "OU_USERS") + " IN ("
								+ OGUCommonDefine.AddMulitStrWithQuotationMark(strObjValues) + ") ";

							if (bOnlyDirectly)
							{
								strSql = @"
							SELECT 'ORGANIZATIONS' AS OBJECTCLASS, S.*
							FROM (
									SELECT OU_USERS.GLOBAL_SORT AS USERS_GLOBAL_SORT, MAX(LEN(ORGANIZATIONS.ORIGINAL_SORT)) AS MAX_LEN
									FROM USERS, OU_USERS, ORGANIZATIONS JOIN RANK_DEFINE 
										ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
										{2}
									WHERE 	USERS.GUID = OU_USERS.USER_GUID
										AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
										{0}
									GROUP BY OU_USERS.GLOBAL_SORT
								) G, 
								(	
									SELECT OU_USERS.GLOBAL_SORT AS USERS_GLOBAL_SORT, {1}
									FROM USERS, OU_USERS, ORGANIZATIONS JOIN RANK_DEFINE 
										ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
										{2}
									WHERE 	USERS.GUID = OU_USERS.USER_GUID
										AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
										{0}
								) S
							WHERE G.USERS_GLOBAL_SORT = S.USERS_GLOBAL_SORT
								AND G.MAX_LEN = LEN(S.ORIGINAL_SORT)
							ORDER BY S.USERS_GLOBAL_SORT DESC
							";
							}
							else
							{
								strSql = @"
							SELECT 'ORGANIZATIONS' AS OBJECTCLASS, {1}
							FROM USERS, OU_USERS, ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
								{2}
							WHERE USERS.GUID = OU_USERS.USER_GUID
								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
								{0}
							ORDER BY OU_USERS.ORIGINAL_SORT, ORGANIZATIONS.ORIGINAL_SORT DESC
							";
							}

							strSql = string.Format(strSql, strWhere,
								DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE", "OU_USERS", "USERS"),
								strWhereRankDefine);
							break;
							#endregion
					}

					Database database = DatabaseFactory.Create(CommonResource.AccreditConnAlias);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetObjectParentOrgsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
		///// </summary>
		///// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		///// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
		///// <param name="soc">自身所具有的数据类型（GUID、OBJ_NAME、ORIGINAL_SORT、GLOBAL_SORT等）</param>
		///// <param name="bOnlyDirectly">是否仅仅获取最接近的机构对象</param>
		///// <param name="bWithVisiual">是否要求忽略虚拟部门</param>
		///// <param name="strOrgRankCodeName">要求最低的机构行政级别</param>
		///// <param name="strAttrs">要求获取的数据字段</param>
		///// <returns>获取指定对象的父部门对象</returns>
		//public static DataSet GetObjectParentOrgs(string strObjType, string strObjValues, SearchObjectColumn soc, bool bOnlyDirectly, bool bWithVisiual, string strOrgRankCodeName, string strAttrs)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetObjectParentOrgs(strObjType, strObjValues, soc, bOnlyDirectly, bWithVisiual, strOrgRankCodeName, strAttrs, da);
		//    }
		//}

		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
		/// <param name="soc">自身所具有的数据类型（GUID、OBJ_NAME、ORIGINAL_SORT、GLOBAL_SORT等）</param>
		/// <param name="bWithVisiual">是否要求忽略虚拟部门</param>
		/// <param name="strOrgRankCodeName">要求最低的机构行政级别</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>获取指定对象的父部门对象</returns>
		public static DataSet GetObjectParentOrgs(string strObjType,
			string strObjValues,
			SearchObjectColumn soc,
			bool bWithVisiual,
			string strOrgRankCodeName,
			string strAttrs)
		{
			return GetObjectParentOrgs(strObjType, strObjValues, soc, false, bWithVisiual, strOrgRankCodeName, strAttrs);
		}

		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
		/// <param name="soc">自身所具有的数据类型（GUID、OBJ_NAME、ORIGINAL_SORT、GLOBAL_SORT等）</param>
		/// <param name="strOrgRankCodeName">要求最低的机构行政级别</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>获取指定对象的父部门对象</returns>
		public static DataSet GetObjectParentOrgs(string strObjType,
			string strObjValues,
			SearchObjectColumn soc,
			string strOrgRankCodeName,
			string strAttrs)
		{
			return GetObjectParentOrgs(strObjType, strObjValues, soc, false, strOrgRankCodeName, strAttrs);
		}

		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
		/// <param name="soc">自身所具有的数据类型（GUID、OBJ_NAME、ORIGINAL_SORT、GLOBAL_SORT等）</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>获取指定对象的父部门对象</returns>
		public static DataSet GetObjectParentOrgs(string strObjType, string strObjValues, SearchObjectColumn soc, string strAttrs)
		{
			return GetObjectParentOrgs(strObjType, strObjValues, soc, string.Empty, strAttrs);
		}

		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjGuids">自身所具有的数据值GUID(多个GUID之间采用","分隔)</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>获取指定对象的父部门对象</returns>
		public static DataSet GetObjectParentOrgs(string strObjType, string strObjGuids, string strAttrs)
		{
			return GetObjectParentOrgs(strObjType, strObjGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjGuids">自身所具有的数据值GUID(多个GUID之间采用","分隔)</param>
		/// <returns>获取指定对象的父部门对象</returns>
		public static DataSet GetObjectParentOrgs(string strObjType, string strObjGuids)
		{
			return GetObjectParentOrgs(strObjType, strObjGuids, string.Empty);
		}

		#endregion

		#region GetObjectDepOrgs
		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门(指定层次)）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjValues">自身所具有的数据值</param>
		/// <param name="soc">自身所具有的数据类型（GUID、OBJ_NAME、ORIGINAL_SORT、GLOBAL_SORT等）</param>
		/// <param name="iDep">要求获取的深度</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>获取指定对象的父部门对象</returns>
		public static DataSet GetObjectDepOrgs(string strObjType,
			string strObjValues,
			SearchObjectColumn soc,
			int iDep,
			string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strObjType, strObjValues, soc, iDep, strAttrs);
			DataSet result;
			//if (false == GetObjectDepOrgsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetObjectDepOrgsQueue))
			//    {
			if (false == GetObjectDepOrgsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				ExceptionHelper.TrueThrow(iDep <= 0, "参数“iDep”的值必须大于0！");

				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				int iLength = iDep * CommonResource.OriginalSortDefault.Length;// OGUCommonDefine.OGU_ORIGINAL_SORT.Length;

				string strSql = string.Empty;
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					switch (strObjType)
					{
						case "ORGANIZATIONS":
						case "GROUPS":
							#region ORGANIZATIONS And GROUPS
							strSql = @"
						SELECT 'ORGANIZATIONS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE") + @" 
						FROM	(	
									SELECT ORIGINAL_SORT 
									FROM " + strObjType + @" 
									WHERE " + strColName + " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strObjValues) + @") 
								) ROS,
							ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME 
						WHERE LEN(ORGANIZATIONS.ORIGINAL_SORT) = " + iLength.ToString() + @" 
							AND ROS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%' 
						ORDER BY RANK_DEFINE.SORT_ID, ORGANIZATIONS.GLOBAL_SORT ";
							break;
							#endregion
						case "USERS":
							#region USERS
							strSql = @"
						SELECT 'ORGANIZATIONS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE") + @" 
						FROM	(	
									SELECT OU_USERS.ORIGINAL_SORT 
									FROM USERS, OU_USERS 
									WHERE USERS.GUID = OU_USERS.USER_GUID 
										AND " + DatabaseSchema.Instence.GetTableColumns(strColName, "USERS", "OU_USERS")
													  + " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strObjValues) + @")
								)  ROS,
							ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME 
						WHERE LEN(ORGANIZATIONS.ORIGINAL_SORT) = " + iLength.ToString() + @" 
							AND ROS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
						ORDER BY RANK_DEFINE.SORT_ID, ORGANIZATIONS.GLOBAL_SORT";
							break;
							#endregion
						default:
							ExceptionHelper.TrueThrow(true, "（" + strObjType + "）未知的对象数据类型！");
							break;
					}
					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetObjectDepOrgsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门(指定层次)）
		///// </summary>
		///// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		///// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
		///// <param name="soc">自身所具有的数据类型（GUID、OBJ_NAME、ORIGINAL_SORT、GLOBAL_SORT等）</param>
		///// <param name="iDep">要求获取的深度</param>
		///// <param name="strAttrs">要求获取的数据字段</param>
		///// <returns>获取指定对象的父部门对象</returns>
		//public static DataSet GetObjectDepOrgs(string strObjType, string strObjValues, SearchObjectColumn soc, int iDep, string strAttrs)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetObjectDepOrgs(strObjType, strObjValues, soc, iDep, strAttrs, da);
		//    }
		//}

		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门(指定层次)）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjValues">自身所具有的数据值(多个之间采用","分隔)</param>
		/// <param name="soc">自身所具有的数据类型（GUID、OBJ_NAME、ORIGINAL_SORT、GLOBAL_SORT等）</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>获取指定对象的父部门对象</returns>
		public static DataSet GetObjectDepOrgs(string strObjType, string strObjValues, SearchObjectColumn soc, string strAttrs)
		{
			return GetObjectDepOrgs(strObjType, strObjValues, soc, 1, strAttrs);
		}

		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门(指定层次)）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjGuids">自身所具有的数据值GUID(多个GUID之间采用","分隔)</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>获取指定对象的父部门对象</returns>
		public static DataSet GetObjectDepOrgs(string strObjType, string strObjGuids, string strAttrs)
		{
			return GetObjectDepOrgs(strObjType, strObjGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// 获取指定对象的父机构对象（GROUPS、USERS或者是ORGANIZATIONS的父部门(指定层次)）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjGuids">自身所具有的数据值GUID(多个GUID之间采用","分隔)</param>
		/// <returns>获取指定对象的父部门对象</returns>
		public static DataSet GetObjectDepOrgs(string strObjType, string strObjGuids)
		{
			return GetObjectDepOrgs(strObjType, strObjGuids, string.Empty);
		}

		#endregion

		#region GetObjectsSort
		/// <summary>
		/// 对于指定对象在系统中重新排序以后返回
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被排序对象</param>
		/// <param name="soc">xmlDoc中适用于标识机构人员系统对象的属性类别</param>
		/// <param name="bSortByRank">是否要求采用级别排序</param>
		/// <param name="strAttrs">附加数据属性</param>
		/// <returns>重新排序结果</returns>
		public static DataSet GetObjectsSort(XmlDocument xmlDoc,
			SearchObjectColumn soc,
			bool bSortByRank,
			string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(xmlDoc.DocumentElement.OuterXml, soc, bSortByRank, strAttrs);
			DataSet result;
			//if (false == GetObjectsSortQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetObjectsSortQueue))
			//    {
			if (false == GetObjectsSortQueue.Instance.TryGetValue(cacheKey, out result))
			{
				if (strAttrs.Length == 0)
					strAttrs = "RANK_CODE";
				else
				{
					if (strAttrs.IndexOf("RANK_CODE") < 0)
						strAttrs += ",RANK_CODE";
				}
				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				string strColumnName = OGUCommonDefine.GetSearchObjectColumn(soc);

				StringBuilder strB = new StringBuilder(1024);
				XmlElement root = xmlDoc.DocumentElement;
				foreach (XmlElement elem in root.ChildNodes)
				{
					if (strB.Length > 0)
						strB.Append(",");

					strB.Append(TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute(strColumnName), true));
				}

				string strSortByRank = string.Empty;
				string strRankSortID = string.Empty;
				if (bSortByRank)
				{
					strSortByRank = " RANK_DEFINE.SORT_ID, ";
					strRankSortID = ", ISNULL (RANK_DEFINE.SORT_ID, 99) AS SORT_ID ";
				}

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					#region Sql Prepare
					string strSql = @"
				SELECT RESULT.* " + strRankSortID + @"
				FROM	(
							SELECT 'ORGANIZATIONS' AS OBJECTCLASS," + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE") + @"
							FROM ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
							WHERE " + DatabaseSchema.Instence.GetTableColumns(strColumnName, "ORGANIZATIONS") + @" IN ({0})
						UNION
							SELECT 'GROUPS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "GROUPS") + @"
							FROM GROUPS
							WHERE " + DatabaseSchema.Instence.GetTableColumns(strColumnName, "GROUPS") + @" IN ({0})
						UNION
							SELECT 'USERS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "OU_USERS", "USERS", "RANK_DEFINE") + @"
							FROM ORGANIZATIONS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
							WHERE ORGANIZATIONS.GUID = OU_USERS.PARENT_GUID
								AND USERS.GUID = OU_USERS.USER_GUID
								AND " + DatabaseSchema.Instence.GetTableColumns(strColumnName, "OU_USERS", "USERS") + @" IN ({0})
						)RESULT JOIN RANK_DEFINE 
							ON RANK_DEFINE.CODE_NAME = RESULT.RANK_CODE
				ORDER BY " + strSortByRank + " RESULT.GLOBAL_SORT";

					strSql = string.Format(strSql, strB.ToString());
					#endregion
					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetObjectsSortQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// 对于指定对象在系统中重新排序以后返回
		///// </summary>
		///// <param name="xmlDoc">包含所有要求被排序对象</param>
		///// <param name="soc">xmlDoc中适用于标识机构人员系统对象的属性类别</param>
		///// <param name="bSortByRank">是否要求采用级别排序</param>
		///// <param name="strAttrs">附加属性</param>
		///// <returns>重新排序结果</returns>
		//public static DataSet GetObjectsSort(XmlDocument xmlDoc, SearchObjectColumn soc, bool bSortByRank, string strAttrs)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetObjectsSort(xmlDoc, soc, bSortByRank, strAttrs, da);
		//    }
		//}

		/// <summary>
		/// 对于指定对象在系统中重新排序以后返回
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被排序对象</param>
		/// <param name="soc">xmlDoc中适用于标识机构人员系统对象的属性类别</param>
		/// <param name="strAttrs">附加属性</param>
		/// <returns>重新排序结果</returns>
		public static DataSet GetObjectsSort(XmlDocument xmlDoc, SearchObjectColumn soc, string strAttrs)
		{
			return GetObjectsSort(xmlDoc, soc, true, strAttrs);
		}

		/// <summary>
		/// 对于指定对象在系统中重新排序以后返回
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被排序对象</param>
		/// <param name="soc">xmlDoc中适用于标识机构人员系统对象的属性类别</param>
		/// <returns>重新排序结果</returns>
		public static DataSet GetObjectsSort(XmlDocument xmlDoc, SearchObjectColumn soc)
		{
			return GetObjectsSort(xmlDoc, soc, string.Empty);
		}
		#endregion

		#region GetRootDSE
		///// <summary>
		///// 获取系统指定的根部门
		///// </summary>
		///// <returns>获取系统指定的根部门</returns>
		//public static DataSet GetRootDSE()
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetRootDSE(da);
		//    }
		//}
		/// <summary>
		/// 获取系统指定的根部门
		/// </summary>
		/// <returns>获取系统指定的根部门</returns>
		public static DataSet GetRootDSE()
		{
			string strRootAllPathName = AccreditSection.GetConfig().AccreditSettings.OguRootName;
			DataSet result;
			//if (false == GetRootDSEQueue.Instance.TryGetValue(strRootAllPathName, out result))
			//{
			//    lock (typeof(GetRootDSEQueue))
			//    {
			if (false == GetRootDSEQueue.Instance.TryGetValue(strRootAllPathName, out result))
			{
				ExceptionHelper.TrueThrow(strRootAllPathName.Length == 0, "对不起，请设置好系统中的默认根部门！");
				string strSql = "SELECT * FROM ORGANIZATIONS WHERE ALL_PATH_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strRootAllPathName, true);

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);

					ExceptionHelper.TrueThrow(result.Tables[0].Rows.Count == 0, "对不起,系统设置的默认根部门不存在,请查证！");
				}
				GetRootDSEQueue.Instance.Add(strRootAllPathName, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		#endregion

		#region SignInCheck
		/// <summary>
		/// 验证人员登录名和密码是否正确
		/// </summary>
		/// <param name="strLogonName">用户登录名</param>
		/// <param name="strUserPwd">用户密码</param>
		/// <returns>登录名和密码是否匹配</returns>
		public static bool SignInCheck(string strLogonName, string strUserPwd)
		{
			try
			{
				ExceptionHelper.TrueThrow<ApplicationException>(string.IsNullOrEmpty(strLogonName), "对不起，登录名称不能为空");

				bool result = false;
				string key = strLogonName + strUserPwd;
				if (false == SignInCheckQueue.Instance.TryGetValue(key, out result))
				{
					ILogOnUserInfo logonUserInfo = new LogOnUserInfo(strLogonName, strUserPwd);

					result = (logonUserInfo != null);

					SignInCheckQueue.Instance.Add(key, result, InnerCacheHelper.PrepareDependency());
				}

				return result;
			}
			catch (ApplicationException)
			{
				return false;
			}
		}
		#endregion

		#region GetIndependOrganizationOfUser

		/// <summary>
		/// 获取当前制定对象的所在独立部门（隶属海关或者是派驻机构、直属海关、总署、分署、特派办）
		/// </summary>
		/// <param name="strUserGuid">自身所具有的数据值(用户)</param>
		/// <returns>获取当前制定对象的所在独立部门</returns>
		public static DataSet GetIndependOrganizationOfUser(string strUserGuid)
		{
			return GetIndependOrganizationOfUser("USERS", strUserGuid);
		}

		/// <summary>
		/// 获取当前制定对象的所在独立部门（隶属海关或者是派驻机构、直属海关、总署、分署、特派办）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjGuid">自身所具有的数据值</param>
		/// <returns>获取当前制定对象的所在独立部门</returns>
		public static DataSet GetIndependOrganizationOfUser(string strObjType, string strObjGuid)
		{
			return GetIndependOrganizationOfUser(strObjType, strObjGuid, string.Empty);
		}

		/// <summary>
		/// 获取当前制定对象的所在独立部门（隶属海关或者是派驻机构、直属海关、总署、分署、特派办）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjGuid">自身所具有的数据值</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>获取当前制定对象的所在独立部门</returns>
		public static DataSet GetIndependOrganizationOfUser(string strObjType, string strObjGuid, string strAttrs)
		{
			return GetIndependOrganizationOfUser(strObjType, strObjGuid, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// 获取当前制定对象的所在独立部门（隶属海关或者是派驻机构、直属海关、总署、分署、特派办）
		/// </summary>
		/// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		/// <param name="strObjValue">自身所具有的数据值</param>
		/// <param name="soc">自身所具有的数据类型（GUID、OBJ_NAME、ORIGINAL_SORT、GLOBAL_SORT等）</param>
		/// <param name="strAttrs">要求获取的数据字段</param>
		/// <returns>获取当前制定对象的所在独立部门</returns>
		public static DataSet GetIndependOrganizationOfUser(string strObjType, string strObjValue, SearchObjectColumn soc, string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strObjType, strObjValue, soc, strAttrs);
			DataSet result;
			//if (false == GetIndependOrganizationOfUserQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetIndependOrganizationOfUserQueue))
			//    {
			if (false == GetIndependOrganizationOfUserQueue.Instance.TryGetValue(cacheKey, out result))
			{
				if (strAttrs.IndexOf("ORG_CLASS") < 0)
				{
					if (strAttrs.Length > 0)
						strAttrs += ",ORG_CLASS";
					else
						strAttrs = "ORG_CLASS";
				}

				DataSet resultDS = GetObjectDepOrgs(strObjType, strObjValue, soc, 3, strAttrs);
				DataTable oTable = resultDS.Tables[0];
				if (oTable.Rows.Count > 0)
				{
					if (false == (oTable.Rows[0]["ORG_CLASS"] is DBNull))
						if (((int)oTable.Rows[0]["ORG_CLASS"] & (32 + 64)) != 0)
							return resultDS;
				}

				result = GetObjectDepOrgs(strObjType, strObjValue, soc, 2, strAttrs);
				GetIndependOrganizationOfUserQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		#endregion

		#region GetDirectCustoms
		/// <summary>
		/// 获取系统中的所有直属海关（依据于所设置的“关区代码”）
		/// </summary>
		/// <returns></returns>
		public static DataSet GetDirectCustoms()
		{
			return GetDirectCustoms(string.Empty);
		}

		/// <summary>
		/// 获取系统中的所有直属海关（依据于所设置的“关区代码”）
		/// </summary>
		/// <param name="strAttrs">所需要的附加字段名称</param>
		/// <returns></returns>
		public static DataSet GetDirectCustoms(string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strAttrs);
			DataSet result;
			//if (false == GetDirectCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetDirectCustomsQueue))
			//    {
			if (false == GetDirectCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					string strSql = @"
					SELECT " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS") + @" 
					FROM ORGANIZATIONS 
					WHERE LEN(CUSTOMS_CODE) = 4
						AND LEN(ORIGINAL_SORT) = 12 
					ORDER BY GLOBAL_SORT";

					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetDirectCustomsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		#endregion

		#region GetSubjectionCustoms
		/// <summary>
		/// 依据直属海关属性获取该直属海关辖的隶属海关
		/// </summary>
		/// <param name="strParentOrgValue"></param>
		/// <param name="soc"></param>
		/// <param name="strAttrs"></param>
		/// <returns></returns>
		public static DataSet GetSubjectionCustoms(string strParentOrgValue, SearchObjectColumn soc, string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strParentOrgValue, soc, strAttrs);
			DataSet result;
			//if (false == GetSubjectionCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetSubjectionCustomsQueue))
			//    {
			if (false == GetSubjectionCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					if (soc == SearchObjectColumn.SEARCH_GUID)
					{
						//这里在内部进行缓存处理，所以直接返回即可
						return GetSubjectionCustoms(strParentOrgValue, strAttrs);
						//result = GetSubjectionCustoms(strParentOrgValue, strAttrs);
					}
					else
					{
						strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
						string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

						Database database = DatabaseFactory.Create(context);

						string strSql = @"
						SELECT " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS") + @" 
						FROM ORGANIZATIONS
						WHERE LEN(CUSTOMS_CODE) = 4
							AND PARENT_GUID IN (
								SELECT GUID 
								FROM ORGANIZATIONS 
								WHERE " + TSqlBuilder.Instance.CheckQuotationMark(strColName, false)
											+ " = " + TSqlBuilder.Instance.CheckQuotationMark(strParentOrgValue, true) + @")
						ORDER BY GLOBAL_SORT";

						result = database.ExecuteDataSet(CommandType.Text, strSql);
					}
				}
				GetSubjectionCustomsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		/// <summary>
		/// 依据直属海关属性获取该直属海关辖的隶属海关
		/// </summary>
		/// <param name="strParentOrgValue"></param>
		/// <param name="soc"></param>
		/// <returns></returns>
		public static DataSet GetSubjectionCustoms(string strParentOrgValue, SearchObjectColumn soc)
		{
			return GetSubjectionCustoms(strParentOrgValue, soc, string.Empty);
		}

		/// <summary>
		/// 依据直属海关属性获取该直属海关辖的隶属海关
		/// </summary>
		/// <param name="strParentOrgGuid"></param>
		/// <param name="strAttrs"></param>
		/// <returns></returns>
		public static DataSet GetSubjectionCustoms(string strParentOrgGuid, string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strParentOrgGuid, SearchObjectColumn.SEARCH_GUID, strAttrs);
			DataSet result;
			//if (false == GetSubjectionCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetSubjectionCustomsQueue))
			//    {
			if (false == GetSubjectionCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					string strSql = @"
					SELECT " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS") + @" 
					FROM ORGANIZATIONS
					WHERE LEN(CUSTOMS_CODE) = 4
						AND PARENT_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(strParentOrgGuid, true);

					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetSubjectionCustomsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		/// <summary>
		/// 依据直属海关属性获取该直属海关辖的隶属海关
		/// </summary>
		/// <param name="strParentOrgGuid"></param>
		/// <returns></returns>
		public static DataSet GetSubjectionCustoms(string strParentOrgGuid)
		{
			return GetSubjectionCustoms(strParentOrgGuid, string.Empty);
		}

		#endregion

		#region GetLevelSortXmlDocAttr
		/// <summary>
		/// 按照各个对象中的strSortCol的指定字段设置来对tabls中的对象实现按层次送入XML中
		/// </summary>
		/// <param name="table">包含所有数据对象的数据表</param>
		/// <param name="strSortCol">层次实现的依据字段（一般是ORIGINAL_SORT和GLOBAL_SORT）</param>
		/// <param name="strNameCol">对象的类型对应字段名成（一般是OBJECTCLASS）</param>
		/// <param name="iSortLength">层次划分的长度依据（默认为6）</param>
		/// <returns>按照各个对象中的strSortCol的指定字段设置来对tabls中的对象实现按层次送入XML中</returns>
		public static XmlDocument GetLevelSortXmlDocAttr(DataTable table, string strSortCol, string strNameCol, int iSortLength)
		{
			if (iSortLength == 0)
				iSortLength = 6;

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<DataTable />");
			XmlNode root = xmlDoc.DocumentElement;
			string strParentSort = string.Empty;

			foreach (DataRow row in table.Rows)
			{
				XmlElement rowElem = (XmlElement)xmlDoc.CreateNode(XmlNodeType.Element, OGUCommonDefine.DBValueToString(row[strNameCol]), string.Empty);
				string strSortValue = OGUCommonDefine.DBValueToString(row[strSortCol]);

				while (strParentSort != strSortValue.Substring(0, strSortValue.Length - iSortLength))
				{
					if (root == xmlDoc.DocumentElement)
						break;
					else
					{
						if (strParentSort.Length > 0
							&& strSortValue.Length > strParentSort.Length
							&& strParentSort == strSortValue.Substring(0, strParentSort.Length))
							break;
						//{
						//	root = root.ParentNode;
						//	strParentSort = ((XmlElement)root).GetAttribute(strSortCol);
						//}
						else
						{
							root = root.ParentNode;
							strParentSort = ((XmlElement)root).GetAttribute(strSortCol);
						}
					}
				}

				root = root.AppendChild(rowElem);
				strParentSort = strSortValue;
				foreach (DataColumn col in table.Columns)
				{
					rowElem.SetAttribute(col.ColumnName, OGUCommonDefine.DBValueToString(row[col.ColumnName]));
				}
			}

			return xmlDoc;
		}
		#endregion

		#region RemoveAllDataCache
		/// <summary>
		/// 清理数据缓存
		/// </summary>
		public static void RemoveAllCache()
		{
			InnerCacheHelper.RemoveAllCache();
		}
		#endregion

		#endregion

		#region public functions 【Deleted】
		///// <summary>
		///// 获取当前制定对象的所在独立部门（隶属海关或者是派驻机构、直属海关、总署、分署、特派办）
		///// </summary>
		///// <param name="strUserGuid">自身所具有的数据值(用户)</param>
		///// <param name="da">数据库操作对象</param>
		///// <returns>获取当前制定对象的所在独立部门</returns>
		//public static DataSet GetIndependOrganizationOfUser(string strUserGuid)
		//{
		//    return GetIndependOrganizationOfUser("USERS", strUserGuid, da);
		//}

		///// <summary>
		///// 获取当前制定对象的所在独立部门（隶属海关或者是派驻机构、直属海关、总署、分署、特派办）
		///// </summary>
		///// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		///// <param name="strObjGuid">自身所具有的数据值</param>
		///// <param name="da">数据库操作对象</param>
		///// <returns>获取当前制定对象的所在独立部门</returns>
		//public static DataSet GetIndependOrganizationOfUser(string strObjType, string strObjGuid)
		//{
		//    return GetIndependOrganizationOfUser(strObjType, strObjGuid, string.Empty, da);
		//}

		///// <summary>
		///// 获取当前制定对象的所在独立部门（隶属海关或者是派驻机构、直属海关、总署、分署、特派办）
		///// </summary>
		///// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		///// <param name="strObjGuid">自身所具有的数据值</param>
		///// <param name="strAttrs">要求获取的数据字段</param>
		///// <param name="da">数据库操作对象</param>
		///// <returns>获取当前制定对象的所在独立部门</returns>
		//public static DataSet GetIndependOrganizationOfUser(string strObjType, string strObjGuid, string strAttrs)
		//{
		//    return GetIndependOrganizationOfUser(strObjType, strObjGuid, SearchObjectColumn.SEARCH_GUID, strAttrs, da);
		//}

		///// <summary>
		///// 获取当前制定对象的所在独立部门（隶属海关或者是派驻机构、直属海关、总署、分署、特派办）
		///// </summary>
		///// <param name="strObjType">要查询自身的对象（GROUPS、USERS或者ORGANIZATIONS）</param>
		///// <param name="strObjValue">自身所具有的数据值</param>
		///// <param name="soc">自身所具有的数据类型（GUID、OBJ_NAME、ORIGINAL_SORT、GLOBAL_SORT等）</param>
		///// <param name="strAttrs">要求获取的数据字段</param>
		///// <param name="da">数据库操作对象</param>
		///// <returns>获取当前制定对象的所在独立部门</returns>
		//public static DataSet GetIndependOrganizationOfUser(string strObjType, 
		//    string strObjValue, 
		//    SearchObjectColumn soc, 
		//    string strAttrs)
		//{
		//    if (strAttrs.IndexOf("ORG_CLASS") < 0)
		//    {
		//        if (strAttrs.Length > 0)
		//            strAttrs += ",ORG_CLASS";
		//        else
		//            strAttrs = "ORG_CLASS";
		//    }

		//    DataSet resultDS = GetObjectDepOrgs(strObjType, strObjValue, soc, 3, strAttrs, da);
		//    DataTable oTable = resultDS.Tables[0];
		//    if (oTable.Rows.Count > 0)
		//    {
		//        if (false==(oTable.Rows[0]["ORG_CLASS"] is DBNull))
		//            if (((int)oTable.Rows[0]["ORG_CLASS"] & (32 + 64)) != 0)
		//                return resultDS;
		//    }

		//    resultDS = GetObjectDepOrgs(strObjType, strObjValue, soc, 2, strAttrs, da);

		//    return resultDS;
		//}
		#endregion

		#region private functions
		/// <summary>
		/// 形成根据指定条件查询机构“机构”的SQL语句
		/// </summary>
		/// <param name="strOrgValues">指定的范围机构标识（多个之间采用","分隔）</param>
		/// <param name="soc">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）
		/// </param>
		/// <param name="strLikeName">机构上的名称模糊匹配对象</param>
		/// <param name="bLike">是否采用模糊匹配</param>
		/// <param name="strOrgRankCodeName">机构要求的级别范围</param>
		/// <param name="strAttr">要求获取的字段属性</param>
		/// <param name="iDep">要求查询的深度</param>
		/// <param name="strHideType">要求屏蔽的类型设置</param>
		/// <param name="rootPath">传入的ORIGINAL_SORT</param>
		/// <returns>形成根据指定条件查询机构的SQL语句</returns>
		/// 
		//2009-05-11
		private static string QueryOrganizationsByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			string strOrgRankCodeName,
			string strAttr,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append("	SELECT 'ORGANIZATIONS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttr, "ORGANIZATIONS", "RANK_DEFINE") + @"
							FROM ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
									{0} 
							WHERE ( " + GetSqlSearchParOriginal2("ORGANIZATIONS", iDep, rootPath) + " ) ");

			string strListDelete = GetSqlSearchStatus("ORGANIZATIONS", ListObjectDelete.ALL_TYPE);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			string strRankLimit = string.Empty;
			if (strOrgRankCodeName.Length > 0)
				strRankLimit = " AND RANK_CLASS = 1 AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
					+ TSqlBuilder.Instance.CheckQuotationMark(strOrgRankCodeName, true) + " ) ";

			strB.Append(BuildSearchCondition("ORGANIZATIONS.SEARCH_NAME", strLikeName, bLike));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "ORGANIZATIONS"));

			return string.Format(strB.ToString(), strRankLimit);
		}

		//2009-05-06 删除 RANK_DEFINE 约束，调整ORIGINAL_SORT约束
		private static string QueryOrganizationsByCondition2(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			string strAttr,
			ListObjectDelete iLod,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append("	SELECT 'ORGANIZATIONS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttr, "ORGANIZATIONS") + @"
							FROM ORGANIZATIONS 								
							WHERE ( " + GetSqlSearchParOriginal2("ORGANIZATIONS", iDep, rootPath) + " ) ");

			string strListDelete = GetSqlSearchStatus("ORGANIZATIONS", iLod);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			strB.Append(BuildSearchCondition("ORGANIZATIONS.SEARCH_NAME", strLikeName, bLike));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "ORGANIZATIONS"));

			return strB.ToString();
		}

		/// <summary>
		/// 形成根据指定条件查询“人员组”的SQL语句
		/// </summary>
		/// <param name="strOrgValues">指定的范围机构标识（多个之间采用","分隔）</param>
		/// <param name="soc">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strLikeName">“人员组”上的名称模糊匹配对象</param>
		/// <param name="bLike">是否采用模糊匹配</param>
		/// <param name="strAttr">要求获取的字段属性</param>
		/// <param name="iDep">要求查询的深度</param>
		/// <param name="strHideType">要求屏蔽的类型设置</param>
		/// <param name="rootPath">传入的ORIGINAL_SORT</param>
		/// <returns>形成根据指定条件查询“人员组”的SQL语句</returns>
		/// 
		//2009-05-11
		private static string QueryGroupsByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			string strAttr,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'GROUPS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttr, "GROUPS") + @"
							FROM GROUPS 
							WHERE ( " + GetSqlSearchParOriginal2("GROUPS", iDep, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("GROUPS", ListObjectDelete.ALL_TYPE);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			strB.Append(BuildSearchCondition("GROUPS.SEARCH_NAME", strLikeName, bLike));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "GROUPS"));

			return strB.ToString();
		}

		/// <summary>
		/// 形成根据指定条件查询“人员组”的SQL语句
		/// </summary>
		/// <param name="strOrgValues">指定的范围机构标识（多个之间采用","分隔）</param>
		/// <param name="soc">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strLikeName">“人员组”上的名称模糊匹配对象</param>
		/// <param name="bLike">是否采用模糊匹配</param>
		/// <param name="strAttr">要求获取的字段属性</param>
		/// <param name="iLod">查询删除的对象策略</param>
		/// <param name="iDep">要求查询的深度</param>
		/// <param name="strHideType">要求屏蔽的类型设置</param>
		/// <param name="rootPath">传入的ORIGINAL_SORT</param>
		/// <returns>形成根据指定条件查询“人员组”的SQL语句</returns>
		/// 
		//2009-05-06 删除 RANK_DEFINE 约束，调整ORIGINAL_SORT约束
		private static string QueryGroupsByCondition2(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			string strAttr,
			ListObjectDelete iLod,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'GROUPS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttr, "GROUPS") + @"
							FROM GROUPS
							WHERE ( " + GetSqlSearchParOriginal2("GROUPS", iDep, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("GROUPS", iLod);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			strB.Append(BuildSearchCondition("GROUPS.SEARCH_NAME", strLikeName, bLike));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "GROUPS"));

			return strB.ToString();
		}

		/// <summary>
		/// 形成根据指定条件查询“人员”的SQL语句
		/// </summary>
		/// <param name="strOrgValues">指定的范围机构标识（多个之间采用","分隔）</param>
		/// <param name="soc">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strLikeName">“人员”上的名称模糊匹配对象</param>
		/// <param name="bLike">是否采用模糊匹配</param>
		/// <param name="bFirstPerson">查询对象是否是一把手</param>
		/// <param name="strUserRankCodeName">“人员”要求的级别范围</param>
		/// <param name="strAttr">要求获取的字段属性</param>
		/// <param name="iListObjType">用于是否查询兼职问题</param>
		/// <param name="iDep">要求查询的深度</param>
		/// <param name="strHideType">要求屏蔽的类型设置</param>
		/// <param name="rootPath">传入的ORIGINAL_SORT</param>
		/// <returns>形成根据指定条件查询“人员”的SQL语句</returns>
		/// 
		//2009-05-11
		private static string QueryUsersByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			bool bFirstPerson,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'USERS' AS OBJECTCLASS, "
							+ DatabaseSchema.Instence.GetTableColumns(strAttr, "USERS", "OU_USERS", "RANK_DEFINE") + @"
							FROM USERS JOIN RANK_DEFINE 
								ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE {0}, OU_USERS  
							WHERE USERS.GUID = OU_USERS.USER_GUID 
								AND ( " + GetSqlSearchParOriginal2("OU_USERS", iDep, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("OU_USERS", ListObjectDelete.ALL_TYPE);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			string strRankLimit = string.Empty;
			if (strUserRankCodeName.Length > 0)
				strRankLimit = " AND RANK_CLASS = 2 AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
					+ TSqlBuilder.Instance.CheckQuotationMark(strUserRankCodeName, true) + " ) ";

			if ((iListObjType & (int)ListObjectType.SIDELINE) == 0)
				strB.Append(" \n AND OU_USERS.SIDELINE = 0 ");

			strB.Append(BuildSearchCondition("OU_USERS.SEARCH_NAME", strLikeName, bLike));

			if (bFirstPerson)//要求是部门一把手
				strB.Append("	AND OU_USERS.INNER_SORT = " + TSqlBuilder.Instance.CheckQuotationMark(CommonResource.OriginalSortDefault, true));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "OU_USERS"));

			return string.Format(strB.ToString(), strRankLimit);
		}

		/// <summary>
		/// 形成根据指定条件查询“人员”的SQL语句
		/// </summary>
		/// <param name="strOrgValues">指定的范围机构标识（多个之间采用","分隔）</param>
		/// <param name="soc">用户的属性名称
		/// （ＧＵＩＤ、ＵＳＥＲ＿ＧＵＩＤ、ＬＯＧＯＮ＿ＮＡＭＥ、ＯＲＩＧＩＮＡＬ＿ＳＯＲＴ、ＧＬＯＢＡＬ＿ＳＯＲＴ、ＡＬＬ＿ＰＡＴＨ＿ＮＡＭＥ）</param>
		/// <param name="strLikeName">“人员”上的名称模糊匹配对象</param>
		/// <param name="bLike">是否采用模糊匹配</param> 
		/// <param name="strAttr">要求获取的字段属性</param>
		/// <param name="iListObjType">用于是否查询兼职问题</param>
		/// <param name="iLod">查询删除的对象策略</param>
		/// <param name="iDep">要求查询的深度</param>
		/// <param name="strHideType">要求屏蔽的类型设置</param>
		/// <param name="rootPath">传入的ORIGINAL_SORT</param>
		/// <returns>形成根据指定条件查询“人员”的SQL语句</returns>
		/// 
		//2009-05-06 删除 RANK_DEFINE 约束，调整ORIGINAL_SORT约束
		private static string QueryUsersByCondition2(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			string strAttr,
			int iListObjType,
			ListObjectDelete iLod,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'USERS' AS OBJECTCLASS, "
							+ DatabaseSchema.Instence.GetTableColumns(strAttr, "USERS", "OU_USERS") + @"
							FROM USERS , OU_USERS
							WHERE USERS.GUID = OU_USERS.USER_GUID 
								AND ( " + GetSqlSearchParOriginal2("OU_USERS", iDep, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("OU_USERS", iLod);

			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			if ((iListObjType & (int)ListObjectType.SIDELINE) == 0)
				strB.Append(" \n AND OU_USERS.SIDELINE = 0 ");

			if (strLikeName != SearchAllTerm)
				strB.Append(string.Format(@" AND (OU_USERS.SEARCH_NAME  like '%{0}%')", strLikeName));
			else
				strB.Append(BuildSearchCondition("OU_USERS.SEARCH_NAME", strLikeName, bLike));

			//strB.Append(BuildSearchCondition("OU_USERS.SEARCH_NAME", strLikeName, bLike));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "OU_USERS"));

			return strB.ToString();
		}

		private static string BuildSearchCondition(string columnName, string searchTerm, bool bLike)
		{
			string result = string.Empty;

			if (string.Compare(searchTerm, SearchAllTerm, true) != 0)
			{
				if (bLike)
				{
					if (AccreditSection.GetConfig().AccreditSettings.FuzzySearch)
						searchTerm = string.Format("\"*{0}*\"", searchTerm);
					else
						searchTerm = string.Format("\"{0}\"", searchTerm);
				}

				result = string.Format(" AND (CONTAINS({0}, {1}))", columnName, TSqlBuilder.Instance.CheckQuotationMark(searchTerm, true));
			}

			return result;
		}

		/// <summary>
		/// 通过设置在查询中屏蔽掉指定要求屏蔽的数据对象的展现
		/// </summary>
		/// <param name="strHideType">对应于配置文件中的配置名称（多个之间采用","分隔）</param>
		/// <param name="strObjClass">要求屏蔽的数据对象类型</param>
		/// <returns>用于形成SQL语句中的条件语句（要求屏蔽的数据对象）</returns>
		private static string GetHideTypeFromXml(string strHideType, string strObjClass)
		{
			StringBuilder strB = new StringBuilder(1024);

			XmlDocument xmlDoc = GetMaskObjectDocument();// (new SysConfig()).GetConfigXmlDocument("MaskObjects");

			if (xmlDoc != null)
			{
				string[] strArrs = strHideType.Split(',', ' ');

				for (int i = 0; i < strArrs.Length; i++)
				{
					XmlNode hideNode = xmlDoc.DocumentElement.SelectSingleNode("HideType[@name=\"" + strArrs[i] + "\"]");
					ExceptionHelper.TrueThrow(hideNode == null, "在配置文件中找不到要求屏蔽的数据设置！");

					foreach (XmlElement elem in hideNode.SelectNodes(strObjClass))
					{
						if (strB.Length > 0)
							strB.Append(",");
						string strValue = elem.GetAttribute("ALL_PATH_NAME");

						ExceptionHelper.TrueThrow(strValue.Length == 0, "数据配置文件中的配置不正确！");
						strB.Append(TSqlBuilder.Instance.CheckQuotationMark(strValue, true));
					}
				}
			}

			return strB.ToString();
		}

		private static XmlDocument GetMaskObjectDocument()
		{
			if (OGUReader.MaskObjectDocument == null)
			{
				string filePath = AccreditSection.GetConfig().AccreditSettings.MaskObjects;

				bool IsFileExist = true;
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
					OGUReader.MaskObjectDocument = XmlHelper.LoadDocument(filePath);
				}
			}

			return OGUReader.MaskObjectDocument;
		}


		/// <summary>
		/// 通过设置在查询中屏蔽掉指定要求屏蔽的数据对象的展现
		/// </summary>
		/// <param name="strHideType">对应于配置文件中的配置名称（多个之间采用","分隔）</param>
		/// <param name="strObjClass">要求屏蔽的数据对象类型</param>
		/// <returns>通过设置在查询中屏蔽掉指定要求屏蔽的数据对象的展现</returns>
		private static string GetHideTypeFromXmlForLike(string strHideType, string strObjClass)
		{
			StringBuilder strB = new StringBuilder(1024);

			XmlDocument xmlDoc = GetMaskObjectDocument();// (new SysConfig()).GetConfigXmlDocument("MaskObjects");

			if (xmlDoc != null)
			{
				string[] strArrs = strHideType.Split(',', ' ');

				for (int i = 0; i < strArrs.Length; i++)
				{
					XmlNode hideNode = xmlDoc.DocumentElement.SelectSingleNode("HideType[@name=\"" + strArrs[i] + "\"]");
					ExceptionHelper.TrueThrow(hideNode == null, "在配置文件中找不到要求屏蔽的数据设置！");

					foreach (XmlElement elem in hideNode.SelectNodes("ORGANIZATIONS"))
					{
						string strValue = elem.GetAttribute("ALL_PATH_NAME");

						ExceptionHelper.TrueThrow(strValue.Length == 0, "数据配置文件中的配置不正确！");
						strB.Append(" AND " + strObjClass + ".ALL_PATH_NAME NOT LIKE " + TSqlBuilder.Instance.CheckQuotationMark(strValue, true) + " + '%' ");
					}

					if (strObjClass == "GROUPS" || strObjClass == "OU_USERS")
					{
						foreach (XmlElement elem in hideNode.SelectNodes(strObjClass))
						{
							string strValue = elem.GetAttribute("ALL_PATH_NAME");

							ExceptionHelper.TrueThrow(strValue.Length == 0, "数据配置文件中的配置不正确！");
							strB.Append(" AND " + strObjClass + ".ALL_PATH_NAME <> " + TSqlBuilder.Instance.CheckQuotationMark(strValue, true) + " ");
						}
					}
				}
			}

			return strB.ToString();
		}

		/// <summary>
		/// 将HashTable中的数据转换到字符串中，用于SQL查询使用（单项前后都得要加单引号，而且采用“,”分隔）
		/// </summary>
		/// <param name="hash">要求转换的HashTable</param>
		/// <returns>转换以后的字符串</returns>
		private static string TransHashToSqlString(SortedList<string, string> hash)
		{
			StringBuilder strB = new StringBuilder(1024);
			foreach (KeyValuePair<string, string> dict in hash)
			{
				if (strB.Length > 0)
					strB.Append(", ");
				strB.Append(TSqlBuilder.Instance.CheckQuotationMark(dict.Value.ToString(), true));
			}
			return strB.ToString();
		}

		///// <summary>
		///// 根据数据表名称以及要求获取的数据表中的数据列名称组合成为核数据表相关的数据查询语言
		///// </summary>
		///// <param name="strAttrs">数据表中的数据列名称组合</param>
		///// <param name="da">数据库操作对象</param>
		///// <param name="strTables">指定的数据表名称</param>
		///// <returns>数据查询语言SQL</returns>
		//private static string GetTableColumns(string strAttrs, params string[] strTables)
		//{
		//    StringBuilder strB = new StringBuilder(1024);
		//    ExceptionHelper.TrueThrow(strAttrs.Length == 0, "对不起,程序没有指定要求查询的列名称！请验证！");
		//    ExceptionHelper.TrueThrow(strTables.Length == 0, "对不起，没有确定的数据表名称！");

		//    if (strAttrs.Trim() == "*")
		//        strB.Append(strTables[0] + "." + strAttrs.Trim());
		//    else
		//    {
		//        string[] strAttrArr = strAttrs.Split(',');
		//        InitHashTable(da);
		//        for (int i = 0; i < strAttrArr.Length; i++)
		//        {
		//            strAttrArr[i] = strAttrArr[i].Trim();

		//            if (strB.Length > 0)
		//                strB.Append(", ");

		//            bool bComplicated = false;

		//            for (int j = 0; j < strTables.Length; j++)
		//            {
		//                DataTable table = _DataSet_Schema.Tables[strTables[j]];

		//                DataRow[] drs = table.Select("CNAME=" + TSqlBuilder.Instance.CheckQuotationMark(strAttrArr[i]));
		//                if (drs.Length > 0)
		//                {
		//                    strB.Append(XmlHelper.DBValueToString(drs[0]["TNAME"]) + "." + strAttrArr[i]);
		//                    bComplicated = true;
		//                    break;
		//                }
		//            }

		//            if (false==bComplicated)
		//                strB.Append(" NULL AS " + strAttrArr[i]);
		//        }
		//    }

		//    return strB.ToString();
		//}

		/// <summary>
		/// 根据指定的查询条件类，生成系统中对于“机构”的查询SQL语句
		/// </summary>
		/// <param name="scc">查询条件类对象</param>
		/// <param name="rootPath">传入的ORIGINAL_SORT</param>
		/// <returns>针对于“机构”的查询SQL语句</returns>
		/// 
		//2009-05-11
		private static string GetOrganizationsSqlByScc(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			string strRootGuids = TransHashToSqlString(scc.RootGuids);

			strB.Append(@"	SELECT 'ORGANIZATIONS' AS OBJECTCLASS, "
				+ DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "ORGANIZATIONS", "RANK_DEFINE") + @"
							FROM ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
									{1} 
							WHERE(	GUID IN(" + strRootGuids + @") 
									OR	
									(
											( " + GetSqlSearchParOriginal2("ORGANIZATIONS", scc.Depth, rootPath) + @" ) 
											{0}
									)
								)");

			string strListDelete = GetSqlSearchStatus("ORGANIZATIONS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strListDelete = " AND (" + strListDelete + ")";

			string strRankLimit = string.Empty;
			if (scc.OrgRankCN.Length > 0)
				strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
					+ TSqlBuilder.Instance.CheckQuotationMark(scc.OrgRankCN, true) + " ) ";

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "ORGANIZATIONS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND ORGANIZATIONS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}

			if (scc.OrgClass != 1024 * 1024 - 1)
				strB.Append(" \n AND (ORGANIZATIONS.ORG_CLASS = 0 OR ORGANIZATIONS.ORG_CLASS & " + scc.OrgClass + " <> 0 ) ");

			if (scc.OrgType != 1024 * 1024 - 1)
				strB.Append(" \n AND (ORGANIZATIONS.ORG_TYPE = 0 OR ORGANIZATIONS.ORG_TYPE & " + scc.OrgType + " <> 0 ) ");

			return string.Format(strB.ToString(), strListDelete, strRankLimit);
		}

		//2009-05-07删除RANK_DEFINE约束，修改ORIGINAL_SORT约束
		private static string GetOrganizationsSqlByScc2(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			string strRootGuids = TransHashToSqlString(scc.RootGuids);

			strB.Append(@"	SELECT 'ORGANIZATIONS' AS OBJECTCLASS, "
				+ DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "ORGANIZATIONS") + @"
							FROM ORGANIZATIONS							
							WHERE(	GUID IN(" + strRootGuids + @") 
									OR	
									(
											( " + GetSqlSearchParOriginal2("ORGANIZATIONS", scc.Depth, rootPath) + @" ) 
											{0}
									)
								)");

			string strListDelete = GetSqlSearchStatus("ORGANIZATIONS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strListDelete = " AND (" + strListDelete + ")";

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "ORGANIZATIONS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND ORGANIZATIONS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}

			if (scc.OrgClass != 1024 * 1024 - 1)
				strB.Append(" \n AND (ORGANIZATIONS.ORG_CLASS = 0 OR ORGANIZATIONS.ORG_CLASS & " + scc.OrgClass + " <> 0 ) ");

			if (scc.OrgType != 1024 * 1024 - 1)
				strB.Append(" \n AND (ORGANIZATIONS.ORG_TYPE = 0 OR ORGANIZATIONS.ORG_TYPE & " + scc.OrgType + " <> 0 ) ");

			return string.Format(strB.ToString(), strListDelete);
		}

		/// <summary>
		/// 根据指定的查询条件类，生成系统中对于“人员组”的查询SQL语句
		/// </summary>
		/// <param name="scc">查询条件类对象</param>
		/// <param name="rootPath">传入的ORIGINAL_SORT</param>
		/// <returns>针对于“人员组”的查询SQL语句</returns>
		/// 
		//2009-05-11
		private static string GetGroupsSqlByScc(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@" SELECT 'GROUPS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "GROUPS") + @"
							FROM GROUPS 
							WHERE ( " + GetSqlSearchParOriginal2("GROUPS", scc.Depth, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("GROUPS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "GROUPS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND GROUPS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}

			return strB.ToString();
		}

		/// <summary>
		/// 根据指定的查询条件类，生成系统中对于“人员组”的查询SQL语句
		/// </summary>
		/// <param name="scc">查询条件类对象</param>
		/// <param name="rootPath">传入的ORIGINAL_SORT</param>
		/// <returns>针对于“人员组”的查询SQL语句</returns>
		/// 
		//2009-05-07删除RANK_DEFINE约束，修改ORIGINAL_SORT约束
		private static string GetGroupsSqlByScc2(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@" SELECT 'GROUPS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "GROUPS") + @"
							FROM GROUPS 
							WHERE ( " + GetSqlSearchParOriginal2("GROUPS", scc.Depth, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("GROUPS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "GROUPS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND GROUPS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}

			return strB.ToString();
		}

		/// <summary>
		/// 根据指定的查询条件类，生成系统中对于“人员”的查询SQL语句
		/// </summary>
		/// <param name="scc">查询条件类对象</param>
		/// <param name="rootPath">传入的ORIGINAL_SORT</param>
		/// <returns>针对于“人员”的查询SQL语</returns>
		/// 
		//2009-05-11
		private static string GetUsersSqlByScc(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'USERS' AS OBJECTCLASS, "
				+ DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
							FROM USERS JOIN RANK_DEFINE 
								ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
									{0}, OU_USERS 
							WHERE USERS.GUID = OU_USERS.USER_GUID 
								AND ( " + GetSqlSearchParOriginal2("OU_USERS", scc.Depth, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("OU_USERS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			string strRankLimit = string.Empty;
			if (scc.UserRankCN.Length > 0)
				strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(scc.UserRankCN, true) + " ) ";

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "OU_USERS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND OU_USERS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}
			if ((scc.ListObjType & ListObjectType.SIDELINE) == 0)
				strB.Append(" \n AND OU_USERS.SIDELINE = 0 ");

			return string.Format(strB.ToString(), strRankLimit);
		}

		/// <summary>
		/// 根据指定的查询条件类，生成系统中对于“人员”的查询SQL语句
		/// </summary>
		/// <param name="scc">查询条件类对象</param>
		/// <param name="rootPath">传入的ORIGINAL_SORT</param>
		/// <returns>针对于“人员”的查询SQL语</returns>
		/// 
		//2009-05-07删除RANK_DEFINE约束，修改ORIGINAL_SORT约束
		private static string GetUsersSqlByScc2(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'USERS' AS OBJECTCLASS, "
				+ DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "USERS", "OU_USERS") + @"
							FROM USERS , OU_USERS
							WHERE USERS.GUID = OU_USERS.USER_GUID 
								AND ( " + GetSqlSearchParOriginal2("OU_USERS", scc.Depth, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("OU_USERS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "OU_USERS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND OU_USERS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}
			if ((scc.ListObjType & ListObjectType.SIDELINE) == 0)
				strB.Append(" \n AND OU_USERS.SIDELINE = 0 ");

			return strB.ToString();
		}

		/// <summary>
		/// 根据要求查询对象中是否展现被（逻辑）删除的对象生成数据查询条件
		/// </summary>
		/// <param name="strTableName">要求查询的数据表</param>
		/// <param name="lod">要求展现的数据删除类型（默认都是数据删除上的普通对象）</param>
		/// <returns>SQL中与数据删除相关的</returns>
		private static string GetSqlSearchStatus(string strTableName, ListObjectDelete lod)
		{
			string strListDelete = string.Empty;
			if ((lod & ListObjectDelete.COMMON) != ListObjectDelete.None)
				strListDelete = " (" + strTableName + ".STATUS = " + ((int)ListObjectDelete.COMMON).ToString() + ") ";
			if ((lod & ListObjectDelete.DIRECT_LOGIC) != ListObjectDelete.None)
			{
				if (strListDelete.Length > 0)
					strListDelete += " OR ";
				strListDelete += " (" + strTableName + ".STATUS & " + ((int)ListObjectDelete.DIRECT_LOGIC).ToString() + ") <> 0 ";
			}
			if ((lod & ListObjectDelete.CONJUNCT_ORG_LOGIC) != ListObjectDelete.None)
			{
				if (strListDelete.Length > 0)
					strListDelete += " OR ";
				strListDelete += " (" + strTableName + ".STATUS & " + ((int)ListObjectDelete.CONJUNCT_ORG_LOGIC).ToString() + ") <> 0 ";
			}
			if ((lod & ListObjectDelete.CONJUNCT_USER_LOGIC) != ListObjectDelete.None)
			{
				if (strListDelete.Length > 0)
					strListDelete += " OR ";
				strListDelete += " (" + strTableName + ".STATUS & " + ((int)ListObjectDelete.CONJUNCT_USER_LOGIC).ToString() + ") <> 0 ";
			}
			return strListDelete;
		}

		/// <summary>
		/// 设置对于查询中涉及的查询深度的数据处理
		/// </summary>
		/// <param name="strTableName">要求查询的数据表</param>
		/// <param name="iDep">要求查询的深度（0表示不限深度）</param>
		/// <returns>SQL中与深度相关的数据查询条件</returns>
		private static string GetSqlSearchParOriginal(string strTableName, int iDep)
		{
			string strOriginalSort = string.Empty;
			if (iDep == 0)
				strOriginalSort = " " + strTableName + ".ORIGINAL_SORT LIKE RootOrg.ORIGINAL_SORT + '%'";
			else
			{
				strOriginalSort = " " + strTableName + ".ORIGINAL_SORT = RootOrg.ORIGINAL_SORT ";
				string strDepth = string.Empty;
				for (int i = 0; i < iDep; i++)
				{
					strDepth += "______";
					strOriginalSort += " OR " + strTableName + ".ORIGINAL_SORT LIKE RootOrg.ORIGINAL_SORT + "
						+ TSqlBuilder.Instance.CheckQuotationMark(strDepth, true);
				}
			}
			return strOriginalSort;
		}

		//2009-05-06
		private static string GetSqlSearchParOriginal2(string strTableName, int iDep, string rootPath)
		{
			string strOriginalSort = string.Empty;
			if (iDep == 0)
				strOriginalSort = " " + strTableName + ".ORIGINAL_SORT LIKE " + TSqlBuilder.Instance.CheckQuotationMark(rootPath + "%", true);
			else
			{
				strOriginalSort = " " + strTableName + ".ORIGINAL_SORT = " + TSqlBuilder.Instance.CheckQuotationMark(rootPath, true);

				string strDepth = string.Empty;
				for (int i = 0; i < iDep; i++)
				{
					strDepth += "______";
					strOriginalSort += " OR " + strTableName + ".ORIGINAL_SORT LIKE " + TSqlBuilder.Instance.CheckQuotationMark(rootPath, true) + " + "
						+ TSqlBuilder.Instance.CheckQuotationMark(strDepth, true);
				}
			}
			return strOriginalSort;
		}

		/// <summary>
		/// 根据查询语句判断是否符合查询结果
		/// </summary>
		/// <param name="strB"></param>
		/// <param name="bFitAll"></param>
		/// <returns></returns>
		private static bool CheckIsUserInOrganizations(StringBuilder strB, bool bFitAll)
		{
			bool bResult = false;
			if (strB.Length > 0)
			{
				DataSet ds;
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);

					ds = database.ExecuteDataSet(CommandType.Text, strB.ToString());
				}
				if (bFitAll)
				{
					bResult = true;
					foreach (DataTable table in ds.Tables)
					{
						if (table.Rows.Count <= 0)
						{
							bResult = false;
							break;
						}
					}
				}
				else
				{
					bResult = false;
					foreach (DataTable table in ds.Tables)
					{
						if (table.Rows.Count > 0)
						{
							bResult = true;
							break;
						}
					}
				}
			}

			return bResult;
		}

		/// <summary>
		/// CheckUserInOrganizations函数中获取用户的限制数据
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="strUserColName"></param>
		/// <returns></returns>
		private static string GetUserLimitInCheckUserInOrganizations(XmlDocument xmlDoc, string strUserColName)
		{
			StringBuilder strB = new StringBuilder(512);

			XmlElement root = xmlDoc.DocumentElement;
			ExceptionHelper.TrueThrow(root.ChildNodes.Count == 0, "对不起，你给定的人员结构信息数据结构不符或者数据不完整！");
			foreach (XmlElement elem in root.ChildNodes)
			{
				if (strB.Length > 0)
					strB.Append(" OR ");
				strB.Append(" (" + DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " = " + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true));

				string strParentGuid = elem.GetAttribute("parentGuid");
				if (strParentGuid != null && strParentGuid.Length > 0)
					strB.Append(" AND OU_USERS.PARENT_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(strParentGuid, true));

				strB.Append(" ) ");
			}

			return " ( " + strB.ToString() + " ) ";
		}

		//        /// <summary>
		//        /// 
		//        /// </summary>
		//        private static void InitHashTable()
		//        {
		//            if (_DataSet_Schema == null)
		//            {
		//                string[] strAllTables = { "ORGANIZATIONS", "GROUPS", "USERS", "OU_USERS", "RANK_DEFINE", "GROUP_USERS", "SECRETARIES" };
		//                StringBuilder strB = new StringBuilder(512);
		//                for (int i = 0; i < strAllTables.Length; i++)
		//                {
		//                    strB.Append(@"
		//						SELECT SYSOBJECTS.NAME AS TNAME, SYSCOLUMNS.NAME AS CNAME
		//						FROM SYSOBJECTS, SYSCOLUMNS
		//						WHERE SYSOBJECTS.ID = SYSCOLUMNS.ID
		//							AND SYSOBJECTS.NAME IN (" + TSqlBuilder.Instance.CheckQuotationMark(strAllTables[i]) + @")
		//						ORDER BY TNAME, SYSCOLUMNS.COLID;
		//						");
		//                }
		//                _DataSet_Schema = OGUCommonDefine.ExecuteDataset(strB.ToString(), da, strAllTables);
		//            }
		//        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlObjDoc"></param>
		/// <param name="strUserColName"></param>
		/// <param name="strObjColName"></param>
		/// <param name="strDirect"></param>
		/// <param name="strExtraWhere"></param>
		/// <returns></returns>
		private static StringBuilder GetSqlSearchForCheckUserInObjects(XmlDocument xmlObjDoc,
			string strUserColName,
			string strObjColName,
			string strDirect,
			string strExtraWhere)
		{
			StringBuilder strBuilder = new StringBuilder(1024);

			string strUserList = string.Empty, strOrganizationList = string.Empty, strGroupList = string.Empty;

			foreach (XmlElement elem in xmlObjDoc.DocumentElement.ChildNodes)
			{
				string strRankCode = elem.GetAttribute("rankCode");//如果有行政级别的限制

				switch (elem.LocalName)
				{
					case "ORGANIZATIONS":
						if (strRankCode != null && strRankCode.Length > 0)//有“行政级别”的限制
						{
							strBuilder.Append(@"
							SELECT DISTINCT 'ORGANIZATIONS' AS OBJECTCLASS, " + TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + " AS RANK_CODE, "
									+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
									+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS") + @" AS OBJ_VALUE
							FROM ORGANIZATIONS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
									AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
									+ TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + @") 
							WHERE OU_USERS.USER_GUID = USERS.GUID
								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + "
								+ TSqlBuilder.Instance.CheckQuotationMark(strDirect, true) + @"
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS") + " = "
								+ TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
							ORDER BY OBJ_VALUE;" + "\n");
						}
						else
						{
							if (elem.GetAttribute("oValue").Trim().Length > 0)
							{
								if (strOrganizationList.Length > 0)
									strOrganizationList += ",";
								strOrganizationList += TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true);
							}
						}
						break;
					case "GROUPS":
						if (strRankCode != null && strRankCode.Length > 0)
						{
							strBuilder.Append(@"
							SELECT DISTINCT 'GROUPS' AS OBJECTCLASS, "
								+ TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + " AS RANK_CODE, "
								+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
								+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + @" AS OBJ_VALUE
							FROM GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
									AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
								+ TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + @")
							WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
								AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
								AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
								AND OU_USERS.USER_GUID = USERS.GUID
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + "="
								+ TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
							ORDER BY OBJ_VALUE;" + "\n");
						}
						else
						{
							if (elem.GetAttribute("oValue").Trim().Length > 0)
							{
								if (strGroupList.Length > 0)
									strGroupList += ",";
								strGroupList += TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true);
							}
						}
						break;
					case "USERS":
						if (strRankCode != null && strRankCode.Length > 0)
						{
							strBuilder.Append(@"
							SELECT DISTINCT 'USERS' AS OBJECTCLASS, " + TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + " AS RANK_CODE, "
								+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
								+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "USERS", "OU_USERS") + @" AS OBJ_VALUE
							FROM OU_USERS, USERS
							WHERE OU_USERS.USER_GUID = USERS.GUID
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "USERS", "OU_USERS") + "="
								+ TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
								+ TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + @")
							ORDER BY OBJ_VALUE;" + "\n");
						}
						else
						{
							if (elem.GetAttribute("oValue").Trim().Length > 0)
							{
								if (strUserList.Length > 0)
									strUserList += ",";
								strUserList += TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true);
							}
						}
						break;
					default: ExceptionHelper.TrueThrow(true, "对不起,系统没有对应处理“" + elem.LocalName + "”的相应程序！");
						break;
				}
			}

			if (strOrganizationList.Length > 0)
			{
				strBuilder.Append(@"
							SELECT DISTINCT 'ORGANIZATIONS' AS OBJECTCLASS, NULL AS RANK_CODE, "
								+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
								+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS") + @" AS OBJ_VALUE
							FROM ORGANIZATIONS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
							WHERE OU_USERS.USER_GUID = USERS.GUID
								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + "
								+ TSqlBuilder.Instance.CheckQuotationMark(strDirect, true) + @"
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS")
									  + " IN ( " + strOrganizationList + @" )
							ORDER BY OBJ_VALUE;" + "\n");
			}

			if (strGroupList.Length > 0)
			{
				strBuilder.Append(@"
							SELECT DISTINCT 'GROUPS' AS OBJECTCLASS, NULL AS RANK_CODE, "
								+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
								+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + @" AS OBJ_VALUE
							FROM GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
							WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
								AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
								AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
								AND OU_USERS.USER_GUID = USERS.GUID
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + " IN ( " + strGroupList + @" )
							ORDER BY OBJ_VALUE;" + "\n");
			}

			if (strUserList.Length > 0)
			{
				strBuilder.Append(@"
							SELECT DISTINCT 'USERS' AS OBJECTCLASS, NULL AS RANK_CODE, "
								+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
								+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "USERS", "OU_USERS") + @" AS OBJ_VALUE
							FROM OU_USERS, USERS
							WHERE OU_USERS.USER_GUID = USERS.GUID
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "USERS", "OU_USERS")
									  + " IN ( " + strUserList + @" )
							ORDER BY OBJ_VALUE;" + "\n");
			}

			return strBuilder;
		}
		#endregion
	}
}
