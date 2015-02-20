using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class UACPropertySetter : OguAndADObjectPropertySetterBase
	{
		protected override void SetPropertyValue(IOguObject srcOguObject, string srcPropertyName, DirectoryEntry entry, string targetPropertyName, string context, SetterContext setterContext)
		{
			int userAccountControl = Convert.ToInt32(entry.Properties[targetPropertyName].Value); // 可能为null

			int result = userAccountControl;

			if (userAccountControl == 0 || entry.IsBounded() == false)
			{
				result = (int)(ADS_USER_FLAG.ADS_UF_NORMAL_ACCOUNT | ADS_USER_FLAG.ADS_UF_PASSWD_NOTREQD);
				entry.Properties[targetPropertyName].Value = result;

				setterContext["UACPropertySetter_LazySetProperties"] = true;
			}
			else
			{
				MergeUACValue(srcOguObject, srcPropertyName, entry, targetPropertyName, context);
			}
		}

		public override void AfterObjectUpdated(IOguObject srcOguObject, string srcPropertyName, DirectoryEntry entry, string targetPropertyName, string context, SetterContext setterContext)
		{
			if (DictionaryHelper.GetValue<string, object, bool>(setterContext, "UACPropertySetter_LazySetProperties", false))
			{
				MergeUACValue(srcOguObject, srcPropertyName, entry, targetPropertyName, context);

				setterContext.PropertyChanged = true;
			}
		}

		private static void MergeUACValue(IOguObject srcOguObject, string srcPropertyName, DirectoryEntry entry, string targetPropertyName, string context)
		{
			int userAccountControl = Convert.ToInt32(entry.Properties[targetPropertyName].Value);

			ADS_USER_FLAG uacFlag = ADS_USER_FLAG.ADS_UF_NONE;

			if (Enum.TryParse(context, out uacFlag))
			{
				bool srcPropertyValue = (bool)srcOguObject.Properties[srcPropertyName];

				if (srcPropertyValue)
					userAccountControl = userAccountControl | (int)uacFlag;
				else
					userAccountControl = userAccountControl & ~(int)(uacFlag);
			}

			entry.Properties[targetPropertyName].Value = userAccountControl;
		}
	}
}