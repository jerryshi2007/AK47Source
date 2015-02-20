using System;
using System.Xml;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Transactions;

using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.Accredit.Properties;
using MCS.Library.Accredit.Configuration;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// OGUCommonDefine 的摘要说明。
	/// </summary>
	public class OGUCommonDefine
	{
		#region 构造函数
		/// <summary>
		/// 构造函数
		/// </summary>
		public OGUCommonDefine()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region Public Const Define 【Deleted】
		///// <summary>
		///// 授权系统中的数据库联接配置对应数据
		///// </summary>
		//public const string STR_CONN = "AccreditAdmin";
		///// <summary>
		///// 用于设置排序上的区别（6位一节方式）
		///// </summary>
		//public const string OGU_ORIGINAL_SORT = "000000";
		///// <summary>
		///// 机构人员管理系统的注册名称
		///// </summary>
		//public const string OGU_ADMIN_APP_NAME = "OGU_ADMIN";
		#endregion

		#region 系统权限的定义 【Deleted】
		///// <summary>
		///// 创建新机构
		///// </summary>
		//public const string PM_CREATE_ORGANIZATIONS = "CREATE_ORGANIZATIONS";
		///// <summary>
		///// 创建新用户
		///// </summary>
		//public const string PM_CREATE_USERS = "CREATE_USERS";
		///// <summary>
		///// 创建新用户组
		///// </summary>
		//public const string PM_CREATE_GROUPS = "CREATE_GROUPS";
		///// <summary>
		///// 设置用户兼职
		///// </summary>
		//public const string PM_SET_SIDELINE = "SET_SIDELINE";
		///// <summary>
		///// 修改机构属性
		///// </summary>
		//public const string PM_UPDATE_ORGANIZATIONS = "UPDATE_ORGANIZATIONS";
		///// <summary>
		///// 修改用户属性
		///// </summary>
		//public const string PM_UPDATE_USERS = "UPDATE_USERS";
		///// <summary>
		///// 修改人员组属性
		///// </summary>
		//public const string PM_UPDATE_GROUPS = "UPDATE_GROUPS";
		///// <summary>
		///// 逻辑删除机构
		///// </summary>
		//public const string PM_LOGIC_DELETE_ORGANIZATIONS = "LOGIC_DELETE_ORGANIZATIONS";
		///// <summary>
		///// 逻辑删除用户
		///// </summary>
		//public const string PM_LOGIC_DELETE_USERS = "LOGIC_DELETE_USERS";
		///// <summary>
		///// 逻辑删除用户组
		///// </summary>
		//public const string PM_LOGIC_DELETE_GROUPS = "LOGIC_DELETE_GROUPS";
		///// <summary>
		///// 恢复被逻辑删除机构
		///// </summary>
		//public const string PM_FURBISH_DELETE_ORGANIZATIONS = "FURBISH_DELETE_ORGANIZATIONS";
		///// <summary>
		///// 恢复被逻辑删除用户
		///// </summary>
		//public const string PM_FURBISH_DELETE_USERS = "FURBISH_DELETE_USERS";
		///// <summary>
		///// 恢复被逻辑删除用户组
		///// </summary>
		//public const string PM_FURBISH_DELETE_GROUPS = "FURBISH_DELETE_GROUPS";
		///// <summary>
		///// 机构内对象排序
		///// </summary>
		//public const string PM_SORT_IN_ORGANIZATIONS = "SORT_IN_ORGANIZATIONS";
		///// <summary>
		///// 人员组中人员排序
		///// </summary>
		//public const string PM_SORT_IN_GROUP = "SORT_IN_GROUP";
		///// <summary>
		///// 初始化用户口令
		///// </summary>
		//public const string PM_INIT_USERS_PWD = "INIT_USERS_PWD";
		///// <summary>
		///// 物理删除机构
		///// </summary>
		//public const string PM_REAL_DELETE_ORGANIZATIONS = "REAL_DELETE_ORGANIZATIONS";
		///// <summary>
		///// 物理删除人员组
		///// </summary>
		//public const string PM_REAL_DELETE_GROUPS = "REAL_DELETE_GROUPS";
		///// <summary>
		///// 物理删除用户
		///// </summary>
		//public const string PM_REAL_DELETE_USERS = "REAL_DELETE_USERS";
		///// <summary>
		///// 人员组中增加成员
		///// </summary>
		//public const string PM_GROUP_ADD_USERS = "GROUP_ADD_USERS";
		///// <summary>
		///// 人员组中删除成员
		///// </summary>
		//public const string PM_GROUP_DEL_USERS = "GROUP_DEL_USERS";
		///// <summary>
		///// 设置秘书
		///// </summary>
		//public const string PM_SECRETARY_ADD = "SECRETARY_ADD";
		///// <summary>
		///// 删除秘书
		///// </summary>
		//public const string PM_SECRETARY_DEL = "SECRETARY_DEL";

		#endregion

		#region DataBase Tables 【Deleted】
		///// <summary>
		///// 机构数据表名称
		///// </summary>
		//public const string DB_TABLE_ORGANIZATIONS = "ORGANIZATIONS";
		///// <summary>
		///// 用户信息表
		///// </summary>
		//public const string DB_TABLE_USERS = "USERS";
		///// <summary>
		///// 用户组表
		///// </summary>
		//public const string DB_TABLE_GROUPS = "GROUPS";
		///// <summary>
		///// 用户与机构之间的关系表
		///// </summary>
		//public const string DB_TABLE_OU_USERS = "OU_USERS";
		///// <summary>
		///// 用户与人员组之间的关系表
		///// </summary>
		//public const string DB_TABLE_GROUP_USERS = "GROUP_USERS";
		///// <summary>
		///// 领导与秘书的关系表
		///// </summary>
		//public const string DB_TABLE_SECRETARIES = "SECRETARIES";
		///// <summary>
		///// 系统定义的人员行政级别信息表
		///// </summary>
		//public const string DB_TABLE_RANK_DEFINE = "RANK_DEFINE";
		///// <summary>
		///// 系统中使用的密码类型表
		///// </summary>
		//public const string DB_TABLE_PWD_ARITHMETIC = "PWD_ARITHMETIC";
		#endregion

		#region public function
		/// <summary>
		/// 自动完成SQL的执行过程
		/// </summary>
		/// <param name="strSql">要求执行的数据查询SQL</param>
		/// <returns>本次操作所影响的数据条数</returns>
		internal static int ExecuteNonQuery(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteNonQueryWithoutTransaction--strSql");
			using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
#if DEBUG
				Debug.WriteLine(strSql);
#endif
				ExceptionHelper.TrueThrow(string.IsNullOrEmpty(strSql), "数据处理语句SQL不能为空串！");

				return database.ExecuteNonQuery(CommandType.Text, strSql);
			}
		}

		/// <summary>
		/// 自动完成SQL的执行过程
		/// </summary>
		/// <param name="strSql">要求执行的数据查询SQL</param>
		/// <param name="strTables">要求配置的数据表名称</param>
		/// <returns>本次查询的数据结果集</returns>
		internal static DataSet ExecuteDataset(string strSql, params string[] strTables)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteDatasetWithoutTransaction--strSql");
#if DEBUG
			Debug.WriteLine(strSql);
#endif
			using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteDataSet(CommandType.Text, strSql, strTables);
			}
		}

		/// <summary>
		/// 自动完成SQL的执行过程
		/// </summary>
		/// <param name="strSql">要求执行的数据查询SQL</param>
		/// <returns>本次查询的结果对象</returns>
		internal static object ExecuteScalar(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteScalar--strSql");
#if DEBUG
			Debug.WriteLine(strSql);
#endif
			using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteScalar(CommandType.Text, strSql);
			}
		}

		/// <summary>
		/// 把系统中要求查询数据的默认属性与特别要求属性合并
		/// </summary>
		/// <param name="strAttrs"></param>
		/// <returns></returns>
		public static string CombinateAttr(string strAttrs)
		{
			string[] strOriginalAttrs = {"GUID", "PARENT_GUID", "LOGON_NAME", "OBJ_NAME", "DISPLAY_NAME", "ORIGINAL_SORT",
										"GLOBAL_SORT", "ALL_PATH_NAME", "STATUS", "RANK_NAME", "POSTURAL", "SIDELINE", 
										"CUSTOMS_CODE", "PERSON_ID"};

			for (int i = 0; i < strOriginalAttrs.Length; i++)
			{
				if (strAttrs.IndexOf(strOriginalAttrs[i]) >= 0)
					continue;

				if (strAttrs.Length > 0)
					strAttrs = strOriginalAttrs[i] + ", " + strAttrs;
				else
					strAttrs = strOriginalAttrs[i];
			}

			return TSqlBuilder.Instance.CheckQuotationMark(strAttrs, false);
		}     



		/// <summary>
		/// 结合系统中的隐藏配置，生成对系统中针对数据查询的条件
		/// </summary>
		/// <param name="strOriginalHideType">原始的系统配置</param>
		/// <returns>结合系统中的隐藏配置，生成对系统中针对数据查询的条件</returns>
		internal static string GetHideType(string strOriginalHideType)
		{
			string strResult = strOriginalHideType;

			//string strAutoHide = (new SysConfig()).GetDataFromConfig("AutohideType", string.Empty);
			string strAutoHide = AccreditSection.GetConfig().AccreditSettings.AutohideType;

			if (strOriginalHideType.Length == 0)
				strResult = strAutoHide;
			else
			{
				if (strAutoHide.Length > 0)
				{
					string[] strArrs = strOriginalHideType.Split(',', ' ', ';');
					string[] strAutoArrs = strAutoHide.Split(',', ' ', ';');

					for (int i = 0; i < strAutoArrs.Length; i++)
					{
						bool bAddAuto = true;

						for (int j = 0; j < strArrs.Length; j++)
						{
							if (strArrs[j] == strAutoArrs[i])
								bAddAuto = false;
						}

						if (bAddAuto)
							strResult += "," + strAutoArrs[i];
					}
				}
			}

			return strResult;
		}

		/// <summary>
		/// 获取查询的数据字段名称
		/// </summary>
		/// <param name="soc">数据查询条件类型（对应于字段名称）</param>
		/// <returns>对应查询的数据结果（对应字段名称）</returns>
		internal static string GetSearchObjectColumn(SearchObjectColumn soc)
		{
			string strResult = string.Empty;
			switch (soc)
			{
				case SearchObjectColumn.SEARCH_GUID: strResult = "GUID";
					break;
				case SearchObjectColumn.SEARCH_USER_GUID: strResult = "USER_GUID";
					break;
				case SearchObjectColumn.SEARCH_ORIGINAL_SORT: strResult = "ORIGINAL_SORT";
					break;
				case SearchObjectColumn.SEARCH_GLOBAL_SORT: strResult = "GLOBAL_SORT";
					break;
				case SearchObjectColumn.SEARCH_ALL_PATH_NAME: strResult = "ALL_PATH_NAME";
					break;
				case SearchObjectColumn.SEARCH_LOGON_NAME: strResult = "LOGON_NAME";
					break;
				case SearchObjectColumn.SEARCH_PERSON_ID: strResult = "PERSON_ID";
					break;
				case SearchObjectColumn.SEARCH_IC_CARD: strResult = "IC_CARD";
					break;
				case SearchObjectColumn.SEARCH_CUSTOMS_CODE: strResult = "CUSTOMS_CODE";
					break;
				case SearchObjectColumn.SEARCH_SYSDISTINCT1: strResult = "SYSDISTINCT1";
					break;
				case SearchObjectColumn.SEARCH_SYSDISTINCT2: strResult = "SYSDISTINCT2";
					break;
				case SearchObjectColumn.SEARCH_OUSYSDISTINCT1: strResult = "SYSOUDISTINCT1";
					break;
				case SearchObjectColumn.SEARCH_OUSYSDISTINCT2: strResult = "SYSOUDISTINCT2";
					break;
				//为配合南京海关统一平台切换，新增加字段ID[自增唯一字段]
				case SearchObjectColumn.SEARCH_IDENTITY: strResult = "ID";
					break;
				case SearchObjectColumn.SEARCH_NULL:
				default: ExceptionHelper.TrueThrow<ApplicationException>(true, "对不起，系统不提供该" + soc.ToString() + "查询数据类型！");
					break;
			}
			return strResult;
		}

		/// <summary>
		/// 获取查询的数据字段名称代号
		/// </summary>
		/// <param name="strValueType">对应查询的数据结果（对应字段名称）</param>
		/// <returns>数据查询条件类型（对应于字段名称）</returns>
		public static SearchObjectColumn GetSearchObjectColumn(string strValueType)
		{
			SearchObjectColumn soc = SearchObjectColumn.SEARCH_NULL;
			switch (strValueType)
			{
				case "GUID": soc = SearchObjectColumn.SEARCH_GUID;
					break;
				case "USER_GUID": soc = SearchObjectColumn.SEARCH_USER_GUID;
					break;
				case "ORIGINAL_SORT": soc = SearchObjectColumn.SEARCH_ORIGINAL_SORT;
					break;
				case "GLOBAL_SORT": soc = SearchObjectColumn.SEARCH_GLOBAL_SORT;
					break;
				case "ALL_PATH_NAME": soc = SearchObjectColumn.SEARCH_ALL_PATH_NAME;
					break;
				case "LOGON_NAME": soc = SearchObjectColumn.SEARCH_LOGON_NAME;
					break;
				case "PERSON_ID": soc = SearchObjectColumn.SEARCH_PERSON_ID;
					break;
				case "IC_CARD": soc = SearchObjectColumn.SEARCH_IC_CARD;
					break;
				case "CUSTOMS_CODE": soc = SearchObjectColumn.SEARCH_CUSTOMS_CODE;
					break;
				case "SYSDISTINCT1": soc = SearchObjectColumn.SEARCH_SYSDISTINCT1;
					break;
				case "SYSDISTINCT2": soc = SearchObjectColumn.SEARCH_SYSDISTINCT2;
					break;
				case "SYSOUDISTINCT1": soc = SearchObjectColumn.SEARCH_OUSYSDISTINCT1;
					break;
				case "SYSOUDISTINCT2": soc = SearchObjectColumn.SEARCH_OUSYSDISTINCT2;
					break;
				//为配合南京海关统一平台切换，新增加字段ID[自增唯一字段]
				case "ID":
				case "[ID]": soc = SearchObjectColumn.SEARCH_IDENTITY;
					break;
				default: ExceptionHelper.TrueThrow<ApplicationException>(true, "对不起，系统不提供该" + strValueType + "查询数据类型！");
					break;
			}
			return soc;
		}

		internal static string AddMulitStrWithQuotationMark(string source)
		{
			string[] arrSource = source.Split(',', ';');
			StringBuilder builder = new StringBuilder(128);

			foreach (string temp in arrSource)
			{
				if (false==string.IsNullOrEmpty(temp.Trim()))
				{
					if (builder.Length > 0)
						builder.Append(",");
					builder.Append(TSqlBuilder.Instance.CheckQuotationMark(temp.Trim(), true));
				}
			}

			return builder.Length == 0 ? "''" : builder.ToString();
		}

		/// <summary>
		/// 将数据库字段值转换为字符串
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string DBValueToString(object source)
		{
			string result = string.Empty;

			if (false==(source is DBNull))
			{
				if (source is DateTime)
				{
					if ((DateTime)source != DateTime.MinValue && (DateTime)source != new DateTime(1900, 1, 1, 0, 0, 0, 0))
						result = string.Format("{0:yyyy-MM-dd HH:mm:ss}", source);
				}
				else
					result = source.ToString();
			}

			return result;
		}
		#endregion
	}
}
