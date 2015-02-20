using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.OguAdmin;

namespace MCS.Library.Accredit
{
	internal partial class OguDBOperation
	{

		private void eventContainer_BeforeDelUsersFromGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;

			string strGroupGuid = root.GetAttribute("GUID");
			ExceptionHelper.TrueThrow(strGroupGuid == string.Empty, "未指定用户组对象！");

			string strSql = "UPDATE GROUPS SET MODIFY_TIME = GETDATE() WHERE GUID = {0}";

			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strGroupGuid, true));
			InnerCommon.ExecuteNonQuery(strSql);

			StringBuilder builder = new StringBuilder(1024);
			foreach (XmlElement elem in root.ChildNodes)
			{
				ExceptionHelper.TrueThrow(elem.LocalName != "USERS", "对不起，选项中存在非用户对象！");

				string strUserGuid = elem.GetAttribute("GUID");
				string strOrgGuid = elem.GetAttribute("PARENT_GUID");

				ExceptionHelper.TrueThrow(strUserGuid == string.Empty || strOrgGuid == string.Empty, "存在未确定的用户对象！");

				strSql = @"
					DELETE FROM GROUP_USERS
					WHERE GROUP_GUID = {0}
						AND USER_GUID = {1}
						AND USER_PARENT_GUID = {2}
					";
				strSql = string.Format(strSql,
					TSqlBuilder.Instance.CheckQuotationMark(strGroupGuid, true),
					TSqlBuilder.Instance.CheckQuotationMark(strUserGuid, true),
					TSqlBuilder.Instance.CheckQuotationMark(strOrgGuid, true));
				builder.Append(strSql + Environment.NewLine);
			}
			context.Add("Sql", builder.ToString());
		}

		private void eventContainer_DelUsersFromGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			InnerCommon.ExecuteNonQuery(context["Sql"].ToString());
		}

		private void eventContainer_BeforeLogicDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			PrepareLogicDeleteOrganizations(xmlDoc, context);
			PrepareLogicDeleteUsers(xmlDoc, context);
			PrepareLogicDeleteGroups(xmlDoc, context);
		}

		private void eventContainer_LogicDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			DeleteObjects(xmlDoc, context);
		}

		private void eventContainer_BeforeFurbishDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			PrepareFurbishDeleteOrganizations(xmlDoc, context);
			PrepareFurbishDeleteUsers(xmlDoc, context);
			PrepareFurbishDeleteGroups(xmlDoc, context);
		}

		private void eventContainer_FurbishDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			DeleteObjects(xmlDoc, context);
		}

		private void eventContainer_BeforeRealDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			PrepareRealDeleteOrganizations(xmlDoc, context);
			PrepareRealDeleteUsers(xmlDoc, context);
			PrepareRealDeleteGroups(xmlDoc, context);
		}

		private void eventContainer_RealDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			DeleteObjects(xmlDoc, context);
		}

		#region private

		private void DeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			string strGroupSql = context["GroupSql"].ToString();
			string strUserSql = context["UserSql"].ToString();
			string strOrgSql = context["OrgSql"].ToString();

			if (!string.IsNullOrEmpty(strGroupSql))
				InnerCommon.ExecuteNonQuery(strGroupSql);

			if (!string.IsNullOrEmpty(strUserSql))
				InnerCommon.ExecuteNonQuery(strUserSql);

			if (!string.IsNullOrEmpty(strOrgSql))
				InnerCommon.ExecuteNonQuery(strOrgSql);
		}

		private void PrepareRealDeleteOrganizations(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			XmlNodeList xOrgNodes = root.SelectNodes("ORGANIZATIONS");

			StringBuilder strB = new StringBuilder(1024);

			foreach (XmlElement elem in xOrgNodes)
			{
				string strOriginalSort = elem.GetAttribute("ORIGINAL_SORT");
				string strSql = @"
					DELETE USERS 
					FROM OU_USERS 
					WHERE USERS.GUID = OU_USERS.USER_GUID 
						AND OU_USERS.SIDELINE = '0'
						AND OU_USERS.ORIGINAL_SORT LIKE {0} + '%' ; 
					DELETE ORGANIZATIONS WHERE ORIGINAL_SORT = {0}; ";

				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strOriginalSort, true));
				strB.Append(strSql);
			}

			strB.Append(@"DELETE FROM OU_USERS WHERE PARENT_GUID NOT IN (SELECT GUID FROM ORGANIZATIONS);
						DELETE FROM GROUP_USERS WHERE USER_PARENT_GUID NOT IN (SELECT GUID FROM ORGANIZATIONS);
						DELETE FROM USERS WHERE GUID NOT IN (SELECT USER_GUID FROM OU_USERS);");

			context.Add("OrgSql", strB.ToString());
		}

		private void PrepareRealDeleteUsers(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			XmlNodeList xUserNodes = root.SelectNodes("USERS");

			StringBuilder strB = new StringBuilder(1024);

			foreach (XmlElement elem in xUserNodes)
			{
				string strUserGuid = elem.GetAttribute("GUID");
				string strUserSideline = elem.GetAttribute("SIDELINE");
				string strParentGuid = elem.GetAttribute("PARENT_GUID");
				//默认兼职数据处理
				string strSql = "DELETE FROM OU_USERS WHERE USER_GUID = {0} AND PARENT_GUID = {1} ;\n";

				if (strUserSideline == "0")//主要职务
					strSql += "DELETE FROM USERS WHERE GUID = {0} ;\n";
				// 以下3项有trig自动完成
				//						+ "DELETE FROM OU_USERS WHERE USER_GUID = {0} ;\n";
				//						+ "DELETE FROM GROUP_USERS WHERE USER_GUID = {0} ;\n"
				//						+ "DELETE FROM SECRETARIES WHERE LEADER_GUID = {0} OR SECRETARY_GUID = {0};\n";

				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strUserGuid, true),
					TSqlBuilder.Instance.CheckQuotationMark(strParentGuid, true));

				strB.Append(strSql);
			}
			context.Add("UserSql", strB.ToString());

		}

		private void PrepareRealDeleteGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			XmlNodeList xGroupNodes = root.SelectNodes("GROUPS");

			StringBuilder strB = new StringBuilder(1024);

			foreach (XmlElement elem in xGroupNodes)
			{
				string strGroupGuid = elem.GetAttribute("GUID");
				string strSql = "DELETE FROM GROUPS WHERE GUID = {0} ;\n";
				// 下面由trig自动完成
				//					+ "DELETE FROM GROUP_USERS WHERE GROUP_GUID = {0} ;\n";

				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strGroupGuid, true));
				strB.Append(strSql);
			}
			context.Add("GroupSql", strB.ToString());
		}

		private void PrepareFurbishDeleteOrganizations(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			XmlNodeList xOrgNodes = root.SelectNodes("ORGANIZATIONS");

			StringBuilder strB = new StringBuilder(1024);

			foreach (XmlElement ele in xOrgNodes)
			{
				string strOriginalSort = ele.GetAttribute("ORIGINAL_SORT");
				ExceptionHelper.TrueThrow(strOriginalSort.Length == 0, "对不起，没有确定的要求被恢复对象（" + ele.OuterXml + "）");

				string strOrgsIn = " SELECT ORIGINAL_SORT FROM ORGANIZATIONS WHERE (STATUS & 2) <> 0 AND ORIGINAL_SORT LIKE "
					+ TSqlBuilder.Instance.CheckQuotationMark(strOriginalSort, true) + " + '_%'";
				string strGroupsIn = " SELECT ORIGINAL_SORT FROM GROUPS WHERE (STATUS & 2) <> 0 AND ORIGINAL_SORT LIKE "
					+ TSqlBuilder.Instance.CheckQuotationMark(strOriginalSort, true) + " + '_%'";
				string strUsersIn = " SELECT ORIGINAL_SORT FROM OU_USERS WHERE (STATUS & 2) <> 0 AND ORIGINAL_SORT LIKE "
					+ TSqlBuilder.Instance.CheckQuotationMark(strOriginalSort, true) + " + '_%'";

				// 注：11 = 15 - 4
				string strSql = @"
					UPDATE ORGANIZATIONS 
						SET STATUS = 1, MODIFY_TIME = GETDATE() 
					WHERE ORIGINAL_SORT = {0};
				
					UPDATE ORGANIZATIONS 
						SET STATUS = (STATUS & 11), MODIFY_TIME = GETDATE()
					WHERE ORGANIZATIONS.ORIGINAL_SORT NOT IN	(	SELECT ORGANIZATIONS.ORIGINAL_SORT 
																	FROM ORGANIZATIONS, ({1}) DIR_ORG
																	WHERE ORGANIZATIONS.ORIGINAL_SORT LIKE DIR_ORG.ORIGINAL_SORT + '%'
																)
						AND ORGANIZATIONS.ORIGINAL_SORT LIKE {0} + '_%';

					UPDATE GROUPS 
						SET STATUS = (STATUS & 11), MODIFY_TIME = GETDATE() 
					WHERE GROUPS.ORIGINAL_SORT NOT IN	(	SELECT GROUPS.ORIGINAL_SORT 
															FROM GROUPS, ({1}) DIR_ORG
															WHERE GROUPS.ORIGINAL_SORT LIKE DIR_ORG.ORIGINAL_SORT + '%'
														)
						AND GROUPS.ORIGINAL_SORT NOT IN ({2})
						AND GROUPS.ORIGINAL_SORT LIKE {0} + '_%';

					UPDATE OU_USERS 
						SET STATUS = (STATUS & 11), MODIFY_TIME = GETDATE() 
					WHERE OU_USERS.ORIGINAL_SORT NOT IN (	SELECT OU_USERS.ORIGINAL_SORT 
															FROM OU_USERS, ({1}) DIR_ORG
															WHERE OU_USERS.ORIGINAL_SORT LIKE DIR_ORG.ORIGINAL_SORT + '%'
														)
						AND OU_USERS.ORIGINAL_SORT NOT IN ({3})
						AND OU_USERS.ORIGINAL_SORT LIKE {0} + '_%';";

				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strOriginalSort, true), strOrgsIn, strGroupsIn, strUsersIn);

				strB.Append(strSql);
			}
			context.Add("OrgSql", strB.ToString());
		}

		private void PrepareFurbishDeleteUsers(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			XmlNodeList xUserNodes = root.SelectNodes("USERS");

			StringBuilder strB = new StringBuilder(1024);
			foreach (XmlElement ele in xUserNodes)
			{
				string strUserGuid = ele.GetAttribute("GUID");
				string strParentGuid = ele.GetAttribute("PARENT_GUID");
				ExceptionHelper.TrueThrow(strParentGuid.Length == 0 || strUserGuid.Length == 0, "对不起，没有确定的要求被恢复对象（" + ele.OuterXml + "）");
				string strSql = @" 
					UPDATE OU_USERS 
						SET STATUS = 1, MODIFY_TIME = GETDATE()  
					WHERE USER_GUID = {0}
						AND PARENT_GUID = {1};";

				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strUserGuid, true),
					TSqlBuilder.Instance.CheckQuotationMark(strParentGuid, true));

				strB.Append(strSql);
			}
			context.Add("UserSql", strB.ToString());

		}

		private void PrepareFurbishDeleteGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			XmlNodeList xGroupNodes = root.SelectNodes("GROUPS");

			StringBuilder strB = new StringBuilder(1024);
			foreach (XmlElement ele in xGroupNodes)
			{
				string strGuid = ele.GetAttribute("GUID");
				ExceptionHelper.TrueThrow(strGuid.Length == 0, "对不起，没有确定的要求被恢复对象（" + ele.OuterXml + "）");
				string strSql = @" 
					UPDATE GROUPS 
						SET STATUS = 1, MODIFY_TIME = GETDATE()
					WHERE GUID = {0};";

				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strGuid, true));

				strB.Append(strSql);
			}
			context.Add("GroupSql", strB.ToString());
		}

		private void PrepareLogicDeleteGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			XmlNodeList xGroupNodes = root.SelectNodes("GROUPS");

			StringBuilder strB = new StringBuilder(1024);
			foreach (XmlElement ele in xGroupNodes)
			{
				string strGuid = ele.GetAttribute("GUID");
				ExceptionHelper.TrueThrow(strGuid.Length == 0, "对不起，没有确定的被删除对象（" + ele.OuterXml + "）");

				string strSql = @"
					UPDATE GROUPS 
						SET STATUS = (STATUS | 2), MODIFY_TIME = GETDATE() 
					WHERE GUID = {0};";
				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strGuid, true));
				strB.Append(strSql);
			}

			context.Add("GroupSql", strB.ToString());
		}

		private void PrepareLogicDeleteUsers(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			XmlNodeList xUserNodes = root.SelectNodes("USERS");

			StringBuilder strB = new StringBuilder(1024);
			foreach (XmlElement ele in xUserNodes)
			{
				string strUserGuid = ele.GetAttribute("GUID");
				string strParentGuid = ele.GetAttribute("PARENT_GUID");
				ExceptionHelper.TrueThrow(strParentGuid.Length == 0 || strUserGuid.Length == 0, "对不起，没有确定的被删除对象（" + ele.OuterXml + "）");
				string strSql = @"
					UPDATE OU_USERS 
						SET STATUS = (STATUS | 2), MODIFY_TIME = GETDATE() 
					WHERE USER_GUID = {0}
						AND PARENT_GUID = {1};";
				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strUserGuid, true),
					TSqlBuilder.Instance.CheckQuotationMark(strParentGuid, true));
				strB.Append(strSql);

			}
			context.Add("UserSql", strB.ToString());
		}

		private void PrepareLogicDeleteOrganizations(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			XmlNodeList xOrgNodes = root.SelectNodes("ORGANIZATIONS");

			StringBuilder strB = new StringBuilder(1024);
			foreach (XmlElement ele in xOrgNodes)
			{
				string strOriginalSort = ele.GetAttribute("ORIGINAL_SORT");
				ExceptionHelper.TrueThrow(strOriginalSort.Length == 0, "对不起，没有确定的被删除对象（" + ele.OuterXml + "）");

				string strSql = @"
					UPDATE ORGANIZATIONS 
						SET STATUS = (STATUS | 2), MODIFY_TIME = GETDATE() 
					WHERE ORIGINAL_SORT = {0};
					
					UPDATE ORGANIZATIONS 
						SET STATUS = (STATUS | 4), MODIFY_TIME = GETDATE() 
					WHERE ORIGINAL_SORT LIKE {0} + '_%';
				
					UPDATE GROUPS 
						SET STATUS = (STATUS | 4), MODIFY_TIME = GETDATE() 
					WHERE ORIGINAL_SORT LIKE {0} + '_%';

					UPDATE OU_USERS 
						SET STATUS = (STATUS | 4), MODIFY_TIME = GETDATE()  
					WHERE ORIGINAL_SORT LIKE {0} + '_%';";

				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strOriginalSort, true));
				strB.Append(strSql);
			}
			context.Add("OrgSql", strB.ToString());
		}

		#endregion
	}
}
