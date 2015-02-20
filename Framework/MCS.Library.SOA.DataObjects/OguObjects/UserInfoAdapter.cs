using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// �û���Ϣ�ĸ�������
	/// </summary>
	public class UserInfoAdapter
	{
		public static readonly UserInfoAdapter Instance = new UserInfoAdapter();

		private UserInfoAdapter()
		{
		}

		/// <summary>
		/// �õ��û������м�ְ��Ϣ��TopOU
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
