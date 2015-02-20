using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices;
using MCS.Library;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class ADObjectFinder
	{

		public static ADObjectWrapper Find(IOguObject oguObject)
		{
			return Find(oguObject, false);
		}

		public static ADObjectWrapper Find(IOguObject oguObject, bool keepExists)
		{
			ADObjectWrapper adObject = null;

			switch (oguObject.ObjectType)
			{
				case SchemaType.Organizations:
					adObject = FindOUObject(oguObject);
					break;
				case SchemaType.Users:
					adObject = FindUserObject(oguObject);
					break;
				case SchemaType.Groups:
					adObject = FindGroupObject(oguObject);
					break;
				default:
					throw new Exception("oguObject对象SchemaType不正确");
			}

			if (SynchronizeContext.Current.IDMapper.SCIDMappingDictionary.ContainsKey(oguObject.ID) == false)
			{
				{
					//这里要新增ID映射
					DateTime modifyTime = DateTime.MinValue;

					if (oguObject.Properties["VERSION_START_TIME"] != null)
					{
						DateTime.TryParse(oguObject.Properties["VERSION_START_TIME"].ToString(), out modifyTime);
					}

					var mapping = adObject == null ? null : new IDMapping()
					{
						SCObjectID = oguObject.ID,
						ADObjectGuid = adObject.NativeGuid,
						LastSynchronizedVersionTime = modifyTime
					};

					if (keepExists == false)
					{
						SynchronizeContext.Current.IDMapper.NewIDMappingDictionary[oguObject.ID] = mapping;
					}
					else if (mapping != null)
					{
						SynchronizeContext.Current.IDMapper.SCIDMappingDictionary.AddNotExistsItem(mapping);
					}
				}
			}

			return adObject;
		}

		private static ADObjectWrapper FindOUObject(IOguObject oguObject)
		{
			ADObjectWrapper adObject = null;
			string objectGuid;

			if (SynchronizeContext.Current.IDMapper.SCIDMappingDictionary.ContainsKey(oguObject.ID))
			{
				var idMapping = SynchronizeContext.Current.IDMapper.SCIDMappingDictionary[oguObject.ID];
				objectGuid = idMapping.ADObjectGuid;
				adObject = SynchronizeHelper.GetSearchResultByID(SynchronizeContext.Current.ADHelper, objectGuid, ADSchemaType.Organizations).ToADOjectWrapper();

				if (adObject == null)
				{
					//这里要删除ID映射
					if (!SynchronizeContext.Current.IDMapper.DeleteIDMappingDictionary.ContainsKey(oguObject.ID))
					{
						SynchronizeContext.Current.IDMapper.DeleteIDMappingDictionary.Add(idMapping);
					}

					SynchronizeContext.Current.IDMapper.SCIDMappingDictionary.Remove(c => c.SCObjectID == oguObject.ID);
				}
			}

			if (adObject == null)//通过ID没找到
			{
				string dn = SynchronizeHelper.GetOguObjectDN(oguObject);
				if (dn != "")
				{
					adObject = SynchronizeHelper.GetSearchResultByDN(SynchronizeContext.Current.ADHelper, dn, ADSchemaType.Organizations).ToADOjectWrapper();

					if (adObject != null)
					{
						//这里首先要判断是否已被映射过
						if (SynchronizeContext.Current.IDMapper.ADIDMappingDictionary.ContainsKey(adObject.NativeGuid))
						{
							adObject = null;
						}
					}
				}
				else
				{
					//using (DirectoryEntry root = SynchronizeContext.Current.ADHelper.GetRootEntry())
					//{
					//    adObject = new ADObjectWrapper() { ObjectType = ADSchemaType.Organizations };
					//    adObject.Properties["distinguishedName"] = root.Properties["distinguishedName"][0].ToString();
					//    adObject.Properties["objectGUID"] = root.NativeGuid;
					//    adObject.Properties["name"] = root.Name;
					//}
				}
			}

			return adObject;
		}

		private static ADObjectWrapper FindUserObject(IOguObject oguObject)
		{
			ADObjectWrapper adObject = null;
			string objectGuid;

			if (SynchronizeContext.Current.IDMapper.SCIDMappingDictionary.ContainsKey(oguObject.ID))
			{
				var idMapping = SynchronizeContext.Current.IDMapper.SCIDMappingDictionary[oguObject.ID];
				objectGuid = idMapping.ADObjectGuid;
				adObject = SynchronizeHelper.GetSearchResultByID(SynchronizeContext.Current.ADHelper, objectGuid, ADSchemaType.Users).ToADOjectWrapper();

				if (adObject == null)
				{
					//这里要删除ID映射
					if (!SynchronizeContext.Current.IDMapper.DeleteIDMappingDictionary.ContainsKey(oguObject.ID))
					{
						SynchronizeContext.Current.IDMapper.DeleteIDMappingDictionary.Add(idMapping);
					}

					SynchronizeContext.Current.IDMapper.SCIDMappingDictionary.Remove(c => c.SCObjectID == oguObject.ID);
				}
			}

			if (adObject == null)//通过ID没找到
			{
				adObject = SynchronizeHelper.GetSearchResultByLogonName(oguObject.Properties["LOGON_NAME"].ToString(), ADSchemaType.Users).ToADOjectWrapper();

				if (adObject != null)
				{
					//这里首先要判断是否已被映射过
					if (SynchronizeContext.Current.IDMapper.ADIDMappingDictionary.ContainsKey(adObject.NativeGuid))
					{
						adObject = null;
					}
				}
			}
			return adObject;
		}

		private static ADObjectWrapper FindGroupObject(IOguObject oguObject)
		{
			ADObjectWrapper adObject = null;
			string objectGuid;

			if (SynchronizeContext.Current.IDMapper.SCIDMappingDictionary.ContainsKey(oguObject.ID))
			{
				var idMapping = SynchronizeContext.Current.IDMapper.SCIDMappingDictionary[oguObject.ID];
				objectGuid = idMapping.ADObjectGuid;
				adObject = SynchronizeHelper.GetSearchResultByID(SynchronizeContext.Current.ADHelper, objectGuid, ADSchemaType.Groups).ToADOjectWrapper();

				if (adObject == null)
				{
					//这里要删除ID映射
					if (!SynchronizeContext.Current.IDMapper.DeleteIDMappingDictionary.ContainsKey(oguObject.ID))
					{
						SynchronizeContext.Current.IDMapper.DeleteIDMappingDictionary.Add(idMapping);
					}

					SynchronizeContext.Current.IDMapper.SCIDMappingDictionary.Remove(c => c.SCObjectID == oguObject.ID);
				}
			}

			if (adObject == null)//通过ID没找到
			{
				adObject = SynchronizeHelper.GetSearchResultByLogonName(oguObject.Properties["LOGON_NAME"].ToString(), ADSchemaType.Groups).ToADOjectWrapper();

				if (adObject != null)
				{
					//这里首先要判断是否已被映射过
					if (SynchronizeContext.Current.IDMapper.ADIDMappingDictionary.ContainsKey(adObject.NativeGuid))
					{
						adObject = null;
					}
				}
			}

			return adObject;
		}
	}
}