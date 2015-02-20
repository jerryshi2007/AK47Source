using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	/// <summary>
	/// 注意，在AD中为字符串用分号分隔
	/// </summary>
	public class StringCollectionPropertyComparer : SimplePropertyComparer
	{
		protected override bool ComparePropertyValue(OGUPermission.IOguObject srcOguObject, string srcPropertyName, ADObjectWrapper adObject, string targetPropertyName, string context)
		{
			string srcPropertyValue = null;

			if (srcOguObject.Properties[srcPropertyName] != null)
				srcPropertyValue = srcOguObject.Properties[srcPropertyName] as string;

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
