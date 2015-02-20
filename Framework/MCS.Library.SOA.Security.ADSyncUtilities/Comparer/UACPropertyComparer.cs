using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class UACPropertyComparer : OguAndADObjectPropertyComparerBase
	{
		protected override bool ComparePropertyValue(IOguObject srcOguObject, string srcPropertyName, ADObjectWrapper adObject, string targetPropertyName, string context)
		{
			int userAccountControl = Convert.ToInt32(adObject.Properties[targetPropertyName]);

			ADS_USER_FLAG uacFlag = ADS_USER_FLAG.ADS_UF_NONE;

			bool result = true;

			if (Enum.TryParse(context, out uacFlag))
			{
				bool originalFlag = ((userAccountControl & (int)uacFlag) == (int)uacFlag);

				result = originalFlag == (bool)srcOguObject.Properties[srcPropertyName];
			}

			return result;
		}
	}
}