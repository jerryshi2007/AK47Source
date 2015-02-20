using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.Core;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class StringCollectionPropertySetter : OguAndADObjectPropertySetterBase
	{
		protected override void SetPropertyValue(IOguObject srcOguObject, string srcPropertyName, System.DirectoryServices.DirectoryEntry entry, string targetPropertyName, string context, SetterContext setterContext)
		{
			string srcValue = (string)srcOguObject.Properties[srcPropertyName];
			var targetValues = entry.Properties[targetPropertyName];

			if (entry.IsBounded())
			{
				RefillCollection(srcValue, targetValues);
			}
			else
			{
				// 无中生有,此时检查有无修改，如果有修改，则应延迟操作。
				if (string.IsNullOrEmpty(srcValue) == false)
				{
					setterContext["StringCollectionPropertySetter_LazySetProperties"] = true;
				}
			}
		}

		private static void RefillCollection(string srcValue, System.DirectoryServices.PropertyValueCollection targetValues)
		{
			targetValues.Clear();
			if (string.IsNullOrWhiteSpace(srcValue) == false)
			{
				string[] values = srcValue.Split(';');
				for (int i = values.Length - 1; i >= 0; i--)
				{
					if (string.IsNullOrWhiteSpace(values[i]) == false)
						targetValues.Add(values[i]);
				}
			}
		}

		public override void AfterObjectUpdated(IOguObject srcOguObject, string srcPropertyName, System.DirectoryServices.DirectoryEntry entry, string targetPropertyName, string context, SetterContext setterContext)
		{
			if (DictionaryHelper.GetValue<string, object, bool>(setterContext, "StringCollectionPropertySetter_LazySetProperties", false))
			{
				RefillCollection((string)srcOguObject.Properties[srcPropertyName], entry.Properties[targetPropertyName]);

				setterContext.PropertyChanged = true;
			}
		}
	}
}
