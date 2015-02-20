using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using System.DirectoryServices;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class UserCodeNamePropertySetter : SimplePropertySetter
	{
		protected override void SetPropertyValue(IOguObject srcOguObject, string srcPropertyName, DirectoryEntry entry, string targetPropertyName, string context, SetterContext setterContext)
		{
			string srcPropertyValue = GetNormalizeddSourceValue(srcOguObject, srcPropertyName, context);
			string targetPropertyValue = GetNormalizeddTargetValue(entry, targetPropertyName, context);

			if (srcPropertyValue != targetPropertyValue)
			{
				//entry.CommitChanges();
				try
				{
					string targetValue = srcOguObject.Properties[srcPropertyName].ToString();
					entry.Properties[targetPropertyName].Value = targetValue;
					entry.Properties["userPrincipalName"].Value = targetValue + (string.IsNullOrEmpty(targetValue) ? string.Empty : context);
					// entry.CommitChanges();
				}
				catch (DirectoryServicesCOMException ex)
				{
					if (ex.ErrorCode == -2147019886)
					{
						//对象已存在
						entry.Properties[targetPropertyName].Value = "TMP" + Environment.TickCount.ToString("X");
						entry.CommitChanges();
						SynchronizeContext.Current.DelayActions.Add(new DelayRenameCodeNameAction(srcOguObject, srcPropertyName, entry.NativeGuid, targetPropertyName));
					}
					else
					{
						throw;
					}
				}
			}
		}
	}
}
