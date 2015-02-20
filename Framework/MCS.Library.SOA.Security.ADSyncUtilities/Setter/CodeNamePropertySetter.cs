using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class CodeNamePropertySetter : SimplePropertySetter
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
					entry.Properties[targetPropertyName].Value = srcOguObject.Properties[srcPropertyName];
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
