using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public static class ObjectSetterHelper
	{
		public static void Convert(ObjectModifyType modifyType, IOguObject srcObject, DirectoryEntry targetObject)
		{
			SchemaType schemaType = GetOguSchemaType(targetObject);
			OguAndADObjectSetterBase setter = GetSetter(modifyType, schemaType);

			if (setter != null)
			{
				SchemaMappingInfo mappingInfo = PermissionCenterToADSynchronizeSettings.GetConfig().SchemaMappings.GetSchemaMappingInfo(schemaType.ToString());

				string context = string.Empty;

				if (mappingInfo.ModifyOperations.ContainsKey(modifyType.ToString()))
				{
					SetterObjectMappingConfigurationElement objSetterMappingElement = mappingInfo.ModifyOperations[modifyType.ToString()];
					context = objSetterMappingElement.Context;
				}

				setter.Convert(modifyType, srcObject, targetObject, context);
			}
		}

		private static OguAndADObjectSetterBase GetSetter(ObjectModifyType modifyType, SchemaType schemaType)
		{
			string schemaTypeName = schemaType.ToString();

			string cacheKey = "OguAndADObjectSetterBase" + "-" + modifyType.ToString() + "+" + schemaTypeName;

			return (OguAndADObjectSetterBase)ObjectContextCache.Instance.GetOrAddNewValue(cacheKey, (cache, key) =>
			{
				SchemaMappingInfo mappingInfo = PermissionCenterToADSynchronizeSettings.GetConfig().SchemaMappings.GetSchemaMappingInfo(schemaTypeName);

				OguAndADObjectSetterBase setter = null;

				if (mappingInfo.ModifyOperations.ContainsKey(modifyType.ToString()))
				{
					SetterObjectMappingConfigurationElement objSetterMappingElement = mappingInfo.ModifyOperations[modifyType.ToString()];
					setter = (OguAndADObjectSetterBase)PropertySettersSettings.GetConfig().ObjectSetters[objSetterMappingElement.OperationName].CreateInstance();
				}

				cache.Add(cacheKey, setter);

				return setter;
			});
		}

		private static SchemaType GetOguSchemaType(DirectoryEntry targetObject)
		{
			return (SchemaType)targetObject.ToADSchemaType();
		}
	}
}