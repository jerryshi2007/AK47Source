using System;
using System.Text;
using System.Collections.Generic;
using System.DirectoryServices;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 域用户的修改器
	/// </summary>
	public class DomainUserUpdater : ADSIUserUpdaterBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="strUserID"></param>
		/// <returns></returns>
		protected override DirectoryEntry GetUserDirectoryEntry(string strUserID)
		{
			ServerInfo si = GetServerInfo();

			ADHelper helper = ADHelper.GetInstance(si);

			using (DirectoryEntry root = helper.GetRootEntry())
			{
				string filter = ADSearchConditions.GetFilterByMask(ADSchemaType.Users,
					new ExtraFilter(string.Empty, "(samAccountName=" + strUserID + ")", string.Empty));

				ADSearchConditions condition = new ADSearchConditions(SearchScope.Subtree);

				List<SearchResult> result = helper.ExecuteSearch(root, filter, condition, "distinguishedName");
				ExceptionHelper.FalseThrow(result.Count > 0, "不能在域上找到登录名为{0}的用户", strUserID);

				return helper.NewEntry(helper.GetSearchResultPropertyStrValue("distinguishedName", result[0]));
			}
		}
	}
}
