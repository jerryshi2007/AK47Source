using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Caching;
using MCS.Library.SOA.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.OA.CommonPages.UserInfoExtend
{
	public class SearchQuery
	{
		public OguObjectCollection<IUser> GetIUserWithCount(string userName, string departmentId, ref int totalCount)
		{
			IOrganization dept = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.Guid, departmentId)[0];

			OguObjectCollection<IUser> users = null;
			if (userName.Equals("@SearchAll@"))
			{
				users = dept.QueryChildren<IUser>(userName, true, SearchLevel.OneLevel, 100);
			}
			else
			{
				users = dept.QueryChildren<IUser>(userName, true, SearchLevel.SubTree, 100);
			}

			UserInfoExtendCollection extendCollection =
				UserInfoExtendDataObjectAdapter.Instance.GetUserInfoExtendInfoCollectionByUsers(users);

			UserReportInfoCollection reportLines =
				UserReportInfoAdapter.Instance.LoadUsersReportInfo(users);

			foreach (IUser extend in users)
			{
				UserInfoExtendDataObject ogu = extendCollection.Find(e => string.Compare(e.ID, extend.ID, true) == 0);

				if (ogu != null)
					FillUserExtendProperties(extend, ogu);

				UserReportInfo reportInfo = reportLines.Find(r => string.Compare(r.User.ID, extend.ID, true) == 0);

				if (reportInfo != null)
					extend.Properties["ReportTo"] = reportInfo;
			}

			totalCount = users.Count;
			return new OguObjectCollection<IUser>(users.ToList().OrderBy(u => !u.IsSideline).ToArray());

		}

		private static void FillUserExtendProperties(IUser user, UserInfoExtendDataObject extendInfo)
		{
			user.Properties["Mobile"] = extendInfo.Mobile;
			user.Properties["OfficeTel"] = extendInfo.OfficeTel;
			user.Properties["IntranetEmail"] = extendInfo.IntranetEmail;
			user.Properties["InternetEmail"] = extendInfo.InternetEmail;
			user.Properties["IMAddress"] = extendInfo.IMAddress;
		}

	}
}