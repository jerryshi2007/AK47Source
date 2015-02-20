using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using System.Xml;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.Properties;
using MCS.Library.Data.Builder;
using System.Data;

namespace MCS.Library.Accredit
{
	internal partial class OguDBOperation
	{
		private void eventContainer_BeforeGroupSortObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
		}
		private XmlDocument eventContainer_GroupSortObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlDocument result;

			XmlElement root = xmlDoc.DocumentElement;
			string strGroupGuid = root.GetAttribute("GUID");
			ExceptionHelper.TrueThrow(strGroupGuid == string.Empty, "对不起，没有指定人员组的标识！");

			string strSql = @"
			UPDATE GROUPS
				SET MODIFY_TIME = GETDATE()
			WHERE GUID = {0};";
			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strGroupGuid, true));
			int iPageNo = Convert.ToInt32(root.GetAttribute("PageNo"));
			int iPageSize = Convert.ToInt32(root.GetAttribute("PageSize"));

			StringBuilder builder = new StringBuilder(1024);
			for (int iSort = iPageNo * iPageSize; iSort < root.ChildNodes.Count; iSort++)
			{
				XmlNode elem = root.ChildNodes[iSort];

				string strUserGuid = elem.SelectSingleNode("USER_GUID").InnerText;
				string strOrgGuid = elem.SelectSingleNode("USER_PARENT_GUID").InnerText;
				ExceptionHelper.TrueThrow(strUserGuid == string.Empty || strOrgGuid == string.Empty, "对不起，存在未指定的用户身份标志！");

				strSql = @"
					UPDATE GROUP_USERS
						SET INNER_SORT = {0}, MODIFY_TIME = GETDATE()
					WHERE GROUP_GUID = {1}
						AND USER_GUID = {2}
						AND USER_PARENT_GUID = {3}
				";

				strSql = string.Format(strSql,
					TSqlBuilder.Instance.CheckQuotationMark(iSort.ToString(CommonResource.OriginalSortDefault), true),
					TSqlBuilder.Instance.CheckQuotationMark(strGroupGuid, true),
					TSqlBuilder.Instance.CheckQuotationMark(strUserGuid, true),
					TSqlBuilder.Instance.CheckQuotationMark(strOrgGuid, true));

				builder.Append(strSql + Environment.NewLine);
			}
			InnerCommon.ExecuteNonQuery(strSql);

			DataSet ds = OGUReader.GetUsersInGroups(strGroupGuid,
				SearchObjectColumn.SEARCH_GUID,
				OGUCommonDefine.CombinateAttr(root.GetAttribute("extAttr")),
				string.Empty,
				SearchObjectColumn.SEARCH_NULL,
				string.Empty,
				(int)ListObjectDelete.COMMON);


			result = this.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");

			return result;
		}

		private void eventContainer_BeforeSortObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
		}
		private void eventContainer_SortObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			string strParentGuid = root.GetAttribute("OrgGuid");

			string strSql = @"
				UPDATE ORGANIZATIONS SET MODIFY_TIME = GETDATE() WHERE GUID = {0};
				SELECT GLOBAL_SORT FROM ORGANIZATIONS WHERE GUID = {0};
				";

			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strParentGuid, true));
			object obj = InnerCommon.ExecuteScalar(strSql);
			ExceptionHelper.TrueThrow(obj is DBNull, "对不起，你要排序的父部门不存在！");
			string strParentGlobalSort = obj.ToString();
			int iSort;
			for (iSort = 0; iSort < root.ChildNodes.Count; iSort++)
			{
				XmlNode elem = root.ChildNodes[iSort];
				strSql = SetSortDataSql(elem.LocalName, elem.InnerText, strParentGlobalSort, strParentGuid, iSort);
				InnerCommon.ExecuteNonQuery(strSql);
			}

			strSql = @"
				SELECT * 
				FROM( 
						(SELECT 'USERS' AS OBJECTCLASS, USER_GUID AS GUID, PARENT_GUID, GLOBAL_SORT FROM OU_USERS WHERE STATUS > 1 AND GLOBAL_SORT LIKE {0} + '______') 
							UNION
						(SELECT 'GROUPS' AS OBJECTCLASS, GUID, PARENT_GUID, GLOBAL_SORT FROM GROUPS WHERE STATUS > 1 AND GLOBAL_SORT LIKE {0} + '______')
							UNION
						(SELECT 'ORGANIZATIONS' AS OBJECTCLASS, GUID, PARENT_GUID, GLOBAL_SORT FROM ORGANIZATIONS WHERE STATUS > 1 AND GLOBAL_SORT LIKE {0} + '______')
					)TEMPDB
				ORDER BY GLOBAL_SORT				
				";
			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strParentGlobalSort, true));
			DataSet ds = InnerCommon.ExecuteDataset(strSql);

			StringBuilder builder = new StringBuilder(1024);
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				strSql = SetSortDataSql(OGUCommonDefine.DBValueToString(row["OBJECTCLASS"]),
					OGUCommonDefine.DBValueToString(row["GUID"]),
					strParentGlobalSort,
					strParentGuid,
					iSort++,
					builder.Length == 0);

				builder.Append(strSql + Environment.NewLine);
			}
			if (builder.Length > 0)
				InnerCommon.ExecuteNonQuery(builder.ToString());
		}

		#region private

		/// <summary>
		/// 设置排序是对相关对象的数据处理
		/// </summary>
		/// <param name="strType"></param>
		/// <param name="strGuid"></param>
		/// <param name="strParentGlobalSort"></param>
		/// <param name="strParentGuid"></param>
		/// <param name="iSort"></param>
		/// <returns></returns>
		private static string SetSortDataSql(string strType, string strGuid, string strParentGlobalSort, string strParentGuid, int iSort)
		{
			return SetSortDataSql(strType, strGuid, strParentGlobalSort, strParentGuid, iSort, true);
		}

		/// <summary>
		/// 设置排序是对相关对象的数据处理
		/// Add by Jerry
		/// </summary>
		/// <param name="strType"></param>
		/// <param name="strGuid"></param>
		/// <param name="strParentGlobalSort"></param>
		/// <param name="strParentGuid"></param>
		/// <param name="iSort"></param>
		/// <param name="isFirst">Define sql var if first time</param>
		/// <returns></returns>
		private static string SetSortDataSql(string strType, string strGuid, string strParentGlobalSort, string strParentGuid, int iSort, bool isFirst)
		{
			string strSql = isFirst ? @" DECLARE @ORG_ORIGINAL_SORT NVARCHAR(255); " : string.Empty;

			switch (strType)
			{
				case "ORGANIZATIONS":
					strSql += @"
							UPDATE ORGANIZATIONS
								SET INNER_SORT = {0},
									GLOBAL_SORT = {1} + {0},
									@ORG_ORIGINAL_SORT = ORIGINAL_SORT
							WHERE GUID = {2};
							
							UPDATE ORGANIZATIONS
								SET GLOBAL_SORT = {1} + {0} + SUBSTRING(GLOBAL_SORT, LEN({1} + {0}) + 1, LEN(GLOBAL_SORT) - LEN({1} + {0}))
							WHERE ORIGINAL_SORT LIKE @ORG_ORIGINAL_SORT + '_%';
			
							UPDATE GROUPS
								SET GLOBAL_SORT = {1} + {0} + SUBSTRING(GLOBAL_SORT, LEN({1} + {0}) + 1, LEN(GLOBAL_SORT) - LEN({1} + {0}))
							WHERE ORIGINAL_SORT LIKE @ORG_ORIGINAL_SORT + '_%';

							UPDATE OU_USERS
								SET GLOBAL_SORT = {1} + {0} + SUBSTRING(GLOBAL_SORT, LEN({1} + {0}) + 1, LEN(GLOBAL_SORT) - LEN({1} + {0}))
							WHERE ORIGINAL_SORT LIKE @ORG_ORIGINAL_SORT + '_%';";
					break;
				case "GROUPS":
					strSql = @"
							UPDATE GROUPS 
								SET INNER_SORT = {0}, 
									GLOBAL_SORT = {1} + {0} 
							WHERE GUID = {2};";
					break;
				case "USERS":
					strSql = @"
							UPDATE OU_USERS
								SET INNER_SORT = {0},
									GLOBAL_SORT = {1} + {0}
							WHERE USER_GUID = {2}
								AND PARENT_GUID = {3};";
					break;
				default: ExceptionHelper.TrueThrow(true, "对不起，系统中没有处理对应于＂" + strType + "＂的逻辑程序！");
					break;
			}
			strSql = string.Format(strSql,
				TSqlBuilder.Instance.CheckQuotationMark(iSort.ToString(CommonResource.OriginalSortDefault), true),
				TSqlBuilder.Instance.CheckQuotationMark(strParentGlobalSort, true),
				TSqlBuilder.Instance.CheckQuotationMark(strGuid, true),
				TSqlBuilder.Instance.CheckQuotationMark(strParentGuid, true));

			return strSql;
		}

		/// <summary>
		/// 把table中各个对象的字段设置送入XML对应的属性数据中
		/// </summary>
		/// <param name="table">包含所有数据对象的数据表</param>
		/// <param name="strNameCol">对象的类型对应字段名成（对应节点的指定的对应名称的字段）</param>
		/// <returns>把table中各个对象的字段设置送入XML对应的属性数据中</returns>
		private XmlDocument GetXmlDocAttr(DataTable table, string strNameCol)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<DataTable />");
			XmlNode root = xmlDoc.DocumentElement;

			foreach (DataRow row in table.Rows)
			{
				XmlElement rowElem = (XmlElement)XmlHelper.AppendNode(root, OGUCommonDefine.DBValueToString(row[strNameCol]));
				foreach (DataColumn col in table.Columns)
					rowElem.SetAttribute(col.ColumnName, OGUCommonDefine.DBValueToString(row[col.ColumnName]));
			}

			return xmlDoc;
		}

		#endregion


	}
}
