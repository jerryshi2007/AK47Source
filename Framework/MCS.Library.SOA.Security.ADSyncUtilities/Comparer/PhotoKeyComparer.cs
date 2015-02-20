using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class PhotoKeyComparer : SimplePropertyComparer
	{
		protected override bool ComparePropertyValue(IOguObject srcOguObject, string srcPropertyName, ADObjectWrapper adObject, string targetPropertyName, string context)
		{
			return base.ComparePropertyValue(srcOguObject, srcPropertyName, adObject, targetPropertyName, context);
		}
	}
}
