using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class AddADUserObjectSetter : AddADObjectSetter
	{
		public override void Convert(ObjectModifyType modifyType, IOguObject srcObject, DirectoryEntry targetObject, string context)
		{
			SetterContext setterContext = new SetterContext();

			ConvertProperties(srcObject, targetObject, setterContext);

			targetObject.CommitChanges();

			if (SynchronizeContext.Current.DefaultPassword.IsNotEmpty())
				targetObject.Invoke("SetPassword", SynchronizeContext.Current.DefaultPassword);

			DoAfterObjectUpdatedOP(srcObject, targetObject, setterContext);
		}
	}
}