using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library;
using System.DirectoryServices;
using MCS.Library.Data.Builder;
using System.Text.RegularExpressions;

namespace MCS.Library.Accredit
{
	internal class ADDataOperation
	{
		private AD2DBTransferContext context = null;
		public ADDataOperation(AD2DBTransferContext ctx)
		{
			this.context = ctx;
		}

		#region 从AD开始读出数据

		public void ExecuteConvertion(AD2DBTransferContext transferContext)
		{
			ADSearchConditions condition = new ADSearchConditions(SearchScope.OneLevel);

			condition.ADSearchCompleted += new ADSearchCompletedHandler(condition_ADSearchCompleted);
			condition.SearchCompletedContext = transferContext;

			//condition.Sort = new SortOption("extensionName", SortDirection.Ascending);

			transferContext.InitialParams.DirectoryHelper.ExecuteSearchRecursively(transferContext.InitialParams.Root,
					ADSearchConditions.GetFilterByMask(ADSchemaType.Users | ADSchemaType.Organizations | ADSchemaType.Groups),
					condition,
					new ADSearchRecursivelyDelegate(EnumEntryItemCallBack),
					transferContext,
					AD2DBHelper.ADObjProperties);
		}

		private static void condition_ADSearchCompleted(DirectorySearcher searcher, List<SearchResult> resultList, object objContext)
		{
			DirectoryEntry entry = searcher.SearchRoot;

			AD2DBTransferContext context = (AD2DBTransferContext)objContext;
			ADHelper helper = context.InitialParams.DirectoryHelper;

			string parentGuid = helper.GetPropertyStrValue("objectGuid", entry);

			Dictionary<string, string> guidGlobalSortDict = GetOriginalChildrenGlobalSortDictionary(parentGuid, context);

			resultList.Sort((sr1, sr2) =>
			{
				string guid1 = helper.GetSearchResultPropertyStrValue("objectGuid", sr1);
				string guid2 = helper.GetSearchResultPropertyStrValue("objectGuid", sr2);

				return string.Compare(
					guidGlobalSortDict.GetValue(guid1, "ZZZZZZ"), 
					guidGlobalSortDict.GetValue(guid2, "ZZZZZZ"),
					true);
			});
		}

		/// <summary>
		/// 找到在一个部门内，原始的ID和排序号之间的关系
		/// </summary>
		/// <param name="parentGuid"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		private static Dictionary<string, string> GetOriginalChildrenGlobalSortDictionary(string parentGuid, AD2DBTransferContext context)
		{
			Dictionary<string, string> sortDict = new Dictionary<string, string>();

			context.OriginalOuUserParentGuidView.FindRows(parentGuid).ForEach(r => sortDict[r["USER_GUID"].ToString()] = r["GLOBAL_SORT"].ToString());
			context.OriginalOguParentGuidView.FindRows(parentGuid).ForEach(r => sortDict[r["GUID"].ToString()] = r["GLOBAL_SORT"].ToString());

			return sortDict;
		}

		private static object EnumEntryItemCallBack(SearchResult sr, ADSearchResultParams asrp, object oParams, ref bool bContinue)
		{
			AD2DBTransferContext context = (AD2DBTransferContext)oParams;

			context.InitialParams.OnBeforeProcessADObject(sr, context.InitialParams, ref bContinue);

			if (bContinue)
			{
				string objName = ADHelper.GetInstance().GetSearchResultPropertyStrValue("name", sr);
				string objClass = AD2DBHelper.TranslateObjectClass(sr.Properties["objectClass"][1].ToString());

				ADHelper helper = ADHelper.GetInstance();

				FillContext(context,
					ADHelper.GetParentRdnSequence(helper.GetSearchResultPropertyStrValue("distinguishedName", sr)));

				try
				{
					switch (objClass)
					{
						case "ORGANIZATIONS":
							InsertOrganizations(sr, context);

							context.OrganizationsConverted++;
							break;
						case "USERS":
							InsertUser(sr, context);
							InsertOUUser(sr, context);
							InsertUsersInfoExtend(sr, context);

							context.UsersConverted++;
							break;
						default:
							break;
					}

					UpdateParentChildrenCounter(context);
				}
				catch (System.Exception ex)
				{
					string strMsg = string.Format("转换对象\"{0}\"出错，{1}", objName, ex.Message + ex.StackTrace);

					context.InitialParams.Log.Write(strMsg);
				}
			}

			return null;
		}

		private static void FillContext(AD2DBTransferContext context, string parentOU)
		{
			string rootPath = AD2DBHelper.TranslateDNToFullPath(parentOU);

			DataRowView[] drvs = context.ADOguAllPathView.FindRows(rootPath);

			ExceptionHelper.FalseThrow(drvs.Length > 0, "不能在机构人员数据库中找到父对象\"{0}\"", rootPath);

			context.ParentGuid = drvs[0]["GUID"].ToString();
			context.ParentOriginalSort = drvs[0]["ORIGINAL_SORT"].ToString();

			context.ParentChildrenCount = Convert.ToInt32(drvs[0]["CHILDREN_COUNTER"]);
		}

		/// <summary>
		/// 更新ORG的计数器
		/// </summary>
		/// <param name="context"></param>
		/// <param name="ts"></param>
		private static void UpdateParentChildrenCounter(AD2DBTransferContext context)
		{
			DataRowView[] drvs = context.ADOguGuidView.FindRows(context.ParentGuid);

			if (drvs.Length > 0)
				drvs[0]["CHILDREN_COUNTER"] = context.ParentChildrenCount;
		}

		private static void InsertOrganizations(SearchResult sr, AD2DBTransferContext context)
		{
			//开始添加AD的比较数据
			ADHelper helper = context.InitialParams.DirectoryHelper;
			DataTable oguTable = context.ADData.Tables["ORGANIZATIONS"];

			DataRow dr = oguTable.NewRow();

			dr["GUID"] = helper.GetSearchResultPropertyStrValue("objectGuid", sr);
			dr["PARENT_GUID"] = context.ParentGuid;

			string name = helper.GetSearchResultPropertyStrValue("name", sr);
			dr["OBJ_NAME"] = name;

			string displayName = helper.GetSearchResultPropertyStrValue("displayName", sr);
			displayName = string.IsNullOrEmpty(displayName) ? name : displayName;
			dr["DISPLAY_NAME"] = NormalizeName(displayName);

			dr["DESCRIPTION"] = helper.GetSearchResultPropertyStrValue("description", sr);
			dr["ALL_PATH_NAME"] = AD2DBHelper.TranslateDNToFullPath(helper.GetSearchResultPropertyStrValue("distinguishedName", sr));

			string innerSort = GetInnerSortAndIncCounter(context);
			string originalSort = context.ParentOriginalSort + innerSort;
			dr["ORIGINAL_SORT"] = originalSort;
			dr["GLOBAL_SORT"] = originalSort;
			dr["RANK_CODE"] = AD2DBHelper.ConvertDepartmentRankCode(helper.GetSearchResultPropertyStrValue("physicalDeliveryOfficeName", sr)).ToString();
			dr["CHILDREN_COUNTER"] = 0;
			dr["INNER_SORT"] = innerSort;
			dr["ORG_TYPE"] = (int)AD2DBHelper.TranslateDeptTypeDefine(sr);
			dr["ORG_CLASS"] = (int)AD2DBHelper.TranslateDeptClass(helper.GetSearchResultPropertyStrValue("c", sr));
			dr["STATUS"] = 1;
			dr["SEARCH_NAME"] = name + " " + NormalizeName(displayName);

			oguTable.Rows.Add(dr);
		}

		private static string GetInnerSortAndIncCounter(AD2DBTransferContext context)
		{
			return string.Format("{0:000000}", context.ParentChildrenCount++);
		}

		private static string NormalizeName(string name)
		{
			//Regex regex = new Regex("([A-Za-z]{1,2})");

			//return regex.Replace(name, "");
			return name;
		}

		/// <summary>
		/// 添加用户和OU_USERS(用户信息扩展表需要操作，现在还没操作)
		/// </summary>
		/// <param name="sr"></param>
		/// <param name="context"></param>
		/// <param name="ts"></param>
		private static void InsertUsersInfoExtend(SearchResult sr, AD2DBTransferContext context)
		{
			ADHelper helper = context.InitialParams.DirectoryHelper;

			//判断是否需要更新电话号码
			string mobile = helper.GetSearchResultPropertyStrValue("mobile", sr);
			string telephoneNumber = helper.GetSearchResultPropertyStrValue("telephoneNumber", sr);

			//特殊处理手机号
			int num = mobile.IndexOf('/');

			string newmobile = string.Empty;

			if (num != -1)
			{
				newmobile = mobile.Substring(num - 11, 11);
			}
			else if (mobile.Length >= 11)
			{
				newmobile = mobile.Substring(mobile.Length - 11, 11);
			}

			DataTable usersInfoExtendTable = context.ADData.Tables["USERS_INFO_EXTEND"];
			DataRow dr = usersInfoExtendTable.NewRow();

			dr["ID"] = helper.GetSearchResultPropertyStrValue("objectGuid", sr);
			dr["MOBILE"] = newmobile;
			dr["OFFICE_TEL"] = telephoneNumber;
			dr["GENDER"] = 0;
			dr["NATION"] = string.Empty;
			dr["IM_ADDRESS"] = helper.GetSearchResultPropertyStrValue("msRTCSIP-PrimaryUserAddress", sr);

			usersInfoExtendTable.Rows.Add(dr);
		}

		private static void InsertUser(SearchResult sr, AD2DBTransferContext context)
		{
			ADHelper helper = context.InitialParams.DirectoryHelper;

			string firstName = helper.GetSearchResultPropertyStrValue("givenName", sr);
			string lastName = helper.GetSearchResultPropertyStrValue("sn", sr);
			string logonName = helper.GetSearchResultPropertyStrValue("samAccountName", sr);
			string pinyin = logonName;

			DataTable usersTable = context.ADData.Tables["USERS"];
			DataRow dr = usersTable.NewRow();

			dr["GUID"] = helper.GetSearchResultPropertyStrValue("objectGuid", sr);

			dr["FIRST_NAME"] = firstName;
			dr["LAST_NAME"] = lastName;
			dr["LOGON_NAME"] = logonName;
			dr["PWD_TYPE_GUID"] = "21545d16-a62f-4a7e-ac2f-beca958e0fdf";
			dr["USER_PWD"] = "B0-81-DB-E8-5E-1E-C3-FF-C3-D4-E7-D0-22-74-00-CD";//password "E9-D5-C2-CA-1D-E4-0B-E1-2E-66-CC-57-9D-11-B9-FF";//000000
			dr["RANK_CODE"] = AD2DBHelper.ConvertUserRankCode(helper.GetSearchResultPropertyStrValue("personalTitle", sr)).ToString();
			dr["POSTURAL"] = 2;
			dr["PINYIN"] = pinyin;
			dr["E_MAIL"] = helper.GetSearchResultPropertyStrValue("mail", sr);

			usersTable.Rows.Add(dr);
		}

		private static void InsertOUUser(SearchResult sr, AD2DBTransferContext context)
		{
			ADHelper helper = context.InitialParams.DirectoryHelper;

			string logonName = helper.GetSearchResultPropertyStrValue("samAccountName", sr);
			string pinyin = logonName;

			string displayName = helper.GetSearchResultPropertyStrValue("displayName", sr);

            //by v-zhangbm 20120220 start
            //判断是否需要更新电话号码
            string mobile = helper.GetSearchResultPropertyStrValue("mobile", sr);
            string telephoneNumber = helper.GetSearchResultPropertyStrValue("telephoneNumber", sr);

            //特殊处理手机号
            int num = mobile.IndexOf('/');

            string newmobile = string.Empty;
            if (num != -1)
            {
                newmobile = mobile.Substring(num - 11, 11);
            }
            else if (mobile.Length >= 11)
            {
                newmobile = mobile.Substring(mobile.Length - 11, 11);
            }

            string email = helper.GetSearchResultPropertyStrValue("mail", sr);
            string newEmail = string.Empty;

            if (!string.IsNullOrEmpty(email))
            {
                int index = email.IndexOf('@');
                if (index != -1)
                {
                    newEmail = email.Substring(0, index);
                }
            }
            //by v-zhangbm 20120220 end

			if (string.IsNullOrEmpty(displayName))
			{
				string firstName = helper.GetSearchResultPropertyStrValue("givenName", sr);
				string lastName = helper.GetSearchResultPropertyStrValue("sn", sr);

				displayName = lastName + firstName;	//DisplayName= givenname + sn
			}

			DataTable ouusersTable = context.ADData.Tables["OU_USERS"];
			DataRow dr = ouusersTable.NewRow();

			dr["USER_GUID"] = helper.GetSearchResultPropertyStrValue("objectGuid", sr);
			dr["PARENT_GUID"] = context.ParentGuid;
			dr["ALL_PATH_NAME"] = AD2DBHelper.TranslateDNToFullPath(helper.GetSearchResultPropertyStrValue("distinguishedName", sr));
			dr["DISPLAY_NAME"] = displayName;
			dr["OBJ_NAME"] = helper.GetSearchResultPropertyStrValue("name", sr);

			if (helper.GetUserAccountPolicy(sr).UserAccountDisabled)
				dr["STATUS"] = 3;
			else
				dr["STATUS"] = 1;

			string innerSort = GetInnerSortAndIncCounter(context);
			string originalSort = context.ParentOriginalSort + innerSort;

			dr["ORIGINAL_SORT"] = originalSort;
			dr["GLOBAL_SORT"] = originalSort;
			dr["RANK_NAME"] = helper.GetSearchResultPropertyStrValue("title", sr);
			dr["DESCRIPTION"] = helper.GetSearchResultPropertyStrValue("description", sr);
			dr["ATTRIBUTES"] = (int)AD2DBHelper.TranslateUserAttributes(sr);
            //增加几个搜索条件v-zhangbm 20120220
            dr["SEARCH_NAME"] = displayName + " " + pinyin + " " + logonName + " " + telephoneNumber + " " + newmobile + " " + newEmail;

			ouusersTable.Rows.Add(dr);
		}

		#endregion
	}
}
