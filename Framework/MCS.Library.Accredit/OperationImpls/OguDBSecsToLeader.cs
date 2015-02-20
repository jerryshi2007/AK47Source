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
		private void eventContainer_BeforeDelSecsOfLeader(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;

			string strLeaderGuid = root.GetAttribute("GUID");
			ExceptionHelper.TrueThrow(strLeaderGuid == string.Empty, "未指定领导对象！");

			StringBuilder builder = new StringBuilder(1024);
			foreach (XmlElement elem in root.ChildNodes)
			{
				ExceptionHelper.TrueThrow(elem.LocalName != "USERS", "对不起，选项中存在非用户对象！");

				string strSecGuid = elem.GetAttribute("GUID");

				ExceptionHelper.TrueThrow(strSecGuid == string.Empty, "存在未确定的用户对象！");

				string strSql = @"
					DELETE FROM SECRETARIES
					WHERE LEADER_GUID = {0}
						AND SECRETARY_GUID = {1}
					";
				strSql = string.Format(strSql,
					TSqlBuilder.Instance.CheckQuotationMark(strLeaderGuid, true),
					TSqlBuilder.Instance.CheckQuotationMark(strSecGuid, true));

				builder.Append(strSql + Environment.NewLine);
			}

			context.Add("Sql", builder.ToString());

		}

		private void eventContainer_DelSecsOfLeader(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			InnerCommon.ExecuteNonQuery(context["Sql"].ToString());
		}

		private void eventContainer_BeforeSetSecsToLeader(XmlDocument xmlDoc, Dictionary<object, object> context)
		{ 
		}

		private XmlDocument eventContainer_SetSecsToLeader(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlDocument result;

			XmlElement root = xmlDoc.DocumentElement;
			string strLeaderGuid = root.GetAttribute("GUID");
			string strStartTime = root.GetAttribute("START_TIME");
			string strEndTime = root.GetAttribute("END_TIME");
			string strAccessLevel = root.GetAttribute("USER_ACCESS_LEVEL");

			ExceptionHelper.TrueThrow(strLeaderGuid == string.Empty, "未指定用户组对象！");

			foreach (XmlElement elem in root.ChildNodes)
			{
				switch (elem.GetAttribute("OBJECTCLASS"))
				{
					case "ORGANIZATIONS":
						string strOrgGuid = elem.GetAttribute("GUID");
						DataSet ods = OGUReader.GetOrganizationChildren(strOrgGuid,
							SearchObjectColumn.SEARCH_GUID,
							(int)ListObjectType.ALL_TYPE,
							(int)ListObjectDelete.COMMON,
							0,
							string.Empty,
							strAccessLevel,
							string.Empty,
							OGUCommonDefine.CombinateAttr("USER_GUID"));
						SetOrgOrGroupSecretary(ods, strLeaderGuid, strStartTime, strEndTime);
						break;
					case "GROUPS":
						string strOldGroupGuid = elem.GetAttribute("GUID");
						DataSet gds = OGUReader.GetUsersInGroups(strOldGroupGuid,
							SearchObjectColumn.SEARCH_GUID,
							OGUCommonDefine.CombinateAttr("USER_GUID"),
							string.Empty,
							SearchObjectColumn.SEARCH_NULL,
							string.Empty,
							(int)ListObjectDelete.COMMON);
						SetOrgOrGroupSecretary(gds, strLeaderGuid, strStartTime, strEndTime);
						break;
					case "USERS":
						SetUserSecretaries(elem, strLeaderGuid, strStartTime, strEndTime);
						break;
					default:
						ExceptionHelper.TrueThrow(true, "对不起，系统没有与“" + elem.GetAttribute("OBJECTCLASS") + "”对象相应的数据处理！");
						break;
				}
			}
			OGUReader.RemoveAllCache();//由于这里需要回传数据，所以这里的缓存处理提前
			DataSet ds = OGUReader.GetSecretariesOfLeaders(strLeaderGuid,
				SearchObjectColumn.SEARCH_GUID,
				OGUCommonDefine.CombinateAttr(root.GetAttribute("extAttr")),
				(int)ListObjectDelete.COMMON);

			result = this.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");

			return result;
		}


		#region private

		/// <summary>
		/// 把指定用户对象作为指定领导的秘书
		/// </summary>
		/// <param name="elem"></param>
		/// <param name="strLeaderGuid"></param>
		/// <param name="strStartTime"></param>
		/// <param name="strEndTime"></param>
		private static void SetUserSecretaries(XmlElement elem, string strLeaderGuid, string strStartTime, string strEndTime)
		{
			ExceptionHelper.TrueThrow(elem.GetAttribute("GUID") == strLeaderGuid, "对不起，自己不能给自己做秘书！");
			string strSql = @"
				INSERT INTO SECRETARIES (LEADER_GUID, SECRETARY_GUID, START_TIME, END_TIME)
				VALUES
				({0}, {1}, {2}, {3})
				";

			strSql = string.Format(strSql,
				TSqlBuilder.Instance.CheckQuotationMark(strLeaderGuid, true),
				TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("GUID"), true),
				TSqlBuilder.Instance.CheckQuotationMark(strStartTime, true),
				TSqlBuilder.Instance.CheckQuotationMark(strEndTime, true));
			try
			{
				InnerCommon.ExecuteNonQuery(strSql);
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				if (ex.Number != 2627)//数据重复
					throw ex;
			}
		}

		/// <summary>
		/// 把指定用户组（或机构）中的用户对象作为指定领导的秘书
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="strLeaderGuid"></param>
		/// <param name="strStartTime"></param>
		/// <param name="strEndTime"></param>
		private static void SetOrgOrGroupSecretary(DataSet ds, string strLeaderGuid, string strStartTime, string strEndTime)
		{
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				if (OGUCommonDefine.DBValueToString(row["OBJECTCLASS"]) == "USERS" && OGUCommonDefine.DBValueToString(row["USER_GUID"]) != strLeaderGuid)
				{
					string strSql = @"
						INSERT INTO SECRETARIES (LEADER_GUID, SECRETARY_GUID, START_TIME, END_TIME)
						VALUES
						({0}, {1}, {2}, {3})
						";

					strSql = string.Format(strSql,
						TSqlBuilder.Instance.CheckQuotationMark(strLeaderGuid, true),
						TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(row["USER_GUID"]), true),
						TSqlBuilder.Instance.CheckQuotationMark(strStartTime, true),
						TSqlBuilder.Instance.CheckQuotationMark(strEndTime, true));

					try
					{
						InnerCommon.ExecuteNonQuery(strSql);
					}
					catch (System.Data.SqlClient.SqlException ex)
					{
						if (ex.Number != 2627)//数据重复
							throw ex;
					}
				}
			}
		}

		#endregion

	}
}
