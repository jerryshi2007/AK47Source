using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public abstract class OguAndADObjectPropertySetterBase : PropertySetterBase
	{
		public override void SetValue(object srcObject, string srcPropertyName, object targetObject, string targetPropertyName, string context, SetterContext setterContext)
		{
			(srcObject is IOguObject).FalseThrow("srcObject必须是IOguObject类型");
			(targetObject is DirectoryEntry).FalseThrow("targetObject必须是DirectoryEntry类型");

			IOguObject srcOguObject = (IOguObject)srcObject;
			DirectoryEntry entry = (DirectoryEntry)targetObject;

			SetPropertyValue(srcOguObject, srcPropertyName, entry, targetPropertyName, context, setterContext);
		}

		protected abstract void SetPropertyValue(IOguObject srcOguObject, string srcPropertyName, DirectoryEntry entry, string targetPropertyName, string context, SetterContext setterContext);

		public virtual void AfterObjectUpdated(IOguObject srcOguObject, string srcPropertyName, DirectoryEntry entry, string targetPropertyName, string context, SetterContext setterContext)
		{
		}

		public override void AfterObjectUpdated(object srcObject, string srcPropertyName, object targetObject, string targetPropertyName, string context, SetterContext setterContext)
		{
			(srcObject is IOguObject).FalseThrow("srcObject必须是IOguObject类型");
			(targetObject is DirectoryEntry).FalseThrow("targetObject必须是DirectoryEntry类型");

			IOguObject srcOguObject = (IOguObject)srcObject;
			DirectoryEntry entry = (DirectoryEntry)targetObject;

			AfterObjectUpdated(srcOguObject, srcPropertyName, entry, targetPropertyName, context, setterContext);
		}
	}
}