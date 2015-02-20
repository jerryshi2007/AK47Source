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

		private void eventContainer_BeforeMoveObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
		}

		private void eventContainer_MoveObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			string strOrgGuid = root.GetAttribute("GUID");

			foreach (XmlElement elem in root.ChildNodes)
			{
				switch (elem.LocalName)
				{
					case "ORGANIZATIONS":
						MoveOrganizationsToNewOrg(elem, strOrgGuid, context);
						break;
					case "USERS":
						MoveUsersToNewOrg(elem, strOrgGuid, context);
						break;
					case "GROUPS":
						MoveGroupsToNewOrg(elem, strOrgGuid, context);
						break;
					default: ExceptionHelper.TrueThrow(true, "对不起，系统中没有处理对象＂" + elem.LocalName + "＂的程序！");
						break;
				}
			}
		}

		private void MoveOrganizationsToNewOrg(XmlElement elem, string strOrgGuid, Dictionary<object, object> context)
		{
			string strGuid = elem.GetAttribute("GUID");

			string strSql = @"
				DECLARE @ORG_ORIGINAL_SORT NVARCHAR(255);
				DECLARE @ORG_GLOBAL_SORT NVARCHAR(255);
				DECLARE @ORG_ALL_PATH_NAME NVARCHAR(255);

				DECLARE @NEW_INNER_SORT NVARCHAR(6);
				DECLARE @NEW_ALL_PATH_NAME NVARCHAR(255);

				DECLARE @OLD_ORIGINAL_SORT NVARCHAR(255);
				DECLARE @OLD_GLOBAL_SORT NVARCHAR(255);
				DECLARE @OLD_ALL_PATH_NAME NVARCHAR(255);

				/*获取被放入机构的相关数据*/
				UPDATE ORGANIZATIONS
					SET @ORG_ORIGINAL_SORT = ORIGINAL_SORT, 
						@ORG_GLOBAL_SORT = GLOBAL_SORT, 
						@NEW_INNER_SORT = REPLACE(STR(CHILDREN_COUNTER + 1, 6), ' ', '0'),
						@ORG_ALL_PATH_NAME = ALL_PATH_NAME, 
						CHILDREN_COUNTER = CHILDREN_COUNTER + 1
				WHERE GUID = {0};

				/*修改自身数据字段*/
				UPDATE ORGANIZATIONS
					SET PARENT_GUID = {0},
						INNER_SORT = @NEW_INNER_SORT, 
						ORIGINAL_SORT = @ORG_ORIGINAL_SORT + @NEW_INNER_SORT,
						GLOBAL_SORT = @ORG_GLOBAL_SORT + @NEW_INNER_SORT,
						ALL_PATH_NAME = @ORG_ALL_PATH_NAME + '\' + OBJ_NAME,
						@NEW_ALL_PATH_NAME = @ORG_ALL_PATH_NAME + '\' + OBJ_NAME,
						@OLD_ORIGINAL_SORT = ORIGINAL_SORT,
						@OLD_GLOBAL_SORT = GLOBAL_SORT,
						@OLD_ALL_PATH_NAME = ALL_PATH_NAME
				WHERE GUID = {1};

				/*修改其子孙数据结构*/
				UPDATE ORGANIZATIONS
					SET ORIGINAL_SORT = @ORG_ORIGINAL_SORT + @NEW_INNER_SORT + SUBSTRING(ORIGINAL_SORT, LEN(@OLD_ORIGINAL_SORT) + 1, LEN(ORIGINAL_SORT) - LEN(@OLD_ORIGINAL_SORT)),
						GLOBAL_SORT = @ORG_GLOBAL_SORT + @NEW_INNER_SORT + SUBSTRING(GLOBAL_SORT, LEN(@OLD_GLOBAL_SORT) + 1, LEN(GLOBAL_SORT) - LEN(@OLD_GLOBAL_SORT)),
						ALL_PATH_NAME = @NEW_ALL_PATH_NAME + SUBSTRING(ALL_PATH_NAME, LEN(@OLD_ALL_PATH_NAME) + 1, LEN(ALL_PATH_NAME) - LEN(@OLD_ALL_PATH_NAME))
				WHERE ORIGINAL_SORT LIKE @OLD_ORIGINAL_SORT + '%';

				UPDATE GROUPS
					SET ORIGINAL_SORT = @ORG_ORIGINAL_SORT + @NEW_INNER_SORT + SUBSTRING(ORIGINAL_SORT, LEN(@OLD_ORIGINAL_SORT) + 1, LEN(ORIGINAL_SORT) - LEN(@OLD_ORIGINAL_SORT)),
						GLOBAL_SORT = @ORG_GLOBAL_SORT + @NEW_INNER_SORT + SUBSTRING(GLOBAL_SORT, LEN(@OLD_GLOBAL_SORT) + 1, LEN(GLOBAL_SORT) - LEN(@OLD_GLOBAL_SORT)),
						ALL_PATH_NAME = @NEW_ALL_PATH_NAME + SUBSTRING(ALL_PATH_NAME, LEN(@OLD_ALL_PATH_NAME) + 1, LEN(ALL_PATH_NAME) - LEN(@OLD_ALL_PATH_NAME))
				WHERE ORIGINAL_SORT LIKE @OLD_ORIGINAL_SORT + '%';

				UPDATE OU_USERS
					SET ORIGINAL_SORT = @ORG_ORIGINAL_SORT + @NEW_INNER_SORT + SUBSTRING(ORIGINAL_SORT, LEN(@OLD_ORIGINAL_SORT) + 1, LEN(ORIGINAL_SORT) - LEN(@OLD_ORIGINAL_SORT)),
						GLOBAL_SORT = @ORG_GLOBAL_SORT + @NEW_INNER_SORT + SUBSTRING(GLOBAL_SORT, LEN(@OLD_GLOBAL_SORT) + 1, LEN(GLOBAL_SORT) - LEN(@OLD_GLOBAL_SORT)),
						ALL_PATH_NAME = @NEW_ALL_PATH_NAME + SUBSTRING(ALL_PATH_NAME, LEN(@OLD_ALL_PATH_NAME) + 1, LEN(ALL_PATH_NAME) - LEN(@OLD_ALL_PATH_NAME))
				WHERE ORIGINAL_SORT LIKE @OLD_ORIGINAL_SORT + '%';

				/*获取新的数据值*/
				SELECT @ORG_ORIGINAL_SORT + @NEW_INNER_SORT AS ORIGINAL_SORT, 
					@ORG_GLOBAL_SORT + @NEW_INNER_SORT AS GLOBAL_SORT, 
					@NEW_INNER_SORT AS INNER_SORT, 
					@NEW_ALL_PATH_NAME AS ALL_PATH_NAME;
				";
			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strOrgGuid, true),
				TSqlBuilder.Instance.CheckQuotationMark(strGuid, true));

			DataSet ds = InnerCommon.ExecuteDataset(strSql);

			SetMoveDataToXml(ds, elem, strOrgGuid);
		}
		private void MoveUsersToNewOrg(XmlElement elem, string strOrgGuid, Dictionary<object, object> context)
		{
			string strGuid = elem.GetAttribute("GUID");
			string strOldParentGuid = elem.GetAttribute("PARENT_GUID");

			string strSql = @"
				DECLARE @ORG_ORIGINAL_SORT NVARCHAR(255);
				DECLARE @ORG_GLOBAL_SORT NVARCHAR(255);
				DECLARE @ORG_ALL_PATH_NAME NVARCHAR(255);

				DECLARE @NEW_INNER_SORT NVARCHAR(6);
				DECLARE @NEW_ALL_PATH_NAME NVARCHAR(255);

				/*获取被放入机构的相关数据*/
				UPDATE ORGANIZATIONS
					SET @ORG_ORIGINAL_SORT = ORIGINAL_SORT, 
						@ORG_GLOBAL_SORT = GLOBAL_SORT, 
						@NEW_INNER_SORT = REPLACE(STR(CHILDREN_COUNTER + 1, 6), ' ', '0'),
						@ORG_ALL_PATH_NAME = ALL_PATH_NAME, 
						CHILDREN_COUNTER = CHILDREN_COUNTER + 1
				WHERE GUID = {0};

				/*修改自身数据字段*/
				UPDATE OU_USERS
					SET PARENT_GUID = {0},
						INNER_SORT = @NEW_INNER_SORT, 
						ORIGINAL_SORT = @ORG_ORIGINAL_SORT + @NEW_INNER_SORT,
						GLOBAL_SORT = @ORG_GLOBAL_SORT + @NEW_INNER_SORT,
						ALL_PATH_NAME = @ORG_ALL_PATH_NAME + '\' + OBJ_NAME,
						@NEW_ALL_PATH_NAME = @ORG_ALL_PATH_NAME + '\' + OBJ_NAME
				WHERE USER_GUID = {1}
					AND PARENT_GUID = {2};

				/*获取新的数据值*/
				SELECT @ORG_ORIGINAL_SORT + @NEW_INNER_SORT AS ORIGINAL_SORT, 
					@ORG_GLOBAL_SORT + @NEW_INNER_SORT AS GLOBAL_SORT, 
					@NEW_INNER_SORT AS INNER_SORT, 
					@NEW_ALL_PATH_NAME AS ALL_PATH_NAME
				";

			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strOrgGuid, true),
				TSqlBuilder.Instance.CheckQuotationMark(strGuid, true),
				TSqlBuilder.Instance.CheckQuotationMark(strOldParentGuid, true));

			DataSet ds = InnerCommon.ExecuteDataset(strSql);

			SetMoveDataToXml(ds, elem, strOrgGuid);
		}

		private void MoveGroupsToNewOrg(XmlElement elem, string strOrgGuid, Dictionary<object, object> context)
		{
			string strGuid = elem.GetAttribute("GUID");

			string strSql = @"
				DECLARE @ORG_ORIGINAL_SORT NVARCHAR(255);
				DECLARE @ORG_GLOBAL_SORT NVARCHAR(255);
				DECLARE @ORG_ALL_PATH_NAME NVARCHAR(255);

				DECLARE @NEW_INNER_SORT NVARCHAR(6);
				DECLARE @NEW_ALL_PATH_NAME NVARCHAR(255);

				/*获取被放入机构的相关数据*/
				UPDATE ORGANIZATIONS
					SET @ORG_ORIGINAL_SORT = ORIGINAL_SORT, 
						@ORG_GLOBAL_SORT = GLOBAL_SORT, 
						@NEW_INNER_SORT = REPLACE(STR(CHILDREN_COUNTER + 1, 6), ' ', '0'),
						@ORG_ALL_PATH_NAME = ALL_PATH_NAME, 
						CHILDREN_COUNTER = CHILDREN_COUNTER + 1
				WHERE GUID = {0};

				/*修改自身数据字段*/
				UPDATE GROUPS
					SET PARENT_GUID = {0},
						INNER_SORT = @NEW_INNER_SORT, 
						ORIGINAL_SORT = @ORG_ORIGINAL_SORT + @NEW_INNER_SORT,
						GLOBAL_SORT = @ORG_GLOBAL_SORT + @NEW_INNER_SORT,
						ALL_PATH_NAME = @ORG_ALL_PATH_NAME + '\' + OBJ_NAME,
						@NEW_ALL_PATH_NAME = @ORG_ALL_PATH_NAME + '\' + OBJ_NAME
				WHERE GUID = {1};

				/*获取新的数据值*/
				SELECT @ORG_ORIGINAL_SORT + @NEW_INNER_SORT AS ORIGINAL_SORT, 
					@ORG_GLOBAL_SORT + @NEW_INNER_SORT AS GLOBAL_SORT, 
					@NEW_INNER_SORT AS INNER_SORT, 
					@NEW_ALL_PATH_NAME AS ALL_PATH_NAME
				";

			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strOrgGuid, true),
				TSqlBuilder.Instance.CheckQuotationMark(strGuid, true));

			DataSet ds = InnerCommon.ExecuteDataset(strSql);

			SetMoveDataToXml(ds, elem, strOrgGuid);
		}

		/// <summary>
		/// 对象数据移动以后要求发生的ＸＭＬ数据更新（提供给ＸＭＬＨＴＴＰ返回使用）
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="root"></param>
		/// <param name="strOrgGuid"></param>
		private static void SetMoveDataToXml(DataSet ds, XmlElement root, string strOrgGuid)
		{
			ExceptionHelper.TrueThrow(ds.Tables[0].Rows.Count == 0, "对不起，系统对于移动对象＂" + root.GetAttribute("ALL_PATH_NAME") + "＂操作没有成功！");

			root.SetAttribute("OLD_ORIGINAL_SORT", root.GetAttribute("ORIGINAL_SORT"));
			root.SetAttribute("ORIGINAL_SORT", OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ORIGINAL_SORT"]));

			root.SetAttribute("OLD_GLOBAL_SORT", root.GetAttribute("GLOBAL_SORT"));
			root.SetAttribute("GLOBAL_SORT", OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["GLOBAL_SORT"]));

			root.SetAttribute("OLD_INNER_SORT", root.GetAttribute("INNER_SORT"));
			root.SetAttribute("INNER_SORT", OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["INNER_SORT"]));

			root.SetAttribute("OLD_ALL_PATH_NAME", root.GetAttribute("ALL_PATH_NAME"));
			root.SetAttribute("ALL_PATH_NAME", OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]));

			root.SetAttribute("PARENT_GUID", strOrgGuid);
		}

		
	}
}
