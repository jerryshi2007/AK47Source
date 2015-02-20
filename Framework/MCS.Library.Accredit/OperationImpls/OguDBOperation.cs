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
    internal partial class OguDBOperation : IOguDataOperation
    {
        #region IOguDataOperation 成员

        public void Init(OguDataOperationEventContainer eventContainer)
        {
			eventContainer.BeforeUpdateObjects += new BeforeUpdateObjectsHandler(eventContainer_BeforeUpdateObjects);
			eventContainer.UpdateObjects += new UpdateObjectsHandler(eventContainer_UpdateObjects);
            eventContainer.BeforeInsertObjects += new BeforeInsertObjectsHandler(eventContainer_BeforeInsertObjects);
            eventContainer.InsertObjects += new InsertObjectsHandler(eventContainer_InsertObjects);
            eventContainer.BeforeLogicDeleteObjects += new BeforeLogicDeleteObjectsHandler(eventContainer_BeforeLogicDeleteObjects);
            eventContainer.LogicDeleteObjects += new LogicDeleteObjectsHandler(eventContainer_LogicDeleteObjects);
			eventContainer.BeforeFurbishDeleteObjects += new BeforeFurbishDeleteObjectsHandler(eventContainer_BeforeFurbishDeleteObjects);
			eventContainer.FurbishDeleteObjects += new FurbishDeleteObjectsHandler(eventContainer_FurbishDeleteObjects);
			eventContainer.BeforeRealDeleteObjects += new BeforeRealDeleteObjectsHandler(eventContainer_BeforeRealDeleteObjects);
			eventContainer.RealDeleteObjects += new RealDeleteObjectsHandler(eventContainer_RealDeleteObjects);
			eventContainer.BeforeInitPassword += new BeforeInitPasswordHandler(eventContainer_BeforeInitPassword);
			eventContainer.InitPassword += new InitPasswordHandler(eventContainer_InitPassword);
			eventContainer.BeforeResetPassword += new BeforeResetPasswordHandler(eventContainer_BeforeResetPassword);
			eventContainer.ResetPassword += new ResetPasswordHandler(eventContainer_ResetPassword);
			eventContainer.BeforeMoveObjects += new BeforeMoveObjectsHandler(eventContainer_BeforeMoveObjects);
			eventContainer.MoveObjects += new MoveObjectsHandler(eventContainer_MoveObjects);
			eventContainer.BeforeSortObjects += new BeforeSortObjectsHandler(eventContainer_BeforeSortObjects);
			eventContainer.SortObjects += new SortObjectsHandler(eventContainer_SortObjects);
			eventContainer.BeforeGroupSortObjects += new BeforeGroupSortObjectsHandler(eventContainer_BeforeGroupSortObjects);
			eventContainer.GroupSortObjects += new GroupSortObjectsHandler(eventContainer_GroupSortObjects);
			eventContainer.BeforeSetUserMainDuty += new BeforeSetUserMainDutyHandler(eventContainer_BeforeSetUserMainDuty);
			eventContainer.SetUserMainDuty += new SetUserMainDutyHandler(eventContainer_SetUserMainDuty);
			eventContainer.BeforeDelUsersFromGroups += new BeforeDelUsersFromGroupsHandler(eventContainer_BeforeDelUsersFromGroups);
			eventContainer.DelUsersFromGroups += new DelUsersFromGroupsHandler(eventContainer_DelUsersFromGroups);
			eventContainer.BeforeAddObjectsToGroups += new BeforeAddObjectsToGroupsHandler(eventContainer_BeforeAddObjectsToGroups);
			eventContainer.AddObjectsToGroups += new AddObjectsToGroupsHandler(eventContainer_AddObjectsToGroups);
			eventContainer.BeforeSetSecsToLeader += new BeforeSetSecsToLeaderHandler(eventContainer_BeforeSetSecsToLeader);
			eventContainer.SetSecsToLeader += new SetSecsToLeaderHandler(eventContainer_SetSecsToLeader);
			eventContainer.BeforeDelSecsOfLeader += new BeforeDelSecsOfLeaderHandler(eventContainer_BeforeDelSecsOfLeader);
			eventContainer.DelSecsOfLeader += new DelSecsOfLeaderHandler(eventContainer_DelSecsOfLeader);
        }
        #endregion

		private void eventContainer_BeforeSetUserMainDuty(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;

			StringBuilder strB = new StringBuilder(1024);
			foreach (XmlElement elem in root.ChildNodes)
			{
				string strUserGuid = elem.SelectSingleNode("USER_GUID").InnerText;
				string strOrgGuid = elem.SelectSingleNode("PARENT_GUID").InnerText;

				string strSql = @"
					UPDATE OU_USERS
						SET SIDELINE = 1
					WHERE USER_GUID = {0};
	
					UPDATE OU_USERS
						SET SIDELINE = 0
					WHERE USER_GUID = {0} AND PARENT_GUID = {1}
					";

				strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strUserGuid, true),
					TSqlBuilder.Instance.CheckQuotationMark(strOrgGuid, true));

				strB.Append(strSql);
			}
			context.Add("Sql", strB.ToString());
		}

		private void eventContainer_SetUserMainDuty(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			InnerCommon.ExecuteNonQuery(context["Sql"].ToString());
		}

		private void eventContainer_BeforeUpdateObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			switch (root.FirstChild.LocalName)
			{
				case "GROUPS":
					PrepareUpdateOrgOrGroups(xmlDoc, GetXsdDocument("GROUPS"), context);
					break;
				case "ORGANIZATIONS":
					PrepareUpdateOrgOrGroups(xmlDoc, GetXsdDocument("ORGANIZATIONS"), context);
					break;
				case "USERS":
					PrepareUpdateUserInOrganization(xmlDoc, GetXsdDocument("USERS"), GetXsdDocument("OU_USERS"), context);
					break;
				default: ExceptionHelper.TrueThrow(true, "没有相应与“" + root.FirstChild.LocalName + "”的数据对象类型");
					break;
			}
		}

		private void eventContainer_UpdateObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			try
			{
				XmlElement root = xmlDoc.DocumentElement;
				switch (root.FirstChild.LocalName)
				{
					case "GROUPS":
						UpdateOrgOrGroups(context);
						break;
					case "ORGANIZATIONS":
						UpdateOrgOrGroups(context);
						break;
					case "USERS":
						UpdateUserInOrganization(context);
						break;
					default: ExceptionHelper.TrueThrow(true, "没有相应与“" + root.FirstChild.LocalName + "”的数据对象类型");
						break;
				}
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				switch (ex.Number)
				{
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

      

		#region Private     

		/// <summary>
		/// 更新指定机构或者用户组的属性信息数据
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private static int UpdateOrgOrGroups(Dictionary<object, object> context)
		{
			return InnerCommon.ExecuteNonQuery(context["Sql"].ToString());
		}

		private static void PrepareUpdateOrgOrGroups(XmlDocument xmlDoc, XmlDocument xsdDoc, Dictionary<object, object> context)
		{
			XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
			XmlNode aNode = nodeSet.SelectSingleNode(".//ALL_PATH_NAME");
			if (aNode != null)
				CheckAllPathNameInSystem(aNode.InnerText);

			XmlNode mNode = XmlHelper.AppendNode(nodeSet, "MODIFY_TIME", "GETDATE()");
			XmlHelper.AppendAttr(mNode, "type", "other");

			DataExtraCheck(xmlDoc, nodeSet.ParentNode.LocalName);//附加的数据检查

			string strSql = InnerCommon.GetUpdateSqlStr(xmlDoc, xsdDoc);
			if (xmlDoc.DocumentElement.FirstChild.LocalName == "ORGANIZATIONS")
			{
				strSql += " \n ; \n " + UpdataOrganizationsChildrens(xmlDoc);
			}

			context.Add("Sql", strSql);
		}

		/// <summary>
		/// 因为更新了机构的对象名称，要求同步修改其子孙的ALL_PATH_NAME
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <returns></returns>
		private static string UpdataOrganizationsChildrens(XmlDocument xmlDoc)
		{
			string strResult = string.Empty;

			XmlNode root = xmlDoc.DocumentElement.FirstChild;
			if (root.SelectSingleNode("SET/OBJ_NAME") != null)
			{
				string strNewOrgAPN = root.SelectSingleNode("SET/ALL_PATH_NAME").InnerText;
				string strGuid = root.SelectSingleNode("WHERE/GUID").InnerText;
				ExceptionHelper.TrueThrow(strGuid.Length == 0, "对不起,没有找到确定修改的目标！");

				string strSql = "SELECT * FROM ORGANIZATIONS WHERE GUID = " + TSqlBuilder.Instance.CheckQuotationMark(strGuid, true);
				DataSet ds = InnerCommon.ExecuteDataset(strSql);
				ExceptionHelper.TrueThrow(ds.Tables[0].Rows.Count == 0, "对不起,没有找到确定修改的目标！");

				string strOldOrgAPN = OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]);
				string strOrgOS = OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ORIGINAL_SORT"]);

				strResult = @"
				UPDATE ORGANIZATIONS 
					SET ALL_PATH_NAME = {0} + SUBSTRING(ALL_PATH_NAME, LEN({2}) + 1, LEN(ALL_PATH_NAME) - LEN({2})) 
				WHERE ORIGINAL_SORT LIKE {1} + '_%';  

				UPDATE GROUPS 
					SET ALL_PATH_NAME = {0} +  SUBSTRING(ALL_PATH_NAME, LEN({2}) + 1, LEN(ALL_PATH_NAME) - LEN({2})) 
				WHERE ORIGINAL_SORT LIKE {1} + '_%'; 

				UPDATE OU_USERS 
					SET ALL_PATH_NAME = {0} +  SUBSTRING(ALL_PATH_NAME, LEN({2}) + 1, LEN(ALL_PATH_NAME) - LEN({2})) 
				WHERE ORIGINAL_SORT LIKE {1} + '_%';";

				strResult = string.Format(strResult, TSqlBuilder.Instance.CheckQuotationMark(strNewOrgAPN, true),
					TSqlBuilder.Instance.CheckQuotationMark(strOrgOS, true),
					TSqlBuilder.Instance.CheckQuotationMark(strOldOrgAPN, true));
			}

			return strResult;
		}

		private static int UpdateUserInOrganization(Dictionary<object, object> context)
		{
			return InnerCommon.ExecuteNonQuery(context["Sql"].ToString());
		}

		private static void PrepareUpdateUserInOrganization(XmlDocument xmlDoc, XmlDocument userXsd, XmlDocument orgUserXsd, Dictionary<object, object> context)
		{
			XmlNode nodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");

			XmlNode aNode = nodeSet.SelectSingleNode(".//ALL_PATH_NAME");
			if (aNode != null)
				CheckAllPathNameInSystem(aNode.InnerText);

			DataExtraCheck(xmlDoc, "USERS", "OU_USERS");

			XmlDocument userXml = new XmlDocument();
			userXml.LoadXml("<Update><USERS><SET/></USERS></Update>");
			XmlDocument orgUserXml = new XmlDocument();
			orgUserXml.LoadXml("<Update><OU_USERS><SET/></OU_USERS></Update>");

			XmlDocToUsersAndOrgUsers(xmlDoc, userXml, userXsd, orgUserXml, orgUserXsd);

			string strSql = string.Empty;
			if (userXml.DocumentElement.SelectSingleNode(".//SET").ChildNodes.Count > 0)
			{
				strSql = InnerCommon.GetUpdateSqlStr(userXml, userXsd);
			}

			Debug.WriteLine(strSql, "USERS");

			if (orgUserXml.DocumentElement.SelectSingleNode(".//SET").ChildNodes.Count > 0)
			{
				strSql += " \n \n " + InnerCommon.GetUpdateSqlStr(orgUserXml, orgUserXsd);
			}
			Debug.WriteLine(strSql, "OU_USERS");

			context.Add("Sql", strSql);
		}

		/// <summary>
		/// 针对人员数据插入、修改、改兼职等数据操作时候，一些无用数据无需录入（因为数据录入就要求唯一而且不允许是空字符串）
		/// </summary>
		/// <param name="xmlDoc">要求被检查的数据结构</param>
		/// <param name="strObjectTypes">要求检查的对象数据类型</param>
		private static void DataExtraCheck(XmlDocument xmlDoc, params string[] strObjectTypes)
		{
			XmlElement root = xmlDoc.DocumentElement;

			XmlDocument xmlCheckDoc = GetXmlDocument("DataExtraCheck");

			if (xmlCheckDoc != null)
			{
				XmlElement checkRoot = xmlCheckDoc.DocumentElement;

				foreach (string strObjType in strObjectTypes)
				{
					XmlNode checkNode = checkRoot.SelectSingleNode(strObjType);
					if (checkNode != null)
					{
						foreach (XmlNode itemNode in checkNode.ChildNodes)
						{
							XmlNode nodeCheck = root.SelectSingleNode(".//" + itemNode.SelectSingleNode("FIELD").InnerText);
							if (nodeCheck != null)
							{
								if (itemNode.SelectSingleNode("LENGTH") != null)
								{
									string[] strLengthArray = itemNode.SelectSingleNode("LENGTH").InnerText.Split(',', ' ');
									bool bCompare = false;
									foreach (string strLength in strLengthArray)
									{
										if (nodeCheck.InnerText.Length == int.Parse(strLength))
										{
											bCompare = true;
											break;
										}
									}

									ExceptionHelper.FalseThrow(bCompare, "请注意：录入数据中的“" + itemNode.SelectSingleNode("NAME").InnerText + "”数据不合法！\n\n"
										+ "该处数据要求数据长度满足：" + string.Join("位、", strLengthArray) + "位！\n\n谢谢您的合作，请再试一次吧！");
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 针对机构中的人员的相关数据处理
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="userXml"></param>
		/// <param name="userXsd"></param>
		/// <param name="orgUserXml"></param>
		/// <param name="orgUsersXsd"></param>
		private static void XmlDocToUsersAndOrgUsers(XmlDocument xmlDoc,
			XmlDocument userXml,
			XmlDocument userXsd,
			XmlDocument orgUserXml,
			XmlDocument orgUsersXsd)
		{
			XmlNode oNodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
			XmlNode uNodeSet = userXml.DocumentElement.SelectSingleNode(".//SET");
			XmlNode ouNodeSet = orgUserXml.DocumentElement.SelectSingleNode(".//SET");

			foreach (XmlNode oElemNode in oNodeSet.ChildNodes)
			{
				if (InnerCommon.GetXSDColumnNode(userXsd, oElemNode.LocalName) != null)
					XmlHelper.AppendNode(uNodeSet, oElemNode.LocalName, oElemNode.InnerText);

				if (InnerCommon.GetXSDColumnNode(orgUsersXsd, oElemNode.LocalName) != null)
					XmlHelper.AppendNode(ouNodeSet, oElemNode.LocalName, oElemNode.InnerText);
			}

			XmlNode wNode = xmlDoc.DocumentElement.SelectSingleNode(".//WHERE");
			if (wNode != null)
			{
				XmlNode uWNode = XmlHelper.AppendNode(userXml.DocumentElement.FirstChild, "WHERE");
				XmlHelper.AppendNode(uWNode, "GUID", wNode.SelectSingleNode("USER_GUID").InnerText);

				XmlNode ouWNode = XmlHelper.AppendNode(orgUserXml.DocumentElement.FirstChild, "WHERE");
				foreach (XmlNode wcNode in wNode.ChildNodes)
				{
					XmlHelper.AppendNode(ouWNode, wcNode.LocalName, wcNode.InnerText);
				}
			}

			if (uNodeSet.ChildNodes.Count > 0)
			{
				XmlNode mNode = XmlHelper.AppendNode(uNodeSet, "MODIFY_TIME", "GETDATE()");
				XmlHelper.AppendAttr(mNode, "type", "other");
			}

			if (ouNodeSet.ChildNodes.Count > 0)
			{
				XmlNode mNode = XmlHelper.AppendNode(ouNodeSet, "MODIFY_TIME", "GETDATE()");
				XmlHelper.AppendAttr(mNode, "type", "other");
			}
		}

		/// <summary>
		/// 检测系统中是否存在相同Ａｌｌ＿ＰＡＴＨ＿ＮＡＭＥ属性的对象，如果存在就得要抛出异常提醒。
		/// </summary>
		/// <param name="strAllPathName"></param>
		private static void CheckAllPathNameInSystem(string strAllPathName)
		{
			string strSql = @"
				SELECT * FROM ORGANIZATIONS WHERE ALL_PATH_NAME = {0};
				SELECT * FROM GROUPS WHERE ALL_PATH_NAME = {0};
				SELECT * FROM OU_USERS WHERE ALL_PATH_NAME = {0};";
			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strAllPathName, true));

			DataSet ds = InnerCommon.ExecuteDataset(strSql);

			foreach (DataTable table in ds.Tables)
			{
				ExceptionHelper.TrueThrow(table.Rows.Count > 0,
					"对不起，系统中已经存在您命名的对象＂" + strAllPathName + "＂！\n\n请修改＂对象名称＂后再保存！");
			}
		}

		/// <summary>
		/// 获取应用设置中的XSD数据字典
		/// </summary>
		/// <param name="strFileName">数据字典名称</param>
		/// <returns>数据字典文档对象</returns>
		private static XmlDocument GetXsdDocument(string strFileName)
		{
			string resourceName = string.Format("{0}.XSD.{1}.xsd", typeof(OguDBOperation).Namespace, strFileName);
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

			ExceptionHelper.FalseThrow(stream != null, "不能找到资源\"{0}\"", resourceName);

			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.Load(stream);

			return xmlDoc;
		}

		/// <summary>
		/// 获取应用设置中的XML数据设置
		/// </summary>
		/// <param name="strFileName">XML文件名称</param>
		/// <returns>XML文档对象</returns>
		private static XmlDocument GetXmlDocument(string strFileName)
		{
			string resourceName = string.Format("{0}.XML.{1}.xml", typeof(OguDBOperation).Namespace, strFileName);
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

			ExceptionHelper.FalseThrow(stream != null, "不能找到资源\"{0}\"", resourceName);

			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.Load(stream);

			return xmlDoc;
		}

		
		#endregion Private
	}
}
