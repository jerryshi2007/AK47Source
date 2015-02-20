using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class DeleteADUserObjectSetter : DeleteADObjectSetter
	{
		protected override void ProcessDeleteEntry(ObjectModifyType modifyType, IOguObject srcObject, DirectoryEntry targetObject, string context)
		{
			DisableAccount(targetObject);
		}
	}
}