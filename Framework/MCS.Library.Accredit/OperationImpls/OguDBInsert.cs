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
using MCS.Library.Accredit.Properties;

namespace MCS.Library.Accredit
{
    internal partial class OguDBOperation
    {
		private void eventContainer_BeforeAddObjectsToGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{ 
		}
		private XmlDocument eventContainer_AddObjectsToGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlDocument result;

			XmlElement root = xmlDoc.DocumentElement;
			string strGroupGuid = root.GetAttribute("GUID");
			string strAccessLevel = root.GetAttribute("USER_ACCESS_LEVEL");
			ExceptionHelper.TrueThrow(strGroupGuid == string.Empty, "未指定用户组对象！");

			string strSql = "UPDATE GROUPS SET MODIFY_TIME = GETDATE() WHERE GUID = {0}";

			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strGroupGuid, true));
			InnerCommon.ExecuteNonQuery(strSql);

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
							OGUCommonDefine.CombinateAttr(string.Empty));
						AddOrganizationToGroup(ods, strGroupGuid);
						break;
					case "GROUPS":
						string strOldGroupGuid = elem.GetAttribute("GUID");
						if (strOldGroupGuid != strGroupGuid)//组中用户不能循环倒入倒出
						{
							DataSet gds = OGUReader.GetUsersInGroups(strOldGroupGuid, SearchObjectColumn.SEARCH_GUID,
								OGUCommonDefine.CombinateAttr("PARENT_GUID,USER_GUID"),
								string.Empty, SearchObjectColumn.SEARCH_NULL, string.Empty,
								(int)ListObjectDelete.COMMON);
							AddGroupToGroup(gds, strGroupGuid);
						}
						break;
					case "USERS":
						AddUserToGroup(elem, strGroupGuid);
						break;
					default:
						ExceptionHelper.TrueThrow(true, "对不起，系统没有与“" + elem.GetAttribute("OBJECTCLASS") + "”对象相应的数据处理！");
						break;
				}
			}

			DataSet ds = OGUReader.GetUsersInGroups(strGroupGuid, SearchObjectColumn.SEARCH_GUID,
				OGUCommonDefine.CombinateAttr(root.GetAttribute("extAttr")),
				string.Empty, SearchObjectColumn.SEARCH_NULL, string.Empty,
				(int)ListObjectDelete.COMMON);

			result = this.GetXmlDocAttr(ds.Tables[0], "OBJECTCLASS");

			return result;
		}

        private void eventContainer_BeforeInsertObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
        {
            XmlElement root = xmlDoc.DocumentElement;
            switch (root.FirstChild.LocalName)
            {
                case "GROUPS":
                    PrepareInsertOrgORGroups(xmlDoc, GetXsdDocument("GROUPS"), context);
                    break;
                case "ORGANIZATIONS":
                    PrepareInsertOrgORGroups(xmlDoc, GetXsdDocument("ORGANIZATIONS"), context);
                    break;
                case "USERS":
                    if (root.GetAttribute("opType") == "Insert")
                        PrepareInsertUserToOrganization(xmlDoc, GetXsdDocument("USERS"), GetXsdDocument("OU_USERS"), context);
                    else
                    {
                        if (root.GetAttribute("opType") == "AddSideline")
                            PrepareSetUserSideline(xmlDoc, GetXsdDocument("OU_USERS"), context);
                    }
                    break;
                default: ExceptionHelper.TrueThrow(true, "没有相应与“" + root.FirstChild.LocalName + "”的数据对象类型");
                    break;
            }
        }

        private void eventContainer_InsertObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
        {
            try
            {
                XmlElement root = xmlDoc.DocumentElement;
                switch (root.FirstChild.LocalName)
                {
                    case "GROUPS":
                        InsertOrgORGroups(context);
                        break;
                    case "ORGANIZATIONS":
                        InsertOrgORGroups(context);
                        break;
                    case "USERS":
                        if (root.GetAttribute("opType") == "Insert")
                            InsertUserToOrganization(context);
                        else
                        {
                            if (root.GetAttribute("opType") == "AddSideline")
                                SetUserSideline(context);
                        }
                        break;
                    default: ExceptionHelper.TrueThrow(true, "没有相应与“" + root.FirstChild.LocalName + "”的数据对象类型");
                        break;
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                switch (ex.Number)
                {
                    //					case 2627://关键字冲突
                    //						throw new Exception("对不起,系统中发生数据库关键字冲突，请重试！");
                    case 2601://索引冲突
                        string strMsg = ex.Message;
                        ExceptionHelper.TrueThrow(strMsg.IndexOf("LOGON_NAME") >= 0, "对不起，该登录名在系统中已存在，请换一个登录名！");

                        ExceptionHelper.TrueThrow(strMsg.IndexOf("ALL_PATH_NAME") >= 0, "对不起，系统中已经存在了您命名的数据对象＂"
                                + xmlDoc.DocumentElement.SelectSingleNode(".//ALL_PATH_NAME").InnerText + "＂！\n\n请修改“对象名称”后再保存！");

                        ExceptionHelper.TrueThrow(strMsg.IndexOf("PERSON_ID") >= 0, "对不起，系统中已经存在了指定的“人员编码”--“"
                            + xmlDoc.DocumentElement.SelectSingleNode(".//PERSON_ID").InnerText + "”！\n\n请修改后再保存！");

                        ExceptionHelper.TrueThrow(strMsg.IndexOf("IC_CARD") >= 0, "对不起，系统中已经存在了指定的“IC卡编码”--“"
                            + xmlDoc.DocumentElement.SelectSingleNode(".//IC_CARD").InnerText + "”！\n\n请修改后再保存！");

                        ExceptionHelper.TrueThrow(strMsg.IndexOf("CUSTOMS_CODE") >= 0, "对不起，系统中已经存在了指定的“关区代码”--“"
                            + xmlDoc.DocumentElement.SelectSingleNode(".//CUSTOMS_CODE").InnerText + "”！\n\n请修改后再保存！");

                        throw ex;
                    default: throw ex;
                }
            }
        }

        #region private

        /// <summary>
        /// 为用户设置兼职的部门
        /// </summary>
		/// <param name="context"></param>
        private static int SetUserSideline(Dictionary<object, object> context)
        {
            return InnerCommon.ExecuteNonQuery(context["Sql"].ToString());
        }

        private static void PrepareSetUserSideline(XmlDocument xmlDoc, XmlDocument orgUserXsd, Dictionary<object, object> context)
        {
            XmlElement root = xmlDoc.DocumentElement;
            XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
            CheckAllPathNameInSystem(nodeSet.SelectSingleNode(".//ALL_PATH_NAME").InnerText);
            InsertSqlClauseBuilder ic = new InsertSqlClauseBuilder();
            foreach (XmlElement elem in nodeSet.ChildNodes)
            {
                if (InnerCommon.GetXSDColumnNode(orgUserXsd, elem.LocalName) != null)
                    ic.AppendItem(elem.LocalName, elem.InnerText);
            }
         
            ic.AppendItem("INNER_SORT", "@U_INNER_SORT", "other", true);
            ic.AppendItem("ORIGINAL_SORT", "@O_ORIGINAL_SORT + @U_INNER_SORT", "other", true);
            ic.AppendItem("GLOBAL_SORT", "@O_GLOBAL_SORT + @U_INNER_SORT", "other", true);
            ic.AppendItem("MODIFY_TIME", "GETDATE()", "other", true);
            ic.AppendItem("STATUS", "1");

            string strSql = @"
				DECLARE @U_INNER_SORT NVARCHAR(6);
				DECLARE @O_ORIGINAL_SORT NVARCHAR(255);
				DECLARE @O_GLOBAL_SORT NVARCHAR(255);

				UPDATE ORGANIZATIONS
					SET CHILDREN_COUNTER = CHILDREN_COUNTER + 1, 
						MODIFY_TIME = GETDATE(), 
						@U_INNER_SORT = REPLACE(STR(CHILDREN_COUNTER + 1, 6), ' ', '0'),
						@O_ORIGINAL_SORT = ORIGINAL_SORT,
						@O_GLOBAL_SORT = GLOBAL_SORT
				WHERE GUID = {0};
				
				INSERT INTO OU_USERS " + ic.ToSqlString(TSqlBuilder.Instance);

            strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(nodeSet.SelectSingleNode("PARENT_GUID").InnerText, true));

            context.Add("Sql", strSql);
           // InnerCommon.ExecuteNonQuery(strSql);
        }

        /// <summary>
        /// 在指定机构下新添加＂用户＂
        /// </summary>
		/// <param name="context"></param>
        /// <returns></returns>
        private static int InsertUserToOrganization(Dictionary<object, object> context)
        {
            return InnerCommon.ExecuteNonQuery(context["Sql"].ToString());
        }

        private static void PrepareInsertUserToOrganization(XmlDocument xmlDoc, XmlDocument userXsd, XmlDocument orgUserXsd, Dictionary<object, object> context)
        {
            XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");

            XmlNode tNode = nodeSet.SelectSingleNode(".//IC_CARD");
            if (tNode != null)
                ExceptionHelper.FalseThrow(tNode.InnerText.Trim() == string.Empty || tNode.InnerText.Trim().Length == 16, "请注意，系统中要求“IC卡号”只能是16位！");

            CheckAllPathNameInSystem(nodeSet.SelectSingleNode(".//ALL_PATH_NAME").InnerText);

            string strSql = @"
				UPDATE ORGANIZATIONS 
					SET CHILDREN_COUNTER = CHILDREN_COUNTER + 1, MODIFY_TIME = GETDATE() 
				WHERE GUID = {0} ; 

				SELECT * 
				FROM ORGANIZATIONS 
				WHERE GUID = {0}";
            strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(nodeSet.SelectSingleNode("PARENT_GUID").InnerText, true));

            DataSet ds = InnerCommon.ExecuteDataset(strSql);

            ExceptionHelper.TrueThrow((ds == null) || (ds.Tables.Count == 0) || (ds.Tables[0].Rows.Count == 0), "对不起，系统中没有找到父部门对象！");
            DataRow row = ds.Tables[0].Rows[0];

            string strChildCounter = OGUCommonDefine.DBValueToString(row["CHILDREN_COUNTER"]);
			string strInnerSort = CommonResource.OriginalSortDefault.Substring(0,
				 CommonResource.OriginalSortDefault.Length - strChildCounter.Length) + strChildCounter;

            XmlHelper.AppendNode(nodeSet, "INNER_SORT", strInnerSort);
            XmlHelper.AppendNode(nodeSet, "GLOBAL_SORT", OGUCommonDefine.DBValueToString(row["GLOBAL_SORT"]) + strInnerSort);
            XmlHelper.AppendNode(nodeSet, "ORIGINAL_SORT", OGUCommonDefine.DBValueToString(row["ORIGINAL_SORT"]) + strInnerSort);

            XmlHelper.AppendNode(nodeSet, "STATUS", "1");
            string strUserGuid = Guid.NewGuid().ToString();
            XmlHelper.AppendNode(nodeSet, "GUID", strUserGuid);
            XmlHelper.AppendNode(nodeSet, "USER_GUID", strUserGuid);
            // 默认初始化登录口令[cgac\yuan_yong--2004/07/20]
            object oPwdGuid = InnerCommon.ExecuteScalar("SELECT TOP 1 GUID FROM PWD_ARITHMETIC WHERE VISIBLE = 1 ORDER BY SORT_ID");
            XmlHelper.AppendNode(nodeSet, "PWD_TYPE_GUID", oPwdGuid.ToString());
			XmlHelper.AppendNode(nodeSet, "USER_PWD", SecurityCalculate.PwdCalculate(oPwdGuid.ToString(), CommonResource.OriginalSortDefault));

            DataExtraCheck(xmlDoc, "USERS", "OU_USERS");//附加数据检查

            XmlDocument userXml = new XmlDocument();
            userXml.LoadXml("<Insert><USERS><SET/></USERS></Insert>");
            XmlDocument orgUserXml = new XmlDocument();
            orgUserXml.LoadXml("<Insert><OU_USERS><SET/></OU_USERS></Insert>");
            XmlDocToUsersAndOrgUsers(xmlDoc, userXml, userXsd, orgUserXml, orgUserXsd);

            strSql = InnerCommon.GetInsertSqlStr(userXml, userXsd);
            Debug.WriteLine(strSql, "USERS");

            strSql += " \n ; \n " + InnerCommon.GetInsertSqlStr(orgUserXml, orgUserXsd);
            Debug.WriteLine(strSql, "OU_USERS");

            context.Add("Sql", strSql);           
        }
        /// <summary>
        /// 系统添加一个新的＂机构＂或者＂人员组＂
        /// </summary>
		/// <param name="context"></param>
        /// <returns></returns>
        private static int InsertOrgORGroups(Dictionary<object, object> context)
        {
            return InnerCommon.ExecuteNonQuery(context["Sql"].ToString());
        }

        private static void PrepareInsertOrgORGroups(XmlDocument xmlDoc, XmlDocument xsdDoc, Dictionary<object, object> context)
        {
            XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
            CheckAllPathNameInSystem(nodeSet.SelectSingleNode(".//ALL_PATH_NAME").InnerText);

            string strSql = @"
				UPDATE ORGANIZATIONS 
					SET CHILDREN_COUNTER = CHILDREN_COUNTER + 1, MODIFY_TIME = GETDATE() 
				WHERE GUID = {0} ; 

				SELECT * 
				FROM ORGANIZATIONS 
				WHERE GUID = {0}";
            strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(nodeSet.SelectSingleNode("PARENT_GUID").InnerText, true));

            DataSet ds = InnerCommon.ExecuteDataset(strSql);
            ExceptionHelper.TrueThrow((ds == null) || (ds.Tables.Count == 0) || (ds.Tables[0].Rows.Count == 0), "对不起，系统中没有找到父部门对象！");
            DataRow row = ds.Tables[0].Rows[0];

            string strChildCounter = OGUCommonDefine.DBValueToString(row["CHILDREN_COUNTER"]);
			string strInnerSort = CommonResource.OriginalSortDefault.Substring(0,
				 CommonResource.OriginalSortDefault.Length - strChildCounter.Length) + strChildCounter;

            XmlHelper.AppendNode(nodeSet, "INNER_SORT", strInnerSort);
            XmlHelper.AppendNode(nodeSet, "GLOBAL_SORT", OGUCommonDefine.DBValueToString(row["GLOBAL_SORT"]) + strInnerSort);
            XmlHelper.AppendNode(nodeSet, "ORIGINAL_SORT", OGUCommonDefine.DBValueToString(row["ORIGINAL_SORT"]) + strInnerSort);

            XmlHelper.AppendNode(nodeSet, "GUID", Guid.NewGuid().ToString());
            XmlHelper.AppendNode(nodeSet, "STATUS", "1");

            DataExtraCheck(xmlDoc, nodeSet.ParentNode.LocalName);//附加的数据检查

            strSql = InnerCommon.GetInsertSqlStr(xmlDoc, xsdDoc);

            context.Add("Sql", strSql);
        }

		/// <summary>
		/// 用户组中添加一个新的用户
		/// </summary>
		/// <param name="elem"></param>
		/// <param name="strGroupGuid"></param>
		private static void AddUserToGroup(XmlElement elem, string strGroupGuid)
		{
			string strSql = @"
				DECLARE @MAX_INNER_SORT NVARCHAR(6);

				SET @MAX_INNER_SORT = (SELECT MAX(INNER_SORT) FROM GROUP_USERS WHERE GROUP_GUID = {0});

				INSERT INTO GROUP_USERS (GROUP_GUID, USER_GUID, USER_PARENT_GUID, INNER_SORT, MODIFY_TIME, CREATE_TIME)
				VALUES
				({0}, {1}, {2}, REPLACE(STR(ISNULL(@MAX_INNER_SORT, -1) + 1, 6), ' ', '0'), GETDATE(), GETDATE())
				";

			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strGroupGuid, true),
				TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("GUID"), true),
				TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("PARENT_GUID"), true));
			try
			{
				InnerCommon.ExecuteNonQuery(strSql);
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				//				ExceptionHelper.TrueThrow(ex.Number == 2601, "对不起，系统中已经存在了该名称的");
				//				ExceptionHelper.TrueThrow(ex.Number == 2627, "对不起，系统关键字冲突；");//数据重复（key重复）
				if (ex.Number != 2627) //数据重复（key重复）就把错吃掉
					throw;
			}
		}

		/// <summary>
		/// 把指定用户组中的所有用户对象加入到新的用户组中
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="strGroupGuid"></param>
		private static void AddGroupToGroup(DataSet ds, string strGroupGuid)
		{
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				string strSql = @"
					DECLARE @MAX_INNER_SORT NVARCHAR(6);

					SET @MAX_INNER_SORT = (SELECT MAX(INNER_SORT) FROM GROUP_USERS WHERE GROUP_GUID = {0});

					INSERT INTO GROUP_USERS (GROUP_GUID, USER_GUID, USER_PARENT_GUID, INNER_SORT, MODIFY_TIME, CREATE_TIME)
					VALUES
					({0}, {1}, {2}, REPLACE(STR(ISNULL(@MAX_INNER_SORT, -1) + 1, 6), ' ', '0'), GETDATE(), GETDATE())
				";

				strSql = string.Format(strSql,
					TSqlBuilder.Instance.CheckQuotationMark(strGroupGuid, true),
					TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(row["USER_GUID"]), true),
					TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(row["PARENT_GUID"]), true));

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

		/// <summary>
		/// 把指定机构中的所有用户对象加入到新的用户组中
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="strGroupGuid"></param>
		private static void AddOrganizationToGroup(DataSet ds, string strGroupGuid)
		{
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				if (OGUCommonDefine.DBValueToString(row["OBJECTCLASS"]) == "USERS")
				{
					string strSql = @"
						DECLARE @MAX_INNER_SORT NVARCHAR(6);

						SET @MAX_INNER_SORT = (SELECT MAX(INNER_SORT) FROM GROUP_USERS WHERE GROUP_GUID = {0});

						INSERT INTO GROUP_USERS (GROUP_GUID, USER_GUID, USER_PARENT_GUID, INNER_SORT, MODIFY_TIME, CREATE_TIME)
						VALUES
						({0}, {1}, {2}, REPLACE(STR(ISNULL(@MAX_INNER_SORT, -1) + 1, 6), ' ', '0'), GETDATE(), GETDATE())
					";

					strSql = string.Format(strSql,
						TSqlBuilder.Instance.CheckQuotationMark(strGroupGuid, true),
						TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(row["GUID"]), true),
						TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(row["PARENT_GUID"]), true));

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
