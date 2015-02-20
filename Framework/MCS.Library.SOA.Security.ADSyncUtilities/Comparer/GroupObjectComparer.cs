using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class GroupObjectComparer : OguAndADObjectComparerBase
	{
		public override ObjectModifyType Compare(IOguObject srcObject, ADObjectWrapper targetObject)
		{
			ObjectModifyType result = CompareProperties(srcObject, targetObject);

			//if (IsChildrenModified(srcObject, targetObject))
			//    result |= ObjectModifyType.ChildrenModified;

			if (targetObject.Properties.Contains("displayNamePrintable") == false || SynchronizeHelper.PermissionCenterInvolved.Equals(targetObject.Properties["displayNamePrintable"]) == false)
			{
				result |= ObjectModifyType.MissingMarker;
			}

			return result;
		}

		[Obsolete]
		private static bool IsChildrenModified(IOguObject oguObject, ADObjectWrapper targetObject)
		{
			ADHelper adHelper = SynchronizeContext.Current.ADHelper;
			SearchResult result = SynchronizeHelper.GetSearchResultByDN(adHelper, targetObject.DN, ADSchemaType.Groups); //当前肯定有改组存在
			ResultPropertyValueCollection adMembers = result.Properties["member"];

			IUser[] oguUsers = ((IGroup)oguObject).Members.Where(u => u.FullPath.StartsWith(SynchronizeContext.Current.StartPath) && u.IsSideline == false).ToArray();


			if (adMembers.Count != oguUsers.Count())
				return true;

			foreach (var memberDN in adMembers)
			{
				bool exists = false;
				for (int i = oguUsers.Length - 1; i >= 0; i--)
				{
					if (SynchronizeHelper.AppendNamingContext(SynchronizeHelper.GetOguObjectDN(oguUsers[i])) == memberDN.ToString())
					{
						exists = true;
						break;
					}
				}
				if (exists == false)
					return true;
			}

			return false;
		}
	}
}