using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using System.Diagnostics;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	/// <summary>
	/// 数据源为权限中心，目的为AD的对象比较器
	/// </summary>
	public abstract class OguAndADObjectComparerBase : ObjectComparerBase<IOguObject, ADObjectWrapper>
	{
		public override ObjectModifyType Compare(IOguObject srcObject, ADObjectWrapper targetObject)
		{
			return CompareProperties(srcObject, targetObject);
		}

		protected ObjectModifyType CompareProperties(IOguObject srcObject, ADObjectWrapper targetObject)
		{
			SchemaMappingInfo mappingInfo = PermissionCenterToADSynchronizeSettings.GetConfig().SchemaMappings.GetSchemaMappingInfo(srcObject.ObjectType.ToString());

			ObjectModifyType result = ObjectModifyType.None;

			foreach (ComparerPropertyMappingConfigurationElement propertyComparerElement in mappingInfo.ComparedProperties)
			{
				if (propertyComparerElement.Comparer.Compare(srcObject, propertyComparerElement.Name, targetObject, propertyComparerElement.TargetPropertyName, propertyComparerElement.Context) == false)
				{
					result = ObjectModifyType.PropertyModified;
					Trace.Write("（修改的属性Key： " + propertyComparerElement.Name + "）");
					break;
				}
			}

			return result;
		}
	}
}