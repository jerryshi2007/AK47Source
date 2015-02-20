using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class DeleteADOrganizationObjectSetter : DeleteADObjectSetter
	{
		protected override void ProcessDeleteEntry(ObjectModifyType modifyType, IOguObject srcObject, DirectoryEntry targetObject, string context)
		{
			List<ADObjectWrapper> allChildren = targetObject.FindAllChildrenUser();

			foreach (ADObjectWrapper child in allChildren)
			{
				using (DirectoryEntry subEntry = SynchronizeHelper.GetSearchResultByID(SynchronizeContext.Current.ADHelper, child.NativeGuid).GetDirectoryEntry())
				{
					subEntry.ForceBound();
					DisableAccount(subEntry);
				}
			}
		}
	}
}