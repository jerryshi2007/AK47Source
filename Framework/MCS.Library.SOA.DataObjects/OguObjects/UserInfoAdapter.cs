using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户信息的辅助工具
	/// </summary>
	public class UserInfoAdapter
	{
		public static readonly UserInfoAdapter Instance = new UserInfoAdapter();

		private UserInfoAdapter()
		{
		}

		/// <summary>
		/// 得到用户的所有兼职信息的TopOU
		/// </summary>
		/// <returns></returns>
		public OguDataCollection<IOrganization> GetCurrentAllTopOUs()
		{
			OguDataCollection<IOrganization> result = new OguDataCollection<IOrganization>();

			foreach (IUser user in DeluxeIdentity.CurrentUser.AllRelativeUserInfo)
				result.Add(user.TopOU);

			return result;
		}
	}
}
