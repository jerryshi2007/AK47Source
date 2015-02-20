using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class AddADObjectSetter : OguAndADObjectSetterBase
	{
		public override void Convert(ObjectModifyType modifyType, IOguObject srcObject, DirectoryEntry targetObject, string context)
		{
			SetterContext setterContext = new SetterContext();

			ConvertProperties(srcObject, targetObject, setterContext);

			targetObject.CommitChanges();

			DoAfterObjectUpdatedOP(srcObject, targetObject, setterContext);
		}
	}
}