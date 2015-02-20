using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Setter
{
	public class GroupMarkerSetter : OguAndADObjectPropertySetterBase
	{
		protected override void SetPropertyValue(OGUPermission.IOguObject srcOguObject, string srcPropertyName, System.DirectoryServices.DirectoryEntry entry, string targetPropertyName, string context, DataObjects.Security.Transfer.SetterContext setterContext)
		{
			entry.Properties[srcPropertyName].Value = SynchronizeHelper.PermissionCenterInvolved;
			entry.CommitChanges();
		}
	}
}
