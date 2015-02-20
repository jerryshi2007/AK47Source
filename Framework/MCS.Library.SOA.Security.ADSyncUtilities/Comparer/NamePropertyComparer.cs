using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	/// <summary>
	/// 比较名称是否相同
	/// </summary>
	public class NamePropertyComparer : SimplePropertyComparer
	{
		protected override bool ComparePropertyValue(OGUPermission.IOguObject srcOguObject, string srcPropertyName, ADObjectWrapper adObject, string targetPropertyName, string context)
		{
			if (srcOguObject.FullPath == SynchronizeContext.Current.SourceRootPath)
			{
				//名称以权限中心为准
				return adObject.DN == SynchronizeHelper.AppendNamingContext(SynchronizeContext.Current.TargetRootOU);
			}
			else
			{
				string path = SynchronizeHelper.GetRelativePath(srcOguObject);
				if (path.IndexOf('\\') > 0)
				{
					return base.ComparePropertyValue(srcOguObject, srcPropertyName, adObject, targetPropertyName, context);
				}
				else
				{
					string dn = SynchronizeContext.Current.GetMappedName(srcOguObject.Name);
					return adObject.DN == SynchronizeHelper.AppendNamingContext(dn);
				}
			}
		}
	}
}
