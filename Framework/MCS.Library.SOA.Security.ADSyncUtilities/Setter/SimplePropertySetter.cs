using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using System.Diagnostics;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class SimplePropertySetter : OguAndADObjectPropertySetterBase
	{
		protected override void SetPropertyValue(IOguObject srcOguObject, string srcPropertyName, DirectoryEntry entry, string targetPropertyName, string context, SetterContext setterContext)
		{
			string srcPropertyValue = GetNormalizeddSourceValue(srcOguObject, srcPropertyName, context);
			string targetPropertyValue = GetNormalizeddTargetValue(entry, targetPropertyName, context);

			if (srcPropertyValue != targetPropertyValue)
			{
				TraceItHere(srcOguObject.ObjectType.ToString(), srcPropertyName, entry.Name, targetPropertyName, context, srcPropertyValue, targetPropertyValue);
				entry.Properties[targetPropertyName].Value = srcPropertyValue;

				TracePropertyValue(entry, targetPropertyName);
			}
		}

		[Conditional("PC_TRACE")]
		private void TraceItHere(string srcObjName, string srcPropertyName, string targetObjName, string targetPropertyName, string context, string srcPropertyValue, string targetPropertyValue)
		{
			this.TraceIt(srcObjName, srcPropertyName, targetObjName, targetPropertyName, context, srcPropertyValue, targetPropertyValue);
		}

		[Conditional("PC_TRACE")]
		private void TracePropertyValue(DirectoryEntry ent, string name)
		{
			Trace.WriteLine("跟踪属性值 " + name + "=" + ent.Properties[name].Value);
		}

		protected static string GetNormalizeddSourceValue(IOguObject srcOguObject, string srcPropertyName, string context)
		{
			string srcPropertyValue = null;

			if (srcOguObject.Properties[srcPropertyName] != null)
			{
				srcPropertyValue = srcOguObject.Properties[srcPropertyName].ToString();

				if (srcPropertyValue == string.Empty)
					srcPropertyValue = null;
			}

			return srcPropertyValue;
		}

		protected static string GetNormalizeddTargetValue(DirectoryEntry entry, string targetPropertyName, string context)
		{
			string targetPropertyValue = null;

			object originalValue = entry.Properties[targetPropertyName].Value;

			if (originalValue != null)
			{
				targetPropertyValue = originalValue.ToString();

				if (targetPropertyValue == string.Empty)
					targetPropertyValue = null;
			}

			return targetPropertyValue;
		}
	}
}