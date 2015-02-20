using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public abstract class OguAndADObjectSetterBase : ObjectSetterBase<IOguObject, DirectoryEntry>
	{
		protected static void ConvertProperties(IOguObject srcObject, DirectoryEntry entry, SetterContext setterContext)
		{
			SchemaMappingInfo mappingInfo = PermissionCenterToADSynchronizeSettings.GetConfig().SchemaMappings.GetSchemaMappingInfo(srcObject.ObjectType.ToString());

			foreach (SetterPropertyMappingConfigurationElement element in mappingInfo.ModifiedProperties)
			{
				element.Setter.SetValue(srcObject, element.Name, entry, element.TargetPropertyName, element.Context, setterContext);
			}
		}

		protected static void ConvertProperties(IOguObject srcObject, DirectoryEntry entry, SetterContext setterContext, bool delayed)
		{
			SchemaMappingInfo mappingInfo = PermissionCenterToADSynchronizeSettings.GetConfig().SchemaMappings.GetSchemaMappingInfo(srcObject.ObjectType.ToString());

			foreach (SetterPropertyMappingConfigurationElement element in mappingInfo.ModifiedProperties)
			{
				if (element.Delay == delayed)
					element.Setter.SetValue(srcObject, element.Name, entry, element.TargetPropertyName, element.Context, setterContext);
			}
		}

		protected static void DoAfterObjectUpdatedOP(IOguObject srcObject, DirectoryEntry entry, SetterContext setterContext)
		{
			SchemaMappingInfo mappingInfo = PermissionCenterToADSynchronizeSettings.GetConfig().SchemaMappings.GetSchemaMappingInfo(srcObject.ObjectType.ToString());

			foreach (SetterPropertyMappingConfigurationElement element in mappingInfo.ModifiedProperties)
			{
				element.Setter.AfterObjectUpdated(srcObject, element.Name, entry, element.TargetPropertyName, element.Context, setterContext);
			}

			if (setterContext.PropertyChanged)
				entry.CommitChanges();
		}
	}
}