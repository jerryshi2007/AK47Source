using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class SimplePropertyComparer : OguAndADObjectPropertyComparerBase
	{
		protected override bool ComparePropertyValue(IOguObject srcOguObject, string srcPropertyName, ADObjectWrapper adObject, string targetPropertyName, string context)
		{
			string srcPropertyValue = null;

			if (srcOguObject.Properties[srcPropertyName] != null)
				srcPropertyValue = srcOguObject.Properties[srcPropertyName].ToString();

			string targetPropertyValue = null;

			if (adObject.Properties[targetPropertyName] != null)
				targetPropertyValue = adObject.Properties[targetPropertyName].ToString();

			bool result = false;

			if (srcPropertyValue != null && targetPropertyValue != null)
				result = srcPropertyValue == targetPropertyValue;

			return result;
		}
	}
}