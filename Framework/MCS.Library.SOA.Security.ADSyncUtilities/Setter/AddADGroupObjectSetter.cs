using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class AddADGroupObjectSetter : OguAndADObjectSetterBase
	{
		public override void Convert(DataObjects.Security.Transfer.ObjectModifyType modifyType, OGUPermission.IOguObject srcObject, System.DirectoryServices.DirectoryEntry targetObject, string context)
		{
			IGroup grp = (IGroup)srcObject;
			SetterContext setterContext = new SetterContext();

			ConvertProperties(srcObject, targetObject, setterContext);

			targetObject.Properties["displayNamePrintable"].Value = SynchronizeHelper.PermissionCenterInvolved;

			targetObject.CommitChanges();

			DoAfterObjectUpdatedOP(srcObject, targetObject, setterContext);
		}
	}
}
