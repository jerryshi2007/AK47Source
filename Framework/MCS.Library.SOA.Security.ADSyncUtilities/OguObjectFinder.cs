using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class OguObjectFinder
	{
		public static IOguObject Find(ADObjectWrapper childADObject, IEnumerable<IOguObject> oguObjects)
		{
			IOguObject result = null;

			//这里仅仅是查找所给集合里是不是存在相应的对象
			foreach (var obj in oguObjects)
			{
				if (obj.ObjectType == SchemaType.Users)
				{
					if (((IUser)obj).IsSideline)
					{
						// continue;
					}
				}

				var guid = childADObject.NativeGuid;

				if (SynchronizeContext.Current.IDMapper.ADIDMappingDictionary.ContainsKey(guid)) //有映射
				{
					if (obj.ID == SynchronizeContext.Current.IDMapper.ADIDMappingDictionary[guid].SCObjectID)
					{
						result = obj;
					}
				}
				else
				{
					switch (childADObject.ObjectType)
					{
						case ADSchemaType.Organizations:
							if (SynchronizeHelper.AppendNamingContext(childADObject.DN) ==
								SynchronizeHelper.AppendNamingContext(SynchronizeHelper.GetOguObjectDN(obj)))
							{
								result = obj;
							}
							break;
						case ADSchemaType.Groups:
						case ADSchemaType.Users:
							if (childADObject.Properties["samAccountName"].ToString() == obj.Properties["LOGON_NAME"].ToString())
							{
								result = obj;
							}
							break;
					}
				}
			}

			return result;
		}
	}
}