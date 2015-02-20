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
using MCS.Library.Expression;

namespace MCS.Library.Accredit.AppAdmin
{
	/// <summary>
	/// 这是沈峥添加了优化权限检查的类，仅仅针对于取用户角色的性能问题
	/// </summary>
	public static class ExtSecurityCheck
	{
		/// <summary>
		/// 解析表达式时的上下文
		/// </summary>
		private class ExpContext
		{
			public DataRow UserRow { get; set; }
			public IList<string> UserDepts { get; set; }
			public IList<string> UserGroups { get; set; }
			public DataRowCollection RankDefine { get; set; }
		}

		/// <summary>
		/// 得到用户在某个应用下的角色
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="appCodeName"></param>
		/// <param name="rightMask"></param>
		/// <returns></returns>
		public static DataSet GetUserAppRoles(
			string userID,
			string appCodeName,
			RightMaskType rightMask)
		{
			IList<string> deptIDs = PrepareUserDepartmentIDs(userID);
			DataTable tableExpression = GetAllExpressionsInApp(appCodeName, rightMask);

			ExpContext context = new ExpContext();

			DataTable userTable = OGUReader.GetObjectsDetail("USERS", userID, SearchObjectColumn.SEARCH_GUID, string.Empty, SearchObjectColumn.SEARCH_NULL, "RANK_CODE").Tables[0];

			ExceptionHelper.FalseThrow(userTable.Rows.Count > 0, "不能找到User ID为{0}的用户信息", userID);

			context.UserRow = userTable.Rows[0];
			context.UserDepts = deptIDs;
			context.RankDefine = OGUReader.GetRankDefine(2).Tables[0].Rows;
			context.UserGroups = PrepareUserGroupIDs(userID);

			return GetUserRolesDS(tableExpression, context);
		}

		private static DataSet GetUserRolesDS(DataTable tableExpression, ExpContext context)
		{
			Dictionary<string, string> roleIDs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			foreach (DataRow row in tableExpression.Rows)
			{
				string roleID = row["ROLE_ID"].ToString();

				if (roleIDs.ContainsKey(roleID) == false)
				{
					string exp = row["EXPRESSION"].ToString();

					if (EvaluateExp(exp, context))
						roleIDs.Add(roleID, roleID);
				}
			}

			List<string> listRoleIDs = new List<string>();

			foreach (KeyValuePair<string, string> kp in roleIDs)
				listRoleIDs.Add(kp.Key);

			return GetRolesByIDs(listRoleIDs.ToArray());
		}

		private static bool EvaluateExp(string exp, ExpContext context)
		{
			return (bool)ExpressionParser.Calculate(exp, new CalculateUserFunction(ExpCalculateUserFunction), context);
		}

		private static object ExpCalculateUserFunction(string funcName, ParamObjectCollection arrParams, object callerContext)
		{
			object result = null;
			ExpContext context = (ExpContext)callerContext;

			switch (funcName.ToLower())
			{
				case "users":
					result = "USERS";
					break;
				case "organizations":
					result = "ORGANIZATIONS";
					break;
				case "groups":
					result = "GROUPS";
					break;
				case "belongto":
					result = CalculateBelongTo((string)arrParams[0].Value, (string)arrParams[1].Value, (string)arrParams[2].Value, context);
					break;
				case "userrank":
					result = CompareUserRank((string)arrParams[0].Value, (string)arrParams[1].Value, context);
					break;
			}

			return result;
		}

		private static bool CompareUserRank(string rankName, string op, ExpContext context)
		{
			bool result = false;

			string currentUserRank = context.UserRow["RANK_CODE"].ToString();
			int currentUserSortID = FindRankSortID(currentUserRank, context.RankDefine);
			int predefinedSortID = FindRankSortID(rankName, context.RankDefine);

			switch (op)
			{
				case "<":
					result = currentUserSortID > predefinedSortID;
					break;
				case "<=":
					result = currentUserSortID >= predefinedSortID;
					break;
				case ">":
					result = currentUserSortID < predefinedSortID;
					break;
				case ">=":
					result = currentUserSortID < predefinedSortID;
					break;
				case "==":
					result = currentUserSortID == predefinedSortID;
					break;
			}

			return result;
		}

		private static int FindRankSortID(string rankCode, DataRowCollection rankRows)
		{
			int result = int.MaxValue;

			foreach (DataRow row in rankRows)
			{
				if (row["CODE_NAME"].ToString() == rankCode)
				{
					result = (int)row["SORT_ID"];
					break;
				}
			}

			return result;
		}

		private static bool CalculateBelongTo(string objType, string objID, string parentID, ExpContext context)
		{
			bool result = false;

			switch (objType)
			{
				case "USERS":
					result = objID == context.UserRow["GUID"].ToString();
					break;
				case "ORGANIZATIONS":
					result = ObjIDInList(objID, context.UserDepts);
					break;
				case "GROUPS":
					result = ObjIDInList(objID, context.UserGroups);
					break;
			}

			return result;
		}

		private static bool ObjIDInList(string objID, IList<string> ids)
		{
			bool result = false;

			foreach (string id in ids)
			{
				if (id == objID)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private static DataSet GetRolesByIDs(string[] roleIDs)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder();
			builder.AppendItem(roleIDs);

			string sql = string.Format("SELECT R.ID, R.APP_ID, R.NAME, R.CODE_NAME, R.DESCRIPTION, R.CLASSIFY, R.SORT_ID, R.ALLOW_DELEGATE, R.INHERITED FROM ROLES R WHERE {0} ORDER BY R.CLASSIFY DESC, R.SORT_ID",
				builder.Count > 0 ? "ID " + builder.ToSqlStringWithInOperator(TSqlBuilder.Instance) : "1 = 0");

			return OGUCommonDefine.ExecuteDataset(sql);
		}

		private static IList<string> PrepareUserGroupIDs(string userID)
		{
			List<string> groupIds = new List<string>();

			DataTable table = OGUReader.GetGroupsOfUsers(userID, SearchObjectColumn.SEARCH_GUID, string.Empty, SearchObjectColumn.SEARCH_GUID, string.Empty).Tables[0];

			foreach (DataRow row in table.Rows)
			{
				if (groupIds.Exists(id => id == row["GUID"].ToString()) == false)
					groupIds.Add(row["GUID"].ToString());
			}

			return groupIds;
		}

		private static IList<string> PrepareUserDepartmentIDs(string userID)
		{
			List<string> deptIds = new List<string>();

			DataTable table = OGUReader.GetObjectsDetail("USERS", userID, SearchObjectColumn.SEARCH_GUID, string.Empty, SearchObjectColumn.SEARCH_NULL, "RANK_CODE").Tables[0];

			return GetUserDepartmentsPath(table);
		}

		/// <summary>
		/// 得到某个应用的所有表达式
		/// </summary>
		/// <param name="appCodeName"></param>
		/// <param name="rightMask"></param>
		/// <returns></returns>
		private static DataTable GetAllExpressionsInApp(string appCodeName, RightMaskType rightMask)
		{
			string sql = string.Format("SELECT E.* FROM APPLICATIONS APP INNER JOIN ROLES R ON APP.ID = R.APP_ID INNER JOIN EXPRESSIONS E ON R.ID = E.ROLE_ID WHERE APP.CODE_NAME = '{0}'",
							appCodeName);

			switch (rightMask)
			{
				case RightMaskType.App:
					sql += " AND R.CLASSIFY = 'n'";
					break;
				case RightMaskType.Self:
					sql += " AND R.CLASSIFY = 'y'";
					break;
			}

			return OGUCommonDefine.ExecuteDataset(sql).Tables[0];
		}

		private static IList<string> GetUserDepartmentsPath(DataTable userTable)
		{
			Dictionary<string, string> allPathDict = new Dictionary<string, string>();

			foreach (DataRow row in userTable.Rows)
			{
				string fullPath = row["ALL_PATH_NAME"].ToString();

				string[] allPath = GetAncestorsFullPath(fullPath);

				foreach (string path in allPath)
					allPathDict[path] = path;
			}

			List<string> result = new List<string>();

			if (allPathDict.Count > 0)
			{
				string ids = GetAllPathString(allPathDict);

				DataTable deptTable = OGUReader.GetObjectsDetail("ORGANIZATIONS", ids, SearchObjectColumn.SEARCH_ALL_PATH_NAME).Tables[0];

				foreach (DataRow row in deptTable.Rows)
					result.Add(row["GUID"].ToString());
			}

			return result;
		}

		private static string GetAllPathString(Dictionary<string, string> allPathDict)
		{
			StringBuilder strB = new StringBuilder();

			foreach (KeyValuePair<string, string> kp in allPathDict)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(kp.Key);
			}

			return strB.ToString();
		}

		private static string[] GetAncestorsFullPath(string fullPath)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(fullPath, "FullPath");

			string[] strParts = fullPath.Split('\\');

			string[] result = new string[strParts.Length - 1];

			for (int i = 0; i < result.Length; i++)
				if (i > 0)
					result[i] = result[i - 1] + "\\" + strParts[i];
				else
					result[i] = strParts[i];

			return result;
		}
	}
}
