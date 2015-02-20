using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public abstract class OguAndADObjectPropertyComparerBase : PropertyComparerBase
	{
		public override bool Compare(object srcObject, string srcPropertyName, object targetObject, string targetPropertyName, string context)
		{
			(srcObject is IOguObject).FalseThrow("srcObject必须是IOguObject类型");
			(targetObject is ADObjectWrapper).FalseThrow("targetObject必须是ADObjectWrapper类型");

			IOguObject srcOguObject = (IOguObject)srcObject;
			ADObjectWrapper adObject = (ADObjectWrapper)targetObject;

			return ComparePropertyValue(srcOguObject, srcPropertyName, adObject, targetPropertyName, context);
		}

		protected abstract bool ComparePropertyValue(IOguObject srcOguObject, string srcPropertyName, ADObjectWrapper adObject, string targetPropertyName, string context);
	}
}