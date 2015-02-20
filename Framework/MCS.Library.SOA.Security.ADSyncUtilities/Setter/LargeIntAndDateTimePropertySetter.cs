using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using System.Diagnostics;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class LargeIntAndDateTimePropertySetter : OguAndADObjectPropertySetterBase
	{
		protected override void SetPropertyValue(IOguObject srcOguObject, string srcPropertyName, DirectoryEntry entry, string targetPropertyName, string context, SetterContext setterContext)
		{
			entry.InvokeSet(targetPropertyName,
				new object[] { SynchronizeHelper.GetADAccountExpiresDate(srcOguObject.Properties[srcPropertyName]) });

			TraceItHere(srcOguObject.ObjectType.ToString(), srcPropertyName, entry.Name, targetPropertyName, context, srcOguObject.Properties[srcPropertyName].ToString(), SynchronizeHelper.GetADAccountExpiresDate(srcOguObject.Properties[srcPropertyName]).ToLongTimeString());
		}

		[Conditional("PC_TRACE")]
		private void TraceItHere(string srcObjName, string srcPropertyName, string targetObjName, string targetPropertyName, string context, string srcPropertyValue, string targetPropertyValue)
		{
			this.TraceIt(srcObjName, srcPropertyName, targetObjName, targetPropertyName, context, srcPropertyValue, targetPropertyValue);
		}
	}
}