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
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using System.Runtime.InteropServices;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class ObjNamePropertySetter : SimplePropertySetter
	{
		protected override void SetPropertyValue(IOguObject srcOguObject, string srcPropertyName, DirectoryEntry entry, string targetPropertyName, string context, SetterContext setterContext)
		{
			string srcPropertyValue = GetNormalizeddSourceValue(srcOguObject, srcPropertyName, context);
			string targetPropertyValue = GetNormalizeddTargetValue(entry, targetPropertyName, context);

			if (srcOguObject.FullPath == SynchronizeContext.Current.SourceRootPath)
			{
				srcPropertyValue = new PathPartEnumerator(SynchronizeContext.Current.TargetRootOU).Last(); //极其特别，不一定可靠，权限中心应限制更改这一组织的名称和位置。
			}
			else
			{
				string relativePath = SynchronizeHelper.GetRelativePath(srcOguObject);
				if (relativePath.IndexOf('\\') < 0)
				{
					srcPropertyValue = SynchronizeContext.Current.GetMappedName(srcPropertyValue);
				}
			}

			if (srcPropertyValue != targetPropertyValue && entry.IsBounded() == true)
			{
				TraceItHere(srcOguObject, srcPropertyName, entry, targetPropertyName, context, srcPropertyValue, targetPropertyValue);
				entry.CommitChanges();
				try
				{
					entry.Rename(srcOguObject.ObjectType.SchemaTypeToPrefix() + "=" + ADHelper.EscapeString(srcPropertyValue));
				}
				catch (DirectoryServicesCOMException ex)
				{
					if (ex.ErrorCode == -2147019886)
					{
						//对象已存在
						entry.Rename(srcOguObject.ObjectType.SchemaTypeToPrefix() + "=TMP" + Environment.TickCount.ToString("X"));
						SynchronizeContext.Current.DelayActions.Add(new DelayRenameAction(srcOguObject, entry.NativeGuid));
					}
					else
					{
						throw;
					}
				}
			}
		}

		[Conditional("PC_TRACE")]
		private void TraceItHere(IOguObject srcOguObject, string srcPropertyName, DirectoryEntry entry, string targetPropertyName, string context, string srcPropertyValue, string targetPropertyValue)
		{
			this.TraceIt(srcOguObject.Name, srcPropertyName, entry.Name, targetPropertyName, context, srcPropertyValue, targetPropertyValue);
		}
	}
}