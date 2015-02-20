using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.Caching;
using System.DirectoryServices;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public static class ObjectComparerHelper
	{
		public static ObjectModifyType Compare(IOguObject oguObject, ADObjectWrapper adObject)
		{
			OguAndADObjectComparerBase comparer = GetComparer(oguObject);

			return comparer.Compare(oguObject, adObject);
		}

		internal static string SchemaTypeToPrefix(this SchemaType schemaType)
		{
			string cacheKey = "OguAndADObjectComparerBasePrefix" + "-" + schemaType.ToString();

			return (string)ObjectContextCache.Instance.GetOrAddNewValue(cacheKey, (cache, key) =>
			{
				string prefix = PermissionCenterToADSynchronizeSettings.GetConfig().SchemaMappings.GetSchemaMappingInfo(schemaType.ToString()).Prefix;

				cache.Add(cacheKey, prefix);

				return prefix;
			});
		}

		/// <summary>
		/// 判断AD对象的DN和ogu对象的Path是否相同，此处需要考虑转义的测试
		/// </summary>
		/// <param stringValue="oguObject"></param>
		/// <param stringValue="adObject"></param>
		/// <returns></returns>
		public static bool ArePathEqaul(IOguObject oguObject, ADObjectWrapper adObject)
		{
			var localPathToDN = SynchronizeHelper.AppendNamingContext(SynchronizeHelper.GetOguObjectDN(oguObject));
			return localPathToDN == adObject.DN;
		}

		public static bool AreParentPathEqaul(IOguObject oguObject, ADObjectWrapper adObject)
		{
			var localPathToDN = SynchronizeHelper.AppendNamingContext(SynchronizeHelper.GetParentObjectDN(oguObject));
			return localPathToDN == ADHelper.GetParentRdnSequence(adObject.DN);
		}

		public static bool AreParentPathEqaul(IOguObject oguObject, DirectoryEntry entry)
		{
			var localPathToDN = SynchronizeHelper.AppendNamingContext(SynchronizeHelper.GetParentObjectDN(oguObject));
			return localPathToDN == ADHelper.GetParentRdnSequence(entry.Properties["distinguishedName"].Value.ToString());
		}

		private static OguAndADObjectComparerBase GetComparer(IOguObject oguObject)
		{
			string schemaTypeName = oguObject.ObjectType.ToString();

			string cacheKey = "OguAndADObjectComparerBase" + "-" + schemaTypeName;

			return (OguAndADObjectComparerBase)ObjectContextCache.Instance.GetOrAddNewValue(cacheKey, (cache, key) =>
			{
				SchemaMappingInfo mappingInfo = PermissionCenterToADSynchronizeSettings.GetConfig().SchemaMappings.GetSchemaMappingInfo(schemaTypeName);

				OguAndADObjectComparerBase comparer = (OguAndADObjectComparerBase)PropertyComparersSettings.GetConfig().ObjectComparers[mappingInfo.ComparerName].CreateInstance();

				cache.Add(cacheKey, comparer);

				return comparer;
			});
		}
	}
}